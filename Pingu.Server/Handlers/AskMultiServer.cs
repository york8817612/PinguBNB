using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class AskMultiServer : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        data.Decode4(ref off);
        if (!ServerConfig.IsJP)
            data.Decode4(ref off);

        _ = client.SendPacketAsync(new MultiServerInit());
    }
}
