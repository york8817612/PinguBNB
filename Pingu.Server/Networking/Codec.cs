namespace Pingu.Networking;

public static class Codec
{
    public const int LoopbackIp = 0x7F000001;
    public static int CipherDegreeInit
    {
        get
        {
            if (ServerConfig.IsTH || ServerConfig.IsVN || ServerConfig.IsNA) return 3;
            if (ServerConfig.IsCN) return 2;
            return 1;
        }
    }

    public static int HeaderLen => CipherDegreeInit == 3 ? 4 : 3;
    public static int PayloadLenIdx => CipherDegreeInit == 3 ? 2 : 1;
    public static int OpcodeLen => CipherDegreeInit == 3 ? 2 : 1;
    public static int CrcLen => CipherDegreeInit switch { 1 => 4, 2 or 3 => 1, _ => 0 };
    public static int MinimumLen => HeaderLen + OpcodeLen + CrcLen;

    public const int HeaderType = 0;
    public const int HeaderCodeRcvBase = 192;
    public const int HeaderCodeSndBase = 102;
    public const int HeaderCodeModifier = 231;
    public const int PacketRcvSeqDelta = 3;
    public const int PacketSndSeqDelta = 3;
}
