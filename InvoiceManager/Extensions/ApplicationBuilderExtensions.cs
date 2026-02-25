using InvoiceManager.Middlewares;

namespace InvoiceManager.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseInvoiceManagerPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}