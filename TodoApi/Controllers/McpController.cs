using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TodoApi.Models;
using TodoApi.Dtos;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("mcp")]
    public class McpController : ControllerBase
    {
        private readonly TodoContext _context;

        public McpController(TodoContext context)
        {
            _context = context;
        }

        // GET /mcp/tools
        [HttpGet("tools")]
        public IActionResult GetTools()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "tools.json");

            if (!System.IO.File.Exists(jsonPath))
                return NotFound("tools.json not found.");

            var json = System.IO.File.ReadAllText(jsonPath);
            return Content(json, "application/json");
        }

        // POST /mcp/invoke
        [HttpPost("invoke")]
        public async Task<IActionResult> Invoke([FromBody] InvokePrompt prompt)
        {
            var text = prompt.Prompt.ToLower();

            // CREATE ITEM
            if (text.StartsWith("create an item in the list"))
            {
                var match = Regex.Match(text, @"create an item in the list '(.+?)' with the description '(.+?)'");
                if (!match.Success)
                    return BadRequest("Incorrect prompt format.");

                var listName = match.Groups[1].Value;
                var description = match.Groups[2].Value;

                var todoList = await _context.TodoLists.FirstOrDefaultAsync(l => l.Name.ToLower() == listName.ToLower());
                if (todoList == null)
                    return NotFound($"List '{listName}' was not found.");

                var newItem = new TodoItem
                {
                    TodoListId = todoList.Id,
                    Description = description,
                    IsCompleted = false
                };

                _context.TodoItems.Add(newItem);
                await _context.SaveChangesAsync();
                return Ok(newItem);
            }

            // UPDATE ITEM
            if (text.StartsWith("update item"))
            {
                var match = Regex.Match(text, @"update item (\d+) in list '(.+?)' with description '(.+?)'");
                if (!match.Success)
                    return BadRequest("Incorrect prompt format.");

                int itemId = int.Parse(match.Groups[1].Value);
                string listName = match.Groups[2].Value;
                string description = match.Groups[3].Value;

                var todoList = await _context.TodoLists.FirstOrDefaultAsync(l => l.Name.ToLower() == listName.ToLower());
                if (todoList == null)
                    return NotFound($"List '{listName}' not found.");

                var item = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id == itemId && i.TodoListId == todoList.Id);
                if (item == null)
                    return NotFound($"Item {itemId} not found in list '{listName}'.");

                item.Description = description;
                await _context.SaveChangesAsync();
                return Ok(item);
            }

            // DELETE ITEM
            if (text.StartsWith("delete item"))
            {
                var match = Regex.Match(text, @"delete item (\d+) from list '(.+?)'");
                if (!match.Success)
                    return BadRequest("Incorrect prompt format.");

                int itemId = int.Parse(match.Groups[1].Value);
                string listName = match.Groups[2].Value;

                var todoList = await _context.TodoLists.FirstOrDefaultAsync(l => l.Name.ToLower() == listName.ToLower());
                if (todoList == null)
                    return NotFound($"List '{listName}' not found.");

                var item = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id == itemId && i.TodoListId == todoList.Id);
                if (item == null)
                    return NotFound($"Item {itemId} not found in list '{listName}'.");

                _context.TodoItems.Remove(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }

            // COMPLETE ITEM
            if (text.StartsWith("mark item"))
            {
                var match = Regex.Match(text, @"mark item (\d+) as completed in list '(.+?)'");
                if (!match.Success)
                    return BadRequest("Incorrect prompt format.");

                int itemId = int.Parse(match.Groups[1].Value);
                string listName = match.Groups[2].Value;

                var todoList = await _context.TodoLists.FirstOrDefaultAsync(l => l.Name.ToLower() == listName.ToLower());
                if (todoList == null)
                    return NotFound($"List '{listName}' not found.");

                var item = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id == itemId && i.TodoListId == todoList.Id);
                if (item == null)
                    return NotFound($"Item {itemId} not found in list '{listName}'.");

                item.IsCompleted = true;
                await _context.SaveChangesAsync();
                return Ok(item);
            }

            return BadRequest("Prompt not recognized.");
        }
    }
}    