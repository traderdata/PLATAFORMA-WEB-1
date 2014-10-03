using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_ChaikinMoneyFlow()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")
        2. paramStr1 = Volume (eg "msft.volume")
        3. paramInt2 = Periods (eg 14)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.ChaikinFluxoFinanceiro, typeof(ChaikinMoneyFlow),
                                  "Chaikin Money Flow",
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
                                          // Volume (eg "msft.volume")
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Volume),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Volume,
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
                                        }
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The Chaikin Money Flow oscillator is a momentum indicator that spots buying and selling by calculating price and volume together. This indicator is based upon Chaikin Accumulation/Distribution, which is in turn based upon the premise that if a stock closes above its midpoint [(high+low)/2] for the day then there was accumulation that day, and if it closes below its midpoint, then there was distribution that day.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>str Volume Source</term></item>
    /// <item><term>int Periods</term></item>
    /// </list>
    /// </remarks>
    public class ChaikinMoneyFlow : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public ChaikinMoneyFlow(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.ChaikinFluxoFinanceiro;

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

            int paramInt2 = ParamInt(2);
            if (paramInt2 < 1 || paramInt2 > size / 2)
            {
                DisparaErroIndicador("Períodos inválidos para o indicador Chaikin Money Flow.\nAumente o período selecionado.", ErroIndicador.PeriodosInvalidos);
                return false;
            }


            // Get the data
            string paramStr0 = ParamStr(0);
            string paramStr1 = ParamStr(1);
            Field pOpen = SeriesToField("Abertura", paramStr0 + ".Abertura", size);
            if (!EnsureField(pOpen, paramStr0 + ".Abertura")) return false;
            Field pHigh = SeriesToField("Maximo", paramStr0 + ".Maximo", size);
            if (!EnsureField(pHigh, paramStr0 + ".Maximo")) return false;
            Field pLow = SeriesToField("Minimo", paramStr0 + ".Minimo", size);
            if (!EnsureField(pLow, paramStr0 + ".Minimo")) return false;
            Field pClose = SeriesToField("Ultimo", paramStr0 + ".Ultimo", size);
            if (!EnsureField(pClose, paramStr0 + ".Ultimo")) return false;
            Field pVolume = SeriesToField("Volume", paramStr1, size);
            if (!EnsureField(pVolume, paramStr1)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pOpen);
            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);
            pRS.AddField(pVolume);

            pNav.Recordset_ = pRS;

            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.ChaikinMoneyFlow(pNav, pRS, paramInt2, FullName);

            // Output the indicator values
            Clear();
            for (int n = 0; n < size; ++n)
            {
                AppendValue(DM.GetTimeStampByIndex(n), n < paramInt2 ? null : pInd.Value(FullName, n + 1));
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
            if (paramInt2 < 1 || paramInt2 > records / 2)
                return false;

            return true;
        }
        #endregion VerificaPeriodoValido()
    }
}

