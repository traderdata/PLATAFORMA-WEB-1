using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;
using System.Windows.Media;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_DIDIIndex()
        {
            /*  Required inputs for this indicator:
              Nenhum
            */
            RegisterIndicatorParameters(EnumGeral.IndicatorType.Agulhada, typeof(Agulhada),
                                        "Identificador de Agulhada",
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
                                    });
        }
    }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// DIDI Index é um indicador feito pelo DIDI Aguiar
    /// </summary>
    /// <remarks>Identifica a MM8 como sendo o zero e faz as medias de 3 e 20 em função da média 8
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// </list>
    /// </remarks>
    public class Agulhada : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public Agulhada(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.Agulhada;

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
            if (_params.Count < Parameters.Count)
                return false;


            // Get the data
            Field pSource = SeriesToField("Source", ParamStr(0), size);
            if (!EnsureField(pSource, ParamStr(0)))
                return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pNav.Recordset_ = pRS;

            // Calculate the indicator
            MovingAverage ta = new MovingAverage();
            Recordset pInd3 = ta.SimpleMovingAverage(pNav, pSource, 3, "MM3");
            Recordset pInd8 = ta.SimpleMovingAverage(pNav, pSource, 8, "MM8");
            Recordset pInd20 = ta.SimpleMovingAverage(pNav, pSource, 20, "MM20");

            //forçando o indicador como line chart
            this.ForceLinearChart = true;

            // Output the indicator values
            Clear();

            //Como indicador tem mais de uma linha preciso usar o twinindicator
            IndicadorSerieFilha sMM3 = (IndicadorSerieFilha)EnsureSeries(FullName + " MM3");
            sMM3.SetStrokeColor(CorSerieFilha1, false);
            sMM3.SetStrokeThickness(GrossuraSerieFilha1, false);
            sMM3.SetStrokePattern(TipoLinhaSerieFilha1, false);
            sMM3.ForceLinearChart = true;
            sMM3.IsSerieFilha = true;
            sMM3.SeriePai = this;

            IndicadorSerieFilha sMM20 = (IndicadorSerieFilha)EnsureSeries(FullName + " MM20");
            sMM20.SetStrokeColor(CorSerieFilha2, false);
            sMM20.SetStrokeThickness(GrossuraSerieFilha2, false);
            sMM3.SetStrokePattern(TipoLinhaSerieFilha2, false);
            sMM20.ForceLinearChart = true;
            sMM20.IsSerieFilha = true;
            sMM20.SeriePai = this;

            if (this.SeriesFilhas == null)
                this.SeriesFilhas = new List<Series>();

            this.SeriesFilhas.Add(sMM3);
            this.SeriesFilhas.Add(sMM20);


            for (int n = 0; n < size; n++)
            {
                double? dValue3 = n <= 3 ? null : pInd3.Value("MM3", n + 1);
                double? dValue8 = n <= 8 ? null : pInd8.Value("MM8", n + 1);
                double? dValue20 = n <= 20 ? null : pInd20.Value("MM20", n + 1);

                //Alterando valores
                dValue3 = dValue3 - dValue8;
                dValue20 = dValue20 - dValue8;
                dValue8 = 0;

                AppendValue(DM.TS(n), dValue8);
                sMM3.AppendValue(DM.TS(n), dValue3);
                sMM20.AppendValue(DM.TS(n), dValue20);
            }

            return _calculateResult = PostCalculate();
        }
    }
}
