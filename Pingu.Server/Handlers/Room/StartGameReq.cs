using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class StartGameReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var room = client.CurrentRoom;
        if (room == null) return;

        _ = client.SendPacketAsync(new StartGameResult(0));
        room.StartGame();
    }
}