using DevBoost.DroneDelivery.Core.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Infrastructure.Data.Contexts;
using DevBoost.DroneDelivery.Pagamento.Domain.Entites;
using DevBoost.DroneDelivery.Pagamento.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Repositories
{
    public class PagamentoRepository : Repository<PagamentoCartao>, IPagamentoRepository
    {
        private readonly PagamentoContext _context;

        public PagamentoRepository(PagamentoContext context): base(context)
        {
            _context = context;
        }
    }
}
