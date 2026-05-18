using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using UI.Services.Auth;

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

            // Erstat med din Dev Tunnel URL fra før!
            var apiBaseUrl = "https://din-tunnel-id.euw.devtunnels.ms/";

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl)
            });

            // Registrer din AuthService, som vi lavede tidligere
            builder.Services.AddScoped<IAuthService, AuthService>();
            

            builder.Services.AddAuthorizationCore();

   
            builder.Services.AddScoped<JwtAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
