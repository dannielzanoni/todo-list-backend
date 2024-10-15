using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using todo_list.Controllers;
using todo_list.Models;
using todo_list.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace todo_list.tests
{
    public class TodoControllerTests
    {
        private readonly Mock<ITodoService> _mockTodoService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly TodoController _controller;

        public TodoControllerTests()
        {
            _mockTodoService = new Mock<ITodoService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new TodoController(null, _mockTodoService.Object, _mockUserService.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public async Task GetTasks_ReturnsOkResult_WhenTasksExist()
        {
            // Arrange
            var userId = 1;
            var tasks = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Task teste", Description = "Descricao" }
            };

            _mockUserService.Setup(service => service.GetUserIdByUsernameAsync("1"))
                .ReturnsAsync(userId);

            _mockTodoService.Setup(service => service.GetTasksByUserIdAsync(userId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnTasks = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Single(returnTasks);
        }

        [Fact]
        public async Task UpdateTask_ReturnsOkResult_WhenTaskIsUpdated()
        {
            // Arrange
            var userId = 1;
            var todoItem = new TodoItem { Id = 1, Title = "Task editada", Description = "Descricao editada", UserId = userId };
            _mockUserService.Setup(service => service.GetUserIdByUsernameAsync("1"))
                .ReturnsAsync(userId);
            _mockTodoService.Setup(service => service.UpdateTaskAsync(todoItem))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTask(todoItem.Id, todoItem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(todoItem, okResult.Value);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNoContent_WhenTaskIsDeleted()
        {
            // Arrange
            var userId = 1;
            var todoItemId = 1;
            _mockUserService.Setup(service => service.GetUserIdByUsernameAsync("1"))
                .ReturnsAsync(userId);
            _mockTodoService.Setup(service => service.DeleteTaskAsync(todoItemId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTask(todoItemId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}