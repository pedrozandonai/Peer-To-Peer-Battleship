using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using Serilog;

namespace PeerToPeerBattleship.Core.CustomLogger
{
    public static class LoggerInjection
    {
        public static void CreateLogger()
        {
            //Configura os logs da aplicação
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build())
                .CreateLogger();
        }

        public static ServiceCollection AddLogger(this ServiceCollection services)
        {
            // Registra o Serilog como serviço
            services.AddSingleton(Log.Logger);
            
            // Registra o logger contextual
            services.AddSingleton(typeof(IContextualLogger<>), typeof(ContextualLogger<>));

            return services;
        }
    }
}
