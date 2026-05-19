using Microsoft.Extensions.Logging;

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

            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

            if (string.IsNullOrEmpty(apiBaseUrl))
            {
                apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
                    ? "https://10.0.2.2:7020/"
                    : "https://localhost:7020/";
            }

            builder.Services.AddMauiBlazorWebView();

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
                    BaseAddress = new Uri("https://rj7mxw9r-7020.euw.devtunnels.ms/")
                };

                // Sørg for at Microsofts anti-phishing side ikke blokerer Android-appen
                client.DefaultRequestHeaders.Add("X-Tunnel-Skip-Anti-Phishing-Page", "true");

                return client;
            });
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
