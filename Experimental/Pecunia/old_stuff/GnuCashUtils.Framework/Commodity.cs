using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GnuCashUtils.Framework
{
    [SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes")]
    public class Commodity : IComparable<Commodity>
    {
        public string Space
        {
            get { return space; }
            set { space = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Fraction
        {
            get { return fraction; }
            set { fraction = value; }
        }

        /// <summary>
        /// Gets the full (unique) ID of the commodity.
        /// </summary>
        /// <value>The full (unique) ID of the commodity.</value>
        public string FullId
        {
            get
            {
                return ConstructFullId (space, id);
            }
        }

        static public string ConstructFullId (string space, string id)
        {
            return String.Format (System.Globalization.CultureInfo.InvariantCulture,
                "{0}.{1}", space, id);
        }

        #region IComparable<Commodity> Members

        public int CompareTo (Commodity other)
        {
            if (this == other)
                return 0;

            if (other == null)
                return 1;

            int c;
            c = String.Compare (space, other.space);
            if (c != 0)
                return c;
            c = String.Compare (id, other.id);
            if (c != 0)
                return c;

            return 0;
        }

        #endregion

        private int fraction = 1000000;
        private string name;
        private string id;
        private string space;
    }
}
