using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Contexts
{
    public class BaseDbContext : DbContext, IUnitOfWork
    {

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public async Task<bool> Commit()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
