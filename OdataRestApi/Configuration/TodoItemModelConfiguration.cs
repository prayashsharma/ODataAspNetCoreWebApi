using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using OdataRestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdataRestApi.Configuration
{
    public class TodoItemModelConfiguration : IModelConfiguration
    {
        /// <summary>
        /// Applies model configurations using the provided builder for the specified API version.
        /// </summary>
        /// <param name="builder">The <see cref="ODataModelBuilder">builder</see> used to apply configurations.</param>
        /// <param name="apiVersion">The <see cref="ApiVersion">API version</see> associated with the <paramref name="builder"/>.</param>
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
        {
            var todo = builder.EntitySet<TodoItem>("TodoItem")
                .EntityType.HasKey(o => o.Id);

            if (apiVersion == ApiVersions.V1)
            {
                todo.Collection.Function("FirstTodo").ReturnsFromEntitySet<TodoItem>("TodoItem");
                todo.Collection.Function("ReturnSomeString").Returns<string>();
            }

            todo.Collection.Function("ApiVersion").Returns<string>();
        }
    }
}