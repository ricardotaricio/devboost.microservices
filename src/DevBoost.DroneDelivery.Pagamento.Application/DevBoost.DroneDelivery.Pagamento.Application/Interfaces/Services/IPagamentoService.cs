using DevBoost.DroneDelivery.Pagamento.Application.ViewModels;
using DevBoost.DroneDelivery.Pagamento.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Pagamento.Application.Interfaces.Services
{
    public interface IPagamentoService
    {
        Task<bool> Processar(AdicionarPagamentoCartaoViewModel adicionarPagamentoCartaoViewModel);
    }
}
