using Pingu.Networking;
using Pingu.Packets;
using Pingu.Models;

namespace Pingu.Handlers;

public class LoginReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int userCount = data.Decode1();
        if (userCount is >= 1 and <= 2)
        {
            for (int i = 0; i < userCount; i++)
            {
                var name = data.DecodeEncryptedStr(0x11223344);
                var pw = data.DecodeEncryptedStr(0x44332211);
                var userId = Interlocked.Increment(ref GlobalState.NextUserId);
                client.Users.Add(new User(client, userId, name));
            }

            int unk = data.Decode4();

            _ = client.SendPacketAsync(new LoginResult(client.Users));
        }
    }
}
