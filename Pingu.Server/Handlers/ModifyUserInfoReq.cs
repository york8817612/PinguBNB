using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class ModifyUserInfoReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        var unk = data.Decode1();
        var nickname = data.DecodeStr();
        var gender = data.Decode1();
        var address = data.DecodeStr();
        var introduce = data.DecodeStr();
        var birthday = data.DecodeStr();
        var open = data.Decode1();

        //_ = client.SendPacketsAsync(new MyInfoResult());
    }
}
