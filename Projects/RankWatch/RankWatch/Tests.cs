using System.Diagnostics;
using NUnit.Framework;

namespace RankWatch
{
    public class Tests
    {
        [Test]
        public void Test()
        {
            RankFinder finder = new RankFinder();
            RankInfo rankInfo = finder.FindRank(
                "toronto vector map", 
                //"vector map of europe", 
                10);

            Debug.WriteLine("Ranks: ");
            foreach (int rank in rankInfo.Ranks)
                Debug.WriteLine(rank);
        }
    }
}
