using Pingu.Networking;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class BombIgniteReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int slotId = data.Decode1();
        int unk = ServerConfig.IsJP ? data.Decode1() : 0;
        int pos = data.Decode1();
        int bombAttr = data.Decode1();

        int power = bombAttr & 0x0F;
        bool isSpecial = ((bombAttr >> 6) & 1) == 1;

        Room.AddBomb(slotId, pos, bombAttr, isSpecial, unk);
    }
}
