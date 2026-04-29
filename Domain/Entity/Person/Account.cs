using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Person
{
    //Account class handles the general indentity and authentication information for users. 
    public abstract class Account : Base
    {
        public string Name { get; protected set; }
        public string HashedPassword { get; protected set; }
        public string Username { get; protected set; }
        public string? Email { get; protected set; }
        public string? PhoneNumber { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public DateTime LastSync { get; protected set; } //Last time the account pinged the server (last activity time)

        internal Account(string name, string hashedPassword, string username, string? email, string? phoneNumber) : base()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            HashedPassword = hashedPassword ?? throw new ArgumentNullException(nameof(hashedPassword));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email;
            PhoneNumber = phoneNumber;
            IsDeleted = false;
            UpdatedAt = CreatedAt = LastSync = DateTime.UtcNow;
        }
        public void UpdateContactInfo(string? email, string? phoneNumber)
        {
            Email = email;
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdatePassword(string newHashedPassword)
        {
            HashedPassword = newHashedPassword ?? throw new ArgumentNullException(nameof(newHashedPassword));
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateUsername(string newUsername)
        {
            Username = newUsername ?? throw new ArgumentNullException(nameof(newUsername));
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
