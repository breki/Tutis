using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeSpaceOrganizer
{
    public class PlacementSolution : ISimulatedAnnealingSituation, ISimulatedAnnealingSolution
    {
        public static PlacementSolution Initial(IEnumerable<Place> places, IEnumerable<Stuff> stuffs)
        {
            PlacementSolution solution = new PlacementSolution(places, stuffs);

            foreach (Place place in places)
                solution.placements.Add(place.Name, new PlaceWithStuff(place));

            return solution;
        }

        public bool IsOptimal { get; }

        public double SituationValue
        {
            get { return SolutionValue; }
        }

        public double SolutionValue
        {
            get
            {
                throw new NotImplementedException("todo next:");
            }
        }

        public TimeSpan RunningTime { get; }

        public bool RunOutOfTime { get; }

        public void AcceptChange()
        {
            throw new NotImplementedException();
        }

        public void AcceptAsFinalSolution(TimeSpan runningTime, bool runOutOfTime)
        {
            throw new NotImplementedException();
        }

        public ISimulatedAnnealingSolution GenerateSolutionCandidate()
        {
            return CloneAsCandidate();
        }

        public void RejectChange()
        {
            throw new NotImplementedException();
        }

        //public bool TryPlace(Stuff stuff, Place place)
        //{
        //    int availableWidth = CalculateAvailableWidth(place);
        //    if (availableWidth < stuff.Width)
        //        return false;

        //    Console.Out.WriteLine("Putting '{0}' into '{1}'", stuff.Name, place.Name);
        //    placements.Add(place.Name, stuff.Name);
        //    return true;
        //}

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            foreach (PlaceWithStuff placeWithStuff in placements.Values)
            {
                foreach (Stuff stuff in placeWithStuff.StuffInside)
                {
                    s.AppendFormat("'{0}' goes to '{1}'", stuff, placeWithStuff.Place.Name);
                    s.AppendLine();
                }
            }

            foreach (PlaceWithStuff placeWithStuff in placements.Values)
            {
                s.AppendFormat("{0} has {1} left", placeWithStuff.Place.Name, placeWithStuff.AvailableWidth);
                s.AppendLine();
            }

            return s.ToString();
        }

        private PlacementSolution(IEnumerable<Place> places, IEnumerable<Stuff> stuffs)
        {
            this.places = places.AsQueryable().ToDictionary(x => x.Name);
            this.stuffs = stuffs.AsQueryable().ToDictionary(x => x.Name);
        }

        private ISimulatedAnnealingSolution CloneAsCandidate()
        {
            PlacementSolution clone = new PlacementSolution(places.Values, stuffs.Values);
            foreach (PlaceWithStuff placement in placements.Values)
            {
                PlaceWithStuff placementClone = placement.Clone();
                clone.placements.Add(placementClone.Place.Name, placementClone);
            }

            return clone;
        }

        private readonly Dictionary<string, Place> places;
        private readonly Dictionary<string, Stuff> stuffs;
        private readonly Dictionary<string, PlaceWithStuff> placements = new Dictionary<string, PlaceWithStuff>();
    }
}