using Pingu.Networking;

namespace Pingu.Packets.Game;

public class StartGameResult(int res) : IPacket
{
    public void Encode(SendPacketBase buf) => buf.Encode1(res);
}
