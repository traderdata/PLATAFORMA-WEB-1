using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Line=System.Windows.Shapes.Line;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_Regua()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.Regua, typeof(Regua), "Régua");
        }
    }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies
{
    public class Regua : LineStudy, IContextAbleLineStudy
    {
        #region Campos e Construtores

        #region Painel Regua

        private Point posicaoInicial = new Point();
        private bool painelReguaClicado = false;
        private EnumGeral.InfoPanelPosicaoEnum posicaoInfoPanel = EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse;
        private Border bordaPainelRegua = new Border();
        private StackPanel painelPai = new StackPanel();
        private StackPanel painelTitulo = new StackPanel();
        private StackPanel painelDataInicial = new StackPanel();
        private StackPanel painelDataFinal = new StackPanel();
        private StackPanel painelValorInicial = new StackPanel();
        private StackPanel painelValorFinal = new StackPanel();
        private StackPanel painelVariacaoPercent = new StackPanel();
        private StackPanel painelVariacaoValor = new StackPanel();
        private TextBlock txtDataInicial = new TextBlock();
        private TextBlock txtDataFinal = new TextBlock();
        private TextBlock txtValorInicial = new TextBlock();
        private TextBlock txtValorFinal = new TextBlock();
        private TextBlock txtVariacaoPercent = new TextBlock();
        private TextBlock txtVariacaoValor = new TextBlock();
        private TextBlock txtTitulo = new TextBlock();

        public DateTime DataInicialRegua
        {
            set { txtDataInicial.Text = "Data Inicial: " + value.ToString(); }
        }

        public DateTime DataFinalRegua
        {
            set { txtDataFinal.Text = "Data Final: " + value.ToString();}
        }

        public double ValorInicialRegua
        {
            set { txtValorInicial.Text = "Valor Inicial: " + value.ToString("N2"); }
        }

        public double ValorFinalRegua
        {
            set { txtValorFinal.Text = "Valor Final: " + value.ToString("N2"); }
        }

        public double VariacaoPercentRegua
        {
            set { txtVariacaoPercent.Text = "Variação (%): " + value.ToString("N2"); }
        }

        public double VariacaoValorRegua
        {
            set { txtVariacaoValor.Text = "Variação (R$): " + value.ToString("N2"); }
        }

        #endregion Painel Regua

        private Line line;
        private ContextLine contextLine;
        private bool watchable;
        private Border painelRegua;

        public Border PainelRegua
        {
            get { return painelRegua; }
            set { painelRegua = value; }
        }
        
        /// <summary>
        /// Construtor para régua
        /// </summary>
        /// <param name="key">Nome único para reguá</param>
        /// <param name="stroke">Cor da régua</param>
        /// <param name="chartPanel">Painel em que a régua será inserida</param>
        public Regua(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.Regua;

            //Criando painel da regua dinamicamente
            CriaPainelRegua();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Regua()
        {
            //      _chartPanel.UnRegisterWatchableTrendLine(this);
        }

        #endregion Campos e Construtores

        #region Métodos

        #region Criando Painelregua

        /// <summary>
        /// Criando Painel para régua
        /// </summary>
        private void CriaPainelRegua()
        {
            #region Criando Paineis e textos

            
            txtDataInicial.Text = "Data Inicial: 12/01/2009 12:00:00";
            txtDataInicial.Foreground = new SolidColorBrush(Colors.LightGray);
            txtDataInicial.Margin = new Thickness(5, 5, 0, 0);
            txtDataInicial.FontSize = 9;
            painelDataInicial.Orientation = Orientation.Horizontal;
            painelDataInicial.Children.Add(txtDataInicial);

            
            txtDataFinal.Text = "Data Final: 12/01/2009 12:00:00";
            txtDataFinal.Foreground = new SolidColorBrush(Colors.LightGray);
            txtDataFinal.Margin = new Thickness(5, 5, 0, 0);
            txtDataFinal.FontSize = 9;
            painelDataFinal.Orientation = Orientation.Horizontal;
            painelDataFinal.Children.Add(txtDataFinal);

            
            txtValorInicial.Text = "Valor Inicial: 1,000";
            txtValorInicial.Foreground = new SolidColorBrush(Colors.LightGray);
            txtValorInicial.Margin = new Thickness(5, 5, 0, 0);
            txtValorInicial.FontSize = 9;
            painelValorInicial.Orientation = Orientation.Horizontal;
            painelValorInicial.Children.Add(txtValorInicial);
            
            txtValorFinal.Text = "Valor Final: 1,000";
            txtValorFinal.Foreground = new SolidColorBrush(Colors.LightGray);
            txtValorFinal.Margin = new Thickness(5, 5, 0, 0);
            txtValorFinal.FontSize = 9;
            painelValorFinal.Orientation = Orientation.Horizontal;
            painelValorFinal.Children.Add(txtValorFinal);
            
            txtVariacaoPercent.Text = "Variação (%): 100,00";
            txtVariacaoPercent.Foreground = new SolidColorBrush(Colors.LightGray);
            txtVariacaoPercent.Margin = new Thickness(5, 5, 0, 0);
            txtVariacaoPercent.FontSize = 9;
            painelVariacaoPercent.Orientation = Orientation.Horizontal;
            painelVariacaoPercent.Children.Add(txtVariacaoPercent);

            txtVariacaoValor.Text = "Variação (R$): 100,00";
            txtVariacaoValor.Foreground = new SolidColorBrush(Colors.LightGray);
            txtVariacaoValor.Margin = new Thickness(5, 5, 0, 0);
            txtVariacaoValor.FontSize = 9;
            painelVariacaoValor.Orientation = Orientation.Horizontal;
            painelVariacaoValor.Children.Add(txtVariacaoValor);

            #endregion Criando Paineis e textos

            //Criando painel pai
            
            txtTitulo.Text = "Régua";
            txtTitulo.Margin = new Thickness(5);
            txtTitulo.Foreground = new SolidColorBrush(Colors.Gray);
            painelTitulo.Orientation = Orientation.Horizontal;
            painelTitulo.Children.Add(txtTitulo);

            //Adicionando paineis com textos no stackpanel Texto
            painelPai.Children.Add(painelTitulo);
            painelPai.Children.Add(painelDataInicial);
            painelPai.Children.Add(painelDataFinal);
            painelPai.Children.Add(painelValorInicial);
            painelPai.Children.Add(painelValorFinal);
            painelPai.Children.Add(painelVariacaoPercent);
            painelPai.Children.Add(painelVariacaoValor);

            //Criando borda para agrupar todos os paineis
            bordaPainelRegua.CornerRadius = new CornerRadius(5);
            bordaPainelRegua.Background = WindowsVistaGradiente();
            bordaPainelRegua.Height = 150;
            bordaPainelRegua.Width = 170;
            bordaPainelRegua.Child = painelPai;

            painelRegua = bordaPainelRegua;
            painelRegua.MouseLeftButtonDown += new MouseButtonEventHandler(painelRegua_MouseLeftButtonDown);
            painelRegua.MouseLeftButtonUp += new MouseButtonEventHandler(painelRegua_MouseLeftButtonUp);
            
        }


        #endregion Criando Painelregua

        #region WindowsVistaGradiente()
        /// <summary>
        /// Cria gradiente do Windows Vista.
        /// </summary>
        /// <returns></returns>
        private LinearGradientBrush WindowsVistaGradiente()
        {
            //Criando BackGround para o stackPanel
            GradientStop gs1 = new GradientStop();
            gs1.Color = Color.FromArgb(155, 76, 76, 76);

            GradientStop gs2 = new GradientStop();
            gs2.Color = Color.FromArgb(155, 51, 53, 56);
            gs2.Offset = 1;

            GradientStop gs3 = new GradientStop();
            gs3.Color = Color.FromArgb(155, 60, 61, 63);
            gs3.Offset = 0.200;

            GradientStop gs4 = new GradientStop();
            gs4.Color = Color.FromArgb(155, 21, 21, 22);
            gs4.Offset = 0.150;

            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(gs1);
            gsc.Add(gs2);
            gsc.Add(gs3);
            gsc.Add(gs4);

            LinearGradientBrush lgb = new LinearGradientBrush();

            lgb.GradientStops = gsc;
            lgb.StartPoint = new Point(0.5, 0);
            lgb.EndPoint = new Point(0.5, 1);

            return lgb;
        }

        #endregion WindowsVistaGradiente()

        #region  DrawLineStudy 
        /// <summary>
        /// Pinta linha de estudo no gráfico.
        /// </summary>
        /// <param name="rect">Dimensão do objeto</param>
        /// <param name="lineStatus">Status de ação para linha</param>
        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (line == null && lineStatus != LineStatus.StartPaint)
                DrawLineStudy(rect, LineStatus.StartPaint);
            if (lineStatus == LineStatus.StartPaint)
            {
                line = new Line { Stroke = Stroke, StrokeThickness = StrokeThickness, Tag = this };
                Types.SetLinePattern(line, StrokeType);
                
                C.Children.Add(line);
                C.Children.Add(painelRegua);
                C.MouseMove += new MouseEventHandler(_chartPanel_MouseMove);

                Canvas.SetZIndex(line, ZIndexConstants.LineStudies1);
                Canvas.SetZIndex(painelRegua, ZIndexConstants.InfoPanel);

                _internalObjectCreated = true;

                return;
            }

            line.X1 = rect.Left;
            line.Y1 = rect.Top;

            line.X2 = rect.Right;
            line.Y2 = rect.Bottom;
        }
        #endregion  DrawLineStudy

        #region InsereEstudoDiretamente

        public override void InsereEstudoDiretamente(Types.RectEx rect)
        {
            try
            {
                line = new Line { Stroke = Stroke, StrokeThickness = StrokeThickness, Tag = this };
                Types.SetLinePattern(line, StrokeType);

                C.Children.Add(line);
                C.Children.Add(painelRegua);
                C.MouseMove += new MouseEventHandler(_chartPanel_MouseMove);

                Canvas.SetZIndex(line, ZIndexConstants.LineStudies1);
                Canvas.SetZIndex(painelRegua, ZIndexConstants.InfoPanel);

                if (contextLine == null)
                    contextLine = new ContextLine(this);

                _internalObjectCreated = true;

                line.X1 = rect.Left;
                line.Y1 = rect.Top;

                line.X2 = rect.Right;
                line.Y2 = rect.Bottom;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion InsereEstudoDiretamente

        #region GetSelectionPoints
        /// <summary>
        /// Retorna os pontos de seleção.
        /// </summary>
        /// <returns></returns>
        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = new Point(line.X1, line.Y1)},
                 new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = new Point(line.X2, line.Y2)},
               };
        }
        #endregion GetSelectionPoints

        #region SetCursor
        /// <summary>
        /// Muda o tipo de cursor(Arrow ou Hand).
        /// </summary>
        internal override void SetCursor()
        {
            if (_selectionVisible)
            {
                line.Cursor = Cursors.Hand;
                return;
            }
            if (_selectionVisible || line.Cursor == Cursors.Arrow)
                return;

            line.Cursor = Cursors.Arrow;
        }
        #endregion SetCursor

        #region SetStrokeThickness
        /// <summary>
        /// Seta espessura da linha
        /// </summary>
        internal override void SetStrokeThickness()
        {
            if (line != null)
                line.StrokeThickness = StrokeThickness;
        }
        #endregion SetStrokeThickness

        #region SetStroke
        /// <summary>
        /// Seta a cor da linha de estudo.
        /// </summary>
        internal override void SetStroke()
        {
            if (line != null)
                line.Stroke = Stroke;
        }
        #endregion SetStroke

        #region SetStrokeType
        /// <summary>
        /// Seta o tipo da linha
        /// </summary>
        internal override void SetStrokeType()
        {
            if (line != null)
                Types.SetLinePattern(line, StrokeType);
        }
        #endregion SetStrokeType

        #region RemoveLineStudy
        /// <summary>
        /// Remove linha de estudo direteo do painel.
        /// </summary>
        internal override void RemoveLineStudy()
        {
            C.Children.Remove(line);
            C.Children.Remove(painelRegua);
        }
        #endregion RemoveLineStudy

        #region SetOpacity
        /// <summary>
        /// Seta opacidade da linha.
        /// </summary>
        internal override void SetOpacity()
        {
            line.Opacity = Opacity;
        }
        #endregion SetOpacity

        #region ObtemParametros
        /// <summary>
        /// Obtém parâmetros do Objeto. No caso, a posicao do painel da regua.
        /// </summary>
        /// <param name="strConcatenacao">String que será usada na concatenação.</param>
        /// <returns></returns>
        public override string ObtemParametros(char strConcatenacao)
        {
            Point posicaoAtual = new Point(Canvas.GetLeft(painelRegua), Canvas.GetTop(painelRegua));
            return (posicaoAtual.X.ToString() + strConcatenacao.ToString() + posicaoAtual.Y.ToString());
        }
        #endregion ObtemParametros

        #region SetaParametros
        /// <summary>
        /// Seta os parâmetros para este objeto no formato obtido pelo método ObtemParametros.
        /// No caso, seta a posicao do painel regua.
        /// </summary>
        /// <param name="parametros">String contendo parametros concatenados.</param>
        /// <param name="strConcatenacao">Char usado para realizar o split dos parametros.</param>
        public override void SetaParametros(string parametros, char strConcatenacao)
        {
            string[] param = parametros.Split(strConcatenacao);

            posicaoInicial.X = Convert.ToDouble(param[0]);
            posicaoInicial.Y = Convert.ToDouble(param[1]);

            Canvas.SetLeft(painelRegua, Convert.ToDouble(param[0]));
            Canvas.SetTop(painelRegua, Convert.ToDouble(param[1]));
        }
        #endregion SetaParametros

        #endregion Métodos

        #region Classe BarIntersection
        /// <summary>
        /// Representa a informação sobre interseção da linha de tendência e uam candle do chart
        /// </summary>
        public class BarIntersection
        {
            /// <summary>
            /// Obtém o numero de records do eixo X da interseção.Indice baseado em 0.
            /// </summary>
            public int Record { get; internal set; }

            /// <summary>
            /// Obtém o valor do preço Y da interseção
            /// </summary>
            public double Price { get; internal set; }
        }

        #region BarsIntersection

        /// <summary>
        /// Obtém uma coleção de interseções entre a linha de tendência atual
        /// de todas as barras que interceptam essa linha de tendência.
        /// </summary>
        public IEnumerable<BarIntersection> BarsIntersection
        {
            get
            {
                if (line == null || line.X2 == line.X1)
                    yield break;

                Series open = _chartPanel.SeriesCollection.FirstOrDefault(s => s.OHLCType == EnumGeral.TipoSerieOHLC.Abertura);
                Series close = _chartPanel.SeriesCollection.FirstOrDefault(s => s.OHLCType == EnumGeral.TipoSerieOHLC.Ultimo);

                if (open == null || close == null)
                    yield break;

                var chart = _chartX;
                int startIndex = chart.GetReverseX(Math.Min(line.X1, line.X2)) - 1 + chart.indexInicial;
                int endIndex = chart.GetReverseX(Math.Max(line.X1, line.X2)) + chart.indexInicial;
                startIndex = Math.Max(0, startIndex);
                endIndex = Math.Min(endIndex, chart.RecordCount);

                double slope = (line.Y1 - line.Y2) / (line.X1 - line.X2);

                double k = slope * (line.X1 - line.X2);

                for (int i = startIndex; i < endIndex; i++)
                {
                    double y = line.Y1 - slope * (line.X1 - chart.GetXPixel(i - chart.indexInicial));
                    double priceValue = _chartPanel.GetReverseY(y);

                    double? openValue = open[i].Value;
                    if (!openValue.HasValue)
                        continue;
                    double? closeValue = close[i].Value;
                    if (!closeValue.HasValue)
                        continue;

                    double min = Math.Min(openValue.Value, closeValue.Value);
                    double max = Math.Max(openValue.Value, closeValue.Value);

                    if (priceValue >= min && priceValue <= max)
                        yield return new BarIntersection
                        {
                            Record = i,
                            Price = priceValue
                        };
                }
            }
        }
        #endregion BarsIntersection

        #endregion Classe BarIntersection

        #region Implementação da interface IContextAbleLineStudy

        /// <summary>
        /// Elemento ao qual está vinculado a linha de contexto
        /// </summary>
        public UIElement Element
        {
            get { return line; }
        }

        /// <summary>
        /// Segmento de contexto onde a linha deve ser mostrada
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(line.X1, line.Y1, line.X2, line.Y2).Inflate(-20); }
        }

        /// <summary>
        /// Pai, ao qual <see cref="IContextAbleLineStudy.Element"/> pertence.
        /// </summary>
        public Canvas Parent
        {
            get { return C; }
        }

        /// <summary>
        /// Verifica se <see cref="IContextAbleLineStudy.Element"/> está selecionado.
        /// </summary>
        public bool IsSelected
        {
            get { return _selected; }
        }

        /// <summary>
        /// Z Index de <see cref="IContextAbleLineStudy.Element"/>
        /// </summary>
        public int ZIndex
        {
            get { return ZIndexConstants.LineStudies1; }
        }

        /// <summary>
        /// Obtém o gráfico associado a esse <see cref="IContextAbleLineStudy.Element"/>.
        /// </summary>
        public StockChartX Chart
        {
            get { return _chartX; }
        }

        /// <summary>
        /// Obtém uma cópia referenciada desta linha de estudo.
        /// </summary>
        public LineStudy LineStudy
        {
            get { return this; }
        }

        #endregion Implementação da interface IContextAbleLineStudy

        #region Movimentação do Painel da Régua

        /// <summary>
        /// Movimentação do mouse no painel.
        /// Verifico se o painel da regua foi selecionado, se estiver devo movê-lo.
        /// </summary>
        private void _chartPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (painelReguaClicado)
            {
                Point posicao = new Point(_chartPanel.PosicaoAtual.X - posicaoInicial.X, _chartPanel.PosicaoAtual.Y - posicaoInicial.Y);

                Canvas.SetLeft(painelRegua, posicao.X);
                Canvas.SetTop(painelRegua, posicao.Y);
            }
        }


        /// <summary>
        /// Liberação do clique no painel da regua.
        /// Finaliza movimentacao do painel da regua.
        /// </summary>
        private void painelRegua_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            painelRegua.ReleaseMouseCapture();
            painelReguaClicado = false;
            _chartX.InfoPanelPosicao = posicaoInfoPanel;
        }


        /// <summary>
        /// Clique no painel da regua.
        /// Inicia movimentacao do painel da regua.
        /// </summary>
        private void painelRegua_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            painelRegua.CaptureMouse();
            painelReguaClicado = true;
            posicaoInfoPanel = _chartX.InfoPanelPosicao;
            _chartX.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.Escondido;

            //Obtendo posicao do mouse DENTRO do painel, de forma que ao mover, ele não fique preso apenas ao ponteiro do mouse
            posicaoInicial = new Point(e.GetPosition(painelRegua).X, e.GetPosition(painelRegua).Y);
        }

        #endregion Movimentação do Painel da Régua
    }
}
