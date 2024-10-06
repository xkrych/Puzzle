using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

/// <summary>
/// Class for emoticon part on the side of one card.
/// </summary>
public class EmoticonPart : IEmoticonPart
{
    public EmoticonSide EmoticonSide { get; set; }
    public EmoticonColor EmoticonColor { get; set; }
}