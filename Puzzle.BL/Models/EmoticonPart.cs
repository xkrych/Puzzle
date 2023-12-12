using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models
{
    public class EmoticonPart : IEmoticonPart
    {
        public EmoticonSide EmoticonSide { get; set; }
        public EmoticonColor EmoticonColor { get; set; }
    }
}