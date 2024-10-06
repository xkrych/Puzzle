using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

/// <summary>
/// Class for a edge middle card containing multiple identical parts of emoticon.
/// Edge middle cards:
/// o X o
/// X o X
/// o X o
/// </summary>
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
