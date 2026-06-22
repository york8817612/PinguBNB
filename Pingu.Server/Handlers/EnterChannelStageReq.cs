using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class EnterChannelStageReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        data.Decode4();

        _ = client.SendPacketsAsync(
            new ChannelsInfo(),
            new ChannelsState(),
            new ChannelsGameState(),
            new ChannelsExtraInfo()
        );
    }
}
