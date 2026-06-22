using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class MyInfoReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var player = data.Decode1();

        _ = client.SendPacketsAsync(new MyInfoResult());
    }
}
