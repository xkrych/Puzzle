using Puzzle.BL.Exceptions;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

public class CardMover : ICardMover
{
    private readonly IFactory<IPermutationGenerator> permutationGeneratorFactory;
    private readonly IFactory<ICardMove> cardMoveFactory;

    public CardMover(IFactory<ICardMove> cardMoveFactory,
        IFactory<IPermutationGenerator> permutationGeneratorFactory)
    {
        this.cardMoveFactory = cardMoveFactory;
        this.permutationGeneratorFactory = permutationGeneratorFactory;
    }

    public void MoveCardsToRandomPositions(IBoard board)
    {
        var generator = CreatePermutaionGenerator(board);
        var rndPermutaionIndex = new Random().Next(0, generator.PermutatedNumbers.Count - 1);
        var cardIdsRandomPermutation = generator.PermutatedNumbers[rndPermutaionIndex];
        MoveCardsByIds(cardIdsRandomPermutation, board);
    }

    public void MoveCardsByIds(List<int> cardIdsAfterMove, IBoard board)
    {
        var cardMoves = new List<ICardMove>();

        for (var i = 0; i < cardIdsAfterMove.Count; i++)
        {
            var cardId = cardIdsAfterMove[i];

            if (!board.TryGetCardPositionById(cardId, out var fromRow, out var fromColumn))
                throw new CardNotFoundException($"Card with id {cardId} not found.");

            var toRow = i / board.RowCount;
            var toColumn = i % board.ColumnCount;

            if (fromRow == toRow && fromColumn == toColumn)
                continue;

            var cardMove = cardMoveFactory.Create();
            cardMove.FromRow = fromRow;
            cardMove.FromColumn = fromColumn;
            cardMove.ToRow = toRow;
            cardMove.ToColumn = toColumn;
            cardMoves.Add(cardMove);
        }

        MoveCards(cardMoves, board);
    }

    private IPermutationGenerator CreatePermutaionGenerator(IBoard board)
    {
        // get list of cards IDs on board
        var edgeCardIds = board.GetCardIds();
        // create permutation generator according to card IDs
        var generator = permutationGeneratorFactory.Create();
        generator.Init(edgeCardIds);
        return generator;
    }

    private void MoveCards(List<ICardMove> cardMoves, IBoard board)
    {
        if (cardMoves.Count == 0)
            return;

        var copiedBoard = board.GetBoardCopy();

        foreach (var cardMove in cardMoves)
        {
            board.Cards[cardMove.ToRow, cardMove.ToColumn] = copiedBoard[cardMove.FromRow, cardMove.FromColumn];
        }
    }
}
