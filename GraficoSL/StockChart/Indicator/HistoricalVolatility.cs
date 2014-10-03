using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_HistoricalVolatility()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Source (eg "msft.Ultimo")
        2. paramInt1 = Periods (eg 14)
        3. paramInt2 = Bar History (eg 365)
        4. paramInt3 = Standard Deviations (eg 2)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.VolatilidadeHistorica, typeof(HistoricalVolatility),
                                  "Historical Volatility",
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
                                          // Bar History (eg 365)
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.BarraHistorica),
                                          ParameterType = EnumGeral.TipoParametroIndicador.BarraHistorica,
                                          DefaultValue = 365,
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
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// Historical volatility is the log-normal standard deviation. The Historical Volatility Index is based on the book by Don Fishback, "Odds: The Key to 90% Winners".
    ///
    ///This formula will output a 30-day historical volatility index between 1 and 0: 
    ///Stdev(Log(Close / Close Yesterday), 30) * Sqrt(365)
    ///
    ///Note that some traders use 252 instead of 365 for the bar history that is used in the square root calculation.
    ///The Log value is a natural log (ie Log10).
    /// </summary>
    /// <remarks>Similar to the coefficient of determination, the higher the value is, the more volatile the stock is. 
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source (usually the close price)</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>int BarHistory</term></item>
    /// <item><term>int StandardDeviations</term></item>
    /// </list>
    /// </remarks>
    public class HistoricalVolatility : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public HistoricalVolatility(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.VolatilidadeHistorica;

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
            if (paramInt1 < 1 || paramInt1 > size / 2)
            {
                DisparaErroIndicador("Período inválido para o indicador Volatilidade Histórica.", ErroIndicador.PeriodosInvalidos);
                return false;
            }

            int paramInt2 = ParamInt(2);
            if (paramInt2 < 1)
            {
                DisparaErroIndicador("Barra histórica inválida para o indicador Volatilidade Histórica.", ErroIndicador.BarraHistoricaInvalida);
                return false;
            }

            int paramInt3 = ParamInt(3);
            if (paramInt3 < 0)
            {
                DisparaErroIndicador("Desvio padrão inválido para o indicador Volatilidade Histórica.", ErroIndicador.DesvioPadraoInvalido);
                return false;
            }

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pSource = SeriesToField("Source", paramStr0, size);
            if (!EnsureField(pSource, paramStr0)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pNav.Recordset_ = pRS;

            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.HistoricalVolatility(pNav, pSource, paramInt1, paramInt2, paramInt3, FullName);


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
            if (paramInt1 < 1 || paramInt1 > records / 2)
                return false;

            return true;
        }
        #endregion VerificaPeriodoValido()
    }
}
