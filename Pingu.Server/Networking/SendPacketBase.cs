using Pingu;

namespace Pingu.Networking;

public class SendPacketBase
{
    public MemoryStream Stream { get; }
    private readonly int _cipherDegree;

    public SendPacketBase(int cipherDegree)
    {
        Stream = new MemoryStream();
        _cipherDegree = cipherDegree;
    }

    public void WriteByte(byte v) => Stream.WriteByte(v);
    public void WriteByte(int v) => Stream.WriteByte((byte)v);

    public void Encode1(int n = 0)
    {
        if (_cipherDegree is >= 1 and <= 3) Stream.WriteByte((byte)(n ^ 0x5A));
        else Stream.WriteByte((byte)n);
    }

    public void Encode1Bool(bool b) => Encode1(b ? 1 : 0);

    public void Encode2_LE(int n = 0)
    {
        var val = _cipherDegree is >= 1 and <= 3 ? n ^ 0xA569 : n;
        Stream.WriteByte((byte)val);
        Stream.WriteByte((byte)(val >> 8));
    }

    public void Encode2(int n = 0)
    {
        var val = _cipherDegree is >= 1 and <= 3 ? n ^ 0xA569 : n;
        Stream.WriteByte((byte)(val >> 8));
        Stream.WriteByte((byte)val);
    }

    public void Encode4_LE(int n = 0)
    {
        var val = _cipherDegree is >= 1 and <= 3 ? n ^ unchecked((int)0x96CA5395) : n;
        Stream.WriteByte((byte)val);
        Stream.WriteByte((byte)(val >> 8));
        Stream.WriteByte((byte)(val >> 16));
        Stream.WriteByte((byte)(val >> 24));
    }

    public void Encode4(int n = 0)
    {
        var val = _cipherDegree is >= 1 and <= 3 ? n ^ unchecked((int)0x96CA5395) : n;
        Stream.WriteByte((byte)(val >> 24));
        Stream.WriteByte((byte)(val >> 16));
        Stream.WriteByte((byte)(val >> 8));
        Stream.WriteByte((byte)val);
    }

    public void EncodeStr(string s = "")
    {
        var ba = ServerConfig.ACP.GetBytes(s);
        Encode2(ba.Length);
        Stream.Write(ba);
    }

    public void EncodeBuffer(byte[] ba) => Stream.Write(ba);
    public void EncodeBuffer(string hex) => Stream.Write(HexUtils.HexToBytes(hex));
    public void EncodeBuffer(int count) => Stream.Write(new byte[count]);
}
