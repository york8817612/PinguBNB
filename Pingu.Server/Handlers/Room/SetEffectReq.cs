using Pingu.Networking;

namespace Pingu.Handlers.Game;

public class SetEffectReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data) { }
}
