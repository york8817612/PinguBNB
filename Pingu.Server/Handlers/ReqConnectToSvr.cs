using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class ReqConnectToSvr : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int channelId = data.Decode1(ref off);
        data.Decode4(ref off);

        _ = client.SendPacketAsync(new ResConnectToSvr(1, channelId));
    }
}
