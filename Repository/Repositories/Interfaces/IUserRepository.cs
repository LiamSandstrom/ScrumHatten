using Models;

namespace DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {

        Task<List<User>> GetAllUsersAsync();

        Task<bool> UpdateUserAsync(User user);

        Task<bool> DeleteUserAsync(Guid id);

        Task<User> AddUserAsync(User user);

    }
}
