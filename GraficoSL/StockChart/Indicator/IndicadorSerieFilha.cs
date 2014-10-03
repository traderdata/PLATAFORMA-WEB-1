using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.Indicators
{
    /// <summary>
    /// this type of indicator is used when an indicator has more than one lines
    /// </summary>
    internal class IndicadorSerieFilha : Indicator
    {
        internal Indicator _indicatorParent;

        public IndicadorSerieFilha(string name, ChartPanel chartPanel) 
            : base(name, chartPanel)
        {
            Init();

            _isTwin = true;
            IsSerieFilha = true;

            _indicatorType = EnumGeral.IndicatorType.Unknown;
        }

        private bool _strokeColorSet;
        internal void SetStrokeColor(Color color, bool forceSet)
        {
            if (_strokeColorSet && !forceSet) return;

            TitleBrush = new SolidColorBrush(color);
            _strokeColor = color;
            _strokeColorSet = true;
        }

        private bool _strokeThicknessSet;
        internal void SetStrokeThickness(double thickness, bool forceSet)
        {
            if (_strokeThicknessSet && !forceSet) return;
            _strokeThickness = thickness;
            _strokeThicknessSet = true;
        }

        private bool _strokePatternSet;
        internal void SetStrokePattern(EnumGeral.TipoLinha pattern, bool forceSet)
        {
            if (_strokePatternSet && !forceSet) return;
            _strokePattern = pattern;
            _strokePatternSet = true;
        }

        protected override bool TrueAction()
        {
            return true; //nothing fancy here, its values are calculated by its owner
        }
    }
}
