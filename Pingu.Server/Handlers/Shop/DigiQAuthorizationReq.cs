using Pingu.Networking;
using Pingu.Packets.Shop;

namespace Pingu.Handlers.Shop;

public class DigiQAuthorizationReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        var acc = data.DecodeStr(ref off);
        var pw = data.DecodeStr(ref off);

        _ = client.SendPacketAsync(new DigiQAuthorizationResult(12));
    }
}
