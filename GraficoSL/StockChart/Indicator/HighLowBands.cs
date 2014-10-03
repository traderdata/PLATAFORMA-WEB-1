using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_HighLowBands()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")
        2. paramInt1 = Periods (eg 14)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.BandasMaximoMinimo, typeof(HighLowBands),
                                  "Maximo Minimo Bands",
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
                                          DefaultValue = 14,
                                          ValueType = typeof (int)
                                        },
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// High Low Bands consist of triangular moving averages calculated from the underling price, shifted up and down by a fixed percentage, and include a median value.
    /// </summary>
    /// <remarks>When prices rise above the upper band or fall below the lower band, a change in direction may occur when the price penetrates the band after a small reversal from the opposite direction.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Periods</term></item>
    /// </list>
    /// </remarks>
    public class HighLowBands : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public HighLowBands(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.BandasMaximoMinimo;

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
            if (paramInt1 < 6 || paramInt1 > size / 2)
            {
                DisparaErroIndicador("Período inválido para o indicador Bandas Máxima/Mínima (6 no mínimo).", ErroIndicador.PeriodosInvalidos);
                return false;
            }


            // Get the data
            string paramStr0 = ParamStr(0);
            Field pHigh = SeriesToField("Maximo", paramStr0 + ".Maximo", size);
            if (!EnsureField(pHigh, paramStr0 + ".Maximo")) return false;
            Field pLow = SeriesToField("Minimo", paramStr0 + ".Minimo", size);
            if (!EnsureField(pLow, paramStr0 + ".Minimo")) return false;
            Field pClose = SeriesToField("Ultimo", paramStr0 + ".Ultimo", size);

            if (!EnsureField(pClose, paramStr0 + ".Ultimo")) return false;
            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Bands ta = new Bands();
            Recordset pInd = ta.HighLowBands(pNav, pHigh, pLow, pClose, paramInt1);


            // Output the indicator values
            Clear();

            IndicadorSerieFilha sTop = (IndicadorSerieFilha)EnsureSeries(FullName + " Top");
            sTop.SetStrokeColor(CorSerieFilha1, false);
            sTop.SetStrokeThickness(GrossuraSerieFilha1, false);
            sTop.SetStrokePattern(TipoLinhaSerieFilha1, false);
            sTop.IsSerieFilha = true;
            sTop.SeriePai = this;

            IndicadorSerieFilha sBottom = (IndicadorSerieFilha)EnsureSeries(FullName + " Bottom");
            sBottom.SetStrokeColor(CorSerieFilha2, false);
            sBottom.SetStrokeThickness(GrossuraSerieFilha2, false);
            sBottom.SetStrokePattern(TipoLinhaSerieFilha2, false);
            sBottom.IsSerieFilha = true;
            sBottom.SeriePai = this;


            if (this.SeriesFilhas == null)
                this.SeriesFilhas = new List<Series>();

            this.SeriesFilhas.Add(sTop);
            this.SeriesFilhas.Add(sBottom);

            for (int n = 0; n < size; ++n)
            {
                double? top;
                double? bottom;
                double? median;
                if (n < paramInt1)
                {
                    top = null;
                    median = null;
                    bottom = null;
                }
                else
                {
                    top = pInd.Value("Maximo Minimo Bands Top", n + 1);
                    median = pInd.Value("Maximo Minimo Bands Median", n + 1);
                    bottom = pInd.Value("Maximo Minimo Bands Bottom", n + 1);
                }

                AppendValue(DM.GetTimeStampByIndex(n), median);
                sTop.AppendValue(DM.GetTimeStampByIndex(n), top);
                sBottom.AppendValue(DM.GetTimeStampByIndex(n), bottom);
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
            if (paramInt1 < 6 || paramInt1 > records / 2)
                return false;

            return true;
        }
        #endregion VerificaPeriodoValido()
    }
}

