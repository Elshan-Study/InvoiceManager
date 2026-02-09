using FluentValidation;
using FluentValidation.AspNetCore;
using InvoiceManager.Common;
using InvoiceManager.Data;
using InvoiceManager.Mappings;
using InvoiceManager.Middlewares;
using InvoiceManager.Services;
using InvoiceManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => //add
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); 

builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "InvoiceManager API",
            Description = "API for invoice manager. This API provides a full set of CRUD operations for working with invoices.",
            Contact = new OpenApiContact
            {
                Name = "Elshan Team",
                Email = "support@elshan.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT Licence",
                Url = new Uri("https://opensource.org/license/mit")
            }
        });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
        options.SchemaFilter<SwaggerExampleSchemaFilter>();
    }
    );

var connectionString = builder
    .Configuration
    .GetConnectionString("TaskFlowDBConnetionString");

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<TaskFlowDbContext>(
    options =>
        options.UseSqlServer(connectionString)
    );

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskFlow API v1");
            options.RoutePrefix = string.Empty;
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnableFilter();
            options.EnableTryItOutByDefault();
        }
        );
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
