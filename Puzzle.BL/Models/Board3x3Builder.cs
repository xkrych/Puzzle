using Puzzle.BL.Base;
using Puzzle.BL.Enums;
using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

public class Board3x3Builder : BoardBuilder, IBoardBuilder
{
    Board3x3 board = new Board3x3();

    public Board3x3Builder(IFactory<ICard> cardFactory,
        IFactory<IEmoticonPart> emoticonPartFactory)
        :base(cardFactory, emoticonPartFactory)
    {
    }

    public IBoard GetBoard()
    {
        return board;
    }

    public void SetDefaultSquareCards()
    {
        var card1 = CreateCard(1);
        card1.SetParts(CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Yellow),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Red),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green));

        var card2 = CreateCard(2);
        card2.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Green));

        var card3 = CreateCard(3);
        card3.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Red),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Yellow));

        var card4 = CreateCard(4);
        card4.SetParts(CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red));

        var card5 = CreateCard(5);
        card5.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Yellow));

        var card6 = CreateCard(6);
        card6.SetParts(CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Yellow),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Red),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green));

        var card7 = CreateCard(7);
        card7.SetParts(CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Green),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue));

        var card8 = CreateCard(8);
        card8.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Blue),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Blue),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow));

        var card9 = CreateCard(9);
        card9.SetParts(CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Yellow),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Red),
            CreateEmoticonPart(EmoticonSide.Down, EmoticonColor.Green),
            CreateEmoticonPart(EmoticonSide.Up, EmoticonColor.Green));

        board.Cards = new[,]
            {
                {card1, card2, card3},
                {card4, card5, card6},
                {card7, card8, card9}
            };
    }
}
