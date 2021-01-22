using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using WealthLab;
using WealthLab.Indicators;
using FlerovDeMark;
using FlerovAverages;

namespace Flerov.Strategies
{

	/// <summary>
	/// DonchianDeMarkStop is an example strategy for the blog article "Original stop loss techniques"
	/// By Nikolay Flerov
	///
	/// Position entry - Donchian Channel breakout
	/// Position exit - TrailingSma
	/// </summary>
	public	class DonchianDeMarkStop : WealthScript
	{

		#region Trading system parameters announcement

		private StrategyParameter _countBars; // Number of candles on each side of the extreme
		StrategyParameter _DonchianPeriod;

		#endregion

		#region Parameters initialization

		public MyStrategy()
		{

			_DonchianPeriod = CreateParameter("Period", 470, 100, 700, 2);
			_countBars = CreateParameter("_countBars",3,3,15,2);
		}

		#endregion

		protected override void Execute()
		{
			double orderStopLoss = 0, orderTakeProfit = 0;
			int firstValidValue = 645; // Index of the bar, when all indicators are firstly calculated

			#region Presentation of prices with the required bit depth

			string pricePattern = "0."; // To display the required number of decimal places in the Debug window
			double decimalsValue = 1; // To convert points to prices
			for (int i = 0; i < this.Bars.SymbolInfo.Decimals; i++)
			{
				pricePattern += "0";
				decimalsValue *= 0.1;
			}
			double tick = Bars.SymbolInfo.Tick; // Minimal price step
			#endregion

			PlotStops();
			ClearDebug();
			HideVolume();

			#region Indicators
			// Donchian Channel
			int DonchianPeriod = _DonchianPeriod.ValueInt;
			DataSeries highLevel = Highest.Series(High, DonchianPeriod);
			DataSeries lowLevel = Lowest.Series(Low, DonchianPeriod);
			PlotSeries(PricePane, highLevel, Color.FromArgb(255,119,168,27), LineStyle.Solid, 2);
			PlotSeries(PricePane, lowLevel, Color.FromArgb(119,168,27,0), LineStyle.Solid, 2);

			int countBars = _countBars.ValueInt;
			// Distance between oppositely directed extrema (in points and in candles)
			DataSeries Range= new DataSeries(Bars, "Range");
			DataSeries TimeRange= new DataSeries(Bars, "TimeRange");
			ChartPane DeMarkPane = CreatePane(30, true, true);
			DataSeries Ex;

			Ex = DeMark2.Series(Bars,countBars);
			PlotSeries(DeMarkPane, Ex, Color.FromArgb(255,84,129,153), LineStyle.Histogram, 2);

			int UpBar = -1;
			int DownBar = 1;

			for(int bar = 645; bar < Bars.Count; bar++)
			{
				if ( Ex[bar-countBars] > 0 )
				{

					UpBar = bar;
					// To highlight up-bar in indicator
		            // SetBackgroundColor( bar, Color.LightGreen );

					if(DownBar != -1)
					{
						Range[bar]=	Math.Abs( High[bar]-Low[DownBar]);
						TimeRange[bar]=	bar-DownBar;
					}


				}
				//	LastEx1	 = Ex1;
				if ( Ex[bar-countBars] < 0 )
				{

					DownBar = bar;
					// To highlight down-bar in indicator
		            // SetBackgroundColor( bar, Color.LightPink );

					if(UpBar != -1)
					{
						Range[bar]=	Math.Abs( High[UpBar]-Low[bar]);
						TimeRange[bar]=	bar-UpBar;
					}
				}
			}

					#endregion

			#region Variables for position maintenance

			bool signalBuy = false, signalShort = false;
			#endregion
			ChartPane DeMarkPane2 = CreatePane(30, true, true);
			DataSeries fvMed = FvMedian.Series(Range,10);
			PlotSeries(DeMarkPane2, fvMed, Color.FromArgb(119,168,27,0), LineStyle.Histogram, 2);

			bool isNotLastHour = false;
			for (int bar = firstValidValue; bar < Bars.Count; bar++)
			{
				if(fvMed[bar]==0)
					fvMed[bar]=fvMed[bar-1];

				#region Entry&Exit signals

				signalBuy = Close[bar] > highLevel[bar-1];
				signalShort = Bars.Close[bar] < lowLevel[bar-1];

				#endregion

				#region Exit from position

				if (!IsLastPositionActive) // If position exist
				{

					// Receiving a signal to enter a long position with a trailing below the close
					if (signalBuy)
					{
						BuyAtLimit(bar + 1,  Bars.Close[bar] - tick, "Buy");
						orderStopLoss = Close[bar]-(fvMed[bar]);
						orderTakeProfit = Close[bar]+(fvMed[bar]);

					}
						// Receiving a signal to enter a short position with a trailing above the close
					else if (signalShort)
					{
						ShortAtLimit(bar + 1,  Bars.Close[bar] + tick, "Short");
						orderStopLoss = Close[bar]+(fvMed[bar]);
						orderTakeProfit = Close[bar]-(fvMed[bar]);
					}
				}
				else // If position exist
				{
					if (LastActivePosition.PositionType == PositionType.Long) // For long position
					{
						SellAtStop(bar+1, LastActivePosition, orderStopLoss, "Sell Stop"); // S/L exit
						SellAtLimit(bar+1, LastActivePosition, orderTakeProfit, "Sell"); // T/P exit
					}
					else // For short position
					{
						CoverAtStop(bar+1, LastActivePosition, orderStopLoss, "Cover Stop"); // S/L exit
						CoverAtLimit(bar+1, LastActivePosition,orderTakeProfit , "Cover"); // T/P exit
					}
				}

				#endregion
			}
		}
	}
}
