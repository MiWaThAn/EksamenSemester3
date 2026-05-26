
using Plugin.Firebase.CloudMessaging;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace UI.Services.Auth.Registration
{
    public class PushRegistrationService
    {
        private readonly HttpClient _httpClient;
        public PushRegistrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task RegisterDeviceAsync(Guid currentUserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var permissionGranted = await RequestNotificationPermissionAsync();
                if (!permissionGranted)
                {
                    return;
                }
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                var deviceToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                var requestPayload = new
                {
                    UserId = currentUserId,
                    Token = deviceToken
                };
                var authToken = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    Console.WriteLine("Push registration skipped: No auth token found in SecureStorage.");
                    return;
                }
                var request = new HttpRequestMessage(HttpMethod.Post, "api/notifications/register-token");
                request.Content = JsonContent.Create(requestPayload);

                if (!string.IsNullOrEmpty(authToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                }
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API rejected token registration: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to register push token: {ex.Message}");
            }
        }
        private async Task<bool> RequestNotificationPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }
            return status == PermissionStatus.Granted;
        }
    }
}