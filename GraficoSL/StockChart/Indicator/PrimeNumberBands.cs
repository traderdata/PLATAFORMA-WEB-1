using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_PrimeNumberBands()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.BandasNumerosPrimos, typeof(PrimeNumberBands),
                                  "Prime Number Bands",
                                  new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Symbol (eg "msft")
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Ativo),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Ativo,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// Similar to the Prime Numbers Oscillator, the prime numbers oscillator was developed by Modulus Financial Engineering, Inc. This indicator finds the nearest prime number for the high and low, and plots the two series as bands.
    /// </summary>
    /// <remarks>This indicator can be used to spot market trading ranges.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// </list>
    /// </remarks>
    public class PrimeNumberBands : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public PrimeNumberBands(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.BandasNumerosPrimos;

            Init();
        }

        /// <summary>
        /// Action to be executd for calculating indicator
        /// </summary>
        /// <returns>for future usage. Must be ignored at this time.</returns>
        protected override bool TrueAction()
        {
            // Validate
            int iSize = _chartPanel._chartX.RecordCount;
            if (iSize == 0)
                return false;

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pHigh = SeriesToField("Maximo", paramStr0 + ".Maximo", iSize);
            if (!EnsureField(pHigh, paramStr0 + ".Maximo")) return false;
            Field pLow = SeriesToField("Minimo", paramStr0 + ".Minimo", iSize);
            if (!EnsureField(pLow, paramStr0 + ".Minimo")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Bands ta = new Bands();
            Recordset pInd = ta.PrimeNumberBands(pNav, pHigh, pLow);

            // Output the indicator values
            Clear();

            IndicadorSerieFilha sBottom = (IndicadorSerieFilha)EnsureSeries(FullName + " Bottom");
            sBottom.SetStrokeColor(CorSerieFilha1, false);
            sBottom.SetStrokeThickness(GrossuraSerieFilha1, false);
            sBottom.SetStrokePattern(TipoLinhaSerieFilha1, false);
            sBottom.IsSerieFilha = true;
            sBottom.SeriePai = this;

            if (this.SeriesFilhas == null)
                this.SeriesFilhas = new List<Series>();

            this.SeriesFilhas.Add(sBottom);

            _title = FullName + " Top";

            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), pInd.Value("Prime Bands Top", n + 1));
                sBottom.AppendValue(DM.TS(n), pInd.Value("Prime Bands Bottom", n + 1));
            }
            return _calculateResult = PostCalculate();
        }
    }
}
