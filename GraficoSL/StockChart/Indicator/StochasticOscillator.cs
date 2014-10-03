using System.Collections.Generic;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_StochasticOscillator()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")
        2. paramInt1 = %K Periods (eg 14)
        3. paramInt2 = %K Slowing (eg 3)
        4. paramInt3 = %D Periods (eg 5)
        5. paramInt[4] = Moving Average Type (eg indSimpleMovingAverage)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.OsciladorEstocastico, typeof(StochasticOscillator),
                                  "Stochastic Oscillator",
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
                                          // %K Periods (eg 14)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.PercentKPeriodos),
                                          ParameterType = EnumGeral.TipoParametroIndicador.PercentKPeriodos,
                                          DefaultValue = 9,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // %K Slowing (eg 3)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.PercentKRetardo),
                                          ParameterType = EnumGeral.TipoParametroIndicador.PercentKRetardo,
                                          DefaultValue = 3,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // %D Periods (eg 5)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.PercentDPeriodos),
                                          ParameterType = EnumGeral.TipoParametroIndicador.PercentDPeriodos,
                                          DefaultValue = 9,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Moving Average Type (eg indSimpleMovingAverage)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.TipoMediaMovel),
                                          ParameterType = EnumGeral.TipoParametroIndicador.TipoMediaMovel,
                                          DefaultValue = EnumGeral.IndicatorType.MediaMovelSimples,
                                          ValueType = typeof (EnumGeral.IndicatorType)
                                        },
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The Stochastic Oscillator is a popular indicator that shows where a security’s price has closed in proportion to its closing price range over a specified period of time.
    /// </summary>
    /// <remarks>The Stochastic Oscillator has two components: %K and %D. %K is most often displayed as a solid line and %D is often shown as a dotted line. The most widely used method for interpreting the Stochastic Oscillator is to buy when either component rises above 80 or sell when either component falls below 20. Another way to interpret the Stochastic Oscillator is to buy when %K rises above %D, and conversely, sell when %K falls below %D.
    /// 
    /// The most commonly used arguments are 9 for %K periods, 3 for %K slowing periods and 3 for %D smoothing.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int %K Periods</term></item>
    /// <item><term>int %K Slowing</term></item>
    /// <item><term>int %D Periods</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// </list>
    /// </remarks>
    public class StochasticOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public StochasticOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.OsciladorEstocastico;

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
                DisparaErroIndicador("%K periodos inválidos para o indicador Oscilador Stochastico.", ErroIndicador.PercKPeriodoInvalido);
                return false;
            }
            int paramInt2 = ParamInt(2);
            if (paramInt2 < 2 || paramInt2 > iSize / 2)
            {
                DisparaErroIndicador("%K Slowing periodos inválidos para o indicador Oscilador Stochastico.", ErroIndicador.PercKSlowingPeriodosInvalido);
                return false;
            }
            int paramInt3 = ParamInt(3);
            if (paramInt3 <= 0 || paramInt3 > iSize / 2)
            {
                DisparaErroIndicador("%D periodos inválidos para o indicador Oscilador Stochastico.", ErroIndicador.PercDPeriodsInvalido);
                return false;
            }
            EnumGeral.IndicatorType param4 = (EnumGeral.IndicatorType)ParamInt(4);
            if (param4 < Constants.MA_START || param4 > Constants.MA_END)
            {
                DisparaErroIndicador("Tipo Média móvel inválido para o indicador Oscilador Stochastico.", ErroIndicador.PercDPeriodsInvalido);
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

            Recordset pInd = ta.StochasticOscillator(pNav, pRS, paramInt1, paramInt2, paramInt3, param4);


            // Output the indicator values
            Clear();

            IndicadorSerieFilha sPctK = (IndicadorSerieFilha)EnsureSeries(FullName + " %K");
            sPctK.SetStrokeColor(CorSerieFilha1, false);
            sPctK.SetStrokeThickness(GrossuraSerieFilha1, false);
            sPctK.SetStrokePattern(TipoLinhaSerieFilha1, false);
            sPctK.SeriePai = this;

            sPctK.IsSerieFilha = true;

            if (this.SeriesFilhas == null)
                this.SeriesFilhas = new List<Series>();

            this.SeriesFilhas.Add(sPctK);

            _strokePattern = EnumGeral.TipoLinha.Pontilhado;
            _title = FullName + " %D";

            for (int n = 0; n < iSize; ++n)
            {
                double? pctd = n < paramInt3 ? null : pInd.Value("%D", n + 1);
                double? pctk = n < paramInt1 ? null : pInd.Value("%K", n + 1);

                AppendValue(DM.TS(n), pctd);
                sPctK.AppendValue(DM.TS(n), pctk);
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

            int paramInt2 = ParamInt(2);
            if (paramInt2 < 2 || paramInt2 > records / 2)
                return false;

            int paramInt3 = ParamInt(3);
            if (paramInt3 <= 0 || paramInt3 > records / 2)
                return false;

            return true;
        }
        #endregion VerificaPeriodoValido()
    }
}

