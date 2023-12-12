namespace Puzzle.BL.Interfaces;

public interface ICardMove
{
    int FromRow { get; set; }
    int ToRow { get; set; }
    int FromColumn { get; set; }
    int ToColumn { get; set; }
}