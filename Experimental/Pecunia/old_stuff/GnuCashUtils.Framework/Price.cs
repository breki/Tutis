using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GnuCashUtils.Framework
{
    [SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes")]
    public class Price : IComparable<Price>
    {
        public Guid PriceId
        {
            get { return priceId; }
            set { priceId = value; }
        }

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        public Commodity Commodity
        {
            get { return commodity; }
            set { commodity = value; }
        }

        public Commodity Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public DecimalValue Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public override string ToString ()
        {
            return String.Format (System.Globalization.CultureInfo.InvariantCulture,
                "Price (time='{0}', commodity='{1}', currency='{2}', value={3})",
                time.ToShortDateString (), commodity.FullId, currency.FullId, value.Value);
        }

        #region IComparable<Price> Members

        public int CompareTo (Price other)
        {
            if (this == other)
                return 0;

            if (other == null)
                return 1;

            int c;
            c = commodity.CompareTo (other.commodity);
            if (c != 0)
                return c;
            c = DateTime.Compare (time, other.time);
            if (c != 0)
                return c;
            return priceId.CompareTo (other.priceId);
        }

        #endregion

        private Guid priceId = Guid.NewGuid();
        private DateTime time;
        private Commodity commodity;
        private Commodity currency;
        private string source;
        private string type;
        private DecimalValue value;
    }
}
