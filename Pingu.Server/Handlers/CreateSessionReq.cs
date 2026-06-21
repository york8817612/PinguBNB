using Pingu.Networking;
using Pingu.Packets;
using Pingu.Models;

namespace Pingu.Handlers;

public class CreateSessionReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        var roomName = data.DecodeStr(ref off);
        var pw = data.DecodeStr(ref off);
        int mode = data.Decode1(ref off);
        int userCount = data.Decode1(ref off);

        for (int i = 0; i < userCount; i++)
        {
            data.Decode1(ref off);
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
