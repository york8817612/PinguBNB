using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class SelectBomberReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var room = client.CurrentRoom;
        if (room == null) return;

        int d = data.Decode1();
        int selfIdx = d >> 4;
        int bomberIdx = d & 0x0F;

        room.Slots[selfIdx].Bomber = (byte)bomberIdx;
        room.Broadcast(new SelectBomber(d));
    }
}