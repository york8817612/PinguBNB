using Pingu.Networking;

namespace Pingu.Packets;

public class UnkResult : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(1);
        buf.Encode1(0);
        buf.Encode4(0);
        buf.Encode4(0x7F000001);
        buf.Encode2(3838);
    }
}
