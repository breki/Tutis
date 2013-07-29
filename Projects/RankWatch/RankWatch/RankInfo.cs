using System.Collections.Generic;

namespace RankWatch
{
    public class RankInfo
    {
        public RankInfo(string searchKeyword)
        {
            this.searchKeyword = searchKeyword;
        }

        public string SearchKeyword
        {
            get { return searchKeyword; }
        }

        public IList<int> Ranks
        {
            get { return ranks; }
        }

        private string searchKeyword;
        private List<int> ranks = new List<int>();
    }
}