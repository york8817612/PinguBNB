using Pingu.Networking;
using Pingu.Packets;
using Pingu.Models;

namespace Pingu.Handlers;

public class CreatePracticeSessionReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int userCount = data.Decode1();

        var room = ChannelManager.CreateRoom(client.ChannelId, "", "", 0, client);
        room.InitSlots(userCount, client.Users, true);
        client.CurrentRoom = room;

        _ = client.SendPacketAsync(new CreatePracticeSessionResult());
        _ = room.EncodeSlots(client);
    }
}