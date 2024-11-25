using PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions;
using PeerToPeerBattleship.Core.Extensions;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
using Serilog;

namespace PeerToPeerBattleship.Application.UsersSettings.Services
{
    public class ModifySettingsService : IModifySettingsService
    {
        private readonly ILogger _logger;
        private readonly IUserInputHandler _userInputHandler;

        public void ModifyUserSettings()
        {
            var selectedSettingsOption = DisplaySettingsOptions();

            while (selectedSettingsOption != 9)
            {
                switch (selectedSettingsOption)
                {
                    case 1:
                        HandleInitialDisplaySettings();
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 9:
                        return;
                }
            }
        }

        public short DisplaySettingsOptions()
        {
            ConsoleExtension.Clear();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|                         MENU DE CONFIGURAÇÕES                         |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|             Selecione a configuração que deseja alterar               |");
            Console.WriteLine("|                  1 - Display inicial do programa                      |");
            Console.WriteLine("|                2 - Tempo de expiração dos arquivos                    |");
            Console.WriteLine("|                     3 - Conexão Peer-To-Peer                          |");
            Console.WriteLine("|                 9 - Sair do menu de configuração                      |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            var selectedOption = _userInputHandler.ReadShort("  Selecione a configuração desejada: ");

            if (selectedOption == 1 ||
                selectedOption == 2 ||
                selectedOption == 3 ||
                selectedOption == 9)
                return selectedOption;

            Console.WriteLine("Opção não reconhecida pelo programa, tente novamente.");
            Thread.Sleep(3500);

            return DisplaySettingsOptions();
        }

        public short DisplayProgramDisplaySetting()
        {
            ConsoleExtension.Clear();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|              CONFIGURAÇÕES DO DISPLAY INICIAL DO PROGRAMA             |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|             Selecione a configuração que deseja alterar               |");
            Console.WriteLine("|                             1 - Exibir                                |");
            Console.WriteLine("|                           2 - Não Exibir                              |");
            Console.WriteLine("|                              9 - Sair                                 |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            var selectedOption = _userInputHandler.ReadShort("  Selecione a opção desejada desejada: ");

            if (selectedOption == 1 ||
                selectedOption == 2 ||
                selectedOption == 9)
                return selectedOption;

            Console.WriteLine("Opção não reconhecida pelo programa, tente novamente.");
            Thread.Sleep(3500);

            return DisplaySettingsOptions();
        }

        private void HandleInitialDisplaySettings()
        {
            var selectedDisplaySettingOption = DisplayProgramDisplaySetting();

            while (selectedDisplaySettingOption != 9)
            {
                switch (selectedDisplaySettingOption)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 9:
                        return;
                }
            }
        }
    }
}
