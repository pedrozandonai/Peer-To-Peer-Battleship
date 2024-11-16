using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            //Configura os logs da aplicação
            LoggerInjection.CreateLogger();

            //Adiciona injeção de dependências
            ServiceProvider serviceProvider = AddDependencyInjection();

            var logger = serviceProvider.GetService<IContextualLogger<Program>>()!.Logger;
            logger.Information("Iniciando Aplicação...");
            
            try
            {
                StartProgram(serviceProvider);
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

        private static void StartProgram(ServiceProvider serviceProvider)
        {
            //Pega o serviço de TCP da integração com a aplicação desktop
            var tcpServerService = serviceProvider.GetService<TcpServer>();
            tcpServerService!.ConnectToDesktopApp();
        }

        private static ServiceProvider AddDependencyInjection()
        {
            var services = new ServiceCollection();
            services.AddLogger();
            services.AddTransient<TcpServer>();

            return services.BuildServiceProvider();
        }
    }
}
