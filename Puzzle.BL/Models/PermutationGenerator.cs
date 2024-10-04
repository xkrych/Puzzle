using Puzzle.BL.Interfaces;

namespace Puzzle.BL.Models;

public class PermutationGenerator : IPermutationGenerator
{
    private int numberOfGeneratedPermutations;
    
    public List<List<int>> PermutatedNumbers { get; private set; } = new();
    public bool AllPermutationsGenerated => PermutatedNumbers.Count == numberOfGeneratedPermutations;

    public void Init(List<int> input)
    {
        Heaps(PermutatedNumbers, input, input.Count);
    }

    public List<int> GetNextPermutation()
    {
        if (numberOfGeneratedPermutations == PermutatedNumbers.Count)
            throw new IndexOutOfRangeException("Permutation index is out of range.");

        return PermutatedNumbers[numberOfGeneratedPermutations++];
    }

    // inspired by: https://code-maze.com/csharp-generate-permutations/
    private void Heaps(List<List<int>> result, List<int> input, int size)
    {
        if (size == 1)
        {
            result.Add(new List<int>(input));
        }
        else
        {
            for (var i = 0; i < size; i++)
            {
                Heaps(result, input, size - 1);

                if (size % 2 == 1)
                    (input[size - 1], input[0]) = (input[0], input[size - 1]);
                else
                    (input[size - 1], input[i]) = (input[i], input[size - 1]);
            }
        }
    }
}