using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
using System.Linq;

namespace OdataRestApi.Configuration
{
    public class SwaggerDefaultValues : IOperationFilter
    {
        public ApiVersionMapping Explicit { get; private set; }
        public ApiVersionMapping Implicit { get; private set; }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            // CurrentIssue: Swagger does not display the correct query parameters listed under EnableQueryAttribute in controller
            // this method fixes this issue temporarily
            UpdateOperationParameters(operation, context);

            var apiDescription = context.ApiDescription;
            var apiVersion = apiDescription.GetApiVersion();
            var model = apiDescription.ActionDescriptor?.GetApiVersionModel(Explicit | Implicit);

            operation.Deprecated = model.DeprecatedApiVersions.Contains(apiVersion);

            if (operation.Parameters == null)
            {
                return;
            }

            //// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
            //// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413

            foreach (var parameter in operation.Parameters.OfType<NonBodyParameter>())
            {
                var description = apiDescription.ParameterDescriptions
                                                .FirstOrDefault(p => p.Name == parameter.Name);

                if (description == null) break;
                //if (!apiDescription.ParameterDescriptions.Select(x => x.Name).Contains(parameter.Name)) break;

                if (parameter.Description == null)
                    parameter.Description = description.ModelMetadata?.Description;

                if (parameter.Default == null)
                    parameter.Default = description.DefaultValue;

                parameter.Required |= description.IsRequired;
            }
        }

        private void UpdateOperationParameters(Operation operation, OperationFilterContext context)
        {
            //Clear parameters list where name starts with '$'
            var parametersToRemove = operation.Parameters.Where(x => x.Name.StartsWith('$'));
            operation.Parameters = operation.Parameters.Where(s => !parametersToRemove.Any(p => p.Name == s.Name)).ToList();

            //Get Enable Query attribute for controller action
            var queryAttribute = context.MethodInfo.GetCustomAttributes(true)
                                        .Union(context.MethodInfo.DeclaringType.GetCustomAttributes(true))
                                        .OfType<EnableQueryAttribute>().FirstOrDefault();

            if (queryAttribute != null)
            {
                if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Select))
                {
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "$select",
                        In = "query",
                        Description = "Limits the properties returned in the result.",
                        Type = "string"
                    });
                }

                if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Expand))
                {
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "$expand",
                        In = "query",
                        Description = "Indicates the related entities to be represented inline. The maximum depth is 2.",
                        Type = "string"
                    });
                }

                if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Filter))
                {
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "$filter",
                        In = "query",
                        Description = "Restricts the set of items returned. The maximum number of expressions is 100. The allowed functions are: allfunctions.",
                        Type = "string"
                    });
                }

                if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.OrderBy))
                {
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "$orderby",
                        In = "query",
                        Description = "Specifies the order in which items are returned. The maximum number of expressions is 5.",
                        Type = "string"
                    });
                }

                if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Top))
                {
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "$top",
                        In = "query",
                        Description = "Limits the number of items returned from a collection. The maximum value is 100.",
                        Type = "integer"
                    });
                }

                if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Skip))
                {
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "$skip",
                        In = "query",
                        Description = "Excludes the specified number of items of the queried collection from the result.",
                        Type = "integer"
                    });
                }

                if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Count))
                {
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "$count",
                        In = "query",
                        Description = "Indicates whether the total count of items within a collection are returned in the result.",
                        Type = "boolean"
                    });
                }
            }
        }
    }
}