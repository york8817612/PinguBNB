using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class OpenCloseSlotReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        var msg = data.Decode1(ref off);

        //client.BroadcastPacket(new ChatMessage(client.Users[0].Name, msg));
    }
}
