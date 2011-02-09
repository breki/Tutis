using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCashUtils.Framework
{
    public interface IPriceDataMiner
    {
        void MineForPrices (Book gnuCashbook, PriceDataMiningSettings settings, IList<Price> foundPrices);
    }
}
