
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
                // 1. Definer en flag eller tjek om du er i et testmiljø
                bool pushEnabled = false; // Sæt til 'true' når du er klar til at teste FCM

                string deviceToken = "mock-token-for-testing";

                if (pushEnabled)
                {
                    var permissionGranted = await RequestNotificationPermissionAsync();
                    if (!permissionGranted) return;

                    await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                    deviceToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                }

                // ... resten af din kode for at sende token til API'et ...
                var requestPayload = new { UserId = currentUserId, Token = deviceToken };
                // ... resten af din logik
            }
            catch (Exception ex)
            {
                // Hvis fejlen skyldes NotImplementedException, bliver den fanget her
                Console.WriteLine($"Spring push-registrering over midlertidigt: {ex.Message}");
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
