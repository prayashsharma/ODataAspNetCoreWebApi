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

namespace OdataRestApi.Controllers.V2
{
    [ApiVersion("2.0")]
    [ODataRoutePrefix("TodoItem")]
    public class TodoController : ODataController
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        [ODataRoute]
        [EnableQuery(MaxTop = 100, AllowedQueryOptions = Select | Top | Skip | Count | Expand | OrderBy)]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<TodoItem>>), Status200OK)]
        public IActionResult GetTodoItems()
        {
            return Ok(_context.TodoItems.AsQueryable());
        }

        [ODataRoute("({id})")]
        [EnableQuery(AllowedQueryOptions = All)]
        [ProducesResponseType(typeof(TodoItem), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public SingleResult<TodoItem> GetTodoItem([FromODataUri] long id)
        {
            var result = _context.TodoItems.Where(x => x.Id == id);
            return SingleResult.Create(result);
        }

        [ODataRoute]
        [ProducesResponseType(typeof(TodoItem), Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<IActionResult> PostTodoItem([FromBody] TodoItem model)
        {
            _context.TodoItems.Add(model);
            await _context.SaveChangesAsync();
            return Created(model);
        }

        [ODataRoute("({id})")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<IActionResult> PutTodoItem([FromODataUri] long id, [FromBody] TodoItem model)
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
        [ProducesResponseType(typeof(TodoItem), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> PatchTodoItem([FromODataUri] long id, Delta<TodoItem> entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = await _context.TodoItems.FindAsync(id);

            entity.Patch(model);
            await _context.SaveChangesAsync();
            return Updated(model);
        }

        [ODataRoute("({id})")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> DeleteTodoItem([FromODataUri] long id)
        {
            var model = await _context.TodoItems.FindAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}