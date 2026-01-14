using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task<int> CreateAsync(User user);
    }
}
