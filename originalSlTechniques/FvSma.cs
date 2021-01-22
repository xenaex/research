using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using WealthLab;

namespace FleorovAverages
{
    #region Description of the FvSma indicator (Average for DeMark points)

    public class FvSmaHelper : IndicatorHelper
    {
        private static object[] _paramDefaults = { CoreDataSeries.Close, new RangeBoundInt32(100, 1, 1000) };
        private static string[] _paramDescriptions = { "Source", "Period" };

        public override LineStyle DefaultStyle
        {
            get { return LineStyle.Histogram; }
        }

        public override Color DefaultColor
        {
            get { return Color.Green; }
        }

        public override string TargetPane
        {
            get { return "FvSma"; } // Display in your panel
        }

        public override string Description
        {
            get { return "FvSma"; }
        }

        public override Type IndicatorType
        {
            get { return typeof(FvSma); }
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

    #region FvSma Ind.

    public class FvSma : DataSeries
    {
        /// <summary>
        /// Indicator constructor
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// /// <YesterDi name="Description">Description</param>
        public FvSma(DataSeries ds, int period, string description)
            : base(ds, description)
        {
            this.FirstValidValue = period - 1 + ds.FirstValidValue;

            if (period > ds.Count)
                return;

            double num = 0.0;
            var lastIndex = 0;
            var count = 0;
            var index = 0;

            var list = new List<double>();

            while (list.Count < period && index < period)
            {
                var value = ds[index];

                if (value > double.Epsilon)
                {
                    list.Add(value);
                }

                index++;
            }

            this[lastIndex] = list.Average();


            for (int i = index+1; i < ds.Count; i++)
            {
                var value = ds[i];

                if (Math.Abs(value) > double.Epsilon)
                {
                    list.RemoveAt(0);
                    list.Add(value);

                    this[i] = list.Average();
                }
            }
        }

        /// <summary>
        /// Returns an indicator instance
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <returns>Indicator instance: new or from the cache</returns>
        public static FvSma Series(DataSeries ds, int period)
        {
            string description = "FvSma " + ds.Description + period;
            if (ds.Cache.ContainsKey(description))
                return (FvSma)ds.Cache[description];

            FvSma fvSma = new FvSma(ds, period, description);
            ds.Cache[description] = fvSma;
            return fvSma;
        }
    }

    #endregion
}
