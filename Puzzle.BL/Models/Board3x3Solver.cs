using Puzzle.BL.Enums;
using Puzzle.BL.Exceptions;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

public class Board3x3Solver : ISolver
{
    private readonly IFactory<IPermutationGenerator> permutationGeneratorFactory;
    private IPermutationGenerator? permutationGenerator;
    private readonly List<int> cardIdsInMiddle = new();
    private readonly ICardMover cardMover;

    public Board3x3Solver(ICardMover cardMover,
        IFactory<IPermutationGenerator> permutationGeneratorFactory)
    {
        this.permutationGeneratorFactory = permutationGeneratorFactory;
        this.cardMover = cardMover;
    }

    public Task<bool> SolveBoard(IBoard board)
    {
        return Task.Run(() =>
        {
            cardIdsInMiddle.Clear();
            permutationGenerator = null;
            var isFirstSolveTry = true;
            var isSolved = false;
            var isAllCardsInMiddle = false;
            var isAllPermutations = false;

            while (!isSolved && (!isAllPermutations || !isAllCardsInMiddle))
            {
                if (!isFirstSolveTry)
                {
                    RearrangeBoardCards(board, out isAllPermutations, out isAllCardsInMiddle);
                }
                else
                    isFirstSolveTry = false;

                if (!SolveTopMiddleCard(board))
                    continue;

                if (!SolveRightMiddleCard(board))
                    continue;

                if (!SolveDownMiddleCard(board))
                    continue;

                if (!SolveLeftMiddleCard(board))
                    continue;

                if (!SolveTopLeftCard(board))
                    continue;

                if (!SolveTopRightCard(board))
                    continue;

                if (!SolveDownRightCard(board))
                    continue;

                if (!SolveDownLeftCard(board))
                    continue;

                isSolved = true;
            }

            return isSolved;
        });
    }

    private List<int> GetCardIdsAroundBoard(IBoard board)
    {
        var cardIds = board.GetCardIds();
        var cardIdsAroundBoard = new List<int>();
        var i = 0;

        while (i < cardIds.Count)
        {
            var row = i / board.RowCount;
            var column = i % board.ColumnCount;

            if (row == board.RowCount / 2 && column == board.ColumnCount / 2)
            {
                i++;
                continue;
            }

            cardIdsAroundBoard.Add(cardIds[i++]);
        }

        return cardIdsAroundBoard;
    }

    private void MoveNextCardToMiddleOfTheBoard(IBoard board, out bool isAllCardsInMiddle)
    {
        // if all cards has been in the middle of the board, set flag isAllCardsInMiddle
        isAllCardsInMiddle = cardIdsInMiddle.Count == board.RowCount * board.ColumnCount;
        if (isAllCardsInMiddle)
            return;

        var cardIds = board.GetCardIds();
        for (var i = 0; i < cardIds.Count; i++)
        {
            var cardId = cardIds[i];
            if (!cardIdsInMiddle.Contains(cardId))
            {
                cardIdsInMiddle.Add(cardId);
                // swap card with card in the middle of the board
                cardIds[i] = cardIds[board.MiddleBoard1DIndex];
                cardIds[board.MiddleBoard1DIndex] = cardId;
                cardMover.MoveCardsByIds(cardIds, board);
                break;
            }
        }
    }

    private List<int> GetNextEdgeCardIdsPermutation(IBoard board)
    {
        if (permutationGenerator == null)
            throw new PermutationGenerationException("Permutation generator is null.");
        
        var permutatedEdgeCardIds = permutationGenerator.GetNextPermutation();

        if (permutatedEdgeCardIds == null)
            throw new PermutationGenerationException("Failed to create a permutation.");

        var cardIdsAfterMove = new List<int>();
        var middleCardInserted = false;
        var i = 0;
        while (i < permutatedEdgeCardIds.Count)
        {
            if (i == board.MiddleBoard1DIndex && !middleCardInserted)
            {
                // in the middle of the array is element which is in the middle of the board
                cardIdsAfterMove.Add(board.GetMiddleCard().Id);
                middleCardInserted = true;
            }
            else
            {
                var cardId = permutatedEdgeCardIds[i++];
                cardIdsAfterMove.Add(cardId);
            }
        }

        return cardIdsAfterMove;
    }

    private IPermutationGenerator CreatePermutaionGenerator(IBoard board)
    {
        // get list of card is around board
        var edgeCardIds = GetCardIdsAroundBoard(board);
        // create permutation generator according to card IDs
        var generator = permutationGeneratorFactory.Create();
        generator.Init(edgeCardIds);
        return generator;
    }

    private void RearrangeEdgeBoardCards(IBoard board)
    {
        // get next permutaion of edge cards
        var cardIdsAfterMove = GetNextEdgeCardIdsPermutation(board);
        // move cards around board accoeding to permutation
        cardMover.MoveCardsByIds(cardIdsAfterMove, board);
    }

    private void RearrangeBoardCards(IBoard board, out bool isAllPermutations, out bool isAllCardsInMiddle)
    {
        isAllPermutations = false;
        isAllCardsInMiddle = false;

        if (permutationGenerator == null)
        {
            // move next card to middle of the board
            MoveNextCardToMiddleOfTheBoard(board, out isAllCardsInMiddle);
            permutationGenerator = CreatePermutaionGenerator(board);
        }
        else if (permutationGenerator is { AllPermutationsGenerated: true })
        {
            isAllPermutations = true;
            // move next card to middle of the board
            MoveNextCardToMiddleOfTheBoard(board, out isAllCardsInMiddle);
            
            if (!isAllCardsInMiddle)
                permutationGenerator = CreatePermutaionGenerator(board);
        }

        if (isAllCardsInMiddle && isAllPermutations)
            return;

        RearrangeEdgeBoardCards(board);
    }

    // o X o
    // o X o
    // o o o
    // an attempt to rotate the card so that the smiley is complete
    private bool SolveTopMiddleCard(IBoard board)
    {
        var middleCard = board.GetMiddleCard();
        var topMiddleCard = board.GetTopMiddleCard();

        for (var i = 0; i < topMiddleCard.NumberOfCardSides; i++)
        {
            if (IsCompleteColoredEmoticon(middleCard.TopEmoticonColoredPart,
                    topMiddleCard.DownEmoticonColoredPart))
            {
                return true;
            }

            topMiddleCard.RotateCardToRight();
        }

        return false;
    }

    // o o o
    // o X X
    // o o o
    // an attempt to rotate the card so that the smiley is complete
    private bool SolveRightMiddleCard(IBoard board)
    {
        var middleCard = board.GetMiddleCard();
        var rightMiddleCard = board.GetRightMiddleCard();

        for (var i = 0; i < rightMiddleCard.NumberOfCardSides; i++)
        {
            if (IsCompleteColoredEmoticon(middleCard.RightEmoticonColoredPart,
                    rightMiddleCard.LeftEmoticonColoredPart))
            {
                return true;
            }

            rightMiddleCard.RotateCardToRight();
        }

        return false;
    }

    // o o o
    // o X o
    // o X o
    // an attempt to rotate the card so that the smiley is complete
    private bool SolveDownMiddleCard(IBoard board)
    {
        var middleCard = board.GetMiddleCard();
        var downMiddleCard = board.GetDownMiddleCard();

        for (var i = 0; i < downMiddleCard.NumberOfCardSides; i++)
        {
            if (IsCompleteColoredEmoticon(middleCard.DownEmoticonColoredPart,
                    downMiddleCard.TopEmoticonColoredPart))
            {
                return true;
            }

            downMiddleCard.RotateCardToRight();
        }

        return false;
    }

    // o o o
    // X X o
    // o o o
    // an attempt to rotate the card so that the smiley is complete
    private bool SolveLeftMiddleCard(IBoard board)
    {
        var middleCard = board.GetMiddleCard();
        var leftMiddleCard = board.GetLeftMiddleCard();

        for (var i = 0; i < leftMiddleCard.NumberOfCardSides; i++)
        {
            if (IsCompleteColoredEmoticon(middleCard.LeftEmoticonColoredPart,
                    leftMiddleCard.RightEmoticonColoredPart))
            {
                return true;
            }

            leftMiddleCard.RotateCardToRight();
        }

        return false;
    }

    // X X o
    // X o o
    // o o o
    // an attempt to rotate the card so that the smiley is complete
    private bool SolveTopLeftCard(IBoard board)
    {
        var leftMiddleCard = board.GetLeftMiddleCard();
        var topMiddleCard = board.GetTopMiddleCard();
        var topLeftCard = board.GetTopLeftCard();

        for (var i = 0; i < topLeftCard.NumberOfCardSides; i++)
        {
            if (IsCompleteColoredEmoticon(leftMiddleCard.TopEmoticonColoredPart, topLeftCard.DownEmoticonColoredPart)
                && IsCompleteColoredEmoticon(topMiddleCard.LeftEmoticonColoredPart, topLeftCard.RightEmoticonColoredPart))
            {
                return true;
            }

            topLeftCard.RotateCardToRight();
        }

        return false;
    }

    // o X X
    // o o X
    // o o o
    // an attempt to rotate the card so that the smiley is complete
    private bool SolveTopRightCard(IBoard board)
    {
        var topMiddleCard = board.GetTopMiddleCard();
        var rightMiddleCard = board.GetRightMiddleCard();
        var topRightCard = board.GetTopRightCard();

        for (var i = 0; i < topRightCard.NumberOfCardSides; i++)
        {
            if (IsCompleteColoredEmoticon(topMiddleCard.RightEmoticonColoredPart, topRightCard.LeftEmoticonColoredPart)
                && IsCompleteColoredEmoticon(rightMiddleCard.TopEmoticonColoredPart, topRightCard.DownEmoticonColoredPart))
            {
                return true;
            }

            topRightCard.RotateCardToRight();
        }

        return false;
    }

    // o o o
    // o o X
    // o X X
    // an attempt to rotate the card so that the smiley is complete
    private bool SolveDownRightCard(IBoard board)
    {
        var rightMiddleCard = board.GetRightMiddleCard();
        var downMiddleCard = board.GetDownMiddleCard();
        var downRightCard = board.GetDownRightCard();

        for (var i = 0; i < downRightCard.NumberOfCardSides; i++)
        {
            if (IsCompleteColoredEmoticon(rightMiddleCard.DownEmoticonColoredPart, downRightCard.TopEmoticonColoredPart)
                && IsCompleteColoredEmoticon(downMiddleCard.RightEmoticonColoredPart, downRightCard.LeftEmoticonColoredPart))
            {
                return true;
            }

            downRightCard.RotateCardToRight();
        }
        
        return false;
    }

    // o o o
    // X o o
    // X X o
    // an attempt to rotate the card so that the smiley is complete
    private bool SolveDownLeftCard(IBoard board)
    {
        var downMiddleCard = board.GetDownMiddleCard();
        var leftMiddleCard = board.GetLeftMiddleCard();
        var downLeftCard = board.GetDownLeftCard();

        for (var i = 0; i < downLeftCard.NumberOfCardSides; i++)
        {
            if (IsCompleteColoredEmoticon(downMiddleCard.LeftEmoticonColoredPart, downLeftCard.RightEmoticonColoredPart)
                && IsCompleteColoredEmoticon(leftMiddleCard.DownEmoticonColoredPart, downLeftCard.TopEmoticonColoredPart))
            {
                return true;
            }

            downLeftCard.RotateCardToRight();
        }

        return false;
    }

    private bool IsCompleteColoredEmoticon(IEmoticonPart part1, IEmoticonPart part2)
    {
        var isSameColor = part1.EmoticonColor == part2.EmoticonColor;
        var isCompleteEmoticon = part1.EmoticonSide == EmoticonSide.Up && part2.EmoticonSide == EmoticonSide.Down
            || part1.EmoticonSide == EmoticonSide.Down && part2.EmoticonSide == EmoticonSide.Up;
        return isSameColor && isCompleteEmoticon;
    }
}