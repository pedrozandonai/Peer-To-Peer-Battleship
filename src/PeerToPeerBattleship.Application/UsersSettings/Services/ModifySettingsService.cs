using PeerToPeerBattleship.Application.UsersSettings.Domain;
using PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions;
using PeerToPeerBattleship.Core.CustomLogger;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.Core.Extensions;
using PeerToPeerBattleship.Core.Inputs.Abstractions;
using Serilog;

namespace PeerToPeerBattleship.Application.UsersSettings.Services
{
    public class ModifySettingsService : IModifySettingsService
    {
        private readonly ILogger _logger;
        private readonly IUserInputHandler _userInputHandler;
        private readonly IUserSettingsService _userSettingsService;

        public UserSettings UsersSettings { get; set; }
        private readonly UserSettings _oldUserSettings;

        public ModifySettingsService(IContextualLogger<ModifySettingsService> contextualLogger, IUserInputHandler userInputHandler, UserSettings usersSettings, IUserSettingsService userSettingsService)
        {
            _logger = contextualLogger.Logger;
            _userInputHandler = userInputHandler;
            UsersSettings = usersSettings;
            _userSettingsService = userSettingsService;
            _oldUserSettings = usersSettings.Clone();
        }

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
                        HandleMatchFileExpirationSettings();
                        break;
                    case 3:
                        HandleConnectionSettings();
                        break;
                    case 9:
                        return;
                }

                selectedSettingsOption = DisplaySettingsOptions();
            }

            if (!UsersSettings.Equals(_oldUserSettings))
            {
                var changeSettingsOption = DisplayChangedSettings();
                if (changeSettingsOption == 1)
                {
                    _userSettingsService.SaveUserSettings(UsersSettings);
                }
                else
                {
                    _logger.Information("Desfazendo as alterações...");
                    UsersSettings = _oldUserSettings;
                }
            }
        }

        private short DisplayChangedSettings()
        {
            ConsoleExtension.Clear();

            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|                     CONFIGURAÇÕES ALTERADAS                           |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            if (_oldUserSettings.ShowApplicationInitialDisplay != UsersSettings.ShowApplicationInitialDisplay)
            {
                Console.WriteLine("|                  DISPLAY INICIAL DO PROGRAMA                          |");
            }
            if (_oldUserSettings.MatchExpiresIn.Value != UsersSettings.MatchExpiresIn.Value)
            {
                Console.WriteLine("|              VALOR DE TEMPO DE EXPIRAÇÃO DE UMA PARTIDA               |");
            }
            if (_oldUserSettings.MatchExpiresIn.Time != UsersSettings.MatchExpiresIn.Time)
            {
                Console.WriteLine("|             UNIDADE DE TEMPO DE EXPIRAÇÃO DE UMA PARTIDA              |");
            }
            if (_oldUserSettings.Connection.MaxRetriesAmount != UsersSettings.Connection.MaxRetriesAmount)
            {
                Console.WriteLine("|        QUANTIDADE DE TENTATIVAS DE RECONEXÃO EM CASO DE FALHA         |");
            }
            Console.WriteLine("*-----------------------------------------------------------------------*");

            Console.WriteLine();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|            DESEJA ALTERAR AS CONFIGURAÇÕES DESCRITAS ACIMA?           |");
            Console.WriteLine("|                               1 - Sim                                 |");
            Console.WriteLine("|                               2 - Não                                 |");
            Console.WriteLine("*-----------------------------------------------------------------------*");

            var selectedOption = _userInputHandler.ReadShort("Selecione uma opção: ");

            if (selectedOption == 1 ||
                selectedOption == 2)
                return selectedOption;

            return DisplayChangedSettings();
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
            Console.WriteLine("|                          1 - Exibir (false)                           |");
            Console.WriteLine("|                        2 - Não Exibir (true)                          |");
            Console.WriteLine("|                              9 - Sair                                 |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine(string.Format("VALOR ATUAL: {0}", UsersSettings.ShowApplicationInitialDisplay));
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
                    case 1: //Exibir display inicial
                        UsersSettings.ShowApplicationInitialDisplay = false;
                        break;
                    case 2: //Não exibir display inicial
                        UsersSettings.ShowApplicationInitialDisplay = true;
                        break;
                    case 9: //Sair
                        return;
                }

                selectedDisplaySettingOption = DisplayProgramDisplaySetting();
            }
        }

        private void HandleMatchFileExpirationSettings()
        {
            var selectedMatchFileExpirationSettings = DisplayMatchFileExpirationSettings(false);

            while (selectedMatchFileExpirationSettings != 9)
            {
                switch (selectedMatchFileExpirationSettings)
                {
                    case 1: //Exibe configuração do valor da expiração de arquivos
                        var timeValueConfig = DisplayMatchFileExpirationValueSetting();
                        if (timeValueConfig == 0)
                            break;

                        UsersSettings.MatchExpiresIn.Value = timeValueConfig;
                        break;

                    case 2: //Exibe a configuração do valor da medida de tempo de expiração dos arquivos
                        var timeConfig = DisplayMatchFileExpirationTimeSetting();
                        if (timeConfig.Equals("0"))
                            break;

                        UsersSettings.MatchExpiresIn.Time = timeConfig;
                        break;

                    case 9: //Sair
                        return;
                }

                try
                {
                    UsersSettings.MatchExpiresIn.GetMatchExpirationDuration();
                }
                catch (Exception ex)
                {
                    _logger.LogExceptionError("Configuração inválida detectada, por favor, altere-a para continuar.", ex);
                    selectedMatchFileExpirationSettings = DisplayMatchFileExpirationSettings(true);
                }

                selectedMatchFileExpirationSettings = DisplayMatchFileExpirationSettings(false);
            }
        }

        private short DisplayMatchFileExpirationSettings(bool withoutExitOption)
        {
            ConsoleExtension.Clear();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|   CONFIGURAÇÕES DO TEMPO DE EXPIRAÇÃO DAS DOS ARQUIVOS DAS PARTIDAS   |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|             Selecione a configuração que deseja alterar               |");
            Console.WriteLine("|                             1 - Valor                                 |");
            Console.WriteLine("|                         2 - Medida de tempo                           |");
            if(!withoutExitOption) Console.WriteLine("|                             9 - Sair                                  |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine(string.Format("VALOR ATUAL: {0} {1}", UsersSettings.MatchExpiresIn.Value, UsersSettings.MatchExpiresIn.Time));

            short selectedOption;
            do
            {
                selectedOption = _userInputHandler.ReadShort("  Selecione a opção desejada: ");

                if (withoutExitOption)
                {
                    if (selectedOption == 1 || selectedOption == 2)
                        return selectedOption;

                    Console.WriteLine("Opção inválida. Escolha entre 1 e 2.");
                }
                else
                {
                    if (selectedOption == 1 || selectedOption == 2 || selectedOption == 9)
                        return selectedOption;

                    Console.WriteLine("Opção inválida. Escolha entre 1, 2 ou 9.");
                }

                Thread.Sleep(1500);
            }
            while (true);
        }

        private int DisplayMatchFileExpirationValueSetting()
        {
            ConsoleExtension.Clear();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|      CONFIGURAÇÕES DO VALOR DE TEMPO DE EXPIRAÇÃO DE UMA PARTIDA      |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|              DIGITE APENAS VALORES MAIORES OU IGUAIS À 1              |");
            Console.WriteLine("|                          DIGITE 0 PARA SAIR                           |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine(string.Format("VALOR ATUAL: {0}", UsersSettings.MatchExpiresIn.Value));
            var valueSetting = _userInputHandler.ReadInt("  Selecione um valor desejado para a expiração dos arquivos: ");

            if (valueSetting >= 1 ||
                valueSetting == 0)
                return valueSetting;

            Console.WriteLine("Digite apenas valores maiores ou igual à 1.");
            Thread.Sleep(3500);

            return DisplaySettingsOptions();
        }

        private string DisplayMatchFileExpirationTimeSetting()
        {
            ConsoleExtension.Clear();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|     CONFIGURAÇÕES DO UNIDADE DE TEMPO DE EXPIRAÇÃO DE UMA PARTIDA     |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|      VALORES ACEITOS: ANOS, MESES, DIAS, HORAS, MINUTOS, SEGUNDOS     |");
            Console.WriteLine("|                          DIGITE 0 PARA SAIR                           |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine(string.Format("VALOR ATUAL: {0}", UsersSettings.MatchExpiresIn.Time));
            Console.Write("  Selecione um valor desejado para a medida de tempo da expiração de arquivos arquivos: ");
            string? timeMesurementSetting = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(timeMesurementSetting))
            {
                Thread.Sleep(3500);
                Console.WriteLine("Valor invalido, por favor, tente novamente ou digite 0 para sair.");
                return DisplayMatchFileExpirationTimeSetting();
            }

            if (timeMesurementSetting.Trim().Equals("0", StringComparison.InvariantCultureIgnoreCase))
                return "0";

            foreach (var time in UsersSettings.MatchExpiresIn.GetValidTimeMesurements())
            {
                if (timeMesurementSetting.Trim().Equals(time, StringComparison.InvariantCultureIgnoreCase))
                {
                    return timeMesurementSetting.ToUpper();
                }
            }

            Thread.Sleep(3500);
            Console.WriteLine("Valor invalido, por favor, tente novamente ou digite 0 para sair.");
            return DisplayMatchFileExpirationTimeSetting();
        }

        private void HandleConnectionSettings()
        {
            var selectedConnectionSettings = DisplayConnectionSettings();

            while (selectedConnectionSettings != 9)
            {
                switch (selectedConnectionSettings)
                {
                    case 1: //Exibe configuração da quantidade de tentativas para realizar a reconexão
                        var maxRetriesAmount = DisplayConnectionMaxRetriesAmountSettings();
                        if (maxRetriesAmount == -1)
                            break;

                        UsersSettings.Connection.MaxRetriesAmount = maxRetriesAmount;

                        break;

                    case 2: //Exibe a configuração da porta padrão utilizada nas conexões.
                        var defaultPort = DisplayConnectionPortSettings();
                        if (defaultPort == -1)
                            break;

                        UsersSettings.Connection.Port = defaultPort;

                        break;
                    case 9: //Sair
                        return;
                }

                selectedConnectionSettings = DisplayConnectionSettings();
            }
        }

        private short DisplayConnectionSettings()
        {
            ConsoleExtension.Clear();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|                 CONFIGURAÇÕES DA CONEXÃO PEER-TO-PEER                 |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|              1 - Tentativas de reconexão em caso de falha             |");
            Console.WriteLine("|                 2 - Definir porta padrão para conexões                |");
            Console.WriteLine("|                                9 - Sair                               |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            var connectionSettingsOption = _userInputHandler.ReadShort("  Escolha uma opção: ");

            if (connectionSettingsOption == 1 ||
                connectionSettingsOption == 2 ||
                connectionSettingsOption == 9)
                return connectionSettingsOption;

            Console.WriteLine("Opção não reconhecida pelo programa, por favor, tente novamente.");
            Thread.Sleep(3500);

            return DisplayConnectionSettings();
        }

        private int DisplayConnectionMaxRetriesAmountSettings()
        {
            ConsoleExtension.Clear();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("| CONFIGURAÇÃO DA QUANTIDADE DE TENTATIVAS DE RECONEXÃO EM CASO DE FALHA|");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|                  INSIRA UM VALOR MAIOR OU IGUAL A 0                   |");
            Console.WriteLine("|                        OU DIGITE -1 PARA SAIR                         |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine(string.Format("VALOR ATUAL: {0}", UsersSettings.Connection.MaxRetriesAmount));
            var maxRetriesAmountConfig = _userInputHandler.ReadInt("  Digite o valor desejado: ");

            if (maxRetriesAmountConfig >= 0 ||
                maxRetriesAmountConfig == -1)
                return maxRetriesAmountConfig;

            Console.WriteLine("Opção não reconhecida pelo programa, por favor, tente novamente.");
            Thread.Sleep(3500);

            return DisplayConnectionSettings();
        }

        private short DisplayConnectionPortSettings()
        {
            ConsoleExtension.Clear();
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|               CONFIGURAÇÃO DA PORTA PADRÃO DAS CONEXÕES               |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine("|                  INSIRA UM VALOR MAIOR OU IGUAL A 0                   |");
            Console.WriteLine("|                        OU DIGITE -1 PARA SAIR                         |");
            Console.WriteLine("*-----------------------------------------------------------------------*");
            Console.WriteLine(string.Format("VALOR ATUAL: {0}", UsersSettings.Connection.Port));
            var maxRetriesAmountConfig = _userInputHandler.ReadShort("  Digite o valor desejado: ");

            if (maxRetriesAmountConfig >= 0 ||
                maxRetriesAmountConfig == -1)
                return maxRetriesAmountConfig;

            Console.WriteLine("Opção não reconhecida pelo programa, por favor, tente novamente.");
            Thread.Sleep(3500);

            return DisplayConnectionSettings();
        }
    }
}
