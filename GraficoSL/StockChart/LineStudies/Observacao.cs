using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Line = System.Windows.Shapes.Line;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_Observacao()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.Observacao, typeof(Observacao), "Observação");
        }
    }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies
{
    public class Observacao : LineStudy, IContextAbleLineStudy
    {
        #region Campos e Construtores

        #region Painel Observacao

        private Point posicao;
        private Point posicaoInicial = new Point();
        private bool bordaPainelObservacaoClicado = false;
        private EnumGeral.InfoPanelPosicaoEnum posicaoInfoPanel = EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse;
        private Border bordabordaPainelObservacao = new Border();
        private StackPanel painelPai = new StackPanel();
        private StackPanel painelNome = new StackPanel();
        private StackPanel painelDescricao = new StackPanel();
        private TextBox txtNome = new TextBox();
        private TextBox txtDescricao = new TextBox();
        private string nome;
        private string descricao;
        private ContextLine _contextLine;

        public Point Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public string Titulo
        {
            get { return txtNome.Text; }
            set 
            {
                if (txtNome.Text == null)
                    txtNome.Text = "";
                else
                    txtNome.Text = value;
            }
        }

        public string Descricao
        {
            get { return txtDescricao.Text; }
            set 
            {
                if (txtDescricao.Text == null)
                    txtDescricao.Text = "";
                else
                    txtDescricao.Text = value;
            }
        }

        #endregion Painel Observacao

        private bool watchable;

        public Border bordaPainelObservacao
        {
            get { return bordabordaPainelObservacao; }
            set { bordabordaPainelObservacao = value; }
        }

        /// <summary>
        /// Construtor para Observação
        /// </summary>
        /// <param name="key">Nome único para observação</param>
        /// <param name="stroke">Cor da observação</param>
        /// <param name="chartPanel">Painel em que a observação será inserida</param>
        public Observacao(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.Observacao;

            //Criando painel da observacao dinamicamente
            CriaPainelObservacao();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Observacao()
        {
            //      _chartPanel.UnRegisterWatchableTrendLine(this);
        }

        #endregion Campos e Construtores

        #region Métodos

        #region SetaParametros()
        /// <summary>
        /// Seta parâmetros usando a string obtida no método <see cref="ObtemParametros"/>.
        /// </summary>
        /// <param name="parametros"></param>
        /// <param name="strConcatenacao"></param>
        public override void SetaParametros(string parametros, char strConcatenacao)
        {
            string[] parametrosAUX = parametros.Split(strConcatenacao);

            if (parametrosAUX.Length == 6)
            {
                double x1 = 0;
                double x2 = 0;
                double y1 = 0;
                double y2 = 0;

                Double.TryParse(parametrosAUX[0], out x1);
                Double.TryParse(parametrosAUX[1], out y1);

                Titulo = parametrosAUX[2];
                Descricao = parametrosAUX[3];

                Double.TryParse(parametrosAUX[4], out x2);
                Double.TryParse(parametrosAUX[5], out y2);

                DrawLineStudy(new Types.RectEx(x1, y1, x2, y2), LineStatus.RePaint);
            }
        }
        #endregion SetaParametros()

        #region SetaParametros(+1)
        /// <summary>
        /// Seta os parâmetros da observação.
        /// </summary>
        /// <param name="posicao"></param>
        /// <param name="titulo"></param>
        /// <param name="descricao"></param>
        /// <param name="largura"></param>
        /// <param name="altura"></param>
        public void SetaParametros(Point posicao, string titulo, string descricao, int largura, int altura)
        {
            try
            {
                DrawLineStudy(new Types.RectEx(posicao.X, posicao.Y, posicao.X + largura, posicao.Y + altura), LineStatus.RePaint);

                Descricao = descricao;
                Titulo = titulo;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion SetaParametros(+1)

        #region Obtem Parametros
        /// <summary>
        /// Obtém string contendo os parâmetros da observação. Usar com o método <see cref="SetaParametros"/>.
        /// </summary>
        /// <param name="strConcatenacao"></param>
        /// <returns></returns>
        public override string ObtemParametros(char strConcatenacao)
        {
            string strConcac = strConcatenacao.ToString();

            return (posicao.X + strConcac + posicao.Y + strConcac + Titulo + strConcac + Descricao + strConcac +
                    bordabordaPainelObservacao.Width + strConcac + bordabordaPainelObservacao.Height);
        }
        #endregion Obtem Parametros

        #region Criando PainelObservacao
        /// <summary>
        /// Criando Painel para observacao.
        /// </summary>
        private void CriaPainelObservacao()
        {
            bordaPainelObservacao.Tag = this;

            txtNome.Text = "Nome da observação";
            txtNome.Foreground = new SolidColorBrush(Colors.LightGray);
            txtNome.Margin = new Thickness(5);
            txtNome.FontSize = 9;
            txtNome.Background = new SolidColorBrush(Colors.Transparent);
            txtNome.BorderThickness = new Thickness(0);
            painelNome.Orientation = Orientation.Horizontal;
            painelNome.Children.Add(txtNome);

            txtDescricao.Text = "Descrição da observação";
            txtDescricao.Foreground = new SolidColorBrush(Colors.LightGray);
            txtDescricao.Margin = new Thickness(5);
            txtDescricao.FontSize = 9;
            txtDescricao.Background = new SolidColorBrush(Colors.Transparent);
            txtDescricao.BorderThickness = new Thickness(0);
            txtDescricao.AcceptsReturn = true;
            txtDescricao.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            txtDescricao.TextWrapping = TextWrapping.Wrap;
            painelDescricao.Orientation = Orientation.Horizontal;
            painelDescricao.Children.Add(txtDescricao);
            painelDescricao.Background = GradienteDescricao();

            //Criando painel pai e adicionando painéis de nome e descrição
            painelPai.Children.Add(painelNome);
            painelPai.Children.Add(painelDescricao);
            painelPai.Background = GradienteTitulo();

            //Criando borda para agrupar todos os paineis
            bordaPainelObservacao.CornerRadius = new CornerRadius(5);
            bordaPainelObservacao.Height = 150;
            bordaPainelObservacao.Width = 170;
            bordaPainelObservacao.Child = painelPai;

            bordaPainelObservacao.MouseLeftButtonDown += new MouseButtonEventHandler(observacao_MouseLeftButtonDown);
            bordaPainelObservacao.MouseLeftButtonUp += new MouseButtonEventHandler(observacao_MouseLeftButtonUp);

        }

        #endregion Criando PainelObservacao

        #region Gradientes
        /// <summary>
        /// Cria gradiente do Windows Vista.
        /// </summary>
        /// <returns></returns>
        private LinearGradientBrush GradienteTitulo()
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

        public LinearGradientBrush GradienteDescricao()
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

            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(gs1);
            gsc.Add(gs2);
            gsc.Add(gs3);

            LinearGradientBrush lgb = new LinearGradientBrush();

            lgb.GradientStops = gsc;
            lgb.StartPoint = new Point(0.5, 0);
            lgb.EndPoint = new Point(0.5, 1);

            return lgb;
        }

        #endregion Gradientes

        #region  DrawLineStudy

        /// <summary>
        /// Pinta linha de estudo no gráfico.
        /// </summary>
        /// <param name="rect">Dimensão do objeto</param>
        /// <param name="lineStatus">Status de ação para linha</param>
        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
             if (!C.Children.Contains(bordaPainelObservacao) && lineStatus != LineStatus.StartPaint)
                DrawLineStudy(rect, LineStatus.StartPaint);
             if ((lineStatus == LineStatus.StartPaint) && (!C.Children.Contains(bordaPainelObservacao)))
             {
                 C.Children.Add(bordaPainelObservacao);
                 C.MouseMove += new MouseEventHandler(_chartPanel_MouseMove);
                 Canvas.SetZIndex(bordaPainelObservacao, ZIndexConstants.LineStudies1);

                 _internalObjectCreated = true;
             }

             posicao.X = _x1 = rect.Left;
             _x2 = rect.Right;

             posicao.Y = _y1 = rect.Top;
             _y2 = rect.Bottom;

             bordabordaPainelObservacao.Width = Math.Abs(rect.Right - rect.Left);
             bordabordaPainelObservacao.Height = Math.Abs(rect.Bottom - rect.Top);

            if (bordabordaPainelObservacao.Width > 5)
                txtDescricao.Width = bordabordaPainelObservacao.Width - 5;

            if (bordabordaPainelObservacao.Height > 5)
                txtDescricao.Height = bordabordaPainelObservacao.Height - 5;

             Canvas.SetLeft(bordabordaPainelObservacao, rect.Left);
             Canvas.SetTop(bordabordaPainelObservacao, rect.Top);
        }

        #endregion  DrawLineStudy

        #region RemoveLineStudy
        /// <summary>
        /// Remove linha de estudo direteo do painel.
        /// </summary>
        internal override void RemoveLineStudy()
        {
            C.Children.Remove(bordabordaPainelObservacao);
        }
        #endregion RemoveLineStudy

        #region SetCoordenadas()
        /// <summary>
        /// Seta a posicao da observacao no grafico.
        /// </summary>
        /// <param name="rect"></param>
        public override void SetCoordenadas(Types.RectEx rect)
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

            this.bordabordaPainelObservacao.Width = rect.Right + rect.Left;
            this.bordabordaPainelObservacao.Height = rect.Top + rect.Bottom;

            Canvas.SetLeft(bordabordaPainelObservacao, rect.Left);
            Canvas.SetTop(bordabordaPainelObservacao, rect.Top);
        }
        #endregion SetCoordenadas()

        #region SetCoordenadas()
        /// <summary>
        /// Seta a posicao da observacao no grafico.
        /// </summary>
        public override void  SetCoordenadas(double x1Value, double y1Value, double x2Value, double y2Value)
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

            this.bordabordaPainelObservacao.Width = x2Value + x1Value;
            this.bordabordaPainelObservacao.Height = y1Value + y2Value;

            Canvas.SetLeft(bordabordaPainelObservacao, x1Value);
            Canvas.SetTop(bordabordaPainelObservacao, y1Value);
        }
        #endregion SetCoordenadas()

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            //Obtendo posicao do mouse DENTRO do painel, de forma que ao mover, ele não fique preso apenas ao ponteiro do mouse
            Point posicao = new Point(Canvas.GetLeft(bordabordaPainelObservacao), Canvas.GetTop(bordabordaPainelObservacao));

            return new List<SelectionDotInfo>
               {
                 //new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = new Point(posicao.X, posicao.Y)},
                 //new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = new Point(posicao.X + bordabordaPainelObservacao.Width, posicao.Y)},
                 //new SelectionDotInfo {Corner = Types.Corner.TopCenter, Position = new Point(posicao.X + (bordabordaPainelObservacao.Width /2), posicao.Y)},

                 //new SelectionDotInfo {Corner = Types.Corner.MiddleLeft, Position = new Point(posicao.X, posicao.Y + (bordabordaPainelObservacao.Height /2))},
                 //new SelectionDotInfo {Corner = Types.Corner.MiddleRight, Position = new Point(posicao.X + bordabordaPainelObservacao.Width, posicao.Y + (bordabordaPainelObservacao.Height /2))},

                 //new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = new Point(posicao.X, posicao.Y + bordabordaPainelObservacao.Height)},
                 //new SelectionDotInfo {Corner = Types.Corner.BottomCenter, Position = new Point(posicao.X + (bordabordaPainelObservacao.Width /2), posicao.Y + bordabordaPainelObservacao.Height)},
                 //new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = new Point(posicao.X + bordabordaPainelObservacao.Width, posicao.Y + bordabordaPainelObservacao.Height)},

                 new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = new Point(_x1, _y1)},
                 new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = new Point(_x2, _y1)},
                 new SelectionDotInfo {Corner = Types.Corner.TopCenter, Position = new Point(_x1 + (bordabordaPainelObservacao.Width /2), _y1)},

                 new SelectionDotInfo {Corner = Types.Corner.MiddleLeft, Position = new Point(_x1, _y1 + (bordabordaPainelObservacao.Height /2))},
                 new SelectionDotInfo {Corner = Types.Corner.MiddleRight, Position = new Point(_x2, _y1 + (bordabordaPainelObservacao.Height /2))},

                 new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = new Point(_x1, _y2)},
                 new SelectionDotInfo {Corner = Types.Corner.BottomCenter, Position = new Point(_x1 + (bordabordaPainelObservacao.Width /2), _y2)},
                 new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = new Point(_x2, _y2)},
               };
        }

        internal override void SetCursor()
        {
            if (_selectionVisible)
            {
                bordabordaPainelObservacao.Cursor = Cursors.Hand;
                return;
            }
            if (_selectionVisible || bordabordaPainelObservacao.Cursor == Cursors.Arrow)
                return;

            bordabordaPainelObservacao.Cursor = Cursors.Arrow;
        }

        internal override void SetStrokeThickness()
        {
           
        }

        internal override void SetStroke()
        {
            
        }

        internal override void SetStrokeType()
        {
           
        }

        internal override void SetOpacity()
        {
            bordabordaPainelObservacao.Opacity = Opacity;
        }

        #endregion Métodos

        #region Implementação da interface IContextAbleLineStudy

        /// <summary>
        /// Elemento ao qual está vinculado a linha de contexto
        /// </summary>
        public UIElement Element
        {
            get { return bordabordaPainelObservacao; }
        }

        /// <summary>
        /// Segmento de contexto onde a linha deve ser mostrada
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(posicaoInicial.X, posicaoInicial.Y, posicaoInicial.X + bordabordaPainelObservacao.Width, posicaoInicial.Y + bordabordaPainelObservacao.Height).Normalize(); }
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

        #region Movimentação do Painel de Observacao

        /// <summary>
        /// Movimentação do mouse no observacao.
        /// Verifico se o painel da observacao foi selecionado, se estiver devo movê-lo.
        /// </summary>
        private void _chartPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (bordaPainelObservacaoClicado)
            {
                posicao = new Point(_chartPanel.PosicaoAtual.X - posicaoInicial.X, _chartPanel.PosicaoAtual.Y - posicaoInicial.Y);

                Canvas.SetLeft(bordaPainelObservacao, posicao.X);
                Canvas.SetTop(bordaPainelObservacao, posicao.Y);
            }
        }


        /// <summary>
        /// Liberação do clique no painel da observacao.
        /// Finaliza movimentacao do painel da observacao.
        /// </summary>
        private void observacao_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bordaPainelObservacao.ReleaseMouseCapture();
            bordaPainelObservacaoClicado = false;
            _chartX.InfoPanelPosicao = posicaoInfoPanel;

            //Obtendo posicao do mouse DENTRO do painel, de forma que ao mover, ele não fique preso apenas ao ponteiro do mouse
            posicaoInicial = new Point(e.GetPosition(bordaPainelObservacao).X, e.GetPosition(bordaPainelObservacao).Y);
        }


        /// <summary>
        /// Clique no painel da observacao.
        /// Inicia movimentacao do painel da observacao.
        /// </summary>
        private void observacao_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bordaPainelObservacao.CaptureMouse();
            bordaPainelObservacaoClicado = true;
            posicaoInfoPanel = _chartX.InfoPanelPosicao;
            _chartX.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.Escondido;

            //Obtendo posicao do mouse DENTRO do painel, de forma que ao mover, ele não fique preso apenas ao ponteiro do mouse
            posicaoInicial = new Point(e.GetPosition(bordaPainelObservacao).X, e.GetPosition(bordaPainelObservacao).Y);
        }

        #endregion Movimentação do Painel da Observacao
    }
}
