using Microsoft.EntityFrameworkCore;
using Openride.Data;
using Openride.Models;
using Openride.Repositories.Interfaces;

namespace Openride.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _db.Users.Include(u => u.Driver).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetByPhoneAsync(string phone) =>
            await _db.Users.FirstOrDefaultAsync(u => u.Phone == phone);

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _db.Users.Include(u => u.Driver).ToListAsync();

        public async Task<User> CreateAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
        }
    }
}   