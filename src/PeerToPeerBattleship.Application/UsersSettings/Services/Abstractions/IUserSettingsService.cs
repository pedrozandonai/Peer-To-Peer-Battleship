using PeerToPeerBattleship.Application.UsersSettings.Domain;

namespace PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions
{
    public interface IUserSettingsService
    {
        bool UserSettingsAlreadyExists();
        UserSettings CreateAndSaveDefaultUserSettings();
        UserSettings GetOrCreateUserSettings();
        void SaveUserSettings(UserSettings userSettings);
    }
}
