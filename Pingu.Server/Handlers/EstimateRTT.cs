using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class EstimateRTT : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int sequence = data.Decode4();
        _ = client.SendPacketAsync(new ResEstimateRTT(sequence, ServerConfig.TickCount));
    }
}
