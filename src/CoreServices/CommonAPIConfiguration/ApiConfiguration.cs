using System.Text;
using CoreServices.CustomExceptions;
using CoreServices.DTO;
using CoreServices.ExceptionHandler;
using CoreServices.Filters;
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

        public static void AddDbContextConfiguration<TContext>(this WebApplicationBuilder builder, string connectionString) where TContext : DbContext
        {
            var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString(connectionString));
            builder.Services.AddDbContext<TContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString(connectionString), serverVersion));
        }

        public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration, string connectionString)
        {
            services.AddHealthChecks()
            .AddMySql(configuration.GetConnectionString(connectionString));
                

            // services.AddHealthChecksUI(opt =>
            // {
            //     opt.SetEvaluationTimeInSeconds(10);
            //     opt.MaximumHistoryEntriesPerEndpoint(60);   
            //     opt.SetApiMaxActiveRequests(1);   
            //     opt.AddHealthCheckEndpoint("feedback api", "http://schoolapi:5206/api/health");

            // }).AddInMemoryStorage();
        }
    }
}