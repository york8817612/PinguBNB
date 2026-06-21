using System.Text;
using Pingu;
using Pingu.Networking;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
ServerConfig.Load();
Console.WriteLine($"Server starting... Locale={ServerConfig.Locale}, Ver={ServerConfig.Ver}");
await PinguServer.StartAsync();
