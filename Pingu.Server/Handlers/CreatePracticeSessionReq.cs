using Pingu.Networking;
using Pingu.Packets;
using Pingu.Models;

namespace Pingu.Handlers;

public class CreatePracticeSessionReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int userCount = data.Decode1(ref off);

        for (int i = 0; i < userCount; i++)
        {
            data.Decode1(ref off);
            var slot = Room.Slots[i];
            slot.User = client.Users[i];
            slot.State = SlotFlags.Chief;
            slot.Bomber = 5;
            slot.Color = 0;
        }

        for (int i = 0; i < 6 - userCount; i++)
        {
            var slot = Room.Slots[userCount + i];
            slot.Bomber = 5;
            slot.Color = 7;
            slot.IsAI = true;
        }

        _ = client.SendPacketAsync(new CreatePracticeSessionResult());
        _ = Room.EncodeSlots(client);
    }
}
