using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_PriceVolumeTrend()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Source (eg "msft.Ultimo")
        2. paramStr1 = Volume (eg "msft.volume")
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.TendenciaPrecoVolume, typeof(PriceVolumeTrend),
                                  "Price Volume Trend",
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
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The Price and Volume Trend index is closely related to the On Balance Volume index.
    /// </summary>
    /// <remarks>The Price and Volume Trend index generally precedes actual price movements. The premise is that well-informed investors are buying when the index rises and uninformed investors are buying when the index falls.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>str Volume</term></item>
    /// </list>
    /// </remarks>
    public class PriceVolumeTrend : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public PriceVolumeTrend(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.TendenciaPrecoVolume;

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

            string paramStr0 = ParamStr(0);
            string paramStr1 = ParamStr(1);
            if (paramStr0 == paramStr1)
            {
                DisparaErroIndicador("Série 1 não pode ser igual a série 2 para o indicador Price Volume Trend. A série 2 deve ser a série de volume.", ErroIndicador.Serie1IgualSerie2);
                return false;
            }

            // Get the data
            Field pSource = SeriesToField("Source", paramStr0, iSize);
            if (!EnsureField(pSource, paramStr0)) return false;
            Field pVolume = SeriesToField("Volume", paramStr1, iSize);
            if (!EnsureField(pVolume, paramStr1)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pRS.AddField(pVolume);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.PriceVolumeTrend(pNav, pSource, pVolume, FullName);


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

