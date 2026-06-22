using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class ReqConnectToSvr : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int channelId = data.Decode1();
        data.Decode4();

        _ = client.SendPacketAsync(new ResConnectToSvr(1, channelId));
    }
}
