using System.Text;
using CoreServices.CustomExceptions;
using CoreServices.DTO;
using CoreServices.ExceptionHandler;
using CoreServices.Filters;
using CoreServices.GenericRepository;
using CoreServices.GenericRepository.Interfaces;
using CoreServices.StaticFiles;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolAPI.Business.Services;
using SchoolAPI.Business.Services.Interfaces;
using Serilog;
using Serilog.Filters;

namespace CoreServices.CommonAPIConfiguration
{
    public static class ApiConfiguration
    {

        private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var responseObj = new
            {
                statusCode,
                message
            };
            await context.Response.WriteAsJsonAsync(responseObj);
        }

        public static void AddCommonServices(this IServiceCollection services, IConfiguration configuration, Type autoMapperProfileType, string xmlPath)
        {

            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            services.AddControllers(options => options.Filters.Add<ModelValidationFilter>());
            services.AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);
            services.AddAutoMapper(autoMapperProfileType.Assembly);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidIssuer = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                        ValidateIssuerSigningKey = true,
                        RoleClaimType = "Role"
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = async context =>
                        {
                            var httpContext = context.HttpContext;
                            var exception = context.Exception;
                            var customExceptionHandler = new CustomExceptionHandler();

                            if (exception is SecurityTokenExpiredException)
                            {
                                await WriteResponseAsync(httpContext, StatusCodes.Status401Unauthorized, TokenValidationMessages.TOKEN_EXPIRED);
                                throw new Unauthorized(TokenValidationMessages.TOKEN_EXPIRED);
                            }
                            else if (exception is SecurityTokenInvalidSignatureException)
                            {
                                await WriteResponseAsync(httpContext, StatusCodes.Status401Unauthorized, TokenValidationMessages.INVALID_SIGNATURE);
                                throw new InvalidSignature(TokenValidationMessages.INVALID_SIGNATURE);
                            }
                            else if (exception is SecurityTokenMalformedException)
                            {
                                await WriteResponseAsync(httpContext, StatusCodes.Status400BadRequest, TokenValidationMessages.INVALID_FORMAT);
                                throw new InvalidFormat(TokenValidationMessages.INVALID_FORMAT);
                            }
                            else
                            {
                                await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, TokenValidationMessages.INTERNAL_SERVER_ERROR);
                                throw new Exception(TokenValidationMessages.INTERNAL_SERVER_ERROR);
                            }
                        },

                        OnChallenge = context =>
                        {
                            throw new Unauthorized(TokenValidationMessages.TOKEN_VALIDATION_FAILED);
                        },

                        OnForbidden = context =>
                        {
                            throw new AccessForbidden(TokenValidationMessages.ACCESS_FORBIDDEN);
                        },

                        OnTokenValidated = context =>
                        {
                            return Task.CompletedTask;
                        },
                    };
                });

            services.AddSwaggerGen(opt =>
            {
                var apiTitle = configuration["Swagger:Title"] ?? "API";
                var apiVersion = configuration["Swagger:Version"] ?? "v1";
                opt.SwaggerDoc(apiVersion, new OpenApiInfo { Title = apiTitle, Version = apiVersion });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                opt.IncludeXmlComments(xmlPath);
            });
        }

        public static void AddSerilogLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .WriteTo.Logger(lg => lg
                        .Filter.ByIncludingOnly(Matching.WithProperty("RequestLog"))
                        .WriteTo.File("Logs/api_requests.log", rollingInterval: RollingInterval.Day)
                    )
                    .WriteTo.Logger(lg => lg
                        .Filter.ByIncludingOnly(evt => evt.Exception != null && !evt.Exception.Data.Contains("HandledByCustomHandler"))
                        .WriteTo.File("Logs/exceptions.log", rollingInterval: RollingInterval.Day)
                    )
                    .WriteTo.Console();
            });
        }

        public static void AddDbContextConfiguration<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            var connectionString = "MasterDB";
            var serverVersion = ServerVersion.AutoDetect(configuration.GetConnectionString(connectionString));
            services.AddDbContext<TContext>(options =>
                options.UseMySql(configuration.GetConnectionString(connectionString), serverVersion));
        }

        public static void AddDbContextReadConfiguration<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            var connectionString = "SlaveDB";
            var serverVersion = ServerVersion.AutoDetect(configuration.GetConnectionString(connectionString));
            services.AddDbContext<TContext>(options =>
                options.UseMySql(configuration.GetConnectionString(connectionString), serverVersion));
        }

        public static void AddDbContextWriteConfiguration<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            var connectionString = "MasterDB";
            var serverVersion = ServerVersion.AutoDetect(configuration.GetConnectionString(connectionString));
            services.AddDbContext<TContext>(options =>
                options.UseMySql(configuration.GetConnectionString(connectionString), serverVersion));
        }

        public static void AddDbContextReadWriteConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : DbContext
        {
            string? connectionString;
            services.AddDbContext<T>((serviceProvider, options) =>
            {
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                if (httpContextAccessor.HttpContext?.Request.Method == HttpMethods.Get)
                {
                    connectionString = configuration.GetConnectionString("SlaveDB");
                }
                else
                {
                    connectionString = configuration.GetConnectionString("MasterDB");
                }
                var serverVersion = ServerVersion.AutoDetect(connectionString);
                options.UseMySql(connectionString, serverVersion)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
            });
        }

        public static void AddDockerDbContextReadWriteConfiguration<T>(this IServiceCollection services, IConfiguration configuration) where T : DbContext
        {
            string? connectionString;
            var masterConnectionString = Environment.GetEnvironmentVariable("MYSQL_MASTER_CONNECTION_STRING");
            var slaveConnectionString = Environment.GetEnvironmentVariable("MYSQL_SLAVE_CONNECTION_STRING");
            services.AddDbContext<T>((serviceProvider, options) =>
            {
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                if (httpContextAccessor.HttpContext?.Request.Method == HttpMethods.Get)
                {
                    connectionString = slaveConnectionString;
                }
                else
                {
                    connectionString = slaveConnectionString;
                }
                var serverVersion = ServerVersion.AutoDetect(connectionString);
                options.UseMySql(connectionString, serverVersion)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging();
            });
        }

        public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
            .AddMySql(configuration.GetConnectionString("SlaveDB"));
        }

        // public static void ConfigureGenericRepository<T, TContext>(this IServiceCollection services, IConfiguration configuration) 
        // where T : class
        // where TContext : DbContext 
        // {
        //     services.AddScoped(typeof(IRepository<T>), typeof(Repository<T,TContext>));
        // }
        
        public static void ConfigureGenericReadRepository<T, TContext>(this IServiceCollection services, IConfiguration configuration) 
        where T : class
        where TContext : DbContext 
        {
            services.AddScoped(typeof(IReadRepository<T>), typeof(ReadRepository<T,TContext>));
        }

        public static void ConfigureGenericWriteRepository<T, TContext>(this IServiceCollection services, IConfiguration configuration) 
        where T : class
        where TContext : DbContext 
        {
            services.AddScoped(typeof(IWriteRepository<T>), typeof(WriteRepository<T,TContext>));
        }

        public static void AddEmailService(this IServiceCollection services)
        {
            services.AddSingleton<IEmailService, EmailService>();
        }
    }
}