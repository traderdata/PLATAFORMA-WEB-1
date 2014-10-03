using System.Collections.Generic;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_Aroon()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Symbol (eg "msft")
        2. paramInt1 = Periods (eg 14)
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.Aroon, typeof(Aroon),
                                  "Aroon",
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
                                        }
                                    });
    }
  }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// The Aroon indicator is often used to determine whether a stock is trending or not and how stable the trend is.
    /// </summary>
    /// <remarks>Trends are determined by extreme values (above 80) of both lines (Aroon up and Aroon down), whereas unstable prices are determined when both lines are low (less than 20).
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Periods</term></item>
    /// </list>
    /// </remarks>
    public class Aroon : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public Aroon(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.Aroon;

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
                DisparaErroIndicador("Períodos inválidos para o indicador Aroon.\nAumente o período selecionado.", ErroIndicador.PeriodosInvalidos);
                return false;
            }

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pOpen = SeriesToField("Abertura", paramStr0 + ".Abertura", size);
            if (!EnsureField(pOpen, paramStr0 + ".Abertura")) return false;
            Field pHigh = SeriesToField("Maximo", paramStr0 + ".Maximo", size);
            if (!EnsureField(pHigh, paramStr0 + ".Maximo")) return false;
            Field pLow = SeriesToField("Minimo", paramStr0 + ".Minimo", size);
            if (!EnsureField(pLow, paramStr0 + ".Minimo")) return false;
            Field pClose = SeriesToField("Ultimo", paramStr0 + ".Ultimo", size);
            if (!EnsureField(pClose, paramStr0 + ".Ultimo")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pOpen);
            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.Aroon(pNav, pRS, paramInt1);


            // Output the indicator values
            Clear();

            IndicadorSerieFilha pAroonDown = (IndicadorSerieFilha)EnsureSeries(FullName + " Down");
            pAroonDown.SetStrokeThickness(GrossuraSerieFilha1, false);
            pAroonDown.SetStrokeColor(CorSerieFilha1, false);
            pAroonDown.SetStrokePattern(TipoLinhaSerieFilha1, false);
            pAroonDown.IsSerieFilha = true;
            pAroonDown.SeriePai = this;

            if (this.SeriesFilhas == null)
                this.SeriesFilhas = new List<Series>();

            this.SeriesFilhas.Add(pAroonDown);

            pAroonDown.Clear();
            _title = FullName + " Up";

            for (int n = 0; n < size; ++n)
            {
                double? top;
                double? bottom;
                if (n < paramInt1)
                {
                    top = null;
                    bottom = null;
                }
                else
                {
                    top = pInd.Value("Aroon Up", n + 1);
                    bottom = pInd.Value("Aroon Down", n + 1);
                }
                AppendValue(DM.GetTimeStampByIndex(n), top);
                pAroonDown.AppendValue(DM.GetTimeStampByIndex(n), bottom);
            }

            return _calculateResult = PostCalculate();
        }

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

    }
}

