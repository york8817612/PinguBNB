using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class BombKickThrowReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int slotId = data.Decode1();
        int bombId = data.Decode2();
        data.Decode1(); // fromPos
        int targetPos = data.Decode1();
        int moveDuration = data.Decode2();

        var bomb = Room.GetBomb(bombId);
        if (bomb != null)
        {
            bomb.IsMoving = true;
            bomb.ExpireAt = Environment.TickCount + moveDuration;
            Room.Broadcast(new BombKickThrow(slotId, bombId, targetPos));
        }
    }
}
