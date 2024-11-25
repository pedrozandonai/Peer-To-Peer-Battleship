using PeerToPeerBattleship.Application.UsersSettings.Domain;

namespace PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions
{
    public interface ICreateSettingsService
    {
        UserSettings CreateDefaultSettings();
    }
}
