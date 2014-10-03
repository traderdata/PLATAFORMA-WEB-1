using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_BollingerBands()
    {
      /*  Required inputs for this indicator:
        1. paramStr[0] = Source (eg "msft.Ultimo")
        2. paramInt[1] = Periods (eg 14)
        3. paramInt[2] = Standard Deviations (eg 2)
        4. paramInt[3] = Moving Average Type (eg indSimpleMovingAverage)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.BandasBollinger, typeof(BollingerBands),
                                  "Bollinger Bands",
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
                                          // Periods (eg 14)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Periodos),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Periodos,
                                          DefaultValue = 14,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Standard Deviations (eg 2)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.DesvioPadrao),
                                          ParameterType = EnumGeral.TipoParametroIndicador.DesvioPadrao,
                                          DefaultValue = 2,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Moving Average Type (eg indSimpleMovingAverage)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.TipoMediaMovel),
                                          ParameterType = EnumGeral.TipoParametroIndicador.TipoMediaMovel,
                                          DefaultValue = EnumGeral.IndicatorType.MediaMovelSimples,
                                          ValueType = typeof (EnumGeral.IndicatorType)
                                        }
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// Bollinger Bands are similar in comparison to moving average envelopes. Bollinger Bands are calculated using standard deviations instead of shifting bands by a fixed percentage.
    /// </summary>
    /// <remarks>Bollinger Bands (as with most bands) can be imposed over an actual price or another indicator.
    /// When prices rise above the upper band or fall below the lower band, a change in direction may occur when the price penetrates the band after a small reversal from the opposite direction.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>int Standard Deviations</term></item>
    /// <item><term>int Moving Average Type </term></item>
    /// </list>
    /// </remarks>

    public class BollingerBands : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public BollingerBands(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.BandasBollinger;

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
            if (size == 0) return false;

            if (ParamInt(1) < 1 || ParamInt(1) > size / 2)
            {
                DisparaErroIndicador("Períodos inválidos para o indicador Bandas Bollinger.\nAumente o período selecionado.", ErroIndicador.PeriodosInvalidos);
                return false;
            }
            if (ParamInt(2) < 0 || ParamInt(2) > 10)
            {
                DisparaErroIndicador("Desvio padrão inválido para o indicador Bandas Bollinger", ErroIndicador.DesvioPadraoInvalido);
                return false;
            }
            if (ParamInt(3) < (int)Constants.MA_START || ParamInt(3) > (int)Constants.MA_END)
            {
                DisparaErroIndicador("Tipo de média móvel inválida para o indicador Bandas Bollinger", ErroIndicador.TipoMediaMovelInvalida);
                return false;
            }

            Field pSource = SeriesToField("Source", ParamStr(0), size);
            if (!EnsureField(pSource, ParamStr(0))) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pNav.Recordset_ = pRS;

            // Calculate the indicator
            Bands ta = new Bands();
            Recordset pInd = ta.BollingerBands(pNav, pSource, ParamInt(1), ParamInt(2), (EnumGeral.IndicatorType)ParamInt(3));

            // Output the indicator values
            Clear();
            Series series = _chartPanel._chartX.GetSeriesByName(ParamStr(0));

            IndicadorSerieFilha pTop = (IndicadorSerieFilha)EnsureSeries(FullName + " Top");
            pTop.SetStrokeColor(CorSerieFilha1, false);
            pTop.SetStrokeThickness(GrossuraSerieFilha1, false);
            pTop.SetStrokePattern(TipoLinhaSerieFilha1, false);
            pTop.IsSerieFilha = true;
            pTop.SeriePai = this;

            IndicadorSerieFilha pBottom = (IndicadorSerieFilha)EnsureSeries(FullName + " Bottom");
            pBottom.SetStrokeColor(CorSerieFilha2, false);
            pBottom.SetStrokeThickness(GrossuraSerieFilha2, false);
            pBottom.SetStrokePattern(TipoLinhaSerieFilha2, false);
            pBottom.IsSerieFilha = true;
            pBottom.SeriePai = this;

            if (this.SeriesFilhas == null)
                this.SeriesFilhas = new List<Series>();

            this.SeriesFilhas.Add(pTop);
            this.SeriesFilhas.Add(pBottom);

            int paramInt1 = ParamInt(1);
            for (int n = 0; n < size; ++n)
            {
                double? top;
                double? median;
                double? bottom;
                if (n < paramInt1)
                {
                    top = null;
                    median = null;
                    bottom = null;
                }
                else
                {
                    top = pInd.Value("Bollinger Band Top", n + 1);
                    median = pInd.Value("Bollinger Band Median", n + 1);
                    bottom = pInd.Value("Bollinger Band Bottom", n + 1);
                }
                AppendValue(series[n].TimeStamp, median);
                pTop.AppendValue(series[n].TimeStamp, top);
                pBottom.AppendValue(series[n].TimeStamp, bottom);
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

            if (ParamInt(1) < 1 || ParamInt(1) > records / 2)
                return false;

            return true;
        }
        #endregion VerificaPeriodoValido()
    }
}
