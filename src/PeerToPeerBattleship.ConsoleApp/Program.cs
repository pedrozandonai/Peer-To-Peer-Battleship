using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeerToPeerBattleship.Application.Games.Abstractions;
using PeerToPeerBattleship.ConsoleApp.DependencyInjection;
using PeerToPeerBattleship.Core.Configurations;
using PeerToPeerBattleship.Core.CustomLogger;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.DesktopAppIntegration;
using Serilog;

namespace PeerToPeerBattleship.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configurando o IConfiguration para carregar o appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ApplicationSettings applicationSettings = new ApplicationSettings();
            configuration.GetSection(nameof(ApplicationSettings)).Bind(applicationSettings);

            //Configura os logs da aplicação
            LoggerInjection.CreateLogger();

            //Adiciona injeção de dependências
            ServiceProvider serviceProvider = AddDependencyInjection(applicationSettings);

            var logger = serviceProvider.GetService<IContextualLogger<Program>>()!.Logger;
            logger.Information("Iniciando Aplicação...");
            
            try
            {
                bool enableDesktopApp = args.Contains("--enable-desktop-app");
                StartProgram(serviceProvider, logger, enableDesktopApp);
            }
            catch (Exception ex)
            {
                logger.LogExceptionError("Ocorreu um erro na aplicação", ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void StartProgram(ServiceProvider serviceProvider, ILogger logger, bool enableDesktopApp)
        {
            try
            {
                if (enableDesktopApp)
                {
                    //Pega o serviço de TCP da integração com a aplicação desktop
                    var tcpServerService = serviceProvider.GetService<TcpServer>();
                    tcpServerService!.ConnectToDesktopApp();
                }

                var game = serviceProvider.GetService<IGame>();
                game!.Create();
            }
            catch (Exception ex)
            {
                logger.LogExceptionError("Erro na execução do programa.", ex);
            }
        }

        private static ServiceProvider AddDependencyInjection(ApplicationSettings applicationSettings)
        {
            var services = new ServiceCollection();
            services.AddSingleton(applicationSettings);
            services.AddLogger();
            services.AddServices();

            return services.BuildServiceProvider();
        }
    }
}
