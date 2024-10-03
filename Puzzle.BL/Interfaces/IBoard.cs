namespace Puzzle.BL.Interfaces
{
    public interface IBoard
    {
        int MiddleBoard1DIndex { get; }
        int RowCount { get; }
        int ColumnCount { get; }
        ICard[,] Cards { get; }
        List<int> GetCardIds();
        void MoveCardsByIds(List<int> cardIdsAfterMove);
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
