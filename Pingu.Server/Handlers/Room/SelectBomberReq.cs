using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class SelectBomberReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int d = data.Decode1();
        int selfIdx = d >> 4;
        int bomberIdx = d & 0x0F;

        Room.Slots[selfIdx].Bomber = (byte)bomberIdx;
        Room.Broadcast(new SelectBomber(d));
    }
}
