using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace UI.Services.Auth
{
    public class AuthorizeApiHttpMessageHandler : DelegatingHandler
    {
        private readonly ISecureStorage _secureStorage;

        public AuthorizeApiHttpMessageHandler(ISecureStorage secureStorage)
        {
            _secureStorage = secureStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Fetch the token saved during login from MAUI secure storage
            string token = await _secureStorage.GetAsync("auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                // Attach the "Bearer {token}" header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
