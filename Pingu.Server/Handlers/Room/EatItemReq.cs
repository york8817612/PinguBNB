using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class EatItemReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int slotId = data.Decode1(ref off);
        int itemId = data.Decode1(ref off);
        int itemType = data.Decode1(ref off);

        if (ServerConfig.IsJP)
        {
            int v4 = data.Decode1(ref off);
            Room.Broadcast(new EatItemResultJP(slotId, itemId, itemType, v4));
        }
        else
        {
            Room.Broadcast(new EatItemResult(slotId, itemId, itemType));
        }
    }
}
