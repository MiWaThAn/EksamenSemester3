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
        protected Base()
        {
            Id = Guid.NewGuid();
        }
    }
}
