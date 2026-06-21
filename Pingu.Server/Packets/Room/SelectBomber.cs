using Pingu.Networking;

namespace Pingu.Packets.Game;

public class SelectBomber(int data) : IPacket
{
    public void Encode(SendPacketBase buf) => buf.Encode1(data);
}
