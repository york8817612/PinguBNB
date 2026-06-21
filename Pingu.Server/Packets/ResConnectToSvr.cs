using Pingu.Networking;

namespace Pingu.Packets;

public class ResConnectToSvr(int res, int channelId) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(res);
        if (res == 1)
        {
            buf.Encode1(channelId);
            buf.Encode4(0);
            buf.Encode4(0x7F000001);
            buf.Encode2(channelId != 11 ? 4848 : 4849);
        }
    }
}
