using Pingu.Networking;

namespace Pingu.Packets;

public class ResConnectToSvr(int res, int channelId, int unk) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(res);
        if (res == 1)
        {
            buf.Encode1(unk);
            buf.Encode4(channelId);
            buf.Encode4(Codec.LoopbackIp);
            int targetPort = unk == 11
                ? 4848 + ServerConfig.ChannelCount
                : 4848 + channelId;
            buf.Encode2(targetPort);
        }
    }
}