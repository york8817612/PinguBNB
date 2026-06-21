using Pingu.Networking;

namespace Pingu.Models;

public class User
{
    public ClientSocket Client { get; }
    public int Id { get; }
    public string Name { get; }
    public int Level { get; set; } = 5;
    public int Lucci { get; set; } = 1234567;
    public int Exp { get; set; } = 0;

    public User(ClientSocket client, int id, string name)
    {
        Client = client;
        Id = id;
        Name = name;
    }
}
