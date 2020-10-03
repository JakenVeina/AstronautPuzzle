using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AstronautPuzzle
{
    public enum PuzzleSolutionResult
    {
        Unsolved,
        Solved,
        Invalid
    }

    public record PuzzleSolution(
        ImmutableList<PuzzleStateMutation> Mutations,
        ImmutableList<PuzzleState> States,
        PuzzleSolutionResult Result)
    {
        public static PuzzleSolution Build(
                ImmutableList<PuzzleStateMutation> Mutations,
                ImmutableList<PuzzleState> States)
            => new PuzzleSolution(Mutations, States, CalculateResult(States));

        public static (PuzzleSolution solution, int solutionsChecked) FindBreadth(PuzzleState initialState)
        {
            var solutionsChecked = 0;
            var possibleSolutions = new List<PuzzleSolution>();
            possibleSolutions.Add(Build(
                ImmutableList<PuzzleStateMutation>.Empty,
                ImmutableList.Create(initialState)));

            while (possibleSolutions.Count != 0)
            {
                var nextPossibleSolutions = new List<PuzzleSolution>();

                foreach (var solution in possibleSolutions)
                {
                    ++solutionsChecked;

                    if (solution.Result == PuzzleSolutionResult.Solved)
                        return (solution, solutionsChecked);

                    nextPossibleSolutions.AddRange(solution.EnumeratePossibleSolutions()
                        .OrderBy(possibleSolution => possibleSolution.Mutations[^1].TravelTimeMinutes));
                }

                possibleSolutions = nextPossibleSolutions;
            }

            throw new ArgumentOutOfRangeException(nameof(initialState), initialState, "Unable to find solution");
        }

        public static (PuzzleSolution solution, int solutionsChecked) FindDepth(PuzzleState initialState)
        {
            var solutionsChecked = 0;
            var possibleSolutions = new Stack<PuzzleSolution>();
            possibleSolutions.Push(Build(
                ImmutableList<PuzzleStateMutation>.Empty,
                ImmutableList.Create(initialState)));

            while (possibleSolutions.Count != 0)
            {
                var solution = possibleSolutions.Pop();

                ++solutionsChecked;

                if (solution.Result == PuzzleSolutionResult.Solved)
                    return (solution, solutionsChecked);

                foreach (var possibleSolution in solution.EnumeratePossibleSolutions()
                        .OrderByDescending(possibleSolution => possibleSolution.Mutations[^1].TravelTimeMinutes))
                    possibleSolutions.Push(possibleSolution);
            }

            throw new ArgumentOutOfRangeException(nameof(initialState), initialState, "Unable to find solution");
        }

        public PuzzleSolutionResult Result
        {
            get
            {
                if (States.Count == 0)
                    return PuzzleSolutionResult.Invalid;

                var finalState = States[^1];

                if (finalState.AirTimeMinutesRemaining < 0)
                    return PuzzleSolutionResult.Invalid;

                if (finalState.SuitsLocation != StationSection.SectionB)
                    return PuzzleSolutionResult.Unsolved;

                return finalState.AstronautLocations.All(astronautLocation => astronautLocation.Value == StationSection.SectionB)
                    ? PuzzleSolutionResult.Solved
                    : PuzzleSolutionResult.Unsolved;
            }
        }

        private static PuzzleSolutionResult CalculateResult(ImmutableList<PuzzleState> states)
        {
            if (states.Count == 0)
                return PuzzleSolutionResult.Invalid;

            var finalState = states[^1];

            return !finalState.IsValid ? PuzzleSolutionResult.Invalid
                : finalState.IsSolution ? PuzzleSolutionResult.Solved
                                        : PuzzleSolutionResult.Unsolved;
        }

        private IEnumerable<PuzzleSolution> EnumeratePossibleSolutions()
        {
            if (Result == PuzzleSolutionResult.Invalid)
                return Enumerable.Empty<PuzzleSolution>();

            var currentState = States[^1];

            var astronautsAtSuitLocation = currentState.AstronautLocations
                .Where(astronautLocation => astronautLocation.Value == currentState.SuitsLocation)
                .Select(astronautLocation => astronautLocation.Key);

            var possibleMutations = (currentState.SuitsLocation == StationSection.SectionA)
                ? astronautsAtSuitLocation
                    .SelectMany(astronaut1 => astronautsAtSuitLocation
                        .Where(astronaut2 => astronaut2 != astronaut1)
                        .Select(astronaut2 => PuzzleStateMutation.MoveAstronauts(astronaut1, astronaut2, StationSection.SectionB)))
                : astronautsAtSuitLocation
                    .Select(astronaut => PuzzleStateMutation.MoveAstronaut(astronaut, StationSection.SectionA));

            return possibleMutations
                .Select(possibleMutation => (mutation: possibleMutation, state: currentState.Mutate(possibleMutation)))
                .Where(possibleSolution => !States.Contains(possibleSolution.state))
                .Select(possibleSolution => Build(
                    Mutations.Add(possibleSolution.mutation),
                    States.Add(possibleSolution.state)));
        }
    }
}
