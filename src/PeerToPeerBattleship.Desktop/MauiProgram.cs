using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PeerToPeerBattleship.Desktop.Components.Integration;
using PeerToPeerBattleship.Desktop.Components.Integration.CustomLogger;
using PeerToPeerBattleship.Desktop.Components.Integration.CustomLogger.Abstraction;
using Serilog;

namespace PeerToPeerBattleship.Desktop
{
    public static class MauiProgram
    {
        //TODO: Ver onde o programa encerra e dar o Log.CloseAndFlush(); para que a aplicação salve o log.
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

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

            builder.Services.AddSingleton(typeof(IContextualLogger<>), typeof(ContextualLogger<>));
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<TcpConnection>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
