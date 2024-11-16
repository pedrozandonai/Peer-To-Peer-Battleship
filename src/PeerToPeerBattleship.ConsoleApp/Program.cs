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
            StartProgram();
        }

        private static void StartProgram()
        {
            try
            {
                // Configurando o IConfiguration para carregar o appsettings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                //Configura os logs da aplicação
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build())
                    .CreateLogger();

                //Adiciona injeção de dependências
                ServiceProvider serviceProvider = AddDependencyInjection();

                Log.Information("Iniciando Aplicação...");

                //Pega o serviço de TCP da integração com a aplicação desktop
                var tcpServerService = serviceProvider.GetService<TcpServer>();
                tcpServerService!.ConnectToDesktopApp();
            }
            catch (Exception ex)
            {
                Log.Fatal("Ocorreu um erro na aplicação.\nExceção: {0} \nStack Trace: {1}\nInner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static ServiceProvider AddDependencyInjection()
        {
            var services = new ServiceCollection();
            services.AddSingleton(typeof(IContextualLogger<>), typeof(ContextualLogger<>));
            services.AddSingleton(Log.Logger);
            services.AddTransient<TcpServer>();

            return services.BuildServiceProvider();
        }
    }
}
