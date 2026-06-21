using Pingu.Networking;

namespace Pingu.Packets;

public class CreatePracticeSessionResult : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(0);
        buf.Encode2(0);
        buf.Encode4(2);
    }
}
