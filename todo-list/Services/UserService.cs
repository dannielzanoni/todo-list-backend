using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using todo_list.Data;
using todo_list.Helpers;
using todo_list.Models;

namespace todo_list.Services
{
    public class UserService : IUserService
    {
        private readonly ToDoContext _context;

        public UserService(ToDoContext context)
        {
            _context = context;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null || user.PasswordHash != PasswordHasher.HashPassword(password))
            {
                return null;
            }
            return user;
        }

        public async Task Register(User user)
        {
            user.PasswordHash = PasswordHasher.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetByUsername(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<int?> GetUserIdByUsernameAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user?.Id;
        }
    }
}
