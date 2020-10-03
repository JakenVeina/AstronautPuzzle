using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AstronautPuzzle
{
    public static class EntryPoint
    {
        public static void Main()
        {
            string? searchType = null;
            while(true)
            {
                Console.Write("Depth-first or Breadth-first search? (d/b): ");
                searchType = Console.ReadLine();
                if ((searchType == "d") || (searchType == "b"))
                    break;
                Console.WriteLine("Invalid Selection");
                Console.WriteLine();
            }

            try
            {
                var findStarted = DateTime.UtcNow;

                var initialState = new PuzzleState(
                    AirTimeMinutesRemaining:    21,
                    AstronautLocations:         ImmutableDictionary.CreateRange(new[]
                    {
                        new KeyValuePair<Astronaut, StationSection>(Astronaut.Neil,         StationSection.SectionA),
                        new KeyValuePair<Astronaut, StationSection>(Astronaut.Michael,      StationSection.SectionA),
                        new KeyValuePair<Astronaut, StationSection>(Astronaut.Valentina,    StationSection.SectionA),
                        new KeyValuePair<Astronaut, StationSection>(Astronaut.Yuri,         StationSection.SectionA),
                        new KeyValuePair<Astronaut, StationSection>(Astronaut.Edwin,        StationSection.SectionA)
                    }),
                    SuitsLocation:              StationSection.SectionA);

                var (solution, solutionsChecked) = (searchType == "d")
                    ? PuzzleSolution.FindDepth(initialState)
                    : PuzzleSolution.FindBreadth(initialState);

                var findDuration = DateTime.UtcNow - findStarted;

                Console.WriteLine("Solution found");
                Console.WriteLine("Steps:");
                foreach (var mutation in solution.Mutations)
                    Console.WriteLine($"\t{string.Join(" and ", mutation.AstronautsMoved)} spend{((mutation.AstronautsMoved.Length == 1) ? "s" : "")} {mutation.TravelTimeMinutes} minute{((mutation.TravelTimeMinutes == 1) ? "" : "s")} moving to {mutation.TargetStationSection}");
                var finalState = solution.States[^1];
                Console.WriteLine($"{finalState.AirTimeMinutesRemaining} minute{((finalState.AirTimeMinutesRemaining == 1) ? "" : "s")} of air remain{((finalState.AirTimeMinutesRemaining == 1) ? "s" : "")}");

                Console.WriteLine($"{solutionsChecked} solutions checked");
                Console.WriteLine($"Solution took {findDuration} to find");
            }
            catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "initialState")
            {
                Console.WriteLine("Unable to find a solution for the given initial state:");
                var initialState = (PuzzleState)ex.ActualValue!;

                foreach (var astronautLocation in initialState.AstronautLocations)
                    Console.WriteLine($"{astronautLocation.Key} is at {astronautLocation.Value}");
                Console.WriteLine($"Suits are at {initialState.SuitsLocation}");
                Console.WriteLine($"{initialState.AirTimeMinutesRemaining} minute{((initialState.AirTimeMinutesRemaining == 1) ? "" : "s")} of air remain{((initialState.AirTimeMinutesRemaining == 1) ? "s" : "")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to find a solution, due to faulty logic");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
