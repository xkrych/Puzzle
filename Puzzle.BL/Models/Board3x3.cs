using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

/// <summary>
/// Class for board with cards.
/// </summary>
public class Board3x3 : IBoard
{
    public Board3x3()
    {
        Cards = new ICard[RowCount, ColumnCount];
    }

    // Index of middle card when board is converted to 1D array of cards.
    public int MiddleBoard1DIndex => 4;
    public int RowCount => 3;
    public int ColumnCount => 3;
    public ICard[,] Cards { get; set; }

    // o o o
    // o X o
    // o o o
    public ICard GetMiddleCard()
    {
        return Cards[1, 1];
    }

    // X o o
    // o o o
    // o o o
    public ICard GetTopLeftCard()
    {
        return Cards[0, 0];
    }

    // o X o
    // o o o
    // o o o
    public ICard GetTopMiddleCard()
    {
        return Cards[0, 1];
    }

    // o o X
    // o o o
    // o o o
    public ICard GetTopRightCard()
    {
        return Cards[0, 2];
    }

    // o o o
    // o o X
    // o o o
    public ICard GetRightMiddleCard()
    {
        return Cards[1, 2];
    }

    // o o o
    // o o o
    // o o X
    public ICard GetDownRightCard()
    {
        return Cards[2, 2];
    }

    // o o o
    // o o o
    // o X o
    public ICard GetDownMiddleCard()
    {
        return Cards[2, 1];
    }

    // o o o
    // o o o
    // X o o
    public ICard GetDownLeftCard()
    {
        return Cards[2, 0];
    }

    // o o o
    // X o o
    // o o o
    public ICard GetLeftMiddleCard()
    {
        return Cards[1, 0];
    }

    /// <summary>
    /// Get IDs of all cards on the board.
    /// </summary>
    /// <returns>card IDs</returns>
    public List<int> GetCardIds()
    {
        var cardIds = new List<int>();

        for (var i = 0; i < RowCount; i++)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                var card = Cards[i, j];
                cardIds.Add(card.Id);
            }
        }

        return cardIds;
    }

    /// <summary>
    /// Get row and column number for card with ID.
    /// </summary>
    /// <param name="cardId">card ID</param>
    /// <param name="row">row of the card with ID</param>
    /// <param name="column">column of the card with ID</param>
    /// <returns>true if card was found</returns>
    public bool TryGetCardPositionById(int cardId, out int row, out int column)
    {
        for (var i = 0; i < RowCount; i++)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                var card = Cards[i, j];

                if (card.Id == cardId)
                {
                    row = i;
                    column = j;
                    return true;
                }
            }
        }

        row = -1;
        column = -1;
        return false;
    }

    /// <summary>
    /// Copy current card positions to new array
    /// </summary>
    /// <returns>array with current positions of the cards on the board</returns>
    public ICard[,] GetBoardCopy()
    {
        var copiedCards = new ICard[RowCount, ColumnCount];

        for (var i = 0; i < RowCount; i++)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                copiedCards[i, j] = Cards[i, j];
            }
        }

        return copiedCards;
    }
}