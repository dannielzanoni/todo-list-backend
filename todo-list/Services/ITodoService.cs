using todo_list.Models;

namespace todo_list.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllTasksAsync();
        Task<TodoItem> GetTaskByIdAsync(int id);
        Task<TodoItem> CreateTaskAsync(TodoItem todoItem);
        Task<bool> UpdateTaskAsync(TodoItem todoItem);
        Task<bool> DeleteTaskAsync(int id);
    }
}
