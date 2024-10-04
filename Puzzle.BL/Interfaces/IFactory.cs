namespace Puzzle.BL.Interfaces;

public interface IFactory<out T>
{
    T Create();
}
