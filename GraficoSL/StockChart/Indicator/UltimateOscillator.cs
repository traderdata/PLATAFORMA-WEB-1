using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_UltimateOscillator()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")
        2. paramInt1 = Cycle 1 (eg 7)
        3. paramInt2 = Cycle 2 (eg 14)
        4. paramInt3 = Cycle 3 (eg 28)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.OsciladorUltimate, typeof(UltimateOscillator),
                                  "Ultimate Oscillator",
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
                                          // Cycle 1 (eg 7)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Ciclo1),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Ciclo1,
                                          DefaultValue = 7,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Cycle 2 (eg 14)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Ciclo2),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Ciclo2,
                                          DefaultValue = 14,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Cycle 1 (eg 28)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Ciclo3),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Ciclo3,
                                          DefaultValue = 28,
                                          ValueType = typeof (int)
                                        },
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The Ultimate Oscillator compares prices with three oscillators, using three different periods for calculations.
    /// </summary>
    /// <remarks>The most popular interpretation of the Ultimate Oscillator is price/indicator divergence.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Cycle 1</term></item>
    /// <item><term>int Cycle 2</term></item>
    /// <item><term>int Cycle 3</term></item>
    /// </list>
    /// </remarks>
    public class UltimateOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public UltimateOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.OsciladorUltimate;

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

            int paramInt1 = ParamInt(1);
            if (paramInt1 < 1)
            {
                DisparaErroIndicador("Ciclo 1 inválido para o indicador Oscilador Ultimate", ErroIndicador.Ciclo1Invalido);
                return false;
            }
            int paramInt2 = ParamInt(2);
            if (paramInt2 < 1)
            {
                DisparaErroIndicador("Ciclo 2 inválido para o indicador Oscilador Ultimate", ErroIndicador.Ciclo2Invalido);
                return false;
            }
            int paramInt3 = ParamInt(3);
            if (paramInt3 < 1)
            {
                DisparaErroIndicador("Ciclo 3 inválido para o indicador Oscilador Ultimate", ErroIndicador.Ciclo3Invalido);
                return false;
            }

            // Get the data
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
            Recordset pInd = ta.UltimateOscillator(pNav, pRS, paramInt1, paramInt2, paramInt3, FullName);


            // Output the indicator values
            Clear();
            int max = paramInt1;
            if (paramInt2 > max) max = paramInt2;
            if (paramInt3 > max) max = paramInt3;
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < max ? null : pInd.Value(FullName, n + 1));
            }

            return _calculateResult = PostCalculate();
        }
    }
}

