using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_WilliamsAccumulationDistribution()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")   
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.AcumulacaoDistribuicaoWilliams, typeof(WilliamsAccumulationDistribution),
                                  "Williams Accumulation Distribution",
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
  /// The Accumulation/Distribution indicator shows a relationship of price and volume.
  /// </summary>
  /// <remarks>When the indicator is rising, the security is said to be accumulating. Conversely, when the indicator is falling, the security is said to being distributing. Prices may reverse when the indicator converges with price.
  /// <list type="table">
  /// <listheader>
  /// <term>Parameters</term>
  /// </listheader>
  /// <item><term>str Symbol</term></item>
  /// </list>
  /// </remarks>
  public class WilliamsAccumulationDistribution : Indicator
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Indicator name</param>
    /// <param name="chartPanel">Reference to a panel where it will be placed</param>
    public WilliamsAccumulationDistribution(string name, ChartPanel chartPanel)
      : base(name, chartPanel)
    {
        _indicatorType = EnumGeral.IndicatorType.AcumulacaoDistribuicaoWilliams;

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
      Field pHigh = SeriesToField("Maximo", paramStr0 + ".Maximo", iSize);
      if (!EnsureField(pHigh, paramStr0 + ".Maximo")) return false;
      Field pLow = SeriesToField("Minimo", paramStr0 + ".Minimo", iSize);
      if (!EnsureField(pLow, paramStr0 + ".Minimo")) return false;
      Field pClose = SeriesToField("Ultimo", paramStr0 + ".Ultimo", iSize);
      if (!EnsureField(pClose, paramStr0 + ".Ultimo")) return false;

      Navigator pNav = new Navigator();
      Recordset pRS = new Recordset();

      pRS.AddField(pHigh);
      pRS.AddField(pLow);
      pRS.AddField(pClose);

      pNav.Recordset_ = pRS;


      // Calculate the indicator
      Oscillator ta = new Oscillator();
      Recordset pInd = ta.WilliamsAccumulationDistribution(pNav, pRS, FullName);


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
