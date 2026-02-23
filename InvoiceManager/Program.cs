using InvoiceManager.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInvoiceManagerSwagger()
    .AddInvoiceManagerDbContext(builder.Configuration)
    .AddInvoiceManagerIdentity()
    .AddInvoiceManagerAuthentication(builder.Configuration)
    .AddInvoiceManagerValidation()
    .AddInvoiceManagerAutoMapper()
    .AddInvoiceManagerServices()
    .AddInvoiceManagerControllers();

var app = builder.Build();

app.UseInvoiceManagerPipeline();

app.Run();