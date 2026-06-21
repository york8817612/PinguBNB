using Pingu.Networking;

namespace Pingu.Packets;

public class ChannelsExtraInfo : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.EncodeStr("http://bnb.digicell.com.tw/webpage/BnB/EVENT/20040618/index.htm");
        buf.Encode2(600);
        buf.Encode2(525);
        buf.Encode2(60);

        if (ServerConfig.IsTW || ServerConfig.IsCN)
        {
            buf.Encode1(1);
            buf.Encode4(0x7F000001);
            buf.Encode2(9898);

            buf.Encode1(1);
            buf.Encode4(0x7F000001);

            buf.Encode1(1);
            buf.Encode4(0x7F000001);

            buf.Encode2(0);

            buf.Encode2(15);

            if (ServerConfig.IsTW)
            {
                buf.Encode1(1);
                buf.Encode4(0x7F000001);
                buf.Encode2(7360);
            }
        }
        else if (ServerConfig.IsJP)
        {
            buf.Encode1(1);
            buf.Encode4(0x7F000001);
            buf.Encode1(0);
            buf.Encode2(15);
        }
    }
}
