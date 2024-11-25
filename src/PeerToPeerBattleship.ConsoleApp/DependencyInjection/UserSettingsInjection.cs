using Microsoft.Extensions.DependencyInjection;

namespace PeerToPeerBattleship.ConsoleApp.DependencyInjection
{
    public static class UserSettingsInjection
    {
        public static IServiceCollection AddUserSettings(this IServiceCollection services)
        {

            return services;
        }
    }
}
