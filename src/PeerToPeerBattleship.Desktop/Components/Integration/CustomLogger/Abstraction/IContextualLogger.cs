using Serilog;

namespace PeerToPeerBattleship.Desktop.Components.Integration.CustomLogger.Abstraction
{
    public interface IContextualLogger<T>
    {
        ILogger Logger { get; }
    }
}
