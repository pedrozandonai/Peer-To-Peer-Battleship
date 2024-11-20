using Microsoft.Extensions.DependencyInjection;
using PeerToPeerBattleship.Application.Games;
using PeerToPeerBattleship.Application.Games.Abstractions;
using PeerToPeerBattleship.Core.Inputs;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
using PeerToPeerBattleship.Infraestructure.Networking;
using PeerToPeerBattleship.Infraestructure.Networking.Abstractions;

namespace PeerToPeerBattleship.ConsoleApp.DependencyInjection
{
    public static class ServicesInjection
    {
        public static ServiceCollection AddServices(this ServiceCollection services)
        {
            services.AddSingleton<ISock, Sock>();
            services.AddSingleton<IUserInputHandler, UserInputHandler>();
            services.AddSingleton<IGame, Game>();

            return services;
        }
    }
}
