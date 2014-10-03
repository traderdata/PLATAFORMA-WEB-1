using System;
using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    /// <summary>
    /// Keeps information about indicators' parameters
    /// </summary>
    public static partial class StockChartX_IndicatorsParameters
    {
        /// <summary>
        /// Information about an indicator's parameter
        /// </summary>
        public class IndicatorParameter
        {
            ///<summary>
            /// Gets the indicator parameter name
            ///</summary>
            public string Name { get; internal set; }
            ///<summary>
            /// Gets the parameter type
            ///</summary>
            public EnumGeral.TipoParametroIndicador ParameterType { get; internal set; }
            /// <summary>
            /// Default value
            /// </summary>
            public object DefaultValue { get; internal set; }
            /// <summary>
            /// CLR value type
            /// </summary>
            public Type ValueType { get; internal set; }

            /// <summary>
            /// Ovveride
            /// </summary>
            /// <returns>Name</returns>
            public override string ToString()
            {
                return Name;
            }
        }

        /// <summary>
        /// Collection of indicator parameter
        /// </summary>
        public class IndicatorParameters
        {
            /// <summary>
            /// Gets indicator real name
            /// </summary>
            public string IndicatorRealName { get; internal set; }
            /// <summary>
            /// Indicator type
            /// </summary>
            public EnumGeral.IndicatorType IndicatorType { get; internal set; }
            /// <summary>
            /// Indicator's parameters
            /// </summary>
            public List<IndicatorParameter> Parameters { get; internal set; }
            /// <summary>
            /// CLR indicator type
            /// </summary>
            public Type CLRIndicatorType { get; internal set; }

            /// <summary>
            /// Ovveride
            /// </summary>
            /// <returns>Real Name</returns>
            public override string ToString()
            {
                return IndicatorRealName;
            }
        }

        private static readonly Dictionary<EnumGeral.IndicatorType, IndicatorParameters> _indicatorsParameters =
          new Dictionary<EnumGeral.IndicatorType, IndicatorParameters>();

        private static bool _bRegistered;
        private static void Register_All()
        {
            if (_bRegistered) return;
            _bRegistered = true;
            Register_SimpleMovingAverage();
            Register_BollingerBands();
            Register_AccumulativeSwingIndex();
            Register_Aroon();
            Register_AroonOscillator();
            Register_ChaikinMoneyFlow();
            Register_ChaikinVolatility();
            Register_ChandeMomentumOscillator();
            Register_CommodityChannelIndex();
            Register_ComparativeRelativeStrength();
            Register_DetrendedPriceOscillator();
            Register_DirectionalMovementSystem();
            Register_EaseOfMovement();
            Register_ExponentialMovingAverage();
            Register_FractalChaosBands();
            Register_FractalChaosOscillator();
            Register_HighLowBands();
            Register_HighMinusLow();
            Register_HistoricalVolatility();
            Register_LinearRegressionForecast();
            Register_LinearRegressionIntercept();
            Register_LinearRegressionRSquared();
            Register_LinearRegressionSlope();
            Register_MACD();
            Register_MACDHistogram();
            Register_MassIndex();
            Register_Median();
            Register_MomentumOscillator();
            Register_MoneyFlowIndex();
            Register_MovingAverageEnvelope();
            Register_NegativeVolumeIndex();
            Register_OnBalanceVolume();
            Register_ParabolicSAR();
            Register_PerformanceIndex();
            Register_PositiveVolumeIndex();
            Register_PriceOscillator();
            Register_PriceROC();
            Register_PriceVolumeTrend();
            Register_PrimeNumberBands();
            Register_PrimeNumberOscillator();
            Register_RainbowOscillator();
            Register_RelativeStrengthIndex();
            Register_StandardDeviation();
            Register_StochasticMomentumIndex();
            Register_StochasticOscillator();
            Register_SwingIndex();
            Register_TimeSeriesMovingAverage();
            Register_TradeVolumeIndex();
            Register_TriangularMovingAverage();
            Register_TRIX();
            Register_TrueRange();
            Register_TypicalPrice();
            Register_UltimateOscillator();
            Register_VariableMovingAverage();
            Register_VerticalHorizontalFilter();
            Register_VIDYA();
            Register_VolumeOscillator();
            Register_VolumeROC();
            Register_WeightedClose();
            Register_WeightedMovingAverage();
            Register_WellesWilderSmoothing();
            Register_WilliamsAccumulationDistribution();
            Register_WilliamsPctR();
            Register_CustomIndicator();
            Register_DIDIIndex();
            Register_KeltnerIndex();
        }

        /// <summary>
        /// Returns information about all indicators and their parameters
        /// </summary>
        public static IEnumerable<IndicatorParameters> Indicators
        {
            get
            {
                Register_All();
                foreach (var pair in _indicatorsParameters)
                {
                    yield return pair.Value;
                }
            }
        }

        internal static void RegisterIndicatorParameters(EnumGeral.IndicatorType indicatorType, Type clrIndicatorType, string indicatorRealNamme, List<IndicatorParameter> parameters)
        {
            if (_indicatorsParameters.ContainsKey(indicatorType))
                throw new Exception("Indicator " + indicatorType + " already registered.");

            _indicatorsParameters[indicatorType] = new IndicatorParameters
                                                     {
                                                         IndicatorType = indicatorType,
                                                         IndicatorRealName = indicatorRealNamme,
                                                         Parameters = parameters,
                                                         CLRIndicatorType = clrIndicatorType,
                                                     };
        }

        /// <summary>
        /// Returns a List of of indicator parameters for a given inidcator type
        /// </summary>
        /// <param name="indicatorType">Indicator type</param>
        /// <returns>Indicator Parameters</returns>
        public static List<IndicatorParameter> GetIndicatorParameters(EnumGeral.IndicatorType indicatorType)
        {
            Register_All();
            if (!_indicatorsParameters.ContainsKey(indicatorType))
                throw new Exception("Indicator do tipo " + indicatorType + " não registrado.");

            return _indicatorsParameters[indicatorType].Parameters;
        }

        /// <summary>
        /// Returns indicators CLR type by internal IndicatorType
        /// </summary>
        /// <param name="indicatorType">Indicator type</param>
        /// <returns>CLR Type</returns>
        public static Type GetIndicatorCLRType(EnumGeral.IndicatorType indicatorType)
        {
            Register_All();
            if (!_indicatorsParameters.ContainsKey(indicatorType))
                throw new Exception("Indicator do tipo " + indicatorType + " não registrado.");
            return _indicatorsParameters[indicatorType].CLRIndicatorType;
        }

        /// <summary>
        /// Returns the default indicator name by its type
        /// </summary>
        /// <param name="indicatorType">Indicator type</param>
        /// <returns>Indicator Name</returns>
        public static string GetIndicatorName(EnumGeral.IndicatorType indicatorType)
        {
            Register_All();
            if (!_indicatorsParameters.ContainsKey(indicatorType))
                throw new Exception("Indicator do tipo " + indicatorType + " não registrado.");
            return _indicatorsParameters[indicatorType].IndicatorRealName;
        }
    }

}