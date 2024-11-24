namespace PeerToPeerBattleship.Core.Configurations
{
    public class ApplicationSettings
    {
        public string AppName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool GameTestMode { get; set; }
        public bool PeerToPeerTestMode { get; set; }

        public void VerifySettings()
        {
            if (GameTestMode == true &&
                PeerToPeerTestMode == true)
                throw new NotSupportedException("Não é possível ter o modo de teste de jogo e conexão ativos ao mesmo tempo.");
        }
    }
}
