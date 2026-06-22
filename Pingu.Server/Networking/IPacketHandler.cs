namespace Pingu.Networking;

public interface IPacketHandler
{
    void Handle(ClientSocket client, ReadOnlySpan<byte> payload);
}
