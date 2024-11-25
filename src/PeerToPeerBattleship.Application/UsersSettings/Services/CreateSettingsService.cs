using PeerToPeerBattleship.Application.UsersSettings.Domain;
using PeerToPeerBattleship.Application.UsersSettings.Services.Abstractions;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;
using Serilog;

namespace PeerToPeerBattleship.Application.UsersSettings.Services
{
    public class CreateSettingsService : ICreateSettingsService
    {
        private readonly ILogger _logger;

        public CreateSettingsService(IContextualLogger<CreateSettingsService> contextualLogger)
        {
            _logger = contextualLogger.Logger;
        }

        public UserSettings CreateDefaultSettings()
        {
            UserSettings userSettings =
                new(
                    skipApplicationLogo: false,
                    new MatchExpiresIn(value: 15, time: "minutes"),
                    new Connection(maxRetriesAmount: 10));

            _logger.Information("Arquivo de configuração padrão criado com sucesso.");

            return userSettings;
        }
    }
}
