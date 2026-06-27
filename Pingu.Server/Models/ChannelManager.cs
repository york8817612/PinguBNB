using System.Collections.Concurrent;
using Pingu.Networking;

namespace Pingu.Models;

public static class ChannelManager
{
    private static readonly ConcurrentDictionary<int, List<Room>> _rooms = new();
    private static int _nextRoomId = 1;

    public static Room CreateRoom(int channelId, string name, string password, int mode, ClientSocket host)
    {
        var room = new Room
        {
            Id = _nextRoomId++,
            Name = name,
            Password = password,
            Mode = mode,
            ChannelId = channelId,
            Host = host,
        };

        var list = _rooms.GetOrAdd(channelId, _ => []);
        lock (list)
        {
            list.Add(room);
        }

        return room;
    }

    public static void RemoveRoom(Room room)
    {
        if (!_rooms.TryGetValue(room.ChannelId, out var list)) return;

        lock (list)
        {
            room.EndGame();
            list.Remove(room);
        }
    }

    public static List<Room> GetRooms(int channelId)
    {
        if (!_rooms.TryGetValue(channelId, out var list)) return [];

        lock (list)
        {
            return [.. list];
        }
    }

    public static Room? GetRoom(int channelId, int roomId)
    {
        if (!_rooms.TryGetValue(channelId, out var list)) return null;

        lock (list)
        {
            return list.Find(r => r.Id == roomId);
        }
    }
}
