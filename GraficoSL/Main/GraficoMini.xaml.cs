using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Browser;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Reflection;

using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.Indicators;
using Traderdata.Client.Componente.GraficoSL.StockChart;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.Configuracao;
using Traderdata.Client.Componente.GraficoSL.Util;
using Traderdata.Client.Componente.GraficoSL.DTO;
using System.Windows.Media.Imaging;
using System.Text;

namespace Traderdata.Client.Componente.GraficoSL.Main
{
    public partial class GraficoMini : UserControl
    {
        #region Constantes Aprovadas

        Color corVerde = new Color();

        private string marcaDagua = "";
        private int marcaDaguaLeft = 0;
        private int marcaDaguaTop = 0;
        private int marcaDaguaSize = 0;
        private int marcaDaguaWidth = 0;

        private int lastRecord = -1;
        private int lastRecordIndicadores = -1;

        //Constante que controla o numero de barras visiveis inicialmente
        private const int initialVisibleRecords = 300;

        //Constante que controla o numero máximo de indicadores que podem ser aplicados a um grafico
        private const int maxIndicadores = 5;

        //TODO: Pq existe esta verificação se está sempre como true??
        //Constante que contrla se os paineis serão colocado no mesmo tamanho
        private const bool igualPaneisAoInserirIndicador = true;

        #endregion

        #region Campos

        //variavel que controla se isso é um mini grafico
        private bool miniGrafico = false;

        //variavel de controle de link para versao completa
        private string linkVersaoCompleta = "";

        //variavel de controle de template
        public int templateSelecionado = -1;

        //Variavel de controle do tickbox
        public bool TickBoxHabilitado = false;

        //Variavel que armazena o tamanho do painel de preço
        public double AlturaPainelPreco { get; set; }
        public double AlturaPainelVolume { get; set; }

        //variavel que armazena todas as caracteristicas do grafico
        private ConfiguracaoGraficoDTO configuracaoGraficoLocal;

        //Variavel que armazena as caracteristicas basicas para todos os gráficos para este usuario
        private ConfiguracaoPadraoDTO configuracaoPadraoUsuario;

        /// <summary>Lista com as barras do gráfico atual.</summary>
        private List<BarraDTO> listaDados = new List<BarraDTO>();
        private List<BarraRTDTO> listaDadosRT = new List<BarraRTDTO>();

        /// <summary>Lista de indicadores presentes no gráfico.</summary>
        private List<IndicadorDTO> listaIndicadores = new List<IndicadorDTO>();

        //Lista de series auxiliares
        private List<SerieAuxiliarDTO> listaSerieAuxiliar = new List<SerieAuxiliarDTO>();

        //Lista de Objetos
        private List<ObjetoEstudoDTO> listaObjetos = new List<ObjetoEstudoDTO>();

        //Ativo neste grafico
        private string ativo = "";

        //bolsa que pertence o ativo
        private int bolsa = -1;

        //codigo do grafico dto
        private int codigo = -1;

        //Periodo neste grafico
        private Enum.Tupla periodo;

        //Periodicidade neste grafico
        private Enum.Tupla periodicidade;

        //Comentario vinculado ao grafico
        public string Comentario { get; set; }

        // Indicador se o gráfico possui analise compartilhada
        private bool possuiAnaliseCompartilhada;


        #region Delegates e Eventos

        /// <summary>Evento de troca de período.</summary>
        public event OnAlteraPeriodoDelegate OnAlteraPeriodo;
        public delegate void OnAlteraPeriodoDelegate(GraficoMini grafico);

        /// <summary>Evento de troca de volume.</summary>
        public event OnAlteraTipoVolumeDelegate OnAlteraTipoVolume;
        public delegate void OnAlteraTipoVolumeDelegate(GraficoMini grafico);

        /// <summary>Evento de troca de periodicidade.</summary>
        public event OnAlteraPeriodicidadeDelegate OnAlteraPeriodicidade;
        public delegate void OnAlteraPeriodicidadeDelegate(GraficoMini grafico);

        /// <summary>Evento de atualização de dados.</summary>
        public event OnAtualizaDadosDelegate OnAtualizaDados;
        public delegate void OnAtualizaDadosDelegate(GraficoMini grafico);

        /// <summary>Evento de atualização de dados.</summary>
        public event OnHelpDelegate OnHelp;
        public delegate void OnHelpDelegate(GraficoMini grafico);

        /// <summary>Evento de atualização de dados</summary>
        public event OnAtualizaAtivoDelegate OnAtualizaAtivo;
        public delegate void OnAtualizaAtivoDelegate(GraficoMini grafico);

        /// <summary>Evento de aplicação de template</summary>
        public event OnAplicaTemplateDelegate OnAplicaTemplate;
        public delegate void OnAplicaTemplateDelegate(GraficoMini grafico);

        /// <summary>Evento de salvar de template</summary>
        public event OnSalvaTemplateDelegate OnSalvaTemplate;
        public delegate void OnSalvaTemplateDelegate(GraficoMini grafico);

        /// <summary>Evento de verificar se tem analise compartilhada</summary>
        public event OnVerificaAnaliseCompartilhadaDelegate OnVerificaAnaliseCompartilhada;
        public delegate void OnVerificaAnaliseCompartilhadaDelegate(GraficoMini grafico);

        /// <summary>Evento de troca de template</summary>
        public event OnTrocaTemplateDelegate OnTrocaTemplate;
        public delegate void OnTrocaTemplateDelegate(GraficoMini grafico);

        /// <summary>Evento de inserção de ativo</summary>
        public event OnInsereAtivoDelegate OnInsereAtivo;
        public delegate void OnInsereAtivoDelegate(GraficoMini grafico);


        #endregion Delegates e Eventos

        #region Variaveis de controle

        //Variavel de controle para termino do carregamento
        private bool EncerrouCarregamento = false;

        //Controla se o usuário está utilizando o magnetico ou não
        bool usandoMagnetico = false;
        bool usandoLinhaInfinita = false;

        //Linha de estudo que está sendo aplicada
        int linhaEstudoSelecionada = -1;

        //Objeto que esta sendo inserido como magnetico
        LineStudy objetoMagnetico;

        //Controla se tem uma linha sendo inserida
        bool inserindoLinhaEstudo = false;

        //Variaveis de controle de teclas
        private Key modificadorPressionado = Key.None;
        private DateTime modificadorPressionadoHora = new DateTime();

        private System.Windows.Threading.DispatcherTimer timerLinha = new System.Windows.Threading.DispatcherTimer();

        #endregion Variaveis de controle

        #region Dialogs

        //Tela para configuração de um objeto
        private ConfiguraObjeto configuraObjeto;

        //Tela para configuração do fibonacci
        private ConfiguraFibonacciRetracement configuraFibonnaciRetracement;

        //Tela de mensagem padrão
        private MensagemDialog mensagemDialog;

        #endregion

        #region Outras ainda não aprovadas

        private string ConfigFiboRetracements { get; set; }

        #endregion

        #endregion Campos

        #region Constructor

        public GraficoMini()
        {
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="ativo">Ativo com o qual o gráfico será inicializado</param>
        /// <param name="tipoPeriodicidade"></param>
        /// <param name="periodo"></param>
        public GraficoMini(ConfiguracaoPadraoDTO configuracaoPadraoUsuario, ConfiguracaoGraficoDTO configuracaoInicial)
        {
            InitializeComponent();

            //setando a propriedade de minigrafico
            this.miniGrafico = miniGrafico;

            //setando a cor verde
            corVerde.G = 255;
            corVerde.R = 0;
            corVerde.B = 100;
            corVerde.A = 255;

            //Fazendo com que toda a estrutura do stock chart consiga ver esta classe
            //stockChart.GraficoMain = this;
            stockChart.Background = new SolidColorBrush(Colors.Black);
            stockChartIndicadores.Background = new SolidColorBrush(Colors.Black);
            //setando as variaveis inciiais
            this.configuracaoPadraoUsuario = configuracaoPadraoUsuario;
            this.configuracaoGraficoLocal = configuracaoInicial;

            this.stockChart.CrossHairs = true;

            //Assinando eventos
            stockChart.OnTituloIndicador += new StockChartX.TituloIndicadorDelagate(stockChart_OnTituloIndicador);
            stockChart.SeriesDoubleClick += new EventHandler<StockChartX.SeriesDoubleClickEventArgs>(stockChart_SeriesDoubleClick);
            stockChart.IndicatorDoubleClick += new EventHandler<StockChartX.IndicatorDoubleClickEventArgs>(stockChart_IndicatorDoubleClick);
            stockChart.LineStudyDoubleClick += new EventHandler<StockChartX.LineStudyMouseEventArgs>(stockChart_LineStudyDoubleClick);
            stockChart.UserDrawingComplete += new EventHandler<StockChartX.UserDrawingCompleteEventArgs>(stockChart_UserDrawingComplete);
            stockChart.OnIndicadorError += new StockChartX.OnIndicadorErrorDelegate(stockChart_OnIndicadorError);
            stockChart.KeyUp += new KeyEventHandler(stockChart_KeyUp);
            stockChart.MouseLeftButtonUp += new MouseButtonEventHandler(stockChart_MouseLeftButtonUp);
            stockChart.MouseMove += new MouseEventHandler(stockChart_MouseMove);
            stockChartIndicadores.OnTituloIndicador += new StockChartX.TituloIndicadorDelagate(stockChartIndicadores_OnTituloIndicador);
            
            //Propriedades gerais do stockchart
            //stockChart.AppendTickVolumeBehavior = StockChart.DataManager.AppendTickVolumeBehavior.Change;
            stockChart.ChartType = EnumGeral.TipoGraficoEnum.OHLC;
            //stockChart.TickCompressionType = EnumGeral.CompressaoTickEnum.Tempo;

            //Encerrou o carregamento, logo pode alterar o flag
            EncerrouCarregamento = true;

            //populando a lista de templates
            List<TemplateDTO> listaTemplateAux = new List<TemplateDTO>();
            TemplateDTO templateAux = new TemplateDTO();
            templateAux.Id = -1;
            templateAux.Nome = "Selecione um Template";
            listaTemplateAux.Clear();
            listaTemplateAux.Add(templateAux);
            

        }

        

        void stockChart_MouseMove(object sender, MouseEventArgs e)
        {
            StockChartMouseMove(e);
        }

        private void StockChartMouseMove(MouseEventArgs e)
        {
            lastRecord = stockChart.GetReverseX(e.GetPosition(stockChart).X);

            //alterando cross hair
            stockChart.StartTimerCrossHair();
            stockChartIndicadores.StopTimerCrossHair();
            stockChartIndicadores.MoveCrossHairs(stockChartIndicadores.GetXPixel(lastRecord), e.GetPosition(this).Y - 30);

            //if ((bool)btnInfoPanel.IsChecked)
            //{
            //    //alterando info panel
            //    stockChart.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.Fixo;
            //    stockChartIndicadores.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.Fixo;
            //    this.configuracaoGraficoLocal.PainelInfo = true;
            //    stockChart.StopTimerInfoPanel();
            //    stockChartIndicadores.StopTimerInfoPanel();
            //    stockChart.ShowInfoPanelInternal(lastRecord);
            //    stockChartIndicadores.ShowInfoPanelInternal(lastRecord);
            //}
        }

        void Grafico_OnVerificaAnaliseCompartilhada(Grafico grafico)
        {
            MessageBox.Show("Verifica Analise Compartilhada");
            //throw new NotImplementedException();
        }

        void stockChart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (object obj in stockChart.SelectedObjectsCollection)
            {
                //Point p = e.GetPosition(
                string key = ((LineStudy)obj).Key;
                if (stockChart.GetObjectStartRecord(key) <= 0)
                {
                    stockChart.SetObjectPosition(key, 0, Convert.ToDouble(stockChart.GetObjectStartValue(key)),
                        Convert.ToInt32(stockChart.GetObjectEndRecord(key)), Convert.ToDouble(stockChart.GetObjectEndValue(key)));
                }
                if (stockChart.GetObjectEndRecord(key) <= 0)
                {
                    stockChart.SetObjectPosition(key, Convert.ToInt32(stockChart.GetObjectStartRecord(key)), Convert.ToDouble(stockChart.GetObjectStartValue(key)),
                        0, Convert.ToDouble(stockChart.GetObjectEndValue(key)));
                }
            }
            stockChart.Update();
        }



        #endregion

        #region Propriedades

        #region Indicadores

        #region IndicadoresInseridos
        /// <summary>
        /// Retorna os indicadores que estão inseridos no gráfico.
        /// </summary>
        /// <returns></returns>
        public List<Indicator> IndicadoresInseridos
        {
            get
            {
                List<Indicator> indicadoresInseridos = new List<Indicator>();

                //Obtendo Indicadores selecionados
                foreach (object indicador in stockChart.IndicatorsCollection)
                {
                    if (indicador is Indicator)
                    {
                        ((Indicator)indicador).PainelIndicadoresLateral = false;
                        if (((Indicator)indicador)._chartPanel._hasPrice)
                            ((Indicator)indicador).PainelPreco = true;
                        if (((Indicator)indicador)._chartPanel._hasVolume)
                            ((Indicator)indicador).PainelVolume = true;
                        if ((!((Indicator)indicador)._chartPanel._hasVolume) &&
                            (!((Indicator)indicador)._chartPanel._hasPrice))
                            ((Indicator)indicador).PainelIndicadoresAbaixo = true;

                        indicadoresInseridos.Add((Indicator)indicador);
                    }
                }
                return indicadoresInseridos;
            }
        }
        /// <summary>
        /// Retorna os indicadores que estão inseridos no gráfico.
        /// </summary>
        /// <returns></returns>
        public List<Indicator> IndicadoresInseridosPainelIndicadores
        {
            get
            {
                List<Indicator> indicadoresInseridos = new List<Indicator>();

                //Obtendo Indicadores selecionados
                foreach (object indicador in stockChartIndicadores.IndicatorsCollection)
                {
                    if (indicador is Indicator)
                    {
                        ((Indicator)indicador).PainelIndicadoresLateral = true;
                        indicadoresInseridos.Add((Indicator)indicador);
                    }
                }
                return indicadoresInseridos;
            }
        }
        #endregion IndicadoresInseridos

        #region IndicadoresSelecionados
        /// <summary>
        /// Retorna os indicadores que estão selecionados no gráfico.
        /// </summary>
        /// <returns></returns>
        private List<Indicator> IndicadoresSelecionados
        {
            get
            {
                List<Indicator> indicadoresSelecionados = new List<Indicator>();

                //Obtendo Indicadores selecionados
                foreach (object objeto in stockChart.SelectedObjectsCollection)
                {
                    if (objeto is Indicator)
                        indicadoresSelecionados.Add((Indicator)objeto);
                }
                return indicadoresSelecionados;
            }
        }

        /// <summary>
        /// Retorna os indicadores que estão selecionados no gráfico.
        /// </summary>
        /// <returns></returns>
        private List<Indicator> IndicadoresSelecionadosPainelIndicador
        {
            get
            {
                List<Indicator> indicadoresSelecionados = new List<Indicator>();

                //Obtendo Indicadores selecionados
                foreach (object objeto in stockChartIndicadores.SelectedObjectsCollection)
                {
                    if (objeto is Indicator)
                        indicadoresSelecionados.Add((Indicator)objeto);
                }
                return indicadoresSelecionados;
            }
        }
        #endregion IndicadoresSelecionados

        #region IndicadorSelecionado
        /// <summary>
        /// Indicador selecionado. Se não houver um indicador selcionado, o valor desta propriedade será NULL.
        /// </summary>
        private Indicator IndicadorSelecionado
        {
            get
            {
                if (IndicadoresSelecionados.Count > 0)
                    return IndicadoresSelecionados[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Indicador selecionado. Se não houver um indicador selcionado, o valor desta propriedade será NULL.
        /// </summary>
        private Indicator IndicadorSelecionadoPainelIndicadores
        {
            get
            {
                if (IndicadoresSelecionadosPainelIndicador.Count > 0)
                    return IndicadoresSelecionadosPainelIndicador[0];
                else
                    return null;
            }
        }

        #endregion IndicadorSelecionado

        #region IndicadoresPaisInseridos
        /// <summary>
        /// Retorna os indicadores pais que estão inseridos no gráfico.
        /// </summary>
        /// <returns></returns>
        private List<Indicator> IndicadoresPaisInseridos
        {
            get
            {
                List<Indicator> indicadoresInseridos = new List<Indicator>();

                //Obtendo Indicadores selecionados
                foreach (object indicador in stockChart.IndicatorsCollection)
                {
                    Indicator ind = indicador as Indicator;

                    if ((ind != null) && (!ind.IsSerieFilha))
                        indicadoresInseridos.Add(ind);
                }
                return indicadoresInseridos;
            }
        }
        #endregion IndicadoresPaisInseridos

        #endregion

        #region Paineis

        #region PaineisExistentes
        /// <summary>
        /// Painéis existentes no gráfico.
        /// </summary>
        /// <returns></returns>
        public List<ChartPanel> PaineisExistentes
        {
            get
            {
                List<ChartPanel> chartPanels = new List<ChartPanel>();

                //Obtendo paineis existentes
                foreach (object objeto in stockChart.PanelsCollection)
                {
                    if (objeto is ChartPanel)
                        chartPanels.Add((ChartPanel)objeto);
                }

                return chartPanels;
            }
        }

        /// <summary>
        /// Painéis existentes no gráfico.
        /// </summary>
        /// <returns></returns>
        public List<ChartPanel> PaineisExistentesPainelIndicadores
        {
            get
            {
                List<ChartPanel> chartPanels = new List<ChartPanel>();

                //Obtendo paineis existentes
                foreach (object objeto in stockChartIndicadores.PanelsCollection)
                {
                    if (objeto is ChartPanel)
                    {
                        if (((ChartPanel)objeto).Index != 0)
                            chartPanels.Add((ChartPanel)objeto);
                    }
                }

                return chartPanels;
            }
        }
        #endregion IndicadoresSelecionados

        #region PainelPrincipal
        /// <summary>
        /// Painel que contém a série principal.
        /// </summary>
        public ChartPanel PainelPrincipal
        {
            get
            {
                foreach (ChartPanel painel in PaineisExistentes)
                {
                    foreach (Series serie in painel.Series)
                    {
                        if ((serie.FullName.Contains(stockChart.Symbol + ".Ultimo")) || serie.FullName.Contains(".Ultimo"))
                            return painel;
                    }
                }

                return null;
            }
        }
        #endregion PainelPrincipal

        #region PainelVolume
        /// <summary>
        /// Painel que contém o volume.
        /// </summary>
        public ChartPanel PainelVolume
        {
            get
            {
                foreach (ChartPanel painel in PaineisExistentes)
                {
                    foreach (Series serie in painel.Series)
                    {
                        if ((serie.FullName.Contains(stockChart.Symbol + ".Volume")) || serie.FullName.Contains(".Volume"))
                            return painel;
                    }
                }

                return null;
            }
        }
        #endregion PainelVolume

        #endregion

        #region Series

        #region SeriesExistentes
        /// <summary>
        /// Séries existentes no gráfico.
        /// </summary>
        /// <returns></returns>
        public List<Series> SeriesExistentes
        {
            get
            {
                List<Series> series = new List<Series>();

                //Obtendo series existentes
                foreach (object objeto in stockChart.SeriesCollection)
                {
                    if (objeto is Series)
                        series.Add((Series)objeto);
                }

                return series;
            }
        }
        #endregion SeriesExistentes

        #region SeriesIndicadoresExistentes
        /// <summary>
        /// Séries e indicadores existentes no gráfico.
        /// </summary>
        public List<Series> SeriesIndicadoresExistentes
        {
            get
            {
                List<Series> series = new List<Series>();

                //Obtendo series existentes
                foreach (object objeto in stockChart.SeriesCollection)
                {
                    if (objeto is Series)
                        series.Add((Series)objeto);
                }

                //Obtendo Indicadores selecionados
                foreach (object indicador in stockChart.IndicatorsCollection)
                {
                    if (indicador is Indicator)
                        series.Add((Series)indicador);
                }

                return series;
            }
        }

        public List<Series> SeriesIndicadoresExistentesPainelIndicadores
        {
            get
            {
                List<Series> series = new List<Series>();

                //Obtendo series existentes
                foreach (object objeto in stockChartIndicadores.SeriesCollection)
                {
                    if (objeto is Series)
                        series.Add((Series)objeto);
                }

                //Obtendo Indicadores selecionados
                foreach (object indicador in stockChartIndicadores.IndicatorsCollection)
                {
                    if (indicador is Indicator)
                        series.Add((Series)indicador);
                }

                return series;
            }
        }
        #endregion SeriesIndicadoresExistentes

        #endregion

        #region Outros

        

        #endregion

        #region Grafico

        /// <summary>
        /// Metodo deve retornar o objeto Configuracao Grafico local
        /// </summary>
        /// <returns></returns>
        public ConfiguracaoGraficoDTO ConfiguracaoGraficoLocal
        {
            get
            {
                //Recuperar a lista de indicadores presentes no gráfico
                //this.listaIndicadores = this.RetornaIndicadoresPresentesNoGrafico();
                //Recuprar a lista de objetos de estudo presentes no gráfico
                //this.listaObjetos = this.RetornaObjetosPresentesNoGrafico();

                //Retornando a configuração
                //return this.configuracaoGraficoLocal.Clone();
                return this.configuracaoGraficoLocal;

            }
            set
            {
                //Recebendo o valor passado
                this.configuracaoGraficoLocal = value;
            }

        }

        /// <summary>
        /// Propriedade que armazena os indicadores
        /// </summary>
        public List<IndicadorDTO> Indicadores
        {
            get { return listaIndicadores; }
        }

        /// <summary>
        /// Propriedade que armazena os objetos presentes no grafico
        /// </summary>
        public List<ObjetoEstudoDTO> Objetos
        {
            get { return this.listaObjetos; }
        }

        /// <summary>
        /// Propriedade que armazena os dados do gráfico
        /// </summary>
        public List<BarraDTO> Dados
        {
            get { return this.listaDados; }
        }

        /// <summary>
        /// Propriedade que armazena o periodo do grafico
        /// </summary>
        public Tupla Periodo
        {
            get { return this.periodo; }
            set { this.periodo = value; }
        }

        /// <summary>
        /// Propriedade que armazena a periodicidade do gráfico
        /// </summary>
        public Tupla Periodicidade
        {
            get { return this.periodicidade; }
            set { this.periodicidade = value; }
        }

        /// <summary>
        /// Propriedade que armazena o ativo
        /// </summary>
        public string Ativo
        {
            get { return this.ativo; }
            set { this.ativo = value; }
        }

        public int Bolsa
        {
            get { return this.bolsa; }
            set { this.bolsa = value; }
        }

        public int Codigo
        {
            get { return this.codigo; }
            set { this.codigo = value; }
        }

        /// <summary>
        /// Propriedade seta o tipo de periodicidade de acordo com o periodo estabelecido automaticamente
        /// </summary>
        public EnumGeral.TipoPeriodicidade TipoPeriodicidade
        {
            get
            {
                if (Periodo.Value > 30)
                    return EnumGeral.TipoPeriodicidade.Diario;
                else
                    return EnumGeral.TipoPeriodicidade.Intraday;
            }
        }

        /// <summary>
        /// Retorna o numerod e barras visiveis
        /// </summary>
        /// <returns></returns>
        public int GetVisibleRecordCount()
        {
            return stockChart.VisibleRecordCount;
        }

        
        #endregion

        #endregion Propriedades

        #region Eventos

        #region Eventos de Links

        /// <summary>
        /// Evento que é executado após se clicar no link da Traderdata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkTraderData_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.traderdata.com.br/"), "");
        }

        #endregion

        #region Toolbar Horizontal

        #region Configuracao Geral

        /// <summary>
        /// Botão abre uma tela para configurações do gráfico
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigurarGrafico_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ConfiguracaoGeralGrafico();
        }

        #endregion

        #region Indicador

        /// <summary>
        /// Botão inserir Indicador
        /// Insere um indicador no gráfico.
        /// </summary>
        private void InserirIndicador_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //AdicionaNovoIndicadorPainelLateral(cmbIndicadores.SelectedItem.ToString());
        }

        /// <summary>
        /// Evento disparado ao se trocar a combo de indicadores
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbIndicadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (EncerrouCarregamento)
            //    if (cmbIndicadores.SelectedIndex > 0)
            //        AdicionaNovoIndicadorPainelLateral(cmbIndicadores.SelectedItem.ToString());

            //cmbIndicadores.SelectedIndex = 0;
        }

        #endregion

        #region Atualizar

        /// <summary>
        /// Dispara o evento de atualização dos dados externamente
        /// </summary>
        private void btnAtualizar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (OnAtualizaDados != null)
                OnAtualizaDados(this);
        }


        #endregion

        #region Info Panel

        /// <summary>
        /// Botão tela de cotação fixa na tela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInfoPanel_Click(object sender, RoutedEventArgs e)
        {
            //if (btnInfoPanel.IsChecked == true)
            //{
            //    btnInfoPanel.Background = Brushes.Red;
            //    stockChart.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.Fixo;
            //    stockChartIndicadores.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.Fixo;
            //    this.configuracaoGraficoLocal.PainelInfo = true;
            //}
            //else
            //{
            //    btnInfoPanel.Background = Brushes.Blue;
            //    stockChart.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse;
            //    stockChartIndicadores.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse;
            //    this.configuracaoGraficoLocal.PainelInfo = false;
            //}
        }

        #endregion

        #region DarvaBox

        /// <summary>
        /// Evento disparado ao se clicar no botao DarvaBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Darva_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                SetDarvaBox(!stockChart.DarvasBoxes);
                configuracaoGraficoLocal.DarvaBox = stockChart.DarvasBoxes;
            }
            catch (Exception erro)
            {
                string err = erro.ToString();
                mensagemDialog = new MensagemDialog("Ocorreu um erro.\n" + erro);
                mensagemDialog.Show();
            }
        }

        #endregion

        #region Ajuda

        /// <summary>
        /// Metodo que abre o browser apontando para o help do sistema
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ajuda_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.configuracaoGraficoLocal.EstiloBarra = EnumGeral.TipoSeriesEnum.Barra;
            this.RefreshLayout(false);
            this.configuracaoGraficoLocal.EstiloBarra = EnumGeral.TipoSeriesEnum.Linha;
            this.RefreshLayout(false);
            return;


            if (OnHelp != null)
                OnHelp(this);
        }

        #endregion

        #region Seleção de Combos de Periodo e Periodicidade

        /// <summary>
        /// Mudanca de Selecao na Combo de Periodos
        /// Habilita determinadas periodicidades e atualiza o grafico.
        /// </summary>
        private void cmbPeriodo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //tenho que verificar se houve mudança entre intraday e diario 
                //para poder trocar a combo somente nestes casos
                EnumGeral.TipoPeriodicidade tipoPeriodicidadeAnterior = this.TipoPeriodicidade;

                //Setando o novo periodo
                Periodo = (Tupla)e.AddedItems[0];
                if (tipoPeriodicidadeAnterior != this.TipoPeriodicidade)
                {
                    if (this.TipoPeriodicidade == EnumGeral.TipoPeriodicidade.Intraday)
                        this.Periodicidade = configuracaoPadraoUsuario.PeriodicidadeIntraday;
                    else
                        this.Periodicidade = configuracaoPadraoUsuario.PeriodicidadeDiaria;

                    
                }

                //Disparar evento de troca de periodo
                if (OnAlteraPeriodo != null)
                    OnAlteraPeriodo(this);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #region Botões Periodicidade

        private void Periodicidade1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void Periodicidade2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(2);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void Periodicidade3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(3);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void Periodicidade5_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(5);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void Periodicidade10_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(10);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void Periodicidade15_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(15);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void Periodicidade30_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(30);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void Periodicidade1h_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(60);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void Periodicidade2h_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(120);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void PeriodicidadeD_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(1440);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void PeriodicidadeS_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(10080);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void PeriodicidadeM_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlteraPeriodicidade(43200);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion Botões Periodicidade

        private void AlteraPeriodicidade(int periodicidade)
        {
            try
            {
                EnumGeral.TipoPeriodicidade tipoPeriodicidadeAnterior = this.TipoPeriodicidade;

                //1 minuto – 2 dias
                //5 minutos – 5 dias
                //15 minutos – 10 dias
                //30 minutos – 10 dias
                //60 minutos – 30 dias
                //120 minutos – 30 dias

                //Diario – 1 anos
                //Semanal – 3 anos
                //Mensal – 3 anos

                //Alterando a perdiocidade de compressao do grafico
                //Os periodos do TickPeriodicity são em segundos
                switch (periodicidade)
                {
                    case 1:
                        stockChart.TickPeriodicity = 59;
                        this.Periodicidade = EnumPeriodicidade.UmMinuto;
                        Periodo = EnumPeriodo.DoisDias;
                        break;
                    case 2:
                        stockChart.TickPeriodicity = 119;
                        this.Periodicidade = EnumPeriodicidade.DoisMinutos;
                        Periodo = EnumPeriodo.CincoDias;
                        break;
                    case 3:
                        stockChart.TickPeriodicity = 179;
                        this.Periodicidade = EnumPeriodicidade.TresMinutos;
                        Periodo = EnumPeriodo.CincoDias;
                        break;
                    case 5:
                        stockChart.TickPeriodicity = 299;
                        this.Periodicidade = EnumPeriodicidade.CincoMinutos;
                        Periodo = EnumPeriodo.CincoDias;
                        break;
                    case 10:
                        stockChart.TickPeriodicity = 599;
                        this.Periodicidade = EnumPeriodicidade.DezMinutos;
                        Periodo = EnumPeriodo.UmMes;
                        break;
                    case 15:
                        stockChart.TickPeriodicity = 899;
                        this.Periodicidade = EnumPeriodicidade.QuinzeMinutos;
                        Periodo = EnumPeriodo.DezDias;
                        break;
                    case 30:
                        stockChart.TickPeriodicity = 1799;
                        this.Periodicidade = EnumPeriodicidade.TrintaMinutos;
                        Periodo = EnumPeriodo.DezDias;
                        break;
                    case 60:
                        stockChart.TickPeriodicity = 3599;
                        this.Periodicidade = EnumPeriodicidade.UmaHora;
                        Periodo = EnumPeriodo.UmMes;
                        break;
                    case 120:
                        stockChart.TickPeriodicity = 7080;
                        this.Periodicidade = EnumPeriodicidade.DuasHoras;
                        Periodo = EnumPeriodo.UmMes;
                        break;
                    case 1440:
                        stockChart.TickPeriodicity = 86399;
                        this.Periodicidade = EnumPeriodicidade.Diario;
                        Periodo = EnumPeriodo.UmAno;
                        break;
                    case 10080:
                        stockChart.TickPeriodicity = 604799;
                        this.Periodicidade = EnumPeriodicidade.Semanal;
                        Periodo = EnumPeriodo.UmAno;
                        break;
                    case 43200:
                        stockChart.TickPeriodicity = 2591999;
                        this.Periodicidade = EnumPeriodicidade.Mensal;
                        Periodo = EnumPeriodo.TresAnos;
                        break;
                }

                //Carrega as series na fila de barras RT de acordo com a hora/minuto
                lock (listaDadosRT)
                {
                    listaDadosRT.Clear();
                    for (DateTime j = DateTime.Today.Subtract(new TimeSpan(this.Periodo.Value)); j <= DateTime.Today.AddDays(1).Subtract(new TimeSpan(0, 0, 1)); j = j.AddMinutes(this.Periodicidade.Value))
                    {
                        BarraRTDTO barraAux = new BarraRTDTO();
                        barraAux.HoraInicio = j;
                        barraAux.HoraFinal = j.AddMinutes(this.Periodicidade.Value);
                        barraAux.Publicado = false;
                        listaDadosRT.Add(barraAux);
                    }
                }

                //Disparar evento de troca de periodicidade
                if (OnAlteraPeriodicidade != null)
                    OnAlteraPeriodicidade(this);

                //Disparar evento de troca de periodo
                //if (OnAlteraPeriodo != null)
                //    OnAlteraPeriodo(this);

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Mudanca de Selecao na Combo de Periodicidade
        /// Atualiza o gráfico com nova periodicidade.
        /// </summary>
        //private void cmbPeriodicidade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {                

        //        //Setando o objeto grafico com a periodicidade selecionada
        //        this.Periodicidade = (Tupla)e.AddedItems[0];

        //        //Alterando a perdiocidade de compressao do grafico
        //        //Os periodos do TickPeriodicity são em segundos
        //        switch (this.Periodicidade.Value)
        //        {
        //            case 1:
        //                stockChart.TickPeriodicity = 59;
        //                break;
        //            case 5:
        //                stockChart.TickPeriodicity = 259;
        //                break;
        //            case 15:
        //                stockChart.TickPeriodicity = 899;
        //                break;
        //            case 30:
        //                stockChart.TickPeriodicity = 1799;
        //                break;
        //            case 60:
        //                stockChart.TickPeriodicity = 3599;
        //                break;
        //            case 1440:
        //                stockChart.TickPeriodicity = 86399;
        //                break;
        //            case 10080:
        //                stockChart.TickPeriodicity = 604799;
        //                break;
        //            case 43200:
        //                stockChart.TickPeriodicity = 2591999;
        //                break;
        //        }

        //        //Carrega as series na fila de barras RT de acordo com a hora/minuto
        //        lock (listaDadosRT)
        //        {
        //            listaDadosRT.Clear();
        //            for (DateTime j = DateTime.Today.Subtract(new TimeSpan(this.Periodo.Value)); j <= DateTime.Today.AddDays(1).Subtract(new TimeSpan(0, 0, 1)); j = j.AddMinutes(this.Periodicidade.Value))
        //            {
        //                BarraRTDTO barraAux = new BarraRTDTO();
        //                barraAux.HoraInicio = j;
        //                barraAux.HoraFinal = j.AddMinutes(this.Periodicidade.Value);
        //                barraAux.Publicado = false;
        //                listaDadosRT.Add(barraAux);
        //            }
        //        }

        //        //Disparar evento de troca de periodicidade
        //        if (OnAlteraPeriodicidade != null)
        //            OnAlteraPeriodicidade(this);

        //    }
        //    catch (Exception exc)
        //    {
        //        throw exc;
        //    }
        //}

        #endregion

        #region Label de Ativo

        /// <summary>
        /// Clique no Label Ativo
        /// Dispara um evento no form pai para que ele faça a substituição do ativo
        /// </summary>
        private void lblAtivo_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (OnAtualizaAtivo != null)
                OnAtualizaAtivo(this);
        }

        #endregion

        #region Salvar


        /// <summary>
        /// Evento disparado ao se clicar no botão Salvar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSalvar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                //byte[] imagemPrincipal = stockChart.SaveFile(StockChartX.SerializationTypeEnum.All);
                //byte[] painelIndiacadores = stockChartIndicadores.SaveFile(StockChartX.SerializationTypeEnum.All);

                ////Criando os streams
                //MemoryStream painelPrincipalStream = new MemoryStream(imagemPrincipal);
                //MemoryStream painelIndicadoresStream = new MemoryStream(painelIndiacadores);

                ////Criando os bitmaps
                //BitmapImage painelPrincipalBmp = new BitmapImage();
                //BitmapImage painelIndicadoresBmp = new BitmapImage();

                ////Setando os sources
                //painelPrincipalBmp.SetSource(painelPrincipalStream);
                //painelIndicadoresBmp.SetSource(painelIndicadoresStream);

                ////Criando a imagem conjunta
                //WriteableBitmap wb =
                //    new WriteableBitmap(painelPrincipalBmp.PixelWidth + painelIndicadoresBmp.PixelWidth,
                //painelPrincipalBmp.PixelHeight);

                //Image image = new Image();
                //image.Source = wb;

                ////Transform ta;
                //TranslateTransform tb = new TranslateTransform();
                //tb.Y = 0;
                //tb.X = painelPrincipalBmp.PixelWidth;

                //wb.Render(stockChart,null);
                //wb.Render(stockChartIndicadores, tb);

                //SaveFileDialog a = new SaveFileDialog();

                //if ((bool)a.ShowDialog() == true)
                //{

                //}
                stockChart.SaveToFileComplete(stockChart, stockChartIndicadores, StockChartX.ImageExportType.Png);


            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #endregion

        #region ToolBar Vertical

        #region Botões toolbar vertical

        //<summary>
        //Clique em um item da ToolBar Vertical
        //Realiza uma ação de estudo.
        //</summary>
        private void botoesToolBarVertical_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Antes de inserir outro objeto, devo procurar para ver se o ultimo fora inserido
                stockChart.ResetaStatus();
                stockChartIndicadores.ResetaStatus();


                if (this != null)
                {

                    ToggleButton itemSelecionado = ((System.Windows.Controls.Primitives.ToggleButton)sender);

                    if (itemSelecionado.Tag != null)
                    {
                        int itemEstudo = -1;
                        itemSelecionado.Background = new SolidColorBrush(Colors.Red);

                        //Obtendo item
                        Int32.TryParse(itemSelecionado.Tag.ToString(), out itemEstudo);

                        //Desmarcando botao
                        itemSelecionado.IsChecked = false;

                        switch (itemEstudo)
                        {
                            
                            //Reseta Zoom
                            case 3:
                                this.stockChart.ResetZoom();
                                this.stockChart.ResetYScale(0);
                                btnResetZoom.Background = new SolidColorBrush(Colors.Gray);
                                break;

                            //Quadrado
                            case 4:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.Rectangle, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Elipse
                            case 5:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.Ellipse, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Fibo Arcs
                            case 6:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.FibonacciArcs, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Fibo Retracements
                            case 7:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.FibonacciRetracements, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Fibo Fan
                            case 8:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.FibonacciFan, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Fibo TimeZone
                            case 9:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.FibonacciTimeZones, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Error Channel
                            case 10:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.ErrorChannel, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Gann Fan
                            case 11:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.GannFan, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Quadrant Lines
                            case 12:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.QuadrantLines, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //RaffRegression
                            case 13:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.RaffRegression, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Speed Lines
                            case 14:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.SpeedLines, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //TironeLevels
                            case 15:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.TironeLevels, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Linha Horizontal
                            case 16:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.HorizontalLine, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Linha Vertical
                            case 17:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.VerticalLine, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Linha Tendencia
                            case 18:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.TrendLine, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Zoom de area
                            case 19:
                                this.ZoomArea();
                                break;

                            //Linha de estudo Texto
                            case 20:
                                this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.StaticText, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            //Habilitando magnetico para linhas de estudo
                            
                            //Usando Regua
                            case 22:
                                AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.Regua, configuracaoGraficoLocal.TipoLinhaDefault,
                                    configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                                break;

                            default:
                                break;
                        }
                    }
                }

            }
            catch (Exception erro)
            {
                string err = erro.ToString();
                mensagemDialog = new MensagemDialog("Ocorreu um erro.\n" + erro);
                mensagemDialog.Show();
            }
        }

        #endregion


        #endregion

        #region StockChart Painel Indicadores
               
        /// <summary>
        /// User Drawing Complete
        /// Finaliza o objeto retângulo que define o zoom de área.
        /// </summary>
        private void stockChartIndicadores_UserDrawingComplete(object sender, StockChartX.UserDrawingCompleteEventArgs e)
        {
            //Antes de inserir outro objeto, devo procurar para ver se o ultimo fora inserido
            stockChart.ResetaStatus();
            stockChartIndicadores.ResetaStatus();

            //Zoom de area
            if (e.Key == "ZoomArea")
            {
                //Obtendo record inicial e final
                stockChartIndicadores.FirstVisibleRecord = Convert.ToInt32(stockChartIndicadores.GetObjectStartRecord(e.Key));
                stockChartIndicadores.LastVisibleRecord = Convert.ToInt32(stockChartIndicadores.GetObjectEndRecord(e.Key));

                //Excluindo objeto
                stockChartIndicadores.RemoveObject("ZoomArea");
            }

            DesmarcaItensToolbarVertical();

            //Refresh na lista de objetos
            this.listaObjetos = this.RetornaObjetosPresentesNoGrafico();
        }

        /// <summary>
        /// Duplo Clique na Linha de Estudo
        /// Abre a tela de configuração da linha de estudo selecionada.
        /// </summary>
        private void stockChartIndicadores_LineStudyDoubleClick(object sender, StockChartX.LineStudyMouseEventArgs e)
        {
            ConfigurarObjetoSelecionadoPainelIndicador();
        }

        /// <summary>
        /// Clique no título do indicador
        /// Abre a tela de edicao do indicador.
        /// </summary>        
        void stockChartIndicadores_OnTituloIndicador(Series serie)
        {
            ConfiguraIndicador((Indicator)serie, true);
        }

        /// <summary>
        /// Duplo Clique no Indicador
        /// Abre a tela de configuração do indicador.
        /// </summary>
        private void stockChartIndicador_IndicatorDoubleClick(object sender, StockChartX.IndicatorDoubleClickEventArgs e)
        {
            ConfigurarIndicadorSelecionadoPainelIndicadores();
        }

        #endregion

        #region Stockhart Painel de Preços

        #region OnIndicadorError
        /// <summary>
        /// Evento disparado quando um erro ocorre ao gerar o indicador.
        /// </summary>
        /// <param name="erro"></param>
        /// <param name="tipoErro"></param>
        private void stockChart_OnIndicadorError(string erro, Indicator.ErroIndicador tipoErro)
        {
            MessageBox.Show(erro, "Aviso: Indicador", MessageBoxButton.OK);
        }
        #endregion OnIndicadorError

        #region Key Events


        /// <summary>
        /// Evento disparado quando uma tecla, clicada, é liberada no grafico.
        /// </summary>
        private void stockChart_KeyUp(object sender, KeyEventArgs e)
        {
            if (modificadorPressionado == e.Key)
                modificadorPressionado = Key.None;

            stockChart.Focus();
        }

        #endregion

        #region Indicador

        /// <summary>
        /// Clique no título do indicador
        /// Abre a tela de edicao do indicador.
        /// </summary>
        private void stockChart_OnTituloIndicador(Series serie)
        {
            ConfiguraIndicador((Indicator)serie, false);
        }

        /// <summary>
        /// Duplo Clique no Indicador
        /// Abre a tela de configuração do indicador.
        /// </summary>
        private void stockChart_IndicatorDoubleClick(object sender, StockChartX.IndicatorDoubleClickEventArgs e)
        {
            ConfigurarIndicadorSelecionado();
        }

        #endregion

        #region Series

        /// <summary>
        /// Duplo Clique na Série
        /// Abre a tela de configuração da serie.
        /// </summary>
        private void stockChart_SeriesDoubleClick(object sender, StockChartX.SeriesDoubleClickEventArgs e)
        {
            if ((e.Series.SeriesType != EnumGeral.TipoSeriesEnum.Indicador) && (e.Series.Name != stockChart.Symbol) && (!e.Series.IsSerieFilha))
            {
                ConfiguraSerie serieConfig = new ConfiguraSerie(e.Series.UpColor, e.Series.DownColor, e.Series.StrokeThickness, e.Series.StrokePattern);
                serieConfig.Closing += (sender1, e1) =>
                {
                    if (serieConfig.DialogResult == true)
                    {
                        e.Series.UpColor = serieConfig.CorAltaSerie;
                        e.Series.DownColor = serieConfig.CorBaixaSerie;
                        e.Series.StrokeThickness = serieConfig.GrossuraSerie;
                        e.Series.StrokePattern = serieConfig.TipoLinhaSerie;

                        stockChart.Update();
                    }
                };
                serieConfig.Show();
            }
        }


        #endregion

        #region Objetos/Linhas de Estudo

        /// <summary>
        /// User Drawing Complete
        /// Finaliza o objeto retângulo que define o zoom de área.
        /// </summary>
        private void stockChart_UserDrawingComplete(object sender, StockChartX.UserDrawingCompleteEventArgs e)
        {
            //Antes de inserir outro objeto, devo procurar para ver se o ultimo fora inserido
            stockChart.ResetaStatus();
            stockChartIndicadores.ResetaStatus();

            if (stockChart.GetObjectStartRecord(e.Key) <= 0)
            {
                stockChart.SetObjectPosition(e.Key, 0, Convert.ToDouble(stockChart.GetObjectStartValue(e.Key)),
                    Convert.ToInt32(stockChart.GetObjectEndRecord(e.Key)), Convert.ToDouble(stockChart.GetObjectEndValue(e.Key)));
            }
            if (stockChart.GetObjectEndRecord(e.Key) <= 0)
            {
                stockChart.SetObjectPosition(e.Key, Convert.ToInt32(stockChart.GetObjectStartRecord(e.Key)), Convert.ToDouble(stockChart.GetObjectStartValue(e.Key)),
                    0, Convert.ToDouble(stockChart.GetObjectEndValue(e.Key)));
            }

            //Zoom de area
            if (e.Key == "ZoomArea")
            {
                //Obtendo record inicial e final
                stockChart.FirstVisibleRecord = Convert.ToInt32(stockChart.GetObjectStartRecord(e.Key));
                stockChart.LastVisibleRecord = Convert.ToInt32(stockChart.GetObjectEndRecord(e.Key));

                //Excluindo objeto
                stockChart.RemoveObject("ZoomArea");
            }

            //Ações realizadas após inserir uma linha de estudo
            if ((e.StudyType != LineStudy.StudyTypeEnum.Unknown) && (inserindoLinhaEstudo))
            {
                //Setando linha infinita por padrao
                if ((e.StudyType == LineStudy.StudyTypeEnum.TrendLine) && (configuracaoGraficoLocal.UsarLinhaInfinita))
                {

                    LineStudy linha = stockChart.GetLineStudy(e.Key);

                    if (linha != null)
                        linha.LinhaInfinitaADireita = true;

                    stockChart.Update();
                }

                if (objetoMagnetico != null)
                {
                    bool painelPrincipal = false;
                    LineStudy linhaEstudo = null;
                    inserindoLinhaEstudo = false;


                    //Verificando se a linha de estudo está sendo inserida fora do painel principal, para tirar o magnetico
                    foreach (LineStudy obj in EstudosInseridos)
                    {
                        if (obj.Key == e.Key)
                        {
                            linhaEstudo = obj;

                            foreach (object serie in obj._chartPanel.SeriesCollection)
                            {
                                Series serieAux = serie as Series;

                                if ((serieAux != null) && (serieAux.FullName.Contains(stockChart.Symbol + ".Ultimo")))
                                {
                                    painelPrincipal = true;
                                    break;
                                }
                            }

                            break;
                        }
                    }

                    if (!painelPrincipal)
                    {
                        if (linhaEstudo != null)
                            linhaEstudo.LinhaMagnetica = false;
                    }
                }
            }

            DesmarcaItensToolbarVertical();

            //Refresh na lista de objetos
            this.listaObjetos = this.RetornaObjetosPresentesNoGrafico();

        }


        /// <summary>
        /// Metodo que deve ser executado para se atualizar a lista de objetos presentes no gráfico
        /// </summary>
        public void AtualizaObjetos()
        {
            //Atualizando a lista de objetos
            this.listaObjetos = this.RetornaObjetosPresentesNoGrafico();
        }

        /// <summary>
        /// Metodo que deve ser executado para se atualizar a lista de objetos presentes no gráfico
        /// </summary>
        public void AtualizaIndicadores()
        {
            //Atualizando a lista de objetos
            this.listaIndicadores = this.RetornaIndicadoresPresentesNoGrafico();
        }

        /// <summary>
        /// Duplo Clique na Linha de Estudo
        /// Abre a tela de configuração da linha de estudo selecionada.
        /// </summary>
        private void stockChart_LineStudyDoubleClick(object sender, StockChartX.LineStudyMouseEventArgs e)
        {
            ConfigurarObjetoSelecionado();
        }

        #endregion

        #endregion

        #endregion Eventos

        #region Metodos

        #region Posicionamento de Objeto usando Data

        #region AjustaPosicaoObjeto()
        /// <summary>
        /// Ajusta a posição do objeto, baseada em sua data de insercao.
        /// </summary>
        /// <param name="objeto"></param>
        /// <param name="dataOriginalInicial"></param>
        /// <param name="dataOriginalFinal"></param>
        /// <param name="valorInicial"></param>
        /// <param name="valorFinal"></param>
        /// <param name="periodicidade"></param>
        public void AjustaPosicaoObjeto(LineStudy objeto, DateTime dataOriginalInicial, DateTime dataOriginalFinal, double valorInicial,
                                        double valorFinal, int periodicidade)
        {
            try
            {
                if ((Dados != null) && (Dados.Count > 0))
                {
                    //Atenção: Nunca mudar as datas originais do objeto
                    int recordInicial = -1;
                    int recordFinal = -1;
                    bool recordInicialObtido = false;
                    bool recordFinalObtido = false;

                    #region Calculando nas Barras Passadas

                    if (dataOriginalInicial < Dados[0].TradeDate)
                    {
                        recordInicialObtido = true;
                        recordInicial = Convert.ToInt32(Dados[0].TradeDate.Subtract(dataOriginalInicial).TotalDays) * (-1);
                    }

                    if (dataOriginalFinal < Dados[0].TradeDate)
                    {
                        recordFinalObtido = true;
                        recordFinal = Convert.ToInt32(Dados[0].TradeDate.Subtract(dataOriginalFinal).TotalDays) * (-1);
                    }

                    #endregion Calculando nas Barras Passadas

                    #region Calculando nas Barras Existentes

                    //Percorrendo barras obter records de posicionamento do objeto
                    for (int index = 0; index < Dados.Count; index++)
                    {
                        //Se os records já foram obtidos, posso parar a iteracao
                        if ((recordInicialObtido) && (recordFinalObtido))
                            break;

                        DateTime dataAtual = Dados[index].TradeDate;
                        DateTime? dataAnterior = null;
                        DateTime? dataPosterior = null;

                        //Obtendo data posterior, se existir
                        if (index + 1 < Dados.Count)
                            dataPosterior = Dados[index + 1].TradeDate;

                        //Obtendo data anterior, se existir
                        if (index - 1 >= 0)
                            dataAnterior = Dados[index - 1].TradeDate;

                        //Ajustando o record em que o objeto estará nesse periodo/periocidade
                        AtualizaPosicionamentoAuxiliar(dataOriginalInicial, dataOriginalFinal, dataAtual,
                                                        dataAnterior, dataPosterior,
                                                        ref recordInicial, ref recordFinal,
                                                        ref recordInicialObtido, ref recordFinalObtido, index);
                    }

                    #endregion Calculando nas Barras Existentes

                    #region Calculando nas Barras Futuras

                    //Verificando se um dos records não foi definido
                    if ((!recordInicialObtido) || (!recordFinalObtido))
                    {
                        //Obtendo a ultima hora
                        DateTime dataAtual = Dados[Dados.Count - 1].TradeDate;

                        //Simulando barras futuras
                        for (int index = 0; index < 100000000; index++)
                        {
                            //Se os records já foram obtidos, posso parar a iteracao
                            if ((recordInicialObtido) && (recordFinalObtido))
                                break;

                            dataAtual = dataAtual.AddMinutes(periodicidade);
                            DateTime? dataAnterior = null;
                            DateTime? dataPosterior = null;

                            //Simulando data posterior
                            if (index + 1 < Dados.Count)
                                dataPosterior = dataAtual.AddMinutes(periodicidade);

                            //Simulando data anterior
                            if (index - 1 >= 0)
                                dataAnterior = dataAtual.Subtract(new TimeSpan(0, periodicidade, 0));

                            //Ajustando o record em que o objeto estará nesse periodo/periocidade
                            AtualizaPosicionamentoAuxiliar(dataOriginalInicial, dataOriginalFinal, dataAtual,
                                                            dataAnterior, dataPosterior,
                                                            ref recordInicial, ref recordFinal,
                                                            ref recordInicialObtido, ref recordFinalObtido, stockChart.RecordCount + index);
                        }
                    }

                    #endregion Calculando nas Barras Futuras

                    //Setando coordenadas
                    objeto.SetaValores(recordInicial, valorInicial, recordFinal, valorFinal);
                }
                //Se não tiver dado, devo me desfazer do item
                else
                {
                    if (objeto.PainelIndicador)
                        stockChartIndicadores.RemoveObject(objeto);
                    else
                        stockChart.RemoveObject(objeto);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion AjustaPosicaoObjeto()

        #region AtualizaPosicionamentoAuxiliar()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataOriginalInicial"></param>
        /// <param name="dataOriginalFinal"></param>
        /// <param name="dataAtual"></param>
        /// <param name="dataAnterior"></param>
        /// <param name="dataPosterior"></param>
        /// <param name="recordInicial"></param>
        /// <param name="recordFinal"></param>
        /// <param name="recordInicialObtido"></param>
        /// <param name="recordFinalObtido"></param>
        /// <param name="recordAtual"></param>
        private void AtualizaPosicionamentoAuxiliar(DateTime dataOriginalInicial, DateTime dataOriginalFinal, DateTime dataAtual,
                                                    DateTime? dataAnterior, DateTime? dataPosterior, ref int recordInicial,
                                                    ref int recordFinal, ref bool recordInicialObtido, ref bool recordFinalObtido, int recordAtual)
        {
            //Arredondando para record inicial mais próximo a data do objeto
            if ((dataAtual >= dataOriginalInicial) && (!recordInicialObtido))
            {
                recordInicialObtido = true;

                //Este codigo realiza a aproximacao da barra, n deletar por hora
                ////Se houver data anterior
                //if (dataAnterior != null)
                //{
                //    //Verificando, por segurança, se a data anterior não é mais próxima da data original inicial
                //    if (Math.Abs(dataOriginalInicial.Subtract((DateTime)dataAnterior).TotalMinutes) < Math.Abs(dataAtual.Subtract(dataOriginalInicial).TotalMinutes))
                //    {
                //        recordInicial = recordAtual - 1;
                //        return;
                //    }
                //}

                recordInicial = recordAtual;
            }

            //Arredondando para record inicial mais próximo a data do objeto
            if ((dataAtual >= dataOriginalFinal) && (!recordFinalObtido))
            {
                recordFinalObtido = true;

                //Este codigo realiza a aproximacao da barra, n deletar por hora
                ////Se houver data posterior, devo verificar se é mais proxima
                //if (dataPosterior != null)
                //{
                //    //Verificando, por segurança, se a data posterior não é mais próxima da data original inicial
                //    if (Math.Abs(dataOriginalFinal.Subtract((DateTime)dataPosterior).TotalMinutes) < Math.Abs(dataAtual.Subtract(dataOriginalFinal).TotalMinutes))
                //    {
                //        recordFinal = recordAtual + 1;
                //        return;
                //    }
                //}

                recordFinal = recordAtual;
            }
        }
        #endregion AtualizaPosicionamentoAuxiliar()

        #endregion Posicionamento de Objeto usando Data

        #region Operações no gráfico

        #region Marca Dagua

        /// <summary>
        /// MEtodo que seta a marca dagura que ser apresentada no grafico
        /// </summary>
        /// <param name="marcadagua"></param>
        public void SetMarcaDagua(string texto, int left, int top, int size, int width)
        {
            this.marcaDagua = texto;
            this.marcaDaguaLeft = left;
            this.marcaDaguaTop = top;
            this.marcaDaguaSize = size;
            this.marcaDaguaWidth = width;
        }

        #endregion

        #region Objetos/Linhas de Estudo

        #region AdicionaLinhaEstudo
        /// <summary>
        /// Adiciona um objeto de estudo no gráfico.
        /// </summary>
        /// <param name="tipoEstudo">Tipo de estudo que será inserido.</param>
        /// <param name="EnumGeral.tipoLinha">Tipo da linha do estudo.</param>
        /// <param name="corEstudo">Cor do estudo.</param>
        /// <param name="texto">Utilizado apenas para objetos tipo texto.</param>
        public void AdicionaLinhaEstudo(LineStudy.StudyTypeEnum tipoEstudo, EnumGeral.TipoLinha TipoLinha, Brush corEstudo, double grossuraObjeto, string texto)
        {
            try
            {



                inserindoLinhaEstudo = true;

                object[] args = new object[0];
                string nomeEstudo = tipoEstudo.ToString();
                int qtdEstudosTipo = stockChart.GetLineStudyCountByType(tipoEstudo);
                LineStudy lineStudy = null;
                LineStudy lineStudyIndicador = null;

                //Setando parametros se necessario
                switch (tipoEstudo)
                {
                    case LineStudy.StudyTypeEnum.StaticText:
                        args = new object[] { texto };
                        grossuraObjeto = 12; //for text objects is FontSize
                        break;

                    case LineStudy.StudyTypeEnum.VerticalLine:
                        //when first parameter is false, vertical line will display DataTime instead on record number
                        args = new object[]
                     {
                         
                       false, //true - mostra o record, false - mostra a data
                       true, //true - mostra texto com linha, false - mostra apenas linha
                       "g", //custom datetime format, when args[0] == false. See MSDN:DateTime.ToString(string) for legal values
                     };
                        break;

                    default:
                        break;
                }

                //Validando um nome para o estudo
                if (qtdEstudosTipo > 0)
                    nomeEstudo += qtdEstudosTipo;

                //Adicionando estudo
                lineStudy = stockChart.AddLineStudy(tipoEstudo, nomeEstudo, corEstudo, args);
                lineStudyIndicador = stockChartIndicadores.AddLineStudy(tipoEstudo, nomeEstudo, corEstudo, args);


                //Nem todas as linhas devem ser magneticas
                switch (lineStudy.StudyType)
                {
                    case LineStudy.StudyTypeEnum.ErrorChannel:
                    case LineStudy.StudyTypeEnum.HorizontalLine:
                    case LineStudy.StudyTypeEnum.QuadrantLines:
                    case LineStudy.StudyTypeEnum.RaffRegression:
                    case LineStudy.StudyTypeEnum.TironeLevels:
                    case LineStudy.StudyTypeEnum.VerticalLine:
                    case LineStudy.StudyTypeEnum.Observacao:
                        lineStudy.LinhaMagnetica = false;
                        break;

                    default:
                        lineStudy.LinhaMagnetica = usandoMagnetico;
                        break;
                }

                objetoMagnetico = lineStudy;

                //Setando grossura
                lineStudy.StrokeThickness = grossuraObjeto;

                //Tipo de linha
                lineStudy.StrokeType = TipoLinha;

                if ((lineStudy.GetType() == typeof(FibonacciRetracements)) && (ConfigFiboRetracements != ""))
                    lineStudy.SetaParametros(ConfigFiboRetracements, ';');

                //Se a linha de estudo é um objeto de texto, podemos alterar o texto diretamente
                if (lineStudy.GetType() == typeof(StaticText))
                {
                    List<LineStudy> estudosSelecionados = EstudosSelecionados;
                    ConfiguraTexto configTxt = new ConfiguraTexto(((StaticText)lineStudy).Text, lineStudy.StrokeThickness, lineStudy.Stroke);

                    configTxt.Closing += (sender1, e1) =>
                    {
                        if (configTxt.DialogResult == true)
                        {
                            lineStudy.StrokeThickness = configTxt.TamanhoFonte;
                            ((StaticText)lineStudy).Text = configTxt.Texto;
                            lineStudy.Stroke = configTxt.Cor;
                            stockChart.Update();
                        }
                    };

                    //Mostrando o form
                    configTxt.Show();
                }


            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
        #endregion AdicionaLinhaEstudo

        #region EstudosInseridos
        /// <summary>
        /// Estudos inseridos no gráfico.
        /// </summary>
        /// <returns></returns>
        public List<LineStudy> EstudosInseridos
        {
            get
            {
                List<LineStudy> estudosInseridos = new List<LineStudy>();

                //Obtendo Indicadores selecionados
                foreach (object objeto in stockChart.LineStudiesCollection)
                {
                    if (objeto is LineStudy)
                    {
                        ((LineStudy)objeto).PainelIndicador = false;
                        estudosInseridos.Add((LineStudy)objeto);
                    }
                }

                return estudosInseridos;
            }
        }

        /// <summary>
        /// Estudos inseridos no gráfico.
        /// </summary>
        /// <returns></returns>
        public List<LineStudy> EstudosInseridosPainelIndicador
        {
            get
            {
                List<LineStudy> estudosInseridos = new List<LineStudy>();

                //Obtendo Indicadores selecionados
                foreach (object objeto in stockChartIndicadores.LineStudiesCollection)
                {
                    if (objeto is LineStudy)
                    {
                        ((LineStudy)objeto).PainelIndicador = true;
                        estudosInseridos.Add((LineStudy)objeto);
                    }
                }

                return estudosInseridos;
            }
        }
        #endregion EstudosInseridos

        #region EstudosSelecionados
        /// <summary>
        /// Estudos selecionados no gráfico.
        /// </summary>
        /// <returns></returns>
        public List<LineStudy> EstudosSelecionados
        {
            get
            {
                List<LineStudy> estudosSelecionados = new List<LineStudy>();

                //Obtendo Indicadores selecionados
                foreach (object objeto in stockChart.SelectedObjectsCollection)
                {
                    if (objeto is LineStudy)
                        estudosSelecionados.Add((LineStudy)objeto);
                }

                return estudosSelecionados;
            }
        }

        /// <summary>
        /// Estudos selecionados no painel de indicador.
        /// </summary>
        /// <returns></returns>
        public List<LineStudy> EstudosSelecionadosPainelIndicador
        {
            get
            {
                List<LineStudy> estudosSelecionados = new List<LineStudy>();

                //Obtendo Indicadores selecionados
                foreach (object objeto in stockChartIndicadores.SelectedObjectsCollection)
                {
                    if (objeto is LineStudy)
                        estudosSelecionados.Add((LineStudy)objeto);
                }

                return estudosSelecionados;
            }
        }

        #endregion EstudosSelecionados

        #region ExcluiObjetoSelecionado()
        /// <summary>
        /// Exclui o objeto selecionado.
        /// </summary>
        public void ExcluiObjetoSelecionado()
        {
            try
            {
                List<LineStudy> estudosSelecionados = EstudosSelecionados;

                if (estudosSelecionados.Count > 0)
                    stockChart.RemoveObject(estudosSelecionados[0].Key);

                this.listaObjetos = this.RetornaObjetosPresentesNoGrafico();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Exclui o objeto selecionado.
        /// </summary>
        public void ExcluiObjetoSelecionadoPainelIndicador()
        {
            try
            {
                List<LineStudy> estudosSelecionados = EstudosSelecionadosPainelIndicador;

                if (estudosSelecionados.Count > 0)
                    stockChartIndicadores.RemoveObject(estudosSelecionados[0].Key);

                this.listaObjetos = this.RetornaObjetosPresentesNoGrafico();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion ExcluiTodosObjetos()

        #region ConfigurarObjetoSelecionado()
        /// <summary>
        /// Carrega o form de edição para o objeto selecionado.
        /// </summary>
        public void ConfigurarObjetoSelecionado()
        {
            try
            {
                List<LineStudy> estudosSelecionados = EstudosSelecionados;

                if (estudosSelecionados.Count > 0)
                {
                    LineStudy estudo = estudosSelecionados[0];

                    switch (estudo.StudyType)
                    {
                        //Se for selecionado um objeto Observacao, não deverá fazer nada
                        case LineStudy.StudyTypeEnum.Observacao:
                            break;

                        //Se for selecionado um objeto texto
                        case LineStudy.StudyTypeEnum.StaticText:
                            ConfiguraTexto configTxt = new ConfiguraTexto(((StaticText)estudo).Text, estudo.StrokeThickness, estudo.Stroke);

                            configTxt.Closing += (sender1, e1) =>
                            {
                                if (configTxt.DialogResult == true)
                                {
                                    estudo.StrokeThickness = configTxt.TamanhoFonte;
                                    ((StaticText)estudosSelecionados[0]).Text = configTxt.Texto;
                                    estudo.Stroke = configTxt.Cor;
                                    stockChart.Update();
                                }
                            };

                            //Mostrando o form
                            configTxt.Show();
                            break;

                        //Se for selecionado um objeto ErrorChannel
                        case LineStudy.StudyTypeEnum.ErrorChannel:

                            ConfiguraErrorChannel errorChannConfig = new ConfiguraErrorChannel(((ErrorChannel)estudo).ObtemParametro(), estudo.Stroke, estudo.StrokeThickness);

                            errorChannConfig.Closing += (sender1, e1) =>
                            {
                                if (errorChannConfig.DialogResult == true)
                                {
                                    ((ErrorChannel)estudo).SetaParametros(errorChannConfig.Valor);
                                    estudo.Stroke = errorChannConfig.Cor;
                                    estudo.StrokeThickness = errorChannConfig.Espessura;
                                }
                            };
                            errorChannConfig.Show();

                            break;

                        //Se for selecionado um Fibonacci Retracements
                        case LineStudy.StudyTypeEnum.FibonacciRetracements:

                            ConfiguraFibonacciRetracement fiboRetracemConfig = new ConfiguraFibonacciRetracement(((FibonacciRetracements)estudo).ObtemParametro(), estudo.StrokeThickness, estudo.StrokeType, estudo.Stroke, estudo.LinhaMagnetica);

                            fiboRetracemConfig.Closing += (sender1, e1) =>
                            {
                                if (fiboRetracemConfig.DialogResult == true)
                                {
                                    ((FibonacciRetracements)estudo).SetaParametros(fiboRetracemConfig.Linhas);
                                    estudo.StrokeThickness = fiboRetracemConfig.Espessura;
                                    estudo.StrokeType = fiboRetracemConfig.TipoLinha;
                                    estudo.Stroke = fiboRetracemConfig.Cor;
                                }
                            };
                            fiboRetracemConfig.Show();
                            break;

                        //Se for selecionado qualquer objeto diferente dos casos acima.
                        default:
                            //Criando uma nova instância da configuracaoGrafico do objeto
                            if (estudosSelecionados[0].StudyType == LineStudy.StudyTypeEnum.TrendLine)
                                configuraObjeto = new ConfiguraObjeto(estudosSelecionados[0].Stroke, estudosSelecionados[0].StrokeThickness, estudosSelecionados[0].StrokeType, (TrendLine)estudosSelecionados[0], estudo.LinhaMagnetica, true, false, false, false);
                            else
                                configuraObjeto = new ConfiguraObjeto(estudosSelecionados[0].Stroke, estudosSelecionados[0].StrokeThickness, estudosSelecionados[0].StrokeType, estudo.LinhaMagnetica, true, false, estudo.Suporte, estudo.Resistencia);

                            //Utilizando expressão lambda que faz com que o bloco de código abaixo seja executado
                            //quando o evento ocorrer
                            configuraObjeto.Closing += (sender1, e1) =>
                            {
                                if (configuraObjeto.DialogResult == true)
                                {
                                    estudosSelecionados[0].Stroke = configuraObjeto.BrushObjeto;
                                    estudosSelecionados[0].StrokeType = configuraObjeto.TipoLinhaObjeto;
                                    estudosSelecionados[0].StrokeThickness = configuraObjeto.GrossuraObjeto;
                                    estudosSelecionados[0].LinhaMagnetica = configuraObjeto.LinhaMagnetica;

                                    if (estudosSelecionados[0].StudyType == LineStudy.StudyTypeEnum.TrendLine)
                                        estudosSelecionados[0].LinhaInfinitaADireita = configuraObjeto.LinhaTendenciaInfinita;

                                    stockChart.Update();

                                    //Atualizando a lista de objetos
                                    this.listaObjetos = this.RetornaObjetosPresentesNoGrafico();
                                }
                            };

                            //Mostrando o form
                            configuraObjeto.Show();
                            break;
                    }
                }

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Carrega o form de edição para o objeto selecionado.
        /// </summary>
        public void ConfigurarObjetoSelecionadoPainelIndicador()
        {
            try
            {
                List<LineStudy> estudosSelecionados = EstudosSelecionadosPainelIndicador;

                if (estudosSelecionados.Count > 0)
                {
                    LineStudy estudo = estudosSelecionados[0];

                    switch (estudo.StudyType)
                    {

                        //Se for selecionado um objeto texto
                        case LineStudy.StudyTypeEnum.StaticText:
                            ConfiguraTexto configTxt = new ConfiguraTexto(((StaticText)estudo).Text, estudo.StrokeThickness, estudo.Stroke);

                            configTxt.Closing += (sender1, e1) =>
                            {
                                if (configTxt.DialogResult == true)
                                {
                                    estudo.StrokeThickness = configTxt.TamanhoFonte;
                                    ((StaticText)estudosSelecionados[0]).Text = configTxt.Texto;
                                    estudo.Stroke = configTxt.Cor;
                                    stockChartIndicadores.Update();
                                }
                            };

                            //Mostrando o form
                            configTxt.Show();
                            break;

                        //Se for selecionado um objeto ErrorChannel
                        case LineStudy.StudyTypeEnum.ErrorChannel:

                            ConfiguraErrorChannel errorChannConfig = new ConfiguraErrorChannel(((ErrorChannel)estudo).ObtemParametro(), estudo.Stroke, estudo.StrokeThickness);

                            errorChannConfig.Closing += (sender1, e1) =>
                            {
                                if (errorChannConfig.DialogResult == true)
                                {
                                    ((ErrorChannel)estudo).SetaParametros(errorChannConfig.Valor);
                                    estudo.Stroke = errorChannConfig.Cor;
                                    estudo.StrokeThickness = errorChannConfig.Espessura;
                                }
                            };
                            errorChannConfig.Show();

                            break;

                        //Se for selecionado um Fibonacci Retracements
                        case LineStudy.StudyTypeEnum.FibonacciRetracements:

                            ConfiguraFibonacciRetracement fiboRetracemConfig = new ConfiguraFibonacciRetracement(((FibonacciRetracements)estudo).ObtemParametro(), estudo.StrokeThickness, estudo.StrokeType, estudo.Stroke, estudo.LinhaMagnetica);

                            fiboRetracemConfig.Closing += (sender1, e1) =>
                            {
                                if (fiboRetracemConfig.DialogResult == true)
                                {
                                    ((FibonacciRetracements)estudo).SetaParametros(fiboRetracemConfig.Linhas);
                                    estudo.StrokeThickness = fiboRetracemConfig.Espessura;
                                    estudo.StrokeType = fiboRetracemConfig.TipoLinha;
                                    estudo.Stroke = fiboRetracemConfig.Cor;
                                }
                            };
                            fiboRetracemConfig.Show();
                            break;

                        //Se for selecionado qualquer objeto diferente dos casos acima.
                        default:
                            //Criando uma nova instância da configuracaoGrafico do objeto
                            if (estudosSelecionados[0].StudyType == LineStudy.StudyTypeEnum.TrendLine)
                                configuraObjeto = new ConfiguraObjeto(estudosSelecionados[0].Stroke, estudosSelecionados[0].StrokeThickness, estudosSelecionados[0].StrokeType, (TrendLine)estudosSelecionados[0], estudo.LinhaMagnetica, false, false,false, false);
                            else
                                configuraObjeto = new ConfiguraObjeto(estudosSelecionados[0].Stroke, estudosSelecionados[0].StrokeThickness, estudosSelecionados[0].StrokeType, estudo.LinhaMagnetica, false, false, estudosSelecionados[0].Suporte, estudosSelecionados[0].Resistencia);

                            //Utilizando expressão lambda que faz com que o bloco de código abaixo seja executado
                            //quando o evento ocorrer
                            configuraObjeto.Closing += (sender1, e1) =>
                            {
                                if (configuraObjeto.DialogResult == true)
                                {
                                    estudosSelecionados[0].Stroke = configuraObjeto.BrushObjeto;
                                    estudosSelecionados[0].StrokeType = configuraObjeto.TipoLinhaObjeto;
                                    estudosSelecionados[0].StrokeThickness = configuraObjeto.GrossuraObjeto;
                                    estudosSelecionados[0].LinhaMagnetica = configuraObjeto.LinhaMagnetica;

                                    if (estudosSelecionados[0].StudyType == LineStudy.StudyTypeEnum.TrendLine)
                                        estudosSelecionados[0].LinhaInfinitaADireita = configuraObjeto.LinhaTendenciaInfinita;

                                    stockChartIndicadores.Update();
                                    this.listaObjetos = this.RetornaObjetosPresentesNoGrafico();
                                }
                            };

                            //Mostrando o form
                            configuraObjeto.Show();
                            break;
                    }
                }

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion ConfigurarObjetoSelecionado()

        #region RetornaObjetosPresentesNoGrafico
        /// <summary>
        /// Retorna a lista de objetos de estudo presentes no gráfico no atual momento
        /// </summary>
        /// <returns></returns>
        private List<ObjetoEstudoDTO> RetornaObjetosPresentesNoGrafico()
        {
            try
            {
                List<ObjetoEstudoDTO> listaObjetos = new List<ObjetoEstudoDTO>();

                foreach (ObjetoEstudoDTO obj in RetornaObjetosPresentesNoGraficoPorPainel(EstudosInseridos))
                {
                    listaObjetos.Add(obj);
                }
                foreach (ObjetoEstudoDTO obj in RetornaObjetosPresentesNoGraficoPorPainel(EstudosInseridosPainelIndicador))
                {
                    listaObjetos.Add(obj);
                }
                //Zerando os objetos
                //for (int i = 0; i <= listaObjetos.Count - 1; i++ )
                //{
                //    if (listaObjetos[i].RecordInicial < 0)
                //        listaObjetos[i].RecordInicial = 0;
                //    if (listaObjetos[i].RecordFinal < 0)
                //        listaObjetos[i].RecordFinal = 0;
                //    if (listaObjetos[i].X1 < 0)
                //        listaObjetos[i].X1 = 0;
                //    if (listaObjetos[i].X2 < 0)
                //        listaObjetos[i].X2 = 0;
                //    if (listaObjetos[i].Y1 < 0)
                //        listaObjetos[i].Y1 = 0;
                //    if (listaObjetos[i].Y2 < 0)
                //        listaObjetos[i].Y2 = 0;
                //}

                return listaObjetos;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que retorna a lista de ObjetoDTO para os estudos passados
        /// </summary>
        /// <param name="estudos"></param>
        /// <returns></returns>
        private List<ObjetoEstudoDTO> RetornaObjetosPresentesNoGraficoPorPainel(List<LineStudy> estudos)
        {
            try
            {
                //TODO: Implementar salvamento de parametros de determinados objetos como erro channel, fibo retrac  e texto
                List<ObjetoEstudoDTO> objetos = new List<ObjetoEstudoDTO>();
                List<LineStudy> objetosExistentes = estudos;

                if (objetosExistentes != null)
                {
                    foreach (LineStudy linhaEstudo in objetosExistentes)
                    {
                        ObjetoEstudoDTO objetoDTO = new ObjetoEstudoDTO();

                        objetoDTO.PainelIndicadores = linhaEstudo.PainelIndicador;
                        objetoDTO.Cor = linhaEstudo.Stroke;
                        objetoDTO.TipoObjeto = (int)linhaEstudo.StudyType;

                        objetoDTO.Grossura = (int)linhaEstudo.StrokeThickness;
                        objetoDTO.IndexPainel = linhaEstudo.Panel.Index;
                        objetoDTO.InfinitaADireita = linhaEstudo.LinhaInfinitaADireita;
                        objetoDTO.Magnetica = linhaEstudo.LinhaMagnetica;
                        objetoDTO.TipoLinha = linhaEstudo.StrokeType;

                        objetoDTO.DataInicial = linhaEstudo.DataOriginalInicial;
                        objetoDTO.DataFinal = linhaEstudo.DataOriginalFinal;
                        objetoDTO.RecordInicial = (int)linhaEstudo.X1Value;
                        objetoDTO.RecordFinal = (int)linhaEstudo.X2Value;
                        objetoDTO.ValorInicial = linhaEstudo.Y1Value;
                        objetoDTO.ValorFinal = linhaEstudo.Y2Value;
                        objetoDTO.X1 = linhaEstudo.X1Value;
                        objetoDTO.X2 = linhaEstudo.X2Value;
                        objetoDTO.Y1 = linhaEstudo.Y1Value;
                        objetoDTO.Y2 = linhaEstudo.Y2Value;
                        objetoDTO.Parametros = linhaEstudo.ObtemParametros(';');

                        objetos.Add(objetoDTO);
                    }
                }

                return objetos;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        #endregion GeraTemplateObjetos()

        #region ExcluiTodosObjetos()
        /// <summary>
        /// Exclui todos os objetos d gráfico.
        /// </summary>
        public void ExcluiTodosObjetos()
        {
            try
            {
                List<LineStudy> estudosInseridos = EstudosInseridos;

                foreach (LineStudy obj in estudosInseridos)
                {
                    stockChart.RemoveObject(obj.Key);
                }

                estudosInseridos = EstudosInseridosPainelIndicador;

                foreach (LineStudy obj in estudosInseridos)
                {
                    stockChartIndicadores.RemoveObject(obj.Key);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion ExcluiTodosObjetos()

        #endregion

        #region Indicadores

        #region ConfigurarIndicadorSelecionado()
        /// <summary>
        /// Carrega o form de edição para o indicador selecionado.
        /// </summary>
        public void ConfigurarIndicadorSelecionado()
        {
            try
            {
                if (IndicadorSelecionado != null)
                {
                    Indicator indicadorSelecionado = IndicadorSelecionado;

                    if (indicadorSelecionado.IsSerieFilha)
                    {
                        Color? corAux = indicadorSelecionado.StrokeColor;

                        if (corAux == null)
                            corAux = indicadorSelecionado.UpColor;

                        ConfiguraSerie serieConfig = new ConfiguraSerie(corAux, indicadorSelecionado.StrokeThickness, indicadorSelecionado.StrokePattern);
                        serieConfig.Closing += (sender1, e1) =>
                        {
                            indicadorSelecionado.DownColor = serieConfig.StrokeColor;
                            indicadorSelecionado.UpColor = serieConfig.StrokeColor;
                            indicadorSelecionado.StrokeColor = (Color)serieConfig.StrokeColor;
                            indicadorSelecionado.StrokePattern = serieConfig.TipoLinhaSerie;
                            indicadorSelecionado.StrokeThickness = serieConfig.GrossuraSerie;
                            indicadorSelecionado.TitleBrush = new SolidColorBrush((Color)serieConfig.StrokeColor);

                            stockChart.Update();
                        };
                        serieConfig.Show();
                    }
                    else
                    {

                        List<ConfiguracaoIndicador.IndicadorParametroDTO> parametros = new List<ConfiguracaoIndicador.IndicadorParametroDTO>();

                        //Obtendo parametros padroes do indicador
                        AtualizaValoresPadroesIndicador(ref parametros, indicadorSelecionado.IndicatorType);

                        //Obtendo parametros atuais do indicador
                        for (int i = 0; i < parametros.Count; i++)
                        {
                            switch (parametros[i].TipoParametro)
                            {
                                case ConfiguracaoIndicador.IndicadorTipoParametro.Double:
                                    parametros[i].ValorDouble = Convert.ToDouble(indicadorSelecionado.GetParameterValue(i));
                                    break;

                                case ConfiguracaoIndicador.IndicadorTipoParametro.Inteiro:
                                    parametros[i].ValorInteiro = Convert.ToInt32(indicadorSelecionado.GetParameterValue(i));
                                    break;

                                case ConfiguracaoIndicador.IndicadorTipoParametro.String:
                                    parametros[i].ValorString = indicadorSelecionado.GetParameterValue(i).ToString();
                                    break;
                            }
                        }


                        //ConfiguracaoIndicador indConfig = new ConfiguracaoIndicador(this, parametros, indicadorSelecionado.UpColor, indicadorSelecionado.DownColor,
                         //                                               indicadorSelecionado.StrokePattern, Convert.ToInt32(indicadorSelecionado.StrokeThickness), indicadorSelecionado.FullName);


                        //indConfig.Closing += (sender1, e1) =>
                        //{
                        //    if (indConfig.DialogResult == true)
                        //    {
                        //        if (!indConfig.ExcluiIndicador)
                        //        {

                        //            //Editando parametros do indicador
                        //            //indicadorSelecionado.DownColor = indConfig.CorIndicadorBaixa;
                        //            indicadorSelecionado.DownColor = indConfig.CorIndicadorAlta;
                        //            indicadorSelecionado.UpColor = indConfig.CorIndicadorAlta;
                        //            indicadorSelecionado.StrokePattern = indConfig.TipoLinha;
                        //            indicadorSelecionado.StrokeThickness = indConfig.Grossura;
                        //            indicadorSelecionado.StrokeColor = (Color)indConfig.CorIndicadorAlta;
                        //            indicadorSelecionado.TitleBrush = new SolidColorBrush((Color)indConfig.CorIndicadorAlta);

                        //            //Obtendo parametros fornecidos para este indicador
                        //            int index = 0;
                        //            foreach (ConfiguracaoIndicador.IndicadorParametroDTO obj in parametros)
                        //            {
                        //                switch (obj.TipoParametro)
                        //                {
                        //                    case ConfiguracaoIndicador.IndicadorTipoParametro.Double:
                        //                        indicadorSelecionado.SetParameterValue(index, obj.ValorDouble);
                        //                        break;

                        //                    case ConfiguracaoIndicador.IndicadorTipoParametro.Inteiro:
                        //                        indicadorSelecionado.SetParameterValue(index, obj.ValorInteiro);
                        //                        break;

                        //                    case ConfiguracaoIndicador.IndicadorTipoParametro.String:
                        //                        indicadorSelecionado.SetParameterValue(index, obj.ValorString);
                        //                        break;
                        //                }

                        //                index++;
                        //            }

                        //            stockChart.InvalidateIndicators();

                        //            indicadorSelecionado.Calculate();
                        //            indicadorSelecionado.Paint();
                        //            indicadorSelecionado.HideSelection();

                        //            stockChart.Update();
                        //        }
                        //        else
                        //        {
                        //            ExcluiIndicadorSelecionado(indicadorSelecionado, false);
                        //        }
                        //    }
                        //};

                        //indConfig.Show();
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Carrega o form de edição para o indicador selecionado.
        /// </summary>
        public void ConfigurarIndicadorSelecionadoPainelIndicadores()
        {
            try
            {
                if (IndicadoresSelecionadosPainelIndicador != null)
                {
                    Indicator indicadorSelecionado = IndicadorSelecionadoPainelIndicadores;

                    if (indicadorSelecionado.IsSerieFilha)
                    {
                        Color? corAux = indicadorSelecionado.StrokeColor;

                        if (corAux == null)
                            corAux = indicadorSelecionado.UpColor;

                        ConfiguraSerie serieConfig = new ConfiguraSerie(corAux, indicadorSelecionado.StrokeThickness, indicadorSelecionado.StrokePattern);
                        serieConfig.Closing += (sender1, e1) =>
                        {
                            indicadorSelecionado.DownColor = serieConfig.StrokeColor;
                            indicadorSelecionado.UpColor = serieConfig.StrokeColor;
                            indicadorSelecionado.StrokeColor = (Color)serieConfig.StrokeColor;
                            indicadorSelecionado.StrokePattern = serieConfig.TipoLinhaSerie;
                            indicadorSelecionado.StrokeThickness = serieConfig.GrossuraSerie;
                            indicadorSelecionado.TitleBrush = new SolidColorBrush((Color)serieConfig.StrokeColor);

                            //devo atualizar a lista de indicadores                            
                            this.listaIndicadores = this.RetornaIndicadoresPresentesNoGrafico();


                            stockChartIndicadores.Update();
                        };
                        serieConfig.Show();
                    }
                    else
                    {

                        List<ConfiguracaoIndicador.IndicadorParametroDTO> parametros = new List<ConfiguracaoIndicador.IndicadorParametroDTO>();

                        //Obtendo parametros padroes do indicador
                        AtualizaValoresPadroesIndicador(ref parametros, indicadorSelecionado.IndicatorType);

                        //Obtendo parametros atuais do indicador
                        for (int i = 0; i < parametros.Count; i++)
                        {
                            switch (parametros[i].TipoParametro)
                            {
                                case ConfiguracaoIndicador.IndicadorTipoParametro.Double:
                                    parametros[i].ValorDouble = Convert.ToDouble(indicadorSelecionado.GetParameterValue(i));
                                    break;

                                case ConfiguracaoIndicador.IndicadorTipoParametro.Inteiro:
                                    parametros[i].ValorInteiro = Convert.ToInt32(indicadorSelecionado.GetParameterValue(i));
                                    break;

                                case ConfiguracaoIndicador.IndicadorTipoParametro.String:
                                    parametros[i].ValorString = indicadorSelecionado.GetParameterValue(i).ToString();
                                    break;
                            }
                        }


                        //ConfiguracaoIndicador indConfig = new ConfiguracaoIndicador(this, parametros, indicadorSelecionado.UpColor, indicadorSelecionado.DownColor,
                        //                                                indicadorSelecionado.StrokePattern, Convert.ToInt32(indicadorSelecionado.StrokeThickness), indicadorSelecionado.FullName);


                        //indConfig.Closing += (sender1, e1) =>
                        //{
                        //    if (indConfig.DialogResult == true)
                        //    {
                        //        if (!indConfig.ExcluiIndicador)
                        //        {

                        //            //Editando parametros do indicador
                        //            //indicadorSelecionado.DownColor = indConfig.CorIndicadorBaixa;
                        //            indicadorSelecionado.DownColor = indConfig.CorIndicadorAlta;
                        //            indicadorSelecionado.UpColor = indConfig.CorIndicadorAlta;
                        //            indicadorSelecionado.StrokePattern = indConfig.TipoLinha;
                        //            indicadorSelecionado.StrokeThickness = indConfig.Grossura;
                        //            indicadorSelecionado.StrokeColor = (Color)indConfig.CorIndicadorAlta;
                        //            indicadorSelecionado.TitleBrush = new SolidColorBrush((Color)indConfig.CorIndicadorAlta);

                        //            //Obtendo parametros fornecidos para este indicador
                        //            int index = 0;
                        //            foreach (ConfiguracaoIndicador.IndicadorParametroDTO obj in parametros)
                        //            {
                        //                switch (obj.TipoParametro)
                        //                {
                        //                    case ConfiguracaoIndicador.IndicadorTipoParametro.Double:
                        //                        indicadorSelecionado.SetParameterValue(index, obj.ValorDouble);
                        //                        break;

                        //                    case ConfiguracaoIndicador.IndicadorTipoParametro.Inteiro:
                        //                        indicadorSelecionado.SetParameterValue(index, obj.ValorInteiro);
                        //                        break;

                        //                    case ConfiguracaoIndicador.IndicadorTipoParametro.String:
                        //                        indicadorSelecionado.SetParameterValue(index, obj.ValorString);
                        //                        break;
                        //                }

                        //                index++;
                        //            }

                        //            stockChartIndicadores.InvalidateIndicators();

                        //            indicadorSelecionado.Calculate();
                        //            indicadorSelecionado.Paint();
                        //            indicadorSelecionado.HideSelection();

                        //            stockChartIndicadores.Update();
                        //        }
                        //        else
                        //        {
                        //            ExcluiIndicadorSelecionado(indicadorSelecionado, true);
                        //        }
                        //    }
                        //};

                        //indConfig.Show();
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion ConfigurarIndicadorSelecionado()

        #region ConfiguraIndicador()
        /// <summary>
        /// Permite configurar o indicador desejado.
        /// </summary>
        /// <param name="indicador">Indicador desejado.</param>
        public void ConfiguraIndicador(Indicator indicador, bool PainelIndicadores)
        {
            try
            {
                Indicator indicadorSelecionado = indicador;

                if (indicadorSelecionado.IsSerieFilha)
                {
                    Color? corAux = indicadorSelecionado.StrokeColor;

                    if (corAux == null)
                        corAux = indicadorSelecionado.UpColor;

                    ConfiguraSerie serieConfig =
                        new ConfiguraSerie(corAux, indicadorSelecionado.StrokeThickness, indicadorSelecionado.StrokePattern);
                    serieConfig.Closing += (sender1, e1) =>
                    {
                        if (serieConfig.DialogResult == true)
                        {
                            indicadorSelecionado.DownColor = serieConfig.StrokeColor;
                            indicadorSelecionado.UpColor = serieConfig.StrokeColor;
                            indicadorSelecionado.StrokePattern = serieConfig.TipoLinhaSerie;
                            indicadorSelecionado.StrokeThickness = serieConfig.GrossuraSerie;
                            indicadorSelecionado.StrokeColor = (Color)serieConfig.StrokeColor;
                            indicadorSelecionado.TitleBrush = new SolidColorBrush((Color)serieConfig.StrokeColor);

                            stockChartIndicadores.Update();
                            stockChart.Update();
                        }
                    };
                    serieConfig.Show();
                }
                else
                {

                    List<ConfiguracaoIndicador.IndicadorParametroDTO> parametros = new List<ConfiguracaoIndicador.IndicadorParametroDTO>();

                    //Obtendo parametros padroes do indicador
                    AtualizaValoresPadroesIndicador(ref parametros, indicadorSelecionado.IndicatorType);

                    //Obtendo parametros atuais do indicador
                    for (int i = 0; i < parametros.Count; i++)
                    {
                        switch (parametros[i].TipoParametro)
                        {
                            case ConfiguracaoIndicador.IndicadorTipoParametro.Double:
                                parametros[i].ValorDouble = Convert.ToDouble(indicadorSelecionado.GetParameterValue(i));
                                break;

                            case ConfiguracaoIndicador.IndicadorTipoParametro.Inteiro:
                                parametros[i].ValorInteiro = Convert.ToInt32(indicadorSelecionado.GetParameterValue(i));
                                break;

                            case ConfiguracaoIndicador.IndicadorTipoParametro.String:
                                parametros[i].ValorString = indicadorSelecionado.GetParameterValue(i).ToString();
                                break;
                        }
                    }


                    //Criando uma nova instância da configuracaoGrafico do objeto
                    //ConfiguracaoIndicador indConfig = new ConfiguracaoIndicador(this, parametros, indicadorSelecionado.UpColor, indicadorSelecionado.DownColor,
                    //                                                indicadorSelecionado.StrokePattern, Convert.ToInt32(indicadorSelecionado.StrokeThickness), indicadorSelecionado.FullName);
                    //indConfig.Closing += (sender1, e1) =>
                    //{
                    //    if (indConfig.DialogResult == true)
                    //    {
                    //        if (!indConfig.ExcluiIndicador)
                    //        {
                    //            //Editando parametros do indicador
                    //            indicadorSelecionado.DownColor = indConfig.CorIndicadorAlta;
                    //            indicadorSelecionado.UpColor = indConfig.CorIndicadorAlta;
                    //            indicadorSelecionado.StrokePattern = indConfig.TipoLinha;
                    //            indicadorSelecionado.StrokeThickness = indConfig.Grossura;
                    //            indicadorSelecionado.StrokeColor = (Color)indConfig.CorIndicadorAlta;
                    //            indicadorSelecionado.TitleBrush = new SolidColorBrush((Color)indConfig.CorIndicadorAlta);

                    //            //Obtendo parametros fornecidos para este indicador
                    //            int index = 0;
                    //            foreach (ConfiguracaoIndicador.IndicadorParametroDTO obj in parametros)
                    //            {
                    //                switch (obj.TipoParametro)
                    //                {
                    //                    case ConfiguracaoIndicador.IndicadorTipoParametro.Double:
                    //                        indicadorSelecionado.SetParameterValue(index, obj.ValorDouble);
                    //                        break;

                    //                    case ConfiguracaoIndicador.IndicadorTipoParametro.Inteiro:
                    //                        indicadorSelecionado.SetParameterValue(index, obj.ValorInteiro);
                    //                        break;

                    //                    case ConfiguracaoIndicador.IndicadorTipoParametro.String:
                    //                        indicadorSelecionado.SetParameterValue(index, obj.ValorString);
                    //                        break;
                    //                }

                    //                index++;
                    //            }

                    //            stockChartIndicadores.InvalidateIndicators();

                    //            indicadorSelecionado.Calculate();
                    //            indicadorSelecionado.Paint();
                    //            indicadorSelecionado.HideSelection();

                    //            stockChartIndicadores.Update();
                    //        }
                    //        else
                    //        {
                    //            ExcluiIndicadorSelecionado(indicadorSelecionado, PainelIndicadores);
                    //        }
                    //    }
                    //};

                    //indConfig.Show();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion ConfiguraIndicador()

        #region AtualizaValoresPadroesIndicador()
        /// <summary>
        /// Obtem os valores padroes para um indicador.
        /// </summary>
        /// <param name="indicador">Indicador como referência.</param>
        private void AtualizaValoresPadroesIndicador(ref Indicator indicador)
        {
            switch (indicador.IndicatorType)
            {
                case EnumGeral.IndicatorType.AccumulativeSwingIndex:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 12);
                    break;


                case EnumGeral.IndicatorType.Aroon:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.OsciladorAroon:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.BandasBollinger:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    indicador.SetParameterValue(2, 2);
                    indicador.SetParameterValue(3, 1);
                    break;


                case EnumGeral.IndicatorType.ChaikinFluxoFinanceiro:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, stockChart.Symbol + ".Volume");
                    indicador.SetParameterValue(2, 14);
                    break;


                case EnumGeral.IndicatorType.ChaikinVolatilidade:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 14);
                    indicador.SetParameterValue(2, 2);
                    indicador.SetParameterValue(3, 1);
                    break;


                case EnumGeral.IndicatorType.OsciladorChandeMomentum:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.CommodityChannelIndex:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.IndiceForcaRelativaComparada:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, stockChart.Symbol + ".Abertura");
                    break;


                case EnumGeral.IndicatorType.OsciladorDetrendedPrice:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    indicador.SetParameterValue(2, 1);
                    break;


                case EnumGeral.IndicatorType.MovimentoDirecionalADX:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.EaseOfMovement:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, stockChart.Symbol + ".Volume");
                    indicador.SetParameterValue(2, 8);
                    indicador.SetParameterValue(3, 1);
                    break;


                case EnumGeral.IndicatorType.MediaMovelExponencial:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.BandasFractalChaos:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 10);
                    break;


                case EnumGeral.IndicatorType.OsciladorFractalChaos:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 10);
                    break;


                case EnumGeral.IndicatorType.BandasMaximoMinimo:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    break;


                case EnumGeral.IndicatorType.HighMinusLow:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    break;


                case EnumGeral.IndicatorType.VolatilidadeHistorica:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 30);
                    indicador.SetParameterValue(2, 365);
                    indicador.SetParameterValue(3, 2);
                    break;


                case EnumGeral.IndicatorType.RgressaoLinearForecast:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 9);
                    break;


                case EnumGeral.IndicatorType.RegressaoLinearIntercept:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 9);
                    break;


                case EnumGeral.IndicatorType.RegressaoLinearRaizQuadrada:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 9);
                    break;


                case EnumGeral.IndicatorType.RegressaoLinearSlope:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 9);
                    break;


                case EnumGeral.IndicatorType.MACD:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 26);
                    indicador.SetParameterValue(2, 13);
                    indicador.SetParameterValue(3, 9);
                    break;


                case EnumGeral.IndicatorType.MACDHistograma:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 26);
                    indicador.SetParameterValue(2, 13);
                    indicador.SetParameterValue(3, 9);
                    break;



                case EnumGeral.IndicatorType.MassIndex:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 8);
                    break;


                case EnumGeral.IndicatorType.PrecoMedio:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    break;


                case EnumGeral.IndicatorType.OsciladorMomentum:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.IndiceFluxoFinanceiro:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, stockChart.Symbol + ".Volume");
                    indicador.SetParameterValue(2, 14);
                    break;


                case EnumGeral.IndicatorType.MediaMovelEnvelope:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    indicador.SetParameterValue(2, 1);
                    indicador.SetParameterValue(3, 5);
                    break;


                case EnumGeral.IndicatorType.IndiceVolumeNegativo:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, stockChart.Symbol + ".Volume");
                    break;


                case EnumGeral.IndicatorType.OnBalanceVolume:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, stockChart.Symbol + ".Volume");
                    break;


                case EnumGeral.IndicatorType.SARParabólico:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 0.02);
                    indicador.SetParameterValue(2, 0.2);
                    break;


                case EnumGeral.IndicatorType.IndicePerformance:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    break;


                case EnumGeral.IndicatorType.IndiceVolumePositivo:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, stockChart.Symbol + ".Volume");
                    break;


                case EnumGeral.IndicatorType.OsciladorPreco:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 22);
                    indicador.SetParameterValue(2, 14);
                    indicador.SetParameterValue(3, 1);
                    break;



                case EnumGeral.IndicatorType.PriceROC:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 12);
                    break;


                case EnumGeral.IndicatorType.TendenciaPrecoVolume:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, stockChart.Symbol + ".Volume");
                    break;


                case EnumGeral.IndicatorType.BandasNumerosPrimos:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    break;


                case EnumGeral.IndicatorType.OsciladorNumerosPrimos:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    break;


                case EnumGeral.IndicatorType.OsciladorRainbow:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 9);
                    indicador.SetParameterValue(2, 1);
                    break;


                case EnumGeral.IndicatorType.IndiceForcaRelativa:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.MediaMovelSimples:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.DesvioPadrao:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    indicador.SetParameterValue(2, 2);
                    indicador.SetParameterValue(3, 1);
                    break;


                case EnumGeral.IndicatorType.StochasticMomentumIndex:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 13);
                    indicador.SetParameterValue(2, 25);
                    indicador.SetParameterValue(3, 2);
                    indicador.SetParameterValue(4, 9);
                    indicador.SetParameterValue(5, 1);
                    indicador.SetParameterValue(6, 1);
                    break;


                case EnumGeral.IndicatorType.OsciladorEstocastico:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 9);
                    indicador.SetParameterValue(2, 3);
                    indicador.SetParameterValue(3, 9);
                    indicador.SetParameterValue(4, 1);
                    break;


                case EnumGeral.IndicatorType.SwingIndex:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 12);
                    break;


                case EnumGeral.IndicatorType.MediaMovelSerieTempo:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.TradeVolumeIndex:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, stockChart.Symbol + ".Volume");
                    indicador.SetParameterValue(2, 0.25);
                    break;


                case EnumGeral.IndicatorType.MediaMovelTriangular:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.TRIX:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 8);
                    break;


                case EnumGeral.IndicatorType.TrueRange:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    break;


                case EnumGeral.IndicatorType.TypicalPrice:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    break;


                case EnumGeral.IndicatorType.OsciladorUltimate:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 7);
                    indicador.SetParameterValue(2, 14);
                    indicador.SetParameterValue(3, 28);
                    break;


                case EnumGeral.IndicatorType.MediaMovelVariavel:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.FiltroVerticalHorizontal:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.VIDYA:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    indicador.SetParameterValue(2, 0.65);
                    break;


                case EnumGeral.IndicatorType.OsciladorVolume:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Volume");
                    indicador.SetParameterValue(1, 9);
                    indicador.SetParameterValue(2, 21);
                    indicador.SetParameterValue(3, 1);
                    break;


                case EnumGeral.IndicatorType.VolumeROC:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Volume");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.FechamentoPonderado:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    break;


                case EnumGeral.IndicatorType.MediaMovelPonderada:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.WellesWilderSmoothing:
                    indicador.SetParameterValue(0, stockChart.Symbol + ".Ultimo");
                    indicador.SetParameterValue(1, 14);
                    break;


                case EnumGeral.IndicatorType.AcumulacaoDistribuicaoWilliams:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 12);
                    break;


                case EnumGeral.IndicatorType.WilliamsPctR:
                    indicador.SetParameterValue(0, stockChart.Symbol);
                    indicador.SetParameterValue(1, 14);
                    break;
            }
        }
        #endregion AtualizaValoresPadroesIndicador()

        #region AtualizaValoresPadroesIndicador(+1)
        /// <summary>
        /// Obtem os valores padroes para um indicador.
        /// </summary>
        /// <param name="indicador">Indicador como referência.</param>
        private void AtualizaValoresPadroesIndicador(ref List<ConfiguracaoIndicador.IndicadorParametroDTO> parametrosDTO, EnumGeral.IndicatorType tipoIndicador)
        {
            #region Indicadores Padrões
            switch (tipoIndicador)
            {
                case EnumGeral.IndicatorType.AccumulativeSwingIndex:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Valor limite móvel", (double)12, ""));
                    break;


                case EnumGeral.IndicatorType.Aroon:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.OsciladorAroon:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.BandasBollinger:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Desvio Padrão", (int)2, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    break;


                case EnumGeral.IndicatorType.ChaikinFluxoFinanceiro:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.ChaikinVolatilidade:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Rate of Chg", (int)2, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    break;


                case EnumGeral.IndicatorType.OsciladorChandeMomentum:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.CommodityChannelIndex:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.IndiceForcaRelativaComparada:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Abertura", "Série"));
                    break;


                case EnumGeral.IndicatorType.OsciladorDetrendedPrice:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    break;


                case EnumGeral.IndicatorType.MovimentoDirecionalADX:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.EaseOfMovement:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)8, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    break;


                case EnumGeral.IndicatorType.MediaMovelExponencial:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.BandasFractalChaos:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)10, ""));
                    break;


                case EnumGeral.IndicatorType.OsciladorFractalChaos:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)10, ""));
                    break;


                case EnumGeral.IndicatorType.BandasMaximoMinimo:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.HighMinusLow:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    break;


                case EnumGeral.IndicatorType.VolatilidadeHistorica:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)30, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Histórico Barras", (int)365, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Desvio Padrão", (int)2, ""));
                    break;


                case EnumGeral.IndicatorType.RgressaoLinearForecast:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)9, ""));
                    break;


                case EnumGeral.IndicatorType.RegressaoLinearIntercept:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)9, ""));
                    break;


                case EnumGeral.IndicatorType.RegressaoLinearRaizQuadrada:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)9, ""));
                    break;


                case EnumGeral.IndicatorType.RegressaoLinearSlope:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)9, ""));
                    break;


                case EnumGeral.IndicatorType.MACD:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ciclo longo", (int)26, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ciclo curto", (int)13, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Período sinal", (int)9, ""));
                    break;


                case EnumGeral.IndicatorType.MACDHistograma:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ciclo longo", (int)26, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ciclo curto", (int)13, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Período sinal", (int)9, ""));
                    break;


                case EnumGeral.IndicatorType.MassIndex:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)8, ""));
                    break;


                case EnumGeral.IndicatorType.PrecoMedio:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    break;


                case EnumGeral.IndicatorType.OsciladorMomentum:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.IndiceFluxoFinanceiro:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.MediaMovelEnvelope:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Shift", (double)5, ""));
                    break;


                case EnumGeral.IndicatorType.IndiceVolumeNegativo:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    break;


                case EnumGeral.IndicatorType.OnBalanceVolume:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    break;


                case EnumGeral.IndicatorType.SARParabólico:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Min AF", (double)0.02, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Max AF", (double)0.2, ""));
                    break;


                case EnumGeral.IndicatorType.IndicePerformance:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    break;


                case EnumGeral.IndicatorType.IndiceVolumePositivo:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    break;


                case EnumGeral.IndicatorType.OsciladorPreco:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ciclo longo", (int)22, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ciclo curto", (int)14, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    break;



                case EnumGeral.IndicatorType.PriceROC:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)12, ""));
                    break;


                case EnumGeral.IndicatorType.TendenciaPrecoVolume:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    break;


                case EnumGeral.IndicatorType.BandasNumerosPrimos:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    break;


                case EnumGeral.IndicatorType.OsciladorNumerosPrimos:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    break;


                case EnumGeral.IndicatorType.OsciladorRainbow:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Levels", (int)12, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    break;


                case EnumGeral.IndicatorType.IndiceForcaRelativa:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.MediaMovelSimples:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.DesvioPadrao:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Desvio Padrão", (int)2, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    break;


                case EnumGeral.IndicatorType.StochasticMomentumIndex:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("%K Períodos", (int)13, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("%K Smooth", (int)25, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("%K Dbl Smooth", (int)2, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("%D Períodos", (int)9, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("%D Tipo M.Móvel", (int)1, ""));
                    break;


                case EnumGeral.IndicatorType.OsciladorEstocastico:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("%K Períodos", (int)9, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("%K Slowing", (int)3, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("%D Períodos", (int)9, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Tipo M.Móvel", (int)1, "Tipo M.Móvel"));
                    break;


                case EnumGeral.IndicatorType.SwingIndex:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Limite móvel", (double)12, ""));
                    break;


                case EnumGeral.IndicatorType.MediaMovelSerieTempo:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.TradeVolumeIndex:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Menor valor tick", (double)0.25, ""));
                    break;


                case EnumGeral.IndicatorType.MediaMovelTriangular:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.TRIX:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)8, ""));
                    break;


                case EnumGeral.IndicatorType.TrueRange:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    break;


                case EnumGeral.IndicatorType.TypicalPrice:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    break;


                case EnumGeral.IndicatorType.OsciladorUltimate:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ciclo 1", (int)7, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ciclo 2", (int)14, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ciclo 3", (int)28, ""));
                    break;


                case EnumGeral.IndicatorType.MediaMovelVariavel:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.FiltroVerticalHorizontal:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.VIDYA:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Escala R2", (double)0.65, ""));
                    break;


                case EnumGeral.IndicatorType.OsciladorVolume:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Termo curto", (int)9, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Termo curto", (int)21, ""));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Pontos ou Porc.", (int)1, ""));
                    break;


                case EnumGeral.IndicatorType.VolumeROC:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Volume", stockChart.Symbol + ".Volume", "Volume"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.FechamentoPonderado:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    break;


                case EnumGeral.IndicatorType.MediaMovelPonderada:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.WellesWilderSmoothing:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;


                case EnumGeral.IndicatorType.AcumulacaoDistribuicaoWilliams:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    break;


                case EnumGeral.IndicatorType.WilliamsPctR:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Períodos", (int)14, ""));
                    break;

                case EnumGeral.IndicatorType.Agulhada:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Série", stockChart.Symbol + ".Ultimo", "Série"));
                    break;

                case EnumGeral.IndicatorType.Keltner:
                    parametrosDTO.Add(new ConfiguracaoIndicador.IndicadorParametroDTO("Ativo", stockChart.Symbol, "Ativo"));
                    break;
            }

            #endregion Indicadores Padrões
        }
        #endregion AtualizaValoresPadroesIndicador(+1)

        

        

        

        #region ObtemIndicadorPeloNomeTraduzido()
        /// <summary>
        /// Obtem o tipo do indicador pelo seu nome traduzido.
        /// </summary>
        /// <param name="indicador">Nome do indicador traduzido.</param>
        /// <returns></returns>
        private static EnumGeral.IndicatorType ObtemIndicadorPeloNomeTraduzido(string nomeTraduzido)
        {
            try
            {
                switch (nomeTraduzido)
                {
                    case "Aroon":
                        return EnumGeral.IndicatorType.Aroon;

                    case "MACD Histograma":
                        return EnumGeral.IndicatorType.MACDHistograma;

                    case "Volatilidade Histórica":
                        return EnumGeral.IndicatorType.VolatilidadeHistorica;

                    case "Bandas de Números Primos":
                        return EnumGeral.IndicatorType.BandasNumerosPrimos;

                    case "Oscilador Números Primos":
                        return EnumGeral.IndicatorType.OsciladorNumerosPrimos;

                    case "Oscilador de Fractal Chaos":
                        return EnumGeral.IndicatorType.OsciladorFractalChaos;

                    case "Índice Estocástico":
                        return EnumGeral.IndicatorType.StochasticMomentumIndex;

                    case "Oscilador Estocástico":
                        return EnumGeral.IndicatorType.OsciladorEstocastico;

                    case "Índice de Força Relativa (IFR/RSI)":
                        return EnumGeral.IndicatorType.IndiceForcaRelativa;

                    case "Índice de Volume Positivo":
                        return EnumGeral.IndicatorType.IndiceVolumePositivo;

                    case "On Balance Volume (OBV)":
                        return EnumGeral.IndicatorType.OnBalanceVolume;

                    case "Índice de Volume Negativo":
                        return EnumGeral.IndicatorType.IndiceVolumeNegativo;

                    case "Índice de Fluxo Financeiro":
                        return EnumGeral.IndicatorType.IndiceFluxoFinanceiro;

                    case "Índice de Força Relativa Comparada":
                        return EnumGeral.IndicatorType.IndiceForcaRelativaComparada;

                    case "Envelope de Médias Móveis":
                        return EnumGeral.IndicatorType.MediaMovelEnvelope;

                    case "Bandas Maximo/Minimo (Máximo/Mínimo)":
                        return EnumGeral.IndicatorType.BandasMaximoMinimo;

                    case "Bandas de Fractal Chaos":
                        return EnumGeral.IndicatorType.BandasFractalChaos;

                    case "Bandas Bollinger":
                        return EnumGeral.IndicatorType.BandasBollinger;

                    case "Preço Médio":
                        return EnumGeral.IndicatorType.PrecoMedio;

                    case "Preço - ROC":
                        return EnumGeral.IndicatorType.PriceROC;

                    case "Desvio Padrão":
                        return EnumGeral.IndicatorType.DesvioPadrao;

                    case "Fechamento Ponderado":
                        return EnumGeral.IndicatorType.FechamentoPonderado;

                    case "Chaikin - Fluxo Financeiro (Money Flow)":
                        return EnumGeral.IndicatorType.ChaikinFluxoFinanceiro;

                    case "Commodity Channel Index (CCI)":
                        return EnumGeral.IndicatorType.CommodityChannelIndex;

                    case "Índice de Performance":
                        return EnumGeral.IndicatorType.IndicePerformance;

                    case "Tendência Preço/Volume":
                        return EnumGeral.IndicatorType.TendenciaPrecoVolume;

                    case "TRIX":
                        return EnumGeral.IndicatorType.TRIX;

                    case "Regressão Linear - Intercept":
                        return EnumGeral.IndicatorType.RegressaoLinearIntercept;

                    case "Regressão Linear - Slope":
                        return EnumGeral.IndicatorType.RegressaoLinearSlope;

                    case "Regressão Linear - Forecast":
                        return EnumGeral.IndicatorType.RgressaoLinearForecast;

                    case "Regressão Linear - Raiz Quadrada":
                        return EnumGeral.IndicatorType.RegressaoLinearRaizQuadrada;

                    case "Oscilador Aroon":
                        return EnumGeral.IndicatorType.OsciladorAroon;

                    case "Chaikin - Volatilidade":
                        return EnumGeral.IndicatorType.ChaikinVolatilidade;

                    case "Oscilador Chande Momentum":
                        return EnumGeral.IndicatorType.OsciladorChandeMomentum;

                    case "Oscilador Detrended Price":
                        return EnumGeral.IndicatorType.OsciladorDetrendedPrice;

                    case "Movimento Direcional (ADX)":
                        return EnumGeral.IndicatorType.MovimentoDirecionalADX;

                    case "Oscilador Momentum":
                        return EnumGeral.IndicatorType.OsciladorMomentum;

                    case "SAR Parabólico (PSAR)":
                        return EnumGeral.IndicatorType.SARParabólico;

                    case "Oscilador de Preços":
                        return EnumGeral.IndicatorType.OsciladorPreco;

                    case "Oscilador Rainbow":
                        return EnumGeral.IndicatorType.OsciladorRainbow;

                    case "Oscilador Ultimate":
                        return EnumGeral.IndicatorType.OsciladorUltimate;

                    case "Filtro Vertical/Horizontal":
                        return EnumGeral.IndicatorType.FiltroVerticalHorizontal;

                    case "Oscilador de Volume":
                        return EnumGeral.IndicatorType.OsciladorVolume;

                    case "Acumulação/Distribuição Williams":
                        return EnumGeral.IndicatorType.AcumulacaoDistribuicaoWilliams;

                    case "Média Móvel Ponderada":
                        return EnumGeral.IndicatorType.MediaMovelPonderada;

                    case "Média Móvel Simples (SMA)":
                        return EnumGeral.IndicatorType.MediaMovelSimples;

                    case "Média Móvel Exponencial (EMA)":
                        return EnumGeral.IndicatorType.MediaMovelExponencial;

                    case "Média Móvel Tempo Series":
                        return EnumGeral.IndicatorType.MediaMovelSerieTempo;

                    case "Média Móvel Triangular":
                        return EnumGeral.IndicatorType.MediaMovelTriangular;

                    case "Média Móvel Variável":
                        return EnumGeral.IndicatorType.MediaMovelVariavel;

                    case "Média Móvel VIDYA":
                        return EnumGeral.IndicatorType.VIDYA;

                    case "Identificador de Agulhada":
                        return EnumGeral.IndicatorType.Agulhada;

                    case "MACD":
                        return EnumGeral.IndicatorType.MACD;

                    case "Accumulative Swing index":
                        return EnumGeral.IndicatorType.AccumulativeSwingIndex;

                    case "Ease of Movement":
                        return EnumGeral.IndicatorType.EaseOfMovement;

                    case "High Minus Low":
                        return EnumGeral.IndicatorType.HighMinusLow;

                    case "Mass Index":
                        return EnumGeral.IndicatorType.MassIndex;

                    case "Índice Swing":
                        return EnumGeral.IndicatorType.SwingIndex;

                    case "Índice Trade Volume":
                        return EnumGeral.IndicatorType.TradeVolumeIndex;

                    case "True Range":
                        return EnumGeral.IndicatorType.TrueRange;

                    case "Typical Price":
                        return EnumGeral.IndicatorType.TypicalPrice;

                    case "Volume ROC":
                        return EnumGeral.IndicatorType.VolumeROC;

                    case "Welles Wilder Smoothing":
                        return EnumGeral.IndicatorType.WellesWilderSmoothing;

                    case "Willians %R":
                        return EnumGeral.IndicatorType.WilliamsPctR;

                    case "Keltner":
                        return EnumGeral.IndicatorType.Keltner;

                    default:
                        return EnumGeral.IndicatorType.Unknown;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion ObtemIndicadorPeloNomeTraduzido()

        #region RetornaIndicadoresPresentesNoGrafico

        /// <summary>
        /// Metodo que consolida os indicadores dos 2 componentes stockchart
        /// </summary>
        /// <returns></returns>
        public List<IndicadorDTO> RetornaIndicadoresPresentesNoGrafico()
        {
            try
            {
                List<IndicadorDTO> listaIndicador = new List<IndicadorDTO>();
                foreach (IndicadorDTO obj in this.RetornaIndicadoresPresentesStockChart(IndicadoresInseridos))
                {
                    listaIndicador.Add(obj);
                }
                foreach (IndicadorDTO obj in this.RetornaIndicadoresPresentesStockChart(IndicadoresInseridosPainelIndicadores))
                {
                    listaIndicador.Add(obj);
                }

                return listaIndicador;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo gera a lista de indicadores que estã aplicados e suas configurações no exato momento
        /// </summary>
        /// <returns></returns>
        public List<IndicadorDTO> RetornaIndicadoresPresentesStockChart(List<Indicator> indicadores)
        {
            try
            {
                List<IndicadorDTO> listaIndicadores = new List<IndicadorDTO>();

                foreach (Indicator indicador in indicadores)
                {
                    if (!((Series)indicador).IsSerieFilha)
                    {
                        IndicadorDTO indicadorDTO = new IndicadorDTO();

                        indicadorDTO.PainelIndicadoresLateral = indicador.PainelIndicadoresLateral;
                        indicadorDTO.PainelPreco = indicador.PainelPreco;
                        indicadorDTO.PainelVolume = indicador.PainelVolume;
                        indicadorDTO.PainelIndicadoresAbaixo = indicador.PainelIndicadoresAbaixo;

                        //indicadorDTO.CorSerieFilha1 = indicador.CorSerieFilha1;
                        //indicadorDTO.CorSerieFilha2 = indicador.CorSerieFilha2;

                        //if (indicador.CorSerieFilha1 != Colors.Black)
                        //{
                        //    if (indicador.ContemSeriesFilhas
                        //}

                        Color corAlta;

                        if (indicador.UpColor != null)
                            corAlta = (Color)indicador.UpColor;
                        else
                            corAlta = Colors.White;

                        indicadorDTO.CorAlta = corAlta;

                        Color corBaixa;

                        if (indicador.DownColor != null)
                            corBaixa = (Color)indicador.DownColor;
                        else
                            corBaixa = Colors.White;

                        indicadorDTO.CorBaixa = corBaixa;

                        indicadorDTO.Grossura = Convert.ToInt32(indicador.StrokeThickness);
                        indicadorDTO.IndexPainel = indicador._chartPanel.Index;
                        indicadorDTO.Parametros = "";
                        indicadorDTO.TipoIndicador = (int)indicador.IndicatorType;
                        indicadorDTO.TipoLinha = (EnumGeral.TipoLinha)indicador.StrokePattern;
                        indicadorDTO.StatusPainel = (int)indicador._chartPanel.State;
                        indicadorDTO.AlturaPainel = (double)indicador._chartPanel.ActualHeight;

                        for (int index = 0; index < indicador.Parameters.Count; index++)
                        {
                            if (index != indicador.Parameters.Count - 1)
                                indicadorDTO.Parametros += indicador.GetParameterValue(index) + ";";
                            else
                                indicadorDTO.Parametros += indicador.GetParameterValue(index);
                        }

                        //Gerando configuracaoGrafico para series filhas
                        if (indicador.SeriesFilhas != null)
                        {
                            if (indicador.SeriesFilhas.Count > 0)
                            {
                                indicadorDTO.CorSerieFilha1 = indicador.SeriesFilhas[0].StrokeColor;
                                indicadorDTO.GrossuraSerieFilha1 = (int)indicador.SeriesFilhas[0].StrokeThickness;
                                indicadorDTO.TipoLinhaSerieFilha1 = indicador.SeriesFilhas[0].StrokePattern;
                            }

                            if (indicador.SeriesFilhas.Count > 1)
                            {
                                indicadorDTO.CorSerieFilha2 = indicador.SeriesFilhas[1].StrokeColor;
                                indicadorDTO.GrossuraSerieFilha2 = (int)indicador.SeriesFilhas[1].StrokeThickness;
                                indicadorDTO.TipoLinhaSerieFilha2 = indicador.SeriesFilhas[1].StrokePattern;
                            }
                        }

                        listaIndicadores.Add(indicadorDTO);
                    }
                }

                return listaIndicadores;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }



        #endregion GeraTemplateIndicadores


        #endregion

        #region Serie Auxiliar

        /// <summary>
        /// Metodo que insere na lista de series uma serie auxiliar
        /// </summary>
        public void AdicionaSerieAuxiliar(string nomeSerie, List<double> listaValores, List<DateTime> listaData)
        {
            SerieAuxiliarDTO serie = new SerieAuxiliarDTO();
            serie.Dados = listaValores;
            serie.Nome = nomeSerie;
            Series serieAux = stockChart.AddSeries(serie.Nome, 0);
            serieAux.SeriesType = EnumGeral.TipoSeriesEnum.Linha;
            for (int i = 0; i <= serie.Dados.Count - 1; i++)
            {
                stockChart.AppendValue(serie.Nome, listaData[i], listaValores[i]);
            }

            stockChart.Update();
        }

        #endregion

        #region Paineis

        #region RestauraStatusPaineis()
        /// <summary>
        /// Restaura o status dos paineis para normal.
        /// </summary>
        public void RestauraStatusPaineis()
        {
            try
            {
                ChartPanel painelMax = stockChart.MaximizedPanel;

                if (painelMax != null)
                {
                    painelMax.RestauraPainelMaximizado();
                    painelMax.State = ChartPanel.StateType.Normal;
                }

                foreach (ChartPanel painel in PaineisExistentes)
                {
                    if (painel.State == ChartPanel.StateType.Minimized)
                        if (painel.Series.Count > 0)
                            painel.RestauraPainelMinimizado();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion RestauraStatusPaineis()

        #endregion

        #region Zoom

        #region ZoomArea()
        /// <summary>
        /// Aplica zoom de área no gráfico.
        /// </summary>
        public void ZoomArea()
        {
            try
            {
                string nomeEstudo = "ZoomArea";
                LineStudy lineStudy = null;
                LineStudy lineStudyPainalIndicador = null;

                //Adicionando estudo
                lineStudy = stockChart.AddLineStudy(LineStudy.StudyTypeEnum.Rectangle, nomeEstudo, Brushes.Navy, "");
                lineStudyPainalIndicador = stockChartIndicadores.AddLineStudy(LineStudy.StudyTypeEnum.Rectangle, nomeEstudo, Brushes.Navy, "");

                //Setando grossura
                lineStudy.StrokeThickness = 1;
                lineStudyPainalIndicador.StrokeThickness = 1;

                //Tipo de linha
                lineStudy.StrokeType = EnumGeral.TipoLinha.Solido;
                lineStudyPainalIndicador.StrokeType = EnumGeral.TipoLinha.Solido;

                GradientStop g1 = new GradientStop();
                g1.Color = Colors.Yellow;
                g1.Offset = 0;

                GradientStop g2 = new GradientStop();
                g2.Color = Colors.LightGray;
                g2.Offset = 0.5;

                GradientStop g3 = new GradientStop();
                g3.Color = Colors.Blue;
                g3.Offset = 1;

                GradientStop g4 = new GradientStop();
                g4.Color = Colors.Yellow;
                g4.Offset = 0;

                GradientStop g5 = new GradientStop();
                g5.Color = Colors.LightGray;
                g5.Offset = 0.5;

                GradientStop g6 = new GradientStop();
                g6.Color = Colors.Blue;
                g6.Offset = 1;

                //Colocando gradiente dentro do retangulo de zoom
                ((IShapeAble)lineStudy).Fill = new LinearGradientBrush
                {
                    Opacity = 0.3,
                    StartPoint = new Point(0, 0.5),
                    EndPoint = new Point(1, 0.5),
                    GradientStops = new GradientStopCollection { g1, g2, g3 }
                };

                //Colocando gradiente dentro do retangulo de zoom
                ((IShapeAble)lineStudyPainalIndicador).Fill = new LinearGradientBrush
                {
                    Opacity = 0.3,
                    StartPoint = new Point(0, 0.5),
                    EndPoint = new Point(1, 0.5),
                    GradientStops = new GradientStopCollection { g4, g5, g6 }
                };


            }
            catch (Exception exc)
            {
                mensagemDialog = new MensagemDialog(exc.StackTrace);
                mensagemDialog.Show();
            }
        }
        #endregion ZoomArea()

        #region ZoomIn()
        /// <summary>
        /// Aplica zoom de aproximação no gráfico.
        /// </summary>
        /// <param name="records">Quantidade de records que dita o zoom.</param>
        public void ZoomIn(int records)
        {
            stockChart.ZoomIn(records);
        }
        #endregion ZoomIn()

        #region ZoomOut()
        /// <summary>
        /// Aplica zoom de aproximação no gráfico.
        /// </summary>
        /// <param name="records">Quantidade de records que dita o zoom.</param>
        public void ZoomOut(int records)
        {
            try
            {
                stockChart.ZoomOut(records);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion ZoomOut()

        #region FirstVisibleRecord

        public int GetFirstVisibleRecord()
        {
            return stockChart.FirstVisibleRecord;
        }

        public void SetFirstVisibleRecord(int record)
        {
            stockChart.FirstVisibleRecord = record;
        }

        #endregion

        #region LastVisibleRecord

        public int GetLastVisibleRecord()
        {
            return stockChart.LastVisibleRecord;
        }

        public void SetLastVisibleRecord(int record)
        {
            stockChart.LastVisibleRecord = record;
        }

        #endregion

        #endregion

        #region Atualizar Grafico

        /// <summary>
        /// Metodo deve atualizar o objeto gráfico e replotar o mesmo
        /// </summary>
        /// <param name="configuracaoGrafico"></param>
        public void AtualizaDados(ConfiguracaoGraficoDTO configuracaoGrafico)
        {
            this.configuracaoGraficoLocal = configuracaoGrafico;

            //TODO: como fazer para replotar com o novo objeto configuracao grafico
        }

        /// <summary>
        /// MEtodo que faz a atualização da ultima barra
        /// </summary>
        /// <param name="dataHora"></param>
        /// <param name="minimo"></param>
        /// <param name="maximo"></param>
        /// <param name="ultimo"></param>
        /// <param name="quantidade"></param>
        /// <param name="volume"></param>
        private void AtualizaUltimaBarra(DateTime dataHora, double minimo, double maximo, double ultimo, double quantidade, double volume,
            double variacaoDia, double variacaoUltimaBarra)
        {
            //Se a barra já existe atualiza              
            stockChart.EditValue(stockChart.Symbol + ".Ultimo", dataHora, ultimo);
            stockChartIndicadores.EditValue(stockChart.Symbol + ".Ultimo", dataHora, ultimo);
            if (maximo > (double)stockChart.GetValue(stockChart.Symbol + ".Maximo", dataHora))
            {
                stockChart.EditValue(stockChart.Symbol + ".Maximo", dataHora, maximo);
                stockChartIndicadores.EditValue(stockChart.Symbol + ".Maximo", dataHora, maximo);
            }

            if (minimo < (double)stockChart.GetValue(stockChart.Symbol + ".Minimo", dataHora))
            {
                stockChart.EditValue(stockChart.Symbol + ".Minimo", dataHora, minimo);
                stockChartIndicadores.EditValue(stockChart.Symbol + ".Minimo", dataHora, minimo);
            }
            stockChart.EditValue(stockChart.Symbol + ".Volume", dataHora, volume);
            stockChartIndicadores.EditValue(stockChart.Symbol + ".Volume", dataHora, volume);

            //Alterando a ultima variação do dia
            if (variacaoDia < 0)
                stockChart.GetSeriesByName("VariacaoDia").StrokeColor = Colors.Red;
            else
                stockChart.GetSeriesByName("VariacaoDia").StrokeColor = corVerde;

            stockChart.EditValue("VariacaoDia", dataHora, variacaoDia);
            stockChart.EditValue("Variacao", dataHora, variacaoUltimaBarra);


        }

        /// <summary>
        /// Metodo que faz a inserção de uma nova barra
        /// </summary>
        /// <param name="dataHora"></param>
        /// <param name="abertura"></param>
        /// <param name="minimo"></param>
        /// <param name="maximo"></param>
        /// <param name="ultimo"></param>
        /// <param name="quantidade"></param>
        /// <param name="volume"></param>
        private void InsereBarra(DateTime dataHora, double abertura, double minimo, double maximo, double ultimo, double quantidade,
            double volume, double variacaoDia, double variacaoUltimaBarra)
        {
            //Se ela não existe insiro uma nova barra
            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Abertura, dataHora, abertura);
            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Maximo, dataHora, maximo);
            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Minimo, dataHora, minimo);
            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Ultimo, dataHora, ultimo);
            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Volume, dataHora, volume);

            //Alterando a ultima variação do dia
            if (variacaoDia < 0)
                stockChart.GetSeriesByName("VariacaoDia").StrokeColor = Colors.Red;
            else
                stockChart.GetSeriesByName("VariacaoDia").StrokeColor = corVerde;

            stockChart.AppendValue("VariacaoDia", dataHora, variacaoDia);
            stockChart.AppendValue("Variacao", dataHora, variacaoUltimaBarra);


            stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Abertura, dataHora, abertura);
            stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Maximo, dataHora, maximo);
            stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Minimo, dataHora, minimo);
            stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Ultimo, dataHora, ultimo);
            stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Volume, dataHora, volume);

            //Adicionando no hub de controle
            listaDados.Add(new BarraDTO(stockChart.Symbol, dataHora, abertura, maximo, minimo, ultimo, volume, false));
        }

        private void AtualizaListaDadosRT(TickDTO tick, bool AtualizaInsereBarra)
        {
            DateTime horaTick = new DateTime(tick.Data.Year, tick.Data.Month, tick.Data.Day, Convert.ToInt32(tick.Hora.Substring(0, 2)),
                    Convert.ToInt32(tick.Hora.Substring(2, 2)), 0);

            //Identificando em qual janela vou inserir este tick
            for (int i = 0; i <= listaDadosRT.Count - 1; i++)
            {

                if ((horaTick >= listaDadosRT[i].HoraInicio) && (horaTick < listaDadosRT[i].HoraFinal))
                {
                    listaDadosRT[i].Abertura = tick.Abertura;
                    listaDadosRT[i].Maximo = tick.Maximo;
                    listaDadosRT[i].Minimo = tick.Minimo;
                    listaDadosRT[i].NumeroNegocio = tick.NumeroNegocio;
                    listaDadosRT[i].Quantidade = tick.Quantidade;
                    listaDadosRT[i].Ultimo = tick.Ultimo;
                    if (this.Periodicidade.Value == 1)
                        listaDadosRT[i].Volume = tick.VolumeMinuto;
                    else
                    {
                        listaDadosRT[i].Volume += tick.VolumeIncremento;
                    }

                    if (AtualizaInsereBarra)
                    {
                        if (listaDadosRT[i].Publicado)
                        {
                            AtualizaUltimaBarra(listaDadosRT[i].HoraInicio, listaDadosRT[i].Ultimo,
                                listaDadosRT[i].Ultimo, listaDadosRT[i].Ultimo, listaDadosRT[i].Quantidade,
                                listaDadosRT[i].Volume, 0, 0);

                        }
                        else
                        {
                            InsereBarra(listaDadosRT[i].HoraInicio, listaDadosRT[i].Ultimo, listaDadosRT[i].Ultimo,
                                listaDadosRT[i].Ultimo, listaDadosRT[i].Ultimo, listaDadosRT[i].Quantidade,
                                listaDadosRT[i].Volume, 0, 0);

                        }


                        break;
                    }
                    listaDadosRT[i].Publicado = true;
                }
            }
        }

        /// <summary>
        /// Metodo que irá retornar dada uma data a data normalizada da barra
        /// de acordo com a periodicidade
        /// </summary>
        /// <param name="periodicidade"></param>
        /// <param name="tickDate"></param>
        /// <returns></returns>
        private DateTime RetornaDataUltimaBarra(int periodicidade, DateTime tickDate)
        {
            try
            {
                //devo percorrer as datas minuto a minuto de agora até 30 dias atras até encontrar
                //alguma data cujo minuto seja mod 0
                for (DateTime i = tickDate; i >= DateTime.Today.Date; i = i.Subtract(new TimeSpan(0, 1, 0)))
                {
                    if (i.Minute % periodicidade == 0)
                        return i;
                }

                return DateTime.Today.Date;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public void AtualizaBarrasIntraday(TickDTO tick, double variacaoUltimaBarra)
        {
            DateTime? dataHoraultimaBarra = null;
            DateTime? dataHoraTick = null;
            double? volumeUltimaBarra = 0;

            try
            {


                //Resgatando a data hora da ultima barra no gráfico
                dataHoraultimaBarra = stockChart.GetTimestampByIndex(stockChart.RecordCount - 1);
                dataHoraTick = RetornaDataUltimaBarra(this.Periodicidade.Value, tick.Data);
                volumeUltimaBarra = stockChart.GetValue(this.Ativo + ".Volume", stockChart.RecordCount - 1);


                //Reprocessando a barra
                if (dataHoraultimaBarra == dataHoraTick)
                {
                    //Atualizo
                    if (this.periodicidade.Value == 1)
                        AtualizaUltimaBarra(dataHoraTick.Value, tick.Ultimo,
                                tick.Ultimo, tick.Ultimo, tick.Quantidade,
                                tick.VolumeMinuto, tick.Variacao, variacaoUltimaBarra);
                    else
                        AtualizaUltimaBarra(dataHoraTick.Value, tick.Ultimo,
                                tick.Ultimo, tick.Ultimo, tick.Quantidade,
                                tick.VolumeIncremento + volumeUltimaBarra.Value,
                                tick.Variacao, variacaoUltimaBarra);
                }
                else
                {
                    //Insiro
                    if (this.periodicidade.Value == 1)
                        InsereBarra(dataHoraTick.Value, tick.Ultimo, tick.Ultimo,
                                tick.Ultimo, tick.Ultimo, tick.Quantidade,
                                tick.VolumeMinuto, tick.Variacao, variacaoUltimaBarra);
                    else
                        InsereBarra(dataHoraTick.Value, tick.Ultimo, tick.Ultimo,
                                tick.Ultimo, tick.Ultimo, tick.Quantidade,
                                tick.VolumeIncremento, tick.Variacao, variacaoUltimaBarra);
                }

                //Atualizando
                stockChart.OptimizePainting = false;

                string a = stockChart.LabelTitle.Text;

                stockChart.Update();
                stockChartIndicadores.Update();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que atualiza os dados em tempo real para a periodicidade diaria
        /// </summary>
        /// <param name="dataHora"></param>
        /// <param name="ultimoPreco"></param>
        /// <param name="maximo"></param>
        /// <param name="minimo"></param>
        /// <param name="abertura"></param>
        /// <param name="volume"></param>
        public void AtualizaBarraDASEMANA(DateTime dataHora, double ultimoPreco, double maximo, double minimo, double abertura, double volume, double variacaoDia, double variacaoUltimaBarra)
        {
            try
            {
                DateTime dataDaBarra = DateTime.Today.Date;

                //Recuperando a data correta do inicio da semana
                for (DateTime i = dataHora.Date; i >= DateTime.Today.Subtract(new TimeSpan(8, 0, 0, 0)); i = i.Subtract(new TimeSpan(1, 0, 0, 0)))
                {
                    if (i.DayOfWeek == DayOfWeek.Monday)
                    {
                        dataDaBarra = i;
                        break;
                    }
                }


                //Verificando se a data é igual ou nao a ultima barra
                if (dataDaBarra == Dados[Dados.Count - 1].TradeDate.Date)
                {
                    //Se a barra já existe atualiza
                    stockChart.EditValue(stockChart.Symbol + ".Ultimo", dataDaBarra, ultimoPreco);
                    if (maximo > (double)stockChart.GetValue(stockChart.Symbol + ".Maximo", dataDaBarra))
                        stockChart.EditValue(stockChart.Symbol + ".Maximo", dataDaBarra, maximo);

                    if (minimo < (double)stockChart.GetValue(stockChart.Symbol + ".Minimo", dataDaBarra))
                        stockChart.EditValue(stockChart.Symbol + ".Minimo", dataDaBarra, minimo);

                    stockChart.EditValue(stockChart.Symbol + ".Volume", dataDaBarra, volume);

                    //Se a barra já existe atualiza
                    stockChartIndicadores.EditValue(stockChart.Symbol + ".Ultimo", dataDaBarra, ultimoPreco);
                    if (maximo > (double)stockChartIndicadores.GetValue(stockChart.Symbol + ".Maximo", dataDaBarra))
                        stockChartIndicadores.EditValue(stockChart.Symbol + ".Maximo", dataDaBarra, maximo);

                    if (minimo < (double)stockChartIndicadores.GetValue(stockChart.Symbol + ".Minimo", dataDaBarra))
                        stockChartIndicadores.EditValue(stockChart.Symbol + ".Minimo", dataDaBarra, minimo);

                    stockChartIndicadores.EditValue(stockChart.Symbol + ".Volume", dataDaBarra, volume);

                    //Alterando a ultima variação do dia
                    if (variacaoDia < 0)
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = Colors.Red;
                    else
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = corVerde;

                    stockChart.EditValue("VariacaoDia", dataDaBarra, variacaoDia);
                    stockChart.EditValue("Variacao", dataDaBarra, variacaoUltimaBarra);
                }
                else
                {
                    //Se ela não existe insiro uma nova barra
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Abertura, dataDaBarra, abertura);
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Maximo, dataDaBarra, maximo);
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Minimo, dataDaBarra, minimo);
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Ultimo, dataDaBarra, ultimoPreco);
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Volume, dataDaBarra, volume);

                    //Alterando a ultima variação do dia
                    if (variacaoDia < 0)
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = Colors.Red;
                    else
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = corVerde;

                    stockChart.AppendValue("VariacaoDia", dataDaBarra, variacaoDia);
                    stockChart.AppendValue("Variacao", dataDaBarra, variacaoUltimaBarra);

                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Abertura, dataDaBarra, abertura);
                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Maximo, dataDaBarra, maximo);
                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Minimo, dataDaBarra, minimo);
                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Ultimo, dataDaBarra, ultimoPreco);
                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Volume, dataDaBarra, volume);

                    //Adicionando no hub de controle
                    listaDados.Add(new BarraDTO(stockChart.Symbol, dataDaBarra, abertura, maximo, minimo, ultimoPreco, volume, false));

                }

                stockChart.Update();
                stockChartIndicadores.Update();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que atualiza os dados em tempo real para a periodicidade diaria
        /// </summary>
        /// <param name="dataHora"></param>
        /// <param name="ultimoPreco"></param>
        /// <param name="maximo"></param>
        /// <param name="minimo"></param>
        /// <param name="abertura"></param>
        /// <param name="volume"></param>
        public void AtualizaBarraDOMES(DateTime dataHora, double ultimoPreco, double maximo, double minimo, double abertura, double volume, double variacaoDia, double variacaoUltimaBarra)
        {
            try
            {
                DateTime dataDaBarra = DateTime.Today.Date;

                //Recuperando a data correta do inicio da semana
                for (DateTime i = dataHora.Date; i >= DateTime.Today.Subtract(new TimeSpan(35, 0, 0, 0)); i = i.Subtract(new TimeSpan(1, 0, 0, 0)))
                {
                    if (i.Day == 1)
                    {
                        dataDaBarra = i;
                        break;
                    }
                }


                //Verificando se a data é igual ou nao a ultima barra
                if (dataDaBarra == Dados[Dados.Count - 1].TradeDate.Date)
                {
                    //Se a barra já existe atualiza
                    stockChart.EditValue(stockChart.Symbol + ".Ultimo", dataDaBarra, ultimoPreco);
                    if (maximo > (double)stockChart.GetValue(stockChart.Symbol + ".Maximo", dataDaBarra))
                        stockChart.EditValue(stockChart.Symbol + ".Maximo", dataDaBarra, maximo);

                    if (minimo < (double)stockChart.GetValue(stockChart.Symbol + ".Minimo", dataDaBarra))
                        stockChart.EditValue(stockChart.Symbol + ".Minimo", dataDaBarra, minimo);

                    stockChart.EditValue(stockChart.Symbol + ".Volume", dataDaBarra, volume);

                    //Se a barra já existe atualiza
                    stockChartIndicadores.EditValue(stockChart.Symbol + ".Ultimo", dataDaBarra, ultimoPreco);
                    if (maximo > (double)stockChartIndicadores.GetValue(stockChart.Symbol + ".Maximo", dataDaBarra))
                        stockChartIndicadores.EditValue(stockChart.Symbol + ".Maximo", dataDaBarra, maximo);

                    if (minimo < (double)stockChartIndicadores.GetValue(stockChart.Symbol + ".Minimo", dataDaBarra))
                        stockChartIndicadores.EditValue(stockChart.Symbol + ".Minimo", dataDaBarra, minimo);

                    stockChartIndicadores.EditValue(stockChart.Symbol + ".Volume", dataDaBarra, volume);

                    //Alterando a ultima variação do dia
                    if (variacaoDia < 0)
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = Colors.Red;
                    else
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = corVerde;

                    stockChart.EditValue("VariacaoDia", dataDaBarra, variacaoDia);
                    stockChart.EditValue("Variacao", dataDaBarra, variacaoUltimaBarra);
                }
                else
                {
                    //Se ela não existe insiro uma nova barra
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Abertura, dataDaBarra, abertura);
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Maximo, dataDaBarra, maximo);
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Minimo, dataDaBarra, minimo);
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Ultimo, dataDaBarra, ultimoPreco);
                    stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Volume, dataDaBarra, volume);

                    //Alterando a ultima variação do dia
                    if (variacaoDia < 0)
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = Colors.Red;
                    else
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = corVerde;

                    stockChart.AppendValue("VariacaoDia", dataDaBarra, variacaoDia);
                    stockChart.AppendValue("Variacao", dataDaBarra, variacaoUltimaBarra);

                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Abertura, dataDaBarra, abertura);
                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Maximo, dataDaBarra, maximo);
                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Minimo, dataDaBarra, minimo);
                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Ultimo, dataDaBarra, ultimoPreco);
                    stockChartIndicadores.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Volume, dataDaBarra, volume);

                    //Adicionando no hub de controle
                    listaDados.Add(new BarraDTO(stockChart.Symbol, dataDaBarra, abertura, maximo, minimo, ultimoPreco, volume, false));

                }

                stockChart.Update();
                stockChartIndicadores.Update();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que atualiza os dados em tempo real para a periodicidade diaria
        /// </summary>
        /// <param name="dataHora"></param>
        /// <param name="ultimoPreco"></param>
        /// <param name="maximo"></param>
        /// <param name="minimo"></param>
        /// <param name="abertura"></param>
        /// <param name="volume"></param>
        public void AtualizaBarraDiaria(DateTime dataHora, double ultimoPreco, double maximo, double minimo, double abertura, double volume, double variacaoDia, double variacaoUltimaBarra)
        {
            try
            {

                dataHora = dataHora.Date;
                if (dataHora.Date == Dados[Dados.Count - 1].TradeDate.Date)
                {
                    //Se a barra já existe atualiza
                    stockChart.EditValue(stockChart.Symbol + ".Ultimo", dataHora, ultimoPreco);

                    if (stockChart.GetSeriesByName(stockChart.Symbol + ".Maximo") != null)
                        if (maximo > (double)stockChart.GetValue(stockChart.Symbol + ".Maximo", dataHora))
                            stockChart.EditValue(stockChart.Symbol + ".Maximo", dataHora, maximo);

                    if (stockChart.GetSeriesByName(stockChart.Symbol + ".Minimo") != null)
                        if (minimo < (double)stockChart.GetValue(stockChart.Symbol + ".Minimo", dataHora))
                            stockChart.EditValue(stockChart.Symbol + ".Minimo", dataHora, minimo);

                    stockChart.EditValue(stockChart.Symbol + ".Volume", dataHora, volume);

                    
                    stockChart.EditValue("Variacao", dataHora, variacaoUltimaBarra);

                    //Alterando a ultima variação do dia
                    if (variacaoDia < 0)
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = Colors.Red;
                    else
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = corVerde;

                    stockChart.EditValue("VariacaoDia", dataHora, variacaoDia);

                }
                else
                {
                    if (this.configuracaoGraficoLocal.EstiloBarra != EnumGeral.TipoSeriesEnum.Linha)
                    {
                        //Se ela não existe insiro uma nova barra
                        stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Abertura, dataHora, abertura);
                        stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Maximo, dataHora, maximo);
                        stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Minimo, dataHora, minimo);
                        stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Ultimo, dataHora, ultimoPreco);
                        stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Volume, dataHora, volume);
                    }
                    else
                    {
                        stockChart.AppendValue(stockChart.Symbol + ".Volume", dataHora, volume);
                        stockChart.AppendValue(stockChart.Symbol + ".Ultimo", dataHora, ultimoPreco);
                    }

                    stockChart.AppendValue("Variacao", dataHora, variacaoUltimaBarra);

                    if (variacaoDia < 0)
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = Colors.Red;
                    else
                        stockChart.GetSeriesByName("VariacaoDia").StrokeColor = corVerde;

                    stockChart.AppendValue("VariacaoDia", dataHora, variacaoDia);

                    
                    //Adicionando no hub de controle
                    listaDados.Add(new BarraDTO(stockChart.Symbol, dataHora, abertura, maximo, minimo, ultimoPreco, volume, false));

                }

                stockChart.Update();
                
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="dataHora"></param>
        ///// <param name="ultimoPreco"></param>
        ///// <param name="maximo"></param>
        ///// <param name="minimo"></param>
        ///// <param name="abertura"></param>
        ///// <param name="volume"></param>
        //public void AtualizaBarraSemanalEMensal(DateTime dataHora, double ultimoPreco, double maximo, double minimo, double abertura, double volume, double variacaoDia)
        //{
        //    try
        //    {
        //        if (dataHora == Dados[Dados.Count - 1].TradeDate)
        //        {
        //            //Se a barra já existe atualiza
        //            stockChart.EditValue(stockChart.Symbol + ".Ultimo", dataHora, ultimoPreco);

        //            if (maximo > (double)stockChart.GetValue(stockChart.Symbol + ".Maximo", dataHora))
        //                stockChart.EditValue(stockChart.Symbol + ".Maximo", dataHora, maximo);

        //            if (minimo < (double)stockChart.GetValue(stockChart.Symbol + ".Minimo", dataHora))
        //                stockChart.EditValue(stockChart.Symbol + ".Minimo", dataHora, minimo);

        //            if (Dados[Dados.Count - 1].BarraBanco)
        //            {
        //                //Neste caso eu devo somar o volume do dia ao volume já presente da barra
        //                stockChart.EditValue(stockChart.Symbol + ".Volume", dataHora, volume + Dados[Dados.Count - 1].Volume);
        //            }
        //            else
        //            {
        //                //Neste caso a barra foi inserida pelo real-time, significando que é inicio de período, logo nao
        //                //é necessário somar o volume, basta substituir
        //                //Neste caso eu devo somar o volume do dia ao volume já presente da barra
        //                stockChart.EditValue(stockChart.Symbol + ".Volume", dataHora, volume);
        //            }
        //        }
        //        else
        //        {
        //            //Se ela não existe insiro uma nova barra
        //            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Abertura, dataHora, abertura);
        //            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Maximo, dataHora, maximo);
        //            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Minimo, dataHora, minimo);
        //            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Ultimo, dataHora, ultimoPreco);
        //            stockChart.AppendValue(stockChart.Symbol, EnumGeral.TipoSerieOHLC.Volume, dataHora, volume);

        //            //Adicionando no hub de controle
        //            listaDados.Add(new BarraDTO(stockChart.Symbol, dataHora, abertura, maximo, minimo, ultimoPreco, volume, false));

        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        throw exc;
        //    }
        //}

        #endregion

        #region Darvas

        /// <summary>
        /// Metodo que seta true ou false para o darvabox
        /// </summary>
        /// <param name="darvaBox"></param>
        private void SetDarvaBox(bool darvaBox)
        {
            try
            {
                if (darvaBox == true)
                {
                    if (!stockChart.DarvasBoxes)
                    {
                        ConfiguraDarvaBox darvConfig = new ConfiguraDarvaBox();
                        darvConfig.Closing += (sender1, e1) =>
                        {
                            if (darvConfig.DialogResult == true)
                            {
                                stockChart.DarvasStopPercent = darvConfig.Percentual;

                                if (!stockChart.DarvasBoxes)
                                    stockChart.DarvasBoxes = true;
                            }
                            else
                            {
                                if (stockChart.DarvasBoxes)
                                    stockChart.DarvasBoxes = false;
                            }
                        };
                        darvConfig.Show();
                    }
                }
                else if (stockChart.DarvasBoxes)
                    stockChart.DarvasBoxes = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        #endregion

        #region HeatPanel

        /// <summary>
        /// Metodo que deve habilitar ou desabilitar o heatpanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeatPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            stockChart.AddHeatMapPanel();
        }

        #endregion

        #region Refresh

        #region  RefreshIndicadores

        private void RefreshTamanho()
        {
            try
            {
                if ((PainelPrincipal != null) && (PainelVolume != null))
                {
                    //Setando altura
                    switch (this.configuracaoGraficoLocal.TipoVolume)
                    {
                        case "N":
                            break;
                        case "F":
                            stockChart.SetPanelHeight(PainelPrincipal.Index, AlturaPainelPreco);
                            stockChart.SetPanelHeight(PainelVolume.Index, AlturaPainelVolume);
                            break;
                    }
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public void ResetaAlturaPainel()
        {
            try
            {
                if (PainelPrincipal != null)
                    this.AlturaPainelPreco = PainelPrincipal.ActualHeight;
                if (PainelVolume != null)
                    this.AlturaPainelVolume = PainelVolume.ActualHeight;
                //this.listaIndicadores = this.RetornaIndicadoresPresentesNoGrafico();

            }
            catch (Exception exc)
            {
                throw exc;
            }

        }

        /// <summary>
        /// Metodo limpa os indicadores plotados e recoloca os indicadores de acordo com a configuração
        /// </summary>
        private void RefreshIndicadores()
        {
            int countPainelIndicadores = 0;
            ChartPanel painel = null;
            List<int> listaIndiceAntigo = new List<int>();
            List<int> listaIndiceNovo = new List<int>();

            try
            {

                foreach (IndicadorDTO indicador in this.Indicadores)
                {
                    if (indicador.PainelIndicadoresLateral)
                        countPainelIndicadores++;
                }

                //Verificando se tem algum no painel de indicadores
                if (countPainelIndicadores == 0)
                {
                    //Inserindo o DIDI
                    IndicadorDTO indicador = new IndicadorDTO();
                    indicador.AlturaPainel = 100;
                    indicador.CorAlta = Colors.Blue;
                    indicador.CorBaixa = Colors.Blue;
                    indicador.CorSerieFilha1 = Colors.Red;
                    indicador.CorSerieFilha2 = Colors.White;
                    indicador.Grossura = 1;
                    indicador.GrossuraSerieFilha1 = 1;
                    indicador.GrossuraSerieFilha2 = 1;
                    indicador.PainelIndicadoresLateral = true;
                    indicador.Parametros = "PETR4.Ultimo";
                    indicador.TipoIndicador = 65;
                    indicador.TipoLinha = EnumGeral.TipoLinha.Solido;
                    indicador.TipoLinhaSerieFilha1 = EnumGeral.TipoLinha.Solido;
                    indicador.TipoLinhaSerieFilha2 = EnumGeral.TipoLinha.Solido;
                    indicador.IndexPainel = 2; //Colocando valor enorme para garantir a inserção
                    this.Indicadores.Add(indicador);

                    indicador = new IndicadorDTO();
                    indicador.AlturaPainel = 200;
                    indicador.CorAlta = Colors.White;
                    indicador.CorBaixa = Colors.White;
                    indicador.CorSerieFilha1 = Colors.Red;
                    indicador.CorSerieFilha2 = Colors.White;
                    indicador.Grossura = 1;
                    indicador.GrossuraSerieFilha1 = 1;
                    indicador.GrossuraSerieFilha2 = 1;
                    indicador.PainelIndicadoresLateral = true;
                    indicador.Parametros = "PETR4.Ultimo;14";
                    indicador.TipoIndicador = 54;
                    indicador.TipoLinha = EnumGeral.TipoLinha.Solido;
                    indicador.TipoLinhaSerieFilha1 = EnumGeral.TipoLinha.Solido;
                    indicador.TipoLinhaSerieFilha2 = EnumGeral.TipoLinha.Solido;
                    indicador.IndexPainel = 3; //Colocando valor enorme para garantir a inserção
                    this.Indicadores.Add(indicador);

                    //MACD
                    indicador = new IndicadorDTO();
                    indicador.AlturaPainel = 300;
                    indicador.CorAlta = Colors.White;
                    indicador.CorBaixa = Colors.White;
                    indicador.CorSerieFilha1 = Colors.Red;
                    indicador.CorSerieFilha2 = Colors.White;
                    indicador.Grossura = 1;
                    indicador.GrossuraSerieFilha1 = 1;
                    indicador.GrossuraSerieFilha2 = 1;
                    indicador.PainelIndicadoresLateral = true;
                    indicador.Parametros = "PETR4;26;13;9";
                    indicador.TipoIndicador = 62;
                    indicador.TipoLinha = EnumGeral.TipoLinha.Solido;
                    indicador.TipoLinhaSerieFilha1 = EnumGeral.TipoLinha.Solido;
                    indicador.TipoLinhaSerieFilha2 = EnumGeral.TipoLinha.Solido;
                    indicador.IndexPainel = 4; //Colocando valor enorme para garantir a inserção
                    this.Indicadores.Add(indicador);


                    //TRIX
                    indicador = new IndicadorDTO();
                    indicador.AlturaPainel = 300;
                    indicador.CorAlta = Colors.White;
                    indicador.CorBaixa = Colors.White;
                    indicador.CorSerieFilha1 = Colors.Red;
                    indicador.CorSerieFilha2 = Colors.White;
                    indicador.Grossura = 1;
                    indicador.GrossuraSerieFilha1 = 1;
                    indicador.GrossuraSerieFilha2 = 1;
                    indicador.PainelIndicadoresLateral = true;
                    indicador.Parametros = "PETR4.Ultimo;9";
                    indicador.TipoIndicador = 14;
                    indicador.TipoLinha = EnumGeral.TipoLinha.Solido;
                    indicador.TipoLinhaSerieFilha1 = EnumGeral.TipoLinha.Solido;
                    indicador.TipoLinhaSerieFilha2 = EnumGeral.TipoLinha.Solido;
                    indicador.IndexPainel = 4; //Colocando valor enorme para garantir a inserção
                    this.Indicadores.Add(indicador);
                }

                //Inserindo indicadores novamente
                foreach (IndicadorDTO indicador in this.Indicadores)
                {
                    painel = null;
                    if (indicador.PainelIndicadoresLateral)
                    {
                        painel = stockChartIndicadores.GetPanelByIndex(indicador.IndexPainel) ?? stockChartIndicadores.AddChartPanel();
                        painel.MinimizeBox = true;
                        painel.MaximizeBox = true;
                        painel.CloseBox = true;

                    }
                    else
                    {
                        if ((indicador.PainelPreco) || (indicador.PainelVolume))
                        {
                            painel = stockChart.GetPanelByIndex(indicador.IndexPainel);
                        }
                        else
                        {
                            //Nesse caso nao se trata de um indicador colocado sobre o painel de preços
                            //nem o painel de volumes, então somente resta a opção de ser um novo painel
                            //ou um indicador presente sobre um outro indicador já presente
                            //devo verificar se o indicador.IndexPainel (que é o index do painel antes da atualização)
                            //consta na lista de index antigos, se nao constar devo inserir novo e colocar na lista
                            bool jaExistepainel = false;
                            for (int i = 0; i <= listaIndiceAntigo.Count - 1; i++)
                            {
                                if (listaIndiceAntigo[i] == indicador.IndexPainel)
                                {
                                    painel = stockChart.GetPanelByIndex(listaIndiceNovo[i]);
                                    jaExistepainel = true;
                                }
                            }

                            if (!jaExistepainel)
                            {
                                listaIndiceAntigo.Add(indicador.IndexPainel);
                                painel = stockChart.AddChartPanel();
                                listaIndiceNovo.Add(painel.Index);
                            }


                            painel.MinimizeBox = true;
                            painel.MaximizeBox = true;
                            painel.CloseBox = true;

                        }

                    }
                    EnumGeral.IndicatorType tipoIndicador = (EnumGeral.IndicatorType)indicador.TipoIndicador;

                    Indicator indicadorCriado = null;
                    int indicadorCount = -1;
                    string nome = "";

                    if (!indicador.PainelIndicadoresLateral)
                    {
                        //Montando o nome do indicador
                        indicadorCount = stockChart.GetIndicatorCountByType(tipoIndicador);
                        nome = tipoIndicador.ToString() + (indicadorCount > 0 ? indicadorCount.ToString() : "");
                        indicadorCriado = stockChart.AddIndicator(tipoIndicador, nome, painel, false);
                    }
                    else
                    {
                        //Montando o nome do indicador
                        indicadorCount = stockChartIndicadores.GetIndicatorCountByType(tipoIndicador);
                        nome = tipoIndicador.ToString() + (indicadorCount > 0 ? indicadorCount.ToString() : "");
                        indicadorCriado = stockChartIndicadores.AddIndicator(tipoIndicador, nome, painel, false);
                    }

                    indicadorCriado.PainelIndicadoresAbaixo = indicador.PainelIndicadoresAbaixo;
                    indicadorCriado.PainelIndicadoresLateral = indicador.PainelIndicadoresLateral;
                    indicadorCriado.PainelPreco = indicador.PainelPreco;
                    indicadorCriado.PainelVolume = indicador.PainelVolume;

                    #region Configurações Gerais

                    if (indicadorCriado.PainelIndicadoresLateral)
                    {
                        //Configuracoes do painel
                        painel.MaximizeBox = true;
                        painel.MinimizeBox = true;
                        painel.CloseBox = true;
                    }

                    //Caracteristicas do indicador                    
                    indicadorCriado.UpColor = indicador.CorAlta;
                    indicadorCriado.DownColor = indicador.CorAlta;
                    indicadorCriado.StrokeColor = indicador.CorAlta;
                    indicadorCriado.TitleBrush = new SolidColorBrush(indicador.CorAlta);
                    indicadorCriado.StrokeThickness = indicador.Grossura;
                    indicadorCriado.StrokePattern = (EnumGeral.TipoLinha)indicador.TipoLinha;

                    //Setando as caracteristicas da serie filha 1
                    indicadorCriado.CorSerieFilha1 = indicador.CorSerieFilha1;
                    indicadorCriado.GrossuraSerieFilha1 = indicador.GrossuraSerieFilha1;
                    indicadorCriado.TipoLinhaSerieFilha1 = indicador.TipoLinhaSerieFilha1;

                    //Setando as caracteristicas da serie filha 2
                    indicadorCriado.CorSerieFilha2 = indicador.CorSerieFilha2;
                    indicadorCriado.GrossuraSerieFilha2 = indicador.GrossuraSerieFilha2;
                    indicadorCriado.TipoLinhaSerieFilha2 = indicador.TipoLinhaSerieFilha2;

                    if (indicadorCriado.PainelIndicadoresLateral)
                    {
                        //Outras configurações
                        indicadorCriado._chartPanel.Height = indicador.AlturaPainel;
                        //indicadorCriado._chartPanel.State = (ChartPanel.StateType)indicador.StatusPainel;
                    }

                    #endregion

                    #region Parametros

                    //Setando parametros
                    string[] parametros = indicador.Parametros.Split(';');

                    int index = 0;
                    foreach (object parametro in parametros)
                    {
                        switch (indicadorCriado.Parameters[index].ParameterType)
                        {
                            case EnumGeral.TipoParametroIndicador.Ativo:
                                indicadorCriado.SetParameterValue(index, this.Ativo);
                                break;

                            case EnumGeral.TipoParametroIndicador.Serie:
                            case EnumGeral.TipoParametroIndicador.Serie1:
                            case EnumGeral.TipoParametroIndicador.Serie2:
                            case EnumGeral.TipoParametroIndicador.Serie3:

                                if (parametro.ToString().Contains(".Ultimo") || parametro.ToString().Contains(".Abertura") ||
                                    parametro.ToString().Contains(".Maximo") || parametro.ToString().Contains(".Minimo"))
                                {
                                    string[] aux = parametro.ToString().Split('.');

                                    indicadorCriado.SetParameterValue(index, this.Ativo + "." + aux[1]);
                                }
                                else
                                    indicadorCriado.SetParameterValue(index, parametro);
                                break;

                            case EnumGeral.TipoParametroIndicador.Volume:
                                indicadorCriado.SetParameterValue(index, this.Ativo + ".Volume");
                                break;

                            default:
                                indicadorCriado.SetParameterValue(index, parametro);
                                break;
                        }

                        index++;
                    }


                    #endregion Parametros




                }

                stockChartIndicadores.ResizePanels(PanelsContainer.ResizeType.EvenSemPreco);
                stockChart.ResizePanels(PanelsContainer.ResizeType.Proportional);

                stockChart.Update();
                stockChartIndicadores.Update();



            }
            catch (Exception exc)
            {
                throw exc;
            }
        }



        #endregion

        #region  RefreshObjetos

        /// <summary>
        /// Metodo limpa os objetos plotados e adiciona os objetos que estão no objeto do gráfico
        /// </summary>
        private void RefreshObjetos()
        {
            try
            {
                //Apagando todos os objetos do gráfico
                ExcluiTodosObjetos();

                //Inserindo objetos novamente
                foreach (ObjetoEstudoDTO objeto in this.Objetos)
                {
                    string nomeEstudo = ((LineStudy.StudyTypeEnum)objeto.TipoObjeto).ToString() + stockChart.GetLineStudyCountByType((LineStudy.StudyTypeEnum)objeto.TipoObjeto).ToString();
                    LineStudy novaLinha = null;

                    if (!objeto.PainelIndicadores)
                        novaLinha = stockChart.CreateLineStudy((LineStudy.StudyTypeEnum)objeto.TipoObjeto, nomeEstudo, objeto.Cor, objeto.IndexPainel);
                    else
                        novaLinha = stockChartIndicadores.CreateLineStudy((LineStudy.StudyTypeEnum)objeto.TipoObjeto, nomeEstudo, objeto.Cor, objeto.IndexPainel);

                    //Propriedades padrões
                    novaLinha.StrokeThickness = objeto.Grossura;
                    novaLinha.StrokeType = objeto.TipoLinha;
                    //novaLinha.LinhaInfinitaADireita = objeto.InfinitaADireita;
                    novaLinha.LinhaMagnetica = objeto.Magnetica;

                    //Setando datas originais para efeito de controle de edicao do objeto
                    novaLinha.DataOriginalInicial = objeto.DataInicial;
                    novaLinha.DataOriginalFinal = objeto.DataFinal;

                    //Ajustando posicao do objeto
                    AjustaPosicaoObjeto(novaLinha, objeto.DataInicial, objeto.DataFinal, objeto.ValorInicial, objeto.ValorFinal, periodicidade.Value);

                    stockChart.Update();
                    novaLinha.LinhaInfinitaADireita = objeto.InfinitaADireita;
                    stockChart.Update();

                    if (objeto.Parametros != "")
                        novaLinha.SetaParametros(objeto.Parametros, ';');
                }

                //Atualizando a lista de objetos
                this.listaObjetos = this.RetornaObjetosPresentesNoGrafico();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion

        #region  RefreshDados

        /// <summary>
        /// Metodo reaplica os dados
        /// </summary>
        private void RefreshDados()
        {
            try
            {

                
                //aplicando o template
                stockChart.ApplyTemplate();

                //Zerando gráfico
                stockChart.Freeze();
                stockChart.ClearAll();
                stockChart.Background = new SolidColorBrush(Colors.Black);

                //Criando painéis padrões
                ChartPanel topPanel = stockChart.AddChartPanel();
                topPanel.MinimizeBox = false;
                topPanel.MaximizeBox = false;
                topPanel.CloseBox = false;

                ChartPanel volumePanel = stockChart.AddChartPanel(ChartPanel.PositionType.AlwaysBottom);
                volumePanel.CanResizeEscala = false;
                volumePanel.MinimizeBox = false;
                volumePanel.MaximizeBox = false;
                volumePanel.CloseBox = false;

                switch (this.configuracaoGraficoLocal.TipoVolume)
                {
                    case "N":
                        volumePanel.Visible = false;
                        //cmbTipoVolume.SelectedIndex = 1;
                        break;
                    case "F":
                        volumePanel.Visible = true;
                        //cmbTipoVolume.SelectedIndex = 0;
                        break;
                    default:
                        volumePanel.Visible = true;
                        //cmbTipoVolume.SelectedIndex = 0;
                        break;
                }


                //Setando background
                topPanel.Background = new SolidColorBrush(Colors.Black);
                volumePanel.Background = new SolidColorBrush(Colors.Black);

                topPanel.Visible = true;

                //Setando simbolo interno do stockchart
                stockChart.Symbol = Ativo;

                //Criando Séries
                Series[] ohlcSeries = stockChart.AddOHLCSeries(Ativo, topPanel.Index);
                Series seriesVolume = stockChart.AddVolumeSeries(Ativo, volumePanel.Index);
                Series seriesVaricaonaBarra = stockChart.AddSeries("Variacao", topPanel.Index);
                Series serieVaricaoDia = stockChart.AddSeries("VariacaoDia", topPanel.Index);

                
                //configurando a seriesVariacao
                seriesVaricaonaBarra.SetVisibleTitleBar(false);
                seriesVaricaonaBarra.SetSufixoInfoPanel("%");
                serieVaricaoDia.SetVisibleTitleBar(true);
                serieVaricaoDia.SetTitleBarCaption("Variação");
                serieVaricaoDia.SetTitleBarCaptionComplemento("%");
                serieVaricaoDia.SetVisibleInfoPanel(false);

                foreach (Series obj in ohlcSeries)
                {
                    obj.Visible = true;
                }


                //Percorrendo a lista Data para plotá-las no gráfico
                double variacao = 0;
                double ultimoValor = 0;
                foreach (BarraDTO bd in this.Dados)
                {
                    stockChart.AppendOHLCValues(stockChart.Symbol, bd.TradeDate, bd.OpenPrice, bd.HighPrice, bd.LowPrice, bd.ClosePrice);
                    stockChart.AppendVolumeValue(stockChart.Symbol, bd.TradeDate, bd.Volume);

                    if (ultimoValor == 0)
                        variacao = 0;
                    else
                        variacao = ((bd.ClosePrice - ultimoValor) / ultimoValor) * 100;

                    stockChart.AppendValue("Variacao", bd.TradeDate, variacao);
                    ultimoValor = bd.ClosePrice;

                    stockChart.AppendValue("VariacaoDia", bd.TradeDate, 0);
                }


                //Setando cores e grossuras das séries
                ohlcSeries[0].StrokeColor = ColorsEx.Lime;
                ohlcSeries[0].Selectable = false;
                ohlcSeries[0].StrokeThickness = 1;
                ohlcSeries[1].StrokeColor = ColorsEx.Lime;
                ohlcSeries[1].Selectable = false;
                ohlcSeries[1].StrokeThickness = 1;
                ohlcSeries[2].StrokeColor = ColorsEx.Lime;
                ohlcSeries[2].Selectable = false;
                ohlcSeries[2].StrokeThickness = 1;
                ohlcSeries[3].StrokeColor = ColorsEx.Lime;
                ohlcSeries[3].Selectable = false;
                ohlcSeries[3].StrokeThickness = 1;

               
                
                seriesVolume.StrokeColor = ColorsEx.Lime;
                seriesVolume.StrokeThickness = 1;
                seriesVolume.Selectable = false;

                //Setando o tamanho do painel de precos
                stockChart.SetPanelHeight(0, stockChart.ActualHeight * 0.75);

                //Outras configurações ??
                stockChart.GradeCor = new SolidColorBrush(Color.FromArgb(0x33, 0xCC, 0xCC, 0xCC));
                stockChart.Melt();

                //desligando a optimização do gráfico -- esta alteração foi feita a pedido do pessoal da modulus
                stockChart.OptimizePainting = false;

                //Mostrando labels de datas em tempo real ou n
                if (TipoPeriodicidade == EnumGeral.TipoPeriodicidade.Intraday)
                    stockChart.LabelsEixoXRealTime = true;
                else
                    stockChart.LabelsEixoXRealTime = false;

                //stockChart.TickBox = TickBoxPosition.Right;

                //Setando cor do info panel
                stockChart.InfoPanelCorFonteLabels = new SolidColorBrush(Colors.White);
                stockChart.InfoPanelCorFonteValores = new SolidColorBrush(Colors.White);
                stockChart.InfoPanelCorFundoLabels = GradienteInfoPanel();
                stockChart.InfoPanelCorFundoValores = GradienteInfoPanel();
                stockChart.InfoPanelTamanhoFonte = 9;
                stockChart.InfoPanelFonte = new FontFamily("Verdana");


                


                #region Atualizando a lista RT
                lock (listaDadosRT)
                {
                    if (listaDadosRT.Count == 0)
                    {
                        listaDadosRT.Clear();
                        for (DateTime j = DateTime.Today.Subtract(new TimeSpan(this.Periodo.Value)); j <= DateTime.Today.AddDays(1).Subtract(new TimeSpan(0, 0, 1)); j = j.AddMinutes(this.Periodicidade.Value))
                        {
                            BarraRTDTO barraAux = new BarraRTDTO();
                            barraAux.HoraInicio = j;
                            barraAux.HoraFinal = j.AddMinutes(this.Periodicidade.Value);
                            barraAux.Publicado = false;
                            listaDadosRT.Add(barraAux);
                        }
                    }


                    foreach (BarraDTO obj in Dados)
                    {
                        TickDTO tickAux = new TickDTO();
                        tickAux.Abertura = obj.OpenPrice;
                        tickAux.Data = obj.TradeDate;
                        tickAux.Hora = obj.TradeDate.ToString("HHmm");
                        tickAux.Maximo = obj.HighPrice;
                        tickAux.Minimo = obj.LowPrice;
                        tickAux.Ultimo = obj.ClosePrice;
                        tickAux.Volume = obj.Volume;

                        AtualizaListaDadosRT(tickAux, false);
                    }
                }

                #endregion


                seriesVaricaonaBarra.SeriesType = EnumGeral.TipoSeriesEnum.Desconhecida;
                serieVaricaoDia.SeriesType = EnumGeral.TipoSeriesEnum.Desconhecida;


            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo reaplica os dados
        /// </summary>
        public void RefreshDadosLinha()
        {
            try
            {


                //aplicando o template
                stockChart.ApplyTemplate();

                //Zerando gráfico
                stockChart.Freeze();
                stockChart.ClearAll();
                stockChart.Background = new SolidColorBrush(Colors.Black);

                //Criando painéis padrões
                ChartPanel topPanel = stockChart.AddChartPanel();
                topPanel.MinimizeBox = false;
                topPanel.MaximizeBox = false;
                topPanel.CloseBox = false;

                ChartPanel volumePanel = stockChart.AddChartPanel(ChartPanel.PositionType.AlwaysBottom);
                volumePanel.CanResizeEscala = false;
                volumePanel.MinimizeBox = false;
                volumePanel.MaximizeBox = false;
                volumePanel.CloseBox = false;

                switch (this.configuracaoGraficoLocal.TipoVolume)
                {
                    case "N":
                        volumePanel.Visible = false;
                        //cmbTipoVolume.SelectedIndex = 1;
                        break;
                    case "F":
                        volumePanel.Visible = true;
                        //cmbTipoVolume.SelectedIndex = 0;
                        break;
                    default:
                        volumePanel.Visible = true;
                        //cmbTipoVolume.SelectedIndex = 0;
                        break;
                }


                //Setando background
                topPanel.Background = new SolidColorBrush(Colors.Black);
                volumePanel.Background = new SolidColorBrush(Colors.Black);

                topPanel.Visible = true;

                //Setando simbolo interno do stockchart
                stockChart.Symbol = Ativo;

                //Criando Séries
                Series serieUltima = stockChart.AddSeries("Ultimo", topPanel.Index);
                Series seriesVolume = stockChart.AddVolumeSeries(Ativo, volumePanel.Index);
                Series seriesVaricaonaBarra = stockChart.AddSeries("Variacao", topPanel.Index);
                Series serieVaricaoDia = stockChart.AddSeries("VariacaoDia", topPanel.Index);


                //configurando a seriesVariacao
                seriesVaricaonaBarra.SetVisibleTitleBar(false);
                seriesVaricaonaBarra.SetSufixoInfoPanel("%");
                serieVaricaoDia.SetVisibleTitleBar(true);
                serieVaricaoDia.SetTitleBarCaption("Variação");
                serieVaricaoDia.SetTitleBarCaptionComplemento("%");
                serieVaricaoDia.SetVisibleInfoPanel(false);

                serieUltima.Visible = true;


                //Percorrendo a lista Data para plotá-las no gráfico
                double variacao = 0;
                double ultimoValor = 0;
                foreach (BarraDTO bd in this.Dados)
                {
                    //stockChart.AppendOHLCValues(stockChart.Symbol, bd.TradeDate, bd.OpenPrice, bd.HighPrice, bd.LowPrice, bd.ClosePrice);
                    stockChart.AppendValue("Ultimo", bd.TradeDate, bd.ClosePrice);
                    stockChart.AppendVolumeValue(stockChart.Symbol, bd.TradeDate, bd.Volume);

                    if (ultimoValor == 0)
                        variacao = 0;
                    else
                        variacao = ((bd.ClosePrice - ultimoValor) / ultimoValor) * 100;

                    stockChart.AppendValue("Variacao", bd.TradeDate, variacao);
                    ultimoValor = bd.ClosePrice;

                    stockChart.AppendValue("VariacaoDia", bd.TradeDate, 0);
                }


                //Setando cores e grossuras das séries
                serieUltima.StrokeColor = ColorsEx.Lime;
                serieUltima.Selectable = false;
                serieUltima.StrokeThickness = 1;
                
                seriesVolume.StrokeColor = ColorsEx.Lime;
                seriesVolume.StrokeThickness = 1;
                seriesVolume.Selectable = false;

                //Setando o tamanho do painel de precos
                stockChart.SetPanelHeight(0, stockChart.ActualHeight * 0.75);

                //Outras configurações ??
                stockChart.GradeCor = new SolidColorBrush(Color.FromArgb(0x33, 0xCC, 0xCC, 0xCC));
                stockChart.Melt();

                //desligando a optimização do gráfico -- esta alteração foi feita a pedido do pessoal da modulus
                stockChart.OptimizePainting = false;

                //Mostrando labels de datas em tempo real ou n
                if (TipoPeriodicidade == EnumGeral.TipoPeriodicidade.Intraday)
                    stockChart.LabelsEixoXRealTime = true;
                else
                    stockChart.LabelsEixoXRealTime = false;

                //stockChart.TickBox = TickBoxPosition.Right;

                //Setando cor do info panel
                stockChart.InfoPanelCorFonteLabels = new SolidColorBrush(Colors.White);
                stockChart.InfoPanelCorFonteValores = new SolidColorBrush(Colors.White);
                stockChart.InfoPanelCorFundoLabels = GradienteInfoPanel();
                stockChart.InfoPanelCorFundoValores = GradienteInfoPanel();
                stockChart.InfoPanelTamanhoFonte = 9;
                stockChart.InfoPanelFonte = new FontFamily("Verdana");





                #region Atualizando a lista RT
                lock (listaDadosRT)
                {
                    if (listaDadosRT.Count == 0)
                    {
                        listaDadosRT.Clear();
                        for (DateTime j = DateTime.Today.Subtract(new TimeSpan(this.Periodo.Value)); j <= DateTime.Today.AddDays(1).Subtract(new TimeSpan(0, 0, 1)); j = j.AddMinutes(this.Periodicidade.Value))
                        {
                            BarraRTDTO barraAux = new BarraRTDTO();
                            barraAux.HoraInicio = j;
                            barraAux.HoraFinal = j.AddMinutes(this.Periodicidade.Value);
                            barraAux.Publicado = false;
                            listaDadosRT.Add(barraAux);
                        }
                    }


                    foreach (BarraDTO obj in Dados)
                    {
                        TickDTO tickAux = new TickDTO();
                        tickAux.Abertura = obj.OpenPrice;
                        tickAux.Data = obj.TradeDate;
                        tickAux.Hora = obj.TradeDate.ToString("HHmm");
                        tickAux.Maximo = obj.HighPrice;
                        tickAux.Minimo = obj.LowPrice;
                        tickAux.Ultimo = obj.ClosePrice;
                        tickAux.Volume = obj.Volume;

                        AtualizaListaDadosRT(tickAux, false);
                    }
                }

                #endregion


                seriesVaricaonaBarra.SeriesType = EnumGeral.TipoSeriesEnum.Desconhecida;
                serieVaricaoDia.SeriesType = EnumGeral.TipoSeriesEnum.Desconhecida;


            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion

        #region RefreshLayout

        /// <summary>
        /// Metodo altera a configuração do gráfico de acordo com o objeto passado
        /// </summary>
        /// <param name="configuracaoGrafico"></param>
        public void RefreshLayout(bool GeraIndicadores)
        {
            try
            {

                stockChart.CandleUpOutlineColor = this.configuracaoGraficoLocal.CorBordaCandleAlta;
                stockChart.CandleDownOutlineColor = this.configuracaoGraficoLocal.CorBordaCandleBaixa;
                stockChart.CandleCorAlta = this.configuracaoGraficoLocal.CorCandleAlta;
                stockChart.CandleCorBaixa = this.configuracaoGraficoLocal.CorCandleBaixa;
                stockChart.EscalaTipo = this.configuracaoGraficoLocal.TipoEscala;
                stockChart.EscalaPrecisao = this.configuracaoGraficoLocal.PrecisaoEscala;

                //Comentando a linha abaixo para força a esquerda
                stockChart.EscalaAlinhamento = this.configuracaoGraficoLocal.PosicaoEscala;
                //stockChart.EscalaAlinhamento = EnumGeral.TipoAlinhamentoEscalaEnum.Esquerda;

                stockChart.GradeX = this.configuracaoGraficoLocal.GradeVertical;
                stockChart.GradeY = this.configuracaoGraficoLocal.GradeHorizontal;


                if (stockChart.DarvasBoxes != this.configuracaoGraficoLocal.DarvaBox)
                    stockChart.DarvasBoxes = this.configuracaoGraficoLocal.DarvaBox;

                if (this.configuracaoGraficoLocal.PainelInfo)
                {
                    stockChart.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.Fixo;
                    stockChartIndicadores.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.Fixo;
                }
                else
                {
                    stockChart.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse;
                    stockChartIndicadores.InfoPanelPosicao = EnumGeral.InfoPanelPosicaoEnum.SeguindoMouse;
                }


                stockChart.PriceStyle = this.configuracaoGraficoLocal.EstiloPreco;

                stockChart.SetPriceStyleParam(0, (double)this.configuracaoGraficoLocal.EstiloPrecoParam1);
                stockChart.SetPriceStyleParam(1, (double)this.configuracaoGraficoLocal.EstiloPrecoParam2);

                Series serieUltimo = stockChart.GetSeriesByName(stockChart.Symbol + ".Ultimo");
                if (serieUltimo != null)
                    serieUltimo.SeriesType = this.configuracaoGraficoLocal.EstiloBarra;

                Series serieAbertura = stockChart.GetSeriesByName(stockChart.Symbol + ".Abertura");
                if (serieAbertura != null)
                    serieAbertura.SeriesType = this.configuracaoGraficoLocal.EstiloBarra;

                Series serieMaximo = stockChart.GetSeriesByName(stockChart.Symbol + ".Maximo");
                if (serieMaximo != null)
                    serieMaximo.SeriesType = this.configuracaoGraficoLocal.EstiloBarra;

                Series serieMinimo = stockChart.GetSeriesByName(stockChart.Symbol + ".Minimo");
                if (serieMinimo != null)
                    serieMinimo.SeriesType = this.configuracaoGraficoLocal.EstiloBarra;

                stockChart.EspacoDireitaGrafico = this.configuracaoGraficoLocal.EspacoADireitaGrafico;
                stockChart.UseVolumeUpDownColors = this.configuracaoGraficoLocal.UsarCoresAltaBaixaVolume;

                //Colocando o nome do ativo
                //lblAtivo.Text = this.Ativo;

                //Setando modos de linha de estudo
                usandoMagnetico = this.configuracaoGraficoLocal.LinhaMagnetica;
                usandoLinhaInfinita = this.configuracaoGraficoLocal.UsarLinhaInfinita;

                if (GeraIndicadores)
                    this.RefreshIndicadores();

                this.RefreshObjetos();

                this.RefreshTamanho();


                //pintando paineis
                foreach (ChartPanel obj in PaineisExistentes)
                {
                    obj.Background = this.configuracaoGraficoLocal.CorFundo;
                }
                foreach (ChartPanel obj in PaineisExistentesPainelIndicadores)
                {
                    obj.Background = this.configuracaoGraficoLocal.CorFundo;
                }


            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        void LabelTitle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            stockChart_MouseLeftButtonDown(sender, e);
        }



        void LabelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            StockChartMouseMove(e);
        }

        #endregion

        #region Refresh

        public void Refresh(EnumGeral.TipoRefresh tipoRefresh)
        {
            try
            {
                switch (tipoRefresh)
                {
                    case EnumGeral.TipoRefresh.SomenteUpdate:
                        stockChart.ManterNivelZoom = true;
                        break;
                    case EnumGeral.TipoRefresh.Layout:
                        this.RefreshLayout(true);
                        break;
                    case EnumGeral.TipoRefresh.LayoutSemIndicadores:
                        this.RefreshLayout(false);
                        break;
                    case EnumGeral.TipoRefresh.Dados:
                        this.RefreshDados();
                        break;
                    case EnumGeral.TipoRefresh.Tudo:
                        this.RefreshDados();                        
                        this.RefreshLayout(true);                        
                        break;
                    case EnumGeral.TipoRefresh.TudoLinha:
                        this.RefreshDadosLinha();
                        this.RefreshLayout(true);
                        break;
                    case EnumGeral.TipoRefresh.TudoMantemIndicadoresEObjetos:
                        //Armazenando os indicadores e objetos
                        List<IndicadorDTO> listaIndicadorAux = new List<IndicadorDTO>();

                        foreach (IndicadorDTO obj in this.RetornaIndicadoresPresentesNoGrafico())
                        {
                            listaIndicadorAux.Add(obj);
                        }

                        //TODO: Felipe, neste caso acho que devemos atualizar sua lista com os objetos presentes no grafico
                        List<ObjetoEstudoDTO> listaObjetoAux = new List<ObjetoEstudoDTO>();
                        foreach (ObjetoEstudoDTO obj in RetornaObjetosPresentesNoGrafico())
                        {
                            listaObjetoAux.Add(obj);
                        }



                        //Dando Refresh nos dados
                        this.RefreshDados();

                        //Resetando os indicadores e objetos
                        this.listaIndicadores = listaIndicadorAux;

                        this.listaObjetos = listaObjetoAux;

                        //Dando refresh no layout
                        this.RefreshLayout(true);
                        break;
                }

                stockChart.Update();

                //stockChartIndicadores = stockChart;

                stockChartIndicadores.Update();

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        #endregion

        #endregion

        #endregion

        #region  Configuração do Gráfico

        #region Problemas com Grafico de Linha

        public void SetEstiloLinha()
        {
            Series serieUltimo = stockChart.GetSeriesByName(stockChart.Symbol + ".Ultimo");
            if (serieUltimo != null)
                serieUltimo.SeriesType = this.configuracaoGraficoLocal.EstiloBarra;

            Series serieAbertura = stockChart.GetSeriesByName(stockChart.Symbol + ".Abertura");
            if (serieAbertura != null)
                serieAbertura.SeriesType = this.configuracaoGraficoLocal.EstiloBarra;

            Series serieMaximo = stockChart.GetSeriesByName(stockChart.Symbol + ".Maximo");
            if (serieMaximo != null)
                serieMaximo.SeriesType = this.configuracaoGraficoLocal.EstiloBarra;

            Series serieMinimo = stockChart.GetSeriesByName(stockChart.Symbol + ".Minimo");
            if (serieMinimo != null)
                serieMinimo.SeriesType = this.configuracaoGraficoLocal.EstiloBarra;
        }

        public void SetStockchartSymbol(string ativo)
        {
            this.stockChart.Symbol = ativo;
        }

        #endregion

        #region ConfiguracaoGeralGrafico
        /// <summary>
        /// Abre form de configuração do gráfico.
        /// </summary>
        /// <param name="grafSelec"></param>
        public void ConfiguracaoGeralGrafico()
        {
            //Chamar a tela de configuração passando o objeto grafico
            ConfiguraGrafico configuraGraficoDialog = new ConfiguraGrafico(configuracaoGraficoLocal);

            configuraGraficoDialog.Closing += (sender1, e1) =>
            {
                if (configuraGraficoDialog.DialogResult == true)
                {
                    //Seta a configuração local igual ao que foi alterado
                    this.configuracaoGraficoLocal = configuraGraficoDialog.ConfiguracaoGrafico;

                    //Refresh no layout do gráfico
                    this.RefreshLayout(false);
                }
            };


            configuraGraficoDialog.Show();
        }
        #endregion ConfiguraGrafico()


        #endregion

        #region Metodos Auxiliares

        #region Efeitos Visuais

        #region Windows Vista Gradiente

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

        #endregion

        #region GradienteInfoPanel()
        /// <summary>
        /// Cria gradiente do info panel.
        /// </summary>
        /// <returns></returns>
        private LinearGradientBrush GradienteInfoPanel()
        {
            //Criando BackGround para o stackPanel
            GradientStop gs1 = new GradientStop();
            gs1.Color = Color.FromArgb(155, 76, 76, 76);

            GradientStop gs2 = new GradientStop();
            gs2.Color = Color.FromArgb(155, 51, 53, 56);
            gs2.Offset = 1;

            GradientStop gs3 = new GradientStop();
            gs3.Color = Color.FromArgb(155, 60, 61, 63);
            gs3.Offset = 0.150;

            GradientStop gs4 = new GradientStop();
            gs4.Color = Color.FromArgb(155, 21, 21, 22);
            gs4.Offset = 0.100;

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

        #endregion GradienteInfoPanel()

        #endregion

        #region DesmarcaItensToolbarVertical()
        /// <summary>
        /// Desmarca itens da toolbar Vertical.
        /// </summary>
        private void DesmarcaItensToolbarVertical()
        {
            //Retirando cor do botão na toolbar vertical
            foreach (object obj in stpEstudos.Children)
            {
                ToggleButton botao = obj as ToggleButton;
                if ((botao != null) && (botao.Name != "btnMagnetic") && (botao.Name != "btnSeta") && (botao.Name != "btnCross"))
                {
                    botao.Background = new SolidColorBrush(Colors.Gray);
                }
            }
        }
        #endregion DesmarcaItensToolbarVertical()
                
        #endregion


        #region Dados do grafico

        public void SetUltimaVariacao(double ultimaVariacao)
        {
            StockChart.DataManager.DataManager.UltimaVariacao = ultimaVariacao;
        }

        #endregion

        private void stockChart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (linhaEstudoSelecionada)
            {
                
                //Cross Hair
                case 1:
                    if (this.stockChart.CrossHairs == false)
                    {
                        this.stockChart.CrossHairs = true;
                        this.stockChartIndicadores.CrossHairs = true;
                    }
                    break;

                //Zoom In
                case 2:
                    //this.ZoomIn(nivelZoom);
                    break;

                //Reseta Zoom
                case 3:
                    this.stockChart.ResetZoom();
                    this.stockChart.ResetYScale(0);
                    btnResetZoom.Background = new SolidColorBrush(Colors.Gray);
                    break;

                //Quadrado
                case 4:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.Rectangle, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Elipse
                case 5:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.Ellipse, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Fibo Arcs
                case 6:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.FibonacciArcs, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Fibo Retracements
                case 7:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.FibonacciRetracements, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Fibo Fan
                case 8:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.FibonacciFan, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Fibo TimeZone
                case 9:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.FibonacciTimeZones, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Error Channel
                case 10:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.ErrorChannel, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Gann Fan
                case 11:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.GannFan, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Quadrant Lines
                case 12:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.QuadrantLines, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //RaffRegression
                case 13:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.RaffRegression, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Speed Lines
                case 14:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.SpeedLines, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //TironeLevels
                case 15:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.TironeLevels, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Linha Horizontal
                case 16:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.HorizontalLine, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Linha Vertical
                case 17:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.VerticalLine, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Linha Tendencia
                case 18:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.TrendLine, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                //Zoom de area
                case 19:
                    this.ZoomArea();
                    break;

                //Linha de estudo Texto
                case 20:
                    this.AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.StaticText, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                
                //Usando Regua
                case 22:
                    AdicionaLinhaEstudo(LineStudy.StudyTypeEnum.Regua, configuracaoGraficoLocal.TipoLinhaDefault,
                        configuracaoGraficoLocal.CorObjetoDefault, configuracaoGraficoLocal.GrossuraLinhaDefault, "");
                    break;

                default:
                    break;
            }
        }

        #region Salvar Grafico

        /// <summary>
        /// Metodo que retorna o array de bytes que compoem o gráfico
        /// </summary>
        /// <returns></returns>
        public byte[] SaveAsImage()
        {
            return stockChart.SaveAsImage(stockChart, stockChartIndicadores);
        }



        #endregion

        #region Salvar  gráfico como csv

        public void salvaGraficoCsv()
        {
            SaveFileDialog file = new SaveFileDialog();

            file.DefaultExt = "*.csv";
            file.Filter = "Excel |*.csv";
            if (file.ShowDialog() == false)
                return;

            using (StreamWriter sw = new StreamWriter(file.OpenFile()))
            {
                sw.WriteLine("ATIVO;ABERTURA;MAXIMO;MINIMO;FECHAMENTO;DATA;VOLUME");
                sw.Flush();

                foreach (BarraDTO barra in Dados)
                {
                    sw.WriteLine(barra.Symbol + ";" + barra.OpenPrice + ";" + barra.HighPrice + ";" + barra.LowPrice + ";" + barra.ClosePrice + ";" + barra.TradeDate + ";" + barra.Volume);
                    sw.Flush();
                }

            }
        }

        #endregion Salvar  gráfico como csv

        private void btnPublicar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Traderdata.Client.Componente.GraficoSL.Outros.ComentarioDialog comentarioDialog = new Outros.ComentarioDialog();
            if (Comentario != null)
                comentarioDialog.txtNovoComentario.Text = Comentario;
            else
                comentarioDialog.txtNovoComentario.Text = "";

            comentarioDialog.Closing += (sender1, e1) =>
            {
                if (comentarioDialog.DialogResult == true)
                {
                    Comentario = comentarioDialog.Comentario;
                }
            };
            comentarioDialog.Show();
        }

        /// <summary>
        /// Metodo que habilita o tick box
        /// </summary>
        public void HabilitaTickBox()
        {
            try
            {
                //if (!this.TickBoxHabilitado)
                //{
                foreach (Series obj in stockChart.SeriesCollection)
                {
                    if (obj.FullName.Contains("Ultimo"))
                        obj.TickBox = EnumGeral.PosicaoTickBox.Direita;
                }
                this.TickBoxHabilitado = true;
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Métodos

        private void btnVolume_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)btnVolume.IsChecked)
            {
                btnVolume.Background = Brushes.DarkRed;
                configuracaoGraficoLocal.TipoVolume = "F";

                if (OnAlteraTipoVolume != null)
                    OnAlteraTipoVolume(this);
            }
            else
            {
                configuracaoGraficoLocal.TipoVolume = "N";
                btnVolume.Background = Brushes.White;

                if (OnAlteraTipoVolume != null)
                    OnAlteraTipoVolume(this);
            }

            this.Refresh(EnumGeral.TipoRefresh.Tudo);
        }

        private void btnDiario_Click(object sender, RoutedEventArgs e)
        {
            this.Periodicidade = EnumPeriodicidade.Diario;
            this.Periodo = EnumPeriodo.SeisMeses;                
            if (OnAlteraPeriodicidade != null)
                OnAlteraPeriodicidade(this);
        }

        private void btnIntraday_Click(object sender, RoutedEventArgs e)
        {


            this.Periodicidade = EnumPeriodicidade.CincoMinutos;
            this.Periodo = EnumPeriodo.UmMes;
            if (OnAlteraPeriodicidade != null)
                OnAlteraPeriodicidade(this);
        }
                
        private void btnCandle_Click(object sender, RoutedEventArgs e)
        {
            this.configuracaoGraficoLocal.EstiloBarra = EnumGeral.TipoSeriesEnum.Candle;
            OnAtualizaDados(this);
        }

        private void btnBarra_Click(object sender, RoutedEventArgs e)
        {
            this.configuracaoGraficoLocal.EstiloBarra = EnumGeral.TipoSeriesEnum.Barra;
            OnAtualizaDados(this);
        }

        private void btnLinha_Click(object sender, RoutedEventArgs e)
        {
            this.configuracaoGraficoLocal.EstiloBarra = EnumGeral.TipoSeriesEnum.Linha;
            OnAtualizaDados(this);
        }


      

    }


}
