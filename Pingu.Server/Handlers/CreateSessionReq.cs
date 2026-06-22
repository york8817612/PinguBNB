using Pingu.Networking;
using Pingu.Packets;
using Pingu.Models;

namespace Pingu.Handlers;

public class CreateSessionReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var roomName = data.DecodeStr();
        var pw = data.DecodeStr();
        int mode = data.Decode1();
        int userCount = data.Decode1();

        for (int i = 0; i < userCount; i++)
        {
            data.Decode1();
            var slot = Room.Slots[i];
            slot.User = client.Users[i];
            slot.State = SlotFlags.Chief;
            slot.Bomber = 5;
            slot.Color = (byte)i;
        }

        _ = client.SendPacketAsync(new CreateSessionResult(mode));
        _ = Room.EncodeSlots(client);
    }
}
