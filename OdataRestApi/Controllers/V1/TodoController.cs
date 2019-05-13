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
    [ODataRoutePrefix("TodoItem")]
    public class TodoController : ODataController
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ODataRoute]
        [EnableQuery(MaxTop = 100, AllowedQueryOptions = Select | Top | Skip | Count | Expand | OrderBy)]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<TodoItem>>), Status200OK)]
        public IActionResult GetTodoItems()
        {
            return Ok(_context.TodoItems.AsQueryable());
        }

        [HttpGet]
        [ODataRoute("({id})")]
        [EnableQuery(AllowedQueryOptions = Select | Count)]
        [ProducesResponseType(typeof(TodoItem), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public SingleResult<TodoItem> GetTodoItem([FromODataUri] long id)
        {
            var result = _context.TodoItems.Where(x => x.Id == id);
            return SingleResult.Create(result);
        }

        [HttpPost]
        [ODataRoute]
        [ProducesResponseType(typeof(TodoItem), Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<IActionResult> PostTodoItem([FromBody] TodoItem model)
        {
            _context.TodoItems.Add(model);
            await _context.SaveChangesAsync();
            return Created(model);
        }

        [HttpPut]
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

        [HttpPatch]
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

        [HttpDelete]
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

        [HttpGet]
        [ODataRoute(nameof(ApiVersion))]
        [ProducesResponseType(typeof(string), Status200OK)]
        public IActionResult ApiVersion()
        {
            var apiVersion = HttpContext.GetRequestedApiVersion();
            return Ok($"v{apiVersion.MajorVersion}.{apiVersion.MinorVersion}");
        }

        [HttpGet]
        [ODataRoute(nameof(FirstTodo))]
        [ProducesResponseType(typeof(TodoItem), Status200OK)]
        [EnableQuery(AllowedQueryOptions = Select)]
        public IActionResult FirstTodo()
        {
            return Ok(_context.TodoItems.First(x => x.Id > 0));
        }

        [HttpGet]
        [ODataRoute(nameof(ReturnSomeString))]
        [ProducesResponseType(typeof(string), Status200OK)]
        public IActionResult ReturnSomeString()
        {
            return Ok("Hello World returned from v1");
        }
    }
}