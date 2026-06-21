using Pingu.Networking;
using Pingu.Models;

namespace Pingu.Packets.Game;

public class LaunchGameStage(List<int> spawnPositions) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode4(ServerConfig.TickCount);
        buf.Encode2(Room.MapId);
        buf.Encode1(Room.ActiveSlotCount);

        for (int i = 0; i < Room.Slots.Length; i++)
        {
            var slot = Room.Slots[i];
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
