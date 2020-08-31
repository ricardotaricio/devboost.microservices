using DevBoost.DroneDelivery.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevBoost.DroneDelivery.Repository
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
