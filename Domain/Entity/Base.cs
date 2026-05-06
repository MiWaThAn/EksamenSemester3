using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entity
{
    public abstract class Base
    {
        public Guid Id { get; internal set; }
        [Timestamp]
        public byte[] RowVersion { get; internal set; }
        public bool IsDeleted { get; internal set; }
        public DateTime? DeletedAt { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
        public DateTime UpdatedAt { get; internal set; }
        protected Base()
        {
            Id = Guid.NewGuid();
            IsDeleted = false;
            CreatedAt = UpdatedAt = DateTime.UtcNow;
        }
        public void SoftDelete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
        public void UndoSoftDelete()
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}
