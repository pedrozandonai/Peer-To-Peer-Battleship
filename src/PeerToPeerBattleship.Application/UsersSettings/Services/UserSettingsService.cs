using PeerToPeerBattleship.Application.UsersSettings.Domain;
using PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions;
using PeerToPeerBattleship.Core.Configurations;
using PeerToPeerBattleship.Core.CustomLogger;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using PeerToPeerBattleship.Core.Helpers;
using Serilog;
using System.Text.Json;

namespace PeerToPeerBattleship.Application.UsersSettings.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettings _applicationSettings;

        public UserSettingsService(IContextualLogger<UserSettingsService> contextualLogger, ApplicationSettings applicationSettings)
        {
            _logger = contextualLogger.Logger;
            _applicationSettings = applicationSettings;
        }

        public bool UserSettingsAlreadyExists()
        {
            var filePath = GetDefaultSettingsFileLocation();

            if (File.Exists(filePath))
            {
                _logger.Information("Configuração do usuário encontrada em: {0}", filePath);
                return true;
            }

            _logger.Warning("Nenhuma configuração encontrada em: {0}", filePath);
            return false;
        }


        private static string GetDefaultSettingsFileLocation()
        {
            return UserSettingsPathHelper.GetFilePath();
        }

        public UserSettings CreateAndSaveDefaultUserSettings()
        {
            _logger.Information("Iniciando processo de criação do arquivo de configuração padrão e salvando-o.");

            var userSettings = CreateDefaultSettings();

            SaveUserSettings(userSettings);

            return userSettings;
        }

        private UserSettings CreateDefaultSettings()
        {
            _logger.Information("Criando novo arquivo de configuração.");

            UserSettings userSettings =
                new(
                    false,
                    new MatchExpiresIn(15, "MINUTOS"),
                    new Connection(10));

            _logger.Information("Arquivo de configuração padrão criado com sucesso.");

            return userSettings;
        }

        public void SaveUserSettings(UserSettings userSettings)
        {
            string folderPath = UserSettingsPathHelper.GetFolderPath();
            string filePath = UserSettingsPathHelper.GetFilePath();

            _logger.Information("Salvando arquivo de configuração...");

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    _logger.Information("Diretório criado: {0}", folderPath);
                }

                var serializedUserSettings = SerializeUserSettings(userSettings);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.Information("Arquivo existente foi deletado: {0}", filePath);
                }

                File.WriteAllText(filePath, serializedUserSettings);
                _logger.Information("Arquivo salvo com sucesso em: {0}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogExceptionError("Erro ao salvar configurações do usuário.", ex);
                throw;
            }
        }

        private string SerializeUserSettings(UserSettings userSettings)
        {
            _logger.Information("Serializando arquivo de configuração...");

            string jsonContent = string.Empty;
            try
            {
                jsonContent = JsonSerializer.Serialize(userSettings, _applicationSettings.JsonSerializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogExceptionError("Erro ao tentar serializar o arquivo de configuração.", ex);
                throw;
            }

            _logger.Information("Arquivo serializado com sucesso.");

            return jsonContent;
        }

        public UserSettings GetOrCreateUserSettings()
        {
            string filePath = GetDefaultSettingsFileLocation();

            UserSettings? userSettings = null;
            if (UserSettingsAlreadyExists())
            {
                _logger.Information("Arquivo de configuração já existe e será carregado para o programa.");
                string serializedUserSettings = File.ReadAllText(filePath);
                userSettings = DeserializeUserSettings(serializedUserSettings);
            }
            else
            {
                userSettings = CreateAndSaveDefaultUserSettings();
            }

            return userSettings!;
        }

        private UserSettings? DeserializeUserSettings(string serializedUserSettings)
        {
            if (string.IsNullOrWhiteSpace(serializedUserSettings))
            {
                _logger.Error("String desserializada está nula ou em branco.");
                return null;
            }

            UserSettings? userSettings;
            try
            {
                userSettings = JsonSerializer.Deserialize<UserSettings>(serializedUserSettings, _applicationSettings.JsonSerializerOptions);
            }
            catch(Exception ex)
            {
                _logger.LogExceptionError("Erro ao tentar desserializar arquivo de configuração do usuário.", ex);
                throw;
            }

            return userSettings;
        }
    }
}
