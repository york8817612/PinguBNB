using Pingu.Networking;
using System.Reflection;

namespace Pingu.Packets;

public class MyInfoResult() : IPacket
{
    public void Encode(SendPacketBase buf)
    {
        buf.Encode1();
        buf.EncodeStr($"黯淡"); // 綽號
        buf.Encode1Bool(false); // 性別
        buf.EncodeStr($"臺灣"); // 住址
        buf.EncodeStr($"就是一個人"); // 自我介紹
        buf.EncodeStr($"秘密"); // 生日
        buf.Encode1Bool(false); // 公開自我介紹
    }
}
