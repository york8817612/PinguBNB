using Pingu.Networking;
using Pingu.Models;

namespace Pingu.Packets;

public class LoginResult : IPacket
{
    private readonly List<User> _users;
    public LoginResult(List<User> users) => _users = users;

    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(0);
        if (ServerConfig.IsCN) buf.Encode1(0);
        buf.Encode2(1);
        buf.EncodeStr("");

        buf.Encode1(_users.Count);
        foreach (var user in _users)
        {
            buf.Encode4(user.Id);
            buf.EncodeStr(user.Name);
            buf.Encode1(0);
            buf.Encode2(0);
            buf.Encode1(1);
            buf.Encode1(10);
            buf.Encode2(user.Level);
            buf.Encode4(user.Lucci);
            buf.Encode4(user.Exp);
            if (ServerConfig.IsTW) buf.Encode1(0);
            buf.EncodeStr("");

            if (ServerConfig.IsTW)
            {
                buf.Encode1(2);
                buf.Encode1(2);
            }

            buf.Encode4(0);

            if (ServerConfig.IsTW)
            {
                buf.Encode1(0);
            }

            if (ServerConfig.IsCN)
            {
                buf.Encode1(0);
            }
        }
        buf.EncodeStr("");
    }
}

public class LoginResultJP : IPacket
{
    private readonly List<User> _users;
    public LoginResultJP(List<User> users) => _users = users;

    public void Encode(SendPacketBase buf)
    {
        buf.Encode1(0);
        buf.Encode2(1);
        buf.EncodeStr("");
        buf.Encode1(0);
        buf.Encode1(_users.Count);

        foreach (var user in _users)
        {
            buf.Encode4(user.Id);
            buf.EncodeStr(user.Name);
            buf.Encode1(0);
            buf.EncodeStr(user.Name);
            buf.Encode4(user.Level);
        }

        buf.Encode1(0);
        buf.Encode1(0);
    }
}
