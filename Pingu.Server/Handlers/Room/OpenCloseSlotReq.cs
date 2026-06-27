using Pingu.Networking;
using Pingu.Packets;
using Pingu.Packets.Game;

namespace Pingu.Server.Handlers.Room;

public class OpenCloseSlotReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var pos = data.Decode1();

        int slot = pos >> 4;

        int state = pos & 0xF;

        var room = client.CurrentRoom;

        if (room == null) return;

        if (state == 0)
        {
            _ = room.OpenSlot(client, slot, true);
        } 
        else
        {
            _ = room.CloseSlot(client, slot);
        }
    }
}
