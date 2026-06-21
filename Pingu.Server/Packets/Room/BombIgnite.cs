using Pingu.Networking;

namespace Pingu.Packets.Game;

public class BombIgnite(int slotId, int pos, int bombAttr, int bombId, int unk) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(slotId);
        if (ServerConfig.IsJP)
            buf.Encode1(unk);
        buf.Encode1(pos);
        buf.Encode1(bombAttr);
        buf.Encode2(bombId);
    }
}
