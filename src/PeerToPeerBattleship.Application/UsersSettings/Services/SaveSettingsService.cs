using PeerToPeerBattleship.Application.UsersSettings.Domain;
using PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions;
using System.Text.Json;

namespace PeerToPeerBattleship.Application.UsersSettings.Services
{
    public class SaveSettingsService : ISaveSettingsService
    {
        public string GetDefaultSettingsFileLocation()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = Path.Combine(documentsPath, "PeerToPeerBattleShip/settings");
            string fileName = "UserSettings";
            string filePath = Path.Combine(folderPath, fileName);

            return fileName;
        }

        public bool VerifyIfUserAlreadyHasSettings()
        {
            var filePath = GetDefaultSettingsFileLocation();

            if (File.Exists(filePath)) return true;

            return false;
        }

        public void SaveUserSettings(UserSettings userSettings)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = Path.Combine(documentsPath, "PeerToPeerBattleShip/settings");

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var serializedUserSettings = SerializeUserSettings(userSettings);

            string fileName = "UserSettings";
            string filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath)) File.Delete(filePath);

            File.WriteAllText(filePath, serializedUserSettings);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("O arquivo de configuração padrão não pode ser criado.");
        }

        private string SerializeUserSettings(UserSettings userSettings)
        {
            string jsonContent = JsonSerializer.Serialize(userSettings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return jsonContent;
        }
    }
}
