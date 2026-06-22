namespace Pingu.Networking;

public interface IPacket
{
    void Encode(SendPacketBase buf);
}
