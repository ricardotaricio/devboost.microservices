using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Repositories
{
    public abstract class Repository
    {
        public void DetachLocal<T>(DbContext context, Func<T, bool> predicate) where T : class
        {
            var local = context.Set<T>().Local.Where(predicate).FirstOrDefault();

            if (local != null)
                context.Entry(local).State = EntityState.Detached;
        }
    }
}
