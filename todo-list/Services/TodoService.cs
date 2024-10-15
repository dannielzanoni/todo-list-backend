using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using todo_list.Data;
using todo_list.Models;

namespace todo_list.Services
{
    public class TodoService : ITodoService
    {
        private readonly ToDoContext _context;

        public TodoService(ToDoContext context)
        {
            _context = context;
        }

        public async Task<TodoItem> CreateTaskAsync(TodoItem todoItem)
        {
            if (todoItem != null)
            {
                await _context.TodoItems.AddAsync(todoItem);
                await _context.SaveChangesAsync();

                return todoItem;
            }

            return null;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var taskToDelete = await _context.TodoItems.FindAsync(id);
            if (taskToDelete == null)
            {
                return false;
            }

            _context.TodoItems.Remove(taskToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TodoItem>> GetAllTasksAsync()
        {
            var tasks = await _context.TodoItems.ToListAsync();
            return tasks;
        }

        public async Task<TodoItem> GetTaskByIdAsync(int id)
        {
            return await _context.TodoItems.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> UpdateTaskAsync(TodoItem todoItem)
        {
            var taskToUpdate = await _context.TodoItems.FindAsync(todoItem.Id);
            if (taskToUpdate == null)
            {
                return false;
            }

            taskToUpdate.Title = todoItem.Title;
            taskToUpdate.Description = todoItem.Description;
            taskToUpdate.IsFinished = todoItem.IsFinished;
            taskToUpdate.UserId = todoItem.UserId;

            _context.TodoItems.Update(taskToUpdate);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TodoItem>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.TodoItems
                .Where(task => task.UserId == userId)
                .ToListAsync();
        }
    }
}
