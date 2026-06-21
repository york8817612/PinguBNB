using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class EnterLobbyStageReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        _ = client.SendPacketAsync(new EnterLobbyStage());
    }
}
