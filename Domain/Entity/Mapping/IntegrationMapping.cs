using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Mapping
{
    public class IntegrationMapping : Base
    {
        //Hvis en medarbejder, et projekt eller en aktivitet er blevet migreret fra et eksternt system (f.eks. e-conomic) og ind i vores system, så gemmer vi en mapping mellem det lokale ID i vores system og det eksterne ID i det gamle system.
        //Det gør det muligt for os at soft delete entiteten i vores system og slette mappingen uden at skulle slette entititen helt og miste registrerings data.

        // LocalId kan være ID'et på både en Employee, et Project eller en Activity
        public Guid LocalId { get; internal set; }

        // Hvilken type entitet mapper vi? (f.eks. "Employee", "Project")
        // Dette hjælper med at undgå sammenstød hvis to forskellige typer har samme GUID
        public IntegrationEntityType EntityType { get; internal set; }

        public string ExternalId { get; internal set; }
        public DataSource Provider { get; internal set; } 

        internal IntegrationMapping(Guid localId, IntegrationEntityType entityType, string externalId, DataSource provider)
        {
            LocalId = localId;
            EntityType = entityType;
            ExternalId = externalId;
            Provider = provider;
        }
    }
}
