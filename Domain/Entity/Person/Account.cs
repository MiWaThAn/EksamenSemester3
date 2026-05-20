using Domain.Builders.Person;
using Domain.Entity.Person.Auth;
using Domain.Guards;
using Domain.Interfaces.Person;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity.Person
{
    //Account class handles the general indentity and authentication information for users. 
    public class Account : Base
    {

        //Username is unique and used for login, while phone number is optional and can be used for 2FA or account recovery.
        public string Username { get; internal set; }
        public string HashedPassword { get; internal set; }
        public PhoneNumber PhoneNumber { get; internal set; }
        //HashedPin is used for quick login on mobile devices, it is optional and can be null if the user has not set it up.
        public string? HashedPin { get; internal set; }

        //Company and Employee ID's are nullable because an account might not be associated with a company or employee yet (e.g. during registration or if the account is for a user that only has access to the app but is not an employee). Once the account is linked to a company and/or employee, these fields will be populated.
        //If it is a company account it will have a company id and have admin rights over that companies employees and projects,
        //if it is an employee account it will have an employee id and be linked to a company through that employee.
        //An account could potentially have both if it is an admin user that also has an employee role,
        //but it could also have neither if it is a generic user account that is not linked to any company or employee.
        public Guid? CompanyId { get; internal set; }
        public Company? Company { get; internal set; }
        public Guid? EmployeeId { get; internal set; }
        public Employee? Employee { get; internal set; }
        //Liste der indeholder alle en accounts roller (de giver kontoen primission)
        public List<Role> Roles { get; private set; } = new();

        //Helper methods to quickly check the type of account based on the presence of CompanyId and EmployeeId. This allows for flexible account types and easy role management.
        public bool IsCompanyAccount => CompanyId.HasValue;
        public bool IsEmployeeAccount => EmployeeId.HasValue;

        //Password Recovery
        private string? RecorveryToken;
        private DateTime? RecoveryExpiry;

        //Last time the account pinged the server (last activity time)
        public DateTime LastLogin { get; internal set; } 

        public Account() : base()
        {

        }
        internal Account(string username, string hashedPassword, PhoneNumber phoneNumber,string? hashedPin,Employee? employee,Company? company) : base()
        {
            Guard.AgainstNullOrEmpty(hashedPassword, nameof(hashedPassword));
            Guard.AgainstNullOrEmpty(username, nameof(username));
            HashedPassword = hashedPassword;
            Username = username;
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
            HashedPin = hashedPin;
            if (Company != null) LinkToCompany(Company);
            if (Employee != null) LinkToEmployee(Employee);
        }
        public void UpdatePhoneNumber(PhoneNumber phoneNumber)
        {
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdatePassword(string newHashedPassword)
        {
            Guard.AgainstNullOrEmpty(newHashedPassword, nameof(newHashedPassword));
            HashedPassword = newHashedPassword;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateUsername(string newUsername)
        {
            Guard.AgainstNullOrEmpty(newUsername, nameof(newUsername));
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateHashedPin(string newHashedPin)
        {
            Guard.AgainstNullOrEmpty(newHashedPin, nameof(newHashedPin));
            HashedPin = newHashedPin;
            UpdatedAt = DateTime.UtcNow;
        }
        public void LinkToCompany(Company company)
        {
            CompanyId = company.Id;
            Company = company;
            UpdatedAt = DateTime.UtcNow;
        }
        public void LinkToEmployee(Employee employee)
        {
            EmployeeId = employee.Id;
            Employee = employee;
            UpdatedAt = DateTime.UtcNow;
        }
        public async Task<Result<Company>> CreateCompany(CompanyBuilder builder, ICompanyFactory companyFactory, CancellationToken ct = default)
        {
            Guard.AgainstNull(builder, nameof(builder));
            Guard.AgainstNull(companyFactory, nameof(companyFactory));
            builder = builder.WithAccount(this);
            var result = await companyFactory.CreateAsync(builder, this,ct);
            if(result.IsSuccess)
            {
                LinkToCompany(result.Value);
            }
            UpdatedAt = DateTime.UtcNow;
            return result;
        }
        public void AddRole(Role role)
        {
            if (!Roles.Any(r => r.Id == role.Id))
                Roles.Add(role);
        }
        public void UpdateLastLogin(DateTime time)
        {
            Guard.AgainstNull(time, nameof(time));
            LastLogin = time;
        }
        public string GeneratePasswordResetToken()
        {
            RecorveryToken = Guid.NewGuid().ToString();
            RecoveryExpiry = DateTime.UtcNow.AddMinutes(30);

            return RecorveryToken;
        }
        public void ResetPassword(string token, string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(RecorveryToken) || RecorveryToken != token)
                throw new Exception("Invalid reset token.");

            if (DateTime.UtcNow > RecoveryExpiry)
                throw new Exception("Reset token has expired.");

            HashedPassword = newPasswordHash;
            RecorveryToken = null;
            RecoveryExpiry = null;
        }
    }
}
