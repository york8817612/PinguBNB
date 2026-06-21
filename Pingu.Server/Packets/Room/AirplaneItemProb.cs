using Pingu.Networking;

namespace Pingu.Packets.Game;

public class AirplaneItemProb : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode4(60000);
        buf.Encode4(22000);
        buf.Encode1(4);
        buf.Encode1(0); buf.Encode1(50);
        buf.Encode1(1); buf.Encode1(40);
        buf.Encode1(2); buf.Encode1(5);
        buf.Encode1(5); buf.Encode1(5);
    }
}
