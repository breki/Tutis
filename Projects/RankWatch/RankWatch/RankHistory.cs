using System;
using System.Collections.Generic;

namespace RankWatch
{
    public class RankHistory
    {
        public RankHistory(string searchKeyword)
        {
            this.searchKeyword = searchKeyword;
        }

        public string SearchKeyword
        {
            get { return searchKeyword; }
        }

        public SortedList<int, int> RanksByDays
        {
            get { return ranksByDays; }
        }

        public void AddRank (DateTime dateTime, int rank)
        {
            int daysDiff = (dateTime.Date - new DateTime(2013, 01, 01).Date).Days;
            ranksByDays[daysDiff] = rank;
        }

        private readonly string searchKeyword;
        private SortedList<int, int> ranksByDays = new SortedList<int, int>();
    }
}