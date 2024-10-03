namespace Puzzle.BL.Interfaces
{
    public interface ICardMover
    {
        void MoveCardsByIds(List<int> cardIdsAfterMove, IBoard board);
    }
}
