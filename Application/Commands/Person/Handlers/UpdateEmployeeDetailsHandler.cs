using Application.Commands.Person;
using Application.Interfaces;
using Domain.ValueObjects;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Person.Handlers
{
    public class UpdateEmployeeDetailsHandler : IRequestHandler<UpdateEmployeeDetailsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmployeeDetailsHandler(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<bool> Handle(UpdateEmployeeDetailsCommand request, CancellationToken ct)
        {
            var employee = await _unitOfWork.Employees.GetByIdWithAccountAsync(request.EmployeeId);

            if (employee == null)
                return false;

            employee.UpdateName(request.FullName);

            employee.UpdateEmail(new EmailAddress(request.Email));

            if (employee.Account != null)
            {
                employee.Account.UpdatePhoneNumber(new PhoneNumber(request.MobileNumber));
            }

            await _unitOfWork.CompleteAsync(); 

            return true;
        }
    }
}