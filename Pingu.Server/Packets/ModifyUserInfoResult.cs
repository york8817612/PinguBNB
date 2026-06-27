using Pingu.Networking;

namespace Pingu.Packets;

public class ModifyUserInfoResult() : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1Bool(true); // ModifyDone : CantModifyInfo
    }
}
