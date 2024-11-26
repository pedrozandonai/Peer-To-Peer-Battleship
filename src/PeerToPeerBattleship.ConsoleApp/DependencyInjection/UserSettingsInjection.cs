using Microsoft.Extensions.DependencyInjection;
using PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions;
using PeerToPeerBattleship.Application.UsersSettings.Services;

namespace PeerToPeerBattleship.ConsoleApp.DependencyInjection
{
    public static class UserSettingsInjection
    {
        public static IServiceCollection AddUserSettings(this IServiceCollection services)
        {
            services.AddSingleton<IUserSettingsService, UserSettingsService>();
            services.AddSingleton<IModifySettingsService, ModifySettingsService>();

            return services;
        }
    }
}
