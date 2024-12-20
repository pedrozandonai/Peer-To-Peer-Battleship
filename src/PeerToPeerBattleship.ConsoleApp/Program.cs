﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeerToPeerBattleship.Application.Games.Abstractions;
using PeerToPeerBattleship.Application.UsersSettings.Domain;
using PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions;
using PeerToPeerBattleship.ConsoleApp.DependencyInjection;
using PeerToPeerBattleship.Core.Configurations;
using PeerToPeerBattleship.Core.CustomLogger;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.Core.Extensions;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
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

            //Configura os logs da aplicação
            LoggerInjection.CreateLogger();

            //Adiciona injeção de dependências
            ServiceCollection services = AddDependencyInjection(applicationSettings);
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            services = ValidateUserSettings(serviceProvider, services);
            serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<IContextualLogger<Program>>().Logger;

            var userSettings = serviceProvider.GetRequiredService<UserSettings>();

            if (!userSettings.ShowApplicationInitialDisplay) StartDisplay();

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
                var selectedProgramOption = DisplayProgramOptions(serviceProvider);

                while(selectedProgramOption != 9)
                {
                    switch (selectedProgramOption)
                    {
                        case 1:
                            var game = serviceProvider.GetRequiredService<IGame>();
                            await game.Create();

                            break;
                        case 2:
                            var modifySettingsService = serviceProvider.GetRequiredService<IModifySettingsService>();
                            modifySettingsService.ModifyUserSettings();
                            break;
                        case 9:
                            return;
                    }

                    selectedProgramOption = DisplayProgramOptions(serviceProvider);
                }


            }
            catch (Exception ex)
            {
                logger.LogExceptionError("Erro na execução do programa.", ex);
            }
        }

        private static ServiceCollection AddDependencyInjection(ApplicationSettings applicationSettings)
        {
            var services = new ServiceCollection();
            services.AddSingleton(applicationSettings);
            services.AddLogger();
            services.AddServices();
            services.AddUserSettings();

            return services;
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

        private static short DisplayProgramOptions(ServiceProvider serviceProvider)
        {
            ConsoleExtension.Clear();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|                    O que você deseja fazer agora?                     |");
            Console.WriteLine("|                              1 - Jogar                                |");
            Console.WriteLine("|                           2 - Configurações                           |");
            Console.WriteLine("|                              9 - Sair                                 |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            var userInputHandler = serviceProvider.GetRequiredService<IUserInputHandler>();
            var selectedProgramOption = userInputHandler.ReadShort("    Digite a opção desejada: ");

            if (selectedProgramOption == 1 ||
                selectedProgramOption == 2 ||
                selectedProgramOption == 9)
                return selectedProgramOption;

            Console.WriteLine("Opção não reconhecida pelo programa, tente novamente.");
            Thread.Sleep(3500);

            return DisplayProgramOptions(serviceProvider);
        }

        private static ServiceCollection ValidateUserSettings(ServiceProvider serviceProvider, ServiceCollection services)
        {
            ConsoleExtension.Clear();

            var userSettingsService = serviceProvider.GetRequiredService<IUserSettingsService>();

            services.AddSingleton(userSettingsService.GetOrCreateUserSettings());

            return services;
        }
    }
}
