using Application.Commands.Person.Queries;
using Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Person.Handlers
{
    internal class GetCompanyIdByAccountIdQueryHandler : IRequestHandler<GetCompanyIdByAccountIdQuery, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCompanyIdByAccountIdQueryHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<Guid> Handle(GetCompanyIdByAccountIdQuery request, CancellationToken ct)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);

            if (account == null)
                return Guid.Empty;

            return account.CompanyId ?? Guid.Empty;
        }
    }
}