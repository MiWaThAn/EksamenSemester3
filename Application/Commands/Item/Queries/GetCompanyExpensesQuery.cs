using MediatR;
using Shared.Model;
using System;
using System.Collections.Generic;

namespace Application.Commands.Item.Queries
{
    public class GetCompanyExpensesQuery : IRequest<IEnumerable<CompanyExpenseModel>>
    {
        public Guid CompanyId { get; }

        public GetCompanyExpensesQuery(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
}
