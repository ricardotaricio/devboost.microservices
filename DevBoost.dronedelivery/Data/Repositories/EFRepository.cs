using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Data.Repositories
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        internal DbSet<T> _dbset;

        public EFRepository(DbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();
        }

        public void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbset.AsQueryable().ToList();
        }

        public T GetById(int id)
        {
            return _dbset.Find(id);
        }

        public T GetById(Guid id)
        {
            return _dbset.Find(id);
        }

        public void Insert(T entity)
        {
            _dbset.Add(entity);
        }

        public T Update(T entity)
        {
            _dbset.Update(entity);

            return entity;
        }
    }
}
