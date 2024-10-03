using Puzzle.BL.Enums;
using Puzzle.BL.Exceptions;
using Puzzle.BL.Factories;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models
{
    public class Board : IBoard
    {
        private readonly IFactory<ICardMove> cardMoveFactory;
        private readonly IFactory<IEmoticonPart> emoticonPartFactory;
        private readonly IFactory<ICard> cardFactory;

        public Board(IFactory<ICardMove> cardMoveFactory,
            IFactory<IEmoticonPart> emoticonPartFactory,
            IFactory<ICard> cardFactory)
        {
            this.cardMoveFactory = cardMoveFactory;
            this.emoticonPartFactory = emoticonPartFactory;
            this.cardFactory = cardFactory;
            Cards = GetDefaultBoard();
        }

        public int MiddleBoard1DIndex => RowCount / 2 * ColumnCount + ColumnCount / 2;
        public int RowCount => 3;
        public int ColumnCount => 3;
        public ICard[,] Cards { get; }

        public List<int> GetCardIds()
        {
            var cardIds = new List<int>();

            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                {
                    var card = Cards[i, j];
                    cardIds.Add(card.Id);
                }
            }

            return cardIds;
        }

        public void MoveCardsByIds(List<int> cardIdsAfterMove)
        {
            var cardMoves = new List<ICardMove>();

            for (var i = 0; i < cardIdsAfterMove.Count; i++)
            {
                var cardId = cardIdsAfterMove[i];

                if (!TryGetCardPositionById(cardId, out var fromRow, out var fromColumn))
                    throw new CardNotFoundException($"Card with id {cardId} not found.");

                var toRow = i / RowCount;
                var toColumn = i % ColumnCount;

                if (fromRow == toRow && fromColumn == toColumn)
                    continue;

                var cardMove = cardMoveFactory.Create();
                cardMove.FromRow = fromRow;
                cardMove.FromColumn = fromColumn;
                cardMove.ToRow = toRow;
                cardMove.ToColumn = toColumn;
                cardMoves.Add(cardMove);
            }

            MoveCards(cardMoves);
        }

        // o o o
        // o X o
        // o o o
        public ICard GetMiddleCard()
        {
            return Cards[1, 1];
        }

        // X o o
        // o o o
        // o o o
        public ICard GetTopLeftCard()
        {
            return Cards[0, 0];
        }

        // o X o
        // o o o
        // o o o
        public ICard GetTopMiddleCard()
        {
            return Cards[0, 1];
        }

        // o o X
        // o o o
        // o o o
        public ICard GetTopRightCard()
        {
            return Cards[0, 2];
        }

        // o o o
        // o o X
        // o o o
        public ICard GetRightMiddleCard()
        {
            return Cards[1, 2];
        }

        // o o o
        // o o o
        // o o X
        public ICard GetDownRightCard()
        {
            return Cards[2, 2];
        }

        // o o o
        // o o o
        // o X o
        public ICard GetDownMiddleCard()
        {
            return Cards[2, 1];
        }

        // o o o
        // o o o
        // X o o
        public ICard GetDownLeftCard()
        {
            return Cards[2, 0];
        }

        // o o o
        // X o o
        // o o o
        public ICard GetLeftMiddleCard()
        {
            return Cards[1, 0];
        }

        private ICard CreateCard(int id)
        {
            var card = cardFactory.Create();
            card.Id = id;
            return card;
        }

        private IEmoticonPart CreateEmoticonPart(EmoticonSide side, EmoticonColor color)
        {
            var part = emoticonPartFactory.Create();
            part.EmoticonSide = side;
            part.EmoticonColor = color;
            return part;
        }

        private ICard[,] GetDefaultBoard()
        {
            var card1 = CreateCard(1);
            card1.SetParts(CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Yellow),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Red),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green));

            var card2 = CreateCard(2);
            card2.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Green));

            var card3 = CreateCard(3);
            card3.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Red),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Yellow));

            var card4 = CreateCard(4);
            card4.SetParts(CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red));

            var card5 = CreateCard(5);
            card5.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Yellow));

            var card6 = CreateCard(6);
            card6.SetParts(CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Yellow),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Red),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green));

            var card7 = CreateCard(7);
            card7.SetParts(CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Green),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue));
            
            var card8 = CreateCard(8);
            card8.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow));

            var card9 = CreateCard(9);
            card9.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
                CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Green),
                CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green));
            
            return new [,]
            {
                {card1, card2, card3},
                {card4, card5, card6},
                {card7, card8, card9}
            };
        }

        private void MoveCards(List<ICardMove> cardMoves)
        {
            if (cardMoves.Count == 0)
                return;

            var copiedBoard = GetBoardCopy();

            foreach (var cardMove in cardMoves)
            {
                Cards[cardMove.ToRow, cardMove.ToColumn] = copiedBoard[cardMove.FromRow, cardMove.FromColumn];
            }
        }

        private bool TryGetCardPositionById(int cardId, out int row, out int column)
        {
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                {
                    var card = Cards[i, j];

                    if (card.Id == cardId)
                    {
                        row = i;
                        column = j;
                        return true;
                    }
                }
            }

            row = -1;
            column = -1;
            return false;
        }

        private ICard[,] GetBoardCopy()
        {
            var copiedCards = new ICard[RowCount, ColumnCount];

            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                {
                    copiedCards[i, j] = Cards[i, j];
                }
            }

            return copiedCards;
        }
    }
}