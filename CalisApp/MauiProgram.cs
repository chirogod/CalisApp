using CalisApp.Services;
using CalisApp.Services.Interfaces;
using CalisApp.ViewModels;
using CalisApp.Views;
using Microsoft.Extensions.Logging;

namespace CalisApp
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<ISessionService, SessionService>();

            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HomeView>();

            builder.Services.AddTransient<SessionDetailViewModel>();
            builder.Services.AddTransient<SessionDetailView>();

            builder.Services.AddSingleton<SessionsViewModel>(); 
            builder.Services.AddSingleton<SessionsView>();

            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginView>();

            builder.Services.AddSingleton<ProfileViewModel>();
            builder.Services.AddSingleton<ProfileView>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
