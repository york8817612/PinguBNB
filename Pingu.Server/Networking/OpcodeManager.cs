using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace Pingu.Networking;

public static class OpcodeManager
{
    public static Dictionary<int, string> RecvOps { get; private set; } = [];
    public static IPacketHandler?[] HandlerArray { get; private set; } = new IPacketHandler?[1000];
    public static Dictionary<Type, int> SendOpCache { get; } = [];

    public static void Load()
    {
        var recvYaml = LoadYamlMap($"{ServerConfig.Locale}_{ServerConfig.Ver}_recv");
        var sendYaml = LoadYamlMap($"{ServerConfig.Locale}_{ServerConfig.Ver}_send");

        // Scan assembly for handler types implementing IPacketHandler
        var asm = Assembly.GetExecutingAssembly();
        foreach (var type in asm.GetTypes())
        {
            if (type.GetInterfaces().Contains(typeof(IPacketHandler)) && !type.IsAbstract)
            {
                if (recvYaml.TryGetValue(type.Name, out var op) && op >= 0 && op < HandlerArray.Length)
                {
                    HandlerArray[op] = (IPacketHandler?)Activator.CreateInstance(type);
                }
            }
        }

        // Scan for packet types implementing IPacket
        foreach (var type in asm.GetTypes())
        {
            if (type.GetInterfaces().Contains(typeof(IPacket)) && !type.IsAbstract)
            {
                var name = type.Name;
                if (sendYaml.TryGetValue(name, out var op))
                    SendOpCache[type] = op;
                else
                {
                    // Try nested/anonymous class name stripping
                    var clean = name.Split('_')[0];
                    if (sendYaml.TryGetValue(clean, out var op2))
                        SendOpCache[type] = op2;
                }
            }
        }

        RecvOps = recvYaml.ToDictionary(kv => kv.Value, kv => kv.Key);
        Console.WriteLine($"Initialized: {HandlerArray.Count(h => h != null)} handlers bound, {SendOpCache.Count} send packets registered");
    }

    public static int GetSendOp(Type clazz) => SendOpCache.GetValueOrDefault(clazz, -1);

    private static Dictionary<string, int> LoadYamlMap(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, fileName + ".yaml");
        if (!File.Exists(path))
            path = Path.Combine(Directory.GetCurrentDirectory(), fileName + ".yaml");
        if (!File.Exists(path))
            path = fileName + ".yaml";

        var yml = new YamlStream();
        using var sr = new StreamReader(path);
        yml.Load(sr);
        var root = (YamlMappingNode)yml.Documents[0].RootNode!;
        var result = new Dictionary<string, int>();
        foreach (var entry in root.Children)
            result[((YamlScalarNode)entry.Key).Value!] = int.Parse(((YamlScalarNode)entry.Value).Value!);
        return result;
    }
}
