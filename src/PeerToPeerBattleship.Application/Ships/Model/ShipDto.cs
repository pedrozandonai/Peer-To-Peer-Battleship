using System.Text.Json.Serialization;

namespace PeerToPeerBattleship.Application.Ships.Model
{
    public class ShipDto
    {
        public string Tipo { get; set; }
        public List<List<int>> Posicoes { get; set; }

        [JsonConstructor]
        public ShipDto(string tipo, List<List<int>> posicoes)
        {
            Tipo = tipo;
            Posicoes = posicoes;
        }
    }
}
