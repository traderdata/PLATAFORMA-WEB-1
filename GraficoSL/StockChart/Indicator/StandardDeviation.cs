using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_StandardDeviation()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Source (eg "msft.Ultimo")
        2. paramInt1 = Periods (eg 14)
        3. paramInt2 = Standard Deviations (eg 2)
        4. paramInt3 = Moving Average Type (eg indSimpleMovingAverage)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.DesvioPadrao, typeof(StandardDeviation),
                                  "Standard Deviation",
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
                                          ValueType = typeof (double)
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
    /// Standard Deviation is a common statistical calculation that measures volatility. Other technical indicators are often calculated using standard deviations.
    /// </summary>
    /// <remarks>Major highs and lows often accompany extreme volatility. High values of standard deviations indicate that the price or indicator is more volatile than usual.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>int Standard Deviations</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// </list>
    /// </remarks>
    public class StandardDeviation : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public StandardDeviation(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.DesvioPadrao;

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
            if (paramInt1 < 2 || paramInt1 > iSize / 2)
            {
                DisparaErroIndicador("Períodos inválidos para o indicador Desvio Padrão (2 no mínimo).\nAumente o período selecionado.", ErroIndicador.PeriodosInvalidos);
                return false;
            }
            double paramDbl2 = ParamDbl(2);
            if (paramDbl2 < 0.0001 || paramDbl2 > 10)
            {
                DisparaErroIndicador("Desvio padrão inválido para o indicador Desvio Padrão (min > 0).", ErroIndicador.DesvioPadraoInvalido);
                return false;
            }
            EnumGeral.IndicatorType param3 = (EnumGeral.IndicatorType)ParamInt(3);
            if (param3 < Constants.MA_START || param3 > Constants.MA_END)
            {
                DisparaErroIndicador("Tipo de média móvel inválido para o indicador Desvio Padrão.", ErroIndicador.TipoMediaMovelInvalida);
                return false;
            }

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pSource = SeriesToField("Source", paramStr0, iSize);
            if (pSource == null) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pNav.Recordset_ = pRS;


            // Calculate the indicator  
            General ta = new General();
            Recordset pInd = ta.StandardDeviation(pNav, pSource, paramInt1, paramDbl2, param3, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < paramInt1 ? null : pInd.Value(FullName, n + 1));
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
            if (paramInt1 < 2 || paramInt1 > records / 2)
                return false;

            return true;
        }
        #endregion VerificaPeriodoValido()
    }
}

