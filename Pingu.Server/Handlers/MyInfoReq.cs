using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class MyInfoReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        var player = data.Decode1(ref off);

        _ = client.SendPacketsAsync(new MyInfoResult());
    }
}
