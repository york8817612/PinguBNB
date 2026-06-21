using Pingu.Networking;

namespace Pingu.Packets;

public class EnterShopStage : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(0);
        buf.Encode4(0x286D6440);
        buf.Encode4(unchecked((int)0xDB1BEBAB));
        buf.Encode4(0x0F36E0AA);
        buf.EncodeStr("");
        buf.EncodeStr("");
        buf.Encode1(1);
        buf.Encode1(0);
        buf.Encode1(1);
        buf.Encode1Bool(false);
        buf.Encode2(0);
    }
}
