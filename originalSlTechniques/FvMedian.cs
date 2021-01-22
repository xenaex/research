using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using WealthLab;

namespace FlerovAverages
{
    #region Description of the FvMedian indicator (Median for DeMark points)

    public class FvMedianHelper : IndicatorHelper
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
            get { return "FvMedian"; } // Display in your panel
        }

        public override string Description
        {
            get { return "OwnMedian"; }
        }

        public override Type IndicatorType
        {
            get { return typeof(FvMedian); }
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

    #region FvMedian Ind.

    public class FvMedian : DataSeries
    {
        /// <summary>
        /// Indicator constructor
        /// </summary>
        /// <param name="Bars">Bars</param>
        /// <YesterDi name="Description">Description</param>
        public FvMedian(DataSeries ds, int period, string description)
            : base(ds, description)
        {
            this.FirstValidValue = period - 1 + ds.FirstValidValue;

            if (period > ds.Count)
                return;

            var lastIndex = 0;
            var index = 0;

            var list = new List<double>();

            while (list.Count < period)
            {
                var value = ds[index];

                if (value > double.Epsilon)
                {
                    list.Add(value);
                }

                index++;
            }

            this[lastIndex] = GetValue(list.OrderBy(i => i).ToArray());


            for (int i = index + 1; i < ds.Count; i++)
            {
                var value = ds[i];

                if (Math.Abs(value) > double.Epsilon)
                {
                    list.RemoveAt(0);
                    list.Add(value);

                    this[i] = GetValue(list.OrderBy(v => v).ToArray());
                }
            }
        }

        private double GetValue(double[] values)
        {
            var period = values.Length;

            if (period % 2 != 1)
            {
                int index2 = period / 2 - 1;
                return (values[index2] + values[index2 + 1]) / 2.0;
            }
            else
            {
                int index2 = period / 2;
                return values[index2];
            }
        }


        public static FvMedian Series(DataSeries ds, int period)
        {
            string description = "FvMedian " + ds.Description + period; // Chart description
            if (ds.Cache.ContainsKey(description))
                return (FvMedian)ds.Cache[description];

            FvMedian fvMedian = new FvMedian(ds, period, description);
            ds.Cache[description] = fvMedian;
            return fvMedian;
        }
    }

    #endregion
}

