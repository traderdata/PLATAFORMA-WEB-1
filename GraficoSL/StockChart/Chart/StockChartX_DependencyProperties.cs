using System.Windows;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public partial class StockChartX
    {
        static StockChartX()
        {
            #region Dependency properties registration

            MostrarAnimacoesProperty =
              DependencyProperty.Register("ShowAnimations",
                                          typeof(bool), typeof(StockChartX),
                                          new PropertyMetadata(new PropertyChangedCallback(OnMostrarAnimacoesChanged)));
            EscalaAlinhamentoProperty =
              DependencyProperty.Register("ScaleAlignment", typeof(EnumGeral.TipoAlinhamentoEscalaEnum), typeof(StockChartX),
                                          new PropertyMetadata(EnumGeral.TipoAlinhamentoEscalaEnum.Direita, OnEscalaAlinhamentoChanged));

            EscalaTipoProperty =
              DependencyProperty.Register("ScalingType", typeof(EnumGeral.TipoEscala), typeof(StockChartX),
                                          new PropertyMetadata(EnumGeral.TipoEscala.Linear, OnEscalaTipoChanged));
            GradeYProperty =
              DependencyProperty.Register("ShowYGrid", typeof(bool), typeof(StockChartX),
                                          new PropertyMetadata(true, OnShowGradeYChanged));

            GradeXProperty =
              DependencyProperty.Register("ShowXGrid", typeof(bool), typeof(StockChartX),
                                          new PropertyMetadata(false, OnShowGradeXChanged));

            GradeCorProperty =
              DependencyProperty.Register("GridStroke", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(Brushes.Silver, OnGradeCorChanged));

            Estilo3DProperty =
              DependencyProperty.Register("ThreeDStyle", typeof(bool), typeof(StockChartX),
                                          new PropertyMetadata(true, OnEstilo3DChanged));

            CandleCorAltaProperty =
              DependencyProperty.Register("UpColor", typeof(Color), typeof(StockChartX),
                                          new PropertyMetadata(ColorsEx.Lime, OnCandleCorAltaChanged));

            CandleCorBaixaProperty =
              DependencyProperty.Register("DownColor", typeof(Color), typeof(StockChartX),
                                          new PropertyMetadata(Colors.Red, OnCandleCorBaixaChanged));

            CrossHairsProperty =
              DependencyProperty.Register("CrossHairs", typeof(bool), typeof(StockChartX),
                                          new PropertyMetadata(false, OnCrossHairsChanged));

            CorCrossHairsProperty =
              DependencyProperty.Register("CrossHairsStroke", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(Brushes.Yellow, OnCorCrossHairsChanged));

            CorEsquerdaGraficoProperty =
              DependencyProperty.Register("LeftChartSpace", typeof(double), typeof(StockChartX),
                                          new PropertyMetadata(10.0, OnCorEsquerdaGraficoChanged));

            EspacoDireitaGraficoProperty =
              DependencyProperty.Register("RightChartSpace", typeof(double), typeof(StockChartX),
                                          new PropertyMetadata(50.0, OnEspacoDireitaGraficoChanged));

            LabelsEixoXRealTimeProperty =
              DependencyProperty.Register("RealTimeData", typeof(bool), typeof(StockChartX),
                                          new PropertyMetadata(false, OnLabelsEixoXRealTimeChanged));

            MostrarTitulosPaineisProperty =
              DependencyProperty.Register("DisplayTitles", typeof(bool), typeof(StockChartX),
                                          new PropertyMetadata(true, OnMostrarTitulosPaineisChanged));

            BackGroundDialogoIndicadorProperty =
              DependencyProperty.Register("IndicatorDialogBackground", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(
                                            new RadialGradientBrush
                                              {
                                                  Center = new Point(0.6, 0.7),
                                                  RadiusX = 1,
                                                  RadiusY = 1,
                                                  GradientStops =
                                                    new GradientStopCollection
                                              {
                                                new GradientStop { Color = ColorsEx.LightBlue, Offset = 0 },
                                                new GradientStop { Color = ColorsEx.LightSteelBlue, Offset = 1 },
                                              }
                                              }));

            InfoPanelCorFundoLabelsProperty =
              DependencyProperty.Register("InfoPanelLabelsBackground", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(Brushes.Yellow, OnInfoPanelCorFundoLabelsChanged));

            InfoPanelCorFonteLabelsProperty =
              DependencyProperty.Register("InfoPanelLabelsForeground", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(Brushes.Black, OnInfoPanelCorFonteLabelsChanged));

            InfoPanelCorFundoValoresProperty =
              DependencyProperty.Register("InfoPanelValuesBackground", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(Brushes.White, OnInfoPanelCorFundoValoresChanged));

            InfoPanelCorFonteValoresProperty =
              DependencyProperty.Register("InfoPanelValuesForeground", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(Brushes.Black, OnInfoPanelCorFonteValoresChanged));

            InfoPanelFonteProperty =
              DependencyProperty.Register("InfoPanelFontFamily", typeof(FontFamily), typeof(StockChartX),
                                          new PropertyMetadata(new FontFamily("Arial"), OnInfoPanelFonteChanged));

            InfoPanelTamanhoFonteProperty =
              DependencyProperty.Register("InfoPanelFontSize", typeof(double), typeof(StockChartX),
                                          new PropertyMetadata(9.0, OnInfoPanelTamanhoFonteChanged));

            InfoPanelPosicaoProperty =
              DependencyProperty.Register("InfoPanelPosition", typeof(EnumGeral.InfoPanelPosicaoEnum), typeof(StockChartX),
                                          new PropertyMetadata(EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse, OnInfoPanelPositionChanged));

            SufixoVolumeProperty =
              DependencyProperty.Register("VolumePostfix", typeof(string), typeof(StockChartX),
                                          new PropertyMetadata(""));

            HeatPanelLabelsForegroundProperty =
              DependencyProperty.Register("HeatPanelLabelsForeground", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(Brushes.Yellow, OnHeatPanelLabelsForegroundChanged));

            HeatPanelLabelsBackgroundProperty =
              DependencyProperty.Register("HeatPanelLabelsBackground", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(Brushes.Black, OnHeatPanelLabelsBackgroundChanged));

            HeatPanelLabelsFontSizeProperty =
              DependencyProperty.Register("HeatPanelLabelsFontSize", typeof(double), typeof(StockChartX),
                                          new PropertyMetadata(12.0, OnHeatPanelLabelFontSizeChanged));

            EscalaPrecisaoProperty =
              DependencyProperty.Register("ScalePrecision", typeof(int), typeof(StockChartX),
                                          new PropertyMetadata(2, OnEscalaPrecisaoChanged));


            ManterNivelZoomProperty =
              DependencyProperty.Register("KeepZoomLevel", typeof(bool), typeof(StockChartX),
                                          new PropertyMetadata(false));

            VolumeDivisorProperty =
              DependencyProperty.Register("VolumeDivisor", typeof(int), typeof(StockChartX),
                                          new PropertyMetadata(1000000, OnVolumeDivisorChanged));

            LineStudyPropertyDialogBackgroundProperty =
              DependencyProperty.Register("LineStudyPropertyDialogBackground", typeof(Brush), typeof(StockChartX),
                                          new PropertyMetadata(Brushes.White));

            #endregion
        }

        #region MostrarAnimacoes
        /// <summary>
        /// Obtém ou define o valor que indica se as animações serão mostrados ou não.
        /// </summary>
        public static DependencyProperty MostrarAnimacoesProperty;
        ///<summary>
        /// Obtém ou define o valor que indica se as animações serão mostrados ou não.
        ///</summary>
        public bool MostrarAnimacoes
        {
            get { return (bool)GetValue(MostrarAnimacoesProperty); }
            set { SetValue(MostrarAnimacoesProperty, value); }
        }
        private static void OnMostrarAnimacoesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {

        }
        #endregion MostrarAnimacoes

        #region GradeY
        /// <summary>
        /// Obtém ou define a visibilidade da grade Y.
        /// </summary>
        public static readonly DependencyProperty GradeYProperty;
        ///<summary>
        /// Obtém ou define a visibilidade da grade Y.
        ///</summary>
        public bool GradeY
        {
            get { return (bool)GetValue(GradeYProperty); }
            set { SetValue(GradeYProperty, value); }
        }
        private static void OnShowGradeYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((StockChartX)sender).ShowHideLinhasGradeY();
        }
        #endregion GradeY

        #region GradeX
        /// <summary>
        /// Obtém ou define a visibilidade da grade X.
        /// </summary>
        public static readonly DependencyProperty GradeXProperty;
        ///<summary>
        /// Obtém ou define a visibilidade da grade X.
        ///</summary>
        public bool GradeX
        {
            get { return (bool)GetValue(GradeXProperty); }
            set { SetValue(GradeXProperty, value); }
        }
        private static void OnShowGradeXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((StockChartX)sender).ShowHideLinhasGradeX();
        }
        #endregion GradeX

        #region GradeCor
        ///<summary>
        /// Obtém ou define a <seealso cref="Brush"/> das linhas da grade.
        ///</summary>
        public static readonly DependencyProperty GradeCorProperty;
        ///<summary>
        /// Obtém ou define a <seealso cref="Brush"/> das linhas da grade.
        ///</summary>
        public Brush GradeCor
        {
            get { return (Brush)GetValue(GradeCorProperty); }
            set { SetValue(GradeCorProperty, value); }
        }
        private static void OnGradeCorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((StockChartX)sender).Update();
        }
        #endregion GradeCor

        #region Estilo3D
        ///<summary>
        /// Obtém ou seta o estilo dos candles (3D ou 2D). Obs: 3D só funciona se não estiver usando Otimizacao.
        ///</summary>
        public static readonly DependencyProperty Estilo3DProperty;
        ///<summary>
        /// Obtém ou seta o estilo dos candles (3D ou 2D). Obs: 3D só funciona se não estiver usando Otimizacao.
        ///</summary>
        public bool Estilo3D
        {
            get { return (bool)GetValue(Estilo3DProperty); }
            set { SetValue(Estilo3DProperty, value); }
        }
        private static void OnEstilo3DChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((StockChartX)sender).Update();
        }
        #endregion Estilo3D

        #region EscalaAlinhamento
        ///<summary>
        /// Obtém ou define o alinhamento da escala.
        ///</summary>
        public static readonly DependencyProperty EscalaAlinhamentoProperty;
        ///<summary>
        /// Obtém ou define o alinhamento da escala.
        ///</summary>
        public EnumGeral.TipoAlinhamentoEscalaEnum EscalaAlinhamento
        {
            get { return (EnumGeral.TipoAlinhamentoEscalaEnum)GetValue(EscalaAlinhamentoProperty); }
            set { SetValue(EscalaAlinhamentoProperty, value); }
        }
        private static void OnEscalaAlinhamentoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.SetYAxes();
        }

        #endregion EscalaAlinhamento

        #region EscalaTipo
        ///<summary>
        /// Obtém ou define o tipo da escala: linear ou semilog.
        ///</summary>
        public static readonly DependencyProperty EscalaTipoProperty;
        ///<summary>
        /// Obtém ou define o tipo da escala: linear ou semilog.
        ///</summary>
        public EnumGeral.TipoEscala EscalaTipo
        {
            get { return (EnumGeral.TipoEscala)GetValue(EscalaTipoProperty); }
            set 
            {
                this.escalaTipo = value;
                SetValue(EscalaTipoProperty, value);
            }
        }
        private static void OnEscalaTipoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.Update();
        }
        #endregion EscalaTipo

        #region EscalaPrecisao
        ///<summary>
        /// Obtém ou define a precisão da escala.
        ///</summary>
        public static readonly DependencyProperty EscalaPrecisaoProperty;
        ///<summary>
        /// Obtém ou define a precisão da escala.
        ///</summary>
        public int EscalaPrecisao
        {
            get { return (int)GetValue(EscalaPrecisaoProperty); }
            set { SetValue(EscalaPrecisaoProperty, value); }
        }
        private static void OnEscalaPrecisaoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.Update();
        }

        #endregion EscalaPrecisao

        #region CandleCorAlta
        ///<summary>
        ///Obtém ou define a <seealso cref="Color"/> usada para pintar a barra de alta. 
        ///</summary>
        public static readonly DependencyProperty CandleCorAltaProperty;
        ///<summary>
        /// Gets or sets the <seealso cref="Color"/> used to paint up-tick bars.When the close is higher than the previous close, this color will be used to paint the bar.
        ///</summary>
        public Color CandleCorAlta
        {
            get { return (Color)GetValue(CandleCorAltaProperty); }
            set { SetValue(CandleCorAltaProperty, value); }
        }
        private static void OnCandleCorAltaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((StockChartX)sender).Update();
        }
        #endregion CandleCorAlta

        #region CandleCorBaixa
        ///<summary>
        /// Obtém ou define a <seealso cref="Color"/> usada para pintar a barra de baixa. 
        ///</summary>
        public static readonly DependencyProperty CandleCorBaixaProperty;
        ///<summary>
        /// Obtém ou define a <seealso cref="Color"/> usada para pintar a barra de baixa. 
        ///</summary>
        public Color CandleCorBaixa
        {
            get { return (Color)GetValue(CandleCorBaixaProperty); }
            set { SetValue(CandleCorBaixaProperty, value); }
        }
        private static void OnCandleCorBaixaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((StockChartX)sender).Update();
        }
        #endregion CandleCorBaixa

        #region CrossHairs
        ///<summary>
        /// Obtém ou define a visibilidade do Cross Hairs.
        ///</summary>
        public static readonly DependencyProperty CrossHairsProperty;
        ///<summary>
        /// Obtém ou define a visibilidade do Cross Hairs.
        ///</summary>
        public bool CrossHairs
        {
            get { return (bool)GetValue(CrossHairsProperty); }
            set { SetValue(CrossHairsProperty, value); }
        }
        private static void OnCrossHairsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;

            if ((bool)eventArgs.NewValue)
                chartX._timers.StartTimerWork(TimerCrossHairs);
            else
            {
                chartX._timers.StopTimerWork(TimerCrossHairs);
                chartX.paineisContainers.EscondeCrossHairs();
            }
        }
        #endregion CrossHairs

        #region CorCrossHairs
        /// <summary>
        /// Obtém ou define a cor do Cross Hairs.
        /// </summary>
        public static readonly DependencyProperty CorCrossHairsProperty;
        /// <summary>
        /// Obtém ou define a cor do Cross Hairs.
        /// </summary>
        public Brush CorCrossHairs
        {
            get { return (Brush)GetValue(CorCrossHairsProperty); }
            set { SetValue(CorCrossHairsProperty, value); }
        }
        private static void OnCorCrossHairsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;

            if (chartX.paineisContainers != null)
                chartX.paineisContainers.AtualizaCorCrossHairs();
        }

        #endregion CorCrossHairs

        #region CorEsquerdaGrafico
        ///<summary>
        /// Obtém ou define o espaço não preenchido à direita do gráfico.
        ///</summary>
        public static readonly DependencyProperty CorEsquerdaGraficoProperty;
        ///<summary>
        /// Obtém ou define o espaço não preenchido à direita do gráfico.
        ///</summary>
        public double EspacoEsquerdaGrafico
        {
            get { return (double)GetValue(CorEsquerdaGraficoProperty); }
            set { SetValue(CorEsquerdaGraficoProperty, value); }
        }
        private static void OnCorEsquerdaGraficoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.espacoEsquerdaGrafico = (double)eventArgs.NewValue;
            chartX.Update();
        }
        #endregion CorEsquerdaGrafico

        #region EspacoDireitaGrafico
        ///<summary>
        /// Obtém ou define o espaço à direita do braço.
        ///</summary>
        public static readonly DependencyProperty EspacoDireitaGraficoProperty;
        ///<summary>
        /// Obtém ou define o espaço à direita do braço.
        ///</summary>
        public double EspacoDireitaGrafico
        {
            get { return (double)GetValue(EspacoDireitaGraficoProperty); }
            set { SetValue(EspacoDireitaGraficoProperty, value); }
        }
        private static void OnEspacoDireitaGraficoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.espacoDireitaGrafico = (double)eventArgs.NewValue;
            chartX.Update();
        }

        #endregion EspacoDireitaGrafico

        #region LabelsEixoXRealTime
        /// <summary>
        /// Quando verdadeiro eixo X irá exibir datas para o modo em tempo real.
        /// </summary>
        public static readonly DependencyProperty LabelsEixoXRealTimeProperty;
        /// <summary>
        /// Quando verdadeiro eixo X irá exibir datas para o modo em tempo real.
        /// </summary>
        public bool LabelsEixoXRealTime
        {
            get { return (bool)GetValue(LabelsEixoXRealTimeProperty); }
            set { SetValue(LabelsEixoXRealTimeProperty, value); }
        }
        private static void OnLabelsEixoXRealTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            //((StockChartX)sender)._calendar.PaintEx();     
        }
        #endregion LabelsEixoXRealTime

        #region MostrarTitulosPaineis
        ///<summary>
        /// Obtém ou define se deve exibir painéis de títulos ou não.
        ///</summary>
        public static readonly DependencyProperty MostrarTitulosPaineisProperty;
        ///<summary>
        /// Obtém ou define se deve exibir painéis de títulos ou não.
        ///</summary>
        public bool MostrarTitulosPaineis
        {
            get { return (bool)GetValue(MostrarTitulosPaineisProperty); }
            set { SetValue(MostrarTitulosPaineisProperty, value); }
        }
        private static void OnMostrarTitulosPaineisChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = ((StockChartX)sender);
            foreach (ChartPanel chartPanel in chartX.paineisContainers.Panels)
            {
                chartPanel.ShowHideTitleBar();
            }
        }
        #endregion MostrarTitulosPaineis

        #region VolumeDivisor

        public int VolumeDivisor
        {
            get { return (int)GetValue(VolumeDivisorProperty); }
            set { SetValue(VolumeDivisorProperty, value); }
        }
        public static readonly DependencyProperty VolumeDivisorProperty;
        private static void OnVolumeDivisorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = ((StockChartX)sender);
            foreach (ChartPanel panel in chartX.paineisContainers.Panels)
            {
                if (panel._leftYAxis != null && panel._leftYAxis.Visibility == Visibility.Visible)
                    panel._leftYAxis.Render();
                if (panel._rightYAxis != null && panel._rightYAxis.Visibility == Visibility.Visible)
                    panel._rightYAxis.Render();
            }
        }
        #endregion VolumeDivisor

        #region AppendTickVolumeBehaviorProperty (DependencyProperty)

        /// <summary>
        /// A description of the AppendTickVolumeBehavior.
        /// </summary>
        public DataManager.AppendTickVolumeBehavior AppendTickVolumeBehavior
        {
            get { return (DataManager.AppendTickVolumeBehavior)GetValue(AppendTickVolumeBehaviorProperty); }
            set { SetValue(AppendTickVolumeBehaviorProperty, value); }
        }

        /// <summary>
        /// AppendTickVolumeBehavior
        /// </summary>
        public static readonly DependencyProperty AppendTickVolumeBehaviorProperty =
          DependencyProperty.Register("AppendTickVolumeBehavior", typeof(DataManager.AppendTickVolumeBehavior), typeof(StockChartX),
                                      new PropertyMetadata(DataManager.AppendTickVolumeBehavior.Increment, OnAppendTickVolumeBehaviorChanged));

        private static void OnAppendTickVolumeBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnAppendTickVolumeBehaviorChanged(e);
        }

        protected virtual void OnAppendTickVolumeBehaviorChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region IndicatorDialogBackground
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> used as background for indicators dialog.
        ///</summary>
        public static readonly DependencyProperty BackGroundDialogoIndicadorProperty;
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> used as background for indicators dialog.
        ///</summary>
        public Brush BackGroundDialogoIndicador
        {
            get { return (Brush)GetValue(BackGroundDialogoIndicadorProperty); }
            set { SetValue(BackGroundDialogoIndicadorProperty, value); }
        }
        #endregion IndicatorDialogBackground

        #region SufixoVolume
        /// <summary>
        /// Obtém ou define sufixo usado para exibir o volume em milhões (por exemplo, mostrar 5.200.000 como "5,2 M" na escala de Y).
        /// </summary>
        public static readonly DependencyProperty SufixoVolumeProperty;
        /// <summary>
        /// Obtém ou define sufixo usado para exibir o volume em milhões (por exemplo, mostrar 5.200.000 como "5,2 M" na escala de Y).
        /// </summary>
        public string SufixoVolume
        {
            get { return (string)GetValue(SufixoVolumeProperty); }
            set { SetValue(SufixoVolumeProperty, value); }
        }
        #endregion SufixoVolume

        #region InfoPanel

        #region InfoPanelCorFundoLabels
        ///<summary>
        /// Obtém ou define a cor de fundo (<seealso cref="Brush"/>) dos labels do info panel.
        ///</summary>
        public static readonly DependencyProperty InfoPanelCorFundoLabelsProperty;
        ///<summary>
        /// Obtém ou define a cor de fundo (<seealso cref="Brush"/>) dos labels do info panel.
        ///</summary>
        public Brush InfoPanelCorFundoLabels
        {
            get { return (Brush)GetValue(InfoPanelCorFundoLabelsProperty); }
            set { SetValue(InfoPanelCorFundoLabelsProperty, value); }
        }
        private static void OnInfoPanelCorFundoLabelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.paineisContainers.EnforceInfoPanelUpdate();
        }
        #endregion InfoPanelCorFundoLabels

        #region InfoPanelCorFonteLabels
        ///<summary>
        /// Obtém ou define a cor da fonte (<seealso cref="Brush"/>) dos labels do info panel. 
        ///</summary>
        public static readonly DependencyProperty InfoPanelCorFonteLabelsProperty;
        ///<summary>
        /// Obtém ou define a cor da fonte (<seealso cref="Brush"/>) dos labels do info panel.
        ///</summary>
        public Brush InfoPanelCorFonteLabels
        {
            get { return (Brush)GetValue(InfoPanelCorFonteLabelsProperty); }
            set { SetValue(InfoPanelCorFonteLabelsProperty, value); }
        }
        private static void OnInfoPanelCorFonteLabelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.paineisContainers.EnforceInfoPanelUpdate();
        }
        #endregion InfoPanelCorFonteLabels

        #region InfoPanelCorFundoValores
        ///<summary>
        /// Obtém ou define a cor de fundo (<seealso cref="Brush"/>) dos valores do info panel.
        ///</summary>
        public static readonly DependencyProperty InfoPanelCorFundoValoresProperty;
        ///<summary>
        /// Obtém ou define a cor de fundo (<seealso cref="Brush"/>) dos valores do info panel.
        ///</summary>
        public Brush InfoPanelCorFundoValores
        {
            get { return (Brush)GetValue(InfoPanelCorFundoValoresProperty); }
            set { SetValue(InfoPanelCorFundoValoresProperty, value); }
        }
        private static void OnInfoPanelCorFundoValoresChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.paineisContainers.EnforceInfoPanelUpdate();
        }
        #endregion InfoPanelCorFundoValores

        #region InfoPanelCorFonteValores
        ///<summary>
        /// Obtém ou define a cor da fonte (<seealso cref="Brush"/>) dos valores do info panel.
        ///</summary>
        public static readonly DependencyProperty InfoPanelCorFonteValoresProperty;
        ///<summary>
        /// Obtém ou define a cor da fonte (<seealso cref="Brush"/>) dos valores do info panel.
        ///</summary>
        public Brush InfoPanelCorFonteValores
        {
            get { return (Brush)GetValue(InfoPanelCorFonteValoresProperty); }
            set { SetValue(InfoPanelCorFonteValoresProperty, value); }
        }
        private static void OnInfoPanelCorFonteValoresChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.paineisContainers.EnforceInfoPanelUpdate();
        }
        #endregion InfoPanelCorFonteValores

        #region InfoPanelFonte
        ///<summary>
        /// Obtém ou define a fonte para o info panel.
        ///</summary>
        public static readonly DependencyProperty InfoPanelFonteProperty;
        ///<summary>
        /// Obtém ou define a fonte para o info panel.
        ///</summary>
        public FontFamily InfoPanelFonte
        {
            get { return (FontFamily)GetValue(InfoPanelFonteProperty); }
            set { SetValue(InfoPanelFonteProperty, value); }
        }
        private static void OnInfoPanelFonteChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.paineisContainers.EnforceInfoPanelUpdate();
        }
        #endregion InfoPanelFonte

        #region InfoPanelTamanhoFonte
        ///<summary>
        /// Obtém ou define o tamanho da fonte do info panel.
        ///</summary>
        public static readonly DependencyProperty InfoPanelTamanhoFonteProperty;
        ///<summary>
        /// Obtém ou define o tamanho da fonte do info panel.
        ///</summary>
        public double InfoPanelTamanhoFonte
        {
            get { return (double)GetValue(InfoPanelTamanhoFonteProperty); }
            set { SetValue(InfoPanelTamanhoFonteProperty, value); }
        }
        private static void OnInfoPanelTamanhoFonteChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX.paineisContainers.EnforceInfoPanelUpdate();
        }
        #endregion InfoPanelTamanhoFonte

        #region InfoPanelPosicao
        ///<summary>
        /// Obtém ou define a posição do info panel.
        ///</summary>
        public static readonly DependencyProperty InfoPanelPosicaoProperty;
        ///<summary>
        /// Obtém ou define a posição do info panel.
        ///</summary>
        public EnumGeral.InfoPanelPosicaoEnum InfoPanelPosicao
        {
            get { return (EnumGeral.InfoPanelPosicaoEnum)GetValue(InfoPanelPosicaoProperty); }
            set { SetValue(InfoPanelPosicaoProperty, value); }
        }
        #endregion InfoPanelPosicao

        /// <summary>
        /// Evento disparado quando a posição do info panel muda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private static void OnInfoPanelPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            EnumGeral.InfoPanelPosicaoEnum positionEnum = (EnumGeral.InfoPanelPosicaoEnum)eventArgs.NewValue;

            if (positionEnum == EnumGeral.InfoPanelPosicaoEnum.Escondido || positionEnum == EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse)
            {
                chartX._timers.StopTimerWork(TimerInfoPanel);
                chartX.paineisContainers.EscondeInfoPanel();
                return;
            }

            if (positionEnum != EnumGeral.InfoPanelPosicaoEnum.Fixo)
                return;

            chartX._timers.StartTimerWork(TimerInfoPanel);
            chartX.paineisContainers.TornaInfoPanelEstatico();
        }
        #endregion InfoPanel

        #region Heat Panel Labels

        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> to apply to labels in heat panel.
        ///</summary>
        public static readonly DependencyProperty HeatPanelLabelsForegroundProperty;
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> to apply to labels in heat panel.
        ///</summary>
        public Brush HeatPanelLabelsForeground
        {
            get { return (Brush)GetValue(HeatPanelLabelsForegroundProperty); }
            set { SetValue(HeatPanelLabelsForegroundProperty, value); }
        }
        private static void OnHeatPanelLabelsForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            if (chartX.paineisContainers != null)
                chartX.paineisContainers.ResetHeatMapPanels();
        }

        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> to apply to labels background
        ///</summary>
        public static readonly DependencyProperty HeatPanelLabelsBackgroundProperty;
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> to apply to labels background
        ///</summary>
        public Brush HeatPanelLabelsBackground
        {
            get { return (Brush)GetValue(HeatPanelLabelsBackgroundProperty); }
            set { SetValue(HeatPanelLabelsBackgroundProperty, value); }
        }
        private static void OnHeatPanelLabelsBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            if (chartX.paineisContainers != null)
                chartX.paineisContainers.ResetHeatMapPanels();
        }

        ///<summary>
        /// Gets or sets the font-size for labels used in heat-panel
        ///</summary>
        public static readonly DependencyProperty HeatPanelLabelsFontSizeProperty;
        ///<summary>
        /// Gets or sets the font-size for labels used in heat-panel
        ///</summary>
        public double HeatPanelLabelsFontSize
        {
            get { return (double)GetValue(HeatPanelLabelsFontSizeProperty); }
            set { SetValue(HeatPanelLabelsFontSizeProperty, value); }
        }
        private static void OnHeatPanelLabelFontSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            if (chartX.paineisContainers != null)
                chartX.paineisContainers.ResetHeatMapPanels();
        }
        #endregion

        #region ManterNivelZoom
        /// <summary>
        /// Obtem ou define o valor que indica se o gráfico deve manter o nível atual de zoom do usuário.
        /// </summary>
        /// <value>
        /// True - Quando uma nova barra é adicionada ao gráfico, será deslocada para a esquerda, desta forma, a ultima barra sempre será vista, e o número de barras visíveis será mantido.
        /// False - Quando uma nova barra é adicionada, o gráfico será comprimido para mostrar a ultima barra.
        /// </value>
        public static readonly DependencyProperty ManterNivelZoomProperty;
        /// <summary>
        /// Obtem ou define o valor que indica se o gráfico deve manter o nível atual de zoom do usuário.
        /// </summary>
        /// <value>
        /// True - Quando uma nova barra é adicionada ao gráfico, será deslocada para a esquerda, desta forma, a ultima barra sempre será vista, e o número de barras visíveis será mantido.
        /// False - Quando uma nova barra é adicionada, o gráfico será comprimido para mostrar a ultima barra.
        /// </value>
        public bool ManterNivelZoom
        {
            get { return (bool)GetValue(ManterNivelZoomProperty); }
            set { SetValue(ManterNivelZoomProperty, value); }
        }

        #endregion ManterNivelZoom

        #region Chart Scroller

        #region ChartScrollerVisibleProperty (DependencyProperty)

        /// <summary>
        /// A description of the ChartScrollerVisible.
        /// </summary>
        public bool ChartScrollerVisible
        {
            get { return (bool)GetValue(ChartScrollerVisibleProperty); }
            set { SetValue(ChartScrollerVisibleProperty, value); }
        }

        /// <summary>
        /// ChartScrollerVisible
        /// </summary>
        public static readonly DependencyProperty ChartScrollerVisibleProperty =
          DependencyProperty.Register("ChartScrollerVisible", typeof(bool), typeof(StockChartX),
                                      new PropertyMetadata(true, OnChartScrollerVisibleChanged));

        private static void OnChartScrollerVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnChartScrollerVisibleChanged(e);
        }

        protected virtual void OnChartScrollerVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            ShowHideChartScroller();
        }

        #endregion

        #region ChartScrollerTrackBackgroundProperty (DependencyProperty)

        /// <summary>
        /// A description of the ChartScrollerTrackBackground.
        /// </summary>
        public Brush ChartScrollerTrackBackground
        {
            get { return (Brush)GetValue(ChartScrollerTrackBackgroundProperty); }
            set { SetValue(ChartScrollerTrackBackgroundProperty, value); }
        }

        /// <summary>
        /// ChartScrollerTrackBackground
        /// </summary>
        public static readonly DependencyProperty ChartScrollerTrackBackgroundProperty =
          DependencyProperty.Register("ChartScrollerTrackBackground", typeof(Brush), typeof(StockChartX),
                                      new PropertyMetadata(Brushes.Silver, OnChartScrollerTrackBackgroundChanged));

        private static void OnChartScrollerTrackBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnChartScrollerTrackBackgroundChanged(e);
        }

        protected virtual void OnChartScrollerTrackBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_chartScroller != null)
                _chartScroller.TrackBackground = (Brush)e.NewValue;
        }

        #endregion Chart Scroller

        #region ChartScrollerTrackButtonsBackgroundProperty (DependencyProperty)

        /// <summary>
        /// A description of the ChartScrollerTrackButtonsBackground.
        /// </summary>
        public Brush ChartScrollerTrackButtonsBackground
        {
            get { return (Brush)GetValue(ChartScrollerTrackButtonsBackgroundProperty); }
            set { SetValue(ChartScrollerTrackButtonsBackgroundProperty, value); }
        }

        /// <summary>
        /// ChartScrollerTrackButtonsBackground
        /// </summary>
        public static readonly DependencyProperty ChartScrollerTrackButtonsBackgroundProperty =
          DependencyProperty.Register("ChartScrollerTrackButtonsBackground", typeof(Brush), typeof(StockChartX),
                                      new PropertyMetadata(Brushes.Green, OnChartScrollerTrackButtonsBackgroundChanged));

        private static void OnChartScrollerTrackButtonsBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnChartScrollerTrackButtonsBackgroundChanged(e);
        }

        protected virtual void OnChartScrollerTrackButtonsBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_chartScroller != null)
                _chartScroller.TrackButtonsBackground = (Brush)e.NewValue;
        }

        #endregion

        #region ChartScrollerThumbButtonBackgroundProperty (DependencyProperty)

        /// <summary>
        /// A description of the ChartScrollerThumbButtonBackground.
        /// </summary>
        public Brush ChartScrollerThumbButtonBackground
        {
            get { return (Brush)GetValue(ChartScrollerThumbButtonBackgroundProperty); }
            set { SetValue(ChartScrollerThumbButtonBackgroundProperty, value); }
        }

        /// <summary>
        /// ChartScrollerThumbButtonBackground
        /// </summary>
        public static readonly DependencyProperty ChartScrollerThumbButtonBackgroundProperty =
          DependencyProperty.Register("ChartScrollerThumbButtonBackground", typeof(Brush), typeof(StockChartX),
                                      new PropertyMetadata(new LinearGradientBrush
                                                             {
                                                                 StartPoint = new Point(0.486, 0),
                                                                 EndPoint = new Point(0.486, 0.986),
                                                                 GradientStops = new GradientStopCollection
                                                                             {
                                                                               new GradientStop
                                                                                 {
                                                                                   Color = ColorsEx.Gray,
                                                                                   Offset = 0
                                                                                 },
                                                                               new GradientStop
                                                                                 {
                                                                                   Color = ColorsEx.MidnightBlue,
                                                                                   Offset = 0.5
                                                                                 },
                                                                               new GradientStop
                                                                                 {
                                                                                   Color = ColorsEx.Gray,
                                                                                   Offset = 1
                                                                                 }
                                                                             }
                                                             }, OnChartScrollerThumbButtonBackgroundChanged));

        private static void OnChartScrollerThumbButtonBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnChartScrollerThumbButtonBackgroundChanged(e);
        }

        protected virtual void OnChartScrollerThumbButtonBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_chartScroller != null)
                _chartScroller.ThumbButtonBackground = (Brush)e.NewValue;
        }

        #endregion

        #endregion

        #region LineStudies dialog background
        ///<summary>
        /// Gets or sets the background for the dialog properties if LineStudy objects
        ///</summary>
        public static DependencyProperty LineStudyPropertyDialogBackgroundProperty;
        ///<summary>
        /// /// Gets or sets the background for the dialog properties if LineStudy objects
        ///</summary>
        public Brush LineStudyPropertyDialogBackground
        {
            get { return (Brush)GetValue(LineStudyPropertyDialogBackgroundProperty); }
            set { SetValue(LineStudyPropertyDialogBackgroundProperty, value); }
        }

        #endregion

        #region ShowSecondsProperty (DependencyProperty)

        /// <summary>
        /// Obtém ou define quando mostrar os segundos no painel calendário. Não funciona quando <see cref="RealTimeXLabels"/> = false.
        /// </summary>
        public bool MostrarSegundos
        {
            get { return (bool)GetValue(MostraSegundosProperty); }
            set { SetValue(MostraSegundosProperty, value); }
        }

        /// <summary>
        /// Obtém ou define quando mostrar os segundos no painel calendário. Não funciona quando <see cref="RealTimeXLabels"/> = false.
        /// </summary>
        public static readonly DependencyProperty MostraSegundosProperty =
          DependencyProperty.Register("ShowSeconds", typeof(bool), typeof(StockChartX),
                                      new PropertyMetadata(false, OnMostraSegundosChanged));

        private static void OnMostraSegundosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnMostraSegundosChanged(e);
        }

        protected virtual void OnMostraSegundosChanged(DependencyPropertyChangedEventArgs e)
        {
            if (calendario == null)
                return;

            calendario.Paint();
        }

        #endregion
    }
}

