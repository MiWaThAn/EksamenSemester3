using Domain.Entity.Item.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Person
{
    public class Employee : Account
    {
        public string EmployeeNumber { get; internal set; }
        public Guid CompanyId { get; internal set; }
        public bool Autonomous { get; internal set; }
        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.AsReadOnly();
        public string HashedPin { get; internal set; }
        public EmployeeType EmployeeType { get; internal set; }
        public Employee(string name, string hashedPassword, string username, string? email, string? phoneNumber, string employeeNumber, Guid companyId, bool autonomous, EmployeeType employeeType, string hashedPin) : base(name, hashedPassword, username, email, phoneNumber)
        {
            EmployeeNumber = employeeNumber ?? throw new ArgumentNullException(nameof(employeeNumber));
            CompanyId = companyId;
            Autonomous = autonomous;
            EmployeeType = employeeType;
            HashedPin = hashedPin ?? throw new ArgumentNullException(nameof(hashedPin));
        }
        public Registration CreateRegistration(RegistrationBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var registration = builder.WithEmployee(this).Build();
            if(registration.EmployeeId != this.Id) throw new ArgumentException("Registration does not belong to this employee.");
            registration.ValidateAgainst(_registrations);
            _registrations.Add(registration);
            UpdatedAt = DateTime.UtcNow;
            return registration;
        }
        public void RemoveRegistration(Guid registrationId)
        {
            var registration = _registrations.Find(r => r.Id == registrationId);
            if (registration == null) throw new ArgumentException("Registration not found for this employee.");
            _registrations.Remove(registration);
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateEmployeeType(EmployeeType newType)
        {
            EmployeeType = newType;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateAutonomy(bool isAutonomous)
        {
            Autonomous = isAutonomous;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdatePin(string newHashedPin)
        {
            HashedPin = newHashedPin ?? throw new ArgumentNullException(nameof(newHashedPin));
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
