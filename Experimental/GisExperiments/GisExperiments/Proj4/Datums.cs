using System;
using System.Collections.Generic;
using System.Globalization;

namespace GisExperiments.Proj4
{
    public static class Datums
    {
        public static IDatum GetDatum(string datumCode)
        {
            if (!datums.ContainsKey(datumCode))
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Datum '{0}' is not supported.", datumCode);	
                throw new InvalidOperationException(message);
            }

            return datums[datumCode];
        }

        public static void AddDatum (IDatum datum)
        {
            datums.Add(datum.DatumCode, datum);
        }

        static Datums()
        {
            AddDatum(Datum.ToWgs84("WGS84", "WGS84", "0,0,0", "WGS84"));
            AddDatum (Datum.ToWgs84 ("OSGB36", "airy", "446.448,-125.157,542.060,0.1502,0.2470,0.8421,-20.4894", "Airy 1830"));
        }

        private static Dictionary<string, IDatum> datums = new Dictionary<string, IDatum>();
    }
}