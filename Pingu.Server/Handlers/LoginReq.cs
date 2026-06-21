using Pingu.Networking;
using Pingu.Packets;
using Pingu.Models;

namespace Pingu.Handlers;

public class LoginReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        int userCount = data.Decode1(ref off);
        if (userCount is >= 1 and <= 2)
        {
            for (int i = 0; i < userCount; i++)
            {
                var name = data.DecodeEncryptedStr(ref off, 0x11223344);
                var pw = data.DecodeEncryptedStr(ref off, 0x44332211);
                var userId = Interlocked.Increment(ref GlobalState.NextUserId);
                client.Users.Add(new User(client, userId, name));
            }

            int unk = data.Decode4(ref off);

            if (!ServerConfig.IsJP)
                _ = client.SendPacketAsync(new LoginResult(client.Users));
            else
                _ = client.SendPacketAsync(new LoginResultJP(client.Users));
        }
    }
}
