using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

/// <summary>
/// Class for one card on the board.
/// </summary>
public class Card : ICard
{
    private int numberOfRightRotations;

    public int Id { get; set; }
    public IEmoticonPart TopEmoticonColoredPart { get; private set; } = new EmoticonPart();
    public IEmoticonPart RightEmoticonColoredPart { get; private set; } = new EmoticonPart();
    public IEmoticonPart DownEmoticonColoredPart { get; private set; } = new EmoticonPart();
    public IEmoticonPart LeftEmoticonColoredPart { get; private set; } = new EmoticonPart();
    public int NumberOfCardSides => 4;
    /// <summary>
    /// Number of emoticons parts on the card.
    /// </summary>
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

    /// <summary>
    /// Get number of emoticons parts on the card.
    /// </summary>
    private void CheckEmoticonPartsCounts()
    {
        EmoticonPartCounts.Clear();
        CheckEmoticonPartCounts(TopEmoticonColoredPart.EmoticonSide, TopEmoticonColoredPart.EmoticonColor);
        CheckEmoticonPartCounts(RightEmoticonColoredPart.EmoticonSide, RightEmoticonColoredPart.EmoticonColor);
        CheckEmoticonPartCounts(LeftEmoticonColoredPart.EmoticonSide, LeftEmoticonColoredPart.EmoticonColor);
        CheckEmoticonPartCounts(DownEmoticonColoredPart.EmoticonSide, DownEmoticonColoredPart.EmoticonColor);
    }

    /// <summary>
    /// Get number of emoticon part on the card.
    /// </summary>
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

    /// <summary>
    /// Get number of emoticon part on the card.
    /// </summary>
    /// <param name="emoticonPart">emoticon part</param>
    /// <param name="partCount">number of emoticon parts on the card</param>
    /// <returns>true if there are multiple identical parts of an emoticon on a card</returns>
    public bool HasMultipleEmoticonParts(IEmoticonPart emoticonPart, out int partCount)
    {
        var side = (emoticonPart.EmoticonSide == EmoticonSide.Down) ? EmoticonSide.Up : EmoticonSide.Down;
        var color = emoticonPart.EmoticonColor;

        if (EmoticonPartCounts.ContainsKey((side, color)))
        {
            partCount = EmoticonPartCounts[(side, color)];
            return partCount > 1;
        }

        partCount = 1;
        return false;
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