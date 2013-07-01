using System;
using System.Diagnostics;
using FinanceReport.Analysis;
using FinanceReport.DataModel;
using NUnit.Framework;

namespace FinanceReport.Tests
{
    public class MonthlyBalancesByGroupsTests
    {
        [Test]
        public void Test()
        {
            Database db = FinancistoReadingTests.FetchLatestFinancistoData();
            DatabaseTable categoriesTable = db.Tables["category"];

            CalcMonthlyBalancesByGroups calc = new CalcMonthlyBalancesByGroups(db);
            calc
                .AddGroup("bencin", 2)
                .AddGroup("avto - ostali stroški", 3, 16, 17, 18, 6, 30)
                .AddGroup("hrana in nakupi", 4, 5, 10)
                .AddGroup("kredit", 19)
                .AddGroup("stanovanje", 12, 34, 35)
                .AddGroup("ostalo");

            foreach (TableRow row in categoriesTable.Rows)
            {
                Debug.WriteLine(row);
            }

            CategoriesRangesAmounts data = calc.Calc();
        } 
    }
}