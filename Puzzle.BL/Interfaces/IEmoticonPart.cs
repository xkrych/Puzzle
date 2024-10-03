using Puzzle.BL.Enums;

namespace Puzzle.BL.Interfaces;

public interface IEmoticonPart
{
    EmoticonSide EmoticonSide { get; set; }
    EmoticonColor EmoticonColor { get; set; }
}