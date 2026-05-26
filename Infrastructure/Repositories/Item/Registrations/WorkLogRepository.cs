using Application.Interfaces.Repo.Item.IRegistrationRepo;
using Azure.Core;
using Domain.Entity.Item;
using Domain.Entity.Item.Registrations;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories.Item.Registrations
{
    internal class WorkLogRepository : GenericRepository<WorkLog>, IWorkLogRepository
    {
        public WorkLogRepository(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<WorkLog>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkLogs.Where(wl => wl.EmployeeId == employeeId).ToListAsync(cancellationToken);
        }
        public async Task<WorkLog?> GetTodayByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkLogs.Include(wl => wl.Registrations).FirstOrDefaultAsync(wl => wl.EmployeeId == employeeId && wl.DateCreated.Date == DateTime.UtcNow.Date, cancellationToken);
        }
        public async Task<WorkLog?> GetByIdWithRegistrationsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.WorkLogs.Include(wl => wl.Registrations).FirstOrDefaultAsync(wl => wl.Id == id, cancellationToken);
        }
        public async Task<WorkLog?> GetActiveByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkLogs.Include(wl => wl.Registrations).FirstOrDefaultAsync(wl => wl.EmployeeId == employeeId && wl.IsClosed == false, cancellationToken);
        }
        public async Task<IEnumerable<WorkLog>> GetAllActiveWorkLogsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.WorkLogs.Include(wl => wl.Registrations).Where(wl => wl.IsClosed == false).ToListAsync(cancellationToken);
        }
        public async Task<WorkLog?> GetActiveWorkLogAsNoTrackingAsync(Guid EmployeeId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkLogs.AsNoTracking().Include(wl => wl.Registrations).FirstOrDefaultAsync(wl => wl.EmployeeId == EmployeeId && wl.IsClosed == false, cancellationToken);
        }
        public async Task<IEnumerable<WorkLog>> GetPendingWorkLogsAsNoTrackingAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var emps = await _context.Employees.Where(e => e.CompanyId == companyId).Select(e => e.Id).ToListAsync(cancellationToken);
            return await _context.WorkLogs.AsNoTracking().Include(wl => wl.Registrations).Where( wl => wl.Status == ApprovalStatus.Pending && emps.Contains(wl.EmployeeId)).ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<WorkLog>> GetWorkLogHistoryAsNoTrackingAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkLogs.AsNoTracking().Include(wl => wl.Registrations).Where(wl => wl.EmployeeId == employeeId).ToListAsync(cancellationToken);
        }
        public async Task<WorkLog?> GetWorkLogByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.WorkLogs.AsNoTracking().Include(wl => wl.Registrations).FirstOrDefaultAsync(wl => wl.Id == id, cancellationToken);
        }
    }
}
