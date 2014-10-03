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
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.Util;
using Traderdata.Client.Componente.GraficoSL.DTO;

namespace Traderdata.Client.Componente.GraficoSL.Configuracao
{
    public partial class ConfiguraGrafico : ChildWindow
    {
        #region Campos e Construtores

        //Variavel de controle principal
        private ConfiguracaoGraficoDTO configuracaoGrafico;

        //Dialogs
        private ColorDialog corDialog;

        public ConfiguraGrafico(ConfiguracaoGraficoDTO configuracaoGrafico)
        {
            InitializeComponent();

            this.configuracaoGrafico = configuracaoGrafico;
            
            //Mostrando cores nos retangulos
            rectBordaCAlta.Fill = new SolidColorBrush((Color)configuracaoGrafico.CorBordaCandleAlta);
            rectBordaCBaixa.Fill = new SolidColorBrush((Color)configuracaoGrafico.CorBordaCandleBaixa);
            rectCAlta.Fill = new SolidColorBrush((Color)configuracaoGrafico.CorCandleAlta);
            rectCBaixa.Fill = new SolidColorBrush((Color)configuracaoGrafico.CorCandleBaixa);
            rectCorFundo.Fill = configuracaoGrafico.CorFundo;

            #region Tipo Escala

            //Mudando a cor dos combos
            cmbEstiloBarra.Foreground = Brushes.Black;
            cmbEstiloPreco.Foreground = Brushes.Black;
            cmbTipoEscala.Foreground = Brushes.Black;
            

            //Adicionando tipos de escala       
            cmbTipoEscala.Items.Add("Normal");
            cmbTipoEscala.Items.Add("Semilog");

            //Selecionando tipo escala
            if (configuracaoGrafico.TipoEscala == EnumGeral.TipoEscala.Linear)
                cmbTipoEscala.SelectedIndex = 0;
            else
                cmbTipoEscala.SelectedIndex = 1;

            #endregion Tipo Escala

            #region Estilo Preco

            //Adicionando estilos de preço
            cmbEstiloPreco.Items.Add("Padrão");
            cmbEstiloPreco.Items.Add("Candle Volume");
            cmbEstiloPreco.Items.Add("Equi Volume");
            cmbEstiloPreco.Items.Add("Equi Volume Shadow");
            cmbEstiloPreco.Items.Add("Heikin Ashi");
            cmbEstiloPreco.Items.Add("Kagi");
            cmbEstiloPreco.Items.Add("Point and Figure");
            cmbEstiloPreco.Items.Add("Renko");
            cmbEstiloPreco.Items.Add("Three Line Break");

            switch (configuracaoGrafico.EstiloPreco)
            {
                case EnumGeral.EstiloPrecoEnum.Padrao:
                    cmbEstiloPreco.SelectedIndex = 0;
                    break;

                case EnumGeral.EstiloPrecoEnum.CandleVolume:
                    cmbEstiloPreco.SelectedIndex = 1;
                    break;

                case EnumGeral.EstiloPrecoEnum.EquiVolume:
                    cmbEstiloPreco.SelectedIndex = 2;
                    break;

                case EnumGeral.EstiloPrecoEnum.EquiVolumeShadow:
                    cmbEstiloPreco.SelectedIndex = 3;
                    break;

                case EnumGeral.EstiloPrecoEnum.HeikinAshi:
                    cmbEstiloPreco.SelectedIndex = 4;
                    break;

                case EnumGeral.EstiloPrecoEnum.Kagi:
                    cmbEstiloPreco.SelectedIndex = 5;
                    break;

                case EnumGeral.EstiloPrecoEnum.PontoEFigura:
                    cmbEstiloPreco.SelectedIndex = 6;
                    break;

                case EnumGeral.EstiloPrecoEnum.Renko:
                    cmbEstiloPreco.SelectedIndex = 7;
                    break;

                case EnumGeral.EstiloPrecoEnum.ThreeLineBreak:
                    cmbEstiloPreco.SelectedIndex = 8;
                    break;
            }

            #endregion Estilo Preco

            #region Estilo Barra

            cmbEstiloBarra.Items.Add("Linha");
            cmbEstiloBarra.Items.Add("Barra");
            cmbEstiloBarra.Items.Add("Candle");

            switch (configuracaoGrafico.EstiloBarra)
            {
                case EnumGeral.TipoSeriesEnum.Linha:
                    cmbEstiloBarra.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoSeriesEnum.Barra:
                    cmbEstiloBarra.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoSeriesEnum.Candle:
                    cmbEstiloBarra.SelectedIndex = 2;
                    break;
            }

            #endregion Estilo Barra

            //Selecionando alinhamento da escala
            if (configuracaoGrafico.PosicaoEscala == EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda)
                chkEscalaDireita.IsChecked = false;
            else
                chkEscalaDireita.IsChecked = true;

            //Selecionando precisao da escala
            numPrecisaoEscala.Value = configuracaoGrafico.PrecisaoEscala;

            //Setando grades
            chkHorizontal.IsChecked = configuracaoGrafico.GradeHorizontal;// gradeHorizontal;
            chkVertical.IsChecked = configuracaoGrafico.GradeVertical;

            //Info panel
            chkPainelInfoFixo.IsChecked = configuracaoGrafico.PainelInfo;

            //Espaco a direita do grafico
            numEspacoDireitaGrafico.Value = configuracaoGrafico.EspacoADireitaGrafico;

            chkCorVolume.IsChecked = configuracaoGrafico.UsarCoresAltaBaixaVolume;
        }

        #endregion Campos e Construtores


        #region Propriedades

        public ConfiguracaoGraficoDTO ConfiguracaoGrafico 
        {
            get { return configuracaoGrafico; }
        }

        #endregion

        #region Eventos

        /// <summary>
        /// Clique na cor de fundo.
        /// </summary>
        private void rectFundo_Click(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush corFundoAux = (SolidColorBrush)configuracaoGrafico.CorFundo;

            corDialog = new ColorDialog(corFundoAux.Color);
            corDialog.Closing += (sender1, e1) =>
            { configuracaoGrafico.CorFundo = new SolidColorBrush(corDialog.Cor); rectCorFundo.Fill = configuracaoGrafico.CorFundo; };

            corDialog.Show();
        }


        /// <summary>
        /// Clique na cor borda de candle alta
        /// </summary>
        private void rectBordaCandleAlta_Click(object sender, MouseButtonEventArgs e)
        {
            corDialog = new ColorDialog((Color)configuracaoGrafico.CorBordaCandleAlta);
            corDialog.Closing += (sender1, e1) =>
            { configuracaoGrafico.CorBordaCandleAlta = corDialog.Cor; rectBordaCAlta.Fill = new SolidColorBrush((Color)configuracaoGrafico.CorBordaCandleAlta); };

            corDialog.Show();
        }


        /// <summary>
        /// Clique na cor borda de candle baixa
        /// </summary>
        private void rectBordaCandleBaixa_Click(object sender, MouseButtonEventArgs e)
        {
            corDialog = new ColorDialog((Color)configuracaoGrafico.CorBordaCandleBaixa);
            corDialog.Closing += (sender1, e1) =>
            { configuracaoGrafico.CorBordaCandleBaixa = corDialog.Cor; rectBordaCBaixa.Fill = new SolidColorBrush((Color)configuracaoGrafico.CorBordaCandleBaixa); };

            corDialog.Show();
        }


        /// <summary>
        /// Clique na cor de candle alta
        /// </summary>
        private void rectCAlta_Click(object sender, MouseButtonEventArgs e)
        {
            corDialog = new ColorDialog(configuracaoGrafico.CorCandleAlta);
            corDialog.Closing += (sender1, e1) =>
            { configuracaoGrafico.CorCandleAlta = corDialog.Cor; rectCAlta.Fill = new SolidColorBrush(configuracaoGrafico.CorCandleAlta); };

            corDialog.Show();
        }

        /// <summary>
        /// Clique na cor de candle baixa
        /// </summary>
        private void rectCBaixa_Click(object sender, MouseButtonEventArgs e)
        {
            corDialog = new ColorDialog(configuracaoGrafico.CorCandleBaixa);
            corDialog.Closing += (sender1, e1) =>
            { configuracaoGrafico.CorCandleBaixa = corDialog.Cor; rectCBaixa.Fill = new SolidColorBrush(configuracaoGrafico.CorCandleBaixa); };

            corDialog.Show();
        }


        /// <summary>
        /// Mudanca de item na combo de estilo de preco.
        /// </summary>
        private void cmbEstiloPreco_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEstiloPreco.SelectedIndex == 0)
                cmbEstiloBarra.IsEnabled = true;
            else
            {
                if ((cmbEstiloBarra.IsEnabled) && (cmbEstiloBarra.SelectedIndex != -1))
                    cmbEstiloBarra.SelectedIndex = 2;
                cmbEstiloBarra.IsEnabled = false;
            }


            switch (cmbEstiloPreco.SelectedIndex)
            {
                //Kagi
                case 5:
                    numEstiloPrecoParam1.Value = 0;
                    numEstiloPrecoParam2.Value = (double)EnumGeral.TipoDadoGrafico.Pontos;
                    lblParam1.Text = "RS:";
                    lblParam2.Text = "Pontos:";

                    stpParam1.Visibility = Visibility.Visible;
                    stpParam2.Visibility = Visibility.Visible;
                    break;
                //Point e Figure
                case 6:
                    numEstiloPrecoParam1.Value = 0;
                    numEstiloPrecoParam2.Value = 3;
                    lblParam1.Text = "Box:";
                    lblParam2.Text = "RS:";

                    stpParam1.Visibility = Visibility.Visible;
                    stpParam2.Visibility = Visibility.Visible;
                    break;
                //Renko
                case 7:
                    numEstiloPrecoParam1.Value = 1;
                    lblParam2.Text = "Box:";

                    stpParam1.Visibility = Visibility.Visible;
                    stpParam2.Visibility = Visibility.Collapsed;
                    break;


                //Three Line Break
                case 8:
                    numEstiloPrecoParam1.Value = 3;
                    lblParam2.Text = "Linhas:";

                    stpParam1.Visibility = Visibility.Visible;
                    stpParam2.Visibility = Visibility.Collapsed;
                    break;


                //Demais
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    stpParam1.Visibility = Visibility.Collapsed;
                    stpParam2.Visibility = Visibility.Collapsed;
                    break;
            }
        }


        /// <summary>
        /// Clique no botao ok
        /// </summary>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (chkBordaCandle.IsChecked == true)
            {
                //TODO: Condição se transparência estiver selecionada.
            }

            if (cmbTipoEscala.SelectedIndex == 0)
                configuracaoGrafico.TipoEscala = EnumGeral.TipoEscala.Linear;
            else
                configuracaoGrafico.TipoEscala = EnumGeral.TipoEscala.Semilog;

            configuracaoGrafico.PrecisaoEscala = (int)numPrecisaoEscala.Value;
            configuracaoGrafico.GradeHorizontal = (bool)chkHorizontal.IsChecked;
            configuracaoGrafico.GradeVertical = false;// (bool)chkHorizontal.IsChecked;
            configuracaoGrafico.PainelInfo = (bool)chkPainelInfoFixo.IsChecked;

            if ((bool)chkEscalaDireita.IsChecked)
                configuracaoGrafico.PosicaoEscala = EnumGeral.TipoAlinhamentoEscalaEnum.Direita;
            else
                configuracaoGrafico.PosicaoEscala = EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda;

            #region Estilo Preco
            switch (cmbEstiloPreco.SelectedIndex)
            {
                case 0:
                    configuracaoGrafico.EstiloPreco = EnumGeral.EstiloPrecoEnum.Padrao;
                    break;

                case 1:
                    configuracaoGrafico.EstiloPreco = EnumGeral.EstiloPrecoEnum.CandleVolume;
                    break;

                case 2:
                    configuracaoGrafico.EstiloPreco = EnumGeral.EstiloPrecoEnum.EquiVolume;
                    break;

                case 3:
                    configuracaoGrafico.EstiloPreco = EnumGeral.EstiloPrecoEnum.EquiVolumeShadow;
                    break;

                case 4:
                    configuracaoGrafico.EstiloPreco = EnumGeral.EstiloPrecoEnum.HeikinAshi;
                    break;

                case 5:
                    configuracaoGrafico.EstiloPreco = EnumGeral.EstiloPrecoEnum.Kagi;
                    configuracaoGrafico.EstiloPrecoParam1= Convert.ToDouble(numEstiloPrecoParam1.Value);
                    configuracaoGrafico.EstiloPrecoParam2 = Convert.ToDouble(numEstiloPrecoParam2.Value);
                    break;

                case 6:
                    configuracaoGrafico.EstiloPreco = EnumGeral.EstiloPrecoEnum.PontoEFigura;
                    configuracaoGrafico.EstiloPrecoParam1 = Convert.ToDouble(numEstiloPrecoParam1.Value);
                    configuracaoGrafico.EstiloPrecoParam2 = Convert.ToDouble(numEstiloPrecoParam2.Value);
                    break;

                case 7:
                    configuracaoGrafico.EstiloPreco = EnumGeral.EstiloPrecoEnum.Renko;
                    configuracaoGrafico.EstiloPrecoParam1 = Convert.ToDouble(numEstiloPrecoParam1.Value);
                    break;

                case 8:
                    configuracaoGrafico.EstiloPreco = EnumGeral.EstiloPrecoEnum.ThreeLineBreak;
                    configuracaoGrafico.EstiloPrecoParam1 = Convert.ToDouble(numEstiloPrecoParam1.Value);
                    break;
            }
            #endregion Estilo Preco

            #region Estilo Barra

            switch (cmbEstiloBarra.SelectedIndex)
            {
                case 0:
                    configuracaoGrafico.EstiloBarra = EnumGeral.TipoSeriesEnum.Linha;
                    break;

                case 1:
                    configuracaoGrafico.EstiloBarra = EnumGeral.TipoSeriesEnum.Barra;
                    break;

                case 2:
                    configuracaoGrafico.EstiloBarra = EnumGeral.TipoSeriesEnum.Candle;
                    break;
            }

            #endregion Estilo Barra

            configuracaoGrafico.EspacoADireitaGrafico= Convert.ToInt32(numEspacoDireitaGrafico.Value);
            configuracaoGrafico.UsarCoresAltaBaixaVolume = (bool)chkCorVolume.IsChecked;

            this.DialogResult = true;
        }

        /// <summary>
        /// Se o campo da borda do candle for assinalado a borda permanece transparente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBordaCandle_Checked(object sender, RoutedEventArgs e)
        {
            configuracaoGrafico.CorBordaCandleAlta = Color.FromArgb(0, 0, 0, 0);
            configuracaoGrafico.CorBordaCandleBaixa = Color.FromArgb(0, 0, 0, 0);
            rectBordaCAlta.Visibility = Visibility.Collapsed;
            rectBordaCBaixa.Visibility = Visibility.Collapsed;
            lblBordaCandle.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Se o campo da borda do candle for assinalado a borda recebe a cor selecionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBordaCandle_Unchecked(object sender, RoutedEventArgs e)
        {
            rectBordaCAlta.Visibility = Visibility.Visible;
            rectBordaCBaixa.Visibility = Visibility.Visible;
            lblBordaCandle.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Clique no botao cancel
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion Eventos

    }
}
