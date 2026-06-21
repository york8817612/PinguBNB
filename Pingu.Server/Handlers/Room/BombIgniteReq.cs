using Pingu.Networking;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class BombIgniteReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int slotId = data.Decode1(ref off);
        int unk = ServerConfig.IsJP ? data.Decode1(ref off) : 0;
        int pos = data.Decode1(ref off);
        int bombAttr = data.Decode1(ref off);

        int power = bombAttr & 0x0F;
        bool isSpecial = ((bombAttr >> 6) & 1) == 1;

        Room.AddBomb(slotId, pos, bombAttr, isSpecial, unk);
    }
}
