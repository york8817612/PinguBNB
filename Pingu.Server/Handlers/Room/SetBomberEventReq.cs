using Pingu.Networking;

namespace Pingu.Handlers.Game;

public class SetBomberEventReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int eventTypeAndSlot = data.Decode1();
        int v1 = data.Decode1();
        int v2 = data.Decode2();

        int slotIdx = eventTypeAndSlot & 0xF;
        int eventType = eventTypeAndSlot >> 4;

        switch (eventType)
        {
            case 3: data.Decode1(); break;
            case 4: data.Decode1(); break;
            case 8:
                data.Decode1();
                data.Decode1();
                break;
        }
    }
}
