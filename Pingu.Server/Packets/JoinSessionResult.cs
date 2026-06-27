using Pingu.Networking;

namespace Pingu.Packets;

public class JoinSessionResult(int mode) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(0); // 1 = WrongPassword, 2 = CantJoinRoom
    }
}
