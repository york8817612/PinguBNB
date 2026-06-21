using Pingu.Networking;

namespace Pingu.Packets.Shop;

public class FxStockInit : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        var test = HexUtils.HexToBytes("18 03 00 00 00 00 00 00");
        buf.Encode4(test.Length);
        buf.EncodeBuffer(test);
    }
}
