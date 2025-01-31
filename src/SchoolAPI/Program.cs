using System.Reflection;
using CoreServices.CommonAPIConfiguration;
using CoreServices.ExceptionHandler;
using FluentValidation;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SchoolAPI.Business.Data;
using SchoolAPI.Business.Repository;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.Helper;
using Serilog;
using SchoolAPI.Business.Handlers;
using CoreServices.CustomHealthCheck;
using CoreServices.GenericRepository;
using SchoolAPI.Business.Models;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using SchoolAPI.Listener;
using SchoolAPI.Handlers;
using CoreServices.GenericRepository.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCommonServices(builder.Configuration,typeof(AutoMapperProfiles),xmlPath);
builder.Services.AddDbContextConfiguration<SchoolAPIDbContext>(builder.Configuration);
// builder.Services.AddDbContextReadWriteConfiguration<SchoolAPIDbContext>(builder.Configuration);

builder.Services.AddDbContextReadConfiguration<SchoolAPIReadDbContext>(builder.Configuration);
builder.Services.AddDbContextWriteConfiguration<SchoolAPIWriteDbContext>(builder.Configuration);

builder.AddSerilogLogging();

builder.Services.AddEmailService();
builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));
builder.Services.AddSingleton<Plain.RabbitMQ.IPublisher>( p => new Publisher(
    p.GetRequiredService<IConnectionProvider>(),
    "school.email",
    ExchangeType.Topic
));
builder.Services.AddSingleton<ISubscriber>(s => new Subscriber(
    s.GetService<IConnectionProvider>(),
    "school.email",
    "student.event.queue",
    "student.*",
    ExchangeType.Topic 
));
builder.Services.AddHostedService<StudentEventEmailListener>();

builder.Services.ConfigureHealthChecks(builder.Configuration);
builder.Services.AddHealthChecks().AddCheck<CustomHealthCheck>(nameof(CustomHealthCheck));
builder.Services.AddMediatR(typeof(GetAllStudentHandler).Assembly);

// builder.Services.ConfigureGenericRepository<Student,SchoolAPIDbContext>(builder.Configuration);
builder.Services.ConfigureGenericReadRepository<Student,SchoolAPIReadDbContext>(builder.Configuration);
builder.Services.ConfigureGenericWriteRepository<Student,SchoolAPIWriteDbContext>(builder.Configuration);
// builder.Services.AddScoped<IRepository<Student>, Repository<Student, SchoolAPIDbContext>>();
// builder.Services.AddScoped<IReadRepository<Student>, ReadRepository<Student, SchoolAPIReadDbContext>>();
// builder.Services.AddScoped<IWriteRepository<Student>, WriteRepository<Student,SchoolAPIWriteDbContext>>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = async (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestLog", true);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
    };
});

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
