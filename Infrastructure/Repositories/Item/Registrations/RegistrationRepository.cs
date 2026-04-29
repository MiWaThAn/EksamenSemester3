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
    internal class RegistrationRepository<T> : GenericRepository<T>, IRegistrationRepository<T> where T : Registration
    {
        internal RegistrationRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<T>> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.Set<T>().Where(x => x.EmployeeId == employeeId).AsNoTracking().AsSplitQuery().ToListAsync();
        }
        public async Task<IEnumerable<T>> GetByProjectIdAsync(Guid projectId)
        {
            return await _context.Set<T>().Where(x => x.ProjectId == projectId).AsNoTracking().AsSplitQuery().ToListAsync();
        }
        public async Task<IEnumerable<T>> GetByActivityIdAsync(Guid activityId)
        {
            return await _context.Set<T>().Where(x => x.ActivityId == activityId).AsNoTracking().AsSplitQuery().ToListAsync();
        }
        public async Task<IEnumerable<T>> GetByStatusAsync(Status status)
        {
            return await _context.Set<T>().Where(x => x.Status == status).AsNoTracking().AsSplitQuery().ToListAsync();
        }
    }
}
