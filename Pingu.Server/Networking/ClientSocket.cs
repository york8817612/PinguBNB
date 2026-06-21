using System.Net.Sockets;
using Pingu.Models;

namespace Pingu.Networking;

public class ClientSocket
{
    public TcpClient TcpClient { get; }
    public NetworkStream Stream { get; }
    public List<User> Users { get; } = [];
    public int SendSeq { get; set; } = 40;
    public int RecvSeq { get; set; } = 40;
    public int CipherDegree { get; set; } = 0;

    public ClientSocket(TcpClient tcpClient)
    {
        TcpClient = tcpClient;
        Stream = tcpClient.GetStream();
    }

    public async Task RunAsync()
    {
        try
        {
            // Send initial ConnEstablished
            await SendPacketAsync(new Packets.ConnEstablished());

            var recvBuf = new byte[4096];
            var buffer = new MemoryStream();

            while (true)
            {
                int bytesRead;
                try
                {
                    bytesRead = await Stream.ReadAsync(recvBuf);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0) break;

                buffer.Write(recvBuf, 0, bytesRead);

                while (TryDecodePacket(buffer, out var payload, out var opcode))
                {
                    var handler = opcode >= 0 && opcode < OpcodeManager.HandlerArray.Length
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
        catch (Exception ex)
        {
            if (ServerConfig.DebugMode)
                Console.WriteLine($"Client disconnected: {ex.Message}");
        }
        finally
        {
            TcpClient.Close();
        }
    }

    private bool TryDecodePacket(MemoryStream buffer, out byte[] payload, out int opcode)
    {
        payload = [];
        opcode = -1;
        var buf = buffer.GetBuffer();
        int len = (int)buffer.Length;
        int pos = 0;

        if (len < Codec.MinimumLen) return false;

        // Check header code
        int expectedHeader = (Codec.HeaderType + Codec.HeaderCodeModifier) ^ (Codec.HeaderCodeRcvBase + RecvSeq);
        int recvHeader = buf[pos];
        if (recvHeader != (byte)expectedHeader)
        {
            buffer.SetLength(0);
            return false;
        }
        pos++;

        // Read payload length (always Decode2 for header portion)
        int payloadLen = (buf[pos + 1] | (buf[pos] << 8)) ^ 0xA569;
        pos += 2;

        if (len - pos < payloadLen + Codec.CrcLen) return false;

        // Extract payload
        payload = new byte[payloadLen];
        Array.Copy(buf, pos, payload, 0, payloadLen);
        pos += payloadLen;

        // Decrypt payload
        SimpleStream.Decrypt3(payload, RecvSeq);

        // Verify CRC
        bool crcOk = Codec.CipherDegreeInit switch
        {
            1 => (buf[pos + 3] | (buf[pos + 2] << 8) | (buf[pos + 1] << 16) | (buf[pos] << 24)) == CRC32.Update(RecvSeq, payload),
            2 or 3 => buf[pos] == (byte)CRC8.Update(RecvSeq, payload),
            _ => true
        };

        if (!crcOk)
        {
            Console.WriteLine($"CRC 錯誤，斷開連接");
            buffer.SetLength(0);
            return false;
        }

        pos += Codec.CrcLen;

        // Get opcode from decrypted payload
        int payloadOff = 0;
        opcode = Codec.CipherDegreeInit == 3 ? payload.Decode2(ref payloadOff) : payload.Decode1(ref payloadOff);

        // Remove consumed byte
        payload = payload[payloadOff..];

        RecvSeq += Codec.PacketRcvSeqDelta;
        return true;
    }

    public async Task SendPacketAsync(IPacket packet)
    {
        var opcode = OpcodeManager.GetSendOp(packet.GetType());
        if (opcode < 0) return;

        using var ms = new MemoryStream();

        var spb = new SendPacketBase(CipherDegree);
        if (CipherDegree == 3)
        {
            spb.Encode2(opcode);
        }
        else
        {
            spb.Encode1(opcode);
        }
        packet.Encode(spb);
        var payload = spb.Stream.ToArray();

        ms.SetLength(payload.Length);
        ms.Position = payload.Length;

        if (ServerConfig.DebugMode)
        {
            var name = packet.GetType().Name;
            Console.WriteLine($"[{name}] {opcode} | 0x{opcode:X} | Sending {BitConverter.ToString(payload).Replace("-", " ")}");
        }

        // Write CRC
        int crc = CipherDegree switch
        {
            1 => CRC32.Update(SendSeq, payload),
            2 or 3 => CRC8.Update(SendSeq, payload),
            _ => 0
        };

        switch (CipherDegree)
        {
            case 1:
                ms.WriteByte((byte)(crc >> 24));
                ms.WriteByte((byte)(crc >> 16));
                ms.WriteByte((byte)(crc >> 8));
                ms.WriteByte((byte)crc);
                break;
            case 2 or 3:
                ms.WriteByte((byte)crc);
                break;
        }
        ms.Position = 0;
        var tmp = ms.ToArray();
        spb.Stream.Position = 0;
        spb.Stream.CopyTo(ms);

        var finalPayload = ms.ToArray();

        // Write header
        int headerCode = (Codec.HeaderType + Codec.HeaderCodeModifier) ^ (Codec.HeaderCodeSndBase + SendSeq);
        int payloadLen = payload.Length;
        var header = new MemoryStream();
        header.WriteByte((byte)headerCode);

        // Write Encrypt Payload
        byte[] result;
        if (CipherDegree is >= 1 and <= 3)
        {
            if (CipherDegree == 3)
            {
                header.WriteByte(0);
                var encLen = payloadLen ^ unchecked((int)0x96CA5395);
                header.WriteByte((byte)(encLen >> 24));
                header.WriteByte((byte)(encLen >> 16));
                header.WriteByte((byte)(encLen >> 8));
                header.WriteByte((byte)encLen);
            }
            else
            {
                var encLen = payloadLen ^ 0xA569;
                header.WriteByte((byte)(encLen >> 8));
                header.WriteByte((byte)encLen);
            }
            // Encrypt payload
            SimpleStream.Encrypt3(finalPayload.AsSpan(), SendSeq, 0, payloadLen);

            result = new byte[header.Length + finalPayload.Length];
            Span<byte> span = result;
            header.ToArray().CopyTo(span);
            finalPayload.CopyTo(span[(int)header.Length..]);

            Stream.Write(span);
        }
        else // Write Raw Payload
        {
            header.WriteByte((byte)(payloadLen >> 8));
            header.WriteByte((byte)payloadLen);
            CipherDegree = Codec.CipherDegreeInit;

            result = new byte[header.Length + payload.Length];
            Span<byte> span = result;
            header.ToArray().CopyTo(span);
            payload.CopyTo(span[(int)header.Length..]);

            Stream.Write(span);
        }

        //var headerBytes = header.ToArray();
        //await Stream.WriteAsync(headerBytes);
        //await Stream.WriteAsync(payload);

        SendSeq += Codec.PacketSndSeqDelta;
    }

    public async Task SendPacketsAsync(params IPacket[] packets)
    {
        foreach (var p in packets)
            await SendPacketAsync(p);
    }

    private void LogPacket(int opcode, IPacketHandler? handler, byte[] payload)
    {
        var name = OpcodeManager.RecvOps.GetValueOrDefault(opcode, "UNKNOWN");
        var status = handler != null ? "接收" : "找不到 Handler";
        Console.WriteLine($"[{name}] {opcode} | 0x{opcode:X} | {status}");
        if (payload.Length is > 0 and <= 100)
            Console.WriteLine(BitConverter.ToString(payload).Replace("-", " "));
    }
}
