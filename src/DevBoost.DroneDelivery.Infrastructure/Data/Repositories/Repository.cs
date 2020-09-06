﻿using DevBoost.DroneDelivery.Core.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly BaseDbContext _context;
        private readonly DbSet<T> _repo;

        protected Repository(BaseDbContext context)
        {
            _context = context;
            _repo = _context.Set<T>();

        }
        public async Task Adicionar(T entity)
        {
            _repo.Add(entity);
        }

        public async Task Atualizar(T entity)
        {
            _repo.Update(entity);
        }

        //public void Dispose()
        //{
        //    //if (_context != null)
        //    //    _context.Dispose();

        //    //GC.SuppressFinalize(this);
        //}

        public async Task<T> ObterPorId(Guid id)
        {
            return await _repo.FindAsync(id);
        }
        public async Task<T> ObterPorId(int id)
        {
            return await _repo.FindAsync(id);
        }

        public async Task<IEnumerable<T>> ObterTodos()
        {
            return await _repo.ToListAsync();
        }

        public async Task<IEnumerable<T>> ObterPor(Expression<Func<T, bool>> predicate)
        {
            return await _repo.Where(predicate).AsNoTracking().ToListAsync();
        }
    }
}
