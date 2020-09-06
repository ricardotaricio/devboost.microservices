﻿using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Pagamento.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Contexts
{
    [ExcludeFromCodeCoverage]
    public class PagamentoContext : BaseDbContext
    {
        public PagamentoContext(DbContextOptions options, IMediatrHandler bus) : base(options, bus)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DroneDeliveryPagamento;Trusted_Connection=true;")
                    .UseLazyLoadingProxies(false);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PagamentoContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) 
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<PagamentoCartao> PagamentoCartao { get; set; }
    }
}
