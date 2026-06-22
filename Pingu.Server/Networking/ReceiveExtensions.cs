using Pingu;

namespace Pingu.Networking;

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
