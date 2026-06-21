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
/// BNB ¨Ï¥Î¤jºÝ§Ç
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
    public static byte ReadByte(this ReadOnlySpan<byte> data, ref int offset)
        => data[offset++];

    public static int Decode1(this ReadOnlySpan<byte> data, ref int offset)
    {
        if (Codec.CipherDegreeInit is >= 1 and <= 3)
            return data[offset++] ^ 0x5A;
        return data[offset++];
    }

    public static int Decode2(this ReadOnlySpan<byte> data, ref int offset)
    {
        int val = data[offset + 1] | (data[offset] << 8);
        offset += 2;
        if (Codec.CipherDegreeInit is >= 1 and <= 3)
            return val ^ 0xA569;
        return val;
    }

    public static int Decode4(this ReadOnlySpan<byte> data, ref int offset)
    {
        int val = data[offset + 3] | (data[offset + 2] << 8) | (data[offset + 1] << 16) | (data[offset] << 24);
        offset += 4;
        if (Codec.CipherDegreeInit is >= 1 and <= 3)
            return val ^ unchecked((int)0x96CA5395);
        return val;
    }

    public static string DecodeStr(this ReadOnlySpan<byte> data, ref int offset)
    {
        int len = Codec.CipherDegreeInit == 3 ? data.Decode4(ref offset) : data.Decode2(ref offset);
        var s = ServerConfig.ACP.GetString(data.Slice(offset, len));
        offset += len;
        return s;
    }

    public static string DecodeEncryptedStr(this ReadOnlySpan<byte> data, ref int offset, int key)
    {
        int len = Codec.CipherDegreeInit == 3 ? data.Decode4(ref offset) : data.Decode2(ref offset);
        var segment = new byte[len];
        data.Slice(offset, len).CopyTo(segment);
        SimpleStream.Decrypt3(segment, key);
        var s = ServerConfig.ACP.GetString(segment);
        if (ServerConfig.DebugMode && ServerConfig.ShowDecValue)
            Console.WriteLine($"decryptString: {s}");
        offset += len;
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
