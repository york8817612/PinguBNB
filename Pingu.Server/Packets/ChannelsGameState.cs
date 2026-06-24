using Pingu.Networking;

namespace Pingu.Packets;

public class ChannelsGameState : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        int availableSize = ServerConfig.ChannelCount;
        buf.Encode1(availableSize);
        for (int i = 0; i < availableSize; i++)
        {
            buf.Encode1(10);
            buf.Encode1(1);
            int chVer = ServerConfig.IsCN && ServerConfig.Ver == 12 ? 24 : ServerConfig.Ver;
            buf.Encode2(chVer);
            buf.Encode2(3);
        }
    }
}