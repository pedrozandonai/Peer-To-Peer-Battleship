using System.Text.Json.Serialization;

namespace PeerToPeerBattleship.Application.UsersSettings.Domain
{
    public class UserSettings
    {
        public bool SkipApplicationLogo { get; set; }
        public MatchExpiresIn MatchExpiresIn { get; set; }
        public Connection Connection { get; set; }

        [JsonConstructor]
        public UserSettings(bool skipApplicationLogo, MatchExpiresIn matchExpiresIn, Connection connection)
        {
            SkipApplicationLogo=skipApplicationLogo;
            MatchExpiresIn=matchExpiresIn;
            Connection=connection;
        }
    }

    public class MatchExpiresIn
    {
        public int Value { get; set; }
        public string Time { get; set; } = string.Empty;

        private readonly string[] ValidTimes = ["years", "months", "days", "hours", "minutes", "seconds"];

        [JsonConstructor]
        public MatchExpiresIn(int value, string time)
        {
            Value=value;
            Time=time;
        }

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

        [JsonConstructor]
        public Connection(int maxRetriesAmount)
        {
            MaxRetriesAmount=maxRetriesAmount;
        }
    }
}
