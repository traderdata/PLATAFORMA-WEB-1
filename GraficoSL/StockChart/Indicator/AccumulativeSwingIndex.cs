using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_AccumulativeSwingIndex()
    {
      /*  Required inputs for this indicator:
        1. paramStr[0] = Symbol (eg "msft")
        2. paramDbl[1] = Limit Move Value (eg 12)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.AccumulativeSwingIndex, typeof(AccumulativeSwingIndex),
                                  "Accumulative Swing Index",
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
                                        }
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The Accumulation Swing Index (Wilder) is a cumulative total of the Swing Index.
    /// </summary>
    /// <remarks>The Accumulation Swing Index may be analyzed using technical indicators, line studies, and chart patterns, as an alternative view of price action.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>dbl Limit Move Value</term></item>
    /// </list>
    /// </remarks>
    public class AccumulativeSwingIndex : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public AccumulativeSwingIndex(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.AccumulativeSwingIndex;

            Init();
        }

        /// <summary>
        /// Action to be executd for calculating indicator
        /// </summary>
        /// <returns>for future usage. Must be ignored at this time.</returns>
        protected override bool TrueAction()
        {
            int size = _chartPanel._chartX.RecordCount;
            if (size == 0) return false;

            if (ParamDbl(1) <= 0)
            {
                DisparaErroIndicador("Valor inválido para o Limit Move do indicador Acumulação Swing Index", ErroIndicador.LimitMoveInvalido);
                return false;
            }

            // Get the data
            Field pOpen = SeriesToField("Abertura", ParamStr(0) + ".Abertura", size);

            if (!EnsureField(pOpen, ParamStr(0) + ".Abertura")) 
                return false;

            Field pHigh = SeriesToField("Maximo", ParamStr(0) + ".Maximo", size);

            if (!EnsureField(pHigh, ParamStr(0) + ".Maximo"))
                return false;
            Field pLow = SeriesToField("Minimo", ParamStr(0) + ".Minimo", size);

            if (!EnsureField(pLow, ParamStr(0) + ".Minimo")) 
                return false;

            Field pClose = SeriesToField("Ultimo", ParamStr(0) + ".Ultimo", size);

            if (!EnsureField(pClose, ParamStr(0) + ".Ultimo")) 
                return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pOpen);
            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.AccumulativeSwingIndex(pNav, pRS, ParamDbl(1), FullName);

            // Output the indicator values
            Clear();

            Series series = _chartPanel._chartX.GetSeriesByName(ParamStr(0) + ".Ultimo");

            for (int n = 0; n < size; ++n)
            {
                double? value = pInd.Value(FullName, n + 1);
                AppendValue(series[n].TimeStamp, value);
            }

            return _calculateResult = PostCalculate();
        }
    }
}
