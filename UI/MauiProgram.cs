using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using UI.Services.Auth;
using UI.Services.Theme;

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





            builder.Services.AddScoped<IAuthService, AuthService>();
            

            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);
            builder.Services.AddScoped<JwtAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

            if (string.IsNullOrEmpty(apiBaseUrl))
            {
                // If dev tunnel doesn't work, try localhost instead
                apiBaseUrl = "https://localhost:7020/";
            }
            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl)
            });
#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
