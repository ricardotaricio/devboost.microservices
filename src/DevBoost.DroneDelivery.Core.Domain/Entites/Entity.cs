﻿using DevBoost.DroneDelivery.Core.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Core.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class Entity
    {
        private List<Event> _notificacoes;
        public IReadOnlyCollection<Event> Notificacoes => _notificacoes?.AsReadOnly();

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }

        public void AdicionarEvento(Event evento)
    }
}