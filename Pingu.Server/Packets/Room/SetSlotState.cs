using Pingu.Networking;
using Pingu.Models;

namespace Pingu.Packets.Game;

public class SetSlotState(int slotId, Slot slot) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(slotId);
        slot.Encode(buf);
        buf.Encode1Bool(slot.IsAI);
        if (slot.IsAI)
            buf.Encode2(slot.AILevel);
    }
}
