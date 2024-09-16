using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyAPI.Services
{
    public class UserServices
    {
        private readonly IMongoCollection<User> _collection;

        // Constructor to inject IConfiguration and set up MongoDB collection
        public UserServices(IMongoClient mongoClient, IConfiguration configuration)
        {
            var databaseName = configuration["DatabaseSettings:DatabaseName"];
            _collection = mongoClient.GetDatabase(databaseName).GetCollection<User>("User");
        }

        // GET: Retrieve a single user by Id
        public async Task<User?> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return await _collection.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        // GET: Retrieve all users
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        // CREATE: Add a new user
        public async Task<User> CreateUserAsync(string id)
        {
            
            var user = new User
            {
                Id = id,
                Name = "",
                Sotaikhoan ="",
                Balance =0
            }; 
            await _collection.InsertOneAsync(user);
            return user;
        }

        // UPDATE: Update an existing user by Id
        public async Task UpdateUserAsync(string id, User updatedUser)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var result = await _collection.ReplaceOneAsync(user => user.Id == id, updatedUser);
            if (!result.IsAcknowledged || result.MatchedCount == 0)
            {
                throw new KeyNotFoundException("User not found");
            }
        }

        // DELETE: Remove a user by Id
        public async Task DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var result = await _collection.DeleteOneAsync(user => user.Id == id);
            if (!result.IsAcknowledged || result.DeletedCount == 0)
            {
                throw new KeyNotFoundException("User not found");
            }
        }
    }
}
