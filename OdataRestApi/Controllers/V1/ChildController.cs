using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OdataRestApi.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;

namespace OdataRestApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [ODataRoutePrefix("ChildItem")]
    public class ChildController : ODataController
    {
        private readonly TodoContext _context;

        public ChildController(TodoContext context)
        {
            _context = context;
        }

        [ODataRoute]
        [EnableQuery(MaxTop = 100, AllowedQueryOptions = Select | Top | Skip | Count)]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<ChildItem>>), Status200OK)]
        public IActionResult GetChildItems()
        {
            return Ok(_context.ChildItems.AsQueryable());
        }

        [ODataRoute("({id})")]
        [EnableQuery(AllowedQueryOptions = Select)]
        [ProducesResponseType(typeof(ChildItem), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public SingleResult<ChildItem> GetChildItem([FromODataUri] int id)
        {
            var result = _context.ChildItems.Where(x => x.Id == id);

            return SingleResult.Create(result);
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ChildItem), Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<IActionResult> PostChildItem([FromBody] ChildItem model)
        {
            _context.ChildItems.Add(model);
            await _context.SaveChangesAsync();

            return Created(model);
        }

        [ODataRoute("({id})")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<IActionResult> PutChildItem([FromODataUri] int id, [FromBody] ChildItem model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [ODataRoute("({id})")]
        [ProducesResponseType(typeof(ChildItem), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> PatchChildItem([FromODataUri] int id, Delta<ChildItem> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = await _context.ChildItems.FindAsync(id);

            delta.Patch(model);
            await _context.SaveChangesAsync();
            return Updated(model);
        }

        [ODataRoute("({id})")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> DeleteChildItem([FromODataUri] int id)
        {
            var model = await _context.ChildItems.FindAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            _context.ChildItems.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}