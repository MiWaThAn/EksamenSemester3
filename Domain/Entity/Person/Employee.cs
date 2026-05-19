using Domain.Builders.Item.Registration;
using Domain.Entity.Item;
using Domain.Entity.Item.Activities;
using Domain.Entity.Item.Registrations;
using Domain.Guards;
using Domain.Services;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity.Person
{
    /// <summary>
    /// Fjernet fra employee klassen da logikken nu liger i intergrationmapping objectet (For at gøre det generisk som per spurgte efter):
    ///         public string ExternalId { get; internal set; }
    //DataSource enum til at holde styr på hvor medarbejderen kommer fra (fx. e-conomic, manuel oprettelse i appen osv...)
    //Den tjekkes når data skal sycroniseres mellem kilderne så vi ikke ender med at at forsøge at sycronisere data til en kilde som ikke er kilden for den pågældende medarbejder.
    //          public DataSource DataSource { get; internal set; }
    //Id til konto som medarbejder er forbundet til (valgfrit så hvis medarbejderen kommer fra e-conomic så behøver de ikke en konto indtil firmaet laver dem en)
    /// </summary>
    public class Employee : Base
    {
        //Holder info om medarbejderer i firmaet.



        //Medarbejder info
        public string Name { get; internal set; }
        public EmailAddress? Email { get; internal set; }
        //Medarbejder type (formand, lærling osv...)
        public EmployeeType EmployeeType { get; internal set; }
        //bool på om en medarbejder er selvstændig (har tilladelser til selv registrering osv...)
        public bool IsAutonomous { get; internal set; }
        //Id på firmaet de tilhøre
        public Guid CompanyId { get; internal set; }
        public Company Company { get; internal set; }
        //Id fra extern kilde (fx. e-conomic database)
        public Guid? AccountId { get; internal set; }
        public Account Account { get; internal set; }

        //Medarbejder registreringer
        private readonly List<WorkLog> _workLogs = new();
        public IReadOnlyCollection<WorkLog> WorkLogs => _workLogs.Where(r => !r.IsDeleted).ToList().AsReadOnly();
        public Employee() : base()
        {

        }
        internal Employee(string name, Guid companyId, EmployeeType employeeType, EmailAddress email, bool isAutonomous) : base()
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));;
            Name = name;
            CompanyId = companyId;
            IsAutonomous = isAutonomous;
            Email = email;
            EmployeeType = employeeType;
        }
        public void UpdateEmployeeType(EmployeeType newType)
        {
            EmployeeType = newType;
            UpdatedAt = DateTime.UtcNow;
        }
        public WorkLog CreateWorkLog(WorkLogBuilder builder)
        {
            Guard.AgainstNull(builder, nameof(builder));
            var worklog = builder.WithEmployee(this).Build();
            _workLogs.Add(worklog);
            return worklog;
        }
        public void UpdateAutonomy(bool isAutonomous)
        {
            IsAutonomous = isAutonomous;
            UpdatedAt = DateTime.UtcNow;
        }
        public void LinkToAccount(Account account)
        {
            if (AccountId != null) throw new InvalidOperationException("Denne medarbejder er allerede tilknyttet en konto.");
            AccountId = account.Id;
            Account = account;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateEmail(EmailAddress newEmail)
        {
            Guard.AgainstNull(newEmail, nameof(newEmail));
            Email = newEmail;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateName(string newName)
        {
            Guard.AgainstNullOrEmpty(newName, nameof(newName));
            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
