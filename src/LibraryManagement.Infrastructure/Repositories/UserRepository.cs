using Dapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using System.Data;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Users WHERE Id = @Id";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(User user)
        {
            var sql = @"INSERT INTO Users (Username, Email, PasswordHash, CreatedAt) 
                       VALUES (@Username, @Email, @PasswordHash, @CreatedAt);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _connection.ExecuteScalarAsync<int>(sql, user);
        }
    }
}
