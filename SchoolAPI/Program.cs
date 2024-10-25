using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SchoolAPI.Business.Data;
using SchoolAPI.Helper;
using SchoolAPI.Business.Repository;
using SchoolAPI.Business.Repository.Interfaces;
using SchoolAPI.Business.Services;
using SchoolAPI.Business.Services.Interfaces;



var builder = WebApplication.CreateBuilder(args);
var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SchoolDb"));
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SchoolAPIDbContext>(
    options => options
    .UseMySql(builder.Configuration.GetConnectionString("SchoolDb"), serverVersion,
    b => b.MigrationsAssembly("SchoolAPI")));
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
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

// Configure the HTTP request pipeline.
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
