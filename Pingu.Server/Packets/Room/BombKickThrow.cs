using Pingu.Networking;

namespace Pingu.Packets.Game;

public class BombKickThrow(int slotId, int bombId, int targetPos) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(slotId);
        buf.Encode2(bombId);
        buf.Encode1(targetPos);
    }
}
