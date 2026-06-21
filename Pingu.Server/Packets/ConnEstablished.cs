using Pingu.Networking;

namespace Pingu.Packets;

public class ConnEstablished : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode2(0);
        buf.Encode2(ServerConfig.Ver);
        buf.Encode4(0);
        buf.EncodeStr("");
        if (ServerConfig.IsTH || ServerConfig.IsVN || ServerConfig.IsNA)
            buf.Encode1Bool(false);
    }
}
