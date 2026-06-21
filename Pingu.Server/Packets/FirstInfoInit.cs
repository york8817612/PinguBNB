using Pingu.Networking;

namespace Pingu.Packets;

public class FirstInfoInit : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.EncodeStr("http://mail.nexonclub.com/clubadmin/confirmuser");
        buf.EncodeStr("http://id.nexonclub.com/nxidquery.idsocno");
    }
}
