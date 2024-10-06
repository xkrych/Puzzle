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

    /// <summary>
    /// Attempt to reposition the cards on the board so that 
    /// every two adjacent cards form a complete emoticon.
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
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
            int cnt = 0;

            // The cycle ends when all cards have been resolved or when all permutations
            // have been generated and all cards have been in the center of the board.
            while (!isSolved && (!isAllPermutations || !isAllCardsInMiddle))
            {
                cnt++;

                if (!isFirstSolveTry)
                {
                    RearrangeBoardCards(board, out isAllPermutations, out isAllCardsInMiddle);
                }
                else
                    isFirstSolveTry = false;

                if (!SolveEdgeCards(board))
                    continue;

                isSolved = true;
            }

            return isSolved;
        });
    }

    /// <summary>
    /// Attempt to reposition the edge cards on the board so that 
    /// every two adjacent cards form a complete emoticon and every
    /// edge card form complete emoticon with card in the middle of
    /// the board.
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
    private bool SolveEdgeCards(IBoard board)
    {
        if (!SolveEdgeMiddleCards(board))
            return false;

        var multipleSameEmoticonPartsCards = new List<MultipleEmoticonPartsMiddleEdgeCard>();
        var middleCard = board.GetMiddleCard();
        var topMiddleCard = board.GetTopMiddleCard();
        var rightMiddleCard = board.GetRightMiddleCard();
        var downMiddleCard = board.GetDownMiddleCard();
        var leftMiddleCard = board.GetLeftMiddleCard();

        CheckMultipleEmoticonPartsCard(topMiddleCard, multipleSameEmoticonPartsCards, 
            middleCard.TopEmoticonColoredPart, EdgeMiddleCardPosition.Top);
        CheckMultipleEmoticonPartsCard(rightMiddleCard, multipleSameEmoticonPartsCards,
            middleCard.RightEmoticonColoredPart, EdgeMiddleCardPosition.Right);
        CheckMultipleEmoticonPartsCard(downMiddleCard, multipleSameEmoticonPartsCards,
            middleCard.DownEmoticonColoredPart, EdgeMiddleCardPosition.Down);
        CheckMultipleEmoticonPartsCard(leftMiddleCard, multipleSameEmoticonPartsCards,
            middleCard.LeftEmoticonColoredPart, EdgeMiddleCardPosition.Left);

        if (multipleSameEmoticonPartsCards.Count > 0)
        {
            if (SolveMultipleSameEmoticonPartsCards(board, multipleSameEmoticonPartsCards))
                return true;
        }
        else
        {
            if (SolveCornerCards(board))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Check if there are multiple identical parts of the emoticon on the card, 
    /// and if so, insert the card into the card list.
    /// </summary>
    /// <param name="card">edge middle card</param>
    /// <param name="multipleSameEmoticonPartsCards">all cards with multiple equal parts of a emoticon</param>
    /// <param name="emoticonPart">emoticon part</param>
    /// <param name="position">postition of the edge middle card</param>
    private void CheckMultipleEmoticonPartsCard(ICard card, 
        List<MultipleEmoticonPartsMiddleEdgeCard> multipleSameEmoticonPartsCards,
        IEmoticonPart emoticonPart,
        EdgeMiddleCardPosition position)
    {
        if (card.HasMultipleEmoticonParts(emoticonPart, out var cnt))
            multipleSameEmoticonPartsCards.Add(new MultipleEmoticonPartsMiddleEdgeCard(card, position, cnt));
    }

    /// <summary>
    /// Attempt to reposition the edge cards with multiple identical parts of the emoji on the 
    /// board so that every two adjacent cards form a complete emoticon and each edge card forms 
    /// a complete emoticon with the card in the middle of the board.
    /// </summary>
    /// <param name="board">board</param>
    /// <param name="multipleSameEmoticonPartsCards">all cards with multiple equal parts of a emoticon</param>
    /// <returns>true if attempt was succesfull</returns>
    private bool SolveMultipleSameEmoticonPartsCards(IBoard board, 
        List<MultipleEmoticonPartsMiddleEdgeCard> multipleSameEmoticonPartsCards)
    {
        var allRotationsPerformed = false;

        while (!allRotationsPerformed)
        {
            if (SolveCornerCards(board))
                return true;

            PerformEdgeMiddleCardsRotations(multipleSameEmoticonPartsCards, 
                board,
                multipleSameEmoticonPartsCards.Count - 1,
                out allRotationsPerformed);
        }

        return false;
    }

    /// <summary>
    /// Rotates cards with multiple equal parts of a emoticon.
    /// </summary>
    /// <param name="multipleSameEmoticonPartsCards">all cards with multiple equal parts of a emoticon</param>
    /// <param name="board">boad</param>
    /// <param name="rotatedCardIndex">current rotated card</param>
    /// <param name="allRotationsPerformed">flag indicating whether all card rotations have been completed</param>
    private void PerformEdgeMiddleCardsRotations(List<MultipleEmoticonPartsMiddleEdgeCard> multipleSameEmoticonPartsCards,
        IBoard board,
        int rotatedCardIndex,
        out bool allRotationsPerformed)
    {
        if (rotatedCardIndex < 0)
        {
            allRotationsPerformed = false;
            return;
        }

        var card = multipleSameEmoticonPartsCards[rotatedCardIndex];

        // perform rotation
        if (card.Position == EdgeMiddleCardPosition.Top)
            SolveTopMiddleCardFromNextSide(board);
        else if (card.Position == EdgeMiddleCardPosition.Right)
            SolveRightMiddleCardFromNextSide(board);
        else if (card.Position == EdgeMiddleCardPosition.Down)
            SolveDownMiddleCardFromNextSide(board);
        else
            SolveLeftMiddleCardFromNextSide(board);

        card.RotationsCnt++;

        // if the last card has been completely rotated,
        // then all rotations have been made
        if (card.RotationsCnt == card.SamePartsCnt && rotatedCardIndex == 0)
        {
            allRotationsPerformed = true;
            return;
        }

        // when a card is completely rotated, the following card is rotated
        if (card.RotationsCnt == card.SamePartsCnt)
        {
            PerformEdgeMiddleCardsRotations(multipleSameEmoticonPartsCards, board, 
                rotatedCardIndex - 1, out allRotationsPerformed);
        }

        allRotationsPerformed = false;
    }

    /// <summary>
    /// Attempt to reposition the edge middle cards on the board so that 
    /// every card forms complete emoticon with card in the middle of 
    /// the board.
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
    private bool SolveEdgeMiddleCards(IBoard board)
    {
        if (!SolveTopMiddleCardFromNextSide(board))
            return false;

        if (!SolveRightMiddleCardFromNextSide(board))
            return false;

        if (!SolveDownMiddleCardFromNextSide(board))
            return false;

        if (!SolveLeftMiddleCardFromNextSide(board))
            return false;

        if (SolveCornerCards(board))
            return true;

        return true;
    }

    /// <summary>
    /// Attempt to reposition the edge corner cards on the board so that 
    /// every card forms complete emoticon with card in the middle of 
    /// the board.
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
    private bool SolveCornerCards(IBoard board)
    {
        if (!SolveTopLeftCard(board))
            return false;

        if (!SolveTopRightCard(board))
            return false;

        if (!SolveDownRightCard(board))
            return false;

        if (!SolveDownLeftCard(board))
            return false;

        return true;
    }

    /// <summary>
    /// Get IDs of the edge cards.
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>list of cards IDs</returns>
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

    /// <summary>
    /// Reposition of the edge cards based on number permutation.
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>list of card IDs on the board after repostion</returns>
    /// <exception cref="PermutationGenerationException"></exception>
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

    /// <summary>
    /// Finding the appropriate part of the emoticon from the next 
    /// side of the card so that adjacent cards form a complete emoticon.
    // o X o
    // o X o
    // o o o
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
    private bool SolveTopMiddleCardFromNextSide(IBoard board)
    {
        var middleCard = board.GetMiddleCard();
        var topMiddleCard = board.GetTopMiddleCard();
        topMiddleCard.RotateCardToRight();

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

    /// <summary>
    /// Finding the appropriate part of the emoticon from the next 
    /// side of the card so that adjacent cards form a complete emoticon.
    // o o o
    // o X X
    // o o o
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
    private bool SolveRightMiddleCardFromNextSide(IBoard board)
    {
        var middleCard = board.GetMiddleCard();
        var rightMiddleCard = board.GetRightMiddleCard();
        rightMiddleCard.RotateCardToRight();

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

    /// <summary>
    /// Finding the appropriate part of the emoticon from the next 
    /// side of the card so that adjacent cards form a complete emoticon.
    // o o o
    // o X o
    // o X o
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
    private bool SolveDownMiddleCardFromNextSide(IBoard board)
    {
        var middleCard = board.GetMiddleCard();
        var downMiddleCard = board.GetDownMiddleCard();
        downMiddleCard.RotateCardToRight();

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

    /// <summary>
    /// Finding the appropriate part of the emoticon from the next 
    /// side of the card so that adjacent cards form a complete emoticon.
    // o o o
    // X X o
    // o o o
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
    private bool SolveLeftMiddleCardFromNextSide(IBoard board)
    {
        var middleCard = board.GetMiddleCard();
        var leftMiddleCard = board.GetLeftMiddleCard();
        leftMiddleCard.RotateCardToRight();

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

    /// <summary>
    /// Attempt to rotate a card so that adjacent cards form complete emoticons.
    // X X o
    // X o o
    // o o o
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
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

    /// <summary>
    /// Attempt to rotate a card so that adjacent cards form complete emoticons.
    // o X X
    // o o X
    // o o o
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
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

    /// <summary>
    /// Attempt to rotate a card so that adjacent cards form complete emoticons.
    // o o o
    // o o X
    // o X X
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
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

    /// <summary>
    /// Attempt to rotate a card so that adjacent cards form complete emoticons.
    // o o o
    // X o o
    // X X o
    /// </summary>
    /// <param name="board">board</param>
    /// <returns>true if attempt was succesfull</returns>
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

    /// <summary>
    /// Determine whether two parts of an emoticon form a complete colored emoticon.
    /// </summary>
    /// <param name="part1">one part of emoticon</param>
    /// <param name="part2">one part of emoticon</param>
    /// <returns>true if two parts of emoticon form complete colored emoticon</returns>
    private bool IsCompleteColoredEmoticon(IEmoticonPart part1, IEmoticonPart part2)
    {
        var isSameColor = part1.EmoticonColor == part2.EmoticonColor;
        var isCompleteEmoticon = part1.EmoticonSide == EmoticonSide.Up && part2.EmoticonSide == EmoticonSide.Down
            || part1.EmoticonSide == EmoticonSide.Down && part2.EmoticonSide == EmoticonSide.Up;
        return isSameColor && isCompleteEmoticon;
    }
}