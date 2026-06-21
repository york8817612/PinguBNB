using Pingu.Networking;
using Pingu.Packets.Shop;

namespace Pingu.Handlers.Shop;

public class AskFxStock : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        _ = client.SendPacketAsync(new FxStockInit());
    }
}
