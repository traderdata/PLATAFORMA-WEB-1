using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Exceptions;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.Enum;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
using FrameworkElement=System.Windows.FrameworkElement;

#endif
#if WPF
using System.Windows.Input;
using System.Globalization;
#endif

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    /// <summary>
    /// Represents the chart. It works as a container for all panels.
    /// </summary>
    public partial class StockChartX
    {
        #region Properties
#if SILVERLIGHT
        /// <summary>
        /// This is required to be set, cause it's used internaly
        /// </summary>
        private FrameworkElement _parent;
        public Panel AppRoot
        {
            get
            {
                if (_parent == null)
                {
                    _parent = (FrameworkElement)Parent;
                    while (!(_parent is Panel))
                    {
                        _parent = (FrameworkElement)_parent.Parent;
                    }
                }
                if (_parent == null)
                    throw new ArgumentNullException("");

                return (Panel)_parent;
            }
            set { }
        }
        //    public Panel AppRoot
        //    {
        //      get; set;
        //    }
#endif

        /// <summary>
        /// Gets or sets the main symbol that is used in the chart. i.e. MSFT, DELL, ...
        /// </summary>
        public string Symbol { get; set; }
                
        ///<summary>
        /// Gets font name used to paint Y grid, calendar text
        ///</summary>
        public string FontFace
        {
            get { return fontFamily; }
            set
            {
                if (fontFamily == value) return;
                fontFamily = value;
            }
        }

        ///<summary>
        /// Gets font size used to paint Y grid, calendar text
        ///</summary>
        public new double FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }

        ///<summary>
        /// Gets font foreground used to paint Y grid, calendar text
        ///</summary>
        public Brush FontForeground
        {
            get { return fontForeground; }
            set { fontForeground = value; }
        }

        ///<summary>
        /// Gets or sets the maximum visible record count that are currently visible in the chart
        ///</summary>
        public int VisibleRecordCount
        {
            get { return indexFinal - indexInicial; }
        }

        ///<summary>
        /// Gets record count that are currently stored in the chart
        ///</summary>
        public int RecordCount
        {
            get { return dataManager.RecordCount; }
        }

        ///<summary>
        /// Gets paintable width of panel that is used to paint series. 
        ///</summary>
        public double PaintableWidth
        {
            get
            {
                double width = ActualWidth;
                if (escalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Ambas || escalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda)
                    width -= Constants.YAxisWidth;
                if (escalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Ambas || escalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Direita)
                    width -= Constants.YAxisWidth;
                width -= espacoEsquerdaGrafico;
                width -= espacoDireitaGrafico;

                return width;
            }
        }

        ///<summary>
        /// Gets or sets price style that is currently used in the chart
        ///</summary>
        public EnumGeral.EstiloPrecoEnum PriceStyle
        {
            get { return estiloPreco; }
            set { estiloPreco = value; }
        }

        ///<summary>
        /// Show or hide the Darvas boxes
        ///</summary>
        public bool DarvasBoxes
        {
            get { return darwasBoxes; }
            set
            {
                if (value == darwasBoxes) return;
                darwasBoxes = value;
                Update();
            }
        }

        ///<summary>
        /// Gets or sets darvas boxes stop percent
        ///</summary>
        public double DarvasStopPercent
        {
            get { return darvasPercent; }
            set
            {
                if (value == darvasPercent) return;
                darvasPercent = value;
                Update();
            }
        }

        ///<summary>
        /// Gets panels count used in the chart
        ///</summary>
        public int PanelsCount
        {
            get { return paineisContainers.Panels.Count; }
        }

        ///<summary>
        /// Gets reference to panel that is currently maximized, or null if there isn't such a panel
        ///</summary>
        public ChartPanel MaximizedPanel
        {
            get { return paineisContainers.painelMaximizado; }
        }

        ///<summary>
        /// Gets the collection of all indicators from all panels
        ///</summary>
        public IEnumerable<Indicator> IndicatorsCollection
        {
            get
            {
                foreach (ChartPanel chartPanel in paineisContainers.Panels)
                {
                    foreach (Indicator indicator in chartPanel.IndicatorsCollection)
                    {
                        yield return indicator;
                    }
                }
            }
        }

        /// <summary>
        /// Get the collection of all series from all panels 
        /// </summary>
        public IEnumerable<Series> SeriesCollection
        {
            get
            {
                foreach (ChartPanel chartPanel in paineisContainers.Panels)
                {
                    foreach (Series series in chartPanel.SeriesCollection)
                    {
                        yield return series;
                    }
                }
            }
        }

        ///<summary>
        /// Gets the collection of all LineStudies from all panels
        ///</summary>
        public IEnumerable<LineStudy> LineStudiesCollection
        {
            get
            {
                foreach (ChartPanel chartPanel in paineisContainers.Panels)
                {
                    foreach (LineStudy lineStudy in chartPanel._lineStudies)
                    {
                        yield return lineStudy;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of panels ignoring panels with HeatMap on them
        /// </summary>
        public int UseablePanelsCount
        {
            get
            {
                int count = 0;
                foreach (ChartPanel panel in paineisContainers.Panels)
                {
                    if (panel.IsHeatMap) continue;
                    count++;
                }
                return count;
            }
        }

        ///<summary>
        /// Gets or sets chart type
        ///</summary>
        public EnumGeral.TipoGraficoEnum ChartType
        {
            get { return tipoGrafico; }
            set
            {
                if (tipoGrafico == value) return;
                tipoGrafico = value;
            }
        }

        ///<summary>
        /// Gets or sets bar width used ti paint the wick of candles
        ///</summary>
        public double BarWidth
        {
            get { return larguraBarra; }
            set
            {
                if (larguraBarra == value || value < 1) return;
                larguraBarra = value;
            }
        }

        ///<summary>
        /// When UseLineSeriesUpDownColors is set to True, StockChartX will display the UpColor for values of oscillators that are above 0, and DownColor for values of oscillators that are below 0. 
        ///</summary>
        public bool UseLineSeriesUpDownColors
        {
            get { return usarCoresLinhasSeries; }
            set
            {
                if (value == usarCoresLinhasSeries) return;
                usarCoresLinhasSeries = value;
            }
        }

        ///<summary>
        /// When UseVolumeUpDownColors is set to True, StockChartX will display the UpColor of the symbol's up candle color, and DownColor for symbol's down candle color. The volume series must be named as part of a symbol group (e.g "MSFT.volume").
        ///</summary>
        public bool UseVolumeUpDownColors
        {
            get { return usarCoresAltaBaixaParaVolume; }
            set
            {
                if (value == usarCoresAltaBaixaParaVolume) return;
                usarCoresAltaBaixaParaVolume = value;
            }
        }

        ///<summary>
        /// Gets or sets the candle outline color (for hollow 2D candles). When the close is lower than the previous close, this color will be used to paint the bar outline. 
        ///</summary>
        public Color? CandleUpOutlineColor
        {
            get { return corContornoCandleAlta; }
            set
            {
                if (value == corContornoCandleAlta) return;
                corContornoCandleAlta = value;
            }
        }

        ///<summary>
        /// Gets or sets the candle outline color (for hollow 2D candles). When the close is lower than the previous close, this color will be used to paint the bar outline.
        ///</summary>
        public Color? CandleDownOutlineColor
        {
            get { return corContornoCandleBaixa; }
            set
            {
                if (value == corContornoCandleBaixa) return;
                corContornoCandleBaixa = value;
            }
        }

        /// <summary>
        /// Gets the record number of the first visible record on the chart. This value may change as the chart is zoomed or scrolled.
        /// </summary>
        public int FirstVisibleRecord
        {
            get { return indexInicial; }
            set
            {
                if (value == indexInicial) return;
                if (value >= RecordCount || value >= indexFinal) return;
                indexInicial = value;
                Update();
            }
        }

        /// <summary>
        /// Gets the record number of the last visible record on the chart. This value may change as the chart is zoomed or scrolled. 
        /// </summary>
        public int LastVisibleRecord
        {
            get { return indexFinal; }
            set
            {
                if (value == indexFinal) return;
                //Alterado em 10/09/2011 de 
                //if (value <= indexInicial || value >= RecordCount) return;
                //para
                if (value <= indexInicial || value > RecordCount) return;
                indexFinal = value;
                Update();
            }
        }

        /// <summary>
        /// Gets the selected objects from all panels
        /// </summary>
        public List<object> SelectedObjectsCollection
        {
            get
            {
                List<object> ret = new List<object>();
                foreach (ChartPanel chartPanel in paineisContainers.Panels)
                {
                    if (chartPanel._seriesSelected != null && chartPanel._seriesSelected.Selected)
                        ret.Add(chartPanel._seriesSelected);
                    if (chartPanel._lineStudySelected != null && chartPanel._lineStudySelected.Selected)
                        ret.Add(chartPanel._lineStudySelected);
                }

                return ret;
            }
        }

        ///<summary>
        /// Gets the collection of panels from the chart
        ///</summary>
        public IEnumerable<ChartPanel> PanelsCollection
        {
            get
            {
                if (paineisContainers == null)
                    yield break;

                foreach (ChartPanel chartPanel in paineisContainers.Panels)
                {
                    yield return chartPanel;
                }
            }
        }

        ///<summary>
        /// 
        ///</summary>
        public TextBlock LabelTitle
        {
            get
            {
                return textoLabelTitulo;
            }
        }

        ///<summary>
        ///</summary>
        public bool OptimizePainting { get; set; }
                
        #endregion



        #region Methods

        private bool _frozen;

        ///<summary>
        /// Makes chart do not update itself when changing public visual properties, such as FirstVisibleRecord
        ///</summary>
        public void Freeze()
        {
            _frozen = true;
        }

        ///<summary>
        /// Makes chart to update itself
        ///</summary>
        public void Melt()
        {
            _frozen = false;
        }

        /// <summary>
        /// Forces to invalidate the chart
        /// </summary>
        public void Update()
        {
            _timers.StopTimerWork(TimerUpdate);


            if (_frozen || status != StatusGrafico.Preparado || RecordCount == 0) return;

            if (!CheckRegistration())
            {
                MessageBox.Show("Registration for current version expired or invalid." + Environment.NewLine +
                                "Please contact support@Traderdata.Client.Componente.GraficoSL.StockChart.com for help.",
                                "Registration expired or invalid", MessageBoxButton.OK);
                return;
            }


            if (Symbol.Length == 0)
                throw new SymbolNotSetException();

            if (indexInicial > RecordCount - 1)
                indexInicial = 0;

            if (!_scrollerUpdating && _chartScroller != null)
            {
                _chartScroller.Freeze();
                _chartScroller.MaxValue = RecordCount;
                _chartScroller.LeftValue = indexInicial;
                _chartScroller.RightValue = indexFinal;
                _chartScroller.Melt();
            }

            if (ReCalc)
                InvalidateIndicators();

#if WPF
#if DEMO
      _calendar._demoText = _demoText;
#endif
#endif
            calendario.Paint();

            paineisContainers.ResetPanels();
        }

        /// <summary>
        /// Adds an indicator to a specified panel
        /// </summary>
        /// <param name="indicatorType">Indicator type</param>
        /// <param name="key">An unique key for indicator</param>
        /// <param name="chartPanel">a valid reference to a panel</param>
        /// <param name="userParams">true - the indicator parameters will be set by code
        /// false - indicator will show a dialog where user will choose its parameters</param>
        /// <returns>a reference to an indicator object</returns>
        public Indicator AddIndicator(EnumGeral.IndicatorType indicatorType, string key, ChartPanel chartPanel,
          bool userParams)
        {
            if (GetSeriesByName(key) != null)
                throw new KeyNotUniqueException(key);

            Indicator res =
              (Indicator)Activator.CreateInstance(StockChartX_IndicatorsParameters.GetIndicatorCLRType(indicatorType),
                                                  new object[] { key, chartPanel });

            if (res != null)
            {
                res.UserParams = userParams;
                res._toBeAdded = true;

                ReCalc = true;
                changed = true;
                chartPanel.AddSeries(res);
                return res;
            }
            throw new InvalidIndicatorException(indicatorType);
        }

        private readonly string[] OverlayIndicatorNames
          = new[]
          {
            "PARABOLIC", "PSAR", "FORECAST", "INTERCEPT",
            "WEIGHTED CLOSE", "TYPICAL PRICE", "WEIGHTED PRICE",
            "MEDIAN PRICE", "SMOOTHING", "BOLLINGER",
            "MOVING AVERAGE", "BANDS"
          };

        ///<summary>
        /// By indicator name suggests eother to create a new panel or not. Some indicators have values way different then series and it's recomended to create different panels for them.
        ///</summary>
        ///<param name="indicatorType">Indicator type</param>
        ///<returns>true - it is recomended to create a new panel for indicator.</returns>
        public bool IsOverlayIndicator(EnumGeral.IndicatorType indicatorType)
        {
            string indicatorName = StockChartX_IndicatorsParameters.GetIndicatorName(indicatorType).ToUpper();
            foreach (string overlayIndicatorName in OverlayIndicatorNames)
            {
                if (indicatorName.Contains(overlayIndicatorName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Initiates a line study painted by code
        /// </summary>
        /// <param name="studyTypeEnum">Study type</param>
        /// <param name="key">Unique key</param>
        /// <param name="stroke">Brush used to paint the lines</param>
        /// <param name="panelIndex">Panel index where to place line study</param>
        /// <param name="args">mainly used for ImageObject when setting image path</param>
        /// <returns>Reference to newly created line study</returns>
        public LineStudy AddLineStudy(LineStudy.StudyTypeEnum studyTypeEnum, string key, Brush stroke, int panelIndex, params object[] args)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);

            chartPanel._lineStudyToAdd =
              (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(studyTypeEnum),
                                                  new object[] { key, stroke, chartPanel });
            chartPanel._lineStudyToAdd.SetArgs(args);

            painelAtual = chartPanel;
            Status = StatusGrafico.PreparadoParaPintarLinhaEstudo;

            return chartPanel._lineStudyToAdd;
        }

        /// <summary>
        /// Initiates a line study painted by user
        /// </summary>
        /// <param name="studyTypeEnum">Study type</param>
        /// <param name="key">Unique key</param>
        /// <param name="stroke">Brush used to paint the lines</param>
        /// <param name="args">mainly used for ImageObject when setting image path</param>
        /// <returns>Reference to newly created line study</returns>
        public LineStudy AddLineStudy(LineStudy.StudyTypeEnum studyTypeEnum, string key, Brush stroke, params object[] args)
        {
            painelAtual = null; //will set in panel where user will click

            linhasEstudoParaAdicionar = (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(studyTypeEnum),
                                                  new object[] { key, stroke, null });
            linhasEstudoParaAdicionar.SetArgs(args);

            Status = StatusGrafico.PreparadoParaPintarLinhaEstudo;

            return linhasEstudoParaAdicionar;
        }

        public void ResetaStatus()
        {
            Status = StatusGrafico.Preparado;
        }

        /// <summary>
        /// Initiates a line study painted by user
        /// </summary>
        public LineStudy AddLineStudySemUsuario(LineStudy linha, params object[] args)
        {
            ChartPanel chartPanel = linha._chartPanel;

            LineStudy linhaASerInserida =
              (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(linha.StudyType),
                                                  new object[] { linha.Key, linha.Stroke, chartPanel });
            linhaASerInserida.SetArgs(args);

            painelAtual = chartPanel;

            linhaASerInserida.Stroke = linha.Stroke;
            linhaASerInserida.StrokeThickness = linha.StrokeThickness;
            linhaASerInserida.StrokeType = linha.StrokeType;
            
            if (linhaASerInserida.StudyType == LineStudy.StudyTypeEnum.ErrorChannel)
                ((ErrorChannel)linhaASerInserida).SetaParametros(((ErrorChannel)linha).ObtemParametro());
            else
                ((FibonacciRetracements)linhaASerInserida).SetaParametros(((FibonacciRetracements)linha).ObtemParametro());


            chartPanel.AdicionaLinhaEstudoSemUsuario(linhaASerInserida);

            return linhaASerInserida;
        }

        /// <summary>
        /// Adds a static text and lets user position it at needed position
        /// </summary>
        /// <param name="staticText">A user defined text</param>
        /// <param name="key">Unique key</param>
        /// <param name="foreground">Foreground Brush</param>
        /// <param name="fontSize">Font size</param>
        /// <param name="panelIndex">Panel index where to place the text</param>
        /// <returns>Reference to <seealso cref="StaticText"/> object</returns>
        public StaticText AddStaticText(string staticText, string key, Brush foreground, double fontSize, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);

            StaticText lineStaticText =
              (StaticText)Activator.CreateInstance(typeof(StaticText), new object[] { key, foreground, chartPanel });
            lineStaticText.SetArgs(new object[] { staticText });
            lineStaticText.StrokeThickness = fontSize;

            painelAtual = chartPanel;

            if (Status == StatusGrafico.Preparado)
                lineStaticText.Paint(0, 0, LineStudy.LineStatus.StartPaint);
            chartPanel._lineStudies.Add(lineStaticText);

            return lineStaticText;
        }

        /// <summary>
        /// Initiates a symbol object placed by user by  user
        /// </summary>
        /// <param name="symbolType">Symbol type</param>
        /// <param name="key">Unique key</param>
        /// <param name="panelIndex">Panel index where to place the symbol object</param>
        public void AddSymbolObject(EnumGeral.TipoSimbolo symbolType, string key, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            chartPanel._lineStudyToAdd =
              (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(LineStudy.StudyTypeEnum.ImageObject),
                                                  new object[] { key, Brushes.Transparent, chartPanel });
            chartPanel._lineStudyToAdd.SetArgs(symbolType);

            painelAtual = chartPanel;
            Status = StatusGrafico.PreparadoParaPintarLinhaEstudo;
        }

        ///<summary>
        /// Initiates a symbol object placed by user by  user
        ///</summary>
        /// <param name="symbolType">Symbol type</param>
        /// <param name="key">Unique key</param>
        public void AddSymbolObject(EnumGeral.TipoSimbolo symbolType, string key)
        {
            painelAtual = null;

            linhasEstudoParaAdicionar =
              (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(LineStudy.StudyTypeEnum.ImageObject),
                                       new object[] { key, Brushes.Transparent, null });

            linhasEstudoParaAdicionar.SetArgs(symbolType);

            Status = StatusGrafico.PreparadoParaPintarLinhaEstudo;
        }

        /// <summary>
        /// Adds a line study programmatically
        /// </summary>
        /// <param name="stroke">Brush used to paint line study</param>
        /// <param name="studyTypeEnum">Study type</param>
        /// <param name="key">Unique key</param>
        /// <param name="panelIndex">Panel index where to place line study</param>
        /// <returns>A reference to the line study created</returns>
        public LineStudy CreateLineStudy(LineStudy.StudyTypeEnum studyTypeEnum, string key, Brush stroke, int panelIndex)
        {
            return CreateLineStudy(studyTypeEnum, key, stroke, panelIndex, null);
        }

        /// <summary>
        /// Adds a line study programmatically
        /// </summary>
        /// <param name="stroke">Brush used to paint line study</param>
        /// <param name="studyTypeEnum">Study type</param>
        /// <param name="key">Unique key</param>
        /// <param name="panelIndex">Panel index where to place line study</param>
        /// <param name="args">Optional parameters to be passed to LineStudy</param>
        /// <returns>A reference to the line study created</returns>
        public LineStudy CreateLineStudy(LineStudy.StudyTypeEnum studyTypeEnum, string key, Brush stroke, int panelIndex, object[] args)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);

            LineStudy lineStudy =
              (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(studyTypeEnum),
                                                  new object[] { key, stroke, chartPanel });
            if (args != null)
                lineStudy.SetArgs(args);

            if (Status == StatusGrafico.Preparado)
                lineStudy.Paint(0, 0, LineStudy.LineStatus.StartPaint);
            chartPanel._lineStudies.Add(lineStudy);

            return lineStudy;
        }

        /// <summary>
        /// Adds a symbol object programmatically
        /// </summary>
        /// <param name="symbolType">Symbol type</param>
        /// <param name="key">Unique key</param>
        /// <param name="panelIndex">Panel index where to place symbol object</param>
        /// <returns>Reference to symbol object created.</returns>
        public LineStudy CreateSymbolObject(EnumGeral.TipoSimbolo symbolType, string key, int panelIndex, Size size)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);

            LineStudy lineStudy =
              (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(LineStudy.StudyTypeEnum.ImageObject),
                                                  new object[] { key, Brushes.Transparent, chartPanel });
            lineStudy.SetArgs(symbolType, size);

            if (Status == StatusGrafico.Preparado)
                lineStudy.Paint(0, 0, LineStudy.LineStatus.StartPaint);
            chartPanel._lineStudies.Add(lineStudy);

            return lineStudy;
        }

        /// <summary>
        /// Gets the width of a text including white spaces using current font properties
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Width</returns>
        public double GetTextWidth(string text)
        {
#if WPF
      return new FormattedText(text, CultureInfo.CurrentCulture,
          FlowDirection.LeftToRight, new Typeface(FontFace), FontSize, Brushes.Black).WidthIncludingTrailingWhitespace;
#endif
#if SILVERLIGHT
            TextBlock tb = new TextBlock { FontFamily = new FontFamily(FontFace), FontSize = FontSize, Text = text };
            //tb.UpdateLayout();
            return tb.ActualWidth;
#endif
        }


        /// <summary>
        /// Gets the height of a text using current font properties
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Height</returns>
        public double GetTextHeight(string text)
        {
#if WPF
      return new FormattedText(text, CultureInfo.CurrentCulture,
          FlowDirection.LeftToRight, new Typeface(FontFace), FontSize, Brushes.Black).Height;
#endif
#if SILVERLIGHT
            TextBlock tb = new TextBlock { FontFamily = new FontFamily(FontFace), FontSize = FontSize, Text = text };
            tb.UpdateLayout();
            return tb.ActualHeight;
#endif
        }

        ///<summary>
        /// Gets a series from an OHLC group by a given OHLC type and another series from this group.
        ///</summary>
        ///<param name="series">Series base</param>
        ///<param name="seriesTypeOHLC">OHLC type</param>
        ///<returns>Reference to needed series or null</returns>
        public Series GetSeriesOHLCV(Series series, EnumGeral.TipoSerieOHLC seriesTypeOHLC)
        {
            foreach (ChartPanel chartPanel in paineisContainers.Panels)
            {
                Series s = chartPanel.GetSeriesOHLCV(series, seriesTypeOHLC);
                if (s != null) return s;
            }
            return null;
        }

        /// <summary>
        /// Gets maximum value for a series
        /// </summary>
        /// <param name="series">Reference to <seealso cref="Series"/></param>
        /// <returns>Maximum value</returns>
        public double? GetMaxValue(Series series)
        {
            return GetMax(series, false, false);
        }
        /// <summary>
        /// Gets maximum value for a series
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Maximum value or null if series doesn't exists</returns>
        public double? GetMaxValue(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : GetMaxValue(series);
        }
        /// <summary>
        /// Gets the maximum visible value for a series
        /// </summary>
        /// <param name="series">Reference to <seealso cref="Series"/></param>
        /// <returns>Maximum value from visible records</returns>
        public double? GetVisibleMaxValue(Series series)
        {
            return GetMax(series, false, true);
        }
        /// <summary>
        /// Gets the maximum visible value for a series
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Maximum value from visible records></returns>
        public double? GetVisibleMaxValue(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : GetVisibleMaxValue(series);
        }


        internal double GetMin(Series series, bool ignoreZero, bool onlyVisible)
        {
            double min = double.MaxValue;
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
                    if (series[i].Value < min && series[i].Value.Value != 0.0)
                        min = series[i].Value.Value;
                }
                else
                {
                    if (series[i].Value < min)
                        min = series[i].Value.Value;
                }
            }
            return min;
        }
        internal double GetMin(Series series, bool ignoreZero)
        {
            return GetMin(series, ignoreZero, false);
        }
        /// <summary>
        /// Gets minimum value for a series
        /// </summary>
        /// <param name="series">Reference to <seealso cref="Series"/></param>
        /// <returns>Miniumu value</returns>
        public double? GetMinValue(Series series)
        {
            return GetMin(series, false, false);
        }
        /// <summary>
        /// Gets minimum value for a series
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Minimum value</returns>
        public double? GetMinValue(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : GetMinValue(series);
        }
        /// <summary>
        /// Gets the minimum visible value for a series
        /// </summary>
        /// <param name="series">Reference to <seealso cref="Series"/></param>
        /// <returns>Minimum value from visible records</returns>
        public double? GetVisibleMinValue(Series series)
        {
            return GetMin(series, false, true);
        }
        /// <summary>
        /// Gets the minimum visible value for a series
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Minimum value from visible records</returns>
        public double? GetVisibleMinValue(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : GetVisibleMinValue(series);
        }

        ///<summary>
        /// Adds a new chart panel and Gets a reference to it. Position type is None
        ///</summary>
        ///<returns>Reference to the newly created chart panel</returns>
        public ChartPanel AddChartPanel()
        {
            return paineisContainers.AddPanel(ChartPanel.PositionType.None);
        }

        /// <summary>
        /// Remove o painel desejado do gráfico.
        /// </summary>
        /// <param name="painel"></param>
        public void RemoveChartPanel(ChartPanel painel)
        {
            paineisContainers.RemovePainel(painel);
        }

        ///<summary>
        /// Adds a chart panel with a specified type of positioning
        ///</summary>
        ///<param name="position">Position type</param>
        ///<returns>Reference to the newly created chart panel</returns>
        public ChartPanel AddChartPanel(ChartPanel.PositionType position)
        {
            return paineisContainers.AddPanel(position);
        }
        ///<summary>
        /// Adds a panel that will show the heat map panel. Such a panel can't hold any series or line studies.
        /// Only one instance of heat map can exists
        ///</summary>
        ///<returns>Reference to the newly created chart panel</returns>
        public ChartPanel AddHeatMapPanel()
        {
            //make sure we have only one heat map
            ChartPanel heatMap = PanelsCollection.FirstOrDefault(p => p.IsHeatMap);
            if (heatMap != null)
                return heatMap;

            heatMap = paineisContainers.AddPanel(ChartPanel.PositionType.None, true);
            Update();
            return heatMap;
        }

        ///<summary>
        /// Destroys the heap map panel
        ///</summary>
        public void DeleteHeatMap()
        {
            paineisContainers.CloseHeatMap();
        }

        ///<summary>
        /// Gets a reference to a series by its name or null if such series doesn't exists
        ///</summary>
        ///<param name="seriesName">Series name</param>
        /// <example>
        /// <code>
        /// Series seriesOpen = _stockChartX.GetSeriesByName(_stockChartX.Symbol + ".Abertura");
        /// </code>
        /// </example>
        ///<returns>Reference to series</returns>
        public Series GetSeriesByName(string seriesName)
        {
            foreach (ChartPanel panel in paineisContainers.Panels)
            {
                foreach (Series series in panel.AllSeriesCollection)
                {
                    if (Utils.StrICmp(seriesName, series.FullName))
                        return series;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a panel by its index. ignores panels that have heat map
        /// </summary>
        /// <param name="index">Panel Index</param>
        /// <returns>Reference to ChartPanel</returns>
        public ChartPanel GetPanelByIndex(int index)
        {
            if (index < paineisContainers.Panels.Count)
                return paineisContainers.Panels[index];
            else
                return null;

            //      for (int i = 0; i < _panelsContainer.Panels.Count; i++)
            //      {
            //        if (_panelsContainer.Panels[i].IsHeatMap) continue;
            //        if (index-- == 0)
            //          return _panelsContainer.Panels[i];
            //      }
            //      throw new IndexOutOfRangeException("index");
        }

        /// <summary>
        /// Gets the total number of indicator of a specified type
        /// </summary>
        /// <param name="indicatorType">Indicator type</param>
        /// <returns>Number of indicator that matches given indicator type</returns>
        public int GetIndicatorCountByType(EnumGeral.IndicatorType indicatorType)
        {
            int count = 0;
            foreach (var chartPanel in paineisContainers.Panels)
            {
                foreach (var indicator in chartPanel.IndicatorsCollection)
                {
                    if (indicator.IndicatorType == indicatorType)
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Gets the total number of line studies from the chart by its type
        /// </summary>
        /// <param name="studyTypeEnum">Study type</param>
        /// <returns>Number of line studies that matches the given study type</returns>
        public int GetLineStudyCountByType(LineStudy.StudyTypeEnum studyTypeEnum)
        {
            int count = 0;
            foreach (ChartPanel chartPanel in paineisContainers.Panels)
            {
                foreach (LineStudy study in chartPanel._lineStudies)
                {
                    if (study.StudyType == studyTypeEnum)
                        count++;
                }
            }
            return count;
        }

        ///<summary>
        /// Sets the parameter for a price style.
        ///</summary>
        ///<param name="index">Index of parameter</param>
        ///<param name="value">New value</param>
        /// <example>
        /// <code>
        /// _stockChartX.SetPriceStyleParam(0, 0); //Reversal size
        /// _stockChartX.SetPriceStyleParam(1, (double)StockChartX.ChartDataType.Points);
        /// </code>
        /// </example>
        public void SetPriceStyleParam(int index, double value)
        {
            parametrosEstiloPreco[index] = value;
        }

        /// <summary>
        /// Gets the value of a parameter for current price style
        /// </summary>
        /// <param name="index">Parameter index</param>
        /// <returns>Parameter's value</returns>
        public double GetPriceStyleParam(int index)
        {
            return parametrosEstiloPreco[index];
        }

        ///<summary>
        /// Return price style value 1 by record index
        ///</summary>
        ///<param name="recordIndex">Record index</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue1(int recordIndex)
        {
            return estiloPrecoValor1[recordIndex].Value;
        }
        ///<summary>
        /// Return price style value 1 by time stamp
        ///</summary>
        ///<param name="timeStamp">Timestamp</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue1(DateTime timeStamp)
        {
            int recordIndex = dataManager.GetTimeStampIndex(timeStamp);
            if (recordIndex == -1) return null;
            return estiloPrecoValor1[recordIndex].Value;
        }

        ///<summary>
        /// Return price style value 2 by record index
        ///</summary>
        ///<param name="recordIndex">Record Index</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue2(int recordIndex)
        {
            return estiloPrecoValor2[recordIndex].Value;
        }
        ///<summary>
        /// Return price style value 2 by time stamp
        ///</summary>
        ///<param name="timeStamp">TimeStamp</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue2(DateTime timeStamp)
        {
            int recordIndex = dataManager.GetTimeStampIndex(timeStamp);
            if (recordIndex == -1) return null;
            return estiloPrecoValor2[recordIndex].Value;
        }

        ///<summary>
        /// Return price style value 3 by record index
        ///</summary>
        ///<param name="recordIndex">Record index</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue3(int recordIndex)
        {
            return estiloPrecoValor3[recordIndex].Value;
        }
        ///<summary>
        /// Return price style value 3 by time stamp
        ///</summary>
        ///<param name="timeStamp">Time stamp</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue3(DateTime timeStamp)
        {
            int recordIndex = dataManager.GetTimeStampIndex(timeStamp);
            if (recordIndex == -1) return null;
            return estiloPrecoValor3[recordIndex].Value;
        }

        ///<summary>
        /// Gets an object type from current mouse position
        ///</summary>
        ///<param name="o">Reference to an object or null</param>
        ///<returns>Object's type</returns>
        public EnumGeral.ObjetoSobCursor GetObjectFromCursor(out object o)
        {
            //Linha adicionada pois dá erro ao fechar o gráfico
            try
            {
                o = null;
                Point pg = Mouse.GetPosition(this);
                if (AbsoluteRect(calendario).Contains(pg))
                {
                    o = calendario;
                    return EnumGeral.ObjetoSobCursor.Calendar;
                }

                if (AbsoluteRect(paineisBar).Contains(pg))
                {
                    o = paineisBar;
                    return EnumGeral.ObjetoSobCursor.PanelsBar;
                }

                Point p = Mouse.GetPosition(paineisContainers);
                ChartPanel chartPanel = paineisContainers.PanelByY(p.Y);
                if (chartPanel == null)
                    return EnumGeral.ObjetoSobCursor.NoObject;

                if (AbsoluteRect(chartPanel._leftYAxis).Contains(pg))
                {
                    o = chartPanel._leftYAxis;
                    return EnumGeral.ObjetoSobCursor.PanelLeftYAxis;
                }
                if (AbsoluteRect(chartPanel._rightYAxis).Contains(pg))
                {
                    o = chartPanel._rightYAxis;
                    return EnumGeral.ObjetoSobCursor.PanelRightYAxis;
                }
                if (AbsoluteRect(chartPanel._titleBar).Contains(pg))
                {
                    o = chartPanel._titleBar;
                    return EnumGeral.ObjetoSobCursor.PanelTitleBar;
                }

                GeneralTransform generalTransform =
#if WPF
        chartPanel._rootCanvas.TransformToAncestor(this);
#endif
#if SILVERLIGHT
 chartPanel._rootCanvas.TransformToVisual(this);
#endif
                Point location = generalTransform.Transform(new Point(0, 0));
                Rect rcCanvas = new Rect(location.X, location.Y, chartPanel._rootCanvas.ActualWidth, chartPanel._rootCanvas.ActualHeight);

                o = chartPanel;
                if (pg.X >= rcCanvas.Left && pg.X <= rcCanvas.Left + EspacoEsquerdaGrafico)
                    return EnumGeral.ObjetoSobCursor.PanelLeftNonPaintableArea;
                if (pg.X >= rcCanvas.Right - EspacoDireitaGrafico && pg.X <= rcCanvas.Right)
                    return EnumGeral.ObjetoSobCursor.PanelRightNonPaintableArea;

                return EnumGeral.ObjetoSobCursor.PanelPaintableArea;
            }
            catch
            {
                Point p = Mouse.GetPosition(paineisContainers);
                ChartPanel chartPanel = paineisContainers.PanelByY(p.Y);
                o = chartPanel;
                return EnumGeral.ObjetoSobCursor.PanelPaintableArea;
            }
        }

        /// <summary>
        /// deletes all panels and everyting related to them
        /// </summary>
        public void ClearAll()
        {
            status = StatusGrafico.Construindo;
            if (paineisContainers != null)
                paineisContainers.ClearAll();
            xMap = new double[0];
            xCount = 0;
            xGridMap.Clear();
            indexInicial = indexFinal = 0;
            dataManager.ClearAll();
            barBrushes.Clear();

            FireChartReseted();
            status = StatusGrafico.Preparado;
        }


        public void LimpaGrafico(bool limpaIndicadores, bool limpaObjetos)
        {
            status = StatusGrafico.Construindo;

            if (paineisContainers != null)
                paineisContainers.LimpaGraficoSemDeletarPaineis(limpaIndicadores, limpaObjetos);

            xMap = new double[0];
            xCount = 0;
            xGridMap.Clear();
            indexInicial = indexFinal = 0;
            dataManager.ClearAll();
            barBrushes.Clear();

            FireChartReseted();
            status = StatusGrafico.Preparado;
        }

        /// <summary>
        /// Adds an OHLC group of series to the chart
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="panelIndex">Panel index where to place OHLC group of series</param>
        /// <returns>An array with length = 4 that containes references to newly create series</returns>
        public Series[] AddOHLCSeries(string groupName, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");
            dataManager.AddOHLCSeries(groupName);

            Series[] series = new Series[4];
            series[0] = chartPanel.CreateSeries(groupName, EnumGeral.TipoSerieOHLC.Abertura, EnumGeral.TipoSeriesEnum.Candle);
            series[1] = chartPanel.CreateSeries(groupName, EnumGeral.TipoSerieOHLC.Maximo, EnumGeral.TipoSeriesEnum.Candle);
            series[2] = chartPanel.CreateSeries(groupName, EnumGeral.TipoSerieOHLC.Minimo, EnumGeral.TipoSeriesEnum.Candle);
            series[3] = chartPanel.CreateSeries(groupName, EnumGeral.TipoSerieOHLC.Ultimo, EnumGeral.TipoSeriesEnum.Candle);
            foreach (var series1 in series)
            {
                dataManager.BindSeries(series1);
            }
            return series;
        }

        /// <summary>
        /// Adds an HLC group of series to the chart
        /// </summary>
        /// <param name="groupName">Group Name</param>
        /// <param name="panelIndex">Panel index where to place</param>
        /// <returns>An array with length = 3 that has references to all 3 series</returns>
        public Series[] AddHLCSeries(string groupName, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");

            dataManager.AddHLCSeries(groupName);

            Series[] series = new Series[3];
            series[0] = chartPanel.CreateSeries(groupName, EnumGeral.TipoSerieOHLC.Maximo, EnumGeral.TipoSeriesEnum.Candle);
            series[1] = chartPanel.CreateSeries(groupName, EnumGeral.TipoSerieOHLC.Minimo, EnumGeral.TipoSeriesEnum.Candle);
            series[2] = chartPanel.CreateSeries(groupName, EnumGeral.TipoSerieOHLC.Ultimo, EnumGeral.TipoSeriesEnum.Candle);
            foreach (var series1 in series)
            {
                dataManager.BindSeries(series1);
            }

            return series;
        }

        /// <summary>
        /// Adds volume type of series to the chart
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="panelIndex">Panel index where to place</param>
        /// <returns>Reference to volume series</returns>
        public Series AddVolumeSeries(string groupName, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");

            dataManager.AddSeries(groupName, EnumGeral.TipoSerieOHLC.Volume);

            Series series = chartPanel.CreateSeries(groupName, EnumGeral.TipoSerieOHLC.Volume, EnumGeral.TipoSeriesEnum.Volume);
            dataManager.BindSeries(series);
            return series;
        }

        /// <summary>
        /// Adds a line type of series to the chart
        /// </summary>
        /// <param name="symbolName">Symbol name. Series name get's created from symbol anme and SeriesOHLCType</param>
        /// <param name="panelIndex">Panel index where to place</param>
        /// <param name="ohlcType">OHLC type</param>
        /// <returns>Reference to the newly created series</returns>
        public Series AddLineSeries(string symbolName, int panelIndex, EnumGeral.TipoSerieOHLC ohlcType)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");

            dataManager.AddSeries(symbolName, ohlcType);

            Series series = chartPanel.CreateSeries(symbolName, ohlcType, EnumGeral.TipoSeriesEnum.Linha);
            dataManager.BindSeries(series);
            return series;
        }


        ///<summary>
        /// Adds a linear type of series with an arbitrary series name, not binded to the chart's symbol
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="panelIndex">Panel index where to place</param>
        ///<returns>Reference to the newly created series</returns>
        ///<exception cref="IndexOutOfRangeException"></exception>
        public Series AddSeries(string seriesName, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");

            dataManager.AddSeries(seriesName, EnumGeral.TipoSerieOHLC.Desconhecido);

            Series series = chartPanel.CreateSeries(seriesName, EnumGeral.TipoSerieOHLC.Desconhecido, EnumGeral.TipoSeriesEnum.Linha);
            dataManager.BindSeries(series);

            return series;
        }

        ///<summary>
        /// Appends values for an OHLC group
        ///</summary>
        ///<param name="symbolName">Symbol name. Usually main symbol</param>
        ///<param name="timeStamp">Value's timestamp</param>
        ///<param name="open">Open value</param>
        ///<param name="high">High value</param>
        ///<param name="low">Low value</param>
        ///<param name="close">Close value</param>
        public void AppendOHLCValues(string symbolName, DateTime timeStamp, double? open, double? high, double? low, double? close)
        {
            dataManager.AppendOHLCValues(symbolName, timeStamp, open, high, low, close);
        }

        ///<summary>
        /// Appends values for an HLC group
        ///</summary>
        ///<param name="groupName">Group name. Usually main symbol</param>
        ///<param name="timeStamp">Value's timestamp</param>
        ///<param name="high">High value</param>
        ///<param name="low">Low value</param>
        ///<param name="close">Close value</param>
        public void AppendHLCValues(string groupName, DateTime timeStamp, double? high, double? low, double? close)
        {
            dataManager.AppendHLCValues(groupName, timeStamp, high, low, close);
        }

        ///<summary>
        /// Appends Volume value to the chart
        ///</summary>
        ///<param name="groupName">Group name. Usually main symbol</param>
        ///<param name="timeStamp">Value's timestamp</param>
        ///<param name="volume">Volume value</param>
        public void AppendVolumeValue(string groupName, DateTime timeStamp, double? volume)
        {
            dataManager.AppendValue(groupName, EnumGeral.TipoSerieOHLC.Volume, timeStamp, volume);
        }

        /// <summary>
        /// Appends value for linear type of series. 
        /// </summary>
        /// <param name="symbolName">Symbol name. Series name gets created from symbol name and SeriesTypeOHLC</param>
        /// <param name="ohlcType">OHLC type</param>
        /// <param name="timeStamp">Time stamp</param>
        /// <param name="value">Value</param>
        public void AppendValue(string symbolName, EnumGeral.TipoSerieOHLC ohlcType, DateTime timeStamp, double? value)
        {
            dataManager.AppendValue(symbolName, ohlcType, timeStamp, value);
        }

        /// <summary>
        /// Appends value 
        /// </summary>
        /// <param name="seriesName">Full series name</param>
        /// <param name="timeStamp">Time stamp</param>
        /// <param name="value">Value</param>
        public void AppendValue(string seriesName, DateTime timeStamp, double? value)
        {
            dataManager.AppendValue(seriesName, EnumGeral.TipoSerieOHLC.Desconhecido, timeStamp, value);
        }

        ///<summary>
        /// Adds a tick value to the chart. Make sure chart has Tick type
        ///</summary>
        ///<param name="symbolName">Symbol name</param>
        ///<param name="timeStamp">Time stamp</param>
        ///<param name="lastPrice">Last price value</param>
        ///<param name="lastVolume">Last Volume value</param>
        public void AppendTickValue(string symbolName, DateTime timeStamp, double lastPrice, double lastVolume)
        {
            dataManager.AppendTickValue(symbolName, timeStamp, lastPrice, lastVolume);
        }

        ///<summary>
        /// Edit a value for a given series at a specified position
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="timeStamp">Time stamp where to edit</param>
        ///<param name="newValue">New value</param>
        public void EditValue(string seriesName, DateTime timeStamp, double? newValue)
        {
            dataManager.EditValueCustom(GetSeriesByName(seriesName), timeStamp, newValue);
        }

        ///<summary>
        /// Edit a value for a given series at a specified position
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="timeStamp">Time stamp where to edit</param>
        ///<param name="newValue">New value</param>
        public void EditVariacao(string seriesName, double variacao)
        {
            dataManager.EditVariacao(GetSeriesByName(seriesName), variacao);
        }

        ///<summary>
        /// Edit a value for a given series at a specified position
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="timeStamp">Time stamp where to edit</param>
        ///<param name="newValue">New value</param>
        public void EditValue(Series series, DateTime timeStamp, double? newValue)
        {
            int valueIndex = dataManager.GetTimeStampIndex(timeStamp);
            if (valueIndex == -1) return;
            if (series != null)
                series[valueIndex].Value = newValue;
        }

        ///<summary>
        /// Edit a value for a given series at a specified by index position
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="valueIndex">Value index</param>
        ///<param name="newValue">New value</param>
        public void EditValueByRecord(string seriesName, int valueIndex, double? newValue)
        {
            Series series = GetSeriesByName(seriesName);
            if (series != null)
                series[valueIndex].Value = newValue;
        }

        ///<summary>
        /// Edit a value for a given series at a specified by index position
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="valueIndex">Value index</param>
        ///<param name="newValue">New value</param>
        public void EditValueByRecord(Series series, int valueIndex, double? newValue)
        {
            series[valueIndex].Value = newValue;
        }

        ///<summary>
        /// Gets a last value from a series by index
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<returns>Value</returns>
        public double? GetLastValue(Series series)
        {
            return series[series.DM[series.SeriesIndex].Data.Count - 1].Value;
        }

        ///<summary>
        /// Gets a value from a series by index
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="valueIndex">Record index</param>
        ///<returns>Value</returns>
        public double? GetValue(string seriesName, int valueIndex)
        {
            Series series = GetSeriesByName(seriesName);
            return series != null ? series[valueIndex].Value : null;
        }

        ///<summary>
        /// Gets a value from a series by index
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="valueIndex">Record index</param>
        ///<returns>Value</returns>
        public double? GetValue(Series series, int valueIndex)
        {
            if (series[valueIndex] != null)
                return series[valueIndex].Value;
            else
                return null;
        }

        ///<summary>
        /// Gets a value from a series by time stamp
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="timeStamp">Time stamp</param>
        ///<returns>Value</returns>
        public double? GetValue(string seriesName, DateTime timeStamp)
        {
            int valueIndex = dataManager.GetTimeStampIndex(timeStamp);
            return valueIndex == -1 ? null : GetValue(seriesName, valueIndex);
        }

        ///<summary>
        /// Gets a value from a series by time stamp
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="timeStamp">Time stamp</param>
        ///<returns>Value</returns>
        public double? GetValue(Series series, DateTime timeStamp)
        {
            int valueIndex = dataManager.GetTimeStampIndex(timeStamp);
            return valueIndex == -1 ? null : GetValue(series, valueIndex);
        }

        ///<summary>
        /// Gets the index of a given timeStamp
        ///</summary>
        ///<param name="timeStamp">Time stamp to check</param>
        ///<returns>Index (0-based)</returns>
        public int GetTimeStampIndex(DateTime timeStamp)
        {
            return dataManager.GetTimeStampIndex(timeStamp);
        }

        ///<summary>
        /// Forces to recalculate indicators and their repainting
        ///</summary>
        public void RecalculateIndicators()
        {
            foreach (var panel in paineisContainers.Panels)
            {
                foreach (var indicator in panel.IndicatorsCollection)
                {
                    indicator.Painted = false;
                    indicator._calculated = false;
                    indicator.Calculate();
                    indicator.Paint();
                }
            }
        }

        /// <summary>
        /// represend the periodicity used to compress tick data
        /// it is either value in 
        /// 1. number of ticks - when compression type is ticks
        /// 2. number of seconds - when compression type is time
        /// </summary>
        public int TickPeriodicity
        {
            get { return dataManager.TickPeriodicity; }
            set { dataManager.TickPeriodicity = value; }
        }

        /// <summary>
        /// type of compression used to compress ticks.
        /// </summary>
        public EnumGeral.CompressaoTickEnum TickCompressionType
        {
            get { return dataManager.TickCompressionType; }
            set { dataManager.TickCompressionType = value; }
        }

        /// <summary>
        /// Removes a series object from the chart.
        /// </summary>
        /// <param name="series">Reference to a series to remove</param>
        public void RemoveSeries(Series series)
        {
            if (FireIndicatorBeforeDelete(series))
                return;

            if (series is IndicadorSerieFilha)
                series = ((IndicadorSerieFilha)series)._indicatorParent;

            foreach (Series linkedSeries in series._linkedSeries)
            {
                linkedSeries._recycleFlag = true;
            }
            series._recycleFlag = true;

            ReCalc = true;
            Update();
        }

        /// <summary>
        /// Removes an object from the chart
        /// </summary>
        /// <param name="lineStudy">Reference to linestudy to remove</param>
        public void RemoveObject(LineStudy lineStudy)
        {
            if (FireLineStudyBeforeDelete(lineStudy))
                return;

            lineStudy._chartPanel._lineStudies.Remove(lineStudy);
            lineStudy.Selected = false;
            lineStudy.RemoveLineStudy();
        }

        /// <summary>
        /// Removes an object from the chart with the specified object type and Key.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        public void RemoveObject(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);

            if (lineStudy == null) 
                return;

            RemoveObject(lineStudy);
        }

        /// <summary>
        /// Sets the min and max values for the panel. this values will be ussed instead of min &amp; max
        /// of series from this panel
        /// </summary>
        /// <param name="panelIndex">Panel index</param>
        /// <param name="max">Max Y price value</param>
        /// <param name="min">Min Y price value</param>
        public void SetYScale(int panelIndex, double max, double min)
        {
            if (panelIndex >= PanelsCount)
                return;
            ChartPanel chartPanel = paineisContainers.Panels[panelIndex];
            chartPanel._minChanged = min;
            chartPanel._maxChanged = max;
            chartPanel._staticYScale = true;
            chartPanel.Paint();
        }

        /// <summary>
        /// Resets the min &amp; max values. 
        /// </summary>
        /// <param name="panelIndex">Panel idnex</param>
        public void ResetYScale(int panelIndex)
        {
            if (panelIndex >= PanelsCount)
                return;
            ChartPanel chartPanel = paineisContainers.Panels[panelIndex];
            chartPanel._staticYScale = false;
            chartPanel._enforceSeriesSetMinMax = true;
            chartPanel.Paint();
        }

#if WPF
    ///<summary>
    /// A helper function to show an internal color dialog
    ///</summary>
    ///<returns>Color choosed, or null if user pressed cancel</returns>
    public Color? ShowColorDialog()
    {
      ColorPickerDialog dlg = new ColorPickerDialog();
      if (!dlg.ShowDialog().Value) return null;
      return dlg.SelectedColor;
    }
#endif

        /// <summary>
        /// Sets the height of a given panel.
        ///  </summary>
        /// <param name="panelIndex">0 based index of the panel</param>
        /// <param name="newHeight">New height</param>
        public void SetPanelHeight(int panelIndex, double newHeight)
        {
            try
            {
                if (panelIndex >= PanelsCount) return;
                ChartPanel chartPanel = paineisContainers.Panels[panelIndex];
                if (chartPanel.State != ChartPanel.StateType.Normal) return;
                if (newHeight > paineisContainers.ActualHeight) return;

                paineisContainers.ResizePanelByHeight(chartPanel, newHeight);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Move series from its current location to a new panel
        /// if series is a part of OHLC or HLC group entire group will be moved
        /// </summary>
        /// <param name="seriesName">Series na,e</param>
        /// <param name="toPanelIndex">New panel index</param>
        public void MoveSeries(string seriesName, int toPanelIndex)
        {
            Series series = GetSeriesByName(seriesName);
            if (series == null) return;
            if (series._chartPanel.Index == toPanelIndex) return;

            ChartPanel fromPanel = series._chartPanel;
            ChartPanel toPanel = GetPanelByIndex(toPanelIndex);

            fromPanel.MoveSeriesTo(series, toPanel, MoveSeriesIndicator.MoveStatusEnum.MoverPainelExistente);
        }

        /// <summary>
        /// Gets x-pixel coordinate by record index
        /// </summary>
        /// <param name="index">Record index</param>
        /// <returns>X pixel</returns>
        public double GetXPixel(double index)
        {
            return GetXPixel(index, false);
        }

        /// <summary>
        /// Gets the pixel location of a price at the specified record. Pixel will be located in panel with index 0
        /// </summary>
        /// <param name="priceValue">Price value</param>
        /// <returns>Record index</returns>
        public double? GetYPixel(double priceValue)
        {
            return GetYPixel(priceValue, 0);
        }

        /// <summary>
        /// Gets the pixel location of a price at the specified record. 
        /// </summary>
        /// <param name="priceValue">Price value</param>
        /// <param name="panelIndex">Panel Index</param>
        /// <returns>Pixel value</returns>
        public double? GetYPixel(double priceValue, int panelIndex)
        {
            if (paineisContainers.Panels.Count == 0) return null;
            ChartPanel chartPanel = paineisContainers.Panels[panelIndex];
            if (chartPanel._state != ChartPanel.StateType.Normal) return null;
            return chartPanel.GetY(priceValue);
        }

        /// <summary>
        /// Removes a series object from the chart
        /// </summary>
        /// <param name="seriesName">Series name</param>
        public void RemoveSeries(string seriesName)
        {
            ChartPanel chartPanel = GetPanelBySeriesName(seriesName);
            if (chartPanel == null) return;
            Series series = GetSeriesByName(seriesName);
            if (series == null) return;
            chartPanel.RemoveSeries(series, true);
            series._recycleFlag = true;
            Update();
        }

        /// <summary>
        /// This method Gets the panel reference that contains the specified series. This method is useful 
        /// because users may drag series from one chart panel to another, or delete the series entirely.
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Reference to a series</returns>
        public ChartPanel GetPanelBySeriesName(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : series._chartPanel;
        }

        /// <summary>
        /// Scrolls the chart to the left by the specified amount of Records. 
        /// If the chart is already scrolled to the maximum level, this method will have no effect. 
        /// </summary>
        /// <param name="records">Number of records</param>
        public void ScrollLeft(int records)
        {
            if (status != StatusGrafico.Preparado) return;
            if (indexInicial - records > 0)
            {
                indexInicial -= records;
                indexFinal -= records;
            }
            else
            {
                int oldStartIndex = indexInicial;
                indexInicial = 0;
                indexFinal -= oldStartIndex;
            }
            Update();
            FireChartScroll();
        }

        /// <summary>
        /// Scrolls the chart to the right by the specified amount of Records. 
        /// If the chart is already scrolled to the maximum level, this method will have no effect.
        /// </summary>
        /// <param name="records">Number of records</param>
        public void ScrollRight(int records)
        {
            if (status != StatusGrafico.Preparado) return;
            if (indexFinal + records <= RecordCount)
            {
                indexInicial += records;
                indexFinal += records;
            }
            else
            {
                indexInicial += RecordCount - indexFinal;
                indexFinal = RecordCount;
            }
            Update();
            FireChartScroll();
        }

        /// <summary>
        /// Clears all values for the series object specified by the Name argument. E.g. if you have a series named 
        /// "my series" and have previously inserted data into that series you can erase all data in that series and 
        /// start over by calling ClearValues. This is easier than calling RemoveSeries and AddSeries again, or RemoveAllSeries. 
        /// You can also clear ALL series values via the ClearAllSeries function.
        /// </summary>
        /// <param name="seriesName">Series name</param>
        public void ClearValues(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            if (series == null) return;
            ClearValues(series);
        }

        /// <summary>
        /// Clears all values for the series object specified by the Name argument. E.g. if you have a series named 
        /// "my series" and have previously inserted data into that series you can erase all data in that series and 
        /// start over by calling ClearValues. This is easier than calling RemoveSeries and AddSeries again, or ClearAll(). 
        /// You can also clear ALL series values via the ClearAllSeries function.
        /// </summary>
        /// <param name="series">Reference to a series</param>
        public void ClearValues(Series series)
        {
            dataManager.ClearValues(series._seriesIndex);
            recalc = true;
        }

        /// <summary>
        /// Clears all values from all series on the chart. To clear values from one series only, use the 
        /// ClearValues function instead. To remove series, use the ClearAll() function.
        /// </summary>
        public void ClearAllSeries()
        {
            dataManager.ClearData();
            indexInicial = indexFinal = 0;
        }

        /// <summary>
        /// Gets a reference to a lineStudy (that also includes buy, sell, ... symbols)
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Reference to a line study</returns>
        public LineStudy GetLineStudy(string objectKey)
        {
            foreach (var panel in paineisContainers.Panels)
            {
                foreach (var lineStudy in panel._lineStudies)
                {
                    if (lineStudy.Key == objectKey) return lineStudy;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a symbol (bmp) object, line object, or text object's start record number.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Start records</returns>
        public double? GetObjectStartRecord(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy != null ? lineStudy.X1Value : (double?)null;
        }

        /// <summary>
        /// Gets a symbol (bitmap) object, line object, or text object's end record number.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>End Record</returns>
        public double? GetObjectEndRecord(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy != null ? lineStudy.X2Value : (double?)null;
        }

        /// <summary>
        /// Gets a symbol (bmp) object, line object, or text object's start price value.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Start value</returns>
        public double? GetObjectStartValue(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy != null ? lineStudy.Y1Value : (double?)null;
        }

        /// <summary>
        /// Gets a symbol (bmp) object, line object, or text object's end price value.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>End value</returns>
        public double? GetObjectEndValue(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy != null ? lineStudy.Y2Value : (double?)null;
        }

        /// <summary>
        /// Sets a symbol (bmp) object, line object, or text ojbect's start record, end record, start value, and end value. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="startRecord">Start record</param>
        /// <param name="startValue">Start value</param>
        /// <param name="endRecord">End Record</param>
        /// <param name="endValue">End Value</param>
        /// <returns>Reference to a line study</returns>
        public LineStudy SetObjectPosition(string objectKey, int startRecord, double startValue, int endRecord, double endValue)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return null;
            lineStudy.SetXYValues(startRecord, startValue, endRecord, endValue);
            return lineStudy;
        }

        /// <summary>
        /// Gets the total number of objects on the chart that match the specified type. 
        /// </summary>
        /// <param name="studyTypeEnum">Study type</param>
        /// <returns>Number</returns>
        public int GetObjectCount(LineStudy.StudyTypeEnum studyTypeEnum)
        {
            int res = 0;

            foreach (var panel in paineisContainers.Panels)
            {
                foreach (var study in panel._lineStudies)
                {
                    if (study.StudyType == studyTypeEnum)
                        res++;
                }
            }

            return res;
        }

        /// <summary>
        /// sets a custom background brush for a specified by index bar
        /// </summary>
        /// <param name="barIndex">Bar index</param>
        /// <param name="customBrush">A new background brush</param>
        public void BarBrush(int barIndex, Brush customBrush)
        {
            barBrushes[barIndex] = new BarBrushData
                                      {
                                          Brush = customBrush,
                                          Changed = true
                                      };
            if (OnCandleCustomBrush != null)
                OnCandleCustomBrush(barIndex - indexInicial, customBrush);
        }

        /// <summary>
        /// Gets the bar brush or null if bar has no user-defined brush
        /// </summary>
        /// <param name="barIndex">Bar index</param>
        /// <returns>Brush used</returns>
        public Brush BarBrush(int barIndex)
        {
            BarBrushData outBrush;
            return barBrushes.TryGetValue(barIndex, out outBrush) ? barBrushes[barIndex].Brush : null;
        }

        /// <summary>
        /// Sets the line color for the series as specified by the Name argument. This property may not apply to certain price styles. 
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <param name="seriesColor">Series color</param>
        public void SeriesColor(string seriesName, Color seriesColor)
        {
            Series series = GetSeriesByName(seriesName);
            if (series == null) return;
            series.StrokeColor = seriesColor;
        }

        /// <summary>
        /// Sets the line color for the series as specified by the Name argument
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Color used, or null if such series doesn't exist</returns>
        public Color? SeriesColor(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? (Color?)null : series.StrokeColor;
        }

        /// <summary>
        /// sets the stroke color for a given object 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="newColor">New color</param>
        public void ObjectColor(string objectKey, Color newColor)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            lineStudy.Stroke = new SolidColorBrush(newColor);
        }

        /// <summary>
        /// Retrieves the object color 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Color or null if such object doesn't exist</returns>
        public Color? ObjectColor(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy == null ? (Color?)null : ((SolidColorBrush)lineStudy.Stroke).Color;
        }

        /// <summary>
        /// If this property is set to False, the user will not be able to select the object with the mouse. The object may be of any object type. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="selectAble">true - selectable, false - otherwise</param>
        public void ObjectSelectable(string objectKey, bool selectAble)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            lineStudy.Selectable = selectAble;
        }

        /// <summary>
        /// A property that Gets says if an object is selectable, or null if such an object doesn't exists
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>true or false if object is selectable or not, or null if such objects doesn't exists</returns>
        public bool? ObjectSelectable(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy == null ? (bool?)null : lineStudy.Selectable;
        }

        /// <summary>
        /// Sets the pen style of the specified object. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="objectStyle">Line pattern</param>
        public void ObjectStyle(string objectKey, EnumGeral.TipoLinha objectStyle)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            lineStudy.StrokeType = objectStyle;
        }

        /// <summary>
        /// Gets the pen style of the specified object. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Line pattern used, or null if such object doesn't exists</returns>
        public EnumGeral.TipoLinha? ObjectStyle(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy == null ? (EnumGeral.TipoLinha?)null : lineStudy.StrokeType;
        }

        /// <summary>
        /// Sets the line weight of the object as specified by the object Name argument. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="objectWeight">New line thickness</param>
        public void ObjectWeight(string objectKey, double objectWeight)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            lineStudy.StrokeThickness = objectWeight;
        }

        /// <summary>
        /// Gets the line weight of the object as specified by the object Name argument. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Line thickness used, or null if such object doesn't exists</returns>
        public double? ObjectWeight(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy == null ? 0 : lineStudy.StrokeThickness;
        }

        /// <summary>
        /// Sets the Text value of the object specified by object Name. Works only for StaticText type of objects
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="objectText">New text</param>
        public void ObjectText(string objectKey, string objectText)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            ((StaticText)lineStudy).Text = objectText;
        }

        /// <summary>
        /// Gets the Text value of the object specified by object Name. Works only for StaticText type of objects
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Object's text or null if such object doesn't exists</returns>
        public string ObjectText(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return null;
            StaticText staticText = lineStudy as StaticText;
            return staticText == null ? null : staticText.Text;
        }

        /// <summary>
        /// Gets the fontname for a StaticText object
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="fontName">New font name</param>
        public void TextAreaFontName(string objectKey, string fontName)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            StaticText staticText = lineStudy as StaticText;
            if (staticText != null)
                staticText.FontName = fontName;
        }

        /// <summary>
        /// Gets the fontname for a StaticText object
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Font name used</returns>
        public string TextAreaFontName(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return null;
            StaticText staticText = lineStudy as StaticText;
            return staticText == null ? null : staticText.FontName;
        }

        /// <summary>
        /// This function Gets the Indicator type for any indicator series added via the AddIndicator method.
        /// </summary>
        /// <param name="indicatorKey">Indicator's unique key</param>
        /// <returns>Indicator type</returns>
        public EnumGeral.IndicatorType GetIndicatorType(string indicatorKey)
        {
            Series indicator = GetSeriesByName(indicatorKey);
            if (indicator == null) return EnumGeral.IndicatorType.Unknown;
            Indicator indicator1 = indicator as Indicator;
            return indicator1 == null ? EnumGeral.IndicatorType.Unknown : indicator1.IndicatorType;
        }

        /// <summary>
        /// Zooms the chart in by the specified amount of Records. If the chart is zoomed in all the way (meaning only one bar is visible), this method will have no effect. 
        /// </summary>
        /// <param name="records">Records to zoom in</param>
        public void ZoomIn(int records)
        {
            if (status != StatusGrafico.Preparado) return;
            if (indexFinal - indexInicial - records > records)
            {
                indexInicial += records;
                //indexFinal -= records;
            }
            Update();
            FireZoom();
        }

        /// <summary>
        /// Zooms the chart out by the specified amount of Records. This method will have no effect if the chart is zoomed out all the way.
        /// </summary>
        /// <param name="records">Records to zoom out</param>
        public void ZoomOut(int records)
        {
            if (status != StatusGrafico.Preparado) return;

            int recCnt = RecordCount;
            if (indexInicial - records > -1)
            {
                indexInicial -= records;
                indexFinal += records;
            }
            else if (estiloPreco != EnumGeral.EstiloPrecoEnum.Padrao && indexFinal < recCnt)
            {
                indexFinal += records;
            }
            else
            {
                indexInicial = 0;
            }
            if (indexFinal >= recCnt)
                //indexFinal = recCnt - 1;
                indexFinal = recCnt;

            Update();
            FireZoom();
        }

        /// <summary>
        /// Resets FirstVisibleRecord to 0 and LastVisibleRecord to RecordCount - 1, making the first and last bars visible on the chart. 
        /// </summary>
        public void ResetZoom()
        {
            indexInicial = 0;
            indexFinal = RecordCount;
            Update();
            FireZoom();
        }

        /// <summary>
        /// Returns the Timestamp by its index value
        /// </summary>
        /// <param name="index">Record index for which timestamp is needed. Index is 0 based.</param>
        /// <returns>Timestamp if index value is ok, or null</returns>
        public DateTime? GetTimestampByIndex(int index)
        {
            DateTime result = dataManager.GetTimeStampByIndex(index);
            if (result == DateTime.MinValue)
                return null;
            return result;
        }

        /// <summary>
        /// Gets record by pixel value
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public int GetReverseX(double pixel)
        {
            return (int)GetReverseXInternal(pixel + espacamentoBarra + larguraBarra / 2);
        }

        /// <summary>
        /// Gets record by pixel value
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public int GetRealReverseX(double pixel)
        {
            return Convert.ToInt32(GetReverseXInternal(pixel + espacamentoBarra + larguraBarra / 2 - GetRealLeftOffset()) + indexInicial);
        }

        private double GetRealLeftOffset()
        {
            if (EscalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Direita)
                return 0;
            return Constants.YAxisWidth;
        }
        

        /// <summary>
        /// Returns an aproximated record index for a given timeStamp.
        /// Usefull when an exact timestamp is unknown
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="roundUp">
        /// if true - returns the next recordCount that has a value greater then given timestamp
        /// if false - returns the previos recordCount that has a value less then given timestamp
        /// </param>
        /// <returns>Timestamp index, -1 if no such aproximate value</returns>
        public int GetReverseX(DateTime timestamp, bool roundUp)
        {
            if (dataManager == null)
                return -1;
            return dataManager.GetTimeStampIndex(timestamp, roundUp);
        }
        #endregion
    }
}

