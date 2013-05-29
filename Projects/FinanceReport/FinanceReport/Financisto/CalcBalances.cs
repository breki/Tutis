﻿using System;
using System.Collections.Generic;

namespace FinanceReport.Financisto
{
    public class CalcBalances : CalcBase
    {
        public static AmountByTime Calc(AmountByTime balancesDiffs)
        {
            decimal currentBalance = 0;
            AmountByTime totalBalances = new AmountByTime ();

            foreach (KeyValuePair<DateTime, decimal> balance in balancesDiffs.Amounts)
            {
                DateTime date = balance.Key;
                currentBalance += balance.Value;
                totalBalances.SetAmount (date, currentBalance);
            }

            return totalBalances;
        }
    }
}