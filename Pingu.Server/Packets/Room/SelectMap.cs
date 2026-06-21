using Pingu.Networking;

namespace Pingu.Packets.Game;

public class SelectMap(int map) : IPacket
{
    public void Encode(SendPacketBase buf) => buf.Encode2(map);
}
