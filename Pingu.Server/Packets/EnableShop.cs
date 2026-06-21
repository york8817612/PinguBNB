using Pingu.Networking;

namespace Pingu.Packets;

public class EnableShop : IPacket
{
    public void Encode(SendPacketBase buf) => buf.Encode1Bool(true);
}
