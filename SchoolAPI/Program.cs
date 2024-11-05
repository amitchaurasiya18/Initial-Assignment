using Microsoft.EntityFrameworkCore;
using SchoolAPI.Business.Data;
using SchoolAPI.Helper;
using SchoolAPI.Business.Repository;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services;
using SchoolAPI.Business.Services.Interfaces;
using FluentValidation;
using SchoolAPI.ExceptionHandler;
using FluentValidation.AspNetCore;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SchoolAPI.DTO;

var builder = WebApplication.CreateBuilder(args);
var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SchoolDb"));
// Add services to the container.

// builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "This field is required.");
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToArray()
            );

        var errorDetails = new ErrorDetails
        {
            Message = "One or more validation errors occurred.",
            StatusCode = (int)HttpStatusCode.BadRequest,
            ExceptionMessage = "Validation failed.",
            Errors = errors
        };

        return new BadRequestObjectResult(errorDetails);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SchoolAPIDbContext>(
    options => options
    .UseMySql(builder.Configuration.GetConnectionString("SchoolDb"), serverVersion));

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddExceptionHandler<AppKeyNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<AppInternalServerErrorExceptionHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseExceptionHandler(_ => { });
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
