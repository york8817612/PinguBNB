using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class EnterChannelStageReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        data.Decode4(ref off);

        _ = client.SendPacketsAsync(
            new ChannelsInfo(),
            new ChannelsState(),
            new ChannelsGameState(),
            new ChannelsExtraInfo()
        );
    }
}
