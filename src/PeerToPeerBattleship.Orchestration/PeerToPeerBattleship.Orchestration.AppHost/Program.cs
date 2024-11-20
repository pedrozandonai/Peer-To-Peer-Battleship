using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeerToPeerBattleship.Common.Configurations;
using System.Text.Json;
public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        //Adiciona as configurações do programa
        var applicationConfigurations = new ApplicationConfigurations();
        configuration.GetSection(nameof(ApplicationConfigurations)).Bind(applicationConfigurations);
        builder.Services.AddSingleton(applicationConfigurations);

        var backEndResourceBuilder = builder.AddProject("backend", @"..\..\PeerToPeerBattleship.ConsoleApp\PeerToPeerBattleship.ConsoleApp.csproj");
        if (applicationConfigurations.EnableDesktopApp)
        {
            backEndResourceBuilder.WithArgs("--enable-desktop-app");
            builder.AddProject("desktop", @"..\..\PeerToPeerBattleship.Desktop\PeerToPeerBattleship.Desktop.csproj");
        }

        await builder.Build().RunAsync();
    }
}