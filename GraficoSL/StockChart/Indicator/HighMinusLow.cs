using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_HighMinusLow()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")   
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.HighMinusLow, typeof(HighMinusLow),
                                  "Maximo Minus Minimo",
                                  new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Source (eg "msft.Ultimo")
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Ativo),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Ativo,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        }
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// Returns the high price minus the low price.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// </list>
    /// </remarks>
    public class HighMinusLow : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public HighMinusLow(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.HighMinusLow;

            Init();
        }

        /// <summary>
        /// Action to be executd for calculating indicator
        /// </summary>
        /// <returns>for future usage. Must be ignored at this time.</returns>
        protected override bool TrueAction()
        {
            // Validate
            int size = _chartPanel._chartX.RecordCount;
            if (size == 0)
                return false;

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pHigh = SeriesToField("Maximo", paramStr0 + ".Maximo", size);
            if (!EnsureField(pHigh, paramStr0 + ".Maximo")) return false;
            Field pLow = SeriesToField("Minimo", paramStr0 + ".Minimo", size);
            if (!EnsureField(pLow, paramStr0 + ".Minimo")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);


            pNav.Recordset_ = pRS;


            // Calculate the indicator
            General ta = new General();
            Recordset pInd = ta.HighMinusLow(pNav, pRS, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < size; ++n)
            {
                AppendValue(DM.GetTimeStampByIndex(n), pInd.Value(FullName, n + 1));
            }

            return _calculateResult = PostCalculate();
        }
    }
}
