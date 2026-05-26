using Application.Commands.Item.Queries;
using Application.Interfaces;
using MediatR;
using Shared.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Item.Handlers
{
    public class GetCompanyExpensesHandler : IRequestHandler<GetCompanyExpensesQuery, IEnumerable<CompanyExpenseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCompanyExpensesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CompanyExpenseModel>> Handle(GetCompanyExpensesQuery request, CancellationToken ct)
        {
            var expenses = await _unitOfWork.Expenses.GetByCompanyIdAsync(request.CompanyId, ct);

            if (expenses == null) return Enumerable.Empty<CompanyExpenseModel>();

            return expenses.Select(e => new CompanyExpenseModel
            {
                Id = e.Id,
                Name = e.Name,
                Status = e.Status.ToString(),
                IsSelected = false,
                NotificationCount = 0
            });
        }
    }
}
