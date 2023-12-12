using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models
{
    public class Card : ICard
    {
        private int numberOfRightRotations;

        public int Id { get; set; }
        public IEmoticonPart TopEmoticonColoredPart { get; private set; } = null!;
        public IEmoticonPart RightEmoticonColoredPart { get; private set; } = null!;
        public IEmoticonPart DownEmoticonColoredPart { get; private set; } = null!;
        public IEmoticonPart LeftEmoticonColoredPart { get; private set; } = null!;

        public void SetParts(IEmoticonPart top, 
            IEmoticonPart right, 
            IEmoticonPart down, 
            IEmoticonPart left)
        {
            TopEmoticonColoredPart = top;
            RightEmoticonColoredPart = right;
            DownEmoticonColoredPart = down;
            LeftEmoticonColoredPart = left;
        }

        public CardRightRotation GetCardRightRotation()
        {
            return (CardRightRotation)(numberOfRightRotations % 4);
        }

        public void RotateCardToRight()
        {
            numberOfRightRotations++;

            var topEmoticonColoredPart = TopEmoticonColoredPart;
            var rightEmoticonColoredPart = RightEmoticonColoredPart;
            var downEmoticonColoredPart = DownEmoticonColoredPart;
            var leftEmoticonColoredPart = LeftEmoticonColoredPart;

            TopEmoticonColoredPart = leftEmoticonColoredPart;
            RightEmoticonColoredPart = topEmoticonColoredPart;
            DownEmoticonColoredPart = rightEmoticonColoredPart;
            LeftEmoticonColoredPart = downEmoticonColoredPart;
        }
    }
}