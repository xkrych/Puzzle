namespace Puzzle.BL.Interfaces;

public interface IBoardBuilder
{
    void SetDefaultSquareCards();
    IBoard GetBoard();
}
