using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Person
{
    public class Customer : Base
    {
        public string CustomerNumber { get; internal set; }
        public string Name { get; internal set; }
        public string Email { get; internal set; }
        public string PhoneNumber { get; internal set; }

        public Customer(string customerNumber, string name, string email, string phoneNumber) : base()
        {
            CustomerNumber = customerNumber ?? throw new ArgumentNullException(nameof(customerNumber));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        }
        public void UpdateContactInfo(string newEmail, string newPhoneNumber)
        {
            Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
            PhoneNumber = newPhoneNumber ?? throw new ArgumentNullException(nameof(newPhoneNumber));
        }
        public void UpdateName(string newName)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName));
        }
    }
}