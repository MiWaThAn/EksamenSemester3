using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configuration;

public class EconomicOptions
{
    public const string SectionName = "ExternalProviders:Economic";

    public string XAppSecretToken { get; set; } = string.Empty;
    public string XAgreementGrantToken { get; set; } = string.Empty;
}
