﻿using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.Tasdk;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_IndicatorsParameters
  {
    internal static void Register_ComparativeRelativeStrength()
    {
      /*  Required inputs for this indicator:
        1. paramStr0 = Source1 (eg "msft.Ultimo")
        2. paramStr1 = Source2 (eg "aapl.volume")
      */
        RegisterIndicatorParameters(EnumGeral.IndicatorType.IndiceForcaRelativaComparada, typeof(ComparativeRelativeStrength),
                                  "Comparative Relative Strength",
                                  new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Source1 (eg "msft.Ultimo")
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Serie1),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Serie1,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Source2 (eg "aapl.volume")
                                          Name = Indicator.GetParamName(EnumGeral.TipoParametroIndicador.Serie2),
                                          ParameterType = EnumGeral.TipoParametroIndicador.Serie2,
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
    /// The Comparative Relative Strength index divides one price field by another price field.
    /// </summary>
    /// <remarks>The base security is outperforming the other security when the Comparative RSI is trending upwards.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source 1</term></item>
    /// <item><term>str Source 2</term></item>
    /// </list>
    /// </remarks>
    public class ComparativeRelativeStrength : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public ComparativeRelativeStrength(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = EnumGeral.IndicatorType.IndiceForcaRelativaComparada;

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

            string paramStr0 = ParamStr(0);
            string paramStr1 = ParamStr(1);
            if (paramStr0 == paramStr1)
            {
                DisparaErroIndicador("Série 1 não pode ser igual a série 2.", ErroIndicador.Serie1IgualSerie2);
                return false;
            }


            // Get the data
            Field pSource1 = SeriesToField("Source1", paramStr0, size);
            if (!EnsureField(pSource1, paramStr0)) return false;

            Field pSource2 = SeriesToField("Source2", paramStr1, size);
            if (!EnsureField(pSource2, paramStr1)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource1);
            pRS.AddField(pSource2);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.ComparativeRelativeStrength(pNav, pSource1, pSource2, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < size; ++n)
            {
                AppendValue(DM.GetTimeStampByIndex(n), pInd.Value(FullName, n + 1));
            }

            return _calculateResult = PostCalculate();
        }
    }
}
