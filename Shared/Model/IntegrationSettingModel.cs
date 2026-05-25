using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model
{
    public class IntegrationSettingModel
    {
        public Guid Id { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string KeyName { get; set; } = string.Empty;
        public string KeyValue { get; set; }
        public List<string> SelectedEntityTypes { get; set; } = new();
    }
}
