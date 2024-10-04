namespace Puzzle.BL.Interfaces;

public interface ICardMover
{
    void MoveCardsToRandomPositions(IBoard board);
    void MoveCardsByIds(List<int> cardIdsAfterMove, IBoard board);
}
