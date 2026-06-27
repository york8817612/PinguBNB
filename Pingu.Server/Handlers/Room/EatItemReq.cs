using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class EatItemReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var room = client.CurrentRoom;
        if (room == null) return;

        int slotId = data.Decode1();
        int itemId = data.Decode1();
        int itemType = data.Decode1();

        if (ServerConfig.IsJP)
        {
            int v4 = data.Decode1();
            room.Broadcast(new EatItemResult(slotId, itemId, itemType, v4));
        }
        else
        {
            room.Broadcast(new EatItemResult(slotId, itemId, itemType));
        }
    }
}