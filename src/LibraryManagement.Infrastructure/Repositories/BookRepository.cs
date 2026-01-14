using Dapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using System.Data;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly IDbConnection _connection;

        public BookRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Book> GetByIdAsync(int id, int userId)
        {
            var sql = "SELECT * FROM Books WHERE Id = @Id AND UserId = @UserId";
            return await _connection.QueryFirstOrDefaultAsync<Book>(sql, new { Id = id, UserId = userId });
        }

        public async Task<IEnumerable<Book>> GetAllAsync(int userId)
        {
            var sql = "SELECT * FROM Books WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            return await _connection.QueryAsync<Book>(sql, new { UserId = userId });
        }

        public async Task<int> CreateAsync(Book book)
        {
            var sql = @"INSERT INTO Books (Title, Author, ISBN, PublishedYear, UserId, CreatedAt) 
                       VALUES (@Title, @Author, @ISBN, @PublishedYear, @UserId, @CreatedAt);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _connection.ExecuteScalarAsync<int>(sql, book);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var sql = "DELETE FROM Books WHERE Id = @Id AND UserId = @UserId";
            var rowsAffected = await _connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
            return rowsAffected > 0;
        }
    }
}
