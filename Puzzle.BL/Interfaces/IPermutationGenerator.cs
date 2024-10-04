namespace Puzzle.BL.Interfaces;

public interface IPermutationGenerator
{
    List<List<int>> PermutatedNumbers { get; }
    bool AllPermutationsGenerated { get; }
    List<int> GetNextPermutation();
    void Init(List<int> input);
}
