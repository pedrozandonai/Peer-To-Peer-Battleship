using Serilog;
using PeerToPeerBattleship.Core.CustomLogger.Abstraction;

namespace PeerToPeerBattleship.Core.CustomLogger
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
