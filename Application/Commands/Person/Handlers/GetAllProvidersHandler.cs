using Application.Commands.Person.Handlers;
using Application.Interfaces;
using MediatR;
using Shared.Model;

public class GetAllProvidersHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<GetAllProvidersQuery, IEnumerable<ProviderModel>>
{
    public async Task<IEnumerable<ProviderModel>> Handle(GetAllProvidersQuery request, CancellationToken cancellationToken)
    {
        var providers = await _unitOfWork.Providers.GetAllAsync(cancellationToken);
        return providers.Select(p => new ProviderModel
        {
            Id = p.Id,
            Name = p.Datasource.Value
        });
    }
}