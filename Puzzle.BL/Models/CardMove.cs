using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

public class CardMove : ICardMove
{
    public int FromRow { get; set; }
    public int ToRow { get; set; }
    public int FromColumn { get; set; }
    public int ToColumn { get; set; }
}
