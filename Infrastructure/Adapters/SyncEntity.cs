using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Adapters
{
    public class SyncEntity
    {
        public Base Entity { get; set; }     
        public string ExternalId { get; set; }     
        public string ObjectVersion { get; set; } 
    }
}
