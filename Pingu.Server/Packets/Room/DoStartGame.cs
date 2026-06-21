using Pingu.Networking;

namespace Pingu.Packets.Game;

public class DoStartGame(int random) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode4(ServerConfig.TickCount);
        buf.Encode4(random);
        buf.Encode4(0);
        buf.Encode4(0);
    }
}
