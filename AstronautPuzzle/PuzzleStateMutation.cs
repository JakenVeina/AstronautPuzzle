using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AstronautPuzzle
{
    public record PuzzleStateMutation(
        ImmutableArray<Astronaut> AstronautsMoved,
        StationSection TargetStationSection,
        int TravelTimeMinutes)
    {
        public static PuzzleStateMutation MoveAstronaut(
                Astronaut astronaut,
                StationSection targetStationSection)
            => new PuzzleStateMutation(
                ImmutableArray.Create(astronaut),
                targetStationSection,
                _travelTimeMinutesByAstronaut[astronaut]);

        public static PuzzleStateMutation MoveAstronauts(
                Astronaut astronaut1,
                Astronaut astronaut2,
                StationSection targetStationSection)
            => new PuzzleStateMutation(
                ImmutableArray.Create(astronaut1, astronaut2),
                targetStationSection,
                Math.Max(_travelTimeMinutesByAstronaut[astronaut1], _travelTimeMinutesByAstronaut[astronaut2]));

            private static readonly IReadOnlyDictionary<Astronaut, int> _travelTimeMinutesByAstronaut
                = new Dictionary<Astronaut, int>(5)
                {
                    [Astronaut.Neil] = 1,
                    [Astronaut.Michael] = 2,
                    [Astronaut.Valentina] = 3,
                    [Astronaut.Yuri] = 5,
                    [Astronaut.Edwin] = 10
                };
    }
}
