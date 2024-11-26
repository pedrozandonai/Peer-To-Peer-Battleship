using System.Text.Json;

namespace PeerToPeerBattleship.Core.Configurations
{
    public class ApplicationSettings
    {
        public string AppName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool GameTestMode { get; set; }
        public bool PeerToPeerTestMode { get; set; }
        public bool IsProductionEnvironment { get; set; }
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();

        public void VerifySettings()
        {
            if (GameTestMode &&
                PeerToPeerTestMode)
                throw new NotSupportedException("Não é possível ter o modo de teste de jogo e conexão ativos ao mesmo tempo.");
        }
    }
}
