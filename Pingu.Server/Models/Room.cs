using Pingu.Networking;
using Pingu.Packets.Game;

namespace Pingu.Models;

public static class Room
{
    private static int _nextBombId = 0;
    private static Timer? _tickTimer;
    private static readonly object _lock = new();

    public static Slot[] Slots { get; } = new Slot[8];
    public static int MapId { get; set; } = 89;
    public static List<Bomb> Bombs { get; } = [];
    public static long LastAirplaneTime { get; set; } = 0;

    public static int ActiveSlotCount => Slots.Count(s => s.User != null || s.IsAI);

    public static IEnumerable<ClientSocket> UniqueClients =>
        Slots.Select(s => s.User?.Client)
            .Where(c => c != null)
            .DistinctBy(c => c!.TcpClient.Client.RemoteEndPoint!)!;

    static Room()
    {
        for (int i = 0; i < 8; i++)
            Slots[i] = new Slot();
    }

    public static void StartGame()
    {
        lock (_lock)
        {
            if (ServerConfig.IsJP)
                MapId = 118;

            var rng = new Random();
            var spawnPositions = Enumerable.Range(0, 8).OrderBy(_ => rng.Next()).ToList();
            int random = rng.Next(9999);

            Broadcast(
                new LaunchGameStage(spawnPositions),
                new AirplaneItemProb(),
                new DoStartGame(random)
            );

            _tickTimer?.Dispose();
            _tickTimer = new Timer(_ => OnTick(), null, 0, 200);
        }
    }

    private static void OnTick()
    {
        lock (_lock)
        {
            long now = Environment.TickCount;
            var changed = new List<Bomb>();

            Bombs.RemoveAll(b =>
            {
                if (b.UpdateState(now)) changed.Add(b);
                return b.ShouldRemove();
            });

            if (changed.Count > 0)
                Broadcast(new SetBombState(changed));
        }
    }

    public static void AddBomb(int slotId, int pos, int bombAttr, bool isSpecial, int unk)
    {
        lock (_lock)
        {
            int id = _nextBombId++;
            var bomb = new Bomb(id, slotId, pos, bombAttr, isSpecial);
            Bombs.Add(bomb);

            Broadcast(new BombIgnite(slotId, pos, bombAttr, id, unk));
        }
    }

    public static Bomb? GetBomb(int bombId) =>
        Bombs.Find(b => b.Id == bombId);

    public static void Broadcast(params IPacket[] packets)
    {
        foreach (var client in UniqueClients)
            _ = client.SendPacketsAsync(packets);
    }

    public static void Broadcast(IPacket packet)
    {
        foreach (var client in UniqueClients)
            _ = client.SendPacketAsync(packet);
    }

    public static async Task EncodeSlots(ClientSocket c)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            var slot = Slots[i];
            if (slot.User != null)
                await c.SendPacketAsync(new SetPlayer(i, slot, slot.User));
            else
                await c.SendPacketAsync(new SetSlotState(i, slot));
        }
    }

    public static void EndGame()
    {
        lock (_lock)
        {
            _tickTimer?.Dispose();
            _tickTimer = null;
        }
    }

    public static void Reset()
    {
        lock (_lock)
        {
            _tickTimer?.Dispose();
            _tickTimer = null;
            Bombs.Clear();
            _nextBombId = 0;
            LastAirplaneTime = 0;
        }
    }
}
