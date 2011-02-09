using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;

namespace GnuCashUtils.Framework.Reporting
{
    public enum ReportTimescale
    {
        Daily,
        Weekly,
        TwoWeekly,
        Monthly,
        Quarterly,
        Yearly,
    }

    public class ReportParameters
    {
        public Commodity BaseCurrency
        {
            get { return baseCurrency; }
            set { baseCurrency = value; }
        }

        public string ReportDirectory
        {
            get { return reportDirectory; }
            set { reportDirectory = value; }
        }

        public int ImageWidth
        {
            get { return imageWidth; }
            set { imageWidth = value; }
        }
        public int ImageHeight
        {
            get { return imageHeight; }
            set { imageHeight = value; }
        }

        public DateTime? StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public DateTime? EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        public ReportTimescale Timescale
        {
            get { return timescale; }
            set { timescale = value; }
        }

        public ColorPalette LinePalette
        {
            get { return linePalette; }
        }

        public ColorPalette FillPalette
        {
            get { return fillPalette; }
        }

        public int CommoditiesCount { get { return commodities.Count; } }

        public void IncludeCommodity (Commodity commodity)
        {
            this.commodities[commodity.FullId] = commodity;
        }

        public void IncludeAccount (Account account)
        {
            this.accounts[account.Id] = account;
        }

        public IEnumerable<Commodity> EnumerateCommodities ()
        {
            return commodities.Values;
        }

        public IEnumerable<Account> EnumerateAccounts ()
        {
            return accounts.Values;
        }

        static public string ObjectTypeAccount { get { return "Account"; } }
        static public string ObjectTypeCommodity { get { return "Commodity"; } }

        private string reportDirectory = String.Empty;
        private int imageWidth = 1024;
        private int imageHeight = 768;

        private Commodity baseCurrency;
        private DateTime? startTime;
        private DateTime? endTime;
        private ReportTimescale timescale = ReportTimescale.Monthly;
        private Dictionary<string, Commodity> commodities = new Dictionary<string, Commodity> ();
        private Dictionary<Guid, Account> accounts = new Dictionary<Guid, Account> ();

        private ColorPalette linePalette = new ColorPalette (ColorPaletteType.LineColors);
        private ColorPalette fillPalette = new ColorPalette (ColorPaletteType.FillColors);
    }
}
