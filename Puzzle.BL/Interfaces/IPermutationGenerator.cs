namespace Puzzle.BL.Interfaces;

public interface IPermutationGenerator
{
    bool AllPermutationsGenerated { get; }
    List<int> GetNextPermutation();
    void Init(List<int> input);
}
