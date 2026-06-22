using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class AILevelChangeReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int slotIdx = data.Decode1();
        bool increase = data.Decode1() != 0;

        var slot = slotIdx >= 0 && slotIdx < Room.Slots.Length ? Room.Slots[slotIdx] : null;
        if (slot == null || !slot.IsAI) return;

        int current = slot.AILevel;
        int newLevel;

        if (increase)
            newLevel = current < 45 ? current + 15 : current + 45;
        else
            newLevel = current <= 46 ? current - 15 : current - 45;

        if (newLevel < 1 || newLevel > 31) return;

        slot.AILevel = newLevel;
        Room.Broadcast(new AILevelChange(slotIdx, newLevel));
    }
}
