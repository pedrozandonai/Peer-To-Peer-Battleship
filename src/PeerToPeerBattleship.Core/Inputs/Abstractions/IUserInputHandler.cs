namespace PeerToPeerBattleship.Core.Inputs.Abstractions
{
    public interface IUserInputHandler
    {
        int ReadInt(string inputMessage);
        short ReadShort(string inputMessage);
        string ReadIpAddress(string inputMessage);
        (int X, int Y) ReadPositions(string inputMessage);
    }
}
