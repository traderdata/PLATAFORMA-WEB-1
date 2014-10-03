using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;
using System.Windows.Media;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_KeltnerIndex()
        {
            /*  Required inputs for this indicator:
              Nenhum
            */
            RegisterIndicatorParameters(EnumGeral.IndicatorType.Keltner, typeof(KeltnerIndex),
                                        "Keltner",
                                        new List<IndicatorParameter>
                                    {                                      
                                      new IndicatorParameter
                                        {
                                          //Symbol (eg "msft")
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Ativo),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Ativo,
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
    /// Keltner Index 
    /// </summary>
    /// <remarks>
    /// Typical Price = (high + low + last_trade) / 3 
    /// HL = high - low 
    /// Center Line = SMA (Typical Price, period)
    /// UPPER Keltner Band = Center Line + SMA (HL,period)
    /// LOWER Keltner Band = Center Line - SMA (HL,period)

    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// </list>
    /// </remarks>
    public class KeltnerIndex : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public KeltnerIndex(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.Keltner;

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

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pOpen = SeriesToField("Abertura", paramStr0 + ".Abertura", iSize);
            if (!EnsureField(pOpen, paramStr0 + ".Abertura")) return false;
            Field pHigh = SeriesToField("Maximo", paramStr0 + ".Maximo", iSize);
            if (!EnsureField(pHigh, paramStr0 + ".Maximo")) return false;
            Field pLow = SeriesToField("Minimo", paramStr0 + ".Minimo", iSize);
            if (!EnsureField(pLow, paramStr0 + ".Minimo")) return false;
            Field pClose = SeriesToField("Ultimo", paramStr0 + ".Ultimo", iSize);
            if (!EnsureField(pClose, paramStr0 + ".Ultimo")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pOpen);
            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            General ta = new General();
            Recordset pTypicalPrice = ta.TypicalPrice(pNav, pRS, "TypicalPrice");
            Recordset pHiMinusLow = ta.HighMinusLow(pNav, pRS, "HighMinusLow");

            //Pegando o ultimo cara da serie
            //double typicalPrice = (double)pTypicalPrice.Value(FullName, iSize);
            //double highMinusLow = (double)pHiMinusLow.Value(FullName, iSize);

            //Calculando as SMAS
            MovingAverage ma = new MovingAverage();
            List<double> dTypicialPrice = new List<double>();
            for (int n = 0; n < iSize; ++n)
            {
                dTypicialPrice.Add((double)pTypicalPrice.Value("TypicalPrice", n + 1));
                    //AppendValue(DM.TS(n), pTypicalPrice.Value(FullName, n + 1));
            }


            Recordset pCenterLine = ma.SimpleMovingAverage(pNav, pTypicalPrice.GetField("TypicalPrice"), 10, "CenterLine");
            Recordset pSMAHighLow = ma.SimpleMovingAverage(pNav, pHiMinusLow.GetField("HighMinusLow"), 10, "HighMinusLow");

            // Output the indicator values
            Clear();
            
            //Como indicador tem mais de uma linha preciso usar o twinindicator
            IndicadorSerieFilha sUpper = (IndicadorSerieFilha)EnsureSeries("Upper");
            sUpper.SetStrokeColor(CorSerieFilha1, false);
            sUpper.SetStrokeThickness(GrossuraSerieFilha1, false);
            sUpper.SetStrokePattern(TipoLinhaSerieFilha1, false);
            sUpper.ForceLinearChart = true;
            sUpper.IsSerieFilha = true;
            sUpper.SeriePai = this;

            IndicadorSerieFilha sLower = (IndicadorSerieFilha)EnsureSeries("Lower");
            sLower.SetStrokeColor(CorSerieFilha2, false);
            sLower.SetStrokeThickness(GrossuraSerieFilha2, false);
            sLower.SetStrokePattern(TipoLinhaSerieFilha2, false);
            sLower.ForceLinearChart = true;
            sLower.IsSerieFilha = true;
            sLower.SeriePai = this;

            if (this.SeriesFilhas == null)
                this.SeriesFilhas = new List<Series>();

            this.SeriesFilhas.Add(sUpper);
            this.SeriesFilhas.Add(sLower);

            for (int n = 0; n < iSize; ++n)
            {
                double? dUpper = pCenterLine.Value("CenterLine", n + 1) + pSMAHighLow.Value("HighMinusLow", n+1);
                double? dDown = pCenterLine.Value("CenterLine", n + 1) - pSMAHighLow.Value("HighMinusLow", n + 1);
                
                //Plotando a centerline
                AppendValue(DM.TS(n), pCenterLine.Value("CenterLine", n + 1));
                
                //Plotando a upperLine
                sUpper.AppendValue(DM.TS(n), dUpper);

                //Plotando a lower line
                sLower.AppendValue(DM.TS(n), dDown);
            }


            return _calculateResult = PostCalculate();
        }
    }
}
