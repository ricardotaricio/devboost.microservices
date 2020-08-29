using DevBoost.dronedelivery.Domain;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Repository.Context
{
    public class DroneItinerarioRepository : IDroneItinerarioRepository
    {
        private readonly DCDroneDelivery _context;

        public DroneItinerarioRepository(DCDroneDelivery context)
        {
            _context = context;
        }

        public async Task<bool> Delete(DroneItinerario droneItinerario)
        {
            _context.DroneItinerario.Remove(droneItinerario);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<DroneItinerario> GetById(Guid id)
        {
            return await _context.DroneItinerario.FindAsync(id);
        }

        public async Task<bool> Insert(DroneItinerario droneItinerario)
        {
            _context.DroneItinerario.Add(droneItinerario);
           return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IList<DroneItinerario>> GetAll()
        {
            return await _context.DroneItinerario
                .AsNoTracking()
                .Include(d => d.Drone)
                .ToListAsync();
        }

        public async Task<DroneItinerario> GetById(int id)
        {
            return await _context.DroneItinerario
                .AsNoTracking()
                .Include(d => d.Drone)
                .SingleOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DroneItinerario> Update(DroneItinerario droneItinerario)
        {
            _context.DroneItinerario.Update(droneItinerario);
            await _context.SaveChangesAsync();
            return droneItinerario;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<DroneItinerario> GetDroneItinerarioPorIdDrone(int id)
        {
            return await _context.DroneItinerario
                .AsNoTracking()
                .Include(d => d.Drone)
                .SingleOrDefaultAsync(d => d.DroneId == id);
        }
    }
}
