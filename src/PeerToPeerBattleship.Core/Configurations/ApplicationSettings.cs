namespace PeerToPeerBattleship.Core.Configurations
{
    public class ApplicationSettings
    {
        public string AppName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool GameTestMode { get; set; }
        public bool PeerToPeerTestMode { get; set; }
        public bool IsProductionEnvironment { get; set; }
        public MatchExpiresIn MatchExpiresIn { get; set; } = new();
        public bool SkipApplicationLogo { get; set; }
        public Connection Connection { get; set; } = new();

        public void VerifySettings()
        {
            if (GameTestMode &&
                PeerToPeerTestMode)
                throw new NotSupportedException("Não é possível ter o modo de teste de jogo e conexão ativos ao mesmo tempo.");

            MatchExpiresIn.Validate();
        }
    }

    public class MatchExpiresIn
    {
        public int Value { get; set; }
        public string Time { get; set; } = string.Empty;

        private readonly string[] ValidTimes = ["years", "months", "days", "hours", "minutes", "seconds"];

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Time))
                throw new ArgumentException("Configuração de tempo de expiração dos arquivos da partida não pode ficar em branco.");

            if (!ValidTimes.Any(time => time.Equals(Time, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Não foi possível configurar o vencimento dos arquivos da partida.");
        }

        public TimeSpan GetMatchExpirationDuration()
        {
            return Time.ToLower() switch
            {
                "seconds" => TimeSpan.FromSeconds(Value),
                "minutes" => TimeSpan.FromMinutes(Value),
                "hours" => TimeSpan.FromHours(Value),
                "days" => TimeSpan.FromDays(Value),
                "months" => TimeSpan.FromDays(Value * 30),
                "years" => TimeSpan.FromDays(Value * 365),
                _ => throw new ArgumentException("Unidade de tempo inválida para expiração.")
            };
        }
    }

    public class Connection
    {
        public int MaxRetriesAmount { get; set; }
    }
}
