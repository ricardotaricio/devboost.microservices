using DevBoost.DroneDelivery.Pagamento.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Infrastructure.Data.Mappings
{
    public class PagamentoCartaoMapping : IEntityTypeConfiguration<PagamentoCartao>
    {
        public void Configure(EntityTypeBuilder<PagamentoCartao> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PedidoId);
            builder.Property(p => p.Valor);
            builder.Property(p => p.Situacao);

            builder.Ignore(p => p.Cartao);

            builder.ToTable("Pagamento");
        }
    }
}
