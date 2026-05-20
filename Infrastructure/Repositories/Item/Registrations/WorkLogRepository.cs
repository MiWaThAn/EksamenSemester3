using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Domain.Entity.Item.Registrations;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item.Registrations
{
    internal class WorkLogRepository : GenericRepository<WorkLog>, IWorkLogRepository
    {
        public WorkLogRepository(AppDbContext context) : base(context) { }
    }
}
