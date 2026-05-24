using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers.Integration
{
    public class DeleteIntegrationSettingHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<DeleteIntegrationSettingCommand>
    {
        public async Task Handle(DeleteIntegrationSettingCommand request, CancellationToken cancellationToken)
        {
            bool active = false;
            try
            {
                await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
                active = true;

                var company = await _unitOfWork.Companies.GetByAccountIdAsync(request.AccountId)
                    ?? throw new InvalidOperationException($"Firma ikke fundet.");

                company.RemoveIntegrationSetting(request.SettingId);

                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception)
            {
                if (active)
                    await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
