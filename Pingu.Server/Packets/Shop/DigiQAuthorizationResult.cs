using Pingu.Networking;

namespace Pingu.Packets.Shop;

public class DigiQAuthorizationResult(int res) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(res);
        if (res == 12)
        {
            buf.Encode4(0);
            buf.Encode4(0);
        }
        else
        {
            buf.EncodeStr("");
        }
    }
}
