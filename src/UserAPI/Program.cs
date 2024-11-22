using System.Reflection;
using CoreServices.CommonAPIConfiguration;
using CoreServices.CustomHealthCheck;
using CoreServices.ExceptionHandler;
using FluentValidation;
using Serilog;
using UserAPI.Business.Data;
using UserAPI.Business.Repository;
using UserAPI.Business.Repository.Interfaces;
using UserAPI.Business.Services;
using UserAPI.Business.Services.Interfaces;
using UserAPI.Helper;

var builder = WebApplication.CreateBuilder(args);
var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

builder.Services.AddHttpClient();
builder.Services.AddCommonServices(builder.Configuration,typeof(AutoMapperProfile),xmlPath);
builder.AddDbContextConfiguration<UserAPIDbContext>("SchoolUserDb");
builder.AddSerilogLogging();
builder.Services.ConfigureHealthChecks(builder.Configuration,"SchoolUserDb");
builder.Services.AddHealthChecks().AddCheck<CustomHealthCheck>(nameof(CustomHealthCheck));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
