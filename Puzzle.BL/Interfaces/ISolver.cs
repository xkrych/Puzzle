namespace Puzzle.BL.Interfaces
{
    public interface ISolver
    {
        IBoard Board { get; }
        Task<bool> SolveBoard();
    }
}
