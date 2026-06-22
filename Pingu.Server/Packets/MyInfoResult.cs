using Pingu.Networking;

namespace Pingu.Packets;

public class MyInfoResult() : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1();
        buf.EncodeStr($"Dim"); // nickname
        buf.Encode1Bool(false); // gender
        buf.EncodeStr($"Taiwan"); // location
        buf.EncodeStr($"Just a person"); // bio
        buf.EncodeStr($"secret"); // birthday
        buf.Encode1Bool(false); // public bio
    }
}
