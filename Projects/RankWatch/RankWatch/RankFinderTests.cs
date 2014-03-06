using System.Diagnostics;
using NUnit.Framework;

namespace RankWatch
{
    public class RankFinderTests
    {
        [Test]
        public void Test()
        {
            GoogleSearchRequestBuilder requestBuilder = new GoogleSearchRequestBuilder ();

            RankFinder finder = new RankFinder();
            RankInfo rankInfo = finder.FindRank(
                requestBuilder,
                "toronto vector map", 
                //"vector map of europe", 
                true,
                10);

            Debug.WriteLine("Ranks: ");
            foreach (int rank in rankInfo.Ranks)
                Debug.WriteLine(rank);
        }
    }
}
