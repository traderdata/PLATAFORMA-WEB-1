using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_FractalChaosOscillator()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")
        2. paramInt1 = Periods (eg 14)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.OsciladorFractalChaos, typeof(FractalChaosOscillator),
                                  "Fractal Chaos Oscillator",
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
                                          // Periods (eg 14)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Periodos),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Periodos,
                                          DefaultValue = 10,
                                          ValueType = typeof (int)
                                        },
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The chaotic nature of stock market movements explains why it is sometimes difficult to distinguish hourly charts from monthly charts if the time scale is not given. The patterns are similar regardless of the time resolution. Like the chambers of the nautilus, each level is like the one before it, but the size is different. To determine what is happening in the current level of resolution, the fractal chaos oscillator can be used to examine these patterns.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Periods</term></item>
    /// </list>
    /// </remarks>
    public class FractalChaosOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public FractalChaosOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.OsciladorFractalChaos;

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

            int paramInt1 = ParamInt(1);
            if (paramInt1 < 1 || paramInt1 > size)
            {
                DisparaErroIndicador("Períodos inválidos para o indicador Oscilador Fractal Chaos.\nAumente o período selecionado.", ErroIndicador.PeriodosInvalidos);
                return false;
            }

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
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.FractalChaosOscillator(pNav, pRS, paramInt1, FullName);


            // Output the indicator values
            Clear();

            for (int n = 0; n < size; ++n)
            {
                AppendValue(DM.GetTimeStampByIndex(n), n < paramInt1 ? null : pInd.Value(FullName, n + 1));
            }

            return _calculateResult = PostCalculate();
        }

        #region VerificaPeriodoValido()
        /// <summary>
        ///  Verifica se a quantidade de records/barras são suficientes para o cálculo do indicador.
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public override bool VerificaPeriodoValido(int records)
        {
            // Validate
            if (records == 0)
                return false;

            int paramInt1 = ParamInt(1);
            if (paramInt1 < 1 || paramInt1 > records)
                return false;

            return true;
        }
        #endregion VerificaPeriodoValido()
    }
}
