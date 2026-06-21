namespace Pingu;

public static class CRC8
{
    private static readonly int[] Table = new int[256];
    private static bool _init;

    private static void GenTable()
    {
        for (int i = 0; i < 256; i++)
        {
            int v = i;
            for (int j = 0; j < 8; j++)
                v = (v & 0x80) != 0 ? (v << 1) ^ 7 : v << 1;
            Table[i] = v & 0xFF;
        }
        _init = true;
    }

    public static int Update(int key, ReadOnlySpan<byte> data)
    {
        if (!_init) GenTable();
        int crc = key & 0xFF;
        foreach (var b in data)
            crc = Table[crc ^ b];
        return crc;
    }
}
