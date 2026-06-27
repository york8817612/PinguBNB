using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class SelectColorReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var room = client.CurrentRoom;
        if (room == null) return;

        int d = data.Decode1();
        int selfIdx = d >> 4;
        int colorIdx = d & 0x0F;

        room.Slots[selfIdx].Color = (byte)colorIdx;
        room.Broadcast(new SelectColor(d));
    }
}