using Pingu.Networking;
using Pingu.Models;

namespace Pingu.Packets.Game;

public class SetPlayer(int slotId, Slot slot, User user) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(slotId);
        buf.Encode4(user.Id);
        buf.Encode1Bool(true);
        buf.Encode1(0);
        buf.Encode1(6);

        slot.Encode(buf);

        buf.EncodeStr(user.Name);
        buf.Encode1(0);
        if (ServerConfig.IsJP)
        {
            buf.Encode1(0);
            buf.Encode2(0);
        }
        buf.Encode2(user.Level);
        if (ServerConfig.IsJP)
        {
            buf.Encode4(0);
            buf.EncodeStr("");
        }
        buf.Encode4(0);
        buf.Encode4(user.Level);
        buf.Encode2(0);

        buf.Encode1(0);
        buf.Encode1(0);
        buf.Encode4(0);
    }
}
