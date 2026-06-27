using Pingu.Networking;
using Pingu.Models;

namespace Pingu.Packets.Game;

public class LaunchGameStage(Room room, List<int> spawnPositions) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode4(ServerConfig.TickCount);
        buf.Encode2(room.MapId);
        buf.Encode1(room.ActiveSlotCount);

        for (int i = 0; i < room.Slots.Count; i++)
        {
            var slot = room.Slots[i];
            if (slot.User != null || slot.IsAI)
            {
                buf.Encode1(i);
                buf.Encode1Bool(false);
                buf.Encode1(spawnPositions[i]);
                buf.Encode1(slot.Bomber);
                buf.Encode1(slot.Color);
            }
        }
    }
}