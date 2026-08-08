using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessApp.Shared.Events
{
    public abstract record IntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
