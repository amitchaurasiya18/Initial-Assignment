using System.Reflection;
using CoreServices.CommonAPIConfiguration;
using CoreServices.ExceptionHandler;
using FluentValidation;
using HealthChecks.UI.Client;
using HealthChecks.UI.Configuration;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SchoolAPI.Business.Data;
using SchoolAPI.Business.Repository;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services;
using SchoolAPI.Business.Services.Interfaces;
using SchoolAPI.CustomHealthCheck;
using SchoolAPI.Helper;
using Serilog;
using SchoolAPI.Business.Handlers;

var builder = WebApplication.CreateBuilder(args);
var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

builder.Services.AddHttpClient();
builder.Services.AddCommonServices(builder.Configuration,typeof(AutoMapperProfiles),xmlPath);
builder.AddDbContextConfiguration<SchoolAPIDbContext>("SchoolDb");
builder.AddSerilogLogging();
builder.Services.ConfigureHealthChecks(builder.Configuration,"SchoolDb");
builder.Services.AddHealthChecks().AddCheck<CustomHealthCheck>(nameof(CustomHealthCheck));
builder.Services.AddMediatR(typeof(GetAllStudentHandler).Assembly);
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
