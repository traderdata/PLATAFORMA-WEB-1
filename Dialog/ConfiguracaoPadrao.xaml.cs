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
using Traderdata.Client.TerminalWEB.Util;
using Traderdata.Client.Componente.GraficoSL.Util;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class ConfiguracaoPadrao : ChildWindow
    {
        #region Campos e Construtores

        //Variavel de controle geral para a configuração do cliente
        private TerminalWebSVC.ConfiguracaoPadraoDTO configuracaoPadraoDTO = new TerminalWebSVC.ConfiguracaoPadraoDTO();
       
        //Dialogs
        private ColorDialog ColorDialog;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuracaoPadraoDTO"></param>
        public ConfiguracaoPadrao(TerminalWebSVC.ConfiguracaoPadraoDTO configuracaoPadraoDTO)
        {
            InitializeComponent();

            cmbTipo.Items.Add("Pontilhado");
            cmbTipo.Items.Add("Pontilhado e Tracejado");
            cmbTipo.Items.Add("Tracejado");
            cmbTipo.Items.Add("Solido");
            cmbTipo.SelectedIndex = 0;

            cmbTipoLinhaIndicador.Items.Add("Pontilhado");
            cmbTipoLinhaIndicador.Items.Add("Pontilhado e Tracejado");
            cmbTipoLinhaIndicador.Items.Add("Tracejado");
            cmbTipoLinhaIndicador.Items.Add("Solido");
            cmbTipoLinhaIndicador.SelectedIndex = 0;

            cmbTipoLinhaSerie1.Items.Add("Pontilhado");
            cmbTipoLinhaSerie1.Items.Add("Pontilhado e Tracejado");
            cmbTipoLinhaSerie1.Items.Add("Tracejado");
            cmbTipoLinhaSerie1.Items.Add("Solido");
            cmbTipoLinhaSerie1.SelectedIndex = 0;

            cmbTipoLinhaSerie2.Items.Add("Pontilhado");
            cmbTipoLinhaSerie2.Items.Add("Pontilhado e Tracejado");
            cmbTipoLinhaSerie2.Items.Add("Tracejado");
            cmbTipoLinhaSerie2.Items.Add("Solido");
            cmbTipoLinhaSerie2.SelectedIndex = 0;

            this.configuracaoPadraoDTO = configuracaoPadraoDTO;

            System.Globalization.NumberFormatInfo nf = new System.Globalization.NumberFormatInfo();
            nf.NumberDecimalSeparator = ".";
            nf.NumberDecimalDigits = 2;

            string[] fibo = this.configuracaoPadraoDTO.Configuracao.ConfigFiboRetracements.Replace(",", ".").Split(';');
            if (fibo.Length > 1)
            {
                numNumeroLinhaFiboRetracements.Value = fibo.Length;

                if (fibo.Length > 0)
                    if (fibo[0].Length > 0)
                        numFiboRetracValor1.Value = Convert.ToDouble(fibo[0], nf);
                else
                    numFiboRetracValor1.Value = 0;

                if (fibo.Length > 1)
                    if (fibo[1].Length > 0)
                        numFiboRetracValor2.Value = Convert.ToDouble(fibo[1], nf);
                else
                    numFiboRetracValor2.Value = 0;

                if (fibo.Length > 2)
                    if (fibo[2].Length > 0)
                        numFiboRetracValor3.Value = Convert.ToDouble(fibo[2], nf);
                else
                    numFiboRetracValor3.Value = 0;

                if (fibo.Length > 3)
                    if (fibo[3].Length > 0)
                        numFiboRetracValor4.Value = Convert.ToDouble(fibo[3], nf);
                else
                    numFiboRetracValor4.Value = 0;

                if (fibo.Length > 4)
                    if (fibo[4].Length > 0)
                        numFiboRetracValor5.Value = Convert.ToDouble(fibo[4], nf);
                else
                    numFiboRetracValor5.Value = 0;

                if (fibo.Length > 5)
                    if (fibo[5].Length > 0)
                        numFiboRetracValor6.Value = Convert.ToDouble(fibo[5], nf);
                else
                    numFiboRetracValor6.Value = 0;

                if (fibo.Length > 6)
                    if (fibo[6].Length > 0)
                        numFiboRetracValor7.Value = Convert.ToDouble(fibo[6], nf);
                else
                    numFiboRetracValor7.Value = 0;
            }

            if (this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam1 < 0)
                this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam1 = 0;

            if (this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam2 < 0)
                this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam2 = 0;

            //Mostrando cores nos retangulos
            rectCorIndicador.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorIndicador));
            rectCorSerieAux1.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorIndicadorAux1));
            rectCorSerieAux2.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorIndicadorAux2));

            rectBordaCAlta.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorBordaCandleAlta));
            rectBordaCBaixa.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorBordaCandleBaixa));

            rectCAlta.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorCandleAlta));
            rectCBaixa.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorCandleBaixa));
            rectCorFundo.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorFundo));

            #region Tipo Escala

            //Mudando a cor dos combos
            cmbEstiloBarra.Foreground = new SolidColorBrush(Colors.Black);
            cmbEstiloPreco.Foreground = new SolidColorBrush(Colors.Black);
            cmbTipoEscala.Foreground = new SolidColorBrush(Colors.Black);


            //Adicionando tipos de escala       
            cmbTipoEscala.Items.Add("Normal");
            //cmbTipoEscala.Items.Add("Semilog");

            //Selecionando tipo escala
            //if (this.configuracaoPadraoDTO.Configuracao.TipoEscala == (int)EnumGeral.TipoEscala.Linear)
                cmbTipoEscala.SelectedIndex = 0;
            //else
            //    cmbTipoEscala.SelectedIndex = 1;

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

            switch ((EnumGeral.EstiloPrecoEnum)this.configuracaoPadraoDTO.Configuracao.EstiloPreco)
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

            switch ((EnumGeral.TipoSeriesEnum) this.configuracaoPadraoDTO.Configuracao.EstiloBarra)
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
            if (this.configuracaoPadraoDTO.Configuracao.PosicaoEscala == (int)EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda)
                chkEscalaDireita.IsChecked = false;
            else
                chkEscalaDireita.IsChecked = true;

            //Selecionando precisao da escala
            numPrecisaoEscala.Value = (double)this.configuracaoPadraoDTO.Configuracao.PrecisaoEscala;

            //Setando grades
            chkHorizontal.IsChecked = (bool)this.configuracaoPadraoDTO.Configuracao.GradeHorizontal;// gradeHorizontal;
            chkVertical.IsChecked = (bool)this.configuracaoPadraoDTO.Configuracao.GradeVertical;

            //Info panel
            chkPainelInfoFixo.IsChecked = (bool)this.configuracaoPadraoDTO.Configuracao.PainelInfo;

            //Espaco a direita do grafico
            numEspacoDireitaGrafico.Value = (double)this.configuracaoPadraoDTO.Configuracao.EspacoADireitaDoGrafico;

            chkCorVolume.IsChecked = this.configuracaoPadraoDTO.Configuracao.UsarCoresAltaBaixaVolume;

            rectCorFundoObjeto.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorObjeto));
            numGrossuraObjeto.Value = (double)this.configuracaoPadraoDTO.Configuracao.GrossuraObjeto ;
            chkLinhaMagnetica.IsChecked = this.configuracaoPadraoDTO.Configuracao.LinhaMagnetica;
            chkLinhaInfinita.IsChecked = this.configuracaoPadraoDTO.Configuracao.LinhaTendenciaInfinita;

            switch ((EnumGeral.TipoLinha)this.configuracaoPadraoDTO.Configuracao.TipoLinhaObjeto)
            {
                case EnumGeral.TipoLinha.Pontilhado:
                    cmbTipo.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoLinha.TracejadoPontilhado:
                    cmbTipo.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoLinha.Tracejado:
                    cmbTipo.SelectedIndex = 2;
                    break;

                default:
                    cmbTipo.SelectedIndex = 3;
                    break;
            }

            switch ((EnumGeral.TipoLinha)this.configuracaoPadraoDTO.Configuracao.TipoLinhaIndicador)
            {
                case EnumGeral.TipoLinha.Pontilhado:
                    cmbTipoLinhaIndicador.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoLinha.TracejadoPontilhado:
                    cmbTipoLinhaIndicador.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoLinha.Tracejado:
                    cmbTipoLinhaIndicador.SelectedIndex = 2;
                    break;

                default:
                    cmbTipoLinhaIndicador.SelectedIndex = 3;
                    break;
            }

            switch ((EnumGeral.TipoLinha)this.configuracaoPadraoDTO.Configuracao.TipoLinhaIndicadorAux1)
            {
                case EnumGeral.TipoLinha.Pontilhado:
                    cmbTipoLinhaSerie1.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoLinha.TracejadoPontilhado:
                    cmbTipoLinhaSerie1.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoLinha.Tracejado:
                    cmbTipoLinhaSerie1.SelectedIndex = 2;
                    break;

                default:
                    cmbTipoLinhaSerie1.SelectedIndex = 3;
                    break;
            }

            switch ((EnumGeral.TipoLinha)this.configuracaoPadraoDTO.Configuracao.TipoLinhaIndicadorAux2)
            {
                case EnumGeral.TipoLinha.Pontilhado:
                    cmbTipoLinhaSerie2.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoLinha.TracejadoPontilhado:
                    cmbTipoLinhaSerie2.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoLinha.Tracejado:
                    cmbTipoLinhaSerie2.SelectedIndex = 2;
                    break;

                default:
                    cmbTipoLinhaSerie2.SelectedIndex = 3;
                    break;
            }
        }

        #endregion Campos e Construtores

        #region Propriedades
              
        /// <summary>
        /// Retorna a configuracao padrao
        /// </summary>
        public TerminalWebSVC.ConfiguracaoPadraoDTO ConfiguracaoPadraoDTO
        {
            get { return this.configuracaoPadraoDTO; }            
        }

        #endregion Propriedades                

        #region Eventos

        #region Eventos Principais

        /// <summary>
        /// Leitura da tela e set do objeto local configuracaoDTO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAplicar_Click(object sender, RoutedEventArgs e)
        {
            if (chkBordaCandle.IsChecked == true)
            {
                //TODO: Condição se transparência estiver selecionada.
            }

            if (cmbTipoEscala.SelectedIndex == 0)
                this.configuracaoPadraoDTO.Configuracao.TipoEscala = (int)EnumGeral.TipoEscala.Linear;
            else
                this.configuracaoPadraoDTO.Configuracao.TipoEscala = (int)EnumGeral.TipoEscala.Semilog;

            this.configuracaoPadraoDTO.Configuracao.PrecisaoEscala = (int)numPrecisaoEscala.Value;
            this.configuracaoPadraoDTO.Configuracao.GradeHorizontal = false;// (bool)chkHorizontal.IsChecked;
            this.configuracaoPadraoDTO.Configuracao.GradeVertical = (bool)chkHorizontal.IsChecked;
            this.configuracaoPadraoDTO.Configuracao.PainelInfo = (bool)chkPainelInfoFixo.IsChecked;
            this.configuracaoPadraoDTO.Configuracao.LinhaMagnetica = (bool)chkLinhaMagnetica.IsChecked;
            this.configuracaoPadraoDTO.Configuracao.LinhaTendenciaInfinita = (bool)chkLinhaInfinita.IsChecked;
            this.configuracaoPadraoDTO.Configuracao.GrossuraObjeto = (double)numGrossuraObjeto.Value;
            this.configuracaoPadraoDTO.Configuracao.TipoLinhaObjeto = (int)ObtemLinhaSelecionada();

            this.configuracaoPadraoDTO.Configuracao.TipoLinhaIndicador = (int)ObtemLinhaIndicador();
            this.configuracaoPadraoDTO.Configuracao.TipoLinhaIndicadorAux1 = (int)ObtemLinhaIndicadorAux1();
            this.configuracaoPadraoDTO.Configuracao.TipoLinhaIndicadorAux2 = (int)ObtemLinhaIndicadorAux2();

            if ((bool)chkEscalaDireita.IsChecked)
                this.configuracaoPadraoDTO.Configuracao.PosicaoEscala = (int)EnumGeral.TipoAlinhamentoEscalaEnum.Direita;
            else
                this.configuracaoPadraoDTO.Configuracao.PosicaoEscala = (int)EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda;

            this.configuracaoPadraoDTO.Configuracao.ConfigFiboRetracements = ObtemValorFiboRetrac();

            #region Estilo Preco
            switch (cmbEstiloPreco.SelectedIndex)
            {
                case 0:
                    this.configuracaoPadraoDTO.Configuracao.EstiloPreco = (int)EnumGeral.EstiloPrecoEnum.Padrao;
                    break;

                case 1:
                    this.configuracaoPadraoDTO.Configuracao.EstiloPreco = (int)EnumGeral.EstiloPrecoEnum.CandleVolume;
                    break;

                case 2:
                    this.configuracaoPadraoDTO.Configuracao.EstiloPreco = (int)EnumGeral.EstiloPrecoEnum.EquiVolume;
                    break;

                case 3:
                    this.configuracaoPadraoDTO.Configuracao.EstiloPreco = (int)EnumGeral.EstiloPrecoEnum.EquiVolumeShadow;
                    break;

                case 4:
                    this.configuracaoPadraoDTO.Configuracao.EstiloPreco = (int)EnumGeral.EstiloPrecoEnum.HeikinAshi;
                    break;

                case 5:
                    this.configuracaoPadraoDTO.Configuracao.EstiloPreco = (int)EnumGeral.EstiloPrecoEnum.Kagi;
                    this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam1 = Convert.ToDouble(numEstiloPrecoParam1.Value);
                    this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam2 = Convert.ToDouble(numEstiloPrecoParam2.Value);
                    break;

                case 6:
                    this.configuracaoPadraoDTO.Configuracao.EstiloPreco = (int)EnumGeral.EstiloPrecoEnum.PontoEFigura;
                    this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam1 = Convert.ToDouble(numEstiloPrecoParam1.Value);
                    this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam2 = Convert.ToDouble(numEstiloPrecoParam2.Value);
                    break;

                case 7:
                    this.configuracaoPadraoDTO.Configuracao.EstiloPreco = (int)EnumGeral.EstiloPrecoEnum.Renko;
                    this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam1 = Convert.ToDouble(numEstiloPrecoParam1.Value);
                    break;

                case 8:
                    this.configuracaoPadraoDTO.Configuracao.EstiloPreco = (int)EnumGeral.EstiloPrecoEnum.ThreeLineBreak;
                    this.configuracaoPadraoDTO.Configuracao.EstiloPrecoParam1 = Convert.ToDouble(numEstiloPrecoParam1.Value);
                    break;
            }
            #endregion Estilo Preco

            #region Estilo Barra

            switch (cmbEstiloBarra.SelectedIndex)
            {
                case 0:
                    this.configuracaoPadraoDTO.Configuracao.EstiloBarra = (int)EnumGeral.TipoSeriesEnum.Linha;
                    break;

                case 1:
                    this.configuracaoPadraoDTO.Configuracao.EstiloBarra = (int)EnumGeral.TipoSeriesEnum.Barra;
                    break;

                case 2:
                    this.configuracaoPadraoDTO.Configuracao.EstiloBarra = (int)EnumGeral.TipoSeriesEnum.Candle;
                    break;
            }

            #endregion Estilo Barra

            this.configuracaoPadraoDTO.Configuracao.EspacoADireitaDoGrafico = Convert.ToInt32(numEspacoDireitaGrafico.Value);
            this.configuracaoPadraoDTO.Configuracao.UsarCoresAltaBaixaVolume = (bool)chkCorVolume.IsChecked;

            this.DialogResult = true;
        }

        /// <summary>
        /// Clique no botão cancelar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion

        #region Eventos de interface

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numNumeroLinhaFiboRetracements_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int numValue = 0;

            if (numNumeroLinhaFiboRetracements != null)
            {
                if (Int32.TryParse(numNumeroLinhaFiboRetracements.Value.ToString(), out numValue))
                {
                    stpFiboRet1.Visibility = Visibility.Collapsed;
                    stpFiboRet2.Visibility = Visibility.Collapsed;
                    stpFiboRet3.Visibility = Visibility.Collapsed;
                    stpFiboRet4.Visibility = Visibility.Collapsed;
                    stpFiboRet5.Visibility = Visibility.Collapsed;
                    stpFiboRet6.Visibility = Visibility.Collapsed;
                    stpFiboRet7.Visibility = Visibility.Collapsed;

                    if (numValue > 0)
                        stpFiboRet1.Visibility = Visibility.Visible;

                    if (numValue > 1)
                        stpFiboRet2.Visibility = Visibility.Visible;

                    if (numValue > 2)
                        stpFiboRet3.Visibility = Visibility.Visible;

                    if (numValue > 3)
                        stpFiboRet4.Visibility = Visibility.Visible;

                    if (numValue > 4)
                        stpFiboRet5.Visibility = Visibility.Visible;

                    if (numValue > 5)
                        stpFiboRet6.Visibility = Visibility.Visible;

                    if (numValue > 6)
                        stpFiboRet7.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Evento que seta a cor padrao dos objetos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCorObjeto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog = new ColorDialog(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorObjeto));
            ColorDialog.Closing += (sender1, e1) =>
            {
                this.configuracaoPadraoDTO.Configuracao.CorObjeto = ConvertUtil.ConvertFromColorToString(ColorDialog.Cor); rectCorFundoObjeto.Fill
                = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorObjeto));
            };

            ColorDialog.Show();
        }

        /// <summary>
        /// Clique na cor de fundo.
        /// </summary>
        private void rectFundo_Click(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush corFundoAux = ConvertUtil.ConvertFromStringToBrush(this.configuracaoPadraoDTO.Configuracao.CorFundo);

            ColorDialog = new ColorDialog(corFundoAux.Color);
            ColorDialog.Closing += (sender1, e1) =>
            {
                this.configuracaoPadraoDTO.Configuracao.CorFundo = ConvertUtil.ConvertFromColorToString(ColorDialog.Cor);
                rectCorFundo.Fill = ConvertUtil.ConvertFromStringToBrush(this.configuracaoPadraoDTO.Configuracao.CorFundo);
            };

            ColorDialog.Show();
        }


        /// <summary>
        /// Clique na cor borda de candle alta
        /// </summary>
        private void rectBordaCandleAlta_Click(object sender, MouseButtonEventArgs e)
        {
            ColorDialog = new ColorDialog(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorBordaCandleAlta));
            ColorDialog.Closing += (sender1, e1) =>
            {
                this.configuracaoPadraoDTO.Configuracao.CorBordaCandleAlta = ConvertUtil.ConvertFromColorToString(ColorDialog.Cor); rectBordaCAlta.Fill
                = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorBordaCandleAlta));
            };

            ColorDialog.Show();
        }


        /// <summary>
        /// Clique na cor borda de candle baixa
        /// </summary>
        private void rectBordaCandleBaixa_Click(object sender, MouseButtonEventArgs e)
        {
            ColorDialog = new ColorDialog(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorBordaCandleBaixa));
            ColorDialog.Closing += (sender1, e1) =>
            {
                this.configuracaoPadraoDTO.Configuracao.CorBordaCandleBaixa = ConvertUtil.ConvertFromColorToString(ColorDialog.Cor); rectBordaCBaixa.Fill
                = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorBordaCandleBaixa));
            };

            ColorDialog.Show();
        }


        /// <summary>
        /// Clique na cor de candle alta
        /// </summary>
        private void rectCAlta_Click(object sender, MouseButtonEventArgs e)
        {
            ColorDialog = new ColorDialog(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorCandleAlta));
            ColorDialog.Closing += (sender1, e1) =>
            {
                this.configuracaoPadraoDTO.Configuracao.CorCandleAlta = ConvertUtil.ConvertFromColorToString(ColorDialog.Cor); rectCAlta.Fill
                = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorCandleAlta));
            };

            ColorDialog.Show();
        }

        /// <summary>
        /// Clique na cor de candle baixa
        /// </summary>
        private void rectCBaixa_Click(object sender, MouseButtonEventArgs e)
        {
            ColorDialog = new ColorDialog(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorCandleBaixa));
            ColorDialog.Closing += (sender1, e1) =>
            {
                this.configuracaoPadraoDTO.Configuracao.CorCandleBaixa = ConvertUtil.ConvertFromColorToString(ColorDialog.Cor); rectCBaixa.Fill
                = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorCandleBaixa));
            };

            ColorDialog.Show();
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
                //cmbEstiloBarra.SelectedIndex = 0;
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
        /// Se o campo da borda do candle for assinalado a borda permanece transparente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBordaCandle_Checked(object sender, RoutedEventArgs e)
        {
            this.configuracaoPadraoDTO.Configuracao.CorBordaCandleAlta = ConvertUtil.ConvertFromColorToString(Color.FromArgb(0, 0, 0, 0));
            this.configuracaoPadraoDTO.Configuracao.CorBordaCandleBaixa = ConvertUtil.ConvertFromColorToString(Color.FromArgb(0, 0, 0, 0));
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

        private void rectCorIndicador_Click(object sender, MouseButtonEventArgs e)
        {
            ColorDialog = new ColorDialog(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorIndicador));
            ColorDialog.Closing += (sender1, e1) =>
            {
                this.configuracaoPadraoDTO.Configuracao.CorIndicador = ConvertUtil.ConvertFromColorToString(ColorDialog.Cor); rectCorIndicador.Fill
                = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorIndicador));
            };

            ColorDialog.Show();
        }

        private void rectCorSerieAux1_Click(object sender, MouseButtonEventArgs e)
        {
            ColorDialog = new ColorDialog(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorIndicadorAux1));
            ColorDialog.Closing += (sender1, e1) =>
            {
                this.configuracaoPadraoDTO.Configuracao.CorIndicadorAux1 = ConvertUtil.ConvertFromColorToString(ColorDialog.Cor);
                rectCorSerieAux1.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorIndicadorAux1));
            };

            ColorDialog.Show();
        }

        private void rectCorSerieAux2_Click(object sender, MouseButtonEventArgs e)
        {
            ColorDialog = new ColorDialog(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorIndicadorAux2));
            ColorDialog.Closing += (sender1, e1) =>
            {
                this.configuracaoPadraoDTO.Configuracao.CorIndicadorAux2 = ConvertUtil.ConvertFromColorToString(ColorDialog.Cor);
                rectCorSerieAux2.Fill = new SolidColorBrush(ConvertUtil.ConvertFromStringToColor(this.configuracaoPadraoDTO.Configuracao.CorIndicadorAux2));
            };

            ColorDialog.Show();
        }

        #endregion


        #endregion Eventos

        #region Métodos

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string ObtemValorFiboRetrac()
        {
            string valor = "";
            int numValue = 0;

            if (Int32.TryParse(numNumeroLinhaFiboRetracements.Value.ToString(), out numValue))
            {
                if (numValue > 0)
                {
                    valor += numFiboRetracValor1.Value.ToString();

                    if (valor != "")
                        valor += ";";
                }

                if (numValue > 1)
                {
                    valor += numFiboRetracValor2.Value.ToString();

                    if (valor != "")
                        valor += ";";
                }

                if (numValue > 2)
                {
                    valor += numFiboRetracValor3.Value.ToString();

                    if (valor != "")
                        valor += ";";
                }

                if (numValue > 3)
                {
                    valor += numFiboRetracValor4.Value.ToString();

                    if (valor != "")
                        valor += ";";
                }

                if (numValue > 4)
                {
                    valor += numFiboRetracValor5.Value.ToString();

                    if (valor != "")
                        valor += ";";
                }

                if (numValue > 5)
                {
                    valor += numFiboRetracValor6.Value.ToString();

                    if (valor != "")
                        valor += ";";
                }

                if (numValue > 6)
                    valor += numFiboRetracValor7.Value.ToString();
            }

            return valor;
        }

        /// <summary>
        /// Retorna a linha selecionada na combo.
        /// </summary>
        /// <returns></returns>
        private int ObtemLinhaSelecionada()
        {
            switch (cmbTipo.SelectedIndex)
            {
                case 0:
                    return (int) EnumGeral.TipoLinha.Pontilhado;

                case 1:
                    return (int)EnumGeral.TipoLinha.TracejadoPontilhado;

                case 2:
                    return (int)EnumGeral.TipoLinha.Tracejado;

                default:
                    return (int)EnumGeral.TipoLinha.Solido;
            }
        }

        /// <summary>
        /// Retorna a linha selecionada na combo.
        /// </summary>
        /// <returns></returns>
        private int ObtemLinhaIndicador()
        {
            switch (cmbTipoLinhaIndicador.SelectedIndex)
            {
                case 0:
                    return (int)EnumGeral.TipoLinha.Pontilhado;

                case 1:
                    return (int)EnumGeral.TipoLinha.TracejadoPontilhado;

                case 2:
                    return (int)EnumGeral.TipoLinha.Tracejado;

                default:
                    return (int)EnumGeral.TipoLinha.Solido;
            }
        }

        /// <summary>
        /// Retorna a linha selecionada na combo.
        /// </summary>
        /// <returns></returns>
        private int ObtemLinhaIndicadorAux1()
        {
            switch (cmbTipoLinhaSerie1.SelectedIndex)
            {
                case 0:
                    return (int)EnumGeral.TipoLinha.Pontilhado;

                case 1:
                    return (int)EnumGeral.TipoLinha.TracejadoPontilhado;

                case 2:
                    return (int)EnumGeral.TipoLinha.Tracejado;

                default:
                    return (int)EnumGeral.TipoLinha.Solido;
            }
        }

        /// <summary>
        /// Retorna a linha selecionada na combo.
        /// </summary>
        /// <returns></returns>
        private int ObtemLinhaIndicadorAux2()
        {
            switch (cmbTipoLinhaSerie2.SelectedIndex)
            {
                case 0:
                    return (int)EnumGeral.TipoLinha.Pontilhado;

                case 1:
                    return (int)EnumGeral.TipoLinha.TracejadoPontilhado;

                case 2:
                    return (int)EnumGeral.TipoLinha.Tracejado;

                default:
                    return (int)EnumGeral.TipoLinha.Solido;
            }
        }              

        

        #endregion Métodos

        

        
    }
}

