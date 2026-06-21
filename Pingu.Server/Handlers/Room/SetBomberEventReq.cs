using Pingu.Networking;

namespace Pingu.Handlers.Game;

public class SetBomberEventReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int eventTypeAndSlot = data.Decode1(ref off);
        int v1 = data.Decode1(ref off);
        int v2 = data.Decode2(ref off);

        int slotIdx = eventTypeAndSlot & 0xF;
        int eventType = eventTypeAndSlot >> 4;

        switch (eventType)
        {
            case 3: data.Decode1(ref off); break;
            case 4: data.Decode1(ref off); break;
            case 8:
                data.Decode1(ref off);
                data.Decode1(ref off);
                break;
        }
    }
}
