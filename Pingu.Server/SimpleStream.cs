namespace Pingu;

public static class SimpleStream
{
    public static void Encrypt3(Span<byte> buf, int key, int offset = 0, int? size = null)
    {
        int len = size ?? buf.Length - offset;
        int blockCount = len >> 2;
        int remaining = len & 3;

        int mk = key ^ unchecked((int)0x53351D9C);

        if (blockCount > 0)
        {
            int pp = BitConverter.ToInt32(buf.Slice(offset));
            BitConverter.TryWriteBytes(buf.Slice(offset), mk ^ pp);

            for (int i = 0; i < blockCount - 1; i++)
            {
                mk -= unchecked((int)0x63CAACE3);
                int idx = offset + ((i + 1) * 4);
                int cb = BitConverter.ToInt32(buf.Slice(idx));
                BitConverter.TryWriteBytes(buf.Slice(idx), mk ^ cb ^ pp);
                pp = cb;
            }
            mk = pp;
        }

        if (remaining > 0)
        {
            int start = offset + (blockCount * 4);
            for (int i = 0; i < remaining; i++)
            {
                int mask = mk >> (i * 8);
                buf[start + i] ^= (byte)mask;
            }
        }
    }

    public static void Decrypt3(Span<byte> buf, int key, int offset = 0, int? size = null)
    {
        int len = size ?? buf.Length - offset;
        int blockCount = len >> 2;
        int remaining = len & 3;

        int mk = key ^ unchecked((int)0x53351D9C);

        if (blockCount > 0)
        {
            int fc = BitConverter.ToInt32(buf.Slice(offset));
            int pp = mk ^ fc;
            BitConverter.TryWriteBytes(buf.Slice(offset), pp);

            for (int i = 0; i < blockCount - 1; i++)
            {
                mk -= unchecked((int)0x63CAACE3);
                int idx = offset + ((i + 1) * 4);
                int cc = BitConverter.ToInt32(buf.Slice(idx));
                int cp = mk ^ cc ^ pp;
                BitConverter.TryWriteBytes(buf.Slice(idx), cp);
                pp = cp;
            }
            mk = pp;
        }

        if (remaining > 0)
        {
            int start = offset + (blockCount * 4);
            for (int i = 0; i < remaining; i++)
            {
                int mask = mk >> (i * 8);
                buf[start + i] ^= (byte)mask;
            }
        }
    }
}
