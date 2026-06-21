using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class SelectColorReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int d = data.Decode1(ref off);
        int selfIdx = d >> 4;
        int colorIdx = d & 0x0F;

        Room.Slots[selfIdx].Color = (byte)colorIdx;
        Room.Broadcast(new SelectColor(d));
    }
}
