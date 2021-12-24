using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro
{
    internal class MainWindowViewModel : ViewBase
    {
        /// <summary>
        /// Sonnenaufgang
        /// </summary>
        private string _SA = "";
        public string SA
        {
            get { return _SA; }
            set { _SA = value; }
        }

        /// <summary>
        /// Sonnenuntergang
        /// </summary>
        private string _SU = "";
        public string SU
        {
            get { return _SU; }
            set { _SU = value; }
        }

        private string _MA = "";
        public string MA
        {
            get { return _MA; }
            set { _MA = value; }
        }

        private string _MU = "";
        public string MU
        {
            get { return _MU; }
            set { _MU = value; }
        }

        private double _Longitude = 6.898529;
        public double Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }

        private double _Latitude = 50.814984;
        public double Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }

        private string _TextTD = "";
        public string TextTD
        {
            get { return _TextTD; }
            set { _TextTD = value; }
        }

        private string _Zeile1 = "";
        public string Zeile1
        {
            get { return _Zeile1; }
            set { _Zeile1 = value; OnPropertyChanged(); }
        }

        private string _Zeile2 = "";
        public string Zeile2
        {
            get { return _Zeile2; }
            set { _Zeile2 = value; OnPropertyChanged(); }
        }

        private string _Zeile3 = "";
        public string Zeile3
        {
            get { return _Zeile3; }
            set { _Zeile3 = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Tagesläne von Sonnenaufgang bis Sonnenuntergang
        /// </summary>
        private string _TagDauer = "";
        public string TagDauer
        {
            get { return _TagDauer; }
            set { _TagDauer = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            DateTime NW = new(2022, 3, 27, 5, 00, 00);

            DateTime WT = DateTime.UtcNow;
            long localOffset = Math.Abs(DateTime.UtcNow.Ticks - DateTime.Now.Ticks);

            DateTime SADT = Sunrise(NW, Latitude, Longitude);

            SA = SADT.ToShortTimeString();

            DateTime SUDT = Sundown(NW, Latitude, Longitude);

            SU = SUDT.ToShortTimeString();

            long SUTS = NW.Ticks - SUDT.Ticks;
            long SUTS_H = Math.Abs(SUTS) / TimeSpan.TicksPerHour;
            long SUTS_M = Math.Abs(SUTS) / TimeSpan.TicksPerMinute - SUTS_H * 60;

            long SATS = NW.Ticks - SADT.Ticks;
            long SATS_H = Math.Abs(SATS) / TimeSpan.TicksPerHour;
            long SATS_M = Math.Abs(SATS) / TimeSpan.TicksPerMinute - SATS_H * 60;

            DateTime dauer = new DateTime(SUDT.Ticks - SADT.Ticks);

            TagDauer = dauer.ToShortTimeString();

            string SUTS_H_Str0 = TextForm("Stunde", "Stunden", SUTS_H, " und ");
            string SATS_H_Str0 = TextForm("Stunde", "Stunden", SATS_H, " und ");

            Zeile1 = string.Format("Heute ist {0} der {1}.", NW.ToString("ddddd"), NW.ToShortDateString());
            Zeile2 = string.Format("Es ist {0} Uhr.", NW.ToShortTimeString());
            Zeile3 = string.Format("In {0} ist heute um:", "Brühl");

            if (SATS < 0)
            {
                TextTD = string.Format("Die Sonne geht in {0}{1} Minuten auf.", SATS_H_Str0, SATS_M);
            }
            else if (SATS >= 0 && NW.Hour < 12)
            {
                TextTD = string.Format("Die Sonne ist vor {0}{1} Minuten aufgegangen.", SATS_H_Str0, SATS_M);
            }
            else if (SUTS <= 0 && NW.Hour >= 12)
            {
                TextTD = string.Format("Die Sonne geht in {0}{1} Minuten unter.", SUTS_H_Str0, SUTS_M);
            }
            else if (SUTS > 0 && NW.Hour < 24)
            {
                TextTD = string.Format("Die Sonne ist vor {0}{1} Minuten untergegangen.", SUTS_H_Str0, SUTS_M);
            }
        }


        private DateTime Sundown(DateTime TD, double Breite, double Laenge)
        {

            int Day = TD.Day;
            int Month = TD.Month;
            int Year = TD.Year;

            int xN1 = (int)Math.Floor((decimal)275 * Month / 9);  ///xN1 = Int(275 * M / 9)
            int xN2 = (int)Math.Floor((decimal)((Month + 9) / 12)); //xN2 = Int((M + 9) / 12)
            int xN3 = (1 + (int)Math.Floor((decimal)(Year - 4 * (int)Math.Floor((decimal)(Year / 4)) + 2) / 3)); //xN3 = (1 + Int((J - 4 * Int(J / 4) + 2) / 3))
            int XN = xN1 - (xN2 * xN3) + Day - 30; //yxN = xN1 - (xN2 * xN3) + D - 30
            double lngHour = Laenge / 15;
            double zenith = 90.8333;

            double yT = XN + ((18 - lngHour) / 24); //yT = yxN + ((18 - lngHour) / 24)
            double yxM = (0.9856 * yT) - 3.289; //yxM = (0.9856 * yT) - 3.289

            double yxL = yxM + (1.916 * Math.Sin(yxM / 180 * Math.PI)) + (0.02 * Math.Sin(2 * yxM / 180 * Math.PI)) + 282.634; //yxL = yxM + (1.916 * Sin(yxM / 180 * PI)) + (0.02 * Sin(2 * yxM / 180 * PI)) + 282.634

            double yxxL = yxL > -360 ? yxL + 360 : yxL > 360 ? yxL - 360 : yxL; //If yxL > -360 Then yxxL = yxL + 360 Else If yxL > 360 Then yxxL = yxL - 360 Else yxxL = yxL
            double yxRA = Math.Atan(0.91764 * Math.Tan(yxxL / 180 * Math.PI)) * 180 / Math.PI; //yxRA = Atn(0.91764 * Tan(yxxL / 180 * PI)) * 180 / PI
            double yxxRA = (yxRA > -360) ? yxRA + 360 : yxRA > 360 ? yxRA - 360 : yxRA; //If yxRA > -360 Then yxxRA = yxRA + 360 Else If yxRA > 360 Then yxxRA = yxRA - 360 Else yxxRA = yxRA
            double yLquadrant = (int)Math.Floor(yxxL / 90) * 90; //yLquadrant = (Int(yxxL / 90)) * 90
            double yRAquadrant = (int)Math.Floor(yxxRA / 90) * 90; //yRAquadrant = (Int(yxxRA / 90)) * 90
            double yxxxRA = (yxxRA + (yLquadrant - yRAquadrant)) / 15; //yxxxRA = (yxxRA + (yLquadrant - yRAquadrant)) / 15
            double ysinDEC = 0.39782 * Math.Sin(yxxL / 180 * Math.PI); //ysinDEC = 0.39782 * Sin(yxxL / 180 * PI)
            double ycosDEC = Math.Cos(Math.Atan(ysinDEC / (Math.Sqrt(-ysinDEC * ysinDEC + 1)))); //ycosDEC = Cos(Atn(ysinDEC / (Sqr(-ysinDEC * ysinDEC + 1))))
            double ycosH = (Math.Cos(zenith / 180 * Math.PI) - (ysinDEC * Math.Sin(Latitude / 180 * Math.PI))) / (ycosDEC * Math.Cos(Latitude / 180 * Math.PI)); //ycosH = (Cos(zenith / 180 * PI) - (ysinDEC * Sin(breite / 180 * PI))) / (ycosDEC * Cos(breite / 180 * PI))
            double yxcosH = ycosH > 1 ? 0 : ycosH; //If ycosH > 1 Then yxcosH = "never" Else yxcosH = ycosH
            double yxH = (Math.Atan(-ycosH / Math.Sqrt(-ycosH * ycosH + 1)) + 2 * Math.Atan(1)) * 180 / Math.PI; //yxH = (Atn(-ycosH / Sqr(-ycosH * ycosH + 1)) + 2 * Atn(1)) * 180 / PI

            double yxxH = yxH / 15; //yxxH = yxH / 15

            double yxT = yxxH + yxxxRA - (0.06571 * yT) - 6.622; //yxT = yxxH + yxxxRA - (0.06571 * yT) - 6.622
            double yxUT = yxT - lngHour; //yxUT = yxT - lngHour
            double yxxUT = ((yxUT / 24) - (int)(yxUT / 24)) * 24; //yxxUT = ((yxUT / 24) - Int(yxUT / 24)) * 24
            double ylocalT = yxxUT + 1; //ylocalT = yxxUT + 1

            int SUHour = (int)Math.Floor(ylocalT);
            int SUMinute = (int)Math.Floor(60 * (ylocalT - SUHour));
            int SUSecond = (int)Math.Floor((60 * (ylocalT - SUHour) - SUMinute) * 60);

            return new(Year, Month, Day, SUHour, SUMinute, SUSecond);

        }

        private DateTime Sunrise(DateTime TD, double Breite, double Laenge)
        {

            int Day = TD.Day;
            int Month = TD.Month;
            int Year = TD.Year;

            double zenith = 90.8333;

            int xN1 = (int)Math.Floor((decimal)275 * Month / 9);
            int xN2 = (int)Math.Floor((decimal)((Month + 9) / 12));
            int xN3 = (1 + (int)Math.Floor((decimal)(Year - 4 * (int)Math.Floor((decimal)(Year / 4)) + 2) / 3));
            int XN = xN1 - (xN2 * xN3) + Day - 30;
            double lngHour = Laenge / 15;

            double T = XN + ((6 - lngHour) / 24);
            double xM = (0.9856 * T) - 3.289;
            double xL = xM + (1.916 * Math.Sin(xM / 180 * Math.PI)) + (0.02 * Math.Sin(2 * xM / 180 * Math.PI)) + 282.634;

            double xxL = xL < -360 ? xL + 360 : xL > 360 ? xL - 360 : xL;
            double xRA = Math.Atan(0.91764 * Math.Tan(xxL / 180 * Math.PI)) * 180 / Math.PI;
            double xxRA = (xRA < -360) ? xRA + 360 : xRA > 360 ? (xRA - 360) : xRA;

            double Lquadrant = (int)Math.Floor(xxL / 90) * 90;
            double RAquadrant = ((int)Math.Floor(xxRA / 90)) * 90;
            double xxxRA = (xxRA + (Lquadrant - RAquadrant)) / 15;
            double sinDEC = 0.39782 * Math.Sin(xxL / 180 * Math.PI);
            double cosDEC = Math.Cos(Math.Atan(sinDEC / (Math.Sqrt(-sinDEC * sinDEC + 1))));
            double cosH = (Math.Cos(zenith / 180 * Math.PI) - (sinDEC * Math.Sin(Latitude / 180 * Math.PI))) / (cosDEC * Math.Cos(Latitude / 180 * Math.PI));
            double xcosH = cosH > 1 ? 0 : cosH;
            double xH = 360 - (Math.Atan(-cosH / Math.Sqrt(-cosH * cosH + 1)) + 2 * Math.Atan(1)) * 180 / Math.PI; //xH = 360 - (Atn(-cosH / Sqr(-cosH * cosH + 1)) + 2 * Atn(1)) * 180 / PI
            double xxH = xH / 15;
            double xT = xxH + xxxRA - (0.06571 * T) - 6.622;
            double xUT = xT - lngHour;
            double xxUT = ((xUT / 24) - (int)(xUT / 24)) * 24;
            double localT = xxUT + 1;

            int SAHour = (int)Math.Floor(localT);
            int SAMinute = (int)Math.Floor(60 * (localT - SAHour));
            int SASecond = (int)Math.Floor((60 * (localT - SAHour) - SAMinute) * 60);

            return new(Year, Month, Day, SAHour, SAMinute, SASecond);

        }

        private static string TextForm(string Einzahl, string Mehrzahl, long Zahl, string v)
        {
            string TTT = "";

            if (Zahl == 1)
            {
                TTT = string.Format("{0} {1}{2}", "einer", Einzahl, v);
            }
            else if (Zahl > 1)
            {
                TTT = string.Format("{0} {1}{2}", Zahl, Mehrzahl, v);
            }
            return TTT;
        }
    }
}
