using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Repositories
{
    public class DroneRepository : IDroneRepository
    {
        private readonly DCDroneDelivery _context;

        public DroneRepository(DCDroneDelivery context)
        {
            this._context = context;
        }

        public async Task<bool> Delete(Drone drone)
        {
            _context.Drone.Remove(drone);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IList<Drone>> GetAll()
        {
            return await _context.Drone.AsNoTracking().ToListAsync();
        }

        public async Task<Drone> GetById(Guid id)
        {
            return await _context.Drone.FindAsync(id);
        }

        public async Task<Drone> GetById(int id)
        {
            return await _context.Drone.FindAsync(id);
        }

        public async Task<bool> Insert(Drone drone)
        {
            _context.Drone.Add(drone);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Drone> Update(Drone drone)
        {
            _context.Drone.Update(drone);
            await _context.SaveChangesAsync();
            return drone;
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
