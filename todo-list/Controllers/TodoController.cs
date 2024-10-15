using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUserService _userService;

        public TodoController(IConfiguration configuration, ITodoService todoService, IUserService userService)
        {
            _configuration = configuration;
            _todoService = todoService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTasks()
        {
            var userIdResult = await GetUserIdAsync(); 

            if (userIdResult.Result is UnauthorizedResult || userIdResult.Result is NotFoundObjectResult)
            {
                return userIdResult.Result; 
            }

            var userId = userIdResult.Value; 

            var tasks = await _todoService.GetTasksByUserIdAsync(userId);

            if (tasks == null || !tasks.Any())
            {
                return Ok(new { message = "Nenhuma tarefa encontrada." });
            }

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
            var userIdResult = await GetUserIdAsync();

            if (userIdResult.Result is UnauthorizedResult || userIdResult.Result is NotFoundObjectResult)
            {
                return userIdResult.Result; 
            }

            if (todoItem == null)
            {
                return BadRequest();
            }

            todoItem.UserId = userIdResult.Value;

            var createdTask = await _todoService.CreateTaskAsync(todoItem);

            if (createdTask == null)
            {
                return StatusCode(500, new { message = "Falha ao criar a tarefa." });
            }

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

            return Ok(todoItem);
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

        private async Task<ActionResult<int>> GetUserIdAsync()
        {
            var usernameClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (usernameClaim == null)
            {
                return Unauthorized();
            }

            var userId = await _userService.GetUserIdByUsernameAsync(usernameClaim);

            if (userId == null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            return userId.Value;
        }
    }
}
