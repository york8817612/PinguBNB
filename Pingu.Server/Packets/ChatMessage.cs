using Pingu.Networking;
using System.Reflection;

namespace Pingu.Packets;

public class ChatMessage(string sender, string message) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.EncodeStr($"{sender} : {message}");
    }
}
