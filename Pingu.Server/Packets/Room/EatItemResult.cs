using Pingu.Networking;

namespace Pingu.Packets.Game;

public class EatItemResult(int slotId, int itemId, int itemType, int v4 = 0) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(slotId);

        if (ServerConfig.IsJP)
        {
            buf.Encode1(itemType);
            buf.Encode1(v4);
            buf.Encode1(itemId);
            buf.Encode1(0);
        }
        else
        {
            buf.Encode1(itemId);
            buf.Encode1(itemType);
        }
    }
}
