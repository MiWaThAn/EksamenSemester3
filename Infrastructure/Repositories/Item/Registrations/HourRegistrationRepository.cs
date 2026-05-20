using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Domain.Entity.Item.Registrations;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item.Registrations
{
    public class HourRegistrationRepository : RegistrationRepository<HourRegistration>, IHourRegistrationRepository
    {
        public HourRegistrationRepository(AppDbContext context) : base(context)
        {
        }
    }
}
