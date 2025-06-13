using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParliamentaryVoter.Domain.Common
{

    public abstract class BaseEntity
    {
        private readonly List<DomainEvent> _domainEvents = new();


        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }
        public Guid? CreatedBy { get; protected set; }
        public Guid? UpdatedBy { get; protected set; }
        public int Version { get; protected set; } = 1;
        public bool IsDeleted { get; protected set; } = false;
        public DateTime? DeletedAt { get; protected set; }
        public Guid? DeletedBy { get; protected set; }

        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            _domainEvents.Add(domainEvent);
        }

        protected void RemoveDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public virtual void MarkAsUpdated(Guid? updatedBy = null)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
            Version++;
        }

        public virtual void MarkAsDeleted(Guid? deletedBy = null)
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
            MarkAsUpdated(deletedBy);
        }

        public virtual void Restore(Guid? restoredBy = null)
        {
            if (!IsDeleted)
                return;

            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
            MarkAsUpdated(restoredBy);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            // If both entities are transient (no ID), they are not equal
            if (Id == Guid.Empty || other.Id == Guid.Empty)
                return false;

            return Id == other.Id;
        }


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }
}