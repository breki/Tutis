using System;
using System.Collections.Generic;

namespace HomeSpaceOrganizer
{
    [Obsolete]
    public class PlacementSituation : ISimulatedAnnealingSituation
    {
        public PlacementSituation(IEnumerable<Place> places)
        {
            foreach (Place place in places)
                this.places.Add(place.Name, new PlaceWithStuff(place));
        }

        public bool IsOptimal { get; }

        public double SituationValue { get; }

        public void AcceptChange()
        {
            throw new NotImplementedException();
        }

        public ISimulatedAnnealingSolution GenerateSolutionCandidate()
        {
            throw new NotImplementedException();
        }

        public void RejectChange()
        {
            throw new NotImplementedException();
        }

        private readonly Dictionary<string, PlaceWithStuff> places = new Dictionary<string, PlaceWithStuff>();
    }
}