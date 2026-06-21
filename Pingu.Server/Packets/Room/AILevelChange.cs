using Pingu.Networking;

namespace Pingu.Packets.Game;

public class AILevelChange(int slotIdx, int aiLevel) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(slotIdx);
        buf.Encode2(aiLevel);
    }
}
