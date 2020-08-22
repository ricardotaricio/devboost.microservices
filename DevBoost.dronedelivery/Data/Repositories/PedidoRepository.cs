using DevBoost.dronedelivery.Data.Contexts;
using DevBoost.dronedelivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Data.Repositories
{
    public class PedidoRepository : EFRepository<Pedido>, IPedidoRepository
    {
        private readonly PedidoContext _context;

        public PedidoRepository(PedidoContext context) : base(context)
        {
            this._context = context;
        }

    }
}
