using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;
using FrameworkElement=System.Windows.FrameworkElement;
using Line=Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Line;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
#endif

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    /// <summary>
    /// ChartPanel - container for all series and line studies.
    /// </summary>
    [CLSCompliant(true)]
    public partial class ChartPanel : Control
    {
        //Variaveis de click
        private object objetoClicado = null;
        private DateTime horaClick = new DateTime();
        private double intervaloDesejadoClique = 0.4;

        /// <summary>
        /// Position type of the chart.
        /// </summary>
        public enum PositionType
        {
            /// <summary>
            /// Always top. Panel can't be moved under a panel that has no AlwaysTop type
            /// </summary>
            AlwaysTop,
            /// <summary>
            /// Always bottom - usually used for Volume panels
            /// </summary>
            AlwaysBottom,
            /// <summary>
            /// Arbitrary postion
            /// </summary>
            None
        }

        internal enum StateType
        {
            Normal,
            Maximized,
            Minimized,
            /// <summary>
            /// used when adding new panels, after this it becomes Normal
            /// </summary>
            New,
            Resizing,
            Moving
        }

        internal event EventHandler OnMinimizeClick;
        internal event EventHandler OnMaximizeClick;
        internal event EventHandler OnCloseClick;

        internal PositionType _position;
        internal StockChartX _chartX;
        internal bool _allowDelete;
        internal bool _allowMaxMin;
        internal int _index;
        internal bool _hasPrice;
        internal bool _hasVolume;
        internal bool _enforceSeriesSetMinMax;
        internal Canvas _rootCanvas;
        internal ChartPanelTitleBar _titleBar;
        internal PanelsContainer _panelsContainer;
        /// <summary>
        /// remembers the minimized rectangle, used for restoring panel with animation
        /// </summary>
        internal Rect _minimizedRect;

        internal double _normalTopPosition;//used when restoring panel from maximized state to normal
        internal double _normalHeight;
        internal double _normalHeightPct; //the height on percents, used when restoring panel from minimized state

        private readonly ObservableCollection<Series> _series = new ObservableCollection<Series>();

        internal readonly ObservableCollection<SeriesTitleLabel> _seriesTitle = new ObservableCollection<SeriesTitleLabel>();

        internal readonly List<LineStudy> _lineStudies = new List<LineStudy>();
        internal readonly List<TrendLine> _trendWatch = new List<TrendLine>();
        internal LineStudy _lineStudyToAdd;

        internal LineStudy _lineStudySelected;
        internal Series _seriesSelected;

        internal YAxisCanvas _leftYAxis;
        internal YAxisCanvas _rightYAxis;

        internal StateType _state;

        private Grid _rootGrid;

        #region CanResizeEscala
        /// <summary>
        /// Indica se a escala deste painel pode ser redimensionada ou não.
        /// </summary>
        public bool CanResizeEscala
        {
            get
            {
                switch (_chartX.EscalaAlinhamento)
                {
                    case EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda:
                        if (_leftYAxis != null)
                            return _leftYAxis.CanResizeScale;
                        break;

                    case EnumGeral.TipoAlinhamentoEscalaEnum.Direita:
                        if (_rightYAxis != null)
                            return _rightYAxis.CanResizeScale;
                        break;
                }
                
                return false;
            }
            set
            {
                switch (_chartX.EscalaAlinhamento)
                {
                    case EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda:
                        if (_leftYAxis != null)
                            _leftYAxis.CanResizeScale = value;
                        break;

                    case EnumGeral.TipoAlinhamentoEscalaEnum.Direita:
                        if (_rightYAxis != null)
                            _rightYAxis.CanResizeScale = value;
                        break;
                }
            }
        }
        #endregion CanResizeEscala

        /// <summary>
        /// Actual min of all series (visible records only)
        /// </summary>
        private double _min;
        /// <summary>
        /// Actual max of all series (visible records only)
        /// </summary>
        private double _max;
        /// <summary>
        /// Minimum from all series if panel is not resized with mouse, otherwise keeps th given values
        /// </summary>
        internal double _minChanged;
        /// <summary>
        /// Maximum from all series if panel is not resized with mouse, otherwise keeps th given values
        /// </summary>
        internal double _maxChanged;

        private readonly PaintObjectsManager<Line> _gridXLines = new PaintObjectsManager<Line>();
        private readonly PaintObjectsManager<Line> _gridYLines = new PaintObjectsManager<Line>();
        /// <summary>
        /// used when moving panel up and down
        /// </summary>
        internal double _yOffset;

        internal bool _staticYScale;

        internal TextBlock _betaReminder;
        internal bool _recalc;

        private const string TimerMoveYAxes = "TimerMoveYAxes";
        private const string TimerResizeYAxes = "TimerResizeYAxes";
        internal const string TimerSizeChanged = "TimerSizeChanged";

        internal readonly ChartTimers _timers = new ChartTimers();

        internal bool _templateLoaded;
        /// <summary>
        /// when a panel gets created and a series is added to it its Template may not be loaded
        /// so, set a flag in Paint method and when template is loaded RePaint the panel
        /// </summary>
        internal bool _needRePaint;
        internal bool _painting;
        internal bool _isHeatMap;

        /// <summary>
        /// will ahve references to series that owns(shares) the Y scale
        /// </summary>
        internal readonly List<Series> _shareScaleSeries = new List<Series>();
        static ChartPanel()
        {
#if WPF
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanel), new FrameworkPropertyMetadata(typeof(ChartPanel)));
#endif
            YAxesBackgroundProperty = DependencyProperty.Register("YAxesBackground", typeof(Brush), typeof(ChartPanel),
                                                                  new PropertyMetadata(Brushes.Black, OnYAxesBackgroundChanged));
        }

        ///<summary>
        ///</summary>
        public ChartPanel()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(ChartPanel);
#endif

            InitPanel(null, PositionType.None);

            //ApplyTemplate();
        }

        internal ChartPanel(StockChartX chartX, PositionType positionType)
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(ChartPanel);
#endif
            InitPanel(chartX, positionType);
            
            //ApplyTemplate();
        }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="chartX"></param>
        /// <param name="positionType"></param>
        private void InitPanel(StockChartX chartX, PositionType positionType)
        {
            _chartX = chartX;
            _position = positionType;
            _allowDelete = true;
            _allowMaxMin = true;
            _state = StateType.Normal;
            if (_chartX != null && _chartX.OptimizePainting)
                Background = new SolidColorBrush(Colors.Black);
            else
                Background = new LinearGradientBrush
                               {
                                   StartPoint = new Point(0.5, 0),
                                   EndPoint = new Point(0.5, 1),
                                   GradientStops = new GradientStopCollection
                                           {
                                             new GradientStop
                                               {
                                                 Color = Color.FromArgb(0xFF, 0x7F, 0x7F, 0x7F),
                                                 Offset = 0
                                               },
                                             new GradientStop
                                               {
                                                 Color = Color.FromArgb(0xFF, 0xBF, 0xBF, 0xBF),
                                                 Offset = 1
                                               }
                                           }
                               };

            _timers.RegisterTimer(TimerMoveYAxes, MoveUpDown, 50);
            _timers.RegisterTimer(TimerResizeYAxes, ResizeUpDown, 50);
            _timers.RegisterTimer(TimerSizeChanged, Paint, 50);

            _series.CollectionChanged += Series_OnCollectionChanged;
            if (_chartX != null)
                _chartX.PropertyChanged += ChartX_OnPropertyChanged;
        }

        private void ChartX_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case StockChartX.Property_NewRecord:
                    WatchTrendLines();
                    break;
            }
        }

        public void RestauraPainelMinimizado()
        {
            _panelsContainer.RestauraPainelMinimizado(this);
        }

        public void MaximizaPainel()
        {
            _panelsContainer.MaximizePanel();
        }

        public void MaxMinPanel()
        {
            _panelsContainer.MaxMinPanel(this);
        }

        public void MinimizaPainel()
        {
            _panelsContainer.MinimizePanel(this);
        }

        public void RestauraPainelMaximizado()
        {
            _panelsContainer.MaxMinPanel(this);
            //_panelsContainer.RestoreMaximizedPanel();
        }

        private void Series_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    foreach (var o in e.OldItems)
                    {
                        for (int i = 0; i < _seriesTitle.Count; i++)
                            if (_seriesTitle[i].Series == o)
                            {
                                _seriesTitle[i].UnSubscribe();
                                _seriesTitle.RemoveAt(i);
                                break;
                            }
                    }
                    _panelsContainer.ResetInfoPanelContent();
                    return;
                case NotifyCollectionChangedAction.Add:
                    foreach (var o in e.NewItems)
                    {
                        Series series = (Series)o;
                        if ((series._seriesType == EnumGeral.TipoSeriesEnum.Candle ||
                             series._seriesType == EnumGeral.TipoSeriesEnum.Barra ||
                             series._seriesType == EnumGeral.TipoSeriesEnum.BarraHLC))
                        {
                            if (series.OHLCType == EnumGeral.TipoSerieOHLC.Ultimo)
                                _seriesTitle.Add(new SeriesTitleLabel(series));
                        }
                        else
                        {
                            _seriesTitle.Add(new SeriesTitleLabel(series));
                        }
                    }
                    _panelsContainer.ResetInfoPanelContent();
                    break;
            }
        }

        internal StateType State
        {
            get { return _state; }
            set
            {
                _state = value;
                if (_titleBar != null)
                    _titleBar.PanelGotNewState(this);
            }
        }

        internal double Top
        {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }

        internal double Left
        {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }

        internal Rect Bounds
        {
            get
            {
                return new Rect((double)GetValue(Canvas.LeftProperty), (double)GetValue(Canvas.TopProperty), Width, Height);
            }
            set
            {
                SetValue(Canvas.LeftProperty, value.Left);
                SetValue(Canvas.TopProperty, value.Top);
                Height = value.Height;
                Width = value.Width;
            }
        }


        private string _reasonCantBeDeleted = "";
        internal bool CanBeDeleted
        {
            get { return _reasonCantBeDeleted.Length == 0; }
        }
        internal string ReasonCantBeDeleted
        {
            get { return _reasonCantBeDeleted; }
        }

        internal Rect CanvasRect
        {
            get
            {
                return new Rect(CanvasLeft, Canvas.GetTop(_rootCanvas), _rootCanvas.ActualWidth, _rootCanvas.ActualHeight);
            }
        }

        private double CanvasLeft
        {
            get
            {
                return _chartX.EscalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Ambas ||
                       _chartX.EscalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda
                         ? Constants.YAxisWidth
                         : 0;
            }
        }

        //    internal new double Height
        //    {
        //      get { return base.Height; }
        //      set { base.Height = value; }
        //    }


        internal void SetYAxes()
        {
            if (_chartX == null) return;

            if (_leftYAxis != null)//we do this, cause this function is called from OnYAxesChanged, and in design mode they are null at this time
            {
                _leftYAxis.Visibility = (_chartX.EscalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Ambas || _chartX.EscalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda)
                                          ? Visibility.Visible
                                          : Visibility.Collapsed;
                _rootGrid.ColumnDefinitions[0].Width = new GridLength(_leftYAxis.Visibility == Visibility.Visible ? Constants.YAxisWidth : 0);
            }

            if (_rightYAxis == null) return;
            _rightYAxis.Visibility = (_chartX.EscalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Ambas || _chartX.EscalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Direita)
                                       ? Visibility.Visible
                                       : Visibility.Collapsed;

            _rootGrid.ColumnDefinitions[2].Width =
              new GridLength(_rightYAxis.Visibility == Visibility.Visible ? Constants.YAxisWidth : 0);
        }

        #region Overrides
        /// <summary>
        /// Ovveride
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootCanvas = GetTemplateChild("PART_RootCanvas") as Canvas;
            if (_rootCanvas == null) throw new NullReferenceException();
            _titleBar = GetTemplateChild("PART_TitleBar") as ChartPanelTitleBar;
            if (_titleBar == null) throw new NullReferenceException();
            _rootGrid = GetTemplateChild("rootGrid") as Grid;
            if (_rootGrid == null) throw new NullReferenceException();

            _leftYAxis = GetTemplateChild("leftYAxis") as YAxisCanvas;
            if (_leftYAxis == null) throw new NullReferenceException();
            _rightYAxis = GetTemplateChild("rightYAxis") as YAxisCanvas;
            if (_rightYAxis == null) throw new NullReferenceException();

            //TODO:Tirar, isso apenas um teste
            //_leftYAxis.SetMinMax(0, 0);

            _leftYAxis._chartPanel = _rightYAxis._chartPanel = this;
            _leftYAxis._isLeftAligned = true;
            _rightYAxis._isLeftAligned = false;
            SetYAxes();

            _titleBar.MinimizeClick += (sender, e) => { if (OnMinimizeClick != null) OnMinimizeClick(this, EventArgs.Empty); };
            _titleBar.MaximizeClick += (sender, e) => { if (OnMaximizeClick != null) OnMaximizeClick(this, EventArgs.Empty); };
            _titleBar.CloseClick += (sender, e) => { if (OnCloseClick != null) OnCloseClick(this, EventArgs.Empty); };
#if SILVERLIGHT
            _titleBar.ApplyTemplate();
#endif

            _titleBar._chartPanel = this;
            _titleBar.LabelsDataSource = _seriesTitle;
            _titleBar.Visibility = _chartX == null ? Visibility.Visible : _chartX.MostrarTitulosPaineis ? Visibility.Visible : Visibility.Collapsed;
            if (_titleBar.Visibility == Visibility.Collapsed)
            {
                _rootGrid.RowDefinitions[0].Height = new GridLength(0);
            }
            _titleBar.MaximizeBox = MaximizeBox;
            _titleBar.MinimizeBox = MinimizeBox;
            _titleBar.CloseBox = CloseBox;
            
            _rootCanvas.SizeChanged += RootCanvas_OnSizeChanged;
            _rootCanvas.MouseLeftButtonDown += RootCanvas_OnMouseLeftButtonDown;
            _rootCanvas.MouseLeftButtonUp += RootCanvas_OnMouseLeftButtonUp;
            _rootCanvas.MouseMove += RootCanvas_OnMouseMove;
#if WPF
      _rootCanvas.MouseRightButtonDown += RootCanvas_OnMouseRightButtonDown;
#endif

            if (_chartX != null && _chartX._isBeta)
            {
                _betaReminder = new TextBlock { FontSize = 18 };
                Canvas.SetZIndex(_betaReminder, ZIndexConstants.CrossHairs);
                _rootCanvas.Children.Add(_betaReminder);
            }

            _templateLoaded = true;
        }

        #endregion

        #region Root Canvas events
#if WPF
    private void RootCanvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (_chartX.Status != StockChartX.ChartStatus.Ready) return;

      FrameworkElement element =
        _rootCanvas.HitTest(e.GetPosition(_rootCanvas), new[ ] { typeof (Series), typeof (LineStudy) });
      if (element == null) return;
      Series series = element.Tag as Series;
      if (series != null)
      {
        if (e.ClickCount == 1)
          _chartX.FireSeriesRightClick(series, e.GetPosition(this));
        return;
      }
      LineStudy lineStudy = element.Tag as LineStudy;
      if (lineStudy == null) return;
      if (e.ClickCount == 1)
        _chartX.FireLineStudyRightClick(lineStudy, e.GetPosition(this));
    }
#endif

        private bool _leftMouseDown;
        private Point _currentPoint;

        private static double mouseY;
        private static int mouseX;

        /// <summary>
        /// Obtém o valor (correspondente na escala Y) da posição atual do mouse.
        /// </summary>
        public static double MouseY
        {
            get { return mouseY; }
        }

        /// <summary>
        /// Obtém o record correspondente a posição atual do mouse.
        /// </summary>
        public static int MouseX
        {
            get { return mouseX; }
        }

        /// <summary>
        /// Obtém o record correspondente a posição atual do mouse.
        /// </summary>
        public static void SetMouseX(int record)
        {
            mouseX = record; 
        }

        private static ChartPanel mousePainel = null;

        public static ChartPanel MousePainel
        {
            get { return ChartPanel.mousePainel; }
        }


        private double GetRealLeftOffset()
        {
            if (_chartX.EscalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Direita)
                return 0;
            return Constants.YAxisWidth;
        }

        private Point posicaoAtual = new Point();

        public Point PosicaoAtual
        {
            get { return posicaoAtual; }
            set { posicaoAtual = value; }
        }

        private void RootCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            //mouseX = _chartX.GetReverseX(e.GetPosition(this._rootCanvas).X);
            try
            {
                Point mX = e.GetPosition(this._rootCanvas);

                if (mX != null)
                {
                    mouseX = _chartX.GetRealReverseX(mX.X);
                    mouseY = GetReverseY(mX.Y);
                }
            }
            catch { }

            posicaoAtual = e.GetPosition(this._rootCanvas);

            mousePainel = this;

            Point p = e.GetPosition(_rootCanvas);
            if (!_leftMouseDown)
            {
                _chartX.InvokeChartPanelMouseMove(Index, p.Y, p.X, GetReverseY(p.Y), _chartX.GetReverseX(p.X));
                return;
            }

            switch (_chartX.Status)
            {
                case StockChartX.StatusGrafico.PintandoLinhaEstudo:
                    _lineStudyToAdd.Paint(p.X, p.Y, LineStudy.LineStatus.Painting);
                    break;
                case StockChartX.StatusGrafico.MovendoLinhaEstudo:
                    _lineStudySelected.Paint(p.X, p.Y, LineStudy.LineStatus.Moving);
                    break;
                case StockChartX.StatusGrafico.MovendoSelecao:
                    SeriesMoving(e);
                    break;
                case StockChartX.StatusGrafico.Preparado:
                    if (_lineStudySelected != null && _lineStudySelected.Selected)
                    {
                        if (_currentPoint.Distance(p) > 5)
                        {
                            _lineStudySelected.Paint(p.X, p.Y, LineStudy.LineStatus.StartMove);
                            _chartX.Status = StockChartX.StatusGrafico.MovendoLinhaEstudo;
                        }
                    }
                    else if (_seriesSelected != null && _seriesSelected.Selected && !_chartX.locked)
                    {
                        if (_currentPoint.Distance(p) > 5)
                            _chartX.Status = StockChartX.StatusGrafico.MovendoSelecao;
                    }
                    else
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void AdicionaLinhaEstudoSemUsuario(LineStudy linhaEstudo)
        {
            linhaEstudo.Paint(linhaEstudo.X1Value, linhaEstudo.Y1Value, LineStudy.LineStatus.InserindoDiretamente);
            _lineStudies.Add(_lineStudyToAdd);
        }
        

        private void RootCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point p;

            _leftMouseDown = false;
            _rootCanvas.ReleaseMouseCapture();

            switch (_chartX.Status)
            {
                case StockChartX.StatusGrafico.PintandoLinhaEstudo:
                    p = e.GetPosition(_rootCanvas);
                    _lineStudyToAdd.Paint(p.X, p.Y, LineStudy.LineStatus.EndPaint);
                    _chartX.Status = StockChartX.StatusGrafico.Preparado;
                    _lineStudies.Add(_lineStudyToAdd);

                    _chartX.FireUserDrawingComplete(_lineStudyToAdd.StudyType, _lineStudyToAdd.Key);

                    _lineStudyToAdd = null;
                    break;
                case StockChartX.StatusGrafico.MovendoLinhaEstudo:
                    p = e.GetPosition(_rootCanvas);
                    _lineStudySelected.Paint(p.X, p.Y, LineStudy.LineStatus.EndMove);
                    _chartX.Status = StockChartX.StatusGrafico.Preparado;

                    _chartX.FireLineStudyLeftClick(_lineStudySelected);
                    break;
                case StockChartX.StatusGrafico.MovendoSelecao:
                    MoveSeriesTo(_seriesSelected, _chartPanelToMoveTo, _moveStatusEnum);
                    break;
                case StockChartX.StatusGrafico.Preparado:
                    if (_chartX.InfoPanelPosicao == EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse)
                    {
                        _chartX.StopShowingInfoPanel();
                        _rootCanvas.ReleaseMouseCapture();
                    }
                    break;
            }
        }
        private static int clickCount = 0;
        private void RootCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p;
#if WPF
      int clickCount = e.ClickCount;
#endif
#if SILVERLIGHT
            
#endif
            if (_chartX == null) return;

            _leftMouseDown = true;
            _currentPoint = e.GetPosition(_rootCanvas);

            switch (_chartX.Status)
            {
                case StockChartX.StatusGrafico.PreparadoParaPintarLinhaEstudo:  //begin paiting a line study
                    if (_lineStudyToAdd == null) //user paints
                        _chartX.GetLineStudyToAdd(this);

                    _chartX.Status = StockChartX.StatusGrafico.PintandoLinhaEstudo;
                    p = e.GetPosition(_rootCanvas);
                    if (_lineStudyToAdd != null)
                        _lineStudyToAdd.Paint(p.X, p.Y, LineStudy.LineStatus.StartPaint);
                    if (_lineStudySelected != null)
                    {
                        _lineStudySelected.Selected = false;
                        _lineStudySelected = null;
                    }
                    _rootCanvas.CaptureMouse();
                    break;
                case StockChartX.StatusGrafico.Preparado: //check for hitting a series or line studies
                    FrameworkElement element =
                      _rootCanvas.HitTest(e.GetPosition(_rootCanvas),
                                          new[] { typeof(LineStudy), typeof(Series), typeof(ContextLine) });
                    Debug.WriteLine("HitTest " + element);

                    if (element == null)
                    {
                        if (_lineStudySelected != null)
                        {
                            _lineStudySelected.Selected = false;
                            _lineStudySelected = null;
                        }
                        if (_seriesSelected != null)
                        {
                            _seriesSelected.HideSelection();
                            _seriesSelected = null;
                        }

                        objetoClicado = null;
                        clickCount = 0;
                    }
                    else
                    {
                        LineStudy lineStudy = element.Tag as LineStudy;
                        if (lineStudy != null && lineStudy.Selectable)
                        {
                            if (_lineStudySelected != null && lineStudy != _lineStudySelected)
                            {
                                _lineStudySelected.Selected = false;
                                _lineStudySelected = null;
                            }

                            _lineStudySelected = lineStudy;
                            _lineStudySelected.Selected = true;

                            if (_seriesSelected != null)
                            {
                                _seriesSelected.HideSelection();
                                _seriesSelected = null;
                            }

                            if ((objetoClicado is LineStudy) && (((LineStudy)objetoClicado).Key == _lineStudySelected.Key) && (DateTime.Now.Subtract(horaClick).TotalSeconds < intervaloDesejadoClique))
                                clickCount = 2;

                            objetoClicado = _lineStudySelected;
                            horaClick = DateTime.Now;

                            if (objetoClicado is LineStudy)
                                _chartX.FireLineStudyLeftClick((LineStudy)objetoClicado);

                            if (clickCount >= 2)
                            {
                                _leftMouseDown = false;
                                _rootCanvas.ReleaseMouseCapture();
                                _chartX.FireLineStudyDoubleClick(lineStudy);
                                clickCount = 1;
                            }
                            else
                                _rootCanvas.CaptureMouse();
                            break;
                        }
                        //series drag & drop
                        Series series = element.Tag as Series;
                        if (CanStartMoveSeries(series) != MoveSeriesIndicator.MoveStatusEnum.ImpossivelMover)
                        {
                            if ((objetoClicado is Series) && (((Series)objetoClicado).Name == series.Name) && (DateTime.Now.Subtract(horaClick).TotalSeconds < intervaloDesejadoClique))
                                clickCount = 2;

                            StartMoveSeries(series, clickCount);

                            objetoClicado = series;
                            horaClick = DateTime.Now;

                            if (clickCount >= 2)
                                clickCount = 1;

                            break;
                        }

                        ContextLine contextLine = element.Tag as ContextLine;
                        if (contextLine != null)
                        {
                            break;
                        }
                    }


                    //just mouse down
                    if (_chartX.InfoPanelPosicao == EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse) //start timer and show infopanel
                    {
                        _chartX.StartShowingInfoPanel();
                        _rootCanvas.CaptureMouse();
                    }

                    break;
            }
        }

        private readonly EnumGeral.TipoSerieOHLC[] _ohlcTypes = new EnumGeral.TipoSerieOHLC[]
                                      {
                                        EnumGeral.TipoSerieOHLC.Abertura, EnumGeral.TipoSerieOHLC.Maximo,
                                        EnumGeral.TipoSerieOHLC.Minimo, EnumGeral.TipoSerieOHLC.Ultimo,
                                      };

        private void RootCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _rootCanvas.Clip = new RectangleGeometry
                                 {
                                     Rect = new Rect(0, 0, _rootCanvas.ActualWidth, _rootGrid.ActualHeight)
                                 };

            var ps = _panelsContainer.stockChart.estiloPreco;

            //only for non-standard price styles
            if (ps != EnumGeral.EstiloPrecoEnum.Padrao && ps != EnumGeral.EstiloPrecoEnum.HeikinAshi)
            {
                int count = SeriesCollection.Count(s => _ohlcTypes.Contains(s.OHLCType));
                if (count < 3)
                {
                    //a panel that does not have OHLC series
                    //queue it's painting until an OHLC panel is repainted

                    _panelsContainer._panelsToBeRepainted.Add(this);
                    return;
                }
            }

            _timers.StartTimerWork(TimerSizeChanged);
        }
        #endregion


        #region Series Related methods
        /// <summary>
        /// Gind a series with same name but different OHLCV type
        /// </summary>
        /// <param name="series">A series from OHLC group.</param>
        /// <param name="seriesTypeOHLC">Needed OHLCV type</param>
        /// <returns>Reference to a series or null</returns>
        public Series GetSeriesOHLCV(Series series, EnumGeral.TipoSerieOHLC seriesTypeOHLC)
        {
            foreach (Series s in _series)
            {
                if (s.Name == series.Name && s.OHLCType == seriesTypeOHLC) return s;
            }
            return null;
        }

        internal Series CreateSeries(string seriesName, EnumGeral.TipoSerieOHLC ohlcType, EnumGeral.TipoSeriesEnum seriesType)
        {
            Series series = CreateSeries(seriesName, seriesType, ohlcType);
            return AddSeries(series);
        }

        internal Series FirstSeries
        {

            get
            {
                return _series.Count > 0 ? _series[0] : null;
            }
        }

        internal int SeriesCount
        {
            get { return _series.Count; }
        }

        /// <summary>
        /// Adds a series to internal collection
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        internal Series AddSeries(Series series)
        {
            _series.Add(series);
            if (_series.Count == 1) //first series, it will own Y scale
                _shareScaleSeries.Add(series);
            else
            {
                if (series.Name != _shareScaleSeries[0].Name && !(series is Indicator)) //if different group it can't share scale, unless is an indicator
                    series._shareScale = false;

                if (series._shareScale)
                    _shareScaleSeries.Add(series);
            }
            return series;
        }

        /// <summary>
        /// Deletes a series from internal collection
        /// </summary>
        /// <param name="series"></param>
        internal void DeleteSeries(Series series)
        {
            _shareScaleSeries.Remove(series);
            _series.Remove(series);
            if (_shareScaleSeries.Count != 0 || _series.Count <= 0) return;
            _series[0]._shareScale = true;
            _shareScaleSeries.Add(_series[0]);
        }

        internal IEnumerable<Series> GetSeriesFromGroup(string groupName)
        {
            foreach (Series series in _series)
            {
                if (series.Name == groupName)
                    yield return series;
            }
        }

        internal IEnumerable<Series> AllSeriesCollection
        {
            get
            {
                foreach (Series series in _series)
                {
                    yield return series;
                }
            }
        }

        internal void RemoveSeries(Series series)
        {
            RemoveSeries(series, false);
        }
        internal void RemoveSeries(Series series, bool delete)
        {
            if (series == null) return;

            series.RemovePaint();
            series.HideSelection();
            if (delete)
                _series.Remove(series);

            _chartX.dataManager.UnRegisterSeries(series.Name, series.OHLCType);

            foreach (Series child in series._linkedSeries)
            {
                RemoveSeries(child, true);
            }
        }
        #endregion

        #region Private & Internal Methods
        private DataManager.DataManager DM
        {
            get { return _chartX.dataManager; }
        }

        /*
            private void GetMinMax(IEnumerable<Series> series, out double min, out double max)
            {
              min = double.MaxValue;
              max = double.MinValue;

              foreach (Series s in series)
              {
                if (_enforceSeriesSetMinMax)
                  s.SeriesEntry._visibleDataChanged = true;

                DM.VisibleMinMax(s._seriesIndex, out s._min, out s._max);

                if (s._min < min) min = s._min;
                if (s._max > max) max = s._max;
              }
            }
        */

        private int _labelCount;
        private double _gridStep;
        /// <summary>
        /// Get the max/min for scaling
        /// </summary>
        internal void SetMaxMin()
        {
            //if (_staticYScale) return;

            if (!_staticYScale)
            {
                _min = double.MaxValue;
                _max = double.MinValue;
            }

            _hasPrice = false;
            _hasVolume = false;
            List<Series> shareScaleSeries = new List<Series>();
            foreach (Series series in _shareScaleSeries)
            {
                if (!series._shareScale) continue;
                shareScaleSeries.AddRange(GetSeriesFromGroup(series.Name));
            }
            foreach (Series series in _series)
            {
                if (!series._shareScale || shareScaleSeries.IndexOf(series) != -1) continue;
                shareScaleSeries.Add(series);
            }

            foreach (Series series in AllSeriesCollection)
            {
                if (_enforceSeriesSetMinMax)
                    series.SeriesEntry._visibleDataChanged = true;

                DM.VisibleMinMax(series._seriesIndex, out series._min, out series._max);
                //        series._min = DM.Min(series._seriesIndex);
                //        series._max = DM.Max(series._seriesIndex);

                if (shareScaleSeries.Count == 0)
                {
                    if (series._min < _min) _min = series._min;
                    if (series._max > _max) _max = series._max;
                }
                else
                {
                    //analize just series that hold the Y scale
                    int seriesIndex = series._seriesIndex;
                    if (shareScaleSeries.FindIndex(series1 => series1._seriesIndex == seriesIndex) != -1)
                    {
                        if (series._min < _min) _min = series._min;
                        if (series._max > _max) _max = series._max;
                    }
                }

                switch (series.OHLCType)
                {
                    case EnumGeral.TipoSerieOHLC.Ultimo:
                        _hasPrice = true;
                        break;
                    case EnumGeral.TipoSerieOHLC.Volume:
                        _hasVolume = true;
                        break;
                }
            }

            if (!_staticYScale)
            {
                if (_max == double.MinValue)
                {
                    _max = 1;
                    _min = 0;
                }

                _minChanged = _min;
                _maxChanged = _max;

                _labelCount = (int)(ActualHeight / (_chartX.GetTextHeight("0") * 2.5));
                if (_labelCount < 4)
                    _labelCount = 4;

                double realMin, realMax;
                Utils.GridScaleReal(_minChanged, _maxChanged, _labelCount, out realMin, out realMax, out _labelCount, out _gridStep);

                _minChanged = realMin;
                _maxChanged = realMax;
                //
                //        if (Index == 0)
                //          Debug.WriteLine(string.Format("Panel min = {0}, max = {1}. PanelHeight = {2}. Count = {3}", _minChanged, _maxChanged, PaintableHeight, _labelCount));
            }

            if (Math.Abs(_minChanged - _maxChanged) <= 0.00001)
            {
                if (_minChanged == 0)
                {
                    _minChanged = -0.01;
                    _maxChanged = 0.01;
                }
                else
                {
                    double median = (_minChanged + _maxChanged) / 2;
                    _maxChanged = median + Math.Abs(median) * 0.005;
                    _minChanged = median - Math.Abs(median) * 0.005;
                }
            }

            _enforceSeriesSetMinMax = false;
        }

        private void PostIndicatorCalculate()
        {
            _timers.StopTimerWork(TimerSizeChanged);

            if (!_staticYScale)
                SetMaxMin();

            if (_leftYAxis.Visibility == Visibility.Visible)
            {
                _leftYAxis.GridStep = _gridStep;
                _leftYAxis.LabelCount = _labelCount;
                _leftYAxis.SetMinMax(Min, Max);
            }
            if (_rightYAxis.Visibility == Visibility.Visible)
            {
                _rightYAxis.GridStep = _gridStep;
                _rightYAxis.LabelCount = _labelCount;
                _rightYAxis.SetMinMax(Min, Max);
            }

            PaintSideVolumeDepthBars();

            foreach (Series series in _series)
            {
                series.Painted = false; //reset flag
                series.Paint(); //repaint
            }

            _lineStudies.ForEach(ls => ls.Paint(0, 0, LineStudy.LineStatus.RePaint));

            //PaintXGrid();

            if (_chartX._isBeta)
            {
                _betaReminder.Text = "BETA VERSION - please report bugs to support@Traderdata.Client.Componente.GraficoSL.StockChart.com";
                Canvas.SetLeft(_betaReminder, 100);
                Canvas.SetTop(_betaReminder, 50);
                _betaReminder.Foreground = Brushes.Red;
                Canvas.SetZIndex(_betaReminder, ZIndexConstants.PriceStyles1);
            }

            _painting = false;
            _recalc = false;

            if (_series.Count > 0) //only if panel has series left then it was actually painted, otherwise it will be deleted
                _chartX.FireChartPanelPaint(this);

            if (_needRePaint)
            {
                _needRePaint = false;
                if (!IsHeatMap)
                {
                    _panelsContainer.ResetHeatMapPanels();
                    _panelsContainer.RecyclePanels();
                }
            }
            //      Debug.WriteLine("After series paint end.");
        }

        private bool PreIndicatorCalculate()
        {
            if (!_chartX.CheckRegistration()) return false;

            if (!_templateLoaded) _needRePaint = true;
            if (!_templateLoaded || _chartX.locked) return false;
            if (_rootCanvas.ActualHeight == 0) return false;

            if (_painting)
            {
                _timers.StopTimerWork(TimerSizeChanged);
                return false;
            }
            _painting = true;

            //      Debug.WriteLine("PreIndicatorCalculate");

            return true;
        }


        internal Action _afterPaintAction;
        internal virtual void Paint()
        {
            if (!PreIndicatorCalculate()) return;

            CalculateIndicators();
#if WPF      
      PostIndicatorCalculate(); //for Silverlight we move this code to ProcessIndicators, cause of the non-modal behavior of pseudo-dialogs in SL
      if (_afterPaintAction != null)
        _afterPaintAction();
#endif

            //      Debug.WriteLine("Paint - end");

            if (!_panelsContainer._panelsToBeRepainted.Contains(this))
            {
                _panelsContainer._panelsToBeRepainted.ForEach(panel => panel.Paint());
                _panelsContainer._panelsToBeRepainted.Clear();
            }
        }

        private YAxisCanvas _yaxisMovingInAction;
        private double _yMoveUpDownStart;
        internal void StartYMoveUpDown(YAxisCanvas axisCanvas)
        {
            if (_isHeatMap) return;
            _yaxisMovingInAction = axisCanvas;
            _yMoveUpDownStart = Mouse.GetPosition(_yaxisMovingInAction).Y;
            _timers.StartTimerWork(TimerMoveYAxes);

        }

        internal void StopYMoveUpDown(YAxisCanvas axisCanvas)
        {
            if (_isHeatMap) return;
            _yaxisMovingInAction = null;
            _timers.StopTimerWork(TimerMoveYAxes);
        }
        /// <summary>
        /// this method will be called from timer, when user will press in Y axes the right mouse button
        /// </summary>
        internal void MoveUpDown()
        {
            if (_isHeatMap) return;
            Point p = Mouse.GetPosition(_yaxisMovingInAction);
            _yOffset -= (_yMoveUpDownStart - p.Y);
            Paint();
            _yMoveUpDownStart = p.Y;
        }

        private YAxisCanvas _yaxisResizeInAction;
        private double _yResizeStart;
        internal void StartYResize(YAxisCanvas axisCanvas)
        {
            if (_isHeatMap) return;
            _yaxisResizeInAction = axisCanvas;
            _yResizeStart = Mouse.GetPosition(_yaxisResizeInAction).Y;
            //Debug.WriteLine(string.Format("ResizeUpDown start {0}", _y));
            _timers.StartTimerWork(TimerResizeYAxes);
        }

        internal void StopYResize(YAxisCanvas axisCanvas)
        {
            if (_isHeatMap) return;
            _yaxisResizeInAction = null;
            _timers.StopTimerWork(TimerResizeYAxes);
        }

        internal void ResizeUpDown()
        {
            if (_isHeatMap) return;

            Point p = Mouse.GetPosition(_yaxisResizeInAction);
            _staticYScale = true;

            if (_yResizeStart == p.Y) return;

            double diff = (_maxChanged - _minChanged) * 0.05; //5%
            if ((_yResizeStart - p.Y) > 0)
            {
                //by increasing max and decreasing min, chart will become "shorter" by Y
                _maxChanged += diff;
                _minChanged -= diff;
            }
            else
            {
                _maxChanged -= diff;
                _minChanged += diff;
            }

            Paint();
            _yResizeStart = p.Y;
        }

        internal void StartPaintingYGridLines()
        {
            if (!_chartX.GradeY || _isHeatMap) return;
            _gridYLines.C = _rootCanvas;
            _gridYLines.Start();
        }

        internal void StopPaintingYGridLines()
        {
            if (!_chartX.GradeY || _isHeatMap) return;
            _gridYLines.Stop();
            _gridYLines.Do(l => l.ZIndex = ZIndexConstants.GridLines);
        }

        internal void PaintYGridLine(double y)
        {
            if (!_chartX.GradeY || _isHeatMap) return;
            //      System.Windows.Shapes.Line line = _gridYLines.GetPaintObject()._line;
            //      line.Stroke = _chartX.GridStroke;
            //      line.StrokeThickness = 1;
            //      Canvas.SetLeft(line, 0);
            //      line.Width = _rootCanvas.ActualWidth;
            //      Canvas.SetTop(line, y);
            //rectangle.Height = 1;
            Utils.DrawLine(0, y, _rootCanvas.ActualWidth, y, _chartX.GradeCor, EnumGeral.TipoLinha.Solido, 1, _gridYLines);
        }

        internal void ShowHideYGridLines()
        {
            bool linesVisible = _chartX.GradeY;
            _gridYLines.Do(l => l._line.Visibility = linesVisible ? Visibility.Visible : Visibility.Collapsed);
        }

        internal void StartPaintingXGridLines()
        {
            if (!_chartX.GradeX) return;
            _gridXLines.C = _rootCanvas;
            _gridXLines.Start();
        }

        internal void StopPaintingXGridLines()
        {
            if (!_chartX.GradeX) return;
            _gridXLines.Stop();
            _gridXLines.Do(l => l.ZIndex = ZIndexConstants.GridLines);
        }

        internal void PaintXGridLine(double x)
        {
            if (!_chartX.GradeX || !_templateLoaded) return;

            //Utils.DrawLine(x, 0, x, _rootCanvas.ActualHeight, _chartX.GradeCor, EnumGeral.tipoLinha.Solido, 1, _gridXLines);
            foreach (var pair in _chartX.xGridMap)
            {
                Utils.DrawLine(pair.Value, 0, pair.Value, _rootCanvas.ActualHeight, _chartX.GradeCor, EnumGeral.TipoLinha.Solido, 1, _gridXLines);
            }
        }

        internal void ShowHideXGridLines()
        {
            if (_chartX.GradeX && _gridXLines.Count == 0)
            {
                PaintXGrid();
            }
            _gridXLines.Do(l => l._line.Visibility = _chartX.GradeX ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// Paint X Grid
        /// Mainly called fron Calendar OnPaint, after it paints itself and prepares xGridMap
        /// </summary>
        internal virtual void PaintXGrid()
        {
            StartPaintingXGridLines();
            PaintXGridLine(0);
            StopPaintingXGridLines();
        }

        internal virtual Series CreateSeries(string seriesName,
          EnumGeral.TipoSeriesEnum seriesType, EnumGeral.TipoSerieOHLC seriesTypeOHLC)
        {
            switch (seriesType)
            {
                case EnumGeral.TipoSeriesEnum.Linha:
                    return new Standard(seriesName, seriesType, seriesTypeOHLC, this);
                case EnumGeral.TipoSeriesEnum.Barra:
                    return new Stock(seriesName, seriesType, seriesTypeOHLC, this);
                case EnumGeral.TipoSeriesEnum.BarraHLC:
                    return new Stock(seriesName, seriesType, seriesTypeOHLC, this);
                case EnumGeral.TipoSeriesEnum.Candle:
                    return new Stock(seriesName, seriesType, seriesTypeOHLC, this);
                case EnumGeral.TipoSeriesEnum.Volume:
                    return new Standard(seriesName, EnumGeral.TipoSeriesEnum.Volume, EnumGeral.TipoSerieOHLC.Volume, this);
                default:
                    throw new ArgumentException("[AddNewSeriesType] SeriesType " + seriesType + " not supported.");
            }
        }

        /// <summary>
        /// updates only the visual presentation of he trendline
        /// actual penetration check happens within Series class
        /// </summary>
        internal void WatchTrendLines()
        {
            int recordCount = _chartX.RecordCount;

            foreach (TrendLine trendLine in _trendWatch)
            {
                // Automatically extend the trend line into the future
                double x1 = trendLine.X1Value;
                double y1 = trendLine.Y1Value;
                double x2 = trendLine.X2Value;
                double y2 = trendLine.Y2Value;

                if (x2 == x1) return;

                double incr = (y2 - y1) / (x2 - x1);

                trendLine.SetXYValues(x1, y1, x1 + (recordCount - x1), y1 + (incr * (recordCount - x1)));
            }
        }

        internal void RegisterWatchableTrendLine(TrendLine trendLine)
        {
            _trendWatch.Add(trendLine);
            WatchTrendLines();
            foreach (Series series in _series)
            {
                series.CheckTrendLinesPenetration();
            }
        }

        internal void UnRegisterWatchableTrendLine(TrendLine trendLine)
        {
            _trendWatch.Remove(trendLine);
        }

        internal IList<TrendLine> WatchableTrendLines
        {
            get { return _trendWatch; }
        }

        internal void ShowHideTitleBar()
        {
            if (!_templateLoaded) return;
            if (_chartX.MostrarTitulosPaineis)
            {
                _titleBar.Visibility = Visibility.Visible;
                _rootGrid.RowDefinitions[0].Height = new GridLength(Constants.PanelTitleBarHeight);
            }
            else
            {
                _titleBar.Visibility = Visibility.Collapsed;
                _rootGrid.RowDefinitions[0].Height = new GridLength(0);
            }
        }

        internal void UnRegisterSeriesFromDataManager()
        {
            while (_series.Count > 0)
            {
                string seriesName = _series[0].Name;
                EnumGeral.TipoSerieOHLC ohlcType = _series[0].OHLCType;
                _series.RemoveAt(0);
                _chartX.dataManager.UnRegisterSeries(seriesName, ohlcType);
            }
        }
        #endregion


        /// <summary>
        /// Séries presentes neste painel.
        /// </summary>
        /// <returns></returns>
        public List<Series> Series
        {
            get
            {
                List<Series> series = new List<Series>();

                foreach (object obj in _series)
                {
                    Series serieAUX = obj as Series;

                    if (serieAUX != null)
                    {
                        series.Add(serieAUX);
                    }
                }

                return series;
            }
        }

        public List<LineStudy> ObjetosPainel
        {
            get {return _lineStudies;}
        }
    }
}
