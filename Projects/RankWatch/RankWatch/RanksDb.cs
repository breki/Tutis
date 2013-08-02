using System;
using System.Collections.Generic;

namespace RankWatch
{
    public class RanksDb
    {
        public SortedList<string, RankHistory> Keywords
        {
            get { return keywords; }
        }

        public void AddRank(DateTime dateTime, string keyword, int rank)
        {
            RankHistory hist;
            if (!keywords.TryGetValue(keyword, out hist))
            {
                hist = new RankHistory(keyword);
                keywords.Add(keyword, hist);
            }

            hist.AddRank(dateTime, rank);
        }

        private SortedList<string, RankHistory> keywords = new SortedList<string, RankHistory>();
    }
}