using System.Text.Json.Serialization;

namespace PeerToPeerBattleship.Application.UsersSettings.Domain
{
    public class UserSettings
    {
        public bool ShowApplicationInitialDisplay { get; set; }
        public MatchExpiresIn MatchExpiresIn { get; set; }
        public Connection Connection { get; set; }

        [JsonConstructor]
        public UserSettings(bool showApplicationInitialDisplay, MatchExpiresIn matchExpiresIn, Connection connection)
        {
            ShowApplicationInitialDisplay = showApplicationInitialDisplay;
            MatchExpiresIn = matchExpiresIn;
            Connection = connection;
        }

        public UserSettings Clone()
        {
            return new UserSettings(ShowApplicationInitialDisplay,
                new MatchExpiresIn(MatchExpiresIn.Value, MatchExpiresIn.Time),
                new Connection(Connection.MaxRetriesAmount));
        }
    }

    public class MatchExpiresIn
    {
        public int Value { get; set; }
        public string Time { get; set; }

        private readonly string[] ValidTimesMesurements = ["ANOS", "MESES", "DIAS", "HORAS", "MINUTOS", "SEGUNDOS"];

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

            if (!ValidTimesMesurements.Any(time => time.Equals(Time, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Não foi possível configurar o vencimento dos arquivos da partida.");
        }

        public TimeSpan GetMatchExpirationDuration()
        {
            return Time switch
            {
                "SEGUNDOS" => TimeSpan.FromSeconds(Value),
                "MINUTOS" => TimeSpan.FromMinutes(Value),
                "HORAS" => TimeSpan.FromHours(Value),
                "DIAS" => TimeSpan.FromDays(Value),
                "MESES" => TimeSpan.FromDays(Value * 30),
                "ANOS" => TimeSpan.FromDays(Value * 365),
                _ => throw new ArgumentException("Unidade de tempo inválida para expiração.")
            };
        }

        public string[] GetValidTimeMesurements()
        {
            return ValidTimesMesurements;
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
