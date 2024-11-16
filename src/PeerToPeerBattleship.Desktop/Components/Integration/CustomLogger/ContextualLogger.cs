using Serilog;
using PeerToPeerBattleship.Desktop.Components.Integration.CustomLogger.Abstraction;

namespace PeerToPeerBattleship.Desktop.Components.Integration.CustomLogger
{
    public class ContextualLogger<T> : IContextualLogger<T>
    {
        public ILogger Logger { get; }

        public ContextualLogger()
        {
            Logger = Log.ForContext<T>();
        }
    }
}
