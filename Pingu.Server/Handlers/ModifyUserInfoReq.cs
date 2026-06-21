using Pingu.Networking;
using Pingu.Packets;

namespace Pingu.Handlers;

public class ModifyUserInfoReq : IPacketHandler
{
    public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
    {
        int off = 0;
        var unk = data.Decode1(ref off);
        var nickname = data.DecodeStr(ref off);
        var gender = data.Decode1(ref off);
        var address = data.DecodeStr(ref off);
        var introduce = data.DecodeStr(ref off);
        var birthday = data.DecodeStr(ref off);
        var open = data.Decode1(ref off);

        //_ = client.SendPacketsAsync(new MyInfoResult());
    }
}
