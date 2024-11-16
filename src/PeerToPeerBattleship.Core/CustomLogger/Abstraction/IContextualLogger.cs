using Serilog;

namespace PeerToPeerBattleship.Core.CustomLogger.Abstraction
{
    public interface IContextualLogger<T>
    {
        ILogger Logger { get; }
    }
}
