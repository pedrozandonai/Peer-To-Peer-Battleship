namespace PeerToPeerBattleship.Core.Extensions
{
    public static class ConsoleExtension
    {
        public static void Clear()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            Console.SetCursorPosition(0, 0);
        }

        public static async Task ClearAsync()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            await Task.Delay(100);
            Console.SetCursorPosition(0, 0);
        }
    }
}
