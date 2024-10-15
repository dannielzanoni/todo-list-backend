using todo_list.Models;

namespace todo_list.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task Register(User user);
        Task<User> GetByUsername(string username);
        Task<int?> GetUserIdByUsernameAsync(string username);
    }
}
