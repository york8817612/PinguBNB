using Pingu.Networking;
using Pingu.Packets.Game;
using Pingu.Models;

namespace Pingu.Handlers.Game;

public class MovableBoxMoveReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int v1 = data.Decode1(ref off);
        int v2 = data.Decode1(ref off);
        int v3 = data.Decode1(ref off);
        int v4 = data.Decode1(ref off);
        int v5 = ServerConfig.IsJP ? data.Decode1(ref off) : 0;
        int v6 = ServerConfig.IsJP ? data.Decode1(ref off) : 0;

        Room.Broadcast(new MovableBoxMove(v1, v2, v3, v4, v5, v6));
    }
}
