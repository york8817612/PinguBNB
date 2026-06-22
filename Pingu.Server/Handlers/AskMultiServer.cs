using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class AskMultiServer : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        data.Decode4();
        if (!ServerConfig.IsJP)
            data.Decode4();

        _ = client.SendPacketAsync(new MultiServerInit());
    }
}
