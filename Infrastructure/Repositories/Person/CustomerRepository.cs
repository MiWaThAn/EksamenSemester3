using Application.Interfaces.Repo.Person;
using Domain.Entity.Person;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Person
{
    internal class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Customer?> GetByCompanyIdAsync(Guid companyId)
        {
            throw new NotImplementedException();
        }
    }
}
