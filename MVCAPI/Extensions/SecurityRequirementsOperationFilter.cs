using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MVCAPI.Extensions
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check if the endpoint allows anonymous access
            var allowsAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .Any(m => m is IAllowAnonymous);

            // Check if request is authenticated (requires authorization)
            var requiresAuth = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .Any(m => m is IAuthorizeData);

            if (requiresAuth && !allowsAnonymous)
            {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
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
                            new List<string>()
                        }
                    }
                };
            }
        }
    }
}
