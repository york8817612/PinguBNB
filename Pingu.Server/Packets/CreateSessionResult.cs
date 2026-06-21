using Pingu.Networking;

namespace Pingu.Packets;

public class CreateSessionResult(int mode) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(0);
        buf.Encode2(0);
        buf.Encode4(0);
        buf.Encode1(mode);
    }
}
