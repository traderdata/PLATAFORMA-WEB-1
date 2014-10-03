using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
#endif


namespace Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies
{
    /// <summary>
    /// Base class for all line studies used in the chart
    /// </summary>
    [CLSCompliant(true)]
    public partial class LineStudy : IChartElementPropertyAble
    {
        #region Suporte e Resistencia

        private bool suporte = false;
        private bool resistencia = false;
        public bool Suporte
        {
            get { return suporte; }
            set { suporte = value;}
        }
        public bool Resistencia
        {
            get { return resistencia; }
            set { resistencia = value; }
        }


        #endregion

        #region Linha Infinita para a Direita

        private bool linhaInfinitaADireita = false;
        private double qtdRecordsOriginalLinhaDireita = 0;
        private static int valorFuturoLinhaInfinita = 8000;

        /// <summary>
        /// Obtém ou define (aplica) a parte direita da linha como sendo infinita. 
        /// </summary>
        public bool LinhaInfinitaADireita
        {
            get { return linhaInfinitaADireita; }
            set
            {
                //Não deve funcionar em conjunto com a linha magnetica
                if ((linhaMagnetica) && (value == true))
                    return;

                /* Fórmulas:
                * 1) M = (Y - Ya) / (X - Xa)
                * 2) Y - YA = M(X - Xa)*/

                valorFuturoLinhaInfinita = 8000;

                //Ativando extensao
                if ((!linhaInfinitaADireita) && (value == true) && (false))
                {

                    _x1 = _chartX.GetXPixel(_x1Value);
                    _x2 = _chartX.GetXPixel(_x2Value);
                    //_y1 = _chartX.GetYPixel(_y1Value).Value;
                    //_y2 = _chartX.GetYPixel(_y2Value).Value;


                    //Obtendo posicoes iniciais
                    qtdRecordsOriginalLinhaDireita = Math.Abs(_x1Value - _x2Value);

                    //Obtendo coordenada futura do x2
                    double x2Aux = _chartX.GetXPixel(_x2Value + valorFuturoLinhaInfinita - _chartX.indexInicial, true);

                    //Obtendo coeficiente da regua
                    double m = (_y2 - _y1) / (_x2 - _x1);

                    //Usando fórmula 2 para determinar a nova coordenada de y2
                    _y2 = (m * (x2Aux - _x1)) + _y1;

                    //Essa será o novo valor de x2
                    _x2Value += valorFuturoLinhaInfinita;

                    //Essa será a nova coordenada de x2
                    _x2 = _chartX.GetXPixel(_x2Value - _chartX.indexInicial, true);

                    //Obtendo o valor correspondente ao pixel
                    _y2Value = _chartPanel.GetReverseY(_y2);

                    Update();
                }

                //Desativando extensao
                if ((linhaInfinitaADireita) && (value == false))
                {
                    //Obtendo coeficiente da regua
                    double m = (_y2 - _y1) / (_x2 - _x1);

                    //Obtendo coordenada futura do x2
                    double x2Aux = _chartX.GetXPixel(_x1Value + qtdRecordsOriginalLinhaDireita, true);

                    //Usando fórmula 2 para determinar a nova coordenada de y2
                    _y2 = (m * (x2Aux - _x1)) + _y1;

                    //Essa será o novo valor
                    _x2Value = _x1Value + qtdRecordsOriginalLinhaDireita;

                    //Essa será a nova coordenada de x2
                    _x2 = _chartX.GetXPixel(_x2Value, true);

                    //Obtendo o valor correspondente ao pixel
                    _y2Value = _chartPanel.GetReverseY(_y2);

                    Update();
                }

                //Atualizando o valor da variavel local
                linhaInfinitaADireita = value;
            }
        }

        

        #endregion Linha Infinita para a Direita

        /// <summary>
        /// Line Study Types
        /// </summary>
        public enum StudyTypeEnum
        {
            /// <summary>
            /// Observação
            /// </summary>
            Observacao,
            /// <summary>
            /// Régua
            /// </summary>
            Regua,
            /// <summary>
            /// Ellipse
            /// </summary>
            Ellipse,
            /// <summary>                        
            /// Rectangle
            /// </summary>
            Rectangle,
            /// <summary>
            /// Trend Line
            /// </summary>
            TrendLine,
            /// <summary>
            /// Speed Lines
            /// </summary>
            SpeedLines,
            /// <summary>
            /// Gann Fan
            /// </summary>
            GannFan,
            /// <summary>
            /// Fibonacci Arcs
            /// </summary>
            FibonacciArcs,
            /// <summary>
            /// Fibonacci Fan
            /// </summary>
            FibonacciFan,
            /// <summary>
            /// Fibonacci Retracements
            /// </summary>
            FibonacciRetracements,
            /// <summary>
            /// Fibonacci Time Zones
            /// </summary>
            FibonacciTimeZones,
            /// <summary>
            /// Tirone Levels
            /// </summary>
            TironeLevels,
            /// <summary>
            /// Quadrant Lines
            /// </summary>
            QuadrantLines,
            /// <summary>
            /// Raff Regression
            /// </summary>
            RaffRegression,
            /// <summary>
            /// Error Channel
            /// </summary>
            ErrorChannel,
            /// <summary>
            /// used for buy, sell and exit Symbols - images
            /// </summary>
            SymbolObject,
            /// <summary>
            /// Horizontal line
            /// </summary>
            HorizontalLine,
            /// <summary>
            /// Vertical Line
            /// </summary>
            VerticalLine,
            /// <summary>
            /// Image Object
            /// </summary>
            ImageObject,
            /// <summary>
            /// Static Text
            /// </summary>
            StaticText,
            /// <summary>
            /// User defined text (not yet supported)
            /// </summary>
            UserDefinedText,
            /// <summary>
            /// WPF UI Element
            /// </summary>
            FrameworkElement,

            /// <summary>
            /// Unknown
            /// </summary>
            Unknown
        }

        /// <summary>
        /// for internal usage only
        /// </summary>
        internal enum LineStatus
        {
            StartPaint, Painting, EndPaint,
            StartMove, Moving, EndMove,
            Selected, Normal,
            RePaint, InserindoDiretamente
        }

        private bool linhaMagnetica = false;

        public bool LinhaMagnetica
        {
            get { return linhaMagnetica; }
            set 
            {
                //A linha magnetica deve impedir o uso de linha infinita
                if (value == true)
                    LinhaInfinitaADireita = false;

                linhaMagnetica = value; 
            }
        }

        public bool PainelIndicador { get; set; }

        internal bool _selectable;

        private double _strokeThickness;
        private Brush _stroke;
        private EnumGeral.TipoLinha _strokeType;
        private double _opacity = 1.0;

        //internal int _zOrder;
        internal bool _selected;
        internal bool _drawing;
        internal bool _drawn;
        internal bool _internalObjectCreated; //gets if the internal object used to paint the lineStudy was created
        internal String _key;
        internal bool _selectionVisible;
        internal object _extraArgs;
        internal double _x1;
        internal double _y1;
        internal double _x2;
        internal double _y2;
        internal double _x1Value; //Chart values
        internal double _y1Value;
        internal double _x2Value;
        internal double _y2Value;
        internal double?[] _params = new double?[Constants.MaxParams];
        internal StudyTypeEnum _studyType;
        internal StockChartX _chartX;

        internal Types.Corner _resizingCorner;
        internal ChartPanel _chartPanel;
        internal PaintObjectsManager<SelectionDot> _selectionDots = new PaintObjectsManager<SelectionDot>();
        internal class SelectionDotInfo
        {
            public Types.Corner Corner;
            public Point Position;
            public bool Clickable = true;
        }

        internal LineStudy()
        {
            _stroke = Brushes.Red;

            Initialize();
        }

        internal LineStudy(string key, Brush stroke, ChartPanel chartPanel)
        {
            _key = key;
            _stroke = stroke;

            if (chartPanel != null)
                SetChartPanel(chartPanel);

            Initialize();
        }

        internal void SetChartPanel(ChartPanel chartPanel)
        {
            _chartPanel = chartPanel;
            _chartX = _chartPanel._chartX;
        }

        internal void Initialize()
        {
            _x1 = _y1 = _x2 = _y2 = 0.0;
            _x1Value = _y1Value = 0.0;
            _x2Value = _y2Value = 0.0;
            _strokeThickness = 1;
            _strokeType = EnumGeral.TipoLinha.Solido;
            _selectable = true;
            _selected = false;
            _drawn = false;
            _drawing = false;
            _selectionVisible = false;
        }

        /// <summary>
        /// Sets chart value lookup based on actual pixel position
        /// </summary>
        internal virtual void Reset()
        {
            if (_x1 < 1 || _x2 < 1) return;

            _x1Value = _chartX.GetReverseXInternal(_x1) + _chartX.indexInicial;
            _x2Value = _chartX.GetReverseXInternal(_x2) + _chartX.indexInicial;
            _y1Value = _chartPanel.GetReverseY(_y1);
            _y2Value = _chartPanel.GetReverseY(_y2);

            _x1 = _chartX.GetXPixel(_x1Value - _chartX.indexInicial);
            _x2 = _chartX.GetXPixel(_x2Value - _chartX.indexInicial);
            _y1 = _chartPanel.GetY(_y1Value);
            _y2 = _chartPanel.GetY(_y2Value);
        }

        internal void Update()
        {
            if (_chartPanel == null)
                return;

            _x1 = _chartX.GetXPixel(_x1Value - _chartX.indexInicial, true);
            _x2 = _chartX.GetXPixel(_x2Value - _chartX.indexInicial, true);
            _y1 = _chartPanel.GetY(_y1Value);
            _y2 = _chartPanel.GetY(_y2Value);
        }

        internal double _dStartX, _dStartY;
        internal Types.RectEx _newRect;
        internal Types.RectEx _oldRect;
        internal void Paint(double x, double y, LineStatus lineStatus)
        {
            if (C == null)
                return;

            if (lineStatus == LineStatus.RePaint)
                Update();
            if (lineStatus == LineStatus.Moving)
                if (_resizingCorner == Types.Corner.None)
                    Paint(x, y, lineStatus, Types.Corner.MoveAll);
                else
                    Paint(x, y, lineStatus, _resizingCorner);
            else
                Paint(x, y, lineStatus, Types.Corner.None);
        }
        internal void Paint(double x, double y, LineStatus lineStatus, Types.Corner corner)
        {
            if (!_drawn && lineStatus == LineStatus.RePaint)
            {
                _drawn = true;
                Update();
                DrawLineStudy(new Types.RectEx(), LineStatus.StartPaint);
            }
            switch (lineStatus)
            {

                case LineStatus.StartPaint:
                    _x1 = _dStartX = x;
                    _y1 = _dStartY = y;
                    DrawLineStudy(new Types.RectEx(), lineStatus);

                    //...
                    _x1Value = _x2Value = _chartX.MouseX;
                    _y1Value = _y2Value = _chartX.MouseY;
                    

                    MovendoComMagnetico(MoveMagnetico.Esquerda);
                    MovendoRegua();

                    if (linhaMagnetica)
                        _dStartY = _y1;
                    break;

                case LineStatus.Painting:
                    _x2 = x;
                    _y2 = y;
                    MovendoComMagnetico(MoveMagnetico.Direita);

                    if (linhaMagnetica)
                        y = _y2;

                    _y2Value = _chartX.MouseY;

                    SetRect(x, y);
                    DrawLineStudy(_newRect, lineStatus);

                    //Movendo o painel da regua
                    MovendoRegua();
                    
                    break;

                case LineStatus.EndPaint:
                    _x2 = x;
                    _y2 = y;
                    MovendoComMagnetico(MoveMagnetico.Direita);

                    if (linhaMagnetica)
                        y = _y2;

                    SetXY(x, y, lineStatus);
                    _dStartX = _dStartY = 0.0;
                    Reset();

                    _newRect = new Types.RectEx(_x1, _y1, _x2, _y2);
                    DrawLineStudy(_newRect, lineStatus);
                    MovendoRegua();
                    break;

                case LineStatus.RePaint:
                    DrawLineStudy(_newRect = new Types.RectEx(_x1, _y1, _x2, _y2), lineStatus);
                    MovendoRegua();
                    break;

                case LineStatus.StartMove:
                    _dStartX = x;
                    _dStartY = y;

                    MovendoRegua();
                    MovendoComMagnetico(MoveMagnetico.Ambos);

                    //Atualizando datas originais
                    dataOriginalInicial = DataInicial;
                    dataOriginalFinal = DataFinal;
                    break;

                case LineStatus.Moving:
                    switch (corner)
                    {
                        case Types.Corner.BottomRight:
                            _x2 = x;
                            _y2 = y;

                            _y2Value = _chartX.MouseY;

                            MovendoComMagnetico(MoveMagnetico.Direita);
                            MovendoRegua();
                            break;
                        case Types.Corner.MiddleRight:
                            _x2 = x;

                            _y2Value = _chartX.MouseY;

                            MovendoComMagnetico(MoveMagnetico.Direita);
                            MovendoRegua();
                            break;
                        case Types.Corner.TopRight:
                            _x2 = x;
                            _y1 = y;

                            _y2Value = _chartX.MouseY;

                            MovendoComMagnetico(MoveMagnetico.Direita);
                            MovendoRegua();
                            break;
                        case Types.Corner.TopCenter:
                            _y1 = y;

                            MovendoComMagnetico(MoveMagnetico.Ambos);
                            MovendoRegua();
                            break;
                        case Types.Corner.TopLeft:
                            _x1 = x;
                            _y1 = y;

                            _y1Value = _chartX.MouseY;

                            MovendoComMagnetico(MoveMagnetico.Esquerda);
                            MovendoRegua();
                            break;
                        case Types.Corner.MiddleLeft:
                            _x1 = x;


                            _y1Value = _chartX.MouseY;

                            MovendoComMagnetico(MoveMagnetico.Esquerda);
                            MovendoRegua();
                            break;
                        case Types.Corner.BottomLeft:
                            _x1 = x;
                            _y2 = y;

                            _y1Value = _chartX.MouseY;

                            MovendoComMagnetico(MoveMagnetico.Esquerda);
                            MovendoRegua();
                            break;
                        case Types.Corner.BottomCenter:
                            _y2 = y;

                            MovendoComMagnetico(MoveMagnetico.Ambos);
                            MovendoRegua();
                            break;
                        case Types.Corner.MoveAll:
                            _x1 -= (_dStartX - x);
                            _y1 -= (_dStartY - y);
                            _x2 -= (_dStartX - x);
                            _y2 -= (_dStartY - y);
                            _dStartX = x;
                            _dStartY = y;
                            MovendoRegua();
                            MovendoComMagnetico(MoveMagnetico.Ambos);
                            break;
                    }



                    _newRect = new Types.RectEx(_x1, _y1, _x2, _y2);
                    DrawLineStudy(_newRect, lineStatus);
                    ShowSelection(true);
                    _oldRect = _newRect;

                    //Atualizando datas originais
                    dataOriginalInicial = DataInicial;
                    dataOriginalFinal = DataFinal;

                    break;
                case LineStatus.EndMove:
                    SetXY(x, y, lineStatus);
                    Reset();

                    _newRect = new Types.RectEx(_x1, _y1, _x2, _y2);
                    DrawLineStudy(_newRect, lineStatus);
                    _resizingCorner = Types.Corner.None;

                    //Atualizando datas originais
                    dataOriginalInicial = DataInicial;
                    dataOriginalFinal = DataFinal;
                    break;
            }
            if (lineStatus == LineStatus.RePaint)
                ShowSelection(_selected);

            _drawn = true;


        }

        private enum MoveMagnetico { Esquerda, Direita, Ambos };
        private void MovendoComMagnetico(MoveMagnetico tipo)
        {
            if (linhaMagnetica)
            {
                double mouseY = _chartX.MouseY;

                Series serieMax = _chartX.GetSeriesByName(_chartX.Symbol + ".Maximo");
                Series serieMinimo = _chartX.GetSeriesByName(_chartX.Symbol + ".Minimo");

                if ((tipo == MoveMagnetico.Esquerda) || (tipo == MoveMagnetico.Ambos))
                {
                    double? maximoX1 = _chartX.GetValue(serieMax, _chartX.GetRealReverseX(_x1)) ?? _chartX.MouseY;
                    double? minimoX1 = _chartX.GetValue(serieMinimo, _chartX.GetRealReverseX(_x1)) ?? _chartX.MouseY;

                    //Se o Valor de y1 for maior que a média do candle correspondente, y1 deve receber o valor maximo
                    if (mouseY > (maximoX1 + minimoX1) / 2)
                        _y1Value = (double)maximoX1;
                    else
                        _y1Value = (double)minimoX1;

                    _y1 = (double)_chartX.GetYPixel(_y1Value);
                }


                if ((tipo == MoveMagnetico.Direita) || (tipo == MoveMagnetico.Ambos))
                {
                    double? maximoX2 = _chartX.GetValue(serieMax, _chartX.GetRealReverseX(_x2)) ?? _chartX.MouseY;
                    double? minimoX2 = _chartX.GetValue(serieMinimo, _chartX.GetRealReverseX(_x2)) ?? _chartX.MouseY;

                    //Se o Valor de y2 for maior que a média do candle correspondente, y1 deve receber o valor maximo
                    if (mouseY > (maximoX2 + minimoX2) / 2)
                        _y2Value = (double)maximoX2;
                    else
                        _y2Value = (double)minimoX2;

                    _y2 = (double)_chartX.GetYPixel(_y2Value);
                }
            }
        }

        private void MovendoRegua()
        {
            Regua regua = this as Regua;

            if (regua != null)
            {
                regua.PainelRegua.Visibility = Visibility.Visible;

                //Se a posicao atual mais o tamanho do painel for menor que o tamanho do util do grafico (tirando a escala)
                double tamanhoUtilXChart = _chartPanel.ActualWidth - _chartPanel._rightYAxis.ActualWidth;
                double posicaoX = _chartPanel.PosicaoAtual.X;
                //double posicaoX = _x2Value;
                if (posicaoX + regua.PainelRegua.ActualWidth < tamanhoUtilXChart)
                {
                    if (posicaoX > 0)
                        Canvas.SetLeft(regua.PainelRegua, _x2/*posicaoX*/);
                    else
                        Canvas.SetLeft(regua.PainelRegua, 0);
                }
                else
                    Canvas.SetLeft(regua.PainelRegua, tamanhoUtilXChart - regua.PainelRegua.ActualWidth);

                //Se a posicao atual mais o tamanho do painel for menor que o tamanho do util do grafico (tirando a escala)
                double tamanhoUtilYChart = _chartPanel.ActualHeight - _chartPanel._titleBar.ActualHeight;
                double posicaoY = _chartPanel.PosicaoAtual.Y;
                //double posicaoY = _y2Value;
                if (posicaoY + regua.PainelRegua.ActualHeight < tamanhoUtilYChart)
                {
                    if (posicaoY > 0)
                        Canvas.SetTop(regua.PainelRegua, _y2/*posicaoY*/);
                    else
                        Canvas.SetTop(regua.PainelRegua, 0);
                }
                else
                    Canvas.SetTop(regua.PainelRegua, tamanhoUtilYChart - regua.PainelRegua.ActualHeight);

                double valorInicial = _y1Value;

                double valorFinal = _y2Value;

                if (linhaMagnetica)
                    valorFinal = _y2Value;

                regua.DataInicialRegua = _chartX.GetTimestampByIndex(Convert.ToInt32(_x1Value)) ?? new DateTime();
                regua.DataFinalRegua = _chartX.GetTimestampByIndex(Convert.ToInt32(_chartX.MouseX)) ?? new DateTime(); ;
                regua.ValorInicialRegua = valorInicial;
                regua.ValorFinalRegua = valorFinal;
                regua.VariacaoPercentRegua = ((valorFinal - valorInicial) * 100) / valorInicial;
                regua.VariacaoValorRegua = valorFinal - valorInicial;
            }
        }

        #region SetaValores()
        /// <summary>
        /// Seta valores, mas não atualiza posicao imediatamento.
        /// </summary>
        /// <param name="x1Value"></param>
        /// <param name="y1Value"></param>
        /// <param name="x2Value"></param>
        /// <param name="y2Value"></param>
        public void SetaValores(double x1Value, double y1Value, double x2Value, double y2Value)
        {
            _x1Value = x1Value;
            _x2Value = x2Value;
            _y1Value = y1Value;
            _y2Value = y2Value;
        }
        #endregion SetaValores()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1Value">X1 record index</param>
        /// <param name="y1Value">Y1 price value</param>
        /// <param name="x2Value">X2 record index</param>
        /// <param name="y2Value">Y2 price value</param>
        public virtual void SetCoordenadas(double x1Value, double y1Value, double x2Value, double y2Value)
        {
            _x1Value = x1Value;
            _x2Value = x2Value;
            _y1Value = y1Value;
            _y2Value = y2Value;

            Update();

            _newRect.Left = _chartX.GetXPixel(x1Value);
            _newRect.Right = _chartX.GetXPixel(x2Value);
            _newRect.Top = _chartX.GetYPixel(Math.Round(y1Value, 2)) ?? 0;
            _newRect.Bottom = _chartX.GetYPixel(Math.Round(y2Value, 2)) ?? 0;

            if (_chartX.Status == StockChartX.StatusGrafico.Preparado && C != null)
                DrawLineStudy(_newRect, LineStatus.RePaint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1Value">X1 record index</param>
        /// <param name="y1Value">Y1 price value</param>
        /// <param name="x2Value">X2 record index</param>
        /// <param name="y2Value">Y2 price value</param>
        public virtual void SetCoordenadas(Types.RectEx rect)
        {
            _x1Value = rect.Left;
            _x2Value = rect.Right;
            _y1Value = rect.Top;
            _y2Value = rect.Bottom;

            Update();

            _newRect.Left = rect.Left;
            _newRect.Right = rect.Right;
            _newRect.Top = rect.Top;
            _newRect.Bottom = rect.Bottom;

            if (_chartX.Status == StockChartX.StatusGrafico.Preparado && C != null)
                DrawLineStudy(_newRect, LineStatus.RePaint);
        }

        public virtual void SetaParametros(string parametros, char strConcatenacao)
        {
            if (parametros == null)
                return;
        }

        public virtual string ObtemParametros(char strConcatenacao)
        {
            return "";
        }

        internal void SetXY(double x, double y, LineStatus lineStatus)
        {
            switch (lineStatus)
            {
                case LineStatus.EndPaint:
                    _x1 = _dStartX;
                    _y1 = _dStartY;
                    _x2 = x;
                    _y2 = y;
                    if (_x1 > _x2 && _studyType != StudyTypeEnum.ImageObject)
                    {
                        Utils.Swap(ref _x1, ref _x2);
                        Utils.Swap(ref _y1, ref _y2);
                    }
                    break;
                case LineStatus.EndMove:
                    _x1 = _newRect.Left;
                    _y1 = _newRect.Top;
                    _x2 = _newRect.Right;
                    _y2 = _newRect.Bottom;
                    break;
            }
        }

        internal void SetRect(double x, double y)
        {
            _newRect.Left = _dStartX;
            _newRect.Right = x;
            _newRect.Top = _dStartY;
            _newRect.Bottom = y;

            if (_newRect.Right < _x1)
            {
                _newRect.Left = _x1 + 5;
            }
        }

        /// <summary>
        /// Selects or diselects a line study
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (!_selectable) return;
                if (_selected == value || !_internalObjectCreated) return;
                ShowSelection(_selected = value);
            }
        }

        /// <summary>
        /// Gets the unique key that is associated with current line study
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets either the line study is selectable or not
        /// </summary>
        public bool Selectable
        {
            get { return _selectable; }
            set { _selectable = value; }
        }

        /// <summary>
        /// Gets the study type
        /// </summary>
        public StudyTypeEnum StudyType
        {
            get { return _studyType; }
        }

        /// <summary>
        /// Start record
        /// </summary>
        public double X1Value
        {
            get { return _x1Value; }
        }

        /// <summary>
        /// Start price value
        /// </summary>
        public double Y1Value
        {
            get { return _y1Value; }
        }

        /// <summary>
        /// End Record
        /// </summary>
        public double X2Value
        {
            get { return _x2Value; }
        }

        /// <summary>
        /// End price value
        /// </summary>
        public double Y2Value
        {
            get { return _y2Value; }
        }

        /// <summary>
        /// Sets the color of lineStudy, when LineStudy is a text object it will set the Foreground color
        /// </summary>
        public Brush Stroke
        {
            get { return _stroke; }
            set
            {
                if (_stroke == value) return;
                _stroke = value;
                if (_internalObjectCreated)
                    SetStroke();
            }
        }

        /// <summary>
        /// Sets the thicknes if lines used to paint the lineStudy. In case of a text object it sets the font size
        /// </summary>
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                if (value == _strokeThickness) return;
                _strokeThickness = value;
                if (_internalObjectCreated)
                    SetStrokeThickness();
            }
        }

        /// <summary>
        /// Sets the stroke type (solid, dash, dot, ...) for lines used to paint the <see cref="LineStudy"/>
        /// </summary>
        public EnumGeral.TipoLinha StrokeType
        {
            get { return _strokeType; }
            set
            {
                if (_strokeType == value) return;
                _strokeType = value;
                if (_internalObjectCreated)
                    SetStrokeType();
            }
        }

        ///<summary>
        /// Gets or sets the opacity of <see cref="LineStudy"/>
        ///</summary>
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (_opacity == value) return;
                _opacity = value;
                if (_internalObjectCreated)
                    SetOpacity();
            }
        }

        /// <summary>
        /// Gets the panel that has ownership on this <see cref="LineStudy"/>
        /// </summary>
        public ChartPanel Panel
        {
            get { return _chartPanel; }
        }

        /// <summary>
        /// extra parameters that were passed when creating the <see cref="LineStudy"/>. 
        /// Used for Image object to set image path and for text object to set the text
        /// </summary>
        public object ExtraArgs
        { 
            get { return _extraArgs; }
        }

        internal Canvas C
        {
            get { return _chartPanel._rootCanvas; }
        }

        internal virtual void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            throw new NotImplementedException();
        }
        internal virtual List<SelectionDotInfo> GetSelectionPoints()
        {
            throw new NotImplementedException();
        }
        public virtual void InsereEstudoDiretamente(Types.RectEx rect)
        {
            throw new NotImplementedException();
        }
        internal virtual void SetCursor()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeThickness()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStroke()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeType()
        {
            throw new NotImplementedException();
        }
        internal virtual void RemoveLineStudy()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetOpacity()
        {
            throw new NotImplementedException();
        }

        internal void ShowSelection(bool bShow)
        {
            _selectionVisible = bShow;

            SetCursor();

            if (!_selectionVisible)
            {
                _selectionDots.RemoveAll();
                return;
            }
            _selectionDots.C = C;
            _selectionDots.Start();

            foreach (SelectionDotInfo point in GetSelectionPoints())
            {
                SelectionDot dot = _selectionDots.GetPaintObject(point.Corner, point.Clickable);
                dot.SetPos(point.Position);
                dot.Tag = this;
                dot.ZIndex = ZIndexConstants.SelectionPoint1;
            }

            _selectionDots.Stop();
        }

        internal Series GetSeriesOHLC(EnumGeral.TipoSerieOHLC ohlc)
        {
            return _chartPanel.SeriesCount == 0 ? null : _chartPanel.GetSeriesOHLCV(_chartPanel.FirstSeries, ohlc);
        }

        internal void StartResize(SelectionDot selectionDot, MouseButtonEventArgs e)
        {
            _resizingCorner = selectionDot.Corner;
            _chartX.Status = StockChartX.StatusGrafico.MovendoLinhaEstudo;
            C.CaptureMouse();
        }

        /// <summary>
        /// Called mainly by DataManager when a new record gets inserted before _x1 index value, in this case
        /// we must update X1 value so it will keep up with the needed record
        /// </summary>
        /// <param name="step"></param>
        internal void UpdatePosition(int step)
        {
            if (step >= _x1Value)
                return; //value was appended after
            //shift line study
            _x1Value += 1;
            _x2Value += 1;
        }

        internal virtual void SetArgs(params object[] args) { }

        /// <summary>
        /// Sets programmatically the logical coordinates of a <see cref="LineStudy"/>. Internally they are transformed to canvas coordinates
        /// every line study has its own logic for seting canvas coordinates, you must know very well what every line study does
        /// and what are its rules of paiting.
        /// </summary>
        /// <param name="x1Value">X1 record index</param>
        /// <param name="y1Value">Y1 price value</param>
        /// <param name="x2Value">X2 record index</param>
        /// <param name="y2Value">Y2 price value</param>
        public virtual void SetXYValues(double x1Value, double y1Value, double x2Value, double y2Value)
        {
            _x1Value = x1Value;
            _x2Value = x2Value;
            _y1Value = y1Value;
            _y2Value = y2Value;

            Update();

            _newRect.Left = _x1;
            _newRect.Right = _x2;
            _newRect.Top = _y1;
            _newRect.Bottom = Double.IsNaN(_y2) ? _y1 + 1 : _y2;

            if (_chartX.Status == StockChartX.StatusGrafico.Preparado && C != null)
                DrawLineStudy(_newRect, LineStatus.RePaint);
        }

        #region Implementation of IChartElementPropertyAble

        ///<summary>
        /// Gets <see cref="LineStudy"/>'s title
        ///</summary>
        public string Title
        {
            get { return StockChartX_LineStudiesParams.GetLineStudyFriendlyName(_studyType) + " Properties"; }
        }

        ///<summary>
        /// Gets <see cref="LineStudy"/>'s properties
        ///</summary>
        public IEnumerable<IChartElementProperty> Properties
        {
            get
            {
                foreach (var property in BaseProperties)
                {
                    yield return property;
                }
            }
        }

        #endregion

        internal ChartElementColorProperty propertyStroke;
        internal ChartElementStrokeThicknessProperty propertyStrokeThickness;
        internal ChartElementStrokeTypeProperty propertyStrokeType;
        internal ChartElementColorProperty propertyFill;
        internal ChartElementOpacityProperty propertyOpacity;

        /// <summary>
        /// Gets the basic properties that are common for all LineStudies
        /// </summary>
        protected virtual IEnumerable<IChartElementProperty> BaseProperties
        {
            get
            {
                GetChartElementColorProperty();
                yield return propertyStroke;

                GetChartElementStrokeThicknessProperty("Stroke Thickness");
                yield return propertyStrokeThickness;

                GetChartElementStrokeTypeProperty();
                yield return propertyStrokeType;

                GetChartElementOpacityProperty();
                yield return propertyOpacity;

                GetChartElementFillProperty();
                if (propertyFill != null)
                    yield return propertyFill;
            }
        }

        /// <summary>
        /// Fill Propeerty
        /// </summary>
        protected void GetChartElementFillProperty()
        {
            if (!(this is IShapeAble)) return;

            IShapeAble shapeAble = (IShapeAble)this;
            propertyFill = new ChartElementColorProperty("Fill Color");
            if (shapeAble.Fill is SolidColorBrush)
                propertyFill.ValuePresenter.Value = shapeAble.Fill;
            propertyFill.SetChartElementPropertyValue
              += presenter =>
                   {
                       shapeAble.Fill = (SolidColorBrush)presenter.Value;
                   };
        }

        /// <summary>
        /// Opacity Property
        /// </summary>
        protected void GetChartElementOpacityProperty()
        {
            propertyOpacity = new ChartElementOpacityProperty("Opacity");
            propertyOpacity.ValuePresenter.Value = _opacity;
            propertyOpacity.SetChartElementPropertyValue
              += presenter =>
                   {
                       Opacity = Convert.ToDouble(presenter.Value);
                   };
        }

        /// <summary>
        /// Stroke Property
        /// </summary>
        protected void GetChartElementStrokeTypeProperty()
        {
            propertyStrokeType = new ChartElementStrokeTypeProperty("Stroke Type");
            propertyStrokeType.ValuePresenter.Value = StrokeType.ToString();
            propertyStrokeType.SetChartElementPropertyValue
              += presenter =>
                   {
                       StrokeType = (EnumGeral.TipoLinha)System.Enum.Parse(typeof(EnumGeral.TipoLinha), presenter.Value.ToString()
#if SILVERLIGHT
, true
#endif
);
                       SetStrokeType();
                   };
        }

        /// <summary>
        /// Stroke Thickness property
        /// </summary>
        /// <param name="propertyName"></param>
        protected void GetChartElementStrokeThicknessProperty(string propertyName)
        {
            propertyStrokeThickness = new ChartElementStrokeThicknessProperty(propertyName);
            propertyStrokeThickness.ValuePresenter.Value = StrokeThickness;
            propertyStrokeThickness.SetChartElementPropertyValue
              += presenter =>
                   {
                       StrokeThickness = Convert.ToDouble(presenter.Value);
                       SetStrokeThickness();
                   };
        }

        /// <summary>
        /// Color property
        /// </summary>
        protected void GetChartElementColorProperty()
        {
            propertyStroke = new ChartElementColorProperty("Border Stroke Color");
            propertyStroke.ValuePresenter.Value = Stroke;
            propertyStroke.SetChartElementPropertyValue
              += presenter =>
                   {
                       Stroke = (SolidColorBrush)presenter.Value;
                       SetStroke();
                   };
        }

        /// <summary>
        /// Indica se a linha de estudo possui parametros.
        /// </summary>
        public bool UsaParametros
        {
            get
            {
                if ((_params.Length > 0) && (_params[0] != null))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Obtém a data correpondente ao record inicial deste objeto.
        /// </summary>
        public DateTime DataInicial
        {
            get 
            {
                return _chartX.GetTimestampByIndex(Convert.ToInt32(X1Value)) ?? new DateTime();
            }
        }

        /// <summary>
        /// Obtém a data correpondente ao record final deste objeto.
        /// </summary>
        public DateTime DataFinal
        {
            get
            {
                return _chartX.GetTimestampByIndex(Convert.ToInt32(X2Value)) ?? new DateTime();
            }
        }

        #region Controle de Posicionamento

        private DateTime dataOriginalInicial = new DateTime();
        private DateTime dataOriginalFinal = new DateTime();

        /// <summary>
        /// Propriedade responsável por guardar a data original de um objeto. Não é uma propriedade de uso interno desta classe.
        /// Seu intuito é receber a data original indicada de fora da classe e atualiza-la quando houver apenas operações de movimento ou resize
        /// do objeto. Dessa forma, podemos usar o posicionamento de objetos com records negativos por exemplo.
        /// </summary>
        public DateTime DataOriginalInicial
        {
            get
            {
                if (dataOriginalInicial.Year != 1)
                    return dataOriginalInicial;
                else if (DataInicial.Year != 1)
                    dataOriginalInicial = DataInicial;
                //Em ultimo caso, devo verificar se a data está no futuro
                else if (_x1Value > _chartX.RecordCount)
                {
                    //Obs: Se estiver no passado, n devo me preocupar, pois o usuario não consegue traçar no passado, apenas internamente
                    //é possível fazer isso

                    //Neste caso, se houver dados, pego a ultima data e adiciono a quantidade de records excedentes
                    if (_chartX.GraficoMain.Dados.Count > 0)
                    {
                        //Obtendo ultima data
                        DateTime ultimaData = _chartX.GraficoMain.Dados[_chartX.GraficoMain.Dados.Count - 1].TradeDate;

                        //Obtendo records excedentes, tirando do valor atual
                        int recordsExcedentes = Convert.ToInt32(_x1Value - _chartX.RecordCount);

                        //Simulando data futura, a quantidade de periodos excedentes
                        dataOriginalInicial = ultimaData.AddMinutes(recordsExcedentes * _chartX.GraficoMain.Periodicidade.Value); 
                    }
                } 

                return dataOriginalInicial;
            }
            set { dataOriginalInicial = value; }
        }

        /// <summary>
        /// Propriedade responsável por guardar a data original de um objeto. Não é uma propriedade de uso interno desta classe.
        /// Seu intuito é receber a data original indicada de fora da classe e atualiza-la quando houver apenas operações de movimento ou resize
        /// do objeto. Dessa forma, podemos usar o posicionamento de objetos com records negativos por exemplo.
        /// </summary>
        public DateTime DataOriginalFinal
        {
            get 
            {
                if (dataOriginalFinal.Year != 1)
                    return dataOriginalFinal;
                else if (DataFinal.Year != 1)
                    dataOriginalFinal = DataFinal;
                //Em ultimo caso, devo verificar se a data está no futuro
                else if (_x2Value > _chartX.RecordCount)
                {
                    //Obs: Se estiver no passado, n devo me preocupar, pois o usuario não consegue traçar no passado, apenas internamente
                    //é possível fazer isso

                    //Neste caso, se houver dados, pego a ultima data e adiciono a quantidade de records excedentes
                    if (_chartX.GraficoMain.Dados.Count > 0)
                    {
                        //Obtendo ultima data
                        DateTime ultimaData = _chartX.GraficoMain.Dados[_chartX.GraficoMain.Dados.Count - 1].TradeDate;

                        //Obtendo records excedentes, tirando do valor atual
                        int recordsExcedentes = Convert.ToInt32(_x2Value - _chartX.RecordCount);

                        //Simulando data futura, a quantidade de periodos excedentes
                        dataOriginalFinal = ultimaData.AddMinutes(recordsExcedentes * _chartX.GraficoMain.Periodicidade.Value);
                    }
                }

                return dataOriginalFinal;
            }
            set { dataOriginalFinal = value; }
        }

        #endregion Controle de Posicionamento
    }
}

