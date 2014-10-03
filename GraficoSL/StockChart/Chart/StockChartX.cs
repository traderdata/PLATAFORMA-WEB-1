using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.StockChart.Data;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
using FrameworkElement=System.Windows.FrameworkElement;


[assembly: CLSCompliant(true)]
namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    [CLSCompliant(true)]
    public partial class StockChartX : Control
    {
        //Evento de Click no titulo indicador
        public delegate void TituloIndicadorDelagate(Series serie);
        public event TituloIndicadorDelagate OnTituloIndicador;

        #region Guardando Instancia do Grafico Main a que este objeto pertence

        private GraficoSL.Main.Grafico graficoMain = null;

        public static double Variacao = 0;

        public GraficoSL.Main.Grafico GraficoMain
        {
            get { return graficoMain; }
            set { graficoMain = value; }
        }
        #endregion Guardando Instancia do Grafico Main a que este objeto pertence

        public ChartPanel MousePainel
        {
            get { return ChartPanel.MousePainel;}
        }

        public double MouseY
        {
            get { return ChartPanel.MouseY; }
        }

        public int MouseX
        {
            get { return ChartPanel.MouseX; }
        }

        public void SetMouseX(int record)
        {
            ChartPanel.SetMouseX(record);
        }

        public double HeightTotal
        {
            get { return HeightPaineisBar + HeightCalendario + HeightPaineisContainer + HeightScrollBar; }
        }

        public double HeightScrollBar
        {
            get
            {
                if (_chartScroller != null)
                    return _chartScroller.ActualHeight;
                else
                    return 0;
            }
        }

        public double HeightPaineisBar
        {
            get
            {
                if (paineisBar != null)
                    return paineisBar.ActualHeight;
                else
                    return 0;
            }
        }

        public double HeightCalendario
        {
            get
            {
                if (calendario != null)
                    return calendario.ActualHeight;
                else
                    return 0;
            }
        }


        public double HeightPaineisContainer
        {
            get
            {
                if (paineisContainers != null)
                    return paineisContainers.ActualHeight;
                else
                    return 0;
            }
        }

        #if PERSONAL
            private bool? _registered;
        #elif DEMO
            private bool? _registered;
            private string _demoText;
        #endif
        /// <summary>
        /// will be turned off when release
        /// </summary>
        internal readonly bool _isBeta;

        internal int indexInicial;
        internal int indexFinal;

        #region Internal Fields

        internal PanelsBarContainer paineisBar;
        internal Calendar calendario;
        internal PanelsContainer paineisContainers;

        internal bool darwasBoxes;
        internal bool mostraTitulos;
        internal bool usarCoresAltaBaixaParaVolume;
        internal bool usarCoresLinhasSeries;
        internal bool mostraDialogoErro;
        internal bool atualizandoIndicador;
        internal bool locked;
        private bool recalc;
        internal bool changed;

        internal double larguraBarra;
        internal double espacamentoBarra;
        internal double intervaloBarra;
        internal double[] parametrosEstiloPreco = new double[Constants.MaxPriceStyleParams];
        internal double darvasPercent;

        internal Color? corContornoCandleBaixa;
        internal Color? corContornoCandleAlta;
        internal Color corLinhasHorizontais;

        internal EnumGeral.EstiloPrecoEnum estiloPreco;
        internal EnumGeral.TipoGraficoEnum tipoGrafico = EnumGeral.TipoGraficoEnum.OHLC;
        internal EnumGeral.TipoEscala escalaTipo = EnumGeral.TipoEscala.Linear;
        internal EnumGeral.TipoAlinhamentoEscalaEnum escalaAlinhamento = EnumGeral.TipoAlinhamentoEscalaEnum.Direita;
        internal Double espacoEsquerdaGrafico = 10.0;
        internal Double espacoDireitaGrafico = 50.0;

        internal PriceStyleValuesCollection estiloPrecoValor1 = new PriceStyleValuesCollection();
        internal PriceStyleValuesCollection estiloPrecoValor2 = new PriceStyleValuesCollection();
        internal PriceStyleValuesCollection estiloPrecoValor3 = new PriceStyleValuesCollection();

        /// <summary>
        /// Mantem o valor da coordenada X para qualquer record visível na tela.
        /// Usado para pintar as grades X
        /// </summary>
        internal Dictionary<int, double> xGridMap = new Dictionary<int, double>();
        internal double[] xMap = new double[0];
        internal int xCount;

        internal class BarBrushData
        {
            public bool Changed;
            public Brush Brush;
        }
        internal Dictionary<int, BarBrushData> barBrushes = new Dictionary<int, BarBrushData>(128);

        internal DataManager.DataManager dataManager;
        #endregion

        #region Private Fields
        private ChartPanel painelAtual;

        public ChartPanel PainelAtual
        {
            get { return painelAtual; }
            set { painelAtual = value; }
        }
        private Grid rootGrid;
        private ChartScroller _chartScroller;
        //font properties used to paint all the text on the chart
        private string fontFamily = "Verdana";
        private double fontSize = 9;
        private Brush fontForeground = Brushes.Yellow;

        /// <summary>
        /// used to show an arbitraty text in any position of the chart
        /// </summary>
        private readonly TextBlock textoLabelTitulo = new TextBlock();

        internal enum StatusGrafico
        {
            Construindo,
            RedimensionandoPaineis,
            Preparado,
            MovendoSelecao,
            PreparadoParaPintarLinhaEstudo,
            PintandoLinhaEstudo,
            MovendoLinhaEstudo,
            ZoomStart,
            ZoomPaitingRect
        }

        private StatusGrafico status;
        private bool templateLoaded;

        private LineStudy linhasEstudoParaAdicionar;
        #endregion

        ///<summary>
        /// Initializes a new instance of the <seealso cref="StockChartX"/> class.
        ///</summary>
        public StockChartX()
        {
            _isBeta = false;

#if WPF
      //RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
#endif

#if SILVERLIGHT
            DefaultStyleKey = typeof(StockChartX);
#endif

            dataManager = new DataManager.DataManager(this);

            Background = Brushes.Black;
            Foreground = Brushes.White;
            indexInicial = indexFinal = 0; //no data. all panels' data are related to these indexes

            darwasBoxes = false;
            mostraTitulos = true;
            usarCoresAltaBaixaParaVolume = false;
            usarCoresLinhasSeries = false;

            corLinhasHorizontais = Colors.White;
            corContornoCandleBaixa = null;
            corContornoCandleAlta = null;

            espacamentoBarra = 0;
            intervaloBarra = 60;
            larguraBarra = 1;
            darvasPercent = 0.01;

            estiloPreco = EnumGeral.EstiloPrecoEnum.Padrao;

            status = StatusGrafico.Preparado;

            _timers.RegisterTimer(TimerMove, MoveChart, 50);
            _timers.RegisterTimer(TimerResize, ResizeChart, 50);
            _timers.RegisterTimer(TimerUpdate, Update, 50);
            _timers.RegisterTimer(TimerCrossHairs, MoveCrossHairs, 50);            
            _timers.RegisterTimer(TimerInfoPanel, ShowInfoPanelInternal, 50);

#if WPF
      MouseWheel += Chart_MouseWheel;
#endif
            //initialize needed variables
            CheckRegistration();

#if SILVERLIGHT
            Mouse.RegisterMouseMoveAbleElement(this);
            MouseMove += (sender, e) => Mouse.UpdateMousePosition(this, e.GetPosition(this));
#endif      
            IndicatorLeftClick += new EventHandler<IndicatorDoubleClickEventArgs>(StockChartX_IndicatorLeftClick);
            LineStudyLeftClick += new EventHandler<LineStudyMouseEventArgs>(StockChartX_LineStudyLeftClick);


            //TODO:tive que inicizalir o painelsContainer e o calendar por alguma razao
            paineisContainers = new PanelsContainer();
            paineisContainers.stockChart = this;
            calendario = new Calendar();
            calendario._chartX = this;
            
        }

        #region Internal Properties
        #endregion

        #region Overrides

        /// <summary>
        /// Ovveride
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            rootGrid = GetTemplateChild("rootGrid") as Grid;
            if (rootGrid == null) throw new NullReferenceException();
            paineisContainers = GetTemplateChild("rootCanvas") as PanelsContainer;
            if (paineisContainers == null) throw new NullReferenceException();
            calendario = GetTemplateChild("calendarPanel") as Calendar;
            if (calendario == null) throw new NullReferenceException();
            paineisBar = GetTemplateChild("panelsBar") as PanelsBarContainer;
            if (paineisBar == null) throw new NullReferenceException();
            _chartScroller = GetTemplateChild("scroller") as ChartScroller;

            if (_chartScroller != null)
            {
                _chartScroller.OnPositionChanged += ChartScrollerOnPositionChanged;
                _chartScroller.DuploCliqueScroll += new ChartScroller.DuploCliqueScrollDelegate(_chartScroller_DuploCliqueScroll);
                ShowHideChartScroller();
                _chartScroller.TrackBackground = ChartScrollerTrackBackground;
                _chartScroller.TrackButtonsBackground = ChartScrollerTrackButtonsBackground;
                _chartScroller.ThumbButtonBackground = ChartScrollerThumbButtonBackground;
            }



            //init objects
            paineisContainers.stockChart = this;
            paineisContainers.panelsHolder.Children.Add(textoLabelTitulo);
            textoLabelTitulo.Visibility = Visibility.Collapsed;

            Canvas.SetZIndex(textoLabelTitulo, ZIndexConstants.TextLabelTitle);

            paineisBar._chartX = this;
            paineisBar.OnButtonClicked += (sender, e) => paineisContainers.RestorePanel(e._chartPanel);

            calendario._chartX = this;

            SetPanelsBarVisibility();

            calendario.MouseLeftButtonDown += Calendar_OnMouseLeftButtonDown;
            calendario.MouseLeftButtonUp += Calendar_OnMouseLeftButtonUp;
            calendario.KeyDown += Calendar_OnKeyDown;
            calendario.KeyUp += Calendar_OnKeyUp;
            KeyDown += Calendar_OnKeyDown;
            KeyUp += Calendar_OnKeyUp;
            MouseLeftButtonDown += (sender, e) => Focus();
#if WPF
      _calendar.MouseRightButtonDown += Calendar_OnMouseRightButtonDown;
      _calendar.MouseRightButtonUp += Calendar_OnMouseRightButtonUp;
#endif

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                AddChartPanel();
                paineisContainers.InvalidateMeasure();
                paineisContainers.InvalidateArrange();
            }

            SetYAxes();
            templateLoaded = true;

            ChartLoaded(this, EventArgs.Empty);
        }

        private bool _scrollerUpdating;
        private void ChartScrollerOnPositionChanged(object sender, int left, int right, ref bool cancel)
        {
            if (left == indexInicial && right == indexFinal)
                return;
            _scrollerUpdating = true;
            indexInicial = left;
            if (indexFinal != right)
            {
                indexFinal = right;
                OnPropertyChanged(Property_EndIndex);
            }

            //dispara um evento de que o scroll foi alterado de alguma forma
            FireChartScroll();

#if SILVERLIGHT
            ThreadPool.QueueUserWorkItem(state => Dispatcher.BeginInvoke(Update));
#endif
#if WPF
      ThreadPool.QueueUserWorkItem(
        state => Dispatcher
          .BeginInvoke(DispatcherPriority.Normal, new Action(Update)));
#endif

            //      Update();
            _scrollerUpdating = false;
        }

        #endregion Overrides

        #region Private Methods
        private void SetYAxes()
        {
            foreach (ChartPanel panel in paineisContainers.Panels)
            {
                panel.SetYAxes();
            }
            if (calendario == null) return;
            switch (EscalaAlinhamento)
            {
                case EnumGeral.TipoAlinhamentoEscalaEnum.Direita:
                    rootGrid.ColumnDefinitions[0].Width = new GridLength(0);
                    rootGrid.ColumnDefinitions[2].Width = new GridLength(Constants.YAxisWidth);
                    break;
                case EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda:
                    rootGrid.ColumnDefinitions[0].Width = new GridLength(Constants.YAxisWidth);
                    rootGrid.ColumnDefinitions[2].Width = new GridLength(0);
                    break;
                default:
                    rootGrid.ColumnDefinitions[0].Width = new GridLength(Constants.YAxisWidth);
                    rootGrid.ColumnDefinitions[2].Width = new GridLength(Constants.YAxisWidth);
                    break;
            }
            rootGrid.UpdateLayout();
            Update();
        }

        private void SetPanelsBarVisibility()
        {
            if (paineisBar == null)
                return;
            paineisBar.UpdateVisibility();
            rootGrid.RowDefinitions[2].Height = new GridLength(paineisBar.Visible ? Constants.PanelsBarHeight : 0);
        }

        private void ShowHideLinhasGradeY()
        {
            if (paineisContainers == null)
                return;
            paineisContainers.Panels.ForEach(p => p.ShowHideYGridLines());
        }

        private void ShowHideLinhasGradeX()
        {
            if (paineisContainers == null)
                return;
            paineisContainers.Panels.ForEach(p => p.ShowHideXGridLines());
        }
        #endregion

        #region Internal Methods
        internal void AddMinimizedPanel(ChartPanel panel)
        {
            paineisBar.AddPanel(panel);
            SetPanelsBarVisibility();
        }

        internal void DeleteMinimizedPanel(ChartPanel panel)
        {
            paineisBar.DeletePanel(panel);
            SetPanelsBarVisibility();
        }
        
        public void ResizePanels(PanelsContainer.ResizeType tipoResize)
        {
            try
            {
                paineisContainers.ResizePanels(tipoResize);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        internal double GetXPixel(double index, bool offscreen)
        {
            if (!templateLoaded) return 0;

            if (index < 0 && !offscreen) return 0;

            if (estiloPreco != EnumGeral.EstiloPrecoEnum.Padrao && estiloPreco != EnumGeral.EstiloPrecoEnum.HeikinAshi && index >= 0 && index < xMap.Length)
            {
                if (xMap[(int)index] > 0)
                    return xMap[(int)index];
            }

            return PaintableWidth * index / VisibleRecordCount + EspacoEsquerdaGrafico - espacamentoBarra;
        }

        /// <summary>
        /// Gets record by pixel value
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public double GetReverseXInternal(double pixel)
        {
            pixel -= EspacoEsquerdaGrafico;

            if (estiloPreco != EnumGeral.EstiloPrecoEnum.Padrao && estiloPreco != EnumGeral.EstiloPrecoEnum.HeikinAshi && xMap.Length > 0)
            {
                for (int i = 0; i < xMap.Length; i++)
                {
                    double v1 = 0;
                    if (i > 0) v1 = xMap[i - 1];
                    double v2 = xMap[i];
                    if (i == 0 && pixel <= v2) return Math.Floor(i + 0.5);
                    if (pixel >= v1 && pixel <= v2) return Math.Floor(i + 0.5);
                }
                return -1;
            }

            return (pixel + (espacamentoBarra)) * VisibleRecordCount / PaintableWidth;
        }


        internal void DoActionOnPanels(Action<ChartPanel> action)
        {
            paineisContainers.Panels.ForEach(action);
        }

        internal void InvalidateIndicators()
        {
            foreach (ChartPanel chartPanel in paineisContainers.Panels)
            {
                foreach (Indicator indicator in chartPanel.IndicatorsCollection)
                {
                    indicator._calculated = false;
                }
            }
        }

        internal StatusGrafico Status
        {
            get { return status; }
            set
            {
                if (status == value) return;
                status = value;
                switch (status)
                {
                    case StatusGrafico.PreparadoParaPintarLinhaEstudo:
                        //            Debug.Assert(_currentPanel != null);
                        Mouse.OverrideCursor =
#if WPF
              Cursors.Pen;
#endif
#if SILVERLIGHT
 Cursors.Stylus;
#endif
                        break;
                    case StatusGrafico.Preparado:
                        Mouse.OverrideCursor = Cursors.Arrow;
                        if (painelAtual != null) painelAtual.Cursor = Cursors.Arrow;
                        break;
                    case StatusGrafico.MovendoSelecao:
                        Mouse.OverrideCursor = Cursors.Hand;
                        break;
                }
            }
        }

        internal bool ReCalc
        {
            get { return recalc; }
            set
            {
                recalc = value;
                paineisContainers.Panels.ForEach(panel => panel._recalc = value);
            }
        }

        internal void UpdateByTimer()
        {
            _timers.StartTimerWork(TimerUpdate);
        }


#if SILVERLIGHT
        private readonly List<string> AllowedIPs = new List<string>
                                        {
                                          "localhost",
                                          "platform.Traderdata.Client.Componente.GraficoSL.StockChart.com",
                                          "myfno.com"
                                        };
#endif
        //private static readonly DateTime _expirationDate = new DateTime(2008, 10, 19);
        internal bool CheckRegistration()
        {
            //in Design-Mode always return true
            if (Utils.GetIsInDesignMode(this))
                return true;
#if WPF
#if PERSONAL
      if (_registered.HasValue) return _registered.Value;
      _registered = IsLicenseValid();

      return _registered.Value;
#elif DEMO
      try
      {
        if (!_registered.HasValue)
        {
          string assemblyLocation = Assembly.GetAssembly(typeof (StockChartX)).Location;
          string directory = Path.GetDirectoryName(assemblyLocation);
          string s = Security.LicenseChecker.I.CheckLicense(directory + "\\DemoClient.lix");

          Debug.WriteLine("License info: " + s);

          _demoText = s;

          return (_registered = (s.Length > 0)).Value;
        }
        return _registered.Value;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Corrupt license information. \r\n Extended error: \r\n" + ex, 
        	"Error", MessageBoxButton.OK, MessageBoxImage.Error);
        _registered = false;
        return false;
      }
#else
      //return DateTime.Now <= _expirationDate;
      return true;
#endif
#endif
#if SILVERLIGHT
            //if you can see this it means you have source code, so change this checking as you want.
            string host = Application.Current.Host.Source.Host.ToLower();
            foreach (string h in AllowedIPs)
            {
                if (h.ToLower() == host)
                    return true;
            }
            return true;
#endif
        }

        /// <summary>
        /// returns a brush for a specified by index candle. this function is used toghether with BarColor public function.
        /// </summary>
        /// <param name="barIndex"></param>
        /// <param name="defaultBrush"></param>
        /// <returns></returns>
        internal Brush GetBarBrush(int barIndex, Brush defaultBrush)
        {
            BarBrushData barBrush;
            return barBrushes.TryGetValue(barIndex, out barBrush) ? (barBrush.Brush ?? defaultBrush) : defaultBrush;
        }

        internal bool GetBarBrushChanged(int barIndex)
        {
            BarBrushData data;
            return barBrushes.TryGetValue(barIndex, out data) ? data.Changed : false;
        }

        internal void SetBarBrushChanged(int barIndex, bool newValue)
        {
            if (barBrushes.ContainsKey(barIndex))
                barBrushes[barIndex].Changed = newValue;
        }

        internal double GetMax(Series series, bool ignoreZero, bool onlyVisible)
        {
            double max = double.MinValue;
            int start = 0;
            int count = series.RecordCount;
            if (onlyVisible)
            {
                start = indexInicial;
                count = indexFinal;
            }
            for (int i = start; i < count; i++)
            {
                if (ignoreZero)
                {
                    if (series[i].Value > max && series[i].Value.Value != 0.0)
                        max = series[i].Value.Value;
                }
                else
                {
                    if (series[i].Value > max)
                        max = series[i].Value.Value;
                }
            }
            return max;
        }
        internal double GetMax(Series series, bool ignoreZero)
        {
            return GetMax(series, ignoreZero, false);
        }

        internal Rect AbsoluteRect(FrameworkElement element)
        {
            //Linha adicionada pois dá erro ao esconder o objeto
            if ((element.ActualHeight == 0) && (element.ActualWidth == 0))
                return new Rect(0, 0, 0, 0);
            //--------------------------------------------------
            
            GeneralTransform generalTransform =
#if WPF
        element.TransformToAncestor(this); 
#endif
#if SILVERLIGHT
 element.TransformToVisual(this);
#endif
            Point location = generalTransform.Transform(new Point(0, 0));
            return new Rect(location.X, location.Y, element.ActualWidth, element.ActualHeight);
        }

        #endregion

        public void StopTimerCrossHair()
        {
            _timers.StopTimerWork(TimerCrossHairs);
        }

        public void StartTimerCrossHair()
        {
            _timers.StartTimerWork(TimerCrossHairs);
        }

        public void StopTimerInfoPanel()
        {
            _timers.StopTimerWork(TimerInfoPanel);
        }

        public void StartTimerInfoPanel()
        {
            _timers.StartTimerWork(TimerInfoPanel);
        }

        private static bool IsLicenseValid()
        {
            // the personal license must always check for the license 
            // file and is limited to only three installations:
#if PERSONAL
      // Personal license must check the license file
      // because it can only run on this computer

      //Bios date
      string biosDate = Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System", "SystemBiosDate", "").ToString();

      //Central processor 0 Identifier
      string processorId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "Identifier", "").ToString();

      string id = biosDate + processorId;
      if (string.IsNullOrEmpty(id))
        id = "modulus-bt8-keycode"; //failsafe

      string temp = "";
      const string key = "bt8";

      // Encode or decode and extract letters
      for (int i = 0; i < id.Length; i++)
      {
        int c = key[i % key.Length] ^ id[i];
        if ((c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122))
          temp += (char)c;
      }
      id = temp;

      // Now create the encoded hardware id
      string hid = id;
      temp = "";
      for (int i = 0; i < id.Length; i++)
      {
        char c = hid[i];
        temp += (c >= 48 && c < 57) || (c >= 65 && c < 90) || (c >= 97 && c < 121) ? (char)(c + 1) : c;
      }
      string encId = temp;

      // Get the path of the Traderdata.Client.Componente.GraficoSL.StockChart.StockChartX.dll assembly file
      string assemblyPath = Assembly.GetExecutingAssembly().Location;
      if (string.IsNullOrEmpty(assemblyPath))
      {
        MessageBox.Show("Critical error. Could not find assembly path.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
        return false;                        
      }
      FileInfo assemblyInfo = new FileInfo(assemblyPath);
      FileInfo licenseInfo = new FileInfo(assemblyInfo.DirectoryName + @"\StockChartX.WPF.plc");
      string licenseText;
      try
      {
        licenseText = licenseInfo.OpenText().ReadLine();  
      }
      catch
      {
        licenseText = "";
      }
      
      if (licenseText != encId)
      {
        MessageBox.Show("Development license missing or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return false;
      }

      return true;
#else
            return true;
#endif
        }

        internal void GetLineStudyToAdd(ChartPanel chartPanel)
        {
            painelAtual = chartPanel;

            chartPanel._lineStudyToAdd = linhasEstudoParaAdicionar;
            linhasEstudoParaAdicionar.SetChartPanel(chartPanel);
        }

        private void ShowHideChartScroller()
        {
            if (_chartScroller == null) return;
            if (ChartScrollerVisible)
            {
                _chartScroller.Visibility = Visibility.Visible;
                rootGrid.RowDefinitions[3].Height = new GridLength(15);
            }
            else
            {
                _chartScroller.Visibility = Visibility.Collapsed;
                rootGrid.RowDefinitions[3].Height = new GridLength(0);
            }
        }

        internal void ShowLineStudyContextMenu(Point position, ContextLine contextLine)
        {
            paineisContainers._lineStudyContextMenu.ContextLine = contextLine;
            paineisContainers._lineStudyContextMenu.Show(position);
        }

        internal LineStudyContextMenu LsContextMenu
        {
            get { return paineisContainers._lineStudyContextMenu; }
        }

        #region Novos campos e propriedades

        private bool disparoOntituloIndicador = true;

        /// <summary>Habilita ou desabilita o evento de disparo de Clique no Título do Indicador.</summary>
        public bool DisparoOntituloIndicador
        {
            get { return disparoOntituloIndicador; }
            set { disparoOntituloIndicador = value; }
        }

        private bool disparoDuploCliqueScrollZoom = true;

        /// <summary>Habilita ou desabilita o evento de disparo de Duplo Clique no Scroll Zoom.</summary>
        public bool DisparoDuploCliqueScrollZoom
        {
            get { return disparoDuploCliqueScrollZoom; }
            set { disparoDuploCliqueScrollZoom = value; }
        }

        #endregion Novos campos e propriedades

        #region MétodosNovos

        
        #endregion MétodosNovos

        #region Eventos Novos

        /// <summary>
        /// Evento disparado quando há um duplo clique no scroll.
        /// </summary>
        private void _chartScroller_DuploCliqueScroll()
        {
            if (DisparoDuploCliqueScrollZoom)
            {
                ResetZoom();
                ResetYScale(0);
                FireChartScroll();
            }
        }

        /// <summary>
        /// Evento disparado quando clicamos no titulo do indicador no painel.
        /// </summary>
        /// <param name="serie"></param>
        public void DisparaOnTituloIndicador(Series serie)
        {
            if (DisparoOntituloIndicador)
                OnTituloIndicador(serie);
        }

        /// <summary>
        /// Clique esquerdo no indicador
        /// Deve desmarcar os outros indicadores
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockChartX_IndicatorLeftClick(object sender, StockChartX.IndicatorDoubleClickEventArgs e)
        {
            //Desmcarcando outros indicadores
            foreach (object indicador in IndicatorsCollection)
            {
                if ((indicador is Indicator) && (!((Indicator)indicador).Equals(e.Indicator)))
                    ((Indicator)indicador).HideSelection();
            }
        }

        /// <summary>
        /// Clique esquerdo na linha de estudo.
        /// Desmarca as outras linhas de estudos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockChartX_LineStudyLeftClick(object sender, StockChartX.LineStudyMouseEventArgs e)
        {
            //Desmcarcando outras linhas
            foreach (object objeto in LineStudiesCollection)
            {
                if ((objeto is LineStudy) && (!((LineStudy)objeto).Equals(e.LineStudy)))
                    ((LineStudy)objeto).Selected = false;
            }
        }

        #endregion Eventos Novos
    }
}

