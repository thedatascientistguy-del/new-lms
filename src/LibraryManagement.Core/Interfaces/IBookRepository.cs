using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> GetByIdAsync(int id, int userId);
        Task<IEnumerable<Book>> GetAllAsync(int userId);
        Task<int> CreateAsync(Book book);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
