using Serilog;

namespace PeerToPeerBattleship.Desktop.Components.Integration.CustomLogger
{
    public static class CustomLogExtension
    {
        public static void LogExceptionError(this ILogger logger, string message, Exception ex)
        {
            logger.Error("{Message}\nExceção: {ExceptionMessage}\nStack Trace: {StackTrace}\nInner Exception: {InnerException}",
                message,
                ex.Message,
                ex.StackTrace,
                ex.InnerException?.Message ?? "Nenhuma");
        }
    }
}
