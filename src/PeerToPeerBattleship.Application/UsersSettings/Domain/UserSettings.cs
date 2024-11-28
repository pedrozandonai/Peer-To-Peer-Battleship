using System.Text.Json.Serialization;

namespace PeerToPeerBattleship.Application.UsersSettings.Domain
{
    public class UserSettings
    {
        public bool ShowApplicationInitialDisplay { get; set; } = true;
        public MatchExpiresIn MatchExpiresIn { get; set; } = new MatchExpiresIn();
        public Connection Connection { get; set; } = new Connection();

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
                new Connection(Connection.MaxRetriesAmount, Connection.Port));
        }

        public UserSettings()
        {
        }

        public bool Equals(UserSettings other)
        {
            if (other == null) return false;

            return ShowApplicationInitialDisplay == other.ShowApplicationInitialDisplay &&
                   MatchExpiresIn.Equals(other.MatchExpiresIn) &&
                   Connection.Equals(other.Connection);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UserSettings);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                ShowApplicationInitialDisplay,
                MatchExpiresIn,
                Connection
            );
        }
    }

    public class MatchExpiresIn
    {
        public int Value { get; set; } = 15;
        public string Time { get; set; } = "MINUTOS";

        private readonly string[] ValidTimesMesurements = ["ANOS", "MESES", "DIAS", "HORAS", "MINUTOS", "SEGUNDOS"];

        [JsonConstructor]
        public MatchExpiresIn(int value, string time)
        {
            Value=value;
            Time=time;
        }

        public MatchExpiresIn()
        {
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
        public int MaxRetriesAmount { get; set; } = 10;
        public short Port { get; set; } = 8080;

        [JsonConstructor]
        public Connection(int maxRetriesAmount, short port)
        {
            MaxRetriesAmount = maxRetriesAmount;
            Port = port;
        }

        public Connection()
        {
        }
    }
}
