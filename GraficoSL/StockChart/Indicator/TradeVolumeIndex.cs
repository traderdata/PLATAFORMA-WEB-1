using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_TradeVolumeIndex()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Source (eg "msft.Ultimo")
        2. paramStr1 = Volume (eg "msft.volume")
        3. paramDbl2 = Minimum Tick Value (eg 0.25)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.TradeVolumeIndex, typeof(TradeVolumeIndex),
                                  "Trade Volume Index",
                                  new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Source (eg "msft.Ultimo")
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Serie),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Serie,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Volume (eg "msft.volume")
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Volume),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Volume,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Minimum Tick Value (eg 0.25)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.ValorMinimoTick),
                                          ParameterType = EnumGeral.TipoParametroIndicador.ValorMinimoTick,
                                          DefaultValue = 0.25,
                                          ValueType = typeof (double)
                                        },
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The Trade Volume index shows whether a security is being accumulated or distribute (similar to the Accumulation/Distribution index).
    /// </summary>
    /// <remarks>When the indicator is rising, the security is said to be accumulating. Conversely, when the indicator is falling, the security is said to being distributing. Prices may reverse when the indicator converges with price.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>str Volume</term></item>
    /// <item><term>dbl Minimum Tick Value</term></item>
    /// </list>
    /// </remarks>
    public class TradeVolumeIndex : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public TradeVolumeIndex(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.TradeVolumeIndex;

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

            double paramDbl2 = ParamDbl(2);
            if (paramDbl2 <= 0.01 || paramDbl2 >= 0.9)
            {
                DisparaErroIndicador("Valor de tick mínimo inválido para o indicador Trade Volume Index (0.01 até 0.9).", ErroIndicador.MinTickValueInvalido);
                return false;
            }


            // Get the data
            string paramStr0 = ParamStr(0);
            Field pSource = SeriesToField("Source", paramStr0, iSize);
            if (!EnsureField(pSource, paramStr0)) return false;
            string paramStr1 = ParamStr(1);
            Field pVolume = SeriesToField("Volume", paramStr1, iSize);
            if (!EnsureField(pVolume, paramStr0)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pRS.AddField(pVolume);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.TradeVolumeIndex(pNav, pSource, pVolume, paramDbl2, FullName);

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

