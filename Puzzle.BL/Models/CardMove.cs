using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

/// <summary>
/// Class for card move performed on the board.
/// </summary>
public class CardMove : ICardMove
{
    public int FromRow { get; set; }
    public int ToRow { get; set; }
    public int FromColumn { get; set; }
    public int ToColumn { get; set; }
}
