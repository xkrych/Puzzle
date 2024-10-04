namespace Puzzle.BL.Interfaces;

public interface ISolver
{
    Task<bool> SolveBoard(IBoard board);
}
