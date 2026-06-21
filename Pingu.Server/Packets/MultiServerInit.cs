using Pingu.Networking;

namespace Pingu.Packets;

public class MultiServerInit : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1Bool(false);
        buf.Encode1Bool(false);
    }
}
