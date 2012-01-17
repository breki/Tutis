using System;
using System.Globalization;
using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public class Datum : IDatum
    {
        public static Datum ToWgs84 (string datumCode, string ellipsoid, string toWgs84Params, string datumName)
        {
            Datum datum = new Datum ();
            datum.DatumCode = datumCode;
            datum.DatumName = datumName;
            datum.DatumType = DatumType.Wgs84;
            datum.Ellipsoid = Ellipsoids.GetEllipsoid (ellipsoid);

            datum.DatumParams = ParseDatumParams (toWgs84Params);

            if (datum.DatumParams.Length > 0)
            {
                if (datum.DatumParams[0] != 0 || datum.DatumParams[1] != 0 || datum.DatumParams[2] != 0)
                    datum.DatumType = DatumType.Param3;

                if (datum.DatumParams.Length > 3)
                {
                    if (datum.DatumParams[3] != 0 || datum.DatumParams[4] != 0 ||
                        datum.DatumParams[5] != 0 || datum.DatumParams[6] != 0)
                    {
                        datum.DatumType = DatumType.Param7;
                        datum.DatumParams[3] *= 4.84813681109535993589914102357e-6;
                        datum.DatumParams[4] *= 4.84813681109535993589914102357e-6;
                        datum.DatumParams[5] *= 4.84813681109535993589914102357e-6;
                        datum.DatumParams[6] = (datum.DatumParams[6] / 1000000.0) + 1.0;
                    }
                }
            }

            datum.DeriveConstants();

            return datum;
        }

        public static double[] ParseDatumParams (string datumParamsString)
        {
            string[] splits = datumParamsString.Split (',');
            double[] datumParams = new double[splits.Length];

            for (int i = 0; i < splits.Length; i++)
                datumParams[i] = double.Parse (splits[i], NumberStyles.Number, CultureInfo.InvariantCulture);

            return datumParams;
        }

        private void DeriveConstants()
        {
            double a2 = Ellipsoid.SemimajorRadius * Ellipsoid.SemimajorRadius;          // used in geocentric
            double b2 = Ellipsoid.SemiminorRadius * Ellipsoid.SemiminorRadius;          // used in geocentric
            Es = (a2 - b2) / a2;  // e ^ 2
            e = Math.Sqrt (Es);        // eccentricity            
            Ep2 = (a2 - b2) / b2; // used in geocentric
        }

        public string DatumCode { get; private set; }
        public DatumType DatumType { get; private set; }
        public double Es { get; private set; }
        public double e { get; private set; }
        public string towgs84 { get; private set; }
        public IEllipsoid Ellipsoid { get; private set; }
        public string DatumName { get; private set; }
        public double Ep2 { get; private set; }
        public double[] DatumParams { get; private set; }

        public void GeodeticToGeocentric (double?[] point)
        {
            double Longitude = point[0].Value;
            double Latitude = point[1].Value;
            double Height = point[2] ?? 0;   //Z value not always supplied
            double X;  // output
            double Y;
            double Z;

            double Rn;            /*  Earth radius at location  */
            double Sin_Lat;       /*  Math.sin(Latitude)  */
            double Sin2_Lat;      /*  Square of Math.sin(Latitude)  */
            double Cos_Lat;       /*  Math.cos(Latitude)  */

            /*
            ** Don't blow up if Latitude is just a little out of the value
            ** range as it may just be a rounding issue.  Also removed longitude
            ** test, it should be wrapped by Math.cos() and Math.sin().  NFW for PROJ.4, Sep/2001.
            */
            if (Latitude < -MathExt.PI2 && Latitude > -1.001 * MathExt.PI2)
                Latitude = -MathExt.PI2;
            else if (Latitude > MathExt.PI2 && Latitude < 1.001 * MathExt.PI2)
                Latitude = MathExt.PI2;
            else if ((Latitude < -MathExt.PI2) || (Latitude > MathExt.PI2))
                throw new InvalidOperationException ("Latitude out of range");

            if (Longitude > Math.PI)
                Longitude -= MathExt.C2PI;

            Sin_Lat = Math.Sin (Latitude);
            Cos_Lat = Math.Cos (Latitude);
            Sin2_Lat = Sin_Lat * Sin_Lat;
            Rn = Ellipsoid.SemimajorRadius / (Math.Sqrt (1.0e0 - Es * Sin2_Lat));
            X = (Rn + Height) * Cos_Lat * Math.Cos (Longitude);
            Y = (Rn + Height) * Cos_Lat * Math.Sin (Longitude);
            Z = ((Rn * (1 - Es)) + Height) * Sin_Lat;

            point[0] = X;
            point[1] = Y;
            point[2] = Z;
        }

        public void GeocentricToWgs84 (double?[] point)
        {
            throw new System.NotImplementedException ();
        }

        public void GeocentricFromWgs84 (double?[] point)
        {
            if (DatumType == DatumType.Param3)
            {
                point[0] -= DatumParams[0];
                point[1] -= DatumParams[1];
                point[2] -= DatumParams[2];
            }
            else if (DatumType == DatumType.Param7)
            {
                double Dx_BF = DatumParams[0];
                double Dy_BF = DatumParams[1];
                double Dz_BF = DatumParams[2];
                double Rx_BF = DatumParams[3];
                double Ry_BF = DatumParams[4];
                double Rz_BF = DatumParams[5];
                double M_BF = DatumParams[6];
                double x_tmp = (point[0].Value - Dx_BF) / M_BF;
                double y_tmp = (point[1].Value - Dy_BF) / M_BF;
                double z_tmp = (point[2].Value - Dz_BF) / M_BF;

                point[0] = x_tmp + Rz_BF * y_tmp - Ry_BF * z_tmp;
                point[1] = -Rz_BF * x_tmp + y_tmp + Rx_BF * z_tmp;
                point[2] = Ry_BF * x_tmp - Rx_BF * y_tmp + z_tmp;
            }
        }

        public void GeocentricToGeodetic (double?[] point)
        {
            double genau = 1E-12;
            double genau2 = (genau * genau);
            double maxiter = 30;

            double P;        /* distance between semi-minor axis and location */
            double RR;       /* distance between center and location */
            double CT;       /* sin of geocentric latitude */
            double ST;       /* cos of geocentric latitude */
            double RX;
            double RK;
            double RN;       /* Earth radius at location */
            double CPHI0;    /* cos of start or old geodetic latitude in iterations */
            double SPHI0;    /* sin of start or old geodetic latitude in iterations */
            double CPHI;     /* cos of searched geodetic latitude */
            double SPHI;     /* sin of searched geodetic latitude */
            double SDPHI;    /* end-criterium: addition-theorem of sin(Latitude(iter)-Latitude(iter-1)) */
            //bool At_Pole;     /* indicates location is in polar region */
            double iter;        /* # of continous iteration, max. 30 is always enough (s.a.) */

            double X = point[0].Value;
            double Y = point[1].Value;
            double Z = point[2] ?? 0.0;   //Z value not always supplied
            double Longitude;
            double Latitude;
            double Height;

            //At_Pole = false;
            P = Math.Sqrt (X * X + Y * Y);
            RR = Math.Sqrt (X * X + Y * Y + Z * Z);

            /*      special cases for latitude and longitude */
            if (P / Ellipsoid.SemimajorRadius < genau)
            {

                /*  special case, if P=0. (X=0., Y=0.) */
                //At_Pole = true;
                Longitude = 0.0;

                /*  if (X,Y,Z)=(0.,0.,0.) then Height becomes semi-minor axis
                 *  of ellipsoid (=center of mass), Latitude becomes PI/2 */
                if (RR / Ellipsoid.SemimajorRadius < genau)
                {
                    Latitude = MathExt.PI2;
                    Height = -Ellipsoid.SemiminorRadius;
                    return;
                }
            }
            else
            {
                /*  ellipsoidal (geodetic) longitude
                 *  interval: -PI < Longitude <= +PI */
                Longitude = Math.Atan2 (Y, X);
            }

            /* --------------------------------------------------------------
             * Following iterative algorithm was developped by
             * "Institut f?r Erdmessung", University of Hannover, July 1988.
             * Internet: www.ife.uni-hannover.de
             * Iterative computation of CPHI,SPHI and Height.
             * Iteration of CPHI and SPHI to 10**-12 radian resp.
             * 2*10**-7 arcsec.
             * --------------------------------------------------------------
             */
            CT = Z / RR;
            ST = P / RR;
            RX = 1.0 / Math.Sqrt (1.0 - Es * (2.0 - Es) * ST * ST);
            CPHI0 = ST * (1.0 - Es) * RX;
            SPHI0 = CT * RX;
            iter = 0;

            /* loop to find sin(Latitude) resp. Latitude
             * until |sin(Latitude(iter)-Latitude(iter-1))| < genau */
            do
            {
                iter++;
                RN = Ellipsoid.SemimajorRadius / Math.Sqrt (1.0 - Es * SPHI0 * SPHI0);

                /*  ellipsoidal (geodetic) height */
                Height = P * CPHI0 + Z * SPHI0 - RN * (1.0 - Es * SPHI0 * SPHI0);

                RK = Es * RN / (RN + Height);
                RX = 1.0 / Math.Sqrt (1.0 - RK * (2.0 - RK) * ST * ST);
                CPHI = ST * (1.0 - RK) * RX;
                SPHI = CT * RX;
                SDPHI = SPHI * CPHI0 - CPHI * SPHI0;
                CPHI0 = CPHI;
                SPHI0 = SPHI;
            }
            while (SDPHI * SDPHI > genau2 && iter < maxiter);

            /*      ellipsoidal (geodetic) latitude */
            Latitude = Math.Atan (SPHI / Math.Abs (CPHI));

            point[0] = Longitude;
            point[1] = Latitude;
            point[2] = Height;
        }
    }
}