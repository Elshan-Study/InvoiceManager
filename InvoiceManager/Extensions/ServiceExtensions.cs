using FluentValidation;
using FluentValidation.AspNetCore;
using InvoiceManager.Data;
using InvoiceManager.Mappings;
using InvoiceManager.Middlewares;
using InvoiceManager.Models;
using InvoiceManager.Services;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace InvoiceManager.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInvoiceManagerControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }

    public static IServiceCollection AddInvoiceManagerDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<TaskFlowDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("TaskFlowDBConnetionString")));

        return services;
    }

    public static IServiceCollection AddInvoiceManagerIdentity(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        return services;
    }

    public static IServiceCollection AddInvoiceManagerServices(this IServiceCollection services)
    {
        services.AddSingleton<TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IInvoiceService, InvoiceService>();

        return services;
    }

    public static IServiceCollection AddInvoiceManagerAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        return services;
    }

    public static IServiceCollection AddInvoiceManagerValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddFluentValidationAutoValidation();
        return services;
    }

    public static IServiceCollection AddInvoiceManagerAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var jwtKey = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var key = Encoding.UTF8.GetBytes(jwtKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    public static IServiceCollection AddInvoiceManagerSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "InvoiceManager API",
                Version = "v1"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter JWT token with Bearer prefix",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}