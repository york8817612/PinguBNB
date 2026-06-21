using Pingu.Networking;

namespace Pingu.Packets;

public class ChannelsState : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        int size = !ServerConfig.IsJP ? 5 : 1;
        buf.Encode2(size);
        for (int i = 0; i < size; i++)
            buf.Encode2(0);
    }
}
