using Serilog;

namespace PeerToPeerBattleship.Core.CustomLogger
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

        public static void LogMinorExceptionError(this ILogger logger, string message, Exception ex)
        {
            logger.Error("{Message}\nExceção: {ExceptionMessage}",
                message,
                ex.Message);
        }
    }
}
