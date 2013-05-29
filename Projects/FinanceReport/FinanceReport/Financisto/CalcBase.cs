using System;

namespace FinanceReport.Financisto
{
    public abstract class CalcBase
    {
        protected static int GetMonthIndex (DateTime date)
        {
            return date.Year * 12 + date.Month;
        }
    }
}