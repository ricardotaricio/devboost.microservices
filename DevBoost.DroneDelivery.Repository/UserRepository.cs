using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DCDroneDelivery _context;

        public UserRepository(DCDroneDelivery context)
        {
            this._context = context;
        }

        public async Task<User> GetByUserName(string username)
        {
            return _context.User
                .Where(u => u.UserName == username)
                .Include(u => u.Cliente)
                .FirstOrDefault();
        }

        public async Task<User> GetByUserNameEPassword(string username, string password)
        {
            return await _context.User.AsNoTracking().Where(u => u.UserName == username && u.Password == password).FirstOrDefaultAsync();
        }

        public async Task<bool> Insert(User user)
        {
            _context.Entry(user.Cliente).State = EntityState.Unchanged;

            _context.User.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
