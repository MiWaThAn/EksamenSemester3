using Application.Commands.Person.Queries;
using Application.Interfaces;
using MediatR;
using Shared.Model;

namespace Application.Commands.Person.Handlers
{
    //TODO: Du skal tage roller med include for at der kan være nogen roller på konton. Du skal ikke gøre det her gør det igennem [authorize(roles="admin,company")] i controlleren
    public class GetEmployeesByCompanyHandler : IRequestHandler<GetEmployeesByCompanyQuery, IEnumerable<CompanyEmployeeModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEmployeesByCompanyHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<IEnumerable<CompanyEmployeeModel>> Handle(GetEmployeesByCompanyQuery request, CancellationToken ct)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, ct);

            if (account == null)
                throw new UnauthorizedAccessException("Bruger-konto ikke fundet.");

            bool isSystemAdmin = account.Roles != null && account.Roles.Any(r => r.Title.Equals("Admin", StringComparison.OrdinalIgnoreCase));

            //if (!isSystemAdmin)
            //{

            //    if (!account.CompanyId.HasValue)
            //    {
            //        throw new UnauthorizedAccessException("Du har ikke tilladelse til at se dette firmas administrationspanel.");
            //    }

            //    if (account.CompanyId != request.CompanyId)
            //    {
            //        throw new UnauthorizedAccessException("Du kan kun administrere medarbejdere for din egen virksomhed.");
            //    }
            //}

            var company = await _unitOfWork.Companies.GetWithEmployeesAsync(request.CompanyId);

            if (company == null || company.Employees == null)
                return Enumerable.Empty<CompanyEmployeeModel>();

            return company.Employees.Select(e => new CompanyEmployeeModel
            {
                Id = e.Id,
                FullName = e.Name
            });
        }
    }
}