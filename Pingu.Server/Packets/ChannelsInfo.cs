using Pingu.Networking;

namespace Pingu.Packets;

public class ChannelsInfo : IPacket
{
    public void Encode(SendPacketBase buf) => buf.Encode1Bool(false);
}
