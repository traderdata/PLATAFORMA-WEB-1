using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_MovingAverageEnvelope()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Source (eg "msft.Ultimo")
        2. paramInt1 = Periods (eg 14)
        3. paramInt2 = Moving Average Type (eg indSimpleMovingAverage)
        4. paramDbl[3] = Shift (eg 5%)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.MediaMovelEnvelope, typeof(MovingAverageEnvelope),
                                  "Moving Average Envelope",
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
                                          // Moving Average Type (eg indSimpleMovingAverage)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.TipoMediaMovel),
                                          ParameterType = EnumGeral.TipoParametroIndicador.TipoMediaMovel,
                                          DefaultValue = EnumGeral.IndicatorType.MediaMovelSimples,
                                          ValueType = typeof (EnumGeral.IndicatorType)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Shift (eg 14)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Shift),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Shift,
                                          DefaultValue = 5,
                                          ValueType = typeof (double)
                                        },
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// Moving Average Envelopes consist of moving averages calculated from the underling price, shifted up and down by a fixed percentage.
    /// </summary>
    /// <remarks>Moving Average Envelopes (or trading bands) can be imposed over an actual price or another indicator.
    ///When prices rise above the upper band or fall below the lower band, a change in direction may occur when the price penetrates the band after a small reversal from the opposite direction.
    ///
    /// Shift is a double value specifying the percentage of shift for each moving average from the actual values.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// <item><term>dbl Shift</term></item>
    /// </list>
    /// </remarks>
    public class MovingAverageEnvelope : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public MovingAverageEnvelope(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.MediaMovelEnvelope;

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
            if (paramInt1 < 1 || paramInt1 > iSize / 2)
            {
                DisparaErroIndicador("Períodos inválidos para o indicador Média Móvel Envelope.\nAumente o período selecionado.", ErroIndicador.PeriodosInvalidos);
                return false;
            }
            EnumGeral.IndicatorType param2 = (EnumGeral.IndicatorType)ParamInt(2);
            if (param2 < Constants.MA_START || param2 > Constants.MA_END)
            {
                DisparaErroIndicador("Tipo de média móvel inválido para o indicador Média Móvel Envelope.", ErroIndicador.TipoMediaMovelInvalida);
                return false;
            }
            double paramDbl3 = ParamDbl(3);
            if (paramDbl3 < 0 || paramDbl3 > 100)
            {
                DisparaErroIndicador("Porcentagem de Band Shift inválida para o indicador Média Móvel Envelope.", ErroIndicador.PorcBandShift);
                return false;
            }


            // Get the data  
            string paramStr0 = ParamStr(0);
            Field pSource = SeriesToField("Source", paramStr0, iSize);
            if (!EnsureField(pSource, paramStr0)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Bands ta = new Bands();
            Recordset pInd = ta.MovingAverageEnvelope(pNav, pSource, paramInt1, param2, paramDbl3);


            // Output the indicator values
            Clear();

            IndicadorSerieFilha sTop = (IndicadorSerieFilha)EnsureSeries(FullName + " Top");
            sTop.SetStrokeColor(CorSerieFilha1, false);
            sTop.SetStrokeThickness(GrossuraSerieFilha1, false);
            sTop.SetStrokePattern(TipoLinhaSerieFilha1, false);
            sTop.IsSerieFilha = true;
            sTop.SeriePai = this;

            if (this.SeriesFilhas == null)
                this.SeriesFilhas = new List<Series>();

            this.SeriesFilhas.Add(sTop);

            _title = FullName + " Bottom";

            for (int n = 0; n < iSize; ++n)
            {
                double? top;
                double? bottom;
                if (n < paramInt1 * 2)
                {
                    top = null;
                    bottom = null;
                }
                else
                {
                    top = pInd.Value("Envelope Top", n + 1);
                    bottom = pInd.Value("Envelope Bottom", n + 1);
                }
                AppendValue(DM.TS(n), bottom);
                sTop.AppendValue(DM.TS(n), top);
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
            if (paramInt1 < 1 || paramInt1 > records / 2)
                return false;

            return true;
        }
        #endregion VerificaPeriodoValido()
    }
}
