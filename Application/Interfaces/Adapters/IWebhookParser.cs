using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Adapters
{
    public interface IWebhookParser
    {
        string ProviderName { get; }
        bool ValidateSignature(IHeaderDictionary headers, string rawBody);
        Task ProcessWebhookAsync(string rawBody);
    }
}
