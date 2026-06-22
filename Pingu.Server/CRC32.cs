namespace Pingu;

public static class CRC32
{
    private static readonly int[] Table = new int[256];
    private static bool _init;

    private static void GenTable()
    {
        for (int i = 0; i < 256; i++)
        {
            int crc = i << 24;
            int v2 = 8;

            do
            {
                if (crc >= 0)
                {
                    crc *= 2;
                }
                else
                {
                    crc = (2 * crc) ^ 0x04C11DB7;
                }
                --v2;

            } while (v2 > 0);

            Table[i] = crc;
        }
        _init = true;
    }

    public static int Update(int key, ReadOnlySpan<byte> data)
    {
        if (!_init) GenTable();
        int crc = key;
        foreach (var b in data)
        {
            int index = (b ^ (crc >> 24)) & 0xFF;
            crc = Table[index] ^ (crc << 8);
        }
        return crc;
    }
}
