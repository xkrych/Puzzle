using Puzzle.BL.Exceptions;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

/// <summary>
/// Class prforming cards moves on the board.
/// </summary>
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

    /// <summary>
    /// Moving the cards on the board from their current 
    /// positions to match the target card layout.
    /// </summary>
    /// <param name="cardIdsAfterMove">target card layout</param>
    /// <param name="board">board</param>
    /// <exception cref="CardNotFoundException">card on the board wasn't found</exception>
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

    /// <summary>
    /// Move cards on the board according to card moves list.
    /// </summary>
    /// <param name="cardMoves">list of card moves</param>
    /// <param name="board">board</param>
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
