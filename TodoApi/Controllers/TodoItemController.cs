using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Dtos;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/todolists/{listId}/items")]
public class TodoItemsController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoItemsController(TodoContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateItem(long listId, CreateTodoItem item)
    {
        var todoList = await _context.TodoLists.FindAsync(listId);
        if (todoList == null)
        {
            return NotFound();
        }

        var todoItem = new TodoItem
        {
            Description = item.Description,
            IsCompleted = false,
            TodoListId = listId
        };

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItem), new { listId = listId, id = todoItem.Id }, todoItem);
    }

    // GET: api/todolists/1/items/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetItem(int listId, int id)
    {
        var item = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.Id == id && i.TodoListId == listId);

        if (item == null) return NotFound();

        return item;
    }

    // PUT: api/todolists/1/items/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int listId, int id, [FromBody] UpdateTodoItem payload)
    {
        var item = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id == id && i.TodoListId == listId);
        if (item == null)
            return NotFound();

        item.Description = payload.Description;
        item.IsCompleted = payload.IsCompleted;

        await _context.SaveChangesAsync();

        return Ok(item);
    }


    // PATCH: api/todolists/1/items/5/complete
    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompleteItem(int listId, int id)
    {
        var item = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.Id == id && i.TodoListId == listId);

        if (item == null) return NotFound();

        item.IsCompleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/todolists/1/items/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int listId, int id)
    {
        var item = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.Id == id && i.TodoListId == listId);

        if (item == null) return NotFound();

        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
