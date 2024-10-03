namespace Puzzle.BL.Factories
{
    public interface IFactory<out T>
    {
        T Create();
    }
}
