using System;
using NUnit.Framework;

namespace HomeSpaceOrganizer
{
    public class OrganizerTests
    {
        [Test]
        public void Test()
        {
            Place[] places 
                = 
                {
                    new Place("plakar 5", 2, 79, 35, 46),
                    new Place("plakar 4", 1, 79, 34, 46),
                    new Place("plakar 3", 1, 79, 35, 46),
                    new Place("plakar 2", 3, 79, 35, 46),
                    new Place("plakar 1", 4, 79, 35, 46),
                    new Place("kuh omara desno 7", 5, 56, 20, 42),
                    new Place("kuh omara desno 6", 4, 56, 23, 42),
                    new Place("kuh omara desno 5", 1, 56, 40, 51),
                    new Place("kuh omara desno 3", 2, 56, 28, 42),
                    new Place("kuh omara desno 2", 3, 56, 31, 51),
                    new Place("kuh omara desno 1", 4, 56, 33, 51),
                    new Place("kuh omarica desno 2", 4, 36, 32, 42),
                    new Place("kuh omarica desno 1", 3, 36, 24, 51),
                    new Place("kuh omarica levo 2", 4, 46, 26, 42),
                    new Place("kuh omarica levo 1", 3, 46, 31, 51),
                    new Place("dnev soba regal desno 2", 3, 67, 33, 37),
                    new Place("dnev soba regal desno 1", 4, 67, 34, 37),
                    new Place("dnev soba regal LL2", 3, 31, 33, 37),
                    new Place("dnev soba regal LL1", 4, 31, 34, 37),
                    new Place("dnev soba regal LD2", 3, 31, 33, 37),
                    new Place("dnev soba regal LD1", 4, 31, 34, 37),
                };

            Stuff[] stuffs 
                = 
                {
                    //new Stuff("kufer - hilti", 5, 11, 35, 44),           
                    new Stuff("kufer - akum. odvijač", 4, 10, 27, 35),           
                    new Stuff("kufer - orodje (sivo)", 4, 7, 27, 35),           
                    new Stuff("flaše", 4, 30, 35, 44),           
                    new Stuff("Kozmo - hrana", 1, 40, 35, 30),           
                    new Stuff("barve", 5, 30, 20, 45),           
                    new Stuff("škatla za orodje nova", 2, 45, 20, 33),           
                    new Stuff("Actifry", 3, 45, 33, 20),           
                };

            Organizer organizer = new Organizer();
            PlacementSolution solution = organizer.Run(places, stuffs);

            Console.Out.WriteLine("Solution: {0}", solution);
        }
    }
}