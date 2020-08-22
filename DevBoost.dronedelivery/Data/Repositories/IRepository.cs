using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Data.Repositories
{
    public interface IRepository<T> where T: class
    {
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        T GetById(int id);
        void Insert(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}
