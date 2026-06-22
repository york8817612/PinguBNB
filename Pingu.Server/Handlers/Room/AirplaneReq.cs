using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class AirplaneReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        long now = Environment.TickCount;

        if (Room.LastAirplaneTime + 7000 > now) return;

        int count = data.Decode1();

        if (count is >= 1 and <= 3)
        {
            var items = new List<AirplaneItem>();
            for (int i = 0; i < count; i++)
            {
                int v1 = data.Decode1();
                int v2 = data.Decode1();
                int v3 = data.Decode1();
                int v4 = data.Decode1();
                items.Add(new AirplaneItem(v1, v2, v3, v4));
            }

            long scheduledTime = now + 2000;
            Room.LastAirplaneTime = scheduledTime;

            Room.Broadcast(new Airplane((int)scheduledTime, items));
        }
    }
}
