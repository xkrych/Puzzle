using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Base
{
    public class BoardBuilder
    {
        IFactory<ICard> cardFactory;
        IFactory<IEmoticonPart> emoticonPartFactory;
        IFactory<IBoard> boardFactory;

        public BoardBuilder(IFactory<ICard> cardFactory,
            IFactory<IEmoticonPart> emoticonPartFactory,
            IFactory<IBoard> boardFactory)
        {
            this.cardFactory = cardFactory;
            this.emoticonPartFactory = emoticonPartFactory;
            this.boardFactory = boardFactory;
            board = boardFactory.Create();
        }

        protected IBoard board { get; set; }

        protected ICard CreateCard(int id)
        {
            var card = cardFactory.Create();
            card.Id = id;
            return card;
        }

        protected IEmoticonPart CreateEmoticonPart(EmoticonSide side, EmoticonColor color)
        {
            var part = emoticonPartFactory.Create();
            part.EmoticonSide = side;
            part.EmoticonColor = color;
            return part;
        }
    }
}
