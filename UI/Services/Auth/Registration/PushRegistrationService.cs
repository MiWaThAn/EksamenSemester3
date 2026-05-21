
using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Firebase.CloudMessaging;
using System.Net.Http.Json;

namespace UI.Services.Auth.Registration
{
    public class PushRegistrationService
    {
        private readonly HttpClient _httpClient;
        public PushRegistrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task RegisterDeviceAsync(Guid currentUserId, CancellationToken cancellationToken)
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
                if (!string.IsNullOrEmpty(deviceToken))
                {
                    var requestPayload = new
                    {
                        UserId = currentUserId,
                        Token = deviceToken
                    };
                    await _httpClient.PostAsJsonAsync("api/notifications/register-token", requestPayload, cancellationToken);
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
            if(status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }
            return status == PermissionStatus.Granted;
        }
    }
}
