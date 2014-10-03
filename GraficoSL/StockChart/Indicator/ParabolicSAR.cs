using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_ParabolicSAR()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")   
        2. paramDbl1 = MinAF (eg 0.02)
        3. paramDbl2 = MaxAF (eg 0.2)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.SARParabólico, typeof(ParabolicSAR),
                                  "Parabolic SAR",
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
                                          // MinAF (eg 0.02)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.AFMinimo),
                                          ParameterType = EnumGeral.TipoParametroIndicador.AFMinimo,
                                          DefaultValue = 0.02,
                                          ValueType = typeof (double)
                                        },
                                      new IndicatorParameter
                                        {
                                          // MaxAF (eg 0.2)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.AFMaximo),
                                          ParameterType = EnumGeral.TipoParametroIndicador.AFMaximo,
                                          DefaultValue = 0.2,
                                          ValueType = typeof (double)
                                        },
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The Parabolic SAR was  developed by Welles Wilder. This indicator is always in the market (whenever a position is closed, an opposing position is taken).  The Parabolic SAR indicator is most often used to set trailing price stops. A stop and reversal (SAR) occurs when the price penetrates a  Parabolic SAR level.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>dbl Min AF (accumulation factor)</term></item>
    /// <item><term>dbl Max AF (accumulation factor)</term></item>
    /// </list>
    /// </remarks>
    public class ParabolicSAR : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public ParabolicSAR(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.SARParabólico;

            Init();

            _strokePattern = EnumGeral.TipoLinha.Pontilhado;
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
                DisparaErroIndicador("Max AF inválido para o indicador Parabolic SAR.", ErroIndicador.MaxAFInvalido);
                return false;
            }
            double paramDbl2 = ParamDbl(2);
            if (paramDbl2 <= 0)
            {
                DisparaErroIndicador("Min AF inválido para o indicador Parabolic SAR.", ErroIndicador.MinAFInvalido);
                return false;
            }


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
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.ParabolicSAR(pNav, pHigh, pLow, paramDbl1, paramDbl2, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < 2 ? null : pInd.Value(FullName, n + 1));
            }
            return _calculateResult = PostCalculate();
        }
    }
}

