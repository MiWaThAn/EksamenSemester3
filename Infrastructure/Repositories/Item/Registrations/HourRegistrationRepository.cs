using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Domain.Entity.Item.Registrations;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item.Registrations
{
    internal class HourRegistrationRepository : RegistrationRepository<HourRegistration>, IHourRegistrationRepository
    {
        public HourRegistrationRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<HourRegistration?> GetByRegistrationNumberAsync(string registrationNumber)
        {
            return await _context.HourRegistrations.FirstOrDefaultAsync(x => x.RegistrationNumber == registrationNumber);
        }
    }
}
