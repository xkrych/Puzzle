using Puzzle.BL.Enums;

namespace Puzzle.BL.Interfaces;

public interface ICard
{
    int Id { get; set; }
    IEmoticonPart TopEmoticonColoredPart { get; }
    IEmoticonPart RightEmoticonColoredPart { get; }
    IEmoticonPart DownEmoticonColoredPart { get; }
    IEmoticonPart LeftEmoticonColoredPart { get; }
    public int NumberOfCardSides { get; }
    CardRightRotation GetCardRightRotation();
    void RotateCardToRight();
    void SetParts(IEmoticonPart top,
        IEmoticonPart right,
        IEmoticonPart down,
        IEmoticonPart left);
    bool HasMultipleEmoticonParts(IEmoticonPart emoticonPart, out int partCount);
}
