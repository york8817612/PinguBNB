using Pingu.Networking;

namespace Pingu.Packets.Game;

public class MovableBoxMove(int v1, int v2, int v3, int v4, int v5, int v6) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(v1);
        buf.Encode1(v2);
        buf.Encode1(v3);
        buf.Encode1(v4);
        if (ServerConfig.IsJP)
        {
            buf.Encode1(v5);
            buf.Encode1(v6);
        }
    }
}
