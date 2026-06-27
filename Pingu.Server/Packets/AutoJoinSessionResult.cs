using Pingu.Networking;

namespace Pingu.Packets;

public class AutoJoinSessionResult(int type) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(type); // 2 = CantFindEmptyRoom
        if (type == 0)
        {
            buf.Encode2(0);
            buf.Encode4();
        }
    }
}
