using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

public class Card : ICard
{
    private int numberOfRightRotations;

    public int Id { get; set; }
    public IEmoticonPart TopEmoticonColoredPart { get; private set; } = new EmoticonPart();
    public IEmoticonPart RightEmoticonColoredPart { get; private set; } = new EmoticonPart();
    public IEmoticonPart DownEmoticonColoredPart { get; private set; } = new EmoticonPart();
    public IEmoticonPart LeftEmoticonColoredPart { get; private set; } = new EmoticonPart();
    public int NumberOfCardSides => 4;
    public Dictionary<(EmoticonSide, EmoticonColor), int> EmoticonPartCounts { get; private set; } 
        = new Dictionary<(EmoticonSide, EmoticonColor), int>();

    public void SetParts(IEmoticonPart top, 
        IEmoticonPart right, 
        IEmoticonPart down, 
        IEmoticonPart left)
    {
        TopEmoticonColoredPart = top;
        RightEmoticonColoredPart = right;
        DownEmoticonColoredPart = down;
        LeftEmoticonColoredPart = left;
        CheckEmoticonPartsCounts();
    }

    private void CheckEmoticonPartsCounts()
    {
        EmoticonPartCounts.Clear();
        CheckEmoticonPartCounts(TopEmoticonColoredPart.EmoticonSide, TopEmoticonColoredPart.EmoticonColor);
        CheckEmoticonPartCounts(RightEmoticonColoredPart.EmoticonSide, RightEmoticonColoredPart.EmoticonColor);
        CheckEmoticonPartCounts(LeftEmoticonColoredPart.EmoticonSide, LeftEmoticonColoredPart.EmoticonColor);
        CheckEmoticonPartCounts(DownEmoticonColoredPart.EmoticonSide, DownEmoticonColoredPart.EmoticonColor);
    }

    private void CheckEmoticonPartCounts(EmoticonSide side, EmoticonColor color)
    {
        if (!EmoticonPartCounts.ContainsKey((side, color)))
            EmoticonPartCounts.Add((side, color), 0);

        EmoticonPartCounts[(side, color)]++;
    }

    public CardRightRotation GetCardRightRotation()
    {
        return (CardRightRotation)(numberOfRightRotations % NumberOfCardSides);
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