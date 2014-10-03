using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public partial class ChartPanel
    {
        #region Public Methods
        /// <summary>
        /// Normalizes a value between 1 and 0
        /// </summary>
        /// <param name="value">Value to be normalized</param>
        /// <returns>Normalized value</returns>
        public double Normalize(double value)
        {
            if (_chartX.escalaTipo == EnumGeral.TipoEscala.Semilog && _minChanged > 0 && _hasPrice)
                return (value - SLMin) / (SLMax - SLMin);
            return (value - _minChanged) / (_maxChanged - _minChanged);
        }

        /// <summary>
        /// Unscales a value and restores between max and min
        /// </summary>
        /// <param name="value">Value to be unnormalized</param>
        /// <returns>UnNormalized value</returns>
        public double UnNormalize(double value)
        {
            if (_chartX.escalaTipo == EnumGeral.TipoEscala.Semilog && _minChanged > 0 && _hasPrice)
                return SLMin + (value * (SLMax - SLMin));
            return _minChanged + (value * (_maxChanged - _minChanged));
        }

        /// <summary>
        /// Returns Y pixel value by value from a series
        /// </summary>
        /// <param name="seriesValue">Price value</param>
        /// <returns>Y pixel</returns>
        public double GetY(double seriesValue)
        {
            double? realHeight = PaintableHeight;
            if (!realHeight.HasValue) return 0.0;

            if (_chartX.escalaTipo == EnumGeral.TipoEscala.Semilog && _minChanged > 0 && _hasPrice)
                return (double)((realHeight - (realHeight * Normalize(Math.Log10(seriesValue)))) + _yOffset);

            return (double)((realHeight - (realHeight * Normalize(seriesValue))) + _yOffset);
        }

        /// <summary>
        /// Returns Y pixel value by value from a series.
        /// Verifica a escala pelo argumento passado.
        /// </summary>
        /// <param name="seriesValue">Price value</param>
        /// <returns>Y pixel</returns>
        public double GetY(double seriesValue, Traderdata.Client.Componente.GraficoSL.Enum.EnumGeral.TipoEscala tipoEscala)
        {
            double? realHeight = PaintableHeight;
            if (!realHeight.HasValue) return 0.0;

            if (_chartX.escalaTipo == tipoEscala && _minChanged > 0 && _hasPrice)
                return (double)((realHeight - (realHeight * Normalize(Math.Log10(seriesValue)))) + _yOffset);

            return (double)((realHeight - (realHeight * Normalize(seriesValue))) + _yOffset);
        }

        /// <summary>
        /// Gets the paitable height of the panel or null if template was not loaded yet.
        /// </summary>
        public double? PaintableHeight
        {
            get { return !_templateLoaded ? (double?)null : _rootCanvas.ActualHeight; }
        }

        /// <summary>
        /// Returns series value by Y pixel
        /// </summary>
        /// <param name="pixelValue">Pixel value</param>
        /// <returns>Price value</returns>
        public double GetReverseY(double pixelValue)
        {
            double realHeight = _rootCanvas.ActualHeight;

            if (_chartX.EscalaTipo == EnumGeral.TipoEscala.Semilog && Min > 0 && _hasPrice)
            {
                pixelValue = UnNormalize(1 - (pixelValue - _yOffset) / realHeight);
                //if (pixelValue > 0 && Max > 0)
                    return Math.Pow(10, pixelValue);
            }
            
            return UnNormalize(1 - (pixelValue - _yOffset) / realHeight);
        }

        /// <summary>
        /// Returns OHLC group of series from current panel
        /// Returns false if the group does not exist
        /// </summary>
        /// <param name="open">Reference to open series</param>
        /// <param name="high">Reference to high series</param>
        /// <param name="low">Reference to low series</param>
        /// <param name="close">Reference to close series</param>
        /// <returns>true - if OHLC group exists
        /// false - if OHLC doesn't exists</returns>
        public bool GetOHLCSeries(out Series open, out Series high,
          out Series low, out Series close)
        {
            open = high = low = close = null;
            if (_series.Count < 4) return false;
            foreach (Series series in _series)
            {
                if (series.OHLCType != EnumGeral.TipoSerieOHLC.Abertura) continue;
                open = series;
                break;
            }
            if (open == null) return false;
            high = GetSeriesOHLCV(open, EnumGeral.TipoSerieOHLC.Maximo);
            if (high == null) return false;
            low = GetSeriesOHLCV(open, EnumGeral.TipoSerieOHLC.Minimo);
            if (low == null) return false;
            close = GetSeriesOHLCV(open, EnumGeral.TipoSerieOHLC.Ultimo);
            return close != null;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the title of the panel. The title is composed from names all all series &amp; indicators
        /// located on current panel
        /// </summary>
        public string Title
        {
            get
            {
                StringBuilder sb = new StringBuilder(128);
                foreach (SeriesTitleLabel label in _seriesTitle)
                {
                    sb.Append(">").Append(label.Title).Append(" ");
                }
                
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the panel
        /// </summary>
        public bool Visible
        {
            get { return Visibility == Visibility.Visible; }
            internal set
            {
                if (value == Visible || _state == StateType.Minimized) return;
                Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the height of the panel's title bar
        /// </summary>
        public double TitleBarHeight
        {
            get { return _titleBar.ActualHeight; }
        }

        /// <summary>
        /// Gets a collection of all indicators from current panel
        /// </summary>
        public IEnumerable<Indicator> IndicatorsCollection
        {
            get
            {
                foreach (Series series in _series)
                {
                    if (series._seriesType != EnumGeral.TipoSeriesEnum.Indicador || series._recycleFlag) continue;
                    yield return (Indicator)series;
                }
            }
        }

        /// <summary>
        /// Gets a collection of all series from panel
        /// </summary>
        public IEnumerable<Series> SeriesCollection
        {
            get
            {
                foreach (Series series in _series)
                {
                    if (series._seriesType == EnumGeral.TipoSeriesEnum.Indicador ||
                      series._seriesType == EnumGeral.TipoSeriesEnum.Desconhecida) continue;
                    yield return series;
                }
            }
        }

        /// <summary>
        /// Gets index of the panel
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets minimum value from all series located in current panel
        /// </summary>
        public double Min
        {
            get { return _minChanged; }
        }

        /// <summary>
        /// Gets maximum value from all series located in current panel
        /// </summary>
        public double Max
        {
            get { return _maxChanged; }
        }

        /// <summary>
        /// Gets minimum-logarithmic value from all series located in current panel
        /// </summary>
        public double SLMin
        {
            get { return Math.Log10(_minChanged); }
        }

        /// <summary>
        /// Gets maximum-logarithmic value from all series located in current panel
        /// </summary>
        public double SLMax
        {
            get { return Math.Log10(_maxChanged); }
        }

        /// <summary>
        /// Gets if panel has staic Y scale. 
        /// static Y scale set to true means that min &amp; max values from all series
        /// will be ignored, instead users' min &amp; max values will be used to paint series.
        /// </summary>
        public bool StaticYScale
        {
            get { return _staticYScale; }
        }

        /// <summary>
        /// Gets formating string that will be used to format values from Y scale
        /// </summary>
        public string FormatYValueString
        {
            get
            {
                int decimals = _hasVolume ? 0 : _chartX.EscalaPrecisao;
                return "{0:f" + decimals + "}";
            }
        }

        /// <summary>
        /// Gets if current panel has an OHLC group of series
        /// </summary>
        public bool HasOHLC
        {
            get
            {
                if (_series.Count < 4) return false;

                Series series = null;
                foreach (Series series1 in _series)
                {
                    if (series1.OHLCType != EnumGeral.TipoSerieOHLC.Ultimo) continue;
                    series = series1;
                    break;
                }
                if (series == null) return false;

                series = GetSeriesOHLCV(series, EnumGeral.TipoSerieOHLC.Abertura);
                if (series == null) return false;
                series = GetSeriesOHLCV(series, EnumGeral.TipoSerieOHLC.Maximo);
                if (series == null) return false;
                series = GetSeriesOHLCV(series, EnumGeral.TipoSerieOHLC.Minimo);
                if (series == null) return false;
                series = GetSeriesOHLCV(series, EnumGeral.TipoSerieOHLC.Ultimo);
                return series != null;
            }
        }

        /// <summary>
        /// Gets true if current panel is a heat-map
        /// </summary>
        public bool IsHeatMap
        {
            get { return _isHeatMap; }
        }

        ///<summary>
        /// Gets the collection of LineStudies from current panel
        ///</summary>
        public IEnumerable<LineStudy> LineStudiesCollection
        {
            get
            {
                foreach (LineStudy lineStudy in _lineStudies)
                {
                    yield return lineStudy;
                }
            }
        }

        /// <summary>
        /// Gets or sets the background of Y axis
        /// </summary>
        public static DependencyProperty YAxesBackgroundProperty;
        /// <summary>
        /// Gets or sets the background of Y axis
        /// </summary>
        public Brush YAxesBackground
        {
            get { return (Brush)GetValue(YAxesBackgroundProperty); }
            set { SetValue(YAxesBackgroundProperty, value); }
        }

        private static void OnYAxesBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ChartPanel chartPanel = (ChartPanel)sender;
            if (chartPanel == null)
                return;
            if (chartPanel._leftYAxis != null)
                chartPanel._leftYAxis.Background = (Brush)eventArgs.NewValue;
            if (chartPanel._rightYAxis != null)
                chartPanel._rightYAxis.Background = (Brush)eventArgs.NewValue;
        }
        #endregion
    }
}

