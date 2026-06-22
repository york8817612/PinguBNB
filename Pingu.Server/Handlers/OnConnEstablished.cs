using System.Net;
using Pingu.Networking;
using Pingu.Packets;
using Pingu.Models;

namespace Pingu.Handlers;

public class OnConnEstablished : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var port = ((IPEndPoint)client.TcpClient.Client.LocalEndPoint!).Port;
        bool inGameServer = port == 4848;
        bool inShop = port == 4849;

        int userCount = data.Decode1();
        if (userCount is >= 1 and <= 2)
        {
            for (int i = 0; i < userCount; i++)
            {
                var name = data.DecodeStr();
                int userId = data.Decode4();

                if (ServerConfig.IsTW)
                {
                    data.Decode4();
                    if (inShop) data.Decode4();
                }

                if (ServerConfig.IsCN && inGameServer)
                    data.Decode4();

                client.Users.Add(new User(client, userId, name));
            }

            if (inGameServer)
            {
                int c1 = data.Decode4();
                int c2 = data.Decode4();
                data.Decode1();
                _ = client.SendPacketsAsync(new EnableShop(), new EnterLobbyStage());
            }
            else if (inShop)
            {
                data.Decode1();
                _ = client.SendPacketAsync(new EnterShopStage());
            }
        }
    }
}
