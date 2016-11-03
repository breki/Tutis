using System.Collections.Generic;

namespace HomeSpaceOrganizer
{
    public class PlaceWithStuff
    {
        public PlaceWithStuff(Place place)
        {
            this.place = place;
        }

        public Place Place
        {
            get { return place; }
        }

        public int AvailableWidth
        {
            get
            {
                int takenWidth = 0;

                IList<string> placedStuffNames;
                foreach (Stuff stuff in stuffInside)
                    takenWidth += stuff.Width;

                return place.Width - takenWidth;
            }
        }

        public IEnumerable<Stuff> StuffInside
        {
            get { return stuffInside; }
        }

        public PlaceWithStuff Clone()
        {
            throw new System.NotImplementedException();
        }

        private readonly Place place;
        private readonly List<Stuff> stuffInside = new List<Stuff>();
    }
}