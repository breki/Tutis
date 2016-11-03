using System.Collections.Generic;

namespace HomeSpaceOrganizer
{
    public class Organizer
    {
        public PlacementSolution Run(IEnumerable<Place> places, IEnumerable<Stuff> stuffs)
        {
            OrganizingProblem problem = new OrganizingProblem(places, stuffs);
            IRandomizer randomizer = new Randomizer(500);

            SimulatedAnnealingWithSolutionUndo<PlacementSolution, PlacementSolution> simann 
                = new SimulatedAnnealingWithSolutionUndo<PlacementSolution, PlacementSolution>(problem, randomizer);
            PlacementSolution solution = simann.Run();
            return solution;

            //return LookForSolution();
        }

        //private PlacementSolution LookForSolution()
        //{
        //    PlacementSolution solution = new PlacementSolution(places, stuffs);

        //    this.stuffs.Sort(SortStuffByAccessibilityAndSize);
        //    this.places.Sort(SortPlacesByAccessibilityAndSize);

        //    foreach (Stuff stuff in this.stuffs)
        //    {
        //        bool stuffPlaced = false;
        //        foreach (Place place in places)
        //        {
        //            if (!stuff.CanFit(place))
        //                continue;

        //            if (solution.TryPlace(stuff, place))
        //            {
        //                stuffPlaced = true;
        //                break;
        //            }
        //        }

        //        if (!stuffPlaced)
        //            throw new NotImplementedException("todo next: could not place '{0}', current situation: {1}".Fmt(stuff.Name, solution));
        //    }

        //    return solution;
        //}

        private static int SortStuffByAccessibilityAndSize(Stuff x, Stuff y)
        {
            int c = x.Accessibility.CompareTo(y.Accessibility);
            if (c != 0)
                return c;

            return x.Volume.CompareTo(y.Volume);
        }

        private static int SortPlacesByAccessibilityAndSize(Place x, Place y)
        {
            int c = x.Accessibility.CompareTo(y.Accessibility);
            if (c != 0)
                return c;

            return x.Volume.CompareTo(y.Volume);
        }
    }
}