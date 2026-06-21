using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class SelectMapReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int map = data.Decode2(ref off);
        Console.WriteLine($"mapId: {map}");

        Room.MapId = map;
        Room.Broadcast(new SelectMap(map));
    }
}
