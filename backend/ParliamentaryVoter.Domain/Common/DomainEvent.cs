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

        /// <summary>
        /// Equality comparison based on EventId
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not DomainEvent other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return EventId == other.EventId;
        }

        /// <summary>
        /// Hash code based on EventId
        /// </summary>
        public override int GetHashCode()
        {
            return EventId.GetHashCode();
        }

        /// <summary>
        /// Equality operators
        /// </summary>
        public static bool operator ==(DomainEvent? left, DomainEvent? right)
        {
            return left?.Equals(right) ?? right is null;
        }

        public static bool operator !=(DomainEvent? left, DomainEvent? right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// Generic base class for domain events with strongly typed aggregate ID
    /// </summary>
    /// <typeparam name="TAggregateId">Type of the aggregate identifier</typeparam>
    public abstract class DomainEvent<TAggregateId> : DomainEvent
        where TAggregateId : notnull
    {
        /// <summary>
        /// Strongly typed aggregate ID
        /// </summary>
        public new TAggregateId AggregateId { get; }

        /// <summary>
        /// Protected constructor for strongly typed domain events
        /// </summary>
        /// <param name="aggregateId">Strongly typed ID of the aggregate</param>
        /// <param name="aggregateType">Type of the aggregate</param>
        /// <param name="userId">ID of the user who triggered the action</param>
        /// <param name="correlationId">Correlation ID for tracking related events</param>
        protected DomainEvent(
            TAggregateId aggregateId,
            string aggregateType,
            Guid? userId = null,
            string? correlationId = null)
            : base(
                aggregateId is Guid guidId ? guidId : Guid.Parse(aggregateId.ToString()!),
                aggregateType,
                userId,
                correlationId)
        {
            if (aggregateId == null || aggregateId.Equals(default(TAggregateId)))
                throw new ArgumentException("Aggregate ID cannot be null or default", nameof(aggregateId));

            AggregateId = aggregateId;
        }
    }
}