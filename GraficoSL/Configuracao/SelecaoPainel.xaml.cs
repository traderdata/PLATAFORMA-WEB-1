using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Traderdata.Client.Componente.GraficoSL.StockChart;

namespace Traderdata.Client.Componente.GraficoSL.Configuracao
{
    public partial class SelecaoPainel : ChildWindow
    {
        #region Campos e Construtores

        private List<ChartPanel> paineis = new List<ChartPanel>();
        private ChartPanel painelSelecionado = null;

        public ChartPanel PainelSelecionado
        {
            get { return painelSelecionado; }
            set { painelSelecionado = value; }
        }

        

        private delegate void OnPainelClickDelegate(string painelName);
        private event OnPainelClickDelegate OnPainelClick;

        /// <summary>
        /// Construtor padrão. Inicia criacao de paineis.
        /// </summary>
        /// <param name="paineisIndicador">Paineis que compoem os graficos.</param>
        public SelecaoPainel(List<ChartPanel> paineisIndicador, List<ChartPanel> paineisPreco)
        {
            InitializeComponent();

            OnPainelClick += new OnPainelClickDelegate(SelecaoPainel_OnPainelClick);

            this.paineis.Clear();
            foreach (ChartPanel obj in paineisIndicador)
            {
                this.paineis.Add(obj);
            }
            foreach (ChartPanel obj in paineisPreco)
            {
                this.paineis.Add(obj);
            }

            CriaPaineis(paineis);
        }

        #endregion Campos e Construtores

        #region Eventos

        /// <summary>
        /// Evento disparado ao clicar sobre um painel.
        /// </summary>
        /// <param name="painel"></param>
        private void SelecaoPainel_OnPainelClick(string painelName)
        {
            foreach (ChartPanel painel in paineis)
            {

                if ((painel.Index.ToString() == painelName.Split(';')[1]) && (painel._chartX.Name == painelName.Split(';')[0]))
                {
                    painelSelecionado = painel;
                    break;
                }
            }

            this.DialogResult = true;
        }

        #endregion Eventos

        #region CriarPaineis
        /// <summary>
        /// Cria painéis que refletem o gráfico.
        /// </summary>
        /// <param name="paineis"></param>
        private void CriaPaineis(List<ChartPanel> paineis)
        {
            //Percorrendo os paineis
            foreach (ChartPanel painel in paineis)
            {
                StackPanel novoPainel = new StackPanel();
                StackPanel titulo = new StackPanel();
                TextBlock lblTitulo = new TextBlock();
                Canvas canvas = new Canvas();
                TextBlock lblCanvas = new TextBlock();

                titulo.Background = WindowsVistaGradiente();
                titulo.Cursor = Cursors.Hand;
                titulo.Tag = painel._chartX.Name + ";" + painel.Index;
                titulo.MouseLeftButtonDown += (sender1, e1) => { if (OnPainelClick != null) OnPainelClick((string)titulo.Tag); };
                titulo.MouseEnter += (sender1, e1) => { lblCanvas.TextDecorations = TextDecorations.Underline; };

                lblTitulo.Text = painel.Title;
                lblTitulo.Tag = painel._chartX.Name + ";" + painel.Index;
                lblTitulo.Foreground = new SolidColorBrush(Colors.White);
                lblTitulo.Cursor = Cursors.Hand;
                lblTitulo.MouseLeftButtonDown += (sender1, e1) => { if (OnPainelClick != null) OnPainelClick((string)lblTitulo.Tag); };
                lblTitulo.MouseEnter += (sender1, e1) => { lblCanvas.TextDecorations = TextDecorations.Underline; };
                lblTitulo.MouseLeave += (sender1, e1) => { lblCanvas.TextDecorations = null; };


                canvas.Height = 50;
                canvas.Tag = painel._chartX.Name + ";" + painel.Index;
                canvas.Cursor = Cursors.Hand;
                canvas.Background = new SolidColorBrush(Colors.Black);
                canvas.MouseLeftButtonDown += (sender1, e1) => { if (OnPainelClick != null) OnPainelClick((string)canvas.Tag); };
                canvas.MouseEnter += (sender1, e1) => { lblCanvas.TextDecorations = TextDecorations.Underline; };
                canvas.MouseLeave += (sender1, e1) => { lblCanvas.TextDecorations = null; };


                lblCanvas.Margin = new Thickness(80, 17, 0, 0);
                lblCanvas.Cursor = Cursors.Hand;
                lblCanvas.Tag = painel._chartX.Name + ";" + painel.Index;
                lblCanvas.Text = "Clique aqui para selecionar este painel";
                lblCanvas.Foreground = new SolidColorBrush(Colors.White);
                lblCanvas.MouseLeftButtonDown += (sender1, e1) => { if (OnPainelClick != null) OnPainelClick((string)lblCanvas.Tag); };
                //lblCanvas.MouseEnter += (sender1, e1) => { lblCanvas.TextDecorations = TextDecorations.Underline; };
                //lblCanvas.MouseLeave += (sender1, e1) => { lblCanvas.TextDecorations = null; };

                novoPainel.Children.Add(titulo);
                novoPainel.Children.Add(canvas);

                titulo.Children.Add(lblTitulo);
                canvas.Children.Add(lblCanvas);

                stackPrincipal.Children.Add(novoPainel);
            }
        }
        #endregion CriarPaineis

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
            gs3.Offset = 0.394;

            GradientStop gs4 = new GradientStop();
            gs4.Color = Color.FromArgb(155, 21, 21, 22);
            gs4.Offset = 0.417;

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
    }
}

