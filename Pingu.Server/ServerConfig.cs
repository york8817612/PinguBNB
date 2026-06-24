using System.Text;
using YamlDotNet.RepresentationModel;

namespace Pingu;

public enum Region { JP, TW, CN, TH, VN, NA }

public static class ServerConfig
{
    public static Region Locale { get; private set; }
    public static int Ver { get; private set; }
    public static int ChannelCount { get; private set; } = 5;
    public static Encoding ACP { get; private set; } = Encoding.UTF8;

    public static bool IsJP => Locale == Region.JP;
    public static bool IsTW => Locale == Region.TW;
    public static bool IsCN => Locale == Region.CN;
    public static bool IsTH => Locale == Region.TH;
    public static bool IsNA => Locale == Region.NA;
    public static bool IsVN => Locale == Region.VN;

    public static bool DebugMode { get; set; } = true;
    public static bool ShowDecValue { get; set; } = false;

    public static int TickCount => Environment.TickCount;

    public static void Load(string configPath = "Config.yaml")
    {
        var yml = new YamlStream();
        using var sr = new StreamReader(configPath);
        yml.Load(sr);
        var root = (YamlMappingNode)yml.Documents[0].RootNode!;

        var localeStr = ((YamlScalarNode)root["Locale"]).Value!;
        Locale = Enum.Parse<Region>(localeStr);

        var verKey = $"{localeStr}_VER";
        if (root.Children.ContainsKey(new YamlScalarNode(verKey)))
            Ver = int.Parse(((YamlScalarNode)root[verKey]!).Value!);
        else
            throw new Exception($"Missing version config for {localeStr} (XX_VER)");

        if (root.Children.ContainsKey(new YamlScalarNode("ChannelCount")))
            ChannelCount = int.Parse(((YamlScalarNode)root["ChannelCount"]!).Value!);

        ACP = Locale switch
        {
            Region.TH => Encoding.GetEncoding("windows-874"),
            Region.JP => Encoding.GetEncoding("shift-jis"),
            Region.CN => Encoding.GetEncoding("gb2312"),
            Region.TW => Encoding.GetEncoding("big5"),
            Region.VN => Encoding.GetEncoding("windows-1258"),
            _ => Encoding.UTF8,
        };
    }
}