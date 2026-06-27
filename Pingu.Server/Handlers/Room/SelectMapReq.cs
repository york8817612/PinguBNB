using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class SelectMapReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var room = client.CurrentRoom;
        if (room == null) return;

        int map = data.Decode2();
        Console.WriteLine($"mapId: {map}");

        room.MapId = map;
        room.Broadcast(new SelectMap(map));
    }
}