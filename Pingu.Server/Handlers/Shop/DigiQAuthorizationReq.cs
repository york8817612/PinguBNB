using Pingu.Networking;
using Pingu.Packets.Shop;

namespace Pingu.Handlers.Shop;

public class DigiQAuthorizationReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var acc = data.DecodeStr();
        var pw = data.DecodeStr();

        _ = client.SendPacketAsync(new DigiQAuthorizationResult(12));
    }
}
