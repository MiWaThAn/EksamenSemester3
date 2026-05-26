using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using UI.Services.Auth;
using UI.Services.Integration;
using UI.Services.Theme;
using Plugin.Firebase.CloudMessaging;
using Microsoft.Maui.LifecycleEvents;
using UI.Services.Auth.Registration;
using UI.Services.Registration;


#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

namespace UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddScoped<PushRegistrationService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);
            builder.Services.AddTransient<AuthorizeApiHttpMessageHandler>();
            builder.Services.AddScoped<JwtAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
            builder.Services.AddScoped<IIntegrationService, IntegrationService>();
            builder.Services.AddScoped<RegistrationService>();
            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

            if (string.IsNullOrEmpty(apiBaseUrl))
            {
                apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
                    ? "https://10.0.2.2:7020/"
                    : "https://localhost:7020/";
            }

            // Register HttpClient ONCE cleanly
            builder.Services.AddScoped(sp =>
            {
                HttpMessageHandler handler;
#if ANDROID
                // Til Android bruger vi den indfødte Java-handler og tvinger den til at godkende certifikatet
                handler = new Xamarin.Android.Net.AndroidMessageHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
#else                
                // Til Windows / browser (hvis du tester der) bruger vi standard handleren
                handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
#endif

                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(apiBaseUrl)
                };

                // Sørg for at Microsofts anti-phishing side ikke blokerer Android-appen
                client.DefaultRequestHeaders.Add("X-Tunnel-Skip-Anti-Phishing-Page", "true");

                return client;
            }); // <-- This closing statement was missing or out of place!

            builder.Services.AddMauiBlazorWebView();
            builder.RegisterFirebaseServices();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
#if IOS            
                events.AddiOS(iOS => iOS.WillFinishLaunching((_,__) => {
                    CrossFirebase.Initialize();
                    return false;
                }));
#elif ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                    CrossFirebase.Initialize(activity, () => Platform.CurrentActivity)));
#endif
            });
            return builder;
        }
    }
}