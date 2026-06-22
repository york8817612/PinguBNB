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

        Room.InitSlots(userCount, client.Users);

        _ = client.SendPacketAsync(new CreateSessionResult(mode));
        _ = Room.EncodeSlots(client);
    }
}
