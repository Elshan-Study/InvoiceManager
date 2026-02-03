using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;

namespace InvoiceManager.Common;

public class SwaggerExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null || context.Type == null)
            return;

        var properties = context.Type.GetProperties();

        foreach (var property in properties)
        {
            var defaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultValueAttribute == null)
                continue;

            var propertyName = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);

            if (!schema.Properties.ContainsKey(propertyName))
                continue;

            var value = defaultValueAttribute.Value;

            schema.Properties[propertyName].Example = value switch
            {
                string str => new OpenApiString(str),
                int i => new OpenApiInteger(i),
                long l => new OpenApiLong(l),
                bool b => new OpenApiBoolean(b),
                decimal d => new OpenApiDouble((double)d),
                double db => new OpenApiDouble(db),
                DateTime dt => new OpenApiString(dt.ToString("O")),
                DateTimeOffset dto => new OpenApiString(dto.ToString("O")),
                _ => new OpenApiString(value?.ToString() ?? string.Empty)
            };
        }
    }
}