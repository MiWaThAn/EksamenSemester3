using Domain.Entity.Item.Registrations;
using System;
using Domain.Interfaces.Repos;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repo.Item.IRegistrationRepo
{
    public interface IHourRegistrationRepository : IRegistrationRepository<HourRegistration>
    {
        Task<HourRegistration?> GetByRegistrationNumberAsync(string registrationNumber);
    }
}
