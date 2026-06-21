using Pingu.Networking;
using Pingu.Models;

namespace Pingu.Packets.Game;

public class SetBombState(List<Bomb> bombs) : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(bombs.Count);
        foreach (var bomb in bombs)
        {
            buf.Encode2(bomb.Id);
            buf.Encode1(bomb.State);
            if (ServerConfig.IsJP && bomb.State == 0)
                buf.Encode4(0);
        }
    }
}
