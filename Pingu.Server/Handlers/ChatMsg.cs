using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class ChatMsg : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var msg = data.DecodeStr();

        client.BroadcastPacket(new ChatMessage(client.Users[0].Name, msg));
    }
}
