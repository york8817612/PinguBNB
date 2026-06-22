using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class OpenCloseSlotReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var msg = data.Decode1();

        //client.BroadcastPacket(new ChatMessage(client.Users[0].Name, msg));
    }
}
