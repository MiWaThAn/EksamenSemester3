using Application.Commands.Person.Responses;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Builders.Mapping;
using Domain.Builders.Person;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Person.Handlers
{
    public class AddIntegrationHandler : IRequestHandler<AddIntegrationSettingCommand, AddIntagrationSettingReponse>
    {
        private readonly IEncryptionService _encryption;
        private readonly IUnitOfWork _unitOfWork;
        
        public AddIntegrationHandler(IEncryptionService encryption, IUnitOfWork unitOfWork)
        {
            _encryption = encryption;
            _unitOfWork = unitOfWork;
        }
        public async Task<AddIntagrationSettingReponse> Handle(AddIntegrationSettingCommand request, CancellationToken cancellationToken)
        {
            _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var company = await _unitOfWork.Companies.GetByIdAsync(request.CompanyId);

            company.CreateIntegrationSetting(new IntegrationSettingBuilder()
                .WithProvider(request.Provider)
                .WithKey(request.Key)
                .WithEncryptedValue(await _encryption.Encrypt(request.Value)));

            await _unitOfWork.CommitTransactionAsync();

            return new AddIntagrationSettingReponse
            {
                Success = true,
                Message = "Integration setting added successfully.",
                Errors = null
            };
        }
    }
}
