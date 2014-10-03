using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_SwingIndex()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")
        2. paramDbl1 = Limit Move Value (eg 12)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.SwingIndex, typeof(SwingIndex),
                                  "Swing Index",
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
                                      new IndicatorParameter
                                        {
                                          // Limit Move Value (eg 12)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.ValorMovelLimite),
                                          ParameterType = EnumGeral.TipoParametroIndicador.ValorMovelLimite,
                                          DefaultValue = 12,
                                          ValueType = typeof (int)
                                        },
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The Swing Index (Wilder) is a popular indicator that shows comparative price strength within a single security by comparing the current open, high, low and close prices with previous prices.
    /// </summary>
    /// <remarks>The Swing Index is a component of the Accumulation Swing Index
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>dbl Limit Move Value</term></item>
    /// </list>
    /// </remarks>
    public class SwingIndex : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public SwingIndex(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.SwingIndex;

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

            double paramDbl1 = ParamDbl(1);
            if (paramDbl1 <= 0)
            {
                DisparaErroIndicador("Limit Move inválido para o indicador Swing Index.", ErroIndicador.LimitMoveInvalido);
                return false;
            }

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pOpen = SeriesToField("Abertura", paramStr0 + ".Abertura", iSize);
            if (!EnsureField(pOpen, paramStr0 + ".Abertura")) return false;
            Field pHigh = SeriesToField("Maximo", paramStr0 + ".Maximo", iSize);
            if (!EnsureField(pHigh, paramStr0 + ".Maximo")) return false;
            Field pLow = SeriesToField("Minimo", paramStr0 + ".Minimo", iSize);
            if (!EnsureField(pLow, paramStr0 + ".Minimo")) return false;
            Field pClose = SeriesToField("Ultimo", paramStr0 + ".Ultimo", iSize);
            if (!EnsureField(pClose, paramStr0 + ".Ultimo")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pOpen);
            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.SwingIndex(pNav, pRS, paramDbl1, FullName);

            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), pInd.Value(FullName, n + 1));
            }
            return _calculateResult = PostCalculate();
        }
    }
}
