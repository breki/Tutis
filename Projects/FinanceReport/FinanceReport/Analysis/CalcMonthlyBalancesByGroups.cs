using System;
using System.Collections.Generic;
using System.Diagnostics;
using FinanceReport.DataModel;

namespace FinanceReport.Analysis
{
    public class CalcMonthlyBalancesByGroups : CalcBase
    {
        public CalcMonthlyBalancesByGroups(Database db)
        {
            this.db = db;
        }

        public CalcMonthlyBalancesByGroups AddGroup(string name, params int[] categories)
        {
            groups.Add(new CategoriesGroup(name, categories));
            return this;
        }

        public CategoriesRangesAmounts Calc ()
        {
            DateTime now = DateTime.Now;
            int nowMonthIndex = GetMonthIndex (now) - GetMonthIndex (new DateTime (2011, 1, 1));

            CategoriesRangesAmounts all = new CategoriesRangesAmounts ();

            DatabaseTable txTable = db.Tables["transactions"];
            DatabaseTable categoriesTable = db.Tables["category"];

            foreach (TableRow row in txTable.Rows.Values)
            {
                Transaction tx = new Transaction(row);

                if (!tx.IsExpenseTransaction)
                    continue;

                int groupId = FindGroupId(tx.Category);

                int monthIndex = GetMonthIndex(tx.Date) - GetMonthIndex(new DateTime(2011, 1, 1));
                if (nowMonthIndex - monthIndex > 13)
                    continue;

                all.AddAmount(
                    groupId, 
                    groups[groupId].Name,
                    monthIndex,
                    tx.FromAmount);
            }

            return all;
        }

        private int FindGroupId(int category)
        {
            for (int groupId = 0; groupId < groups.Count; groupId++)
            {
                CategoriesGroup @group = groups[groupId];
                if (group.HasCategory(category))
                    return groupId;
            }

            return groups.Count - 1;
        }

        private readonly Database db;
        private List<CategoriesGroup> groups = new List<CategoriesGroup>();
    }
}