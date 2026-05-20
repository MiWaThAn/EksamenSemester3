using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item.Registrations
{
    public class RegistrationRepository<T> : GenericRepository<T>, IRegistrationRepository<T> where T : Registration
    {
        public RegistrationRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<T>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().Where(x => x.EmployeeId == employeeId).AsNoTracking().AsSplitQuery().ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<T>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().Where(x => x.ProjectId == projectId).AsNoTracking().AsSplitQuery().ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<T>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().Where(x => x.ProjectActivityId == activityId).AsNoTracking().AsSplitQuery().ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<T>> GetByStatusAsync(RegistrationStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().Where(x => x.Status == status).AsNoTracking().AsSplitQuery().ToListAsync(cancellationToken);
        }
    }
}
