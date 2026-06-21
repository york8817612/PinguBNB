using System.Buffers;
using System.Net.Sockets;
using Pingu.Models;

namespace Pingu.Networking;

public class ClientSocket : IDisposable
{
    private const int RecvBufferSize = 4096;
    private static readonly List<ClientSocket> _allClients = [];
    private static readonly Lock _allClientsLock = new();

    public TcpClient TcpClient { get; }
    public NetworkStream Stream { get; }
    public List<User> Users { get; } = [];
    public int SendSeq { get; private set; } = 40;
    public int RecvSeq { get; private set; } = 40;
    public int CipherDegree { get; set; } = 0;
    private bool _disposed;

    public ClientSocket(TcpClient tcpClient)
    {
        TcpClient = tcpClient;
        Stream = tcpClient.GetStream();
        lock (_allClientsLock) _allClients.Add(this);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        lock (_allClientsLock) _allClients.Remove(this);
        TcpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    public void BroadcastPacket(IPacket packet)
    {
        lock (_allClientsLock)
        {
            foreach (var client in _allClients)
                _ = client.SendPacketAsync(packet);
        }
    }

    public void BroadcastPackets(params IPacket[] packets)
    {
        lock (_allClientsLock)
        {
            foreach (var client in _allClients)
                _ = client.SendPacketsAsync(packets);
        }
    }

    public async Task RunAsync()
    {
        try
        {
            await SendPacketAsync(new Packets.ConnEstablished());

            var recvBuf = ArrayPool<byte>.Shared.Rent(RecvBufferSize);
            var buffer = new MemoryStream();

            try
            {
                while (true)
                {
                    int bytesRead;
                    try
                    {
                        bytesRead = await Stream.ReadAsync(recvBuf.AsMemory(0, RecvBufferSize));
                    }
                    catch
                    {
                        break;
                    }

                    if (bytesRead == 0) break;

                    buffer.Write(recvBuf, 0, bytesRead);

                    while (TryDecodePacket(buffer, out var payload, out var opcode))
                    {
                        var handler = (uint)opcode < (uint)OpcodeManager.HandlerArray.Length
                            ? OpcodeManager.HandlerArray[opcode]
                            : null;

                        if (ServerConfig.DebugMode || handler == null)
                            LogPacket(opcode, handler, payload);

                        if (handler != null)
                        {
                            try
                            {
                                handler.Handle(this, payload);
                            }
                            catch (Exception ex)
                            {
                                var name = OpcodeManager.RecvOps.GetValueOrDefault(opcode, "UNKNOWN");
                                Console.Error.WriteLine($"業務邏輯錯誤 [{name}] | {ex.Message}");
                            }
                        }
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(recvBuf);
            }
        }
        catch (Exception ex)
        {
            if (ServerConfig.DebugMode)
                Console.WriteLine($"Client disconnected: {ex.Message}");
        }
        finally
        {
            Dispose();
        }
    }

    private bool TryDecodePacket(MemoryStream buffer, out ReadOnlySpan<byte> payload, out int opcode)
    {
        payload = [];
        opcode = -1;
        var buf = buffer.GetBuffer();
        int len = (int)buffer.Length;
        int pos = 0;

        if (len < Codec.MinimumLen) return false;

        int expectedHeader = (Codec.HeaderType + Codec.HeaderCodeModifier) ^ (Codec.HeaderCodeRcvBase + RecvSeq);
        if (buf[pos] != (byte)expectedHeader)
        {
            buffer.SetLength(0);
            return false;
        }
        pos++;

        int payloadLen = (buf[pos + 1] | (buf[pos] << 8)) ^ 0xA569;
        pos += 2;

        if (len - pos < payloadLen + Codec.CrcLen) return false;

        var payloadSpan = buf.AsSpan(pos, payloadLen);
        SimpleStream.Decrypt3(payloadSpan, RecvSeq);

        bool crcOk = Codec.CipherDegreeInit switch
        {
            1 => (buf[pos + payloadLen + 3] | (buf[pos + payloadLen + 2] << 8) | (buf[pos + payloadLen + 1] << 16) | (buf[pos + payloadLen] << 24)) == CRC32.Update(RecvSeq, payloadSpan),
            2 or 3 => buf[pos + payloadLen] == (byte)CRC8.Update(RecvSeq, payloadSpan),
            _ => true
        };

        if (!crcOk)
        {
            Console.WriteLine("CRC 錯誤，斷開連接");
            buffer.SetLength(0);
            return false;
        }

        pos += payloadLen + Codec.CrcLen;

        int payloadOff = 0;
        opcode = Codec.CipherDegreeInit == 3
            ? payloadSpan.Decode2(ref payloadOff)
            : payloadSpan.Decode1(ref payloadOff);

        payload = payloadSpan.Slice(payloadOff);

        RecvSeq += Codec.PacketRcvSeqDelta;
        return true;
    }

    public async Task SendPacketAsync(IPacket packet)
    {
        var opcode = OpcodeManager.GetSendOp(packet.GetType());
        if (opcode < 0) return;

        var spb = new SendPacketBase(CipherDegree);
        if (CipherDegree == 3)
            spb.Encode2(opcode);
        else
            spb.Encode1(opcode);
        packet.Encode(spb);
        var payload = spb.Stream.ToArray();

        if (ServerConfig.DebugMode)
        {
            var name = packet.GetType().Name;
            Console.WriteLine($"[{name}] {opcode} | 0x{opcode:X} | Sending {BitConverter.ToString(payload).Replace("-", " ")}");
        }

        int crc = CipherDegree switch
        {
            1 => CRC32.Update(SendSeq, payload),
            2 or 3 => CRC8.Update(SendSeq, payload),
            _ => 0
        };

        int headerCode = (Codec.HeaderType + Codec.HeaderCodeModifier) ^ (Codec.HeaderCodeSndBase + SendSeq);
        int crcLen = CipherDegree switch { 1 => 4, 2 or 3 => 1, _ => 0 };

        if (CipherDegree is >= 1 and <= 3)
        {
            int headerLen = CipherDegree == 3 ? 6 : 3;
            int totalLen = headerLen + payload.Length + crcLen;

            var result = ArrayPool<byte>.Shared.Rent(totalLen);
            try
            {
                result[0] = (byte)headerCode;

                if (CipherDegree == 3)
                {
                    result[1] = 0;
                    var encLen = payload.Length ^ unchecked((int)0x96CA5395);
                    result[2] = (byte)(encLen >> 24);
                    result[3] = (byte)(encLen >> 16);
                    result[4] = (byte)(encLen >> 8);
                    result[5] = (byte)encLen;
                }
                else
                {
                    var encLen = payload.Length ^ 0xA569;
                    result[1] = (byte)(encLen >> 8);
                    result[2] = (byte)encLen;
                }

                payload.AsSpan().CopyTo(result.AsSpan(headerLen));
                SimpleStream.Encrypt3(result.AsSpan(headerLen, payload.Length), SendSeq, 0, payload.Length);

                int crcPos = headerLen + payload.Length;
                switch (CipherDegree)
                {
                    case 1:
                        result[crcPos] = (byte)(crc >> 24);
                        result[crcPos + 1] = (byte)(crc >> 16);
                        result[crcPos + 2] = (byte)(crc >> 8);
                        result[crcPos + 3] = (byte)crc;
                        break;
                    case 2 or 3:
                        result[crcPos] = (byte)crc;
                        break;
                }

                Stream.Write(result, 0, totalLen);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(result);
            }
        }
        else
        {
            int totalLen = 3 + payload.Length;
            var result = ArrayPool<byte>.Shared.Rent(totalLen);
            try
            {
                result[0] = (byte)headerCode;
                result[1] = (byte)(payload.Length >> 8);
                result[2] = (byte)payload.Length;
                payload.AsSpan().CopyTo(result.AsSpan(3));
                CipherDegree = Codec.CipherDegreeInit;
                Stream.Write(result, 0, totalLen);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(result);
            }
        }

        SendSeq += Codec.PacketSndSeqDelta;
    }

    public async Task SendPacketsAsync(params IPacket[] packets)
    {
        foreach (var p in packets)
            await SendPacketAsync(p);
    }

    private void LogPacket(int opcode, IPacketHandler? handler, ReadOnlySpan<byte> payload)
    {
        var name = OpcodeManager.RecvOps.GetValueOrDefault(opcode, "UNKNOWN");
        var status = handler != null ? "接收" : "找不到 Handler";
        Console.WriteLine($"[{name}] {opcode} | 0x{opcode:X} | {status}");
        if (payload.Length is > 0 and <= 100)
            Console.WriteLine(BitConverter.ToString(payload.ToArray()).Replace("-", " "));
    }
}
