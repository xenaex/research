using System;
using System.Collections.Generic;
using System.Drawing;
using WealthLab;

namespace FlerovDeMark
{
    /// <summary>
	/// 6 versions of Demark points by Nikolay Flerov for the blog article "Original stop loss techniques"
	/// </summary>

    // Book version (DeMark1.Series)
    #region Description of the DeMark indicator

    public class DeMark1Helper : IndicatorHelper
    {
        private static object[] _paramDefaults = { BarDataType.Bars };
        private static string[] _paramDescriptions = { "Source" };

        public override LineStyle DefaultStyle
        {
            get { return LineStyle.Histogram; }
        }

        public override Color DefaultColor
        {
            get { return Color.Blue; }
        }

        public override string TargetPane
        {
            get { return "DeMark1"; }
        }

        public override string Description
        {
            get { return "Book version (DeMark1.Series)"; }
        }

        public override Type IndicatorType
        {
            get { return typeof(DeMark1); }
        }
        public override IList<object> ParameterDefaultValues
        {
            get { return _paramDefaults; }
        }

        public override IList<string> ParameterDescriptions
        {
            get { return _paramDescriptions; }
        }
    }

    #endregion

    #region Demark points (pyramid)

    /// <summary>
    /// Returns extreme values
    ///  0 - no extreme
    ///  1 - up-extreme
    ///  2 - maximum between two downward extremes (atypical upward extremum)
    /// -1 - down-extreme
    /// -2 - minimum between two upward extremes (atypical downward extremum)
    /// </summary>
    public class DeMark1 : DataSeries
    {
        struct DeMarkBar
        {
            public int Bar;
            public double DeMarklValue;
        }

        /// <summary>
        /// Indicator constructor
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <YesterDi name="Description">Description</param>
        public DeMark1(Bars Bars, string Description, int period)
            : base(Bars, Description)
        {
            DeMarkBar lastDeMark;
            lastDeMark.Bar = 0;
            lastDeMark.DeMarklValue = 0.0d;
            // We research 3 bars, therefore calculations starts from the 2nd bar
            this.FirstValidValue = period;
            for (int bar = this.FirstValidValue; bar < Bars.Count; bar++)
            {
                #region DeMark Extremes

                var count = (int)(period / 2);
                var centr = bar - (count + 1);

                // Extremums up
                var f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (Bars.High[centr - i - 1] < Bars.High[centr - i] &&
                         Bars.High[centr + i + 1] < Bars.High[centr + i]);
                }
                if (f)
                    this[centr] = 1;

                // Extremums down
                f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (Bars.Low[centr - i - 1] > Bars.Low[centr - i] &&
                         Bars.Low[centr + i + 1] > Bars.Low[centr + i]);
                }
                if (f)
                    this[centr] = -1;

                #endregion
            }
        }

        /// <summary>
        /// Returns an indicator instance
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <returns>Indicator instance: new or from the cache</returns>
        public static DeMark1 Series(Bars Bars, int period)
        {
            string description = "Extremes " + period;
            if (Bars.Cache.ContainsKey(description))
                return (DeMark1)Bars.Cache[description];

            DeMark1 DeMark = new DeMark1(Bars, description, period);
            Bars.Cache[description] = DeMark;
            return DeMark;
        }
    }

    #endregion

    // Classic version (DeMark2.Series)
    #region Description of the DeMark2 indicator

    public class DeMark2Helper : IndicatorHelper
    {
        private static object[] _paramDefaults = { BarDataType.Bars };
        private static string[] _paramDescriptions = { "Source" };

        public override LineStyle DefaultStyle
        {
            get { return LineStyle.Histogram; }
        }

        public override Color DefaultColor
        {
            get { return Color.Blue; }
        }

        public override string TargetPane
        {
            get { return "DeMark2"; }
        }

        public override string Description
        {
            get { return "Classic version (DeMark2.Series)"; }
        }

        public override Type IndicatorType
        {
            get { return typeof(DeMark2); }
        }
        public override IList<object> ParameterDefaultValues
        {
            get { return _paramDefaults; }
        }

        public override IList<string> ParameterDescriptions
        {
            get { return _paramDescriptions; }
        }
    }

    #endregion

    #region Demark Points2 (5)

    /// <summary>
    /// Returns extreme values
    ///  0 - no extreme
    ///  1 - up-extreme
    ///  2 - maximum between two downward extremes (atypical upward extremum)
    /// -1 - down-extreme
    /// -2 - minimum between two upward extremes (atypical downward extremum)
    /// </summary>
    public class DeMark2 : DataSeries
    {
        struct DeMarkBar
        {
            public int Bar;
            public double DeMarklValue;
        }

       /// <summary>
        /// Indicator constructor
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <YesterDi name="Description">Description</param>
        public DeMark2(Bars Bars, string Description, int period)
            : base(Bars, Description)
        {
            DeMarkBar lastDeMark;
            lastDeMark.Bar = 0;
            lastDeMark.DeMarklValue = 0.0d;

            this.FirstValidValue = period; // We research 3 bars, therefore calculations starts from the 2nd bar
            for (int bar = this.FirstValidValue; bar < Bars.Count; bar++)
            {
                #region Экстремумы ДеМарка

                var count = (int)(period / 2);
                var centr = bar - (count + 1);


                // Extremums up
                var f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (Bars.High[centr - i - 1] < Bars.High[centr] &&
                         Bars.High[centr + i + 1] < Bars.High[centr]);
                }
                if (f)
                    this[centr] = 1;

                // Extremums down
                f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (Bars.Low[centr - i - 1] > Bars.Low[centr] &&
                         Bars.Low[centr + i + 1] > Bars.Low[centr]);
                }
                if (f)
                    this[centr] = -1;

                #endregion

               }
        }

        /// <summary>
        /// Returns an indicator instance
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <returns>Indicator instance: new or from the cache</returns>
        public static DeMark2 Series(Bars Bars, int period)
        {
            string description = "Extremes2 " + period;
            if (Bars.Cache.ContainsKey(description))
                return (DeMark2)Bars.Cache[description];

            DeMark2 DeMark = new DeMark2(Bars, description, period);
            Bars.Cache[description] = DeMark;
            return DeMark;
        }
    }

    #endregion

    // DeMark points by CLose (DeMark3.Series)
    #region Description of the DeMark3 indicator

    public class DeMark3Helper : IndicatorHelper
    {
        private static object[] _paramDefaults = { BarDataType.Bars };
        private static string[] _paramDescriptions = { "Source" };

        public override LineStyle DefaultStyle
        {
            get { return LineStyle.Histogram; }
        }

        public override Color DefaultColor
        {
            get { return Color.Blue; }
        }

        public override string TargetPane
        {
            get { return "DeMark3"; }
        }

        public override string Description
        {
            get { return "DeMark points by CLose (DeMark3.Series)"; }
        }

        public override Type IndicatorType
        {
            get { return typeof(DeMark3); }
        }
        public override IList<object> ParameterDefaultValues
        {
            get { return _paramDefaults; }
        }

        public override IList<string> ParameterDescriptions
        {
            get { return _paramDescriptions; }
        }
    }

    #endregion

    #region Demark points3 (by close)

    /// <summary>
    /// Returns extreme values
    ///  0 - no extreme
    ///  1 - up-extreme
    ///  2 - maximum between two downward extremes (atypical upward extremum)
    /// -1 - down-extreme
    /// -2 - minimum between two upward extremes (atypical downward extremum)
    /// </summary>
    public class DeMark3 : DataSeries
    {
        struct DeMarkBar
        {
            public int Bar;
            public double DeMarklValue;
        }

        /// <summary>
        /// Indicator constructor
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <YesterDi name="Description">Description</param>
        public DeMark3(Bars Bars, string Description, int period)
            : base(Bars, Description)
        {
            DeMarkBar lastDeMark;
            lastDeMark.Bar = 0;
            lastDeMark.DeMarklValue = 0.0d;
            DataSeries UpperPoint = new DataSeries(Bars, "UpperPoint");
            DataSeries LowerPoint = new DataSeries(Bars, "LowerPoint");
            // We research 3 bars, therefore calculations starts from the 2nd bar
            this.FirstValidValue = period;
            for (int bar = this.FirstValidValue; bar < Bars.Count; bar++)
            {
                #region Экстремумы ДеМарка

                var count = (int)(period / 2);
                var centr = bar - (count + 1);


                if (Bars.Close[bar] > Bars.Open[bar])
                {
                    UpperPoint[bar] = Bars.Close[bar];
                    LowerPoint[bar] = Bars.Open[bar];
                }
                else// if (Bars.Close[bar] < Bars.Open[bar])
                {
                    UpperPoint[bar] = Bars.Open[bar];
                    LowerPoint[bar] = Bars.Close[bar];
                }

                // Extreme up
                var f = true;
                for (int i = 0; i < count; i++)
                {

                    f = f &&
                        (UpperPoint[centr - i - 1] <= UpperPoint[centr] &&
                         UpperPoint[centr + i + 1] <= UpperPoint[centr]);
                }
                if (f)
                    this[centr] = 1;

                // Extreme down
                f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (LowerPoint[centr - i - 1] >= LowerPoint[centr] &&
                         LowerPoint[centr + i + 1] >= LowerPoint[centr]);
                }
                if (f)
                    this[centr] = -1;


                #endregion


            }
        }

        /// <summary>
        /// Returns an indicator instance
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <returns>Indicator instance: new or from the cache</returns>
        public static DeMark3 Series(Bars Bars, int period)
        {
            string description = "Extreme3 " + period;
            if (Bars.Cache.ContainsKey(description))
                return (DeMark3)Bars.Cache[description];

            DeMark3 DeMark = new DeMark3(Bars, description, period);
            Bars.Cache[description] = DeMark;
            return DeMark;
        }
    }

    #endregion

    // Book variant with atypical extrema (DeMark1Netip.Series)
    #region Description of the DeMark1Netip indicator

    public class DeMark1NetipHelper : IndicatorHelper
    {
        private static object[] _paramDefaults = { BarDataType.Bars };
        private static string[] _paramDescriptions = { "Source" };

        public override LineStyle DefaultStyle
        {
            get { return LineStyle.Histogram; }
        }

        public override Color DefaultColor
        {
            get { return Color.Blue; }
        }

        public override string TargetPane
        {
            get { return "DeMark1Netip"; }
        }

        public override string Description
        {
            get { return "Book variant with atypical extrema (DeMark1Netip.Series)"; }
        }

        public override Type IndicatorType
        {
            get { return typeof(DeMark1Netip); }
        }
        public override IList<object> ParameterDefaultValues
        {
            get { return _paramDefaults; }
        }

        public override IList<string> ParameterDescriptions
        {
            get { return _paramDescriptions; }
        }
    }

    #endregion

    #region Demark points (pyramid)

     /// <summary>
    /// Returns extreme values
    ///  0 - no extreme
    ///  1 - up-extreme
    ///  2 - maximum between two downward extremes (atypical upward extremum)
    /// -1 - down-extreme
    /// -2 - minimum between two upward extremes (atypical downward extremum)
    /// </summary>
    public class DeMark1Netip : DataSeries
    {
        struct DeMarkBar
        {
            public int Bar;
            public double DeMarklValue;
        }

        /// <summary>
        /// Indicator constructor
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <YesterDi name="Description">Description</param>
        public DeMark1Netip(Bars Bars, string Description, int period)
            : base(Bars, Description)
        {
            DeMarkBar lastDeMark;
            lastDeMark.Bar = 0;
            lastDeMark.DeMarklValue = 0.0d;
            // We research 3 bars, therefore calculations starts from the 2nd bar
            this.FirstValidValue = period;
            for (int bar = this.FirstValidValue; bar < Bars.Count; bar++)
            {
                #region DeMark Extremes

                var count = (int)(period / 2);
                var centr = bar - (count + 1);

                // Extreme up
                var f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (Bars.High[centr - i - 1] < Bars.High[centr - i] &&
                         Bars.High[centr + i + 1] < Bars.High[centr + i]);
                }
                if (f)
                    this[centr] = 1;

                // Extreme down
                f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (Bars.Low[centr - i - 1] > Bars.Low[centr - i] &&
                         Bars.Low[centr + i + 1] > Bars.Low[centr + i]);
                }
                if (f)
                    this[centr] = -1;

                #endregion

                #region Atypical Extremes

                if (this[bar - 2] != 0)
                {
                    if (lastDeMark.DeMarklValue != 0 && //There was an extreme
                        Math.Sign(lastDeMark.DeMarklValue) == Math.Sign(this[bar - 2])) // and this extremum coincides in type (sign) with the previous extremum
                    {
                        int nonTypicalDeMarkBar = lastDeMark.Bar + 1; // First, we set an atypical extremum on the next candle after the previous extremum

                        if (this[bar - 2] > 0) // For extreme up
                        {
                            for (int i = lastDeMark.Bar + 1; i < bar - 2; i++)
                                if (Bars.Low[i] < Bars.Low[nonTypicalDeMarkBar]) //  If bar with minimal value found
                                    nonTypicalDeMarkBar = i;
                            this[nonTypicalDeMarkBar] = -2; // Set an atypical extremum down
                        }
                        else // For extreme down
                        {
                            for (int i = lastDeMark.Bar + 1; i < bar - 2; i++)
                                if (Bars.High[i] > Bars.High[nonTypicalDeMarkBar]) // If bar with minimal value found
                                    nonTypicalDeMarkBar = i;
                            this[nonTypicalDeMarkBar] = 2; // Set an atypical extremum up
                        }
                    }
                    lastDeMark.Bar = bar - 2;
                    lastDeMark.DeMarklValue = this[bar - 2];
                }

                #endregion
            }
        }

        /// <summary>
        /// Returns an indicator instance
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <returns>Indicator instance: new or from the cache</returns>
        public static DeMark1Netip Series(Bars Bars, int period)
        {
            string description = "Extreme4 " + period;
            if (Bars.Cache.ContainsKey(description))
                return (DeMark1Netip)Bars.Cache[description];

            DeMark1Netip DeMark1Netip = new DeMark1Netip(Bars, description, period);
            Bars.Cache[description] = DeMark1Netip;
            return DeMark1Netip;
        }
    }

    #endregion

    //Classic variant with atypical extrema (DeMark2Netip.Series)
    #region Description of the DeMark5Netip indicator

    public class DeMark2NetipHelper : IndicatorHelper
    {
        private static object[] _paramDefaults = { BarDataType.Bars };
        private static string[] _paramDescriptions = { "Источник" };

        public override LineStyle DefaultStyle
        {
            get { return LineStyle.Histogram; }
        }

        public override Color DefaultColor
        {
            get { return Color.Blue; }
        }

        public override string TargetPane
        {
            get { return "DeMark5Netip"; }
        }

        public override string Description
        {
            get { return "Classic variant with atypical extrema (DeMark2Netip.Series)"; }
        }

        public override Type IndicatorType
        {
            get { return typeof(DeMark2Netip); }
        }
        public override IList<object> ParameterDefaultValues
        {
            get { return _paramDefaults; }
        }

        public override IList<string> ParameterDescriptions
        {
            get { return _paramDescriptions; }
        }
    }

    #endregion

    #region Demark points4 (5 with atypical extrema)

     /// <summary>
    /// Returns extreme values
    ///  0 - no extreme
    ///  1 - up-extreme
    ///  2 - maximum between two downward extremes (atypical upward extremum)
    /// -1 - down-extreme
    /// -2 - minimum between two upward extremes (atypical downward extremum)
    /// </summary>
    public class DeMark2Netip : DataSeries
    {
        struct DeMarkBar
        {
            public int Bar;
            public double DeMarklValue;
        }

        /// <summary>
        /// Indicator constructor
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <YesterDi name="Description">Description</param>
        public DeMark2Netip(Bars Bars, string Description, int period)
            : base(Bars, Description)
        {
            DeMarkBar lastDeMark;
            lastDeMark.Bar = 0;
            lastDeMark.DeMarklValue = 0.0d;
            // We research 3 bars, therefore calculations starts from the 2nd bar
            this.FirstValidValue = period;
            for (int bar = this.FirstValidValue; bar < Bars.Count; bar++)
            {
                #region DeMark Extremes

                var count = (int)(period / 2);
                var centr = bar - (count + 1);

                // Extreme up
                var f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (Bars.High[centr - i - 1] < Bars.High[centr] &&
                         Bars.High[centr + i + 1] < Bars.High[centr]);
                }
                if (f)
                    this[centr] = 1;

                // Extreme down
                f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (Bars.Low[centr - i - 1] > Bars.Low[centr] &&
                         Bars.Low[centr + i + 1] > Bars.Low[centr]);
                }
                if (f)
                    this[centr] = -1;

                #endregion

                #region Atypical Extremes

                if (Math.Abs((int)(this[centr] - 0)) > double.Epsilon) //If an extremum has formed at the current moment
                {
                    if (Math.Abs(lastDeMark.DeMarklValue - 0) > double.Epsilon && //There was an extreme
                        Math.Sign(lastDeMark.DeMarklValue) == Math.Sign(this[centr]))
                    // and this extremum coincides in type (sign) with the previous extremum
                    {
                        int nonTypicalDeMarkBar = lastDeMark.Bar + 1;
                        // First, we set an atypical extremum on the next candle after the previous extremum

                        if (this[centr] > 0) // For extreme up
                        {
                            for (int i = lastDeMark.Bar + 1; i < centr; i++)

                                if (Bars.Low[i] < Bars.Low[nonTypicalDeMarkBar])
                                    // If a candlestick with the minimum value is found
                                    nonTypicalDeMarkBar = i;
                            this[nonTypicalDeMarkBar] = -2; // Set an atypical extremum down
                        }
                        else // For extreme down
                        {
                            for (int i = lastDeMark.Bar + 1; i < centr; i++)

                                if (Bars.High[i] > Bars.High[nonTypicalDeMarkBar])
                                    // If a candlestick with the maximum value is found
                                    nonTypicalDeMarkBar = i;
                            this[nonTypicalDeMarkBar] = 2; // Set an atypical extremum up
                        }
                    }
                    lastDeMark.Bar = centr;
                    lastDeMark.DeMarklValue = this[centr];
                }

                #endregion
            }
        }

        /// <summary>
        /// Returns an indicator instance
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <returns>Indicator instance: new or from the cache</returns>
        public static DeMark2Netip Series(Bars Bars, int period)
        {
            string description = "Extreme5 " + period;
            if (Bars.Cache.ContainsKey(description))
                return (DeMark2Netip)Bars.Cache[description];

            DeMark2Netip DeMark = new DeMark2Netip(Bars, description, period);
            Bars.Cache[description] = DeMark;
            return DeMark;
        }
    }

    #endregion

    //DeMark points by CLose with atypical extrema (DeMark3Netip.Series)
    #region Description of the DeMarkCloseNetip indicator

    public class DeMark3NetipHelper : IndicatorHelper
    {
        private static object[] _paramDefaults = { BarDataType.Bars };
        private static string[] _paramDescriptions = { "Source" };

        public override LineStyle DefaultStyle
        {
            get { return LineStyle.Histogram; }
        }

        public override Color DefaultColor
        {
            get { return Color.Blue; }
        }

        public override string TargetPane
        {
            get { return "DeMarkCloseNetip"; }
        }

        public override string Description
        {
            get { return "DeMark points by CLose with atypical extrema (DeMark3Netip.Series)"; }
        }

        public override Type IndicatorType
        {
            get { return typeof(DeMark3Netip); }
        }
        public override IList<object> ParameterDefaultValues
        {
            get { return _paramDefaults; }
        }

        public override IList<string> ParameterDescriptions
        {
            get { return _paramDescriptions; }
        }
    }

    #endregion

    #region Demark points6 (by close with atypical extrema)

    /// <summary>
    /// Returns extreme values
    ///  0 - no extreme
    ///  1 - up-extreme
    ///  2 - maximum between two downward extremes (atypical upward extremum)
    /// -1 - down-extreme
    /// -2 - minimum between two upward extremes (atypical downward extremum)
    /// </summary>
    public class DeMark3Netip : DataSeries
    {
        struct DeMarkBar
        {
            public int Bar;
            public double DeMarklValue;
        }

        /// <summary>
        /// Indicator constructor
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <YesterDi name="Description">Description</param>
        public DeMark3Netip(Bars Bars, string Description, int period)
            : base(Bars, Description)
        {
            DeMarkBar lastDeMark;
            lastDeMark.Bar = 0;
            lastDeMark.DeMarklValue = 0.0d;
            DataSeries UpperPoint = new DataSeries(Bars, "UpperPoint");
            DataSeries LowerPoint = new DataSeries(Bars, "LowerPoint");
            // We research 3 bars, therefore calculations starts from the 2nd bar
            this.FirstValidValue = period;
            for (int bar = this.FirstValidValue; bar < Bars.Count; bar++)
            {
                #region DeMark Extremes

                var count = (int)(period / 2);
                var centr = bar - (count + 1);

                if (Bars.Close[bar] > Bars.Open[bar])
                {
                    UpperPoint[bar] = Bars.Close[bar];
                    LowerPoint[bar] = Bars.Open[bar];
                }
                else// if (Bars.Close[bar] < Bars.Open[bar])
                {
                    UpperPoint[bar] = Bars.Open[bar];
                    LowerPoint[bar] = Bars.Close[bar];
                }

                // Extreme up
                var f = true;
                for (int i = 0; i < count; i++)
                {

                    f = f &&
                        (UpperPoint[centr - i - 1] <= UpperPoint[centr] &&
                         UpperPoint[centr + i + 1] <= UpperPoint[centr]);
                }
                if (f)
                    this[centr] = 1;

                // Extreme down
                f = true;
                for (int i = 0; i < count; i++)
                {
                    f = f &&
                        (LowerPoint[centr - i - 1] >= LowerPoint[centr] &&
                         LowerPoint[centr + i + 1] >= LowerPoint[centr]);
                }
                if (f)
                    this[centr] = -1;

                #endregion

                #region Atypical Extremes

                if (Math.Abs((int)(this[centr] - 0)) > double.Epsilon) // If an extremum has formed at the current moment
                {
                    if (Math.Abs(lastDeMark.DeMarklValue - 0) > double.Epsilon && // There was an extreme
                        Math.Sign(lastDeMark.DeMarklValue) == Math.Sign(this[centr]))
                    // and this extremum coincides in type (sign) with the previous extremum
                    {
                        int nonTypicalDeMarkBar = lastDeMark.Bar + 1;
                        // Atypical extremum is set first on the next candle after the previous extremum

                        if (this[centr] > 0) // For extreme up
                        {
                            for (int i = lastDeMark.Bar + 1; i < centr; i++)

                                if (LowerPoint[i] < LowerPoint[nonTypicalDeMarkBar])
                                    // If a candlestick with the minimum value is found
                                    nonTypicalDeMarkBar = i;
                            this[nonTypicalDeMarkBar] = -2; // Set an atypical extremum down
                        }
                        else // For extreme down
                        {
                            for (int i = lastDeMark.Bar + 1; i < centr; i++)

                                if (UpperPoint[i] > UpperPoint[nonTypicalDeMarkBar])
                                    // If a candlestick with the maximum value is found
                                    nonTypicalDeMarkBar = i;
                            this[nonTypicalDeMarkBar] = 2; // Set an atypical extremum up
                        }
                    }
                    lastDeMark.Bar = centr;
                    lastDeMark.DeMarklValue = this[centr];
                }

                #endregion
            }
        }

        /// <summary>
        /// Returns an indicator instance
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <returns>Indicator instance: new or from the cache</returns>
        public static DeMark3Netip Series(Bars Bars, int period)
        {
            string description = "Extreme6 " + period; // Chart description
            if (Bars.Cache.ContainsKey(description))
                return (DeMark3Netip)Bars.Cache[description];

            DeMark3Netip DeMark = new DeMark3Netip(Bars, description, period);
            Bars.Cache[description] = DeMark;
            return DeMark;
        }
    }

    #endregion
}
