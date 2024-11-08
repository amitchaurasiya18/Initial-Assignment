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
using SchoolAPI.DTO;
using Microsoft.AspNetCore.Mvc.Filters;

var builder = WebApplication.CreateBuilder(args);
var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SchoolDb"));


builder.Services.AddScoped<ModelValidationFilter>();
builder.Services.AddControllers(options => options.Filters.Add<ModelValidationFilter>());
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SchoolAPIDbContext>(
    options => options
    .UseMySql(builder.Configuration.GetConnectionString("SchoolDb"), serverVersion));

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);
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
app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
