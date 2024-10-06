using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Base
{
    public class BoardBuilder
    {
        IFactory<ICard> cardFactory;
        IFactory<IEmoticonPart> emoticonPartFactory;

        public BoardBuilder(IFactory<ICard> cardFactory,
            IFactory<IEmoticonPart> emoticonPartFactory)
        {
            this.cardFactory = cardFactory;
            this.emoticonPartFactory = emoticonPartFactory;
        }

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
