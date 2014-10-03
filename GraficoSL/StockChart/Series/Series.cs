using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.Data;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    ///<summary>
    /// Base class for all series types used in the chart
    ///</summary>
    [CLSCompliant(true)]
    public partial class Series : INotifyPropertyChanged
    {
        internal const string PropertyStrokeBrush = "StrokeBrush";
        internal const string PropertyLastValue = "LastValue";
        internal const string PropertyTitleBrush = "TitleBrush";

        private bool _painted;

        internal ChartPanel _chartPanel;
        internal Color? _upColor;
        internal Color? _downColor;
        internal Color _strokeColor;
        internal double _strokeThickness;
        internal EnumGeral.TipoLinha _strokePattern;
        internal double _opacity = 1.0;
        internal List<Series> _linkedSeries = new List<Series>();
        internal bool _recycleFlag;
        internal EnumGeral.TipoSeriesEnum _seriesType;
        internal bool _selectable;
        internal bool _shareScale;
        internal PaintObjectsManager<SelectionDot> _selectionDots = new PaintObjectsManager<SelectionDot>();

        internal string _title = "";
        internal EnumGeral.TipoSerieOHLC _seriesTypeOHLC = EnumGeral.TipoSerieOHLC.Desconhecido;
        internal string _name;
        internal bool _selected;
        internal bool _visible;

        private bool isSerieFilha = false;
        private Series seriePai = null;

        internal bool _visibleInfoPanel = true;
        internal bool _visibleTitlebar = true;
        internal string _titleBarCaption = "!";
        internal string _titleBarCaptionComplemento = "";
        internal string _sufixoInfoPanel = "";


        /// <summary>
        /// has minimum value from all visible records. Set in ChartPanel.SetMaxMin()
        /// </summary>
        internal double _min;
        /// <summary>
        /// has maximum value from all visible records. Set in ChartPanel.SetMaxMin()
        /// </summary>  
        internal double _max;

        /// <summary>
        /// Keeps the index of the series in DataManager
        /// </summary>
        internal int _seriesIndex;

        internal Series(string name, EnumGeral.TipoSeriesEnum seriesType, EnumGeral.TipoSerieOHLC seriesTypeOHLC, ChartPanel chartPanel)
        {
            _name = name;
            _seriesType = seriesType;
            _chartPanel = chartPanel;
            _seriesTypeOHLC = seriesTypeOHLC;

            ShowInHeatMap = false;
        }

        #region Public properties
        ///<summary>
        /// Series Name
        ///</summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Indica se uma série filha, ou seja, se está ligada a um indicador.
        /// </summary>
        public bool IsSerieFilha
        {
            get { return isSerieFilha; }
            set { isSerieFilha = value; }
        }

        /// <summary>
        /// Série pai desta série. Este campo será diferente de null quando esta série for uma série que depende de um indicador principal.
        /// </summary>
        public Series SeriePai
        {
            get { return seriePai; }
            set { seriePai = value; }
        }

        public double Variacao { get; set; }

        ///<summary>
        ///</summary>
        public bool ShareScale
        {
            get { return _shareScale; }
            set { _shareScale = value; }
        }

        /// <summary>
        /// Gets or sets whether the series will be used in heat map.
        /// By default only indicators are shown in heat map.
        /// </summary>
        public bool ShowInHeatMap { get; set; }


        /// <summary>
        /// Includes Name + OHLCV postfix
        /// </summary>
        public string FullName
        {
            get
            {
                if (_seriesType == EnumGeral.TipoSeriesEnum.Indicador) return Name;
                switch (_seriesTypeOHLC)
                {
                    case EnumGeral.TipoSerieOHLC.Abertura:
                        return Name + ".Abertura";
                    case EnumGeral.TipoSerieOHLC.Maximo:
                        return Name + ".Maximo";
                    case EnumGeral.TipoSerieOHLC.Minimo:
                        return Name + ".Minimo";
                    case EnumGeral.TipoSerieOHLC.Ultimo:
                        return Name + ".Ultimo";
                    case EnumGeral.TipoSerieOHLC.Volume:
                        return Name + ".Volume";
                    default:
                        return Name;
                }
            }
        }

        ///<summary>
        /// Custom title if present. Otherwise FullName
        ///</summary>
        public string Title
        {
            get { return _title.Length == 0 ? FullName : _title; }
        }

        ///<summary>
        /// OHLC type of the series
        ///</summary>
        public EnumGeral.TipoSerieOHLC OHLCType
        {
            get { return _seriesTypeOHLC; }
        }

        /// <summary>
        /// Series Type
        /// </summary>
        public EnumGeral.TipoSeriesEnum SeriesType
        {
            get { return _seriesType; }
            set { _seriesType = value; }
        }

        ///<summary>
        /// Sets the chart's up-tick bar color. When the close is higher than the previous close, this color will be used to paint the bar. 
        ///</summary>
        public Color? UpColor
        {
            get { return _upColor; }
            set
            {
                _upColor = value;
            }
        }

        ///<summary>
        /// Sets the chart's down-tick bar color. When the close is lower than the previous close, this color will be used to paint the bar. 
        ///</summary>
        public Color? DownColor
        {
            get { return _downColor; }
            set
            {
                _downColor = value;
            }
        }

        ///<summary>
        /// Line color
        ///</summary>
        public Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                if (_strokeColor == value) return;
                _strokeColor = value;
                SetStrokeColor();
                OnPropertyChanged(PropertyStrokeBrush);
            }
        }

        private Brush _titleBrush;
        ///<summary>
        /// Gets or sets the brush for series' text displayed in panel's title bar
        ///</summary>
        public Brush TitleBrush
        {
            get { return _titleBrush; }
            set
            {
                if (_titleBrush == value) return;
                _titleBrush = value;
                OnPropertyChanged(PropertyTitleBrush);
            }
        }

        ///<summary>
        /// Gets the line stroke brush
        ///</summary>
        public Brush StrokeColorBrush
        {
            get { return new SolidColorBrush(_strokeColor); }
        }

        ///<summary>
        /// Stroke thickness of lines used. It is used as FontSize for StaticText objects
        ///</summary>
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                if (_strokeThickness == value) return;
                _strokeThickness = value;
                SetStrokeThickness();
            }
        }

        ///<summary>
        /// Stroke pattern
        ///</summary>
        public EnumGeral.TipoLinha StrokePattern
        {
            get { return _strokePattern; }
            set
            {
                if (_strokePattern == value) return;
                _strokePattern = value;
            }
        }

        ///<summary>
        /// Is series selectable or not. If not, the user won't be able to select it with the mouse
        ///</summary>
        public bool Selectable
        {
            get { return _selectable; }
            set
            {
                if (_selectable == value) return;
                _selectable = value;
                if (!_selected || _selectable) return;
                _selected = false;
                ShowSelection();
            }
        }

        ///<summary>
        /// Hides or shows the series.
        ///</summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value) return;
                _visible = value;
                ShowHide();
            }
        }

        ///<summary>
        /// Minimum value that series has
        ///</summary>
        public double Min
        {
            get { return DM.Min(_seriesIndex); }
        }

        ///<summary>
        /// Maximum value that series has
        ///</summary>
        public double Max
        {
            get { return DM.Max(_seriesIndex); }
        }

        ///<summary>
        /// Series index. All series internally have an index, that is used to access their value in internal DataManager object
        ///</summary>
        public int SeriesIndex
        {
            get { return _seriesIndex; }
        }

        ///<summary>
        /// Is series selected by the user with mouse or not.
        ///</summary>
        public bool Selected
        {
            get { return _selected; }
        }

        /// <summary>
        /// Returns a series value by index
        /// </summary>
        /// <param name="recordIndex"></param>
        /// <returns></returns>
        public DataEntry this[int recordIndex]
        {
            get 
            {
                if (recordIndex >= 0)
                {
                    if (recordIndex < DM[_seriesIndex].Data.Count)
                        return DM[_seriesIndex].Data[recordIndex];
                    else
                        return null;
                }
                else
                    return DM[_seriesIndex].Data[0]; 
            }
        }
        #endregion

        #region Virtual methods

        internal virtual void Paint()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeColor()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeThickness()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeType()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetOpacity()
        {
            throw new NotImplementedException();
        }
        internal virtual void RemovePaint()
        {
            throw new NotImplementedException();
        }
        internal virtual void ShowHide()
        {
            throw new NotImplementedException();
        }

        internal virtual void Init()
        {
            _strokeThickness = 1;
            _strokePattern = EnumGeral.TipoLinha.Solido;
            _strokeColor = Color.FromArgb(0xFF, 0x00, 0xFF, 0x00); //Lime

            _selectable = true;
            _selected = false;
            _visible = true;
            _painted = false;
            _upColor = null;
            _downColor = null;
            _shareScale = true;
        }
        #endregion

        private EnumGeral.PosicaoTickBox _showTickBox;
        private SeriesTickBox _seriesTickBoxLeft;
        private SeriesTickBox _seriesTickBoxRight;
        ///<summary>
        /// Places a tick box on one of the Y axes
        ///</summary>
        public EnumGeral.PosicaoTickBox TickBox
        {
            get { return _showTickBox; }
            set
            {
                //        if (_showTickBox == value) return;
                _showTickBox = value;
                if (_seriesTickBoxLeft == null) 
                {
                    _seriesTickBoxLeft = new SeriesTickBox(this);
                    _chartPanel._leftYAxis.Children.Add(_seriesTickBoxLeft);
                    _seriesTickBoxRight = new SeriesTickBox(this);
                    _chartPanel._rightYAxis.Children.Add(_seriesTickBoxRight);

                    _seriesTickBoxLeft.Background = StrokeColorBrush;
                    _seriesTickBoxRight.Background = StrokeColorBrush;
                }
                switch (_showTickBox)
                {
                    case EnumGeral.PosicaoTickBox.Esquerda:
                        _seriesTickBoxLeft.Visibility = Visibility.Visible;
                        _seriesTickBoxRight.Visibility = Visibility.Collapsed;
                        _seriesTickBoxLeft.Show();
                        break;
                    case EnumGeral.PosicaoTickBox.Direita:
                        _seriesTickBoxLeft.Visibility = Visibility.Collapsed;
                        _seriesTickBoxRight.Visibility = Visibility.Visible;
                        _seriesTickBoxRight.Show();
                        break;
                    default:
                        _seriesTickBoxLeft.Visibility = Visibility.Collapsed;
                        _seriesTickBoxRight.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        /// <summary>
        /// Unscales a value and restores between max and min
        /// </summary>
        /// <param name="value">Value to be unnormalized</param>
        /// <returns>UnNormalized value</returns>
        public double UnNormalize(double value)
        {
            if (_chartPanel._chartX.EscalaTipo == EnumGeral.TipoEscala.Semilog && Min > 0 && _chartPanel._hasPrice)
                return Math.Log10(_min) + (value * (Math.Log10(_max) - Math.Log10(_min)));
            return Min + (value * (Max - Min));
        }

        /// <summary>
        /// Normalizes a value between 1 and 0
        /// </summary>
        /// <param name="value">Value to be normalized</param>
        /// <returns>Normalized value</returns>
        public double Normalize(double value)
        {
            if (_chartPanel._chartX.EscalaTipo == EnumGeral.TipoEscala.Semilog && _min > 0 && _chartPanel._hasPrice)
                return (value - Math.Log10(_min)) / (Math.Log10(_max) - Math.Log10(_min));
            return (value - _min) / (_max - _min);
        }

        /// <summary>
        /// Gets the Y pixel by price value
        /// </summary>
        /// <param name="seriesValue">Prive value</param>
        /// <returns>Y pixel</returns>
        public double GetY(double seriesValue)
        {
            if (_shareScale) return _chartPanel.GetY(seriesValue);

            double? panelHeight = _chartPanel.PaintableHeight;
            if (!panelHeight.HasValue) return 0.0;

            if (_chartPanel._chartX.EscalaTipo == EnumGeral.TipoEscala.Semilog && _min > 0 && _chartPanel._hasPrice)
                return (double)((panelHeight - (panelHeight * Normalize(Math.Log10(seriesValue)))) + _chartPanel._yOffset);

            return (double)((panelHeight - (panelHeight * Normalize(seriesValue))) + _chartPanel._yOffset);
        }

        /// <summary>
        /// Returns series value by Y pixel
        /// </summary>
        /// <param name="pixelValue">Pixel value</param>
        /// <returns>Price value</returns>
        public double GetReverseY(double pixelValue)
        {
            if (_shareScale) return _chartPanel.GetReverseY(pixelValue);

            double? realHeight = _chartPanel.PaintableHeight;
            if (!realHeight.HasValue) return 0.0;

            if (_chartPanel._chartX.EscalaTipo == EnumGeral.TipoEscala.Semilog && Min > 0 && _chartPanel._hasPrice)
            {
                pixelValue = UnNormalize(1 - ((pixelValue - _chartPanel._yOffset) / realHeight.Value));
                if (pixelValue > 0 && Max > 0)
                    return Math.Pow(10, pixelValue);
            }
            return UnNormalize(1 - (pixelValue - _chartPanel._yOffset) / realHeight.Value);
        }

        public void SetVisibleTitleBar(bool visible)
        {
            this._visibleTitlebar = visible;
        }

        public void SetTitleBarCaption(string caption)
        {
            this._titleBarCaption = caption;
        }

        public void SetTitleBarCaptionComplemento(string captionComplemento)
        {
            this._titleBarCaptionComplemento = captionComplemento;
        }

        public void SetVisibleInfoPanel(bool visible)
        {
            this._visibleInfoPanel = visible;
        }

        public void SetSufixoInfoPanel(string sufixo)
        {
            this._sufixoInfoPanel = sufixo;
        }

        public string GetSufixoInfoPanel()
        {
            return this._sufixoInfoPanel;
        }

        internal bool Painted
        {
            get { return _painted; }
            set { _painted = value; }
        }

        internal DataManager.DataManager DM
        {
            get { return _chartPanel._chartX.dataManager; }
        }

        internal int RecordCount
        {
            get { return DM.RecordCount; }
        }

        internal DataManager.DataManager.SeriesEntry SeriesEntry
        {
            get { return DM[_seriesIndex]; }
        }

        internal double MaxFromInterval(ref int startIndex, ref int endIndex)
        {
            return DM.MaxFromInterval(_seriesIndex, ref startIndex, ref endIndex);
        }

        internal double MinFromInterval(ref int startIndex, ref int endIndex)
        {
            return DM.MinFromInterval(_seriesIndex, ref startIndex, ref endIndex);
        }

        internal void ShowSelection()
        {
            if (!_selectable) return;
            _selected = true;

            double dx = 0;
            DataEntryCollection data = _chartPanel._chartX.dataManager[_seriesIndex].Data;

            _selectionDots.C = _chartPanel._rootCanvas;
            _selectionDots.Start();
            for (int i = _chartPanel._chartX.indexInicial; i < _chartPanel._chartX.indexFinal; i++)
            {
                if (!data[i].Value.HasValue) continue;
                double x = _chartPanel._chartX.GetXPixel(i - _chartPanel._chartX.indexInicial);
                if (x - dx <= 50) continue;
                dx = x;
                SelectionDot dot = _selectionDots.GetPaintObject(Types.Corner.MoveAll);
                dot.SetPos(new Point(dx, GetY(data[i].Value.Value)));
                dot.Tag = this;
            }
            _selectionDots.Stop();
            _selectionDots.Do(dot => dot.ZIndex = ZIndexConstants.SelectionPoint1);
        }

        internal void HideSelection()
        {
            _selected = false;
            _selectionDots.RemoveAll();
        }

        internal virtual void MoveToPanel(ChartPanel chartPanel)
        {
            //each type of series has its own implementation
            //they will call base at the end to move linked series
            foreach (Series series in _linkedSeries)
            {
                series.MoveToPanel(chartPanel);
            }
        }

        /// <summary>
        /// Occurs when an internal property changes. For internal usage only
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == PropertyLastValue)
                CheckTrendLinesPenetration();
        }

        internal void CheckTrendLinesPenetration()
        {
            //go over each trendline present in current panel and see if it
            //penetrates the current series
            int recordCount = RecordCount;
            if (recordCount < 2) return; //works for record > 2

            double? value1 = this[recordCount - 1].Value;
            double? value2 = this[recordCount - 2].Value;
            if (!value1.HasValue || !value2.HasValue) return; //need both values
            foreach (TrendLine trendLine in _chartPanel.WatchableTrendLines)
            {
                double x1 = trendLine.X1Value;
                double y1 = trendLine.Y1Value;
                double x2 = trendLine.X2Value;
                double y2 = trendLine.Y2Value;

                if (x2 == x1) continue;

                double incr = (y2 - y1) / (x2 - x1);

                double pointB = trendLine.Y2Value;
                double pointA = trendLine.Y2Value - incr;

                if (value1.Value > pointB && value2.Value < pointA)
                    _chartPanel._chartX.FireTrendLinePenetration(trendLine, StockChartX.TrendLinePenetrationEnum.Above, this);
                else if (value1.Value < pointB && value2.Value > pointA)
                    _chartPanel._chartX.FireTrendLinePenetration(trendLine, StockChartX.TrendLinePenetrationEnum.Below, this);
            }
        }

        /// <summary>
        /// Returns a string that represents this <see cref="Series"/> object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Name: {0}, SeriesType: {1}, SeriesTypeOhlc: {2}", FullName, _seriesType, _seriesTypeOHLC);
        }
    }
}

