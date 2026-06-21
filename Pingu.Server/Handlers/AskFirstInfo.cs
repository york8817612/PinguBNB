using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class AskFirstInfo : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        _ = client.SendPacketAsync(new FirstInfoInit());
    }
}
