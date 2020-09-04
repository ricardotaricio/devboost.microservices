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

        public async Task<IEnumerable<Drone>> ObterTodos()
        {
            return await _context.Drone.AsNoTracking().ToListAsync();
        }

        public async Task<Drone> ObterPorId(Guid id)
        {
            return await _context.Drone.FindAsync(id);
        }

        public async Task<Drone> ObterPorId(int id)
        {
            return await _context.Drone.FindAsync(id);
        }

        public async Task Adicionar(Drone drone)
        {
          await Task.Run(()=> _context.Drone.Add(drone));
        }

        public async Task<Drone> Atualizar(Drone drone)
        {
            var retorno = await Task.FromResult(_context.Drone.Update(drone));

            return retorno.Entity;
           
        }


        public void DisposeAsync()
        {
            _context.Dispose();
        }
    }
}
