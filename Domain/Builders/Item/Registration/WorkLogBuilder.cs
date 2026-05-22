using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using Domain.Entity.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services
{
    public class WorkLogBuilder
    {
        private Employee Employee;
        internal WorkLogBuilder WithEmployee(Employee employee)
        {
            Employee = employee;
            return this;
        }
        internal WorkLog Build() => new WorkLog(Employee);
    }
}
