using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class BombKickThrowReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int slotId = data.Decode1(ref off);
        int bombId = data.Decode2(ref off);
        data.Decode1(ref off); // fromPos
        int targetPos = data.Decode1(ref off);
        int moveDuration = data.Decode2(ref off);

        var bomb = Room.GetBomb(bombId);
        if (bomb != null)
        {
            bomb.IsMoving = true;
            bomb.ExpireAt = Environment.TickCount + moveDuration;
            Room.Broadcast(new BombKickThrow(slotId, bombId, targetPos));
        }
    }
}
