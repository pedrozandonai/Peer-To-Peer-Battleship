using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeerToPeerBattleship.Application.Games.Abstractions;
using PeerToPeerBattleship.ConsoleApp.DependencyInjection;
using PeerToPeerBattleship.Core.Configurations;
using PeerToPeerBattleship.Core.CustomLogger;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.Core.Extensions;
using Serilog;

namespace PeerToPeerBattleship.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "PeerToPeerBattleship";

            // Configurando o IConfiguration para carregar o appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ApplicationSettings applicationSettings = new ApplicationSettings();
            configuration.GetSection(nameof(ApplicationSettings)).Bind(applicationSettings);

            if (!applicationSettings.SkipApplicationLogo) StartDisplay();

            //Configura os logs da aplicação
            LoggerInjection.CreateLogger();

            //Adiciona injeção de dependências
            ServiceProvider serviceProvider = AddDependencyInjection(applicationSettings);

            var logger = serviceProvider.GetService<IContextualLogger<Program>>()!.Logger;
            logger.Information("Iniciando Aplicação...");
            
            try
            {
                applicationSettings.VerifySettings();
                await StartProgram(serviceProvider, logger);
            }
            catch (Exception ex)
            {
                logger.LogExceptionError("Ocorreu um erro na aplicação", ex);
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }

        private static async Task StartProgram(ServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                var game = serviceProvider.GetService<IGame>();
                await game!.Create();
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

        private static void StartDisplay()
        {
            ConsoleExtension.Clear();
            Console.WriteLine("██████  ███████ ███████ ██████        ████████  ██████        ██████  ███████ ███████ ██████  ");
            Console.WriteLine("██   ██ ██      ██      ██   ██          ██    ██    ██       ██   ██ ██      ██      ██   ██ ");
            Console.WriteLine("██████  █████   █████   ██████  █████    ██    ██    ██ █████ ██████  █████   █████   ██████  ");
            Console.WriteLine("██      ██      ██      ██   ██          ██    ██    ██       ██      ██      ██      ██   ██ ");
            Console.WriteLine("██      ███████ ███████ ██   ██          ██     ██████        ██      ███████ ███████ ██   ██ ");
            Console.WriteLine("                                                                                              ");
            Console.WriteLine("                                                                                              ");
            Console.WriteLine("    ██████   █████  ████████ ████████ ██      ███████ ███████ ██   ██ ██ ██████               ");
            Console.WriteLine("    ██   ██ ██   ██    ██       ██    ██      ██      ██      ██   ██ ██ ██   ██              ");
            Console.WriteLine("    ██████  ███████    ██       ██    ██      █████   ███████ ███████ ██ ██████               ");
            Console.WriteLine("    ██   ██ ██   ██    ██       ██    ██      ██           ██ ██   ██ ██ ██                   ");
            Console.WriteLine("    ██████  ██   ██    ██       ██    ███████ ███████ ███████ ██   ██ ██ ██                   ");
            Console.WriteLine("");
            Console.WriteLine("Implementado por: Pedro Henrique Zandonai Persch");

            Thread.Sleep(5000);
            ConsoleExtension.Clear();
        }
    }
}
