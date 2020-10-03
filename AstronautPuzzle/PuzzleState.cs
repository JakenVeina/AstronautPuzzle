using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AstronautPuzzle
{
    public enum Astronaut
    {
        Neil,
        Michael,
        Valentina,
        Yuri,
        Edwin
    }

    public enum StationSection
    {
        SectionA,
        SectionB
    }

    public record PuzzleState(
        int AirTimeMinutesRemaining,
        ImmutableDictionary<Astronaut, StationSection> AstronautLocations,
        StationSection SuitsLocation)
    {
        public PuzzleState Mutate(PuzzleStateMutation mutation)
        {
            if (SuitsLocation == mutation.TargetStationSection)
                throw new InvalidOperationException($"Unable to move astronats to {mutation.TargetStationSection}: Suits are not here");

            foreach (var astronaut in mutation.AstronautsMoved)
                if (AstronautLocations[astronaut] == mutation.TargetStationSection)
                    throw new InvalidOperationException($"Unable to move astronaut {astronaut} to {mutation.TargetStationSection}: Astronaut is already there");

            return new PuzzleState(
                AirTimeMinutesRemaining - mutation.TravelTimeMinutes,
                AstronautLocations.SetItems(mutation.AstronautsMoved
                    .Select(astronaut => new KeyValuePair<Astronaut, StationSection>(astronaut, mutation.TargetStationSection))),
                mutation.TargetStationSection);
        }

        public bool IsSolution
            => (SuitsLocation == StationSection.SectionB)
                && IsValid
                && AstronautLocations.All(astronautLocation => astronautLocation.Value == StationSection.SectionB);

        public bool IsValid
            => AirTimeMinutesRemaining >= 0;

        public virtual bool Equals(PuzzleState? other)
            => (other is not null)
                && (AirTimeMinutesRemaining == other.AirTimeMinutesRemaining)
                && (SuitsLocation == other.SuitsLocation)
                && AstronautLocations[Astronaut.Neil] == other.AstronautLocations[Astronaut.Neil]
                && AstronautLocations[Astronaut.Michael] == other.AstronautLocations[Astronaut.Michael]
                && AstronautLocations[Astronaut.Valentina] == other.AstronautLocations[Astronaut.Valentina]
                && AstronautLocations[Astronaut.Yuri] == other.AstronautLocations[Astronaut.Yuri]
                && AstronautLocations[Astronaut.Edwin] == other.AstronautLocations[Astronaut.Edwin];

        public override int GetHashCode()
            => HashCode.Combine(
                AirTimeMinutesRemaining,
                SuitsLocation,
                AstronautLocations[Astronaut.Neil],
                AstronautLocations[Astronaut.Michael],
                AstronautLocations[Astronaut.Valentina],
                AstronautLocations[Astronaut.Yuri],
                AstronautLocations[Astronaut.Edwin]);
    }
}
