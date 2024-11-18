using Microsoft.Extensions.DependencyInjection;
using PeerToPeerBattleship.Application.Games.Abstractions;
using PeerToPeerBattleship.Application.Games;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
using PeerToPeerBattleship.Core.Inputs;
using PeerToPeerBattleship.DesktopAppIntegration;
using PeerToPeerBattleship.Infraestructure.Networking.Abstractions;
using PeerToPeerBattleship.Infraestructure.Networking;

namespace PeerToPeerBattleship.ConsoleApp.DependencyInjection
{
    public static class ServicesInjection
    {
        public static ServiceCollection AddServices(this ServiceCollection services)
        {
            services.AddSingleton<ISock, Sock>();
            services.AddTransient<TcpServer>();
            services.AddSingleton<IUserInputHandler, UserInputHandler>();
            services.AddSingleton<IGame, Game>();

            return services;
        }
    }
}
