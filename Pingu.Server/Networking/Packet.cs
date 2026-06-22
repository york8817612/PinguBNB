using System.Text;
using Pingu;

namespace Pingu.Networking;

public interface IPacket
{
    void Encode(SendPacketBase buf);
}

public interface IPacketHandler
{
    void Handle(ClientSocket client, ReadOnlySpan<byte> payload);
}

/// <summary>
/// BNB �ϥΤj�ݧ�
/// </summary>
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

public static class ReceiveExtensions
{
    public static int Decode1(ref this ReadOnlySpan<byte> data)
    {
        int val = Codec.CipherDegreeInit is >= 1 and <= 3
            ? data[0] ^ 0x5A
            : data[0];
        data = data[1..];
        return val;
    }

    public static int Decode2(ref this ReadOnlySpan<byte> data)
    {
        int val = data[1] | (data[0] << 8);
        data = data[2..];
        return Codec.CipherDegreeInit is >= 1 and <= 3 ? val ^ 0xA569 : val;
    }

    public static int Decode4(ref this ReadOnlySpan<byte> data)
    {
        int val = data[3] | (data[2] << 8) | (data[1] << 16) | (data[0] << 24);
        data = data[4..];
        return Codec.CipherDegreeInit is >= 1 and <= 3 ? val ^ unchecked((int)0x96CA5395) : val;
    }

    public static string DecodeStr(ref this ReadOnlySpan<byte> data)
    {
        int len = Codec.CipherDegreeInit == 3 ? data.Decode4() : data.Decode2();
        var s = ServerConfig.ACP.GetString(data[..len]);
        data = data[len..];
        return s;
    }

    public static string DecodeEncryptedStr(ref this ReadOnlySpan<byte> data, int key)
    {
        int len = Codec.CipherDegreeInit == 3 ? data.Decode4() : data.Decode2();
        var segment = new byte[len];
        data[..len].CopyTo(segment);
        SimpleStream.Decrypt3(segment, key);
        var s = ServerConfig.ACP.GetString(segment);
        if (ServerConfig.DebugMode && ServerConfig.ShowDecValue)
            Console.WriteLine($"decryptString: {s}");
        data = data[len..];
        return s;
    }
}

public static class HexUtils
{
    public static byte[] HexToBytes(string hex)
    {
        hex = hex.Replace("|", "").Replace(" ", "");
        var bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return bytes;
    }
}
