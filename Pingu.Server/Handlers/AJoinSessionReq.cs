using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class AJoinSessionReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        var s1 = data.Decode1(ref off);
        var s2 = data.Decode1(ref off);

        //client.BroadcastPacket(new ChatMessage(client.Users[0].Name, msg));
    }
}
