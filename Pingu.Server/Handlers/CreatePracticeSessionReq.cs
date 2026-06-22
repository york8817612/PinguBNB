using Pingu.Networking;
using Pingu.Packets;
using Pingu.Models;

namespace Pingu.Handlers;

public class CreatePracticeSessionReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int userCount = data.Decode1();

        Room.InitSlots(userCount, client.Users, fillAI: true);

        _ = client.SendPacketAsync(new CreatePracticeSessionResult());
        _ = Room.EncodeSlots(client);
    }
}
