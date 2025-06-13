using System;

namespace ParliamentaryVoter.Domain.Common
{
    public abstract class DomainEvent
    {

        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public string EventName => GetType().Name;
        public virtual int EventVersion => 1;
        public Guid AggregateId { get; protected set; }
        public string AggregateType { get; protected set; }
        public Guid? UserId { get; protected set; }
        public string? CorrelationId { get; protected set; }

        protected DomainEvent(
            Guid aggregateId, 
            string aggregateType, 
            Guid? userId = null, 
            string? correlationId = null)
        {
            if (aggregateId == Guid.Empty)
                throw new ArgumentException("Aggregate ID cannot be empty", nameof(aggregateId));
            
            if (string.IsNullOrWhiteSpace(aggregateType))
                throw new ArgumentException("Aggregate type cannot be null or empty", nameof(aggregateType));

            AggregateId = aggregateId;
            AggregateType = aggregateType;
            UserId = userId;
            CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        }

        public void SetUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
            UserId = userId;
        }

        public void SetCorrelationId(string correlationId)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
                throw new ArgumentException("Correlation ID cannot be null or empty", nameof(correlationId));
            
            CorrelationId = correlationId;
        }

        public override string ToString()
        {
            return $"{EventName} [EventId: {EventId}, AggregateId: {AggregateId}, OccurredOn: {OccurredOn:yyyy-MM-dd HH:mm:ss} UTC]";
        }

    }
}