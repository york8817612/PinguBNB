using Pingu.Networking;
using Pingu.Packets.Game;

namespace Pingu.Models;

public class Room
{
    private int _nextBombId = 0;
    private Timer? _tickTimer;
    private readonly object _lock = new();

    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
    public int Mode { get; set; }
    public bool InGame { get; set; }
    public int ChannelId { get; set; }
    public ClientSocket? Host { get; set; }

    public List<Slot> Slots { get; } = [];
    public int MapId { get; set; } = 89;
    public List<Bomb> Bombs { get; } = [];
    public long LastAirplaneTime { get; set; } = 0;

    public bool HasPassword => !string.IsNullOrEmpty(Password);
    public int PlayerCount => ActiveSlotCount;
    public string HostName => Host?.Users.FirstOrDefault()?.Name ?? "";

    public int ActiveSlotCount => Slots.Count(s => s.User != null || s.IsAI);

    public IEnumerable<ClientSocket> UniqueClients =>
        Slots.Select(s => s.User?.Client)
            .Where(c => c != null)
            .DistinctBy(c => c!.TcpClient.Client.RemoteEndPoint!)!;

    public Room()
    {
        for (int i = 0; i < 8; i++)
            Slots.Add(new Slot());
    }

    public void StartGame()
    {
        lock (_lock)
        {
            if (ServerConfig.IsJP)
                MapId = 118;

            var rng = new Random();
            var spawnPositions = Enumerable.Range(0, 8).OrderBy(_ => rng.Next()).ToList();
            int random = rng.Next(9999);

            Broadcast(
                new LaunchGameStage(this, spawnPositions),
                new AirplaneItemProb(),
                new DoStartGame(random)
            );

            _tickTimer?.Dispose();
            _tickTimer = new Timer(_ => OnTick(), null, 0, 200);
            InGame = true;
        }
    }

    private void OnTick()
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

    public void AddBomb(int slotId, int pos, int bombAttr, bool isSpecial, int unk)
    {
        lock (_lock)
        {
            int id = _nextBombId++;
            var bomb = new Bomb(id, slotId, pos, bombAttr, isSpecial);
            Bombs.Add(bomb);

            Broadcast(new BombIgnite(slotId, pos, bombAttr, id, unk));
        }
    }

    public Bomb? GetBomb(int bombId) =>
        Bombs.Find(b => b.Id == bombId);

    public void Broadcast(params IPacket[] packets)
    {
        foreach (var client in UniqueClients)
            _ = client.SendPacketsAsync(packets);
    }

    public void Broadcast(IPacket packet)
    {
        foreach (var client in UniqueClients)
            _ = client.SendPacketAsync(packet);
    }

    public async Task EncodeSlots(ClientSocket c)
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            var slot = Slots[i];
            if (slot.User != null)
                await c.SendPacketAsync(new SetPlayer(i, slot, slot.User));
            else
                await c.SendPacketAsync(new SetSlotState(i, slot));
        }
    }

    public async Task CloseSlot(ClientSocket c, int slotIdx)
    {
        var slot = CloseSlotMeta(slotIdx);
        await c.SendPacketAsync(new SetSlotState(slotIdx, slot));
    }

    public async Task OpenSlot(ClientSocket c, int slotIdx, bool isPractice = false)
    {
        Slot slot;
        if (isPractice)
        {
            slot = DefaultAIMeta(slotIdx);
        } 
        else
        {
            slot = EmptySlotMeta(slotIdx);
        }
        await c.SendPacketAsync(new SetSlotState(slotIdx, slot));
    }

    public void InitSlots(int userCount, IReadOnlyList<User> users, bool isPractice = false)
    {
        for (int i = 0; i < 8; i++)
        {
            if (i < userCount)
            {
                var slot = Slots[i];
                slot.User = users[i];
                slot.State = SlotFlags.Chief;
                slot.Bomber = 5;
                slot.Color = (byte)(isPractice ? 0 : i);
            } 
            else
            {
                if (isPractice)
                {
                    if (i < userCount * 2)
                    {
                        DefaultAIMeta(i);
                    }
                    else
                    {
                        CloseSlotMeta(i);
                    }
                }
                else
                {
                    EmptySlotMeta(i);
                }
            }
        }
    }

    private Slot DefaultAIMeta(int slotIdx)
    {
        var slot = Slots[slotIdx];
        slot.State = SlotFlags.Ready;
        slot.Bomber = 5;
        slot.Color = 7;
        slot.IsAI = true;
        return slot;
    }

    private Slot EmptySlotMeta(int slotIdx)
    {
        var slot = Slots[slotIdx];
        slot.State = SlotFlags.Ready;
        slot.Bomber = 0;
        slot.Color = 0;
        slot.IsAI = false;
        return slot;
    }

    private Slot CloseSlotMeta(int slotIdx)
    {
        var slot = Slots[slotIdx];
        slot.State = SlotFlags.Closed;
        slot.Bomber = 0;
        slot.Color = 0;
        slot.IsAI = false;
        return slot;
    }

    public void EndGame()
    {
        lock (_lock)
        {
            _tickTimer?.Dispose();
            _tickTimer = null;
            InGame = false;
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _tickTimer?.Dispose();
            _tickTimer = null;
            Bombs.Clear();
            _nextBombId = 0;
            LastAirplaneTime = 0;
            InGame = false;
        }
    }
}
