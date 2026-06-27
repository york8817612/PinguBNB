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

        var room = ChannelManager.CreateRoom(client.ChannelId, roomName, pw, mode, client);
        room.InitSlots(userCount, client.Users);
        client.CurrentRoom = room;

        _ = client.SendPacketAsync(new CreateSessionResult(mode));
        _ = room.EncodeSlots(client);
    }
}