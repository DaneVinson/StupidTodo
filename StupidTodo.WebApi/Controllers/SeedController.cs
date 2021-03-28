using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly TodosDbContext _context;

        public SeedController(TodosDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult> Seed()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            var source = new SimpleTodoDataProvider();
            var sourceTodos = new List<Todo>();
            sourceTodos.AddRange(await source.Get());
            sourceTodos.AddRange(await source.Get(true));

            _context.Todos.AddRange(sourceTodos);
            var count = await _context.SaveChangesAsync();

            return Ok(count);
        }
    }
}
