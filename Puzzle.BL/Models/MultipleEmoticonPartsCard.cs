using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

public class MultipleEmoticonPartsMiddleEdgeCard
{
    public MultipleEmoticonPartsMiddleEdgeCard(ICard card, 
        EdgeMiddleCardPosition position, 
        int samePartsCnt)
    {
        Card = card;
        Position = position;
        SamePartsCnt = samePartsCnt;
    }

    public ICard Card { get; set; }
    public EdgeMiddleCardPosition Position { get; set; }
    public int SamePartsCnt { get; set; }
    public int RotationsCnt { get; set; }
}
