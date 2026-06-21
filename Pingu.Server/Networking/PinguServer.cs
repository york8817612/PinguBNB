using System.Net;
using System.Net.Sockets;

namespace Pingu.Networking;

public static class PinguServer
{
    private static readonly int[] TcpPorts = [3838, 4848, 4849];
    private static readonly int[] UdpPorts = [3839, 4849];

    public static async Task StartAsync()
    {
        OpcodeManager.Load();

        var tcpTasks = TcpPorts.Select(StartTcpServerAsync);
        var udpTasks = UdpPorts.Select(StartUdpServerAsync);

        await Task.WhenAll(tcpTasks.Concat(udpTasks));
    }

    private static async Task StartTcpServerAsync(int port)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start(1024);
        Console.WriteLine($"Bind to TCP port: {port}");

        while (true)
        {
            var tcpClient = await listener.AcceptTcpClientAsync();
            tcpClient.NoDelay = true;
            _ = Task.Run(() => new ClientSocket(tcpClient).RunAsync());
        }
    }

    private static async Task StartUdpServerAsync(int port)
    {
        var udp = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        Console.WriteLine($"Bind to UDP port: {port}");

        while (true)
        {
            var result = await udp.ReceiveAsync();
            HandleUdpPacket(udp, result.Buffer, result.RemoteEndPoint);
        }
    }

    private static void HandleUdpPacket(UdpClient udp, byte[] buf, IPEndPoint sender)
    {
        int offset = 0;
        int len = buf[offset++];

        if (buf.Length < len) return;

        if (ServerConfig.IsJP)
            offset += 4;

        int opcode = buf[offset++];

        if (ServerConfig.DebugMode)
            Console.WriteLine($"{opcode} | 0x{opcode:X} | UDP Recv");

        switch (opcode)
        {
            case 0:
                int userId = BitConverter.ToInt32(buf, offset);
                SendUdp(udp, sender, 2, _ => { });
                break;
            case 1:
            case 11:
                userId = BitConverter.ToInt32(buf, offset);
                break;
        }
    }

    private static void SendUdp(UdpClient udp, IPEndPoint target, int opcode, Action<MemoryStream> writeAction)
    {
        using var ms = new MemoryStream();
        ms.WriteByte(0);
        if (ServerConfig.IsJP)
            ms.Write(BitConverter.GetBytes(0));

        ms.WriteByte((byte)opcode);
        writeAction(ms);

        var packet = ms.ToArray();
        packet[0] = (byte)packet.Length;

        int crcLen2;
        if (ServerConfig.IsTW || ServerConfig.IsCN) crcLen2 = 1;
        else if (ServerConfig.IsJP) crcLen2 = 4;
        else crcLen2 = 0;

        if (crcLen2 == 1)
        {
            int crc = CRC8.Update(0, packet);
            packet = packet.Append((byte)crc).ToArray();
        }
        else if (crcLen2 == 4)
        {
            int crc = CRC32.Update(0, packet);
            packet = packet.Concat(BitConverter.GetBytes(crc)).ToArray();
        }

        udp.Send(packet, packet.Length, target);
    }
}
