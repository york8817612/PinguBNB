namespace Pingu.Networking;

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
