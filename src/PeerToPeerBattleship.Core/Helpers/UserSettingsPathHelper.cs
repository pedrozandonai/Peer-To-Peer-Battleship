namespace PeerToPeerBattleship.Core.Helpers
{
    public static class UserSettingsPathHelper
    {
        private const string SettingsFolder = "PeerToPeerBattleShip/settings";
        private const string SettingsFileName = "UserSettings";

        public static string GetFolderPath()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsPath, SettingsFolder);
        }

        public static string GetFilePath()
        {
            return Path.Combine(GetFolderPath(), SettingsFileName);
        }
    }
}
