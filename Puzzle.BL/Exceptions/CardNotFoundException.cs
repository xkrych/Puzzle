namespace Puzzle.BL.Exceptions;

public class CardNotFoundException : Exception
{
    public CardNotFoundException(string message) : base(message)
    { }
}