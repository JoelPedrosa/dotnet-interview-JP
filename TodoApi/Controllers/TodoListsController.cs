using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/todolists")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoListsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/todolists
        [HttpGet]
        public async Task<ActionResult<IList<TodoList>>> GetTodoLists()
        {
            return Ok(await _context.TodoLists.ToListAsync());
        }

        // GET: api/todolists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoList>> GetTodoList(long id)
        {
            var todoList = await _context.TodoLists.FindAsync(id);

            if (todoList == null)
            {
                return NotFound();
            }

            return Ok(todoList);
        }

        // PUT: api/todolists/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoList(long id, UpdateTodoList payload)
        {
            var todoList = await _context.TodoLists.FindAsync(id);

            if (todoList == null)
            {
                return NotFound();
            }

            todoList.Name = payload.Name;
            await _context.SaveChangesAsync();

            return Ok(todoList);
        }

        // POST: api/todolists
        [HttpPost]
        public async Task<ActionResult<TodoList>> PostTodoList(CreateTodoList payload)
        {
            var todoList = new TodoList { Name = payload.Name };

            _context.TodoLists.Add(todoList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoList", new { id = todoList.Id }, todoList);
        }

        // DELETE: api/todolists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoList(long id)
        {
            var todoList = await _context.TodoLists.FindAsync(id);
            if (todoList == null)
            {
                return NotFound();
            }

            _context.TodoLists.Remove(todoList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoListExists(long id)
        {
            return (_context.TodoLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
