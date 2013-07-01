using System;

namespace FinanceReport.Analysis
{
    public abstract class CalcBase
    {
        protected static int GetMonthIndex (DateTime date)
        {
            return date.Year * 12 + date.Month;
        }
    }
}