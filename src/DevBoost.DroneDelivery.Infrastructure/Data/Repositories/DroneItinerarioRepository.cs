using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Repositories
{
    public class DroneItinerarioRepository : IDroneItinerarioRepository
    {
        private readonly DCDroneDelivery _context;

        public DroneItinerarioRepository(DCDroneDelivery context)
        {
            _context = context;
        }



        public async Task<DroneItinerario> ObterPorId(Guid id)
        {
            return await _context.DroneItinerario.FindAsync(id);
        }

        public async Task Adicionar(DroneItinerario droneItinerario)
        {
            _context.Entry(droneItinerario.Drone).State = EntityState.Unchanged;

            _context.DroneItinerario.Add(droneItinerario);

        }

        public async Task<IEnumerable<DroneItinerario>> ObterTodos()
        {
            return await _context.DroneItinerario
                .AsNoTracking()
                .Include(d => d.Drone)
                .ToListAsync();
        }

        public async Task<DroneItinerario> ObterPorId(int id)
        {
            return await _context.DroneItinerario
                .AsNoTracking()
                .Include(d => d.Drone)
                .SingleOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DroneItinerario> Atualizar(DroneItinerario droneItinerario)
        {
            _context.Attach(droneItinerario).State = EntityState.Unchanged;
          var retorno =  await Task.Run(() => _context.DroneItinerario.Update(droneItinerario));

            return retorno.Entity;
        }

        public void DisposeAsync()
        {
            _context.Dispose();
        }

        public async Task<DroneItinerario> ObterDroneItinerarioPorIdDrone(int id)
        {
            return await _context.DroneItinerario
                .AsNoTracking()
                .Include(d => d.Drone)
                .FirstOrDefaultAsync(d => d.DroneId == id);
        }


    }
}
