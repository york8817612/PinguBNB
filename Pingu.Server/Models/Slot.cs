using Pingu.Networking;

namespace Pingu.Models;

public class Slot
{
    public User? User { get; set; } = null;
    public byte[] Meta { get; } = new byte[3];
    public bool IsAI { get; set; } = false;
    public int AILevel { get; set; } = 1;

    public byte State
    {
        get => Meta[0];
        set => Meta[0] = value;
    }

    public byte Bomber
    {
        get => Meta[1];
        set => Meta[1] = value;
    }

    public byte Color
    {
        get => Meta[2];
        set => Meta[2] = value;
    }

    public void Encode(SendPacketBase buf) => buf.EncodeBuffer(Meta);
}

public static class SlotFlags
{
    public const byte Closed = 0x1;
    public const byte Chief = 0x2;
    public const byte Ready = 0x4;
}
