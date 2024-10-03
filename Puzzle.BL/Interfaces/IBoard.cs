namespace Puzzle.BL.Interfaces
{
    public interface IBoard
    {
        int MiddleBoard1DIndex { get; }
        int RowCount { get; }
        int ColumnCount { get; }
        ICard[,] Cards { get; }
        bool TryGetCardPositionById(int cardId, out int row, out int column);
        ICard[,] GetBoardCopy();
        List<int> GetCardIds();
        ICard GetMiddleCard();
        ICard GetTopLeftCard();
        ICard GetTopMiddleCard();
        ICard GetTopRightCard();
        ICard GetRightMiddleCard();
        ICard GetDownRightCard();
        ICard GetDownMiddleCard();
        ICard GetDownLeftCard();
        ICard GetLeftMiddleCard();
    }
}
