using Pingu.Networking;

namespace Pingu.Packets;

public class ResEstimateRTT(int sequence, int time) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode4(sequence);
        buf.Encode4(time);
    }
}
