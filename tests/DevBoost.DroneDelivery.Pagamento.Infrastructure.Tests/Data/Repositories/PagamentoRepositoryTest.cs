

using AutoBogus;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Pagamento.Domain.Entites;
using DevBoost.DroneDelivery.Pagamento.Infrastructure.Data.Contexts;
using DevBoost.DroneDelivery.Pagamento.Infrastructure.Data.Repositories;
using KellermanSoftware.CompareNetObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

namespace DevBoost.DroneDelivery.Pagamento.Infrastructure.Tests.Data.Repositories
{
    public class PagamentoRepositoryTest
    {
        [Fact(DisplayName = "GetById")]
        [Trait("PagamentoRepositoryTest", "Repository Tests")]
        public async void PagamentoRepository_BuscarPorId_()
        { 
      
            // Given
            var faker = AutoFaker.Create();
            var bus = faker.Generate<IMediatrHandler>();
            var options = new DbContextOptionsBuilder<PagamentoContext>()
           .UseInMemoryDatabase(databaseName: "DroneDelivery")
           .Options;

            var pagamentos = faker.Generate<PagamentoCartao>(3);

            using (var contexto = new PagamentoContext(options, bus))
            {
                contexto.PagamentoCartao.AddRange(pagamentos);
                contexto.SaveChanges();
            }

            var expectResponse = pagamentos.FirstOrDefault();

            using (var contexto = new PagamentoContext(options, bus))
            {
                PagamentoRepository pagamentoRepository = new PagamentoRepository(contexto);
                //When              
                var pagamento = await pagamentoRepository.ObterPorId(expectResponse.Id);

                //Then
                CompareLogic comparer = new CompareLogic();
                Assert.True(comparer.Compare(expectResponse.Id, pagamento.Id).AreEqual);
            }
        }

        [Fact(DisplayName = "InserirPagamentoComSucesso")]
        [Trait("PagamentoRepositoryTest", "Repository Tests")]
        public async void PagamentoRepository_InserirPagamento_ComSucesso()
        {
            //Given
            var faker = AutoFaker.Create();
            var bus = faker.Generate<IMediatrHandler>();
            var options = new DbContextOptionsBuilder<PagamentoContext>()
            .UseInMemoryDatabase(databaseName: "DroneDelivery")
            .Options;

            var pagamentoNovo = faker.Generate<PagamentoCartao>();

            using (var contexto = new PagamentoContext(options, bus))
            {//when
                var pagamentoRepository = new PagamentoRepository(contexto);
                await pagamentoRepository.Adicionar(pagamentoNovo);
                var insertPagamento = await pagamentoRepository.UnitOfWork.Commit();

                //then
                Assert.True(insertPagamento);
            }

        }


        [Fact(DisplayName = "AtualizarPagamentoComSucesso")]
        [Trait("PagamentoRepositoryTest", "Repository Tests")]
        public async void PagamentoRepository_AtualizarPagamento_ComSucesso()
        {
            //Given
            var faker = AutoFaker.Create();
            var bus = faker.Generate<IMediatrHandler>();
            var options = new DbContextOptionsBuilder<PagamentoContext>()
            .UseInMemoryDatabase(databaseName: "DroneDelivery")
            .Options;

            var pagamento = faker.Generate<PagamentoCartao>();

            //Seed
            using (var contexto = new PagamentoContext(options, bus))
            {
                contexto.PagamentoCartao.AddRange(pagamento);
                contexto.SaveChanges();
            }

            bool expectResponse;
            bool result;

            using (var contexto = new PagamentoContext(options, bus))
            {
                contexto.PagamentoCartao.Update(pagamento);
                expectResponse = contexto.SaveChanges() > 0;
            }

            using (var contexto = new PagamentoContext(options, bus))
            {
                var pagamentoRepository = new PagamentoRepository(contexto);
                await pagamentoRepository.Atualizar(pagamento);
                result = contexto.SaveChanges() > 0;

                var comparer = new CompareLogic();
                Assert.True(comparer.Compare(expectResponse, result).AreEqual);
            }

        }

    }
}
