using System;
using System.Collections.Generic;

namespace ParliamentaryVoter.Domain.Common
{

    public interface IAggregateRoot
    {
        Guid Id { get; }
        int Version { get; }
        IReadOnlyCollection<DomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }


    public interface IAggregateRoot<TId> : IAggregateRoot
        where TId : notnull
    {
        new TId Id { get; }
    }

    public abstract class AggregateRoot : BaseEntity, IAggregateRoot
    {

        protected AggregateRoot()
        {
        }


        protected AggregateRoot(Guid id) 
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Aggregate root ID cannot be empty", nameof(id));
            
            // Use reflection to set the protected Id property
            typeof(BaseEntity).GetProperty(nameof(Id))?.SetValue(this, id);
        }

        protected void RaiseDomainEvent(DomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }

        public bool HasUncommittedEvents => DomainEvents.Count > 0;

        public int UncommittedEventCount => DomainEvents.Count;
    }

    public abstract class AggregateRoot<TId> : AggregateRoot, IAggregateRoot<TId>
        where TId : notnull
    {
        public new abstract TId Id { get; protected set; }

        Guid IAggregateRoot.Id => this.Id is Guid guidId ? guidId : Guid.Parse(this.Id.ToString()!);
        protected AggregateRoot()
        {
        }

        protected AggregateRoot(TId id)
        {
            if (id == null || id.Equals(default(TId)))
                throw new ArgumentException("Aggregate root ID cannot be null or default", nameof(id));
            
            Id = id;
        }
    }
}