using Domain.Entity.Mapping;
using Domain.Entity.Person;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity.Item
{
    public class Expense : Base
    {
        public string Name { get; internal set; }
        public ApprovalStatus Status { get; internal set; }
        public Guid CompanyId { get; internal set; }
        public Expense() : base()
        {

        }
        public Expense(string name, Guid companyId) : base()
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstEmptyGuid(companyId, nameof(companyId));
            Name = name;
            CompanyId = companyId;
        }
        public void UpdateExpenseName(string newName)
        {
            Guard.AgainstNullOrEmpty(newName, nameof(newName));
            Name = newName;
            Status = ApprovalStatus.Draft;
            UpdatedAt = DateTime.UtcNow;
        }
        public void SubmitForApproval(Company company)
        {
            Guard.AgainstNull(company, nameof(company));
            if (company.Id != CompanyId)
            {
                throw new InvalidOperationException("Company does not own this expense.");
            }
            Status = ApprovalStatus.Pending;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Approve(Company company)
        {
            Guard.AgainstNull(company, nameof(company));
            if(company.Id != CompanyId)
            {
                throw new InvalidOperationException("Company does not own this expense.");
            }
            Status = ApprovalStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Reject(Company company)
        {
            Guard.AgainstNull(company, nameof(company));
            Status = ApprovalStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
