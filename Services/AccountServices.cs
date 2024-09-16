using MongoDB.Bson;
using MongoDB.Driver;
using MyAPI.Models;

namespace MyAPI.Services
{
    public class AccountServices
    {
        private readonly IMongoCollection<Account> _collection; 

        public AccountServices(IMongoClient mongoClient, IConfiguration configuration)
        {
            var dataname = configuration["DatabaseSettings:DatabaseName"];  
            _collection = mongoClient.GetDatabase(dataname).GetCollection<Account>("Account");
        }

        #region GET: Retrieve a single account by Id
        public async Task<Account> GetAccountByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Invalid ID format", nameof(id));
            }

            return await _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
        }
        #endregion

        #region GET: Retrieve all accounts
        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _collection.Find(a => true).ToListAsync();
        }
        #endregion

        #region CREATE: Add a new account
        public async Task<Account> CreateAccountAsync(string username, string password)
        {

            var account = new Account
            {
                Username = username,
                Password = password,
                Id = ObjectId.GenerateNewId().ToString()
            };

            await _collection.InsertOneAsync(account);

            return account;
        }
        #endregion

        #region UPDATE: Update an existing account by Id
        public async Task<bool> UpdateAccountAsync(string id, Account updatedAccount)
        {
            if (string.IsNullOrEmpty(id) || updatedAccount == null)
            {
                throw new ArgumentException("Input value is null", nameof(id));
            }

            var result = await _collection.ReplaceOneAsync(a => a.Id == id, updatedAccount);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
        #endregion

        #region DELETE: Remove an account by Id
        public async Task<bool> DeleteAccountAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Input value is null", nameof(id));
            }

            var result = await _collection.DeleteOneAsync(a => a.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        #endregion

        #region GET: Retrieve an account by username and password
        public async Task<Account?> GetAccountByUsernameAndPasswordAsync(string username, string password)
        {
            return await _collection.Find(a => a.Username == username && a.Password == password).FirstOrDefaultAsync();
        }
        #endregion
    }
}
