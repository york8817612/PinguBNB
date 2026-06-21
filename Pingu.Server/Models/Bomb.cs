namespace Pingu.Models;

public class Bomb
{
    public int Id { get; }
    public int OwnerSlotId { get; }
    public int Pos { get; }
    public int BombAttr { get; }
    public bool IsSpecial { get; set; }
    public bool IsMoving { get; set; } = false;
    public int State { get; set; } = -1;
    public long ExpireAt { get; set; }

    public Bomb(int id, int ownerSlotId, int pos, int bombAttr, bool isSpecial)
    {
        Id = id;
        OwnerSlotId = ownerSlotId;
        Pos = pos;
        BombAttr = bombAttr;
        IsSpecial = isSpecial;
        ExpireAt = Environment.TickCount + 2500;
    }

    public bool UpdateState(long now)
    {
        if (now < ExpireAt) return false;

        if (IsMoving)
        {
            IsMoving = false;
            ExpireAt = now + 2000;
            State = 1;
        }
        else
        {
            State = 0;
        }
        return true;
    }

    public bool ShouldRemove() => State == 0;
}
