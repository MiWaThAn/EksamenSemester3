using Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Services.Integration
{
    public interface IIntegrationService
    {
        Task<List<ProviderModel>> GetProvidersAsync();
        Task<bool> CreateIntegrationSettingAsync(string providerName, string keyName, string keyValue, List<string> entityTypes);
        Task<bool> DeleteIntegrationSettingAsync(Guid settingId);
        Task<List<IntegrationSettingModel>> GetMyIntegrationSettingsAsync();
    }
}
