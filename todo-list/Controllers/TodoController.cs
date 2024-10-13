using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using todo_list.Models;
using todo_list.Services;

namespace todo_list.Controllers
{
    [Route("api/todo")]
    public class TodoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITodoService _todoService;

        public TodoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTasks()
        {
            var tasks = await _todoService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTask(int id)
        {
            var task = await _todoService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TodoItem todoItem)
        {
            if (todoItem == null)
            {
                return BadRequest();
            }

            var createdTask = await _todoService.CreateTaskAsync(todoItem);
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            var updated = await _todoService.UpdateTaskAsync(todoItem);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _todoService.DeleteTaskAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
