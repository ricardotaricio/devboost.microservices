using DevBoost.dronedelivery.Data.Repositories;
using DevBoost.dronedelivery.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.dronedelivery.Service
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;

        private const double _latitudeLoja = -23.5880684;
        private const double _longitudeLoja = -46.6564195;

        public DeliveryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool IsPedidoValido(Pedido pedido, out double tempoTrajetoCompleto)
        {
            tempoTrajetoCompleto = 0;
            
            // existe drone com capacidade maior que o peso do pedido (limite maximo 12kg)
            Drone drone = _unitOfWork.Drones.GetAll().Where(d => d.Capacidade >= pedido.Peso).FirstOrDefault();

            if (drone == null)
                // return BadRequest("Rejeitado: Pedido acima do peso máximo aceito.");
                return false;

            // calcular distancia do trajeto
            // calcular tempo total (ida e volta) do trajeto (limite maximo 35m)
            // existe um drone que atende essas condicoes
            double distancia = calcularDistanciaEmKilometros(_latitudeLoja, _longitudeLoja, (double)pedido.Latitude, (double)pedido.Longitude);
            distancia = distancia * 2;

            // tempo = distancia / velocidade
            // 80km / 40km/h = 2h
            tempoTrajetoCompleto = calcularTempoTrajetoEmMinutos(distancia, drone.Velocidade);

            if (tempoTrajetoCompleto > drone.Autonomia)
                //return BadRequest("Rejeitado: Fora da área de entrega.");
                return false;

            return true;
        }

        private double calcularDistanciaEmKilometros(double latitudeOrigem, double longitudeOrigem, double latitudeDestino, double longitudeDestino)
        {
            var origemCoord = new GeoCoordinate(latitudeOrigem, longitudeOrigem);
            var destinoCoord = new GeoCoordinate(latitudeDestino, longitudeDestino);

            var distance = origemCoord.GetDistanceTo(destinoCoord);

            distance = distance / 1000;

            return distance;
        }

        private int calcularTempoTrajetoEmMinutos(double distanciaEmKilometros, int velocidadeEmKilometrosPorHora)
        {
            double tempo = distanciaEmKilometros / velocidadeEmKilometrosPorHora;

            tempo = tempo * 60;

            return Convert.ToInt32(Math.Ceiling(tempo));
        }

        public int CalcularTempoTotalEntregaEmMinutos(IList<Pedido> pedidos, Drone drone)
        {
            if (!pedidos.Any())
                return 0;

            double latitudeOrigem = 0;
            double longitudeOrigem = 0;
            double distanciaTotal = 0;

            foreach (var pedido in pedidos)
            {
                // primeiro trajeto sai da loja
                if (distanciaTotal == 0)
                {
                    latitudeOrigem = _latitudeLoja;
                    longitudeOrigem = _longitudeLoja;
                }

                distanciaTotal += calcularDistanciaEmKilometros(latitudeOrigem, longitudeOrigem, (double)pedido.Latitude, (double)pedido.Longitude);

                // origem do proximo trajeto
                latitudeOrigem = (double)pedido.Latitude;
                longitudeOrigem = (double)pedido.Longitude;
            }

            // retorno para loja
            distanciaTotal += calcularDistanciaEmKilometros(latitudeOrigem, longitudeOrigem, _latitudeLoja, _longitudeLoja);

            int tempo = calcularTempoTrajetoEmMinutos(distanciaTotal, drone.Velocidade);

            return tempo;
        }

        public void DespacharPedidos()
        {
            atualizarStatusDrones();

            distribuirPedidos();
        }

        public void atualizarStatusDrones()
        {
            var droneItinerarios = _unitOfWork.DroneItinerario.GetAll().Where(d => d.StatusDrone != EnumStatusDrone.Disponivel).ToList();

            foreach (var droneItinerario in droneItinerarios)
            {
                droneItinerario.Drone = _unitOfWork.Drones.GetById(droneItinerario.DroneId);

                if (droneItinerario.StatusDrone == EnumStatusDrone.Carregando)
                {
                    if (DateTime.Now.Subtract(droneItinerario.DataHora).TotalMinutes >= droneItinerario.Drone.Carga)
                    {
                        droneItinerario.StatusDrone = EnumStatusDrone.Disponivel;
                        droneItinerario.Drone.AutonomiaRestante = droneItinerario.Drone.Autonomia;
                        droneItinerario.DataHora = DateTime.Now;
                    }
                }
                else if (droneItinerario.StatusDrone == EnumStatusDrone.EmTransito)
                {
                    var pedidos = _unitOfWork.Pedidos.GetAll().Where(p => p.Status == EnumStatusPedido.EmTransito && p.Drone.Id == droneItinerario.DroneId).ToList();

                    // int tempoEntrega = pedido.PrevisaoEntrega.Subtract(pedido.DataHora).Minutes;
                    int tempoEntrega = CalcularTempoTotalEntregaEmMinutos(pedidos, droneItinerario.Drone);

                    if (droneItinerario.DataHora.AddMinutes(tempoEntrega) <= DateTime.Now)
                    {
                        if (droneItinerario.Drone.AutonomiaRestante <= 5)
                            droneItinerario.StatusDrone = EnumStatusDrone.Carregando;
                        else
                            droneItinerario.StatusDrone = EnumStatusDrone.Disponivel;

                        droneItinerario.DataHora = DateTime.Now;

                        foreach (var pedido in pedidos)
                        {
                            pedido.Status = EnumStatusPedido.Entregue;

                            _unitOfWork.Pedidos.Update(pedido);
                        }
                    }
                }

                _unitOfWork.DroneItinerario.Update(droneItinerario);
            }

            _unitOfWork.Save();
        }

        private void distribuirPedidos()
        {
            var droneItinerarios = _unitOfWork.DroneItinerario.GetAll()
                .Where(d => d.StatusDrone == EnumStatusDrone.Disponivel)
                .ToList();

            foreach (var droneItinerario in droneItinerarios)
            {
                droneItinerario.Drone = _unitOfWork.Drones.GetById(droneItinerario.DroneId);
            }

            var dronesDisponiveis = droneItinerarios.Select(d => d.Drone).OrderByDescending(d => d.Capacidade).ToList();

            if (!dronesDisponiveis.Any())
                return;

            // pedidos mais antigos vao primeiro
            var pedidos = _unitOfWork.Pedidos.GetAll().Where(p => p.Status == EnumStatusPedido.AguardandoEntregador)
                .OrderBy(p => p.DataHora).ToList();

            if (!pedidos.Any())
                return;

            IDictionary<Drone, IList<Pedido>> pedidosPorDrone = new Dictionary<Drone, IList<Pedido>>();
            IList<Pedido> pedidosEntregar = new List<Pedido>();
            IList<Pedido> pedidosDistribuidos = new List<Pedido>();
            double distanciaTrajeto = 0;
            double distanciaRetorno = 0;
            double distanciaPercorrida = 0;
            double distanciaTotal = 0;
            double latitudeOrigem = 0;
            double longitudeOrigem = 0;
            int tempoTrajetoCompleto = 0;

            foreach (var drone in dronesDisponiveis)
            {
                pedidosEntregar.Clear();

                distanciaTrajeto = 0;
                distanciaRetorno = 0;
                distanciaPercorrida = 0;
                distanciaTotal = 0;
                latitudeOrigem = 0;
                longitudeOrigem = 0;
                tempoTrajetoCompleto = 0;

                int capacidadeDisponivel = drone.Capacidade;
                int autonomiaDisponivel = drone.AutonomiaRestante;

                foreach (var pedido in pedidos)
                {
                    if (pedidosDistribuidos.Contains(pedido))
                        continue;

                    if (pedido.Peso <= capacidadeDisponivel)
                    {
                        if (pedidosEntregar.Any())
                        {
                            latitudeOrigem = (double)pedidosEntregar.Last().Latitude;
                            longitudeOrigem = (double)pedidosEntregar.Last().Longitude;
                        }
                        else
                        {
                            latitudeOrigem = _latitudeLoja;
                            longitudeOrigem = _longitudeLoja;
                        }

                        distanciaTrajeto = calcularDistanciaEmKilometros(latitudeOrigem, longitudeOrigem, (double)pedido.Latitude, (double)pedido.Longitude);
                        distanciaRetorno = calcularDistanciaEmKilometros((double)pedido.Latitude, (double)pedido.Longitude, _latitudeLoja, _longitudeLoja);

                        distanciaTotal = distanciaPercorrida + distanciaTrajeto + distanciaRetorno;

                        tempoTrajetoCompleto = calcularTempoTrajetoEmMinutos(distanciaTotal, drone.Velocidade);

                        if (tempoTrajetoCompleto <= drone.AutonomiaRestante)
                        {
                            pedidosEntregar.Add(pedido);

                            distanciaPercorrida += distanciaTrajeto;

                            capacidadeDisponivel -= pedido.Peso;
                            autonomiaDisponivel = drone.AutonomiaRestante - tempoTrajetoCompleto;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    // se não cabe mais, nao precisa verificar os demais pedidos
                    if (capacidadeDisponivel <= 0 || tempoTrajetoCompleto >= drone.AutonomiaRestante)
                    {
                        break;
                    }
                }

                if (pedidosEntregar.Any())
                {
                    drone.AutonomiaRestante = autonomiaDisponivel;

                    if (!pedidosPorDrone.ContainsKey(drone))
                        pedidosPorDrone.Add(drone, new List<Pedido>());

                    foreach (var pedido in pedidosEntregar)
                    {
                        pedidosPorDrone[drone].Add(pedido);
                    }

                    pedidosDistribuidos = pedidosDistribuidos.Concat(pedidosEntregar).ToList();
                }
            }

            if (pedidosPorDrone.Count > 0)
            {
                foreach (var item in pedidosPorDrone)
                {
                    Drone drone = item.Key;

                    DroneItinerario droneItinerario = droneItinerarios.Where(d => d.DroneId == drone.Id).FirstOrDefault();
                    droneItinerario.DataHora = DateTime.Now;
                    droneItinerario.Drone = drone;
                    droneItinerario.DroneId = drone.Id;
                    droneItinerario.StatusDrone = EnumStatusDrone.EmTransito;

                    _unitOfWork.Drones.Update(drone);
                    _unitOfWork.DroneItinerario.Update(droneItinerario);

                    foreach (var pedido in item.Value)
                    {
                        pedido.Drone = drone;
                        pedido.Status = EnumStatusPedido.EmTransito;

                        _unitOfWork.Pedidos.Update(pedido);
                    }
                }

                _unitOfWork.Save();
            }

        }
    }
}
