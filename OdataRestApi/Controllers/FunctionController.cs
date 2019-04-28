using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdataRestApi.Controllers
{
    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Routing;
    using Microsoft.AspNetCore.Mvc;
    using static Microsoft.AspNetCore.Http.StatusCodes;

    /// <summary>
    /// Provides unbound, utility functions.
    /// </summary>
    [ApiVersionNeutral]
    public class FunctionsController : ODataController
    {
        [HttpGet]
        [ProducesResponseType(typeof(string), Status200OK)]
        [ODataRoute("GetTodoCreatorName(Id={id})")]
        public IActionResult GetSalesTaxRate(int id) => Ok("Prayash");
    }
}