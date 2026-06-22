using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class AJoinSessionReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var s1 = data.Decode1();
        var s2 = data.Decode1();

        //client.BroadcastPacket(new ChatMessage(client.Users[0].Name, msg));
    }
}
