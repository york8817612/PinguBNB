using Pingu.Networking;
using Pingu.Models;

namespace Pingu.Packets.Game;

public class Airplane(int scheduledTime, List<AirplaneItem> items) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode4(scheduledTime);
        buf.Encode1(items.Count);
        foreach (var item in items)
        {
            buf.Encode1(item.V1);
            buf.Encode1(item.V2);
            buf.Encode1(item.V3);
            buf.Encode1(item.V4);
        }
    }
}
