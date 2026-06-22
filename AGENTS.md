# Pingu.Server — BNB Game Server Emulator

## Build & Run

```powershell
dotnet build Pingu.Server\Pingu.Server.csproj
dotnet run --project Pingu.Server\Pingu.Server.csproj
```

Single project, net10.0, AOT-ready (`PublishAot=True`). No test project exists.

## Entrypoint

`Pingu.Server/Program.cs` → `PinguServer.StartAsync()` listens on TCP 3838/4848/4849 and UDP 3839/4849.

## Architecture

| Layer | Pattern | Description |
|-------|---------|-------------|
| **Handler** | `class FooReq : IPacketHandler { void Handle(ClientSocket, ReadOnlySpan<byte>) }` | Receives decrypted payload, decodes fields, runs game logic, sends response |
| **Packet** | `class FooResult : IPacket { void Encode(SendPacketBase) }` | Encodes outbound data into `SendPacketBase` (custom big-endian with XOR cipher) |
| **Dispatch** | `OpcodeManager.Load()` scans assembly for `IPacketHandler` types, maps them by class name to opcode in `{Locale}_{Ver}_recv.yaml` | Same for send-packet lookup via `{Locale}_{Ver}_send.yaml` |
| **Model** | `User`, `Room`, `Slot`, `Bomb`, `AirplaneItem` | Game state objects |

### Decode convention (unique to this repo)

Use `ref this` extension methods on `ReadOnlySpan<byte>` — they advance the span in place:

```csharp
public void Handle(ClientSocket client, ReadOnlySpan<byte> data)
{
    var count = data.Decode1();  // data is now data[1..]
    var userId = data.Decode4(); // data is now data[4..]
    var name = data.DecodeStr(); // variable-length string
    var secret = data.DecodeEncryptedStr(key); // decrypted segment
}
```

Available: `Decode1()`, `Decode2()`, `Decode4()`, `DecodeStr()`, `DecodeEncryptedStr(int key)`.

## Region & Locale

`Config.yaml` sets `Locale` (JP/TW/CN/TH/VN/NA). `{Locale}_{Ver}_recv.yaml` and `{Locale}_{Ver}_send.yaml` define opcode → handler/packet bindings. Encoding is auto-selected per locale.

## Key codec constants

All in `Codec.cs`. The cipher degree (`CipherDegreeInit`) is region-dependent (3 for TH/VN/NA, 2 for CN, 1 otherwise) and controls XOR masks, CRC length, and header length for the binary protocol.

## Common gotchas

- Each new handler class is **auto-discovered** (reflection) — no manual registration needed. Name must match a key in `{Locale}_{Ver}_recv.yaml`.
- `SendPacketsAsync` with multiple packets sends them sequentially on the same connection — not a broadcast.
- `client.BroadcastPacket` / `BroadcastPackets` sends to **all** connected clients.
- `ReadOnlySpan<byte>` is passed by value but `ref this` mutates the local — the caller's span does **not** advance. This is intentional; the handler owns its local `data`.
- No snapshot tests; integration testing would require running the server and connecting real/simulated BNB clients.
