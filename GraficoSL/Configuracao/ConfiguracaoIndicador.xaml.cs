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
using Traderdata.Client.Componente.GraficoSL.Util;
using Traderdata.Client.Componente.GraficoSL.Main;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.Configuracao
{
    public partial class ConfiguracaoIndicador : ChildWindow
    {
        #region Campos e Construtores

        public enum IndicadorTipoParametro {Inteiro, Double, String};
        private Grafico grafico = null;
        private bool edicao = false;
        private List<IndicadorParametroDTO> parametrosIndicador = new List<IndicadorParametroDTO>();

        private ChartPanel painel = null;
        private Color? corIndicadorAlta;
        private Color? corIndicadorBaixa;
        private EnumGeral.TipoLinha tipoLinha = EnumGeral.TipoLinha.Solido;
        private int grossura = 1;
        private string nomeIndicador = "";

        public bool ExcluiIndicador { get; set; }

        /// <summary>
        /// Construtor para adição.
        /// </summary>
        /// <param name="grafico"></param>
        /// <param name="parametrosIndicador"></param>
        public ConfiguracaoIndicador (Grafico grafico, List<IndicadorParametroDTO> parametrosIndicador, Color corIndicador)
        {
            InitializeComponent();

            cmbTipoLinha.Items.Add("Pontilhado");
            cmbTipoLinha.Items.Add("Pontilhado e Tracejado");
            cmbTipoLinha.Items.Add("Tracejado");
            cmbTipoLinha.Items.Add("Solido");
            cmbTipoLinha.SelectedIndex = 3;

            ExcluiButton.Visibility = System.Windows.Visibility.Collapsed;

            this.rectCorAlta.Fill = new SolidColorBrush(corIndicador);
            this.rectCorBaixa.Fill = new SolidColorBrush(corIndicador);

            this.grafico = grafico;
            this.edicao = false;
            this.parametrosIndicador = parametrosIndicador;

            CriaInterfaceParametros(parametrosIndicador);
        }

        /// <summary>
        /// Construtor para edição.
        /// </summary>
        /// <param name="grafico"></param>
        /// <param name="parametrosIndicador"></param>
        /// <param name="corAlta"></param>
        /// <param name="corBaixa"></param>
        /// <param name="EnumGeral.tipoLinha"></param>
        /// <param name="grossura"></param>
        public ConfiguracaoIndicador(Grafico grafico, List<IndicadorParametroDTO> parametrosIndicador, Color? corAlta, Color? corBaixa, EnumGeral.TipoLinha TipoLinha,
                               int grossura, string nomeIndicador)
        {
            InitializeComponent();

            cmbTipoLinha.Items.Add("Pontilhado");
            cmbTipoLinha.Items.Add("Pontilhado e Tracejado");
            cmbTipoLinha.Items.Add("Tracejado");
            cmbTipoLinha.Items.Add("Solido");

            ExcluiButton.Visibility = System.Windows.Visibility.Visible;

            switch (TipoLinha)
            {
                case EnumGeral.TipoLinha.Pontilhado:
                    cmbTipoLinha.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoLinha.TracejadoPontilhado:
                    cmbTipoLinha.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoLinha.Tracejado:
                    cmbTipoLinha.SelectedIndex = 2;
                    break;

                case EnumGeral.TipoLinha.Solido:
                    cmbTipoLinha.SelectedIndex = 3;
                    break;
            }

            if (corAlta != null)
                this.rectCorAlta.Fill = new SolidColorBrush((Color)corAlta);
            else
                this.rectCorAlta.Fill = new SolidColorBrush(Colors.Blue);

            if (corBaixa != null)
                this.rectCorBaixa.Fill = new SolidColorBrush((Color)corBaixa);
            else
                this.rectCorBaixa.Fill = new SolidColorBrush(Colors.Blue);
            
            this.numGrossura.Value = grossura;
            this.rdbNovoPainel.IsEnabled = false;
            this.rdbNovoPainelAbaixo.IsEnabled = false;
            this.rdbPainelOutros.IsEnabled = false;
            this.rdbPainelPrecos.IsEnabled = false;
            this.rdbPainelVolume.IsEnabled = false;
            this.lblPainel.Visibility = System.Windows.Visibility.Collapsed;
            
            this.grafico = grafico;
            this.edicao = true;
            this.parametrosIndicador = parametrosIndicador;
            this.nomeIndicador = nomeIndicador;

            CriaInterfaceParametros(parametrosIndicador);
        }

        #endregion Campos e Construtores

        #region Propriedades

        public int Grossura
        {
            get { return grossura; }
            set { grossura = value; }
        }

        public EnumGeral.TipoLinha TipoLinha
        {
            get { return tipoLinha; }
            set { tipoLinha = value; }
        }

        public Color? CorIndicadorAlta
        {
            get { return corIndicadorAlta; }
            set { corIndicadorAlta = value; }
        }

        public Color? CorIndicadorBaixa
        {
            get { return corIndicadorBaixa; }
            set { corIndicadorBaixa = value; }
        }

        public ChartPanel Painel
        {
            get { return painel; }
            set { painel = value; }
        }

        public List<IndicadorParametroDTO> ParametrosIndicador
        {
            get { return parametrosIndicador; }
            set { parametrosIndicador = value; }
        }

        #endregion Propriedades

        #region Eventos

        private void btnCorAlta_Click(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush corAux = (SolidColorBrush)rectCorAlta.Fill;

            ColorDialog cor = new ColorDialog(corAux.Color);
            cor.Closing += (sender1, e1) => rectCorAlta.Fill = cor.CorBrush;
            cor.Show();
        }

        private void btnCorBaixa_Click(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush corAux = (SolidColorBrush)rectCorBaixa.Fill;

            ColorDialog cor = new ColorDialog(corAux.Color);
            cor.Closing += (sender1, e1) => rectCorBaixa.Fill = cor.CorBrush;
            cor.Show();
        }

        
        /// <summary>
        /// Botão de transferência de cor dos retângulos
        /// </summary>
        private void btnTranfCandleAlta_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.rectCorBaixa.Fill = this.rectCorAlta.Fill;
        }

        /// <summary>
        /// Botão de transferência de cor dos retângulos
        /// </summary>
        private void btnTranfCandleBaixa_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.rectCorAlta.Fill = this.rectCorBaixa.Fill;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            bool verificacoesOK = true;
            this.ExcluiIndicador = false;

            if ((!edicao) && (((bool)rdbNovoPainel.IsChecked) || ((bool)rdbNovoPainelAbaixo.IsChecked)))
            {                
                if ((bool)rdbNovoPainel.IsChecked)
                    painel = grafico.stockChartIndicadores.AddChartPanel();
                else
                    painel = grafico.stockChart.AddChartPanel();

                if (painel != null)
                    painel.Background = grafico.PaineisExistentes[0].Background;
                else
                {
                    MessageBox.Show("Não foi possível adicionar um novo painel, devido a falta de espaço. Exclua um painel ou redimensione-os.", "Atenção", MessageBoxButton.OK);
                    verificacoesOK = false;
                }
            }


            if (verificacoesOK)
            {
                grossura = (int)numGrossura.Value;
                TipoLinha = ObtemLinhaSelecionada();

                SolidColorBrush corAux = (SolidColorBrush)rectCorAlta.Fill;
                SolidColorBrush corAux2 = (SolidColorBrush)rectCorBaixa.Fill;

                corIndicadorAlta = corAux.Color;
                corIndicadorBaixa = corAux2.Color;

                AtualizaParametros();

                this.DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion Eventos

        #region Métodos

        #region CriaInterfaceParametros()
        /// <summary>
        /// Cria campos de parâmetros dinamicamente.
        /// </summary>
        /// <param name="listaParametros"></param>
        private void CriaInterfaceParametros(List<IndicadorParametroDTO> listaParametros)
        {
            try
            {
                //Percorrendo os parametros para criar os campos correspondentes
                foreach (IndicadorParametroDTO parametro in listaParametros)
                {
                    //Criando um stackPanel para conter o parametro (os itens do parametros devem entrar aqui)
                    StackPanel stackParametro = new StackPanel();
                    stackParametro.Orientation = Orientation.Horizontal;
                    
                    //Adicionando este stackPanel ao stack que conterá todos os parametros
                    stpParametros.Children.Add(stackParametro);

                    //Criando label que é comum a todos
                    TextBlock ttb = new TextBlock();
                    ttb.Text = parametro.NomeParametro + ":";
                    ttb.Margin = new Thickness(20, 13, 0, 0);
                    ttb.Foreground = new SolidColorBrush(Colors.White);
                    
                    //Adicionando o label ao stackPanel
                    stackParametro.Children.Add(ttb);

                    //Verificando o tipo do parâmetro para criar o campo correto
                    switch (parametro.TipoParametro)
                    {
                        case IndicadorTipoParametro.Double:
                            NumericUpDown numDouble = new NumericUpDown();
                            numDouble.DecimalPlaces = 2;
                            numDouble.Value = parametro.ValorDouble;
                            numDouble.Maximum = 10000;
                            numDouble.Margin = new Thickness(10, 13, 0, 0);
                            numDouble.Foreground = new SolidColorBrush(Colors.White);

                            //Adicionando ao stackPanel do parametro
                            stackParametro.Children.Add(numDouble);
                            break;


                        case IndicadorTipoParametro.Inteiro:
                            //Se não for um campo especial devo inserir um campo numerico comum
                            if (parametro.CampoEspecial.Trim() == "")
                            {
                                NumericUpDown numInt = new NumericUpDown();
                                numInt.DecimalPlaces = 0;
                                numInt.Value = parametro.ValorInteiro;
                                numInt.Maximum = 10000;
                                numInt.Margin = new Thickness(10, 13, 0, 0);
                                numInt.Foreground = new SolidColorBrush(Colors.White);

                                //Adicionando ao stackPanel do parametro
                                stackParametro.Children.Add(numInt);
                            }
                            //Senão, devo verificar que tipo de campo especial se trata
                            else
                            {
                                #region Campos especiais para Inteiro

                                switch (parametro.CampoEspecial)
                                {
                                    case "Média Móvel":
                                        //Criando Combo Box
                                        ComboBox cmbMediaMovel = new ComboBox();
                                        cmbMediaMovel.Margin = new Thickness(10, 13, 0, 0);
                                        cmbMediaMovel.Items.Add("Simples");
                                        cmbMediaMovel.Items.Add("Exponencial");
                                        cmbMediaMovel.Items.Add("Ponderado");

                                        //Selecionando o tipo de media movel padrao
                                        cmbMediaMovel.SelectedIndex = 0;

                                        //Adicionando ao stackPanel do parametro
                                        stackParametro.Children.Add(cmbMediaMovel);
                                        break;

                                    case "Tipo M.Móvel":
                                        //Criando Combo Box
                                        ComboBox cmbTipoMediaMovel = new ComboBox();
                                        cmbTipoMediaMovel.Margin = new Thickness(10, 13, 0, 0);
                                        cmbTipoMediaMovel.Items.Add("Simples");
                                        cmbTipoMediaMovel.Items.Add("Exponencial");
                                        cmbTipoMediaMovel.Items.Add("Séries Tempo");
                                        cmbTipoMediaMovel.Items.Add("Triangular");
                                        cmbTipoMediaMovel.Items.Add("Variável");
                                        cmbTipoMediaMovel.Items.Add("VIDYA");
                                        cmbTipoMediaMovel.Items.Add("Ponderado");

                                        //Selecionando o tipo de media movel padrao
                                        cmbTipoMediaMovel.SelectedIndex = 0;

                                        //Adicionando ao stackPanel do parametro
                                        stackParametro.Children.Add(cmbTipoMediaMovel);
                                        break;
                                }

                                #endregion Campos especiais para Inteiro
                            }
                            break;

                        //Obs: Usa obrigatoriamento o campo especial
                        case IndicadorTipoParametro.String:
                            //Criando Combo Box
                            ComboBox cmbParamString = new ComboBox();
                            cmbParamString.Margin = new Thickness(10, 13, 0, 0);

                            switch (parametro.CampoEspecial)
                            {
                                case "Ativo":
                                    cmbParamString.Items.Add(grafico.stockChart.Symbol);

                                    //Selecionando o tipo de media movel padrao
                                    cmbParamString.SelectedIndex = 0;
                                    break;

                                case "Série":
                                case "Volume":

                                    List<Series> seriesExistentes = new List<Series>();
                                    foreach (Series obj in grafico.SeriesIndicadoresExistentesPainelIndicadores)
                                    {
                                        seriesExistentes .Add(obj);
                                    }
                                    foreach (Series obj in grafico.SeriesIndicadoresExistentes)
                                    {
                                        bool jaExiste = false;
                                        foreach (Series obj2 in seriesExistentes)
                                        {
                                            if (obj2.Name == obj.Name)
                                                jaExiste = true;
                                        }
                                        if (!jaExiste)
                                            seriesExistentes .Add(obj);
                                    }
                                     
                                    
                                    //Obtendo séries do gráfico
                                    for (int i = 0; i < seriesExistentes.Count; i++)
                                    {
                                        cmbParamString.Items.Add(seriesExistentes[i].FullName);

                                        //Verificando qual deve ser ser o index selecionado
                                        if (parametro.ValorString.ToUpper() == seriesExistentes[i].FullName.ToUpper())
                                            cmbParamString.SelectedIndex = i;
                                    }

                                    cmbParamString.Items.Remove(nomeIndicador);

                                    if ((cmbParamString.SelectedIndex < 0) && (cmbParamString.Items.Count > 0))
                                        cmbParamString.SelectedIndex = 0;

                                    break;
                            }

                            //Adicionando ao stackPanel do parametro
                            stackParametro.Children.Add(cmbParamString);
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion CriaInterfaceParametros()

        #region AtualizaParametros()
        /// <summary>
        /// Atualiza os paramêtros com os novos valores.
        /// </summary>
        private void AtualizaParametros()
        {
            int index = 0;

            foreach (object obj in stpParametros.Children)
            {
                //Percorrendo todos os stack panels
                if (obj is StackPanel)
                {
                    StackPanel stackParametro = (StackPanel)obj;

                    //Percorrendo os campos do parametro
                    foreach (object campoParametro in stackParametro.Children)
                    {
                        switch (parametrosIndicador[index].TipoParametro)
                        {
                            //Só aceita campos NumericaUpDown
                            case IndicadorTipoParametro.Double:
                                if (campoParametro is NumericUpDown)
                                    parametrosIndicador[index].ValorDouble = Convert.ToDouble(((NumericUpDown)campoParametro).Value);
                                break;

                            //Aceita campos NumericaUpDown e Combo box (campo especial)
                            case IndicadorTipoParametro.Inteiro:
                                //NumericUpDown
                                if (parametrosIndicador[index].CampoEspecial == "")
                                {
                                    if (campoParametro is NumericUpDown)
                                        parametrosIndicador[index].ValorInteiro = Convert.ToInt32(((NumericUpDown)campoParametro).Value);
                                }
                                //Combo
                                else
                                {
                                    if (campoParametro is ComboBox)
                                        parametrosIndicador[index].ValorInteiro = ((ComboBox)campoParametro).SelectedIndex;
                                }
                                break;

                            //Só aceita comboBox
                            case IndicadorTipoParametro.String:
                                if (campoParametro is ComboBox)
                                    parametrosIndicador[index].ValorString = ((ComboBox)campoParametro).SelectedItem.ToString();
                                break;
                        }
                    }

                    index++;
                }
            }
        }
        #endregion AtualizaParametros()

        #region ObtemLinhaSelecionada()
        /// <summary>
        /// Retorna a linha selecionada na combo.
        /// </summary>
        /// <returns></returns>
        private EnumGeral.TipoLinha ObtemLinhaSelecionada()
        {
            switch (cmbTipoLinha.SelectedIndex)
            {
                case 0:
                    return EnumGeral.TipoLinha.Pontilhado;

                case 1:
                    return EnumGeral.TipoLinha.TracejadoPontilhado;

                case 2:
                    return EnumGeral.TipoLinha.Tracejado;

                default:
                    return EnumGeral.TipoLinha.Solido;
            }
        }
        #endregion ObtemLinhaSelecionada()

        #endregion Métodos

        public class IndicadorParametroDTO
        {
            private string nomeParametro;
            private string valorString;
            private double valorDouble;
            private int valorInteiro;
            private IndicadorTipoParametro tipoParametro;
            private string campoEspecial;

            public IndicadorParametroDTO()
                : this(string.Empty, string.Empty, 0, 0, IndicadorTipoParametro.Double, "")
            {
            }

            public IndicadorParametroDTO(string nomeParametro, string valor, string campoEspecial)
                : this(nomeParametro, valor, 0, 0, IndicadorTipoParametro.String, campoEspecial)
            {
            }

            public IndicadorParametroDTO(string nomeParametro, int valor, string campoEspecial)
                : this(nomeParametro, "", 0, valor, IndicadorTipoParametro.Inteiro, campoEspecial)
            {
            }

            public IndicadorParametroDTO(string nomeParametro, double valor, string campoEspecial)
                : this(nomeParametro, "", valor, 0, IndicadorTipoParametro.Double, campoEspecial)
            {
            }

            public IndicadorParametroDTO(string nomeParametro, string valorString, double valorDouble, int valorInteiro, IndicadorTipoParametro tipoParametro, string campoEspecial)
            {
                this.nomeParametro = nomeParametro;
                this.valorString = valorString;
                this.valorDouble = valorDouble;
                this.valorInteiro = valorInteiro;
                this.tipoParametro = tipoParametro;
                this.campoEspecial = campoEspecial;
            }

            public string NomeParametro
            {
                get { return nomeParametro; }
                set { nomeParametro = value; }
            }

            public string ValorString
            {
                get { return valorString; }
                set { valorString = value; }
            }

            public int ValorInteiro
            {
                get { return valorInteiro; }
                set { valorInteiro = value; }
            }

            public double ValorDouble
            {
                get { return valorDouble; }
                set { valorDouble = value; }
            }

            public IndicadorTipoParametro TipoParametro
            {
                get { return tipoParametro; }
                set { tipoParametro = value; }
            }

            public string CampoEspecial
            {
                get { return campoEspecial; }
                set { campoEspecial = value; }
            }
        }

        private void rdbPainelOutros_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)rdbPainelOutros.IsChecked)
            {

                if (grafico.PaineisExistentesPainelIndicadores.Count > 0)
                {
                    SelecaoPainel se = new SelecaoPainel(grafico.PaineisExistentesPainelIndicadores, grafico.PaineisExistentes);
                    se.Closing += (sender1, e1) =>
                    {
                        if (se.DialogResult == true)
                        {
                            painel = se.PainelSelecionado;
                            lblPainel.Text = "Painel: " + painel.Title;
                        }
                    };
                    se.Show();
                }
                else
                {
                    MessageBox.Show("Não existem paineis para colocar este indicador.\nSelecione a opção novo painel, painel de preço ou painel de volume.");
                    rdbNovoPainel.IsChecked = true;
                }
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (rdbNovoPainel.IsEnabled)
                rdbNovoPainel.IsChecked = true;
        }

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (rdbPainelPrecos.IsEnabled)
                rdbPainelPrecos.IsChecked = true;
        }

        private void TextBlock_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            if (rdbPainelVolume.IsEnabled)
                rdbPainelVolume.IsChecked = true;
        }

        private void TextBlock_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            if (rdbPainelOutros.IsEnabled)
                rdbPainelOutros.IsChecked = true;
        }

        private void rdbNovoPainel_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbNovoPainel != null)
            if ((bool)rdbNovoPainel.IsChecked)
                lblPainel.Text = "Novo Painel de Indicadores";
        }

        private void rdbPainelPrecos_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbPainelPrecos != null)
            if ((bool)rdbPainelPrecos.IsChecked)
                lblPainel.Text = "Painel de Preços";
        }

        private void rdbPainelVolume_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbPainelVolume != null)
            if ((bool)rdbPainelVolume.IsChecked)
                lblPainel.Text = "Painel de Volume";
        }

        private void ExcluiButton_Click(object sender, RoutedEventArgs e)
        {
            this.ExcluiIndicador = true;
            this.DialogResult = true;
        }

    }
}

