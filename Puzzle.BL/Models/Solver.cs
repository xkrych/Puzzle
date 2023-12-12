using Puzzle.BL.Enums;
using Puzzle.BL.Exceptions;
using Puzzle.BL.Factories;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models
{
    public class Solver : ISolver
    {
        private const int numberOfCardSides = 4;
        private readonly IFactory<IPermutationGenerator> permutationGeneratorFactory;
        private IPermutationGenerator? permutationGenerator;
        private readonly List<int> cardIdsInMiddle = new();

        public Solver(IBoard board,
            IFactory<IPermutationGenerator> permutationGeneratorFactory)
        {
            Board = board;
            this.permutationGeneratorFactory = permutationGeneratorFactory;
        }
        
        public IBoard Board { get; }

        public Task<bool> SolveBoard()
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
                        RearrangeBoardCards(out isAllPermutations, out isAllCardsInMiddle);
                    }
                    else
                        isFirstSolveTry = false;

                    if (!SolveTopMiddleCard())
                        continue;

                    if (!SolveRightMiddleCard())
                        continue;

                    if (!SolveDownMiddleCard())
                        continue;

                    if (!SolveLeftMiddleCard())
                        continue;

                    if (!SolveTopLeftCard())
                        continue;

                    if (!SolveTopRightCard())
                        continue;

                    if (!SolveDownRightCard())
                        continue;

                    if (!SolveDownLeftCard())
                        continue;

                    isSolved = true;
                }

                return isSolved;
            });
        }

        private List<int> GetCardIdsAroundBoard()
        {
            var cardIds = Board.GetCardIds();
            var cardIdsAroundBoard = new List<int>();
            var i = 0;

            while (i < cardIds.Count)
            {
                var row = i / Board.RowCount;
                var column = i % Board.ColumnCount;

                if (row == Board.RowCount / 2 && column == Board.ColumnCount / 2)
                {
                    i++;
                    continue;
                }

                cardIdsAroundBoard.Add(cardIds[i++]);
            }

            return cardIdsAroundBoard;
        }

        private void MoveNextCardToMiddleOfTheBoard(out bool isAllCardsInMiddle)
        {
            // if all cards has been in the middle of the board, set flag isAllCardsInMiddle
            isAllCardsInMiddle = cardIdsInMiddle.Count == Board.RowCount * Board.ColumnCount;
            if (isAllCardsInMiddle)
                return;

            var cardIds = Board.GetCardIds();
            for (var i = 0; i < cardIds.Count; i++)
            {
                var cardId = cardIds[i];
                if (!cardIdsInMiddle.Contains(cardId))
                {
                    cardIdsInMiddle.Add(cardId);
                    // swap card with card in the middle of the board
                    cardIds[i] = cardIds[Board.MiddleBoard1DIndex];
                    cardIds[Board.MiddleBoard1DIndex] = cardId;
                    Board.MoveCardsByIds(cardIds);
                    break;
                }
            }
        }

        private List<int> GetNextEdgeCardIdsPermutation()
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
                if (i == Board.MiddleBoard1DIndex && !middleCardInserted)
                {
                    // in the middle of the array is element which is in the middle of the board
                    cardIdsAfterMove.Add(Board.GetMiddleCard().Id);
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

        private IPermutationGenerator CreatePermutaionGenerator()
        {
            // get list of card is around board
            var edgeCardIds = GetCardIdsAroundBoard();
            // create permutation generator according to card IDs
            var generator = permutationGeneratorFactory.Create();
            generator.Init(edgeCardIds);
            return generator;
        }

        private void RearrangeEdgeBoardCards()
        {
            // get next permutaion of edge cards
            var cardIdsAfterMove = GetNextEdgeCardIdsPermutation();
            // move cards around board accoeding to permutation
            Board.MoveCardsByIds(cardIdsAfterMove);
        }

        private void RearrangeBoardCards(out bool isAllPermutations, out bool isAllCardsInMiddle)
        {
            isAllPermutations = false;
            isAllCardsInMiddle = false;

            if (permutationGenerator == null)
            {
                // move next card to middle of the board
                MoveNextCardToMiddleOfTheBoard(out isAllCardsInMiddle);
                permutationGenerator = CreatePermutaionGenerator();
            }
            else if (permutationGenerator is { AllPermutationsGenerated: true })
            {
                isAllPermutations = true;
                // move next card to middle of the board
                MoveNextCardToMiddleOfTheBoard(out isAllCardsInMiddle);
                
                if (!isAllCardsInMiddle)
                    permutationGenerator = CreatePermutaionGenerator();
            }

            if (isAllCardsInMiddle && isAllPermutations)
                return;

            RearrangeEdgeBoardCards();
        }

        // o X o
        // o X o
        // o o o
        // an attempt to rotate the card so that the smiley is complete
        private bool SolveTopMiddleCard()
        {
            var middleCard = Board.GetMiddleCard();
            var topMiddleCard = Board.GetTopMiddleCard();

            for (var i = 0; i < numberOfCardSides; i++)
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
        private bool SolveRightMiddleCard()
        {
            var middleCard = Board.GetMiddleCard();
            var rightMiddleCard = Board.GetRightMiddleCard();

            for (var i = 0; i < numberOfCardSides; i++)
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
        private bool SolveDownMiddleCard()
        {
            var middleCard = Board.GetMiddleCard();
            var downMiddleCard = Board.GetDownMiddleCard();

            for (var i = 0; i < numberOfCardSides; i++)
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
        private bool SolveLeftMiddleCard()
        {
            var middleCard = Board.GetMiddleCard();
            var leftMiddleCard = Board.GetLeftMiddleCard();

            for (var i = 0; i < numberOfCardSides; i++)
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
        private bool SolveTopLeftCard()
        {
            var leftMiddleCard = Board.GetLeftMiddleCard();
            var topMiddleCard = Board.GetTopMiddleCard();
            var topLeftCard = Board.GetTopLeftCard();

            for (var i = 0; i < numberOfCardSides; i++)
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
        private bool SolveTopRightCard()
        {
            var topMiddleCard = Board.GetTopMiddleCard();
            var rightMiddleCard = Board.GetRightMiddleCard();
            var topRightCard = Board.GetTopRightCard();

            for (var i = 0; i < numberOfCardSides; i++)
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
        private bool SolveDownRightCard()
        {
            var rightMiddleCard = Board.GetRightMiddleCard();
            var downMiddleCard = Board.GetDownMiddleCard();
            var downRightCard = Board.GetDownRightCard();

            for (var i = 0; i < numberOfCardSides; i++)
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
        private bool SolveDownLeftCard()
        {
            var downMiddleCard = Board.GetDownMiddleCard();
            var leftMiddleCard = Board.GetLeftMiddleCard();
            var downLeftCard = Board.GetDownLeftCard();

            for (var i = 0; i < numberOfCardSides; i++)
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
}