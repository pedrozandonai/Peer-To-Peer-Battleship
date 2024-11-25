using PeerToPeerBattleship.Application.UsersSettings.Domain;

namespace PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions
{
    public interface ISaveSettingsService
    {
        void SaveUserSettings(UserSettings userSettings);
        bool VerifyIfUserAlreadyHasSettings();
    }
}
