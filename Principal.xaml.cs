 using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using C1.Silverlight;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

using Traderdata.Client.Componente.GraficoSL.DTO;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.Main;
using Traderdata.Client.TerminalWEB.Dialog;
using Traderdata.Client.TerminalWEB.DTO;
using Traderdata.Client.TerminalWEB.Util;
using Traderdata.Client.TerminalWEB.RT;
using Traderdata.Client.TerminalWEB.RT.DTO;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
using FM.WebSync.Silverlight.Core;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using FTG.Silverlight.Google.Analytics;
using System.Windows.Browser;

namespace Traderdata.Client.TerminalWEB
{
    public partial class Principal : UserControl
    {
        #region Campos

        bool mustUpdate = true;

        internal bool primeiraVez = true;

        internal bool aplicarTemplate;

        /// <summary> Evento genérico, que participa do processo de alternativa ao uso de invoke.</summary>
        internal event SendOrPostCallback GenericEventHandler;

        //Variável que permitirá executar um método no contexto da thread principal, evitando a necessidade de invoke pela aplicacao cliente
        internal AsyncOperation asyncOperation;

        //variavel de controle de realtime
        internal FM.WebSync.Silverlight.Core.Client client;
                        
        //provider
        internal NumberFormatInfo provider = Traderdata.Client.TerminalWEB.RT.Util.NumberProvider;

        //variavel de link para versao completa
        private string linkVersaoCompleta = "";

        //Variavel que guarda o volume que veio do banco para ser usado nas periodicidades semanais e mensais
        private double volumeUltimaBarraBanco = 0;
        private double ultimoVolumeDia = 0;
        private double penultimoVolumeDia = 0;
        private double incrementeVolume = 0;

        //Vairavle que guarda o ultiom dado recebido
        private DateTime dataUltimaDado = DateTime.Now;

        public bool ATSalvaSaidaAPP { get; set; }
        private delegate void DelegateGrafico(Grafico grafico);
                
        //Configuração padrao
        private TerminalWebSVC.ConfiguracaoPadraoDTO configuracaoPadraDTO;

        //Area de trabalho do cliente logado
        private TerminalWebSVC.AreaTrabalhoDTO areaTrabalhoDTO; 

        //Variavel de controle de grafico selecionado
        private int indexGraficoSelecionado = -1;

        //Lista de controle dos graifcos abertos na tela
        private List<Grafico> listaGraficos = new List<Grafico>();

        //Lista de templates
        private List<TemplateDTO> listaTemplates = new List<TemplateDTO>();

        //Varialve de serviço
        private TerminalWebSVC.TerminalWebClient baseTerminalWebSVC;
        private SoaMD.MarketDataTerminalWebClient baseMarketDataCommon;
        private LogSVC.LogServiceClient baseLogService;

        //TODO:Retirar a lista abaixo, pois posso pegar direto da propriedade do DDF
        private List<AtivoLocalDTO> listaAtivosLocal = new List<AtivoLocalDTO>();

        //private System.Windows.Threading.DispatcherTimer timerRefreshDados = new System.Windows.Threading.DispatcherTimer();
   
        //Usuario vindo do HB
        private string usuarioHB = "";

        //Timer de autosave
        private System.Windows.Threading.DispatcherTimer timerAutosave = new System.Windows.Threading.DispatcherTimer();

        //Timer de assinatura de ativos
        private System.Windows.Threading.DispatcherTimer timerAssinatura = new System.Windows.Threading.DispatcherTimer();

        //Timer de ping para contagem de usuarios
        private System.Windows.Threading.DispatcherTimer timerPing = new System.Windows.Threading.DispatcherTimer();

        private System.Windows.Threading.DispatcherTimer timerUpdate = new System.Windows.Threading.DispatcherTimer();

        //Campo que armazena zoom inicial
        private int firstRecord = 0;
        private int visibleRecordCount = 0;
        private bool lastRecordEUltimoRecord = true;
                

        #endregion Campos

        #region Contrutores
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Principal()
        {
            try
            {
                InitializeComponent();


                stpMenu.Background = menu.Background;

                if (ServiceWCF.Simpletrader)
                {
                    stpSimpletrader.Visibility = System.Windows.Visibility.Visible;
                    stpMenu.Visibility = System.Windows.Visibility.Collapsed;
                }

                //Iniciando os serviços
                baseLogService = new LogSVC.LogServiceClient(ServiceWCF.basicBind, ServiceWCF.endPointLogService);
                baseTerminalWebSVC = new TerminalWebSVC.TerminalWebClient(ServiceWCF.basicBind, ServiceWCF.endPointTerminalWebSVC);
                baseMarketDataCommon =new SoaMD.MarketDataTerminalWebClient(ServiceWCF.basicBind, ServiceWCF.endPointMarketDataCommon);

                //Associando o novo behaviuour aos serviços
                //baseTerminalWebSVC.ChannelFactory.Endpoint.Behaviors.Add(new Util.ClassBehaviour());


                //Assinando os serviços de DDF
                baseTerminalWebSVC.ConnectCompleted += new EventHandler<TerminalWebSVC.ConnectCompletedEventArgs>(baseInterbolsa_ConnectCompleted);

                baseMarketDataCommon.GetAtivoCompleted += new EventHandler<SoaMD.GetAtivoCompletedEventArgs>(baseMarketDataCommon_GetAtivoCompleted);

                baseMarketDataCommon.GetAtivosCompactadoCompleted += new EventHandler<SoaMD.GetAtivosCompactadoCompletedEventArgs>(baseMarketDataCommon_GetAtivosCompactadoCompleted);
                baseMarketDataCommon.GetHistoricoBMFCompleted += new EventHandler<SoaMD.GetHistoricoBMFCompletedEventArgs>(baseMarketDataCommon_GetHistoricoBMFCompleted);
                baseMarketDataCommon.GetHistoricoBovespaCompleted += new EventHandler<SoaMD.GetHistoricoBovespaCompletedEventArgs>(baseMarketDataCommon_GetHistoricoBovespaCompleted);
                
                //Assinando os serviços de Grafico
                baseTerminalWebSVC.RetornaGraficoCompleted += new EventHandler<TerminalWebSVC.RetornaGraficoCompletedEventArgs>(baseFreeStockChartPlus_RetornaGraficoCompleted);
                baseTerminalWebSVC.SalvarGraficoCompleted += new EventHandler<AsyncCompletedEventArgs>(baseFreeStockChartPlus_SalvarGraficoCompleted);
                baseTerminalWebSVC.RetornaTemplatePorIdCompleted += new EventHandler<TerminalWebSVC.RetornaTemplatePorIdCompletedEventArgs>(baseFreeStockChartPlus_RetornaTemplatePorIdCompleted);
                baseTerminalWebSVC.RetornaUltimaAnaliseCompleted += new EventHandler<TerminalWebSVC.RetornaUltimaAnaliseCompletedEventArgs>(baseFreeStockChartPlus_RetornaUltimaAnaliseCompleted);
                baseTerminalWebSVC.RetornaAreaTrabalhoPorIdCompleted += new EventHandler<TerminalWebSVC.RetornaAreaTrabalhoPorIdCompletedEventArgs>(baseInterbolsa_RetornaAreaTrabalhoPorIdCompleted);
                baseTerminalWebSVC.RetornaConfiguracaoPadraoPorIdCompleted += new EventHandler<TerminalWebSVC.RetornaConfiguracaoPadraoPorIdCompletedEventArgs>(baseInterbolsa_RetornaConfiguracaoPadraoPorIdCompleted);
                baseTerminalWebSVC.RetornaTemplatesPorClientIdCompleted += new EventHandler<TerminalWebSVC.RetornaTemplatesPorClientIdCompletedEventArgs>(baseInterbolsa_RetornaTemplatesPorClientIdCompleted);
                baseTerminalWebSVC.SalvaAreaTrabalhoCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(baseInterbolsa_SalvaAreaTrabalhoCompleted);
                baseTerminalWebSVC.SalvaConfiguracaoPadraoCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(baseInterbolsa_SalvaConfiguracaoPadraoCompleted);
                baseTerminalWebSVC.SalvaTemplateCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(baseInterbolsa_SalvaTemplateCompleted);
                baseTerminalWebSVC.ExcluiTemplateCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(baseInterbolsa_ExcluiTemplateCompleted);
                baseTerminalWebSVC.SalvarAnaliseCompartilhadaCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(baseFreeStockChartPlus_SalvarAnaliseCompartilhadaCompleted);
                baseTerminalWebSVC.AutenticaCorretoraCompleted += new EventHandler<TerminalWebSVC.AutenticaCorretoraCompletedEventArgs>(baseFreeStockChartPlus_AutenticaCorretoraCompleted);

                //conectando o hubd de dados em rt                
                client = new FM.WebSync.Silverlight.Core.Client(new ClientArgs
                {
                    RequestUrl = ServiceWCF.RTURLHost
                });

                
                //inicializando o connectArgs

                client.Connect(new ConnectArgs
                {
                    //StayConnected = true,
                    OnSuccess = (args) =>
                    {
                        //codigo necessario no sucesso da conexao
                        Dispatcher.BeginInvoke(() =>
                        {
                            txtStatus.Text = dataUltimaDado.ToString("HH:mm");
                        });

                    },
                    OnFailure = (args) =>
                    {
                        //codigo necessario no insucesso da conexao
                        client.Reconnect();
                        //txtStatus.Text = "Sinal RT: Off";
                    },
                    OnStreamFailure = (args) =>
                    {
                        //reconectando
                        //client.Reconnect();
                        //txtStatus.Text = "Sinal RT: Off";
                    }
                });

                

                //Assinando o evento de disparo de dados
                GenericEventHandler = new SendOrPostCallback(ProcessaTick);

                // Cria uma instância de uma AsyncOperation para gerenciar o contexto
                this.asyncOperation = AsyncOperationManager.CreateOperation(null);

                //Iniciando timer de autosave
                timerAutosave.Interval = new TimeSpan(0, 0, 60);
                timerAutosave.Tick += new EventHandler(timerAutosave_Tick);
                timerAutosave.Start();

                //Iniciando o time de assinatura
                timerAssinatura.Interval = new TimeSpan(0, 0, 5);
                timerAssinatura.Tick += new EventHandler(timerAssinatura_Tick);
                timerAssinatura.Start();

                //Iniciando o time de assinatura
                timerPing.Interval = new TimeSpan(0, 1, 0);
                timerPing.Tick += new EventHandler(timerPing_Tick);
                timerPing.Start();
                
                timerUpdate.Interval = new TimeSpan(0, 0, 1);
                timerUpdate.Tick += new EventHandler(timerUpdate_Tick);
                timerUpdate.Start();

            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao carregar o gráfico.", "Construtor", exc);
            }
        }

        void timerUpdate_Tick(object sender, EventArgs e)
        {
            if ((indexGraficoSelecionado >= 0) && (mustUpdate))
                listaGraficos[indexGraficoSelecionado].UpdateGrafico();
        }

        

        void baseMarketDataCommon_GetAtivoCompleted(object sender, SoaMD.GetAtivoCompletedEventArgs e)
        {
            string obj = e.Result;

            if ((obj != null) && (obj.Split(';')[1].ToUpper() == ServiceWCF.AtivoDireto.ToUpper()))
            {
                //adicionando na lista local
                listaAtivosLocal.Add(new AtivoLocalDTO(obj.Split(';')[1], obj.Split(';')[2], (EnumLocal.Bolsa)Convert.ToInt16(obj.Split(';')[3])));

                //carregando as configurações default
                baseTerminalWebSVC.RetornaConfiguracaoPadraoPorIdAsync(ServiceWCF.IdUsuario);
            }
            else
            {
                //adicionando na lista local
                listaAtivosLocal.Add(new AtivoLocalDTO("IBOV", "IBOV", EnumLocal.Bolsa.Bovespa));
                ServiceWCF.AtivoDireto = "IBOV";

                //carregando as configurações default
                baseTerminalWebSVC.RetornaConfiguracaoPadraoPorIdAsync(ServiceWCF.IdUsuario);
            }
        }

        void timerAutosave_Tick(object sender, EventArgs e)
        {
            if (ServiceWCF.Simpletrader)
            {
                if ((bool)chkSalvarAutomatico.IsChecked)
                {
                    ModuloCarregando(true, "Salvando Gráfico");
                    baseTerminalWebSVC.SalvarGraficoAsync(ConvertGraficoComponenteToServer(listaGraficos[indexGraficoSelecionado]),
                        ServiceWCF.IdUsuario, "AUTO");
                }
            }
        }


        void baseMarketDataCommon_GetAtivosCompactadoCompleted(object sender, SoaMD.GetAtivosCompactadoCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    foreach (string obj in e.Result)
                    {
                        if (obj.Length > 0)
                            listaAtivosLocal.Add(new AtivoLocalDTO(obj.Split(';')[0], obj.Split(';')[1], (EnumLocal.Bolsa)Convert.ToInt16(obj.Split(';')[2])));
                    }
                                        
                }

            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao retornar os ativos.", "baseInterbolsa_GetAtivosCompleted", exc);
            }
            finally
            {
                ModuloCarregando(false);


                //Carregando as preferencias de usuários
                ModuloCarregando(true, "Carregando Conf. Pessoais");

                if (!ServiceWCF.Simpletrader)
                    stpMenu.Visibility = System.Windows.Visibility.Visible;

                //Carregando a configuração padrao do usuário
                baseTerminalWebSVC.RetornaConfiguracaoPadraoPorIdAsync(ServiceWCF.IdUsuario);

            }
        }

        void timerPing_Tick(object sender, EventArgs e)
        {            
            //track page
            HtmlPage.Window.Invoke("ga", new string[] { "create", ServiceWCF.GoogleAnalytics, "auto" }); 
            HtmlPage.Window.Invoke("ga", new string[] { "send", "pageview" }); 
        }


        void baseFreeStockChartPlus_AutenticaCorretoraCompleted(object sender, TerminalWebSVC.AutenticaCorretoraCompletedEventArgs e)
        {
            ServiceWCF.Usuario = e.Result;
            ServiceWCF.userHB = e.Result.Id.ToString();

            //Validando o usuário
            ModuloCarregando(true, "Validando Usuário");

            //Conectando o usuario
            baseTerminalWebSVC.ConnectAsync(ServiceWCF.ID);

            if (ServiceWCF.AnaliseCompartilhada)
                mnuAnaliseCompartilhada.Visibility = System.Windows.Visibility.Visible;
            else
                mnuAnaliseCompartilhada.Visibility = System.Windows.Visibility.Collapsed;

            if (e.Result.Perfil == "P")
            {
                mnuPublicar.Visibility = System.Windows.Visibility.Visible;                
            }
            
        }
        
        #endregion Contrutores
         
        #region Eventos

        #region Inicialização

        #region LayoutRoot
        /// <summary>
        /// Evento dispara quando a grid LayoutRoot (grid principal) e carregando a tela para adicionar um novo gráfico.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //track page
            Tracker.TrackPage("mypage");

            if (!ServiceWCF.Site)
                CarregaLayoutInicial();
            else
            {
                ChamaLogin();
            }
        }

        private void ChamaLogin()
        {
            Login login = new Login();
            login.Closing += (sender1, e1) =>
            {
                if (ServiceWCF.Usuario != null)
                    CarregaLayoutInicial();
                else
                    ChamaLogin();
            };
            login.Show();
        }

        private void CarregaLayoutInicial()
        {
            try
            {
                //montando o link da versaõ completa
                SHA256Managed hashSHA = new SHA256Managed();
                hashSHA.ComputeHash(ConvertStringToByteArray(ServiceWCF.ID + "-" + DateTime.Today.ToString("dd-MM-yyyy")));

                string stringHash = ServiceWCF.BaseAddress + "/DefaultGraficoIntegracaoVersaoCompleta.aspx?hashversaocompleta=";

                foreach (byte b in hashSHA.Hash)
                {
                    stringHash += Convert.ToString(Convert.ToInt16(b)) + "-";
                }

                stringHash = stringHash.Remove(stringHash.Length - 1);

                stringHash += "&codcliente=" + ServiceWCF.ID;

                this.linkVersaoCompleta = stringHash;

                //Apresentando a dica se estiover marcado para ser apresentada
                if (ServiceWCF.ApresentarDica)
                {
                    Dicas dicas = new Dicas();
                    dicas.Show();
                }



                if (ServiceWCF.Alerta)
                    mnuFerramentas.Visibility = System.Windows.Visibility.Visible;

                //alterando a cor de fundo
                bkcImage.Background = GetColorFromHexa(ServiceWCF.CorFundo);

                //checando se deve apresentar o menu de suporte
                if (!ServiceWCF.PossuiSuporte)
                    mnuBugReport.Visibility = System.Windows.Visibility.Collapsed;

                //autenticando junto a corretora
                baseTerminalWebSVC.AutenticaCorretoraAsync(ServiceWCF.ID);


            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao carregar o gráfico.", "LayoutRoot_Loaded", exc);
            }
        }
        
        #endregion LayoutRoot

        #endregion Inicialização

        #region Visualização

        #region Mudança de Tamanho da Página
        /// <summary>
        /// Evento Resize: Quando o browser fopr redimensionado para um  tamanho menor que 773, "Powered by Traderdata" some.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (indexGraficoSelecionado > -1)
                {
                    if (e.NewSize.Width < 773)
                        listaGraficos[indexGraficoSelecionado].VisibilidadeLinkTD = false;
                    else
                        listaGraficos[indexGraficoSelecionado].VisibilidadeLinkTD = true;
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao redimensionar gráfico.", "UserControl_SizeChanged", exc);
            }
        }
        #endregion Mudança de Tamanho da Página

        #endregion

        #region Area de Trabalho

        #region Completed



        /// <summary>
        /// Evento realizado ao salvar a área de trabalho
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseInterbolsa_SalvaAreaTrabalhoCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ATSalvaSaidaAPP = true;
            ModuloCarregando(false);
            MessageBox.Show("Área de trabalho salva com sucesso.", "Sucesso", MessageBoxButton.OK);
            if ((bool)e.UserState)
            {
                PublicarGrafico();
            }
        }

        /// <summary>
        /// Evento disparado quando após se carregar a área de trabalho do cliente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseInterbolsa_RetornaAreaTrabalhoPorIdCompleted(object sender, TerminalWebSVC.RetornaAreaTrabalhoPorIdCompletedEventArgs e)
        {
            try
            {
                int index = 0;

                //Setando a area de trabalho local
                this.areaTrabalhoDTO = (TerminalWebSVC.AreaTrabalhoDTO)e.Result;

                //Recuperando a area de trabalho
                foreach (TerminalWebSVC.GraficoDTO grafico in this.areaTrabalhoDTO.Graficos)
                {
                    Grafico gf = ConvertGraficoServerToComponente(grafico);

                    gf.ShowHideBotaoAC(ServiceWCF.AnaliseCompartilhada);
                        
                    //Adicionando na lista de graficos
                    listaGraficos.Add(gf);
                    //pnlGraficoContainer.Children.Add(gf);

                    //Popular acesso a gráfico
                    if (index == 0)
                        CriaTabPainel(listaGraficos[index], true, true);
                    //  CriaEsquemaPainel(listaGraficos.Count - 1, true);
                    else
                        CriaTabPainel(listaGraficos[index], false, true);



                    index++;
                }

                if (this.areaTrabalhoDTO.Graficos.Count > 0)
                {
                    //Setar o indexselecionado
                    this.indexGraficoSelecionado = 0;
                    tabControl.SelectedIndex = 0;

                    listaGraficos[0].Refresh(EnumGeral.TipoRefresh.SomenteUpdate);                    
                }

            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao retornar a área de trabalho.", "baseGrafico_RetornaAreaTrabalhoPorIdCompleted", exc);
            }
            finally
            {
                ModuloCarregando(false);
            }
        }

        

        
        #endregion Area de Trabalho

        #endregion Area de Trabalho

        #region Template

        #region Completed

        /// <summary>
        /// Evento disparado ao terminar de excluir o template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseInterbolsa_ExcluiTemplateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ModuloCarregando(false);
            MessageBox.Show("Template excluído com sucesso.", "Sucesso", MessageBoxButton.OK);
        }

        /// <summary>
        /// Evento disparado após salvar um template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseInterbolsa_SalvaTemplateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ModuloCarregando(false);
            MessageBox.Show("Template salvo com sucesso.");
        }

        /// <summary>
        /// Evento disparado ao terminar de trazer os templates de determinado cliente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseInterbolsa_RetornaTemplatesPorClientIdCompleted(object sender, TerminalWebSVC.RetornaTemplatesPorClientIdCompletedEventArgs e)
        {
            try
            {
                //populando a lista local de templates
                listaTemplates.Clear();
                foreach (TerminalWebSVC.TemplateDTO obj in e.Result)
                {
                    TemplateDTO tempAux = new TemplateDTO();
                    tempAux.Id = obj.Id;
                    tempAux.Nome = obj.Nome;
                    listaTemplates.Add(tempAux);
                }

                switch ((EnumLocal.AcaoTemplate)e.UserState)
                {
                    case EnumLocal.AcaoTemplate.Excluir:
                        ExcluirTemplate((List<TerminalWebSVC.TemplateDTO>)e.Result);
                        break;

                    case EnumLocal.AcaoTemplate.Aplicar:
                        AplicarTemplate((List<TerminalWebSVC.TemplateDTO>)e.Result);
                        break;

                    case EnumLocal.AcaoTemplate.Salvar:
                        SalvarTemplate((List<TerminalWebSVC.TemplateDTO>)e.Result);
                        break;

                    case EnumLocal.AcaoTemplate.CarregarNovoGrafico:
                        AbrirDialogNovoGrafico((List<TerminalWebSVC.TemplateDTO>)e.Result, false);
                        break;

                    case EnumLocal.AcaoTemplate.CarregarAlteracaoGrafico:
                        //Carrego a lista de ativos se ainda nao tiver carregado
                        AbrirDialogNovoGrafico((List<TerminalWebSVC.TemplateDTO>)e.Result, true);                        
                        break;
                    case EnumLocal.AcaoTemplate.Avulso:
                        AbrirGraficoAvulso();
                        break;
                    case EnumLocal.AcaoTemplate.AvulsoSimpleTrader:
                        AbrirGraficoAvulsoSimpleTrader();
                        break;
                    case EnumLocal.AcaoTemplate.CarregaAreaTrabalho:
                        //Carregando a area de trabalho                
                        baseTerminalWebSVC.RetornaAreaTrabalhoPorIdAsync(ServiceWCF.IdUsuario);
                        break;
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao retornar templates.", "baseGrafico_RetornaTemplatesPorClientIdCompleted", exc);
            }

        }

        


        #endregion

        #endregion Template

        #region Configuração Padrão

        #region Completed

        /// <summary>
        /// Evento disparado quando encerra o processamento de salvar configurações padroes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseInterbolsa_SalvaConfiguracaoPadraoCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ModuloCarregando(false);
            MessageBox.Show("Configurações salvas com sucesso.", "Sucesso", MessageBoxButton.OK);
        }

        /// <summary>
        /// Evento disparado quando encerra o carregamento da configuracao padrao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseInterbolsa_RetornaConfiguracaoPadraoPorIdCompleted(object sender, TerminalWebSVC.RetornaConfiguracaoPadraoPorIdCompletedEventArgs e)
        {
            
            try
            {
                //Setando a configuração padrão
                this.configuracaoPadraDTO = (TerminalWebSVC.ConfiguracaoPadraoDTO)e.Result;

                if (ServiceWCF.AtivoDireto == "")
                {
                    baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.CarregaAreaTrabalho);
                    
                }
                else
                {
                    if (!ServiceWCF.Simpletrader)
                        baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.Avulso);
                    else
                        baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.AvulsoSimpleTrader);
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao retornar a configuração padrão do usuário.", "baseGrafico_RetornaConfiguracaoPadraoPorIdCompleted", exc);
            }
        }

        #endregion Completed

        #endregion Configuração Padrão

        #region Eventos do Componente Grafico

        /// <summary>
        /// Evento disparado ao se trocar a periodicidade
        /// </summary>
        /// <param name="grafico"></param>
        private void Principal_OnAlteraPeriodo(Grafico grafico)
        {
            ModuloCarregando(true, "Atualizando período");
            AtualizaGrafico(false);
        }

        /// <summary>
        /// Evento disparado ao se clicar em atualiza dados
        /// </summary>
        /// <param name="grafico"></param>
        private  void Principal_OnAtualizaDados(Grafico grafico)
        {
            ModuloCarregando(true, "Atualizando dados");
            AtualizaGrafico(false);
        }

        /// <summary>
        /// Evento disparado ao se clicar sobre o nome do grafico
        /// </summary>
        /// <param name="grafico"></param>
        private void Principal_OnAtualizaAtivo(Grafico grafico)
        {

            //if (listaAtivosLocal.Count > 1)
            if (1==1)
            {
                ModuloCarregando(true, "Atualizando gráfico");

                //Variavel de retorno de mensagem            
                baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.CarregarAlteracaoGrafico);
            }
            else
            {
                ModuloCarregando(true, "Carregando Ativos");

                baseMarketDataCommon.GetAtivosAsync("NOVOATIVO");
            }
            
        }

        /// <summary>
        /// Evento disparado ao se clicar sobre a periodicidade
        /// </summary>
        /// <param name="grafico"></param>
        private void Principal_OnAlteraPeriodicidade(Grafico grafico)
        {
            ModuloCarregando(true, "Atualizando periodicidade");
            AtualizaGrafico(false);
        }

        #endregion

        #region RT

        /// <summary>
        /// Metodo que será disparado a cada minuto pelo timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void timerRefreshDados_Tick(object sender, EventArgs e)
        {
            //if ((listaGraficos.Count > 0) && (mnuAtualizacaoautomatica.IsChecked))
            //{
            //    firstRecord = listaGraficos[indexGraficoSelecionado].GetFirstVisibleRecord();
            //    visibleRecordCount = listaGraficos[indexGraficoSelecionado].GetVisibleRecordCount() + 2;                
            //    AtualizaGrafico(false);                
            //}
               
        }

        /// <summary>
        /// Metodo que valida a existencia do usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseInterbolsa_ConnectCompleted(object sender, TerminalWebSVC.ConnectCompletedEventArgs e)
        {
            //Setando o id do usuario
            ServiceWCF.IdUsuario = e.Result;

            if (ServiceWCF.AtivoDireto == "")
            {
                if (!ServiceWCF.Simpletrader)
                {
                    stpMenu.Visibility = System.Windows.Visibility.Visible;
                    tabControl.Visibility = System.Windows.Visibility.Visible;
                    bkcImage.Visibility = System.Windows.Visibility.Visible;
                }

                //Carregando os ativos
                ModuloCarregando(true, "Carregando ativos");
                //baseFreeStockChartPlus.GetAtivosAsync();
                baseMarketDataCommon.GetAtivosCompactadoAsync();
            }
            else
            {
                //carregando o grafico direto
                graficoAvulso.Visibility = System.Windows.Visibility.Visible;

                if (ServiceWCF.Simpletrader)
                {
                    Thickness margem = new Thickness(0, 30, 0, 0);
                    graficoAvulso.Margin = margem;
                }
                //retornando o ativo direto
                baseMarketDataCommon.GetAtivoAsync(ServiceWCF.AtivoDireto.ToUpper());
                
            }
        }

       


        /// <summary>
        /// MEtodo que retorna historico para Bovespa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseMarketDataCommon_GetHistoricoBovespaCompleted(object sender, SoaMD.GetHistoricoBovespaCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    if (e.Result.Count > 0)
                    {
                        List<SoaMD.CotacaoServerDTO> lista = (List<SoaMD.CotacaoServerDTO>)e.Result;
                        
                        //Setando o volume da ultima barra
                        volumeUltimaBarraBanco = lista[lista.Count - 1].Volume;

                        if (e.UserState != null)
                        {
                            if (e.UserState.GetType().ToString().Contains("NovoGrafico"))
                                if (((NovoGrafico)e.UserState).MesmoGrafico)
                                    AlteraGraficoBVSP((NovoGrafico)e.UserState, lista);
                                else
                                    CarregaNovoGraficoBVSP((NovoGrafico)e.UserState, lista);
                            else if (e.UserState.GetType().ToString().Contains("NovoAtivoMesmoGrafico"))
                            {
                                List<double> listaDouble = new List<double>();
                                List<DateTime> listaDatetime = new List<DateTime>();
                                foreach (SoaMD.CotacaoServerDTO obj in e.Result)
                                {
                                    listaDouble.Add(obj.Ultimo);
                                    listaDatetime.Add(obj.Data);
                                }
                                listaGraficos[indexGraficoSelecionado].AdicionaSerieAuxiliar("teste", listaDouble, listaDatetime);
                            }
                        }
                        else
                            CarregaGraficoExistenteBVSP(lista);

                        //acertando o zoom
                        listaGraficos[indexGraficoSelecionado].SetFirstVisibleRecord(firstRecord);
                        listaGraficos[indexGraficoSelecionado].SetLastVisibleRecord(firstRecord + visibleRecordCount);
                        listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.SomenteUpdate);
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível carregar cotações para o ativo selecionado.");
                    }
                }
                else
                    MessageBox.Show("Ocorreu um erro ao retornar os dados históricos.\nPor favor feche o gráfico e abra novamente.");

                //assinando cotação em RT
                AssinaCotacao(listaGraficos[indexGraficoSelecionado].Ativo);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao retorna os dados históricos.", "baseInterbolsa_GetHistoricoBovespaCompleted", exc);
            }
            finally
            {
                ModuloCarregando(false);
            }
        }

        /// <summary>
        /// Metodo que retorna hsitorico BMF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseMarketDataCommon_GetHistoricoBMFCompleted(object sender, SoaMD.GetHistoricoBMFCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    ModuloCarregando(true);

                    List<SoaMD.CotacaoServerDTO> lista = (List<SoaMD.CotacaoServerDTO>)e.Result;

                    //Criando o objeto de configuração a partir do padrão do usuário
                    NovoGrafico novoGraficoObj = (NovoGrafico)e.UserState;

                    if (novoGraficoObj != null)
                        if (novoGraficoObj.MesmoGrafico)
                            AlteraGraficoBMF(novoGraficoObj, lista);
                        else
                            CarregaNovoGraficoBMF(novoGraficoObj, lista);
                    else
                        CarregaGraficoExistenteBMF(lista);

                    //acertando o zoom
                    listaGraficos[indexGraficoSelecionado].SetFirstVisibleRecord(firstRecord);
                    listaGraficos[indexGraficoSelecionado].SetLastVisibleRecord(firstRecord + visibleRecordCount);
                    listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.SomenteUpdate);
                }
                else
                    MessageBox.Show("Ocorreu um erro ao retornar os dados históricos.\nPor favor feche o gráfico e abra novamente.");

                //assinando cotação em RT
                AssinaCotacao(listaGraficos[indexGraficoSelecionado].Ativo);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao retornar os dados históricos.", "baseDDF_GetHistoricoBMFCompleted", exc);
            }
            finally
            {
                ModuloCarregando(false);
            }
        }

        /// <summary>
        /// Metodo que retorna os ativos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseInterbolsa_GetAtivosCompleted(object sender, SoaMD.GetAtivosCompletedEventArgs e)
        {
            try
            {
                foreach (string obj in e.Result)
                {
                    listaAtivosLocal.Add(new AtivoLocalDTO(obj.Split(';')[1], obj.Split(';')[2], (EnumLocal.Bolsa)Convert.ToInt16(obj.Split(';')[3])));
                }

                if (e.UserState != null)
                {
                    if (Convert.ToString(e.UserState) == "NOVOATIVO")
                    {
                        //Variavel de retorno de mensagem            
                        baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.CarregarAlteracaoGrafico);
                    }
                }
                                
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao retornar os ativos.", "baseInterbolsa_GetAtivosCompleted", exc);
            }
            finally
            {
                ModuloCarregando(false);

                
                //Carregando as preferencias de usuários
                ModuloCarregando(true, "Carregando Conf. Pessoais");

                stpMenu.Visibility = System.Windows.Visibility.Visible;

                //Carregando a configuração padrao do usuário
                baseTerminalWebSVC.RetornaConfiguracaoPadraoPorIdAsync(ServiceWCF.IdUsuario);
                
            }
        }

        #endregion RT

        #region FreeStockChart

        /// <summary>
        /// Evento de retorno de usuario por senha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void baseFreeStockChartPlus_RetornaUsuarioPorIdCompleted(object sender, TerminalWebSVC.RetornaUsuarioPorIdCompletedEventArgs e)
        //{
        //    try
        //    {
        //        //Calculando o hash
        //        string segura = e.Result.Email + ";" + e.Result.Senha + ";" + DateTime.Today.ToString();
        //        SHA256Managed hashSHA = new SHA256Managed();
        //        hashSHA.ComputeHash(ConvertStringToByteArray(segura));

        //        string stringHash = "";

        //        foreach (byte b in hashSHA.Hash)
        //        {
        //            stringHash += Convert.ToString(Convert.ToInt16(b)) + "-";
        //        }

        //        stringHash = stringHash.Remove(stringHash.Length - 1);



        //        //Checa se strings sao igias
        //        if (stringHash == ServiceWCF.HS)
        //        {
        //            ////Setando os dados do usuario
        //            //ServiceWCF.Usuario = e.Result;
        //            //ServiceWCF.userHB = e.Result.Id.ToString();

        //            ////Avaliando objeto de usuario para determinar se deve ou nao liberar Bovespa e BMF em RT
        //            ////Vendo se esta em periodo de trial
        //            //if (ServiceWCF.Usuario.DataFinalTrial >= DateTime.Today)
        //            //{
        //            //    ServiceWCF.BovespaRT = true;
        //            //    ServiceWCF.BMFRT = true;
        //            //}
        //            //else
        //            //{
        //            //    if (ServiceWCF.Usuario.DataFinalBovespa >= DateTime.Today)
        //            //        ServiceWCF.BovespaRT = true;
        //            //    else
        //            //        ServiceWCF.BovespaRT = false;

        //            //    if (ServiceWCF.Usuario.DataFinalBMF >= DateTime.Today)
        //            //        ServiceWCF.BMFRT = true;
        //            //    else
        //            //        ServiceWCF.BMFRT = false;

        //            //}


        //            ServiceWCF.Usuario = e.Result;
        //            ServiceWCF.userHB = e.Result.Id.ToString();

        //            //Avaliando objeto de usuario para determinar se deve ou nao liberar Bovespa e BMF em RT
        //            //Vendo se esta em periodo de trial
        //            if (ServiceWCF.Usuario.DataFinalTrial >= DateTime.Today)
        //            {
        //                ServiceWCF.BovespaRT = true;
        //                ServiceWCF.BMFRT = true;
        //            }
        //            else
        //            {
        //                if (ServiceWCF.Usuario.DataFinalBovespa >= DateTime.Today)
        //                    ServiceWCF.BovespaRT = true;
        //                else
        //                    ServiceWCF.BovespaRT = false;

        //                if (ServiceWCF.Usuario.DataFinalBMF >= DateTime.Today)
        //                    ServiceWCF.BMFRT = true;
        //                else
        //                    ServiceWCF.BMFRT = false;

        //            }

        //            //Verificando se este usuário é free (para ser free, tem que ter bovespa e bmf como false)
        //            if ((!ServiceWCF.BMFRT) && (!ServiceWCF.BovespaRT))
        //            {
        //                //Apresentar Dialog de diferenças entre um e outro                        
        //                TelaBeneficio telaBeneficios = new TelaBeneficio();

        //                telaBeneficios.Closing += (sender1, e1) =>
        //                {
        //                    if (telaBeneficios.DialogResult == true)
        //                    {

        //                    }
        //                    else
        //                        e1.Cancel = true;
        //                };

        //                telaBeneficios.Show();
                        
        //            }
                    


        //            //Carregando
        //            //Resgatando usuario
        //            usuarioHB = ServiceWCF.userHB;

        //            //Validando o usuário
        //            ModuloCarregando(true, "Validando Usuário");

        //            //Conectando o usuario
        //            baseTerminalWebSVC.ConnectAsync(usuarioHB);
        //        }
        //        else
        //        {
        //            MessageBox.Show("Acesso Negado!");

        //            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.freestockchart.com.br"),
        //            "_self");
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        throw exc;
        //    }
        //}

        #endregion

        #region Seleção Gráficos

        #region Mudança de Seleção na Tab
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
                if (indexGraficoSelecionado >= 0)
                {
                    if (listaGraficos.Count > indexGraficoSelecionado)
                    {
                        //Desassinando ativo antigo
                        client.Unsubscribe(new UnsubscribeArgs
                        {
                            Channel = "/" + listaGraficos[indexGraficoSelecionado].Ativo,
                            OnSuccess = (args) =>
                            {
                                //codigo necessario para sucesso de desassiantura
                            },
                            OnFailure = (args) =>
                            {
                                //codigo de falha de desassinatura
                            }
                        });
                    }
	  
                }

                indexGraficoSelecionado = tabControl.SelectedIndex;


                if (!primeiraVez)
                {
                    if (indexGraficoSelecionado > -1)
                    {
                        ModuloCarregando(true, "Carregando gráfico");
                        AtualizaGrafico(false);
                    }
                }
                else
                    primeiraVez = false;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao mudar a seleção do gráfico.", "tabControl_SelectionChanged", exc);
            }
        }
        #endregion Mudança de Seleção na Tab

        #region Adiciona Grafico
        private void tabAdicionarGrafico_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Variavel de retorno de mensagem
                ModuloCarregando(true, "Carregando gráfico");
                baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.CarregarNovoGrafico);

            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao tentar adicionar um gráfico.", "tabAdicionarGrafico_MouseLeftButtonDown", exc);
            }
        }
        #endregion Adiciona Grafico

        #endregion Seleção Gráficos

        #region Analise Compartilhada

        /// <summary>
        /// Evento disparado ao encerrar a publicacao de analise compartilhada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseFreeStockChartPlus_SalvarAnaliseCompartilhadaCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ModuloCarregando(false);
            MessageBox.Show("Analise Publicada com sucesso.");
        }

        /// <summary>
        /// Evento disparado quando o usuário clica no botão dentro do componente que chama a analise compartilhada
        /// </summary>
        /// <param name="grafico"></param>
        private void novoGrafico_OnVerificaAnaliseCompartilhada(Grafico grafico)
        {
            try
            {
                //devo chamar o metodo da SOA que vai verificar se existe analise para este ativo, se existir deve abrir popup se nao deve avisar
                baseTerminalWebSVC.RetornaUltimaAnaliseAsync(grafico.Ativo);
            }
            catch (Exception exc)
            {                
                throw exc;
            }
        }

        /// <summary>
        /// Evento disparado ao retorna o resultado do retorno da ultima analise compartilhada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseFreeStockChartPlus_RetornaUltimaAnaliseCompleted(object sender, TerminalWebSVC.RetornaUltimaAnaliseCompletedEventArgs e)
        {
            try
            {
                if (e.Result == null)
                    MessageBox.Show("Não existem analises publicadas para este ativo.");
                else
                {
                    int idAnalise = e.Result.Id;
                    string guidAnalise = e.Result.CaminhoImagem;

                    string parametros = "?i=" + idAnalise + "&g=" + guidAnalise;
                    System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(ServiceWCF.LinkVisualizacao + parametros),
                "_new", "menubar=no, scrollbar=yes, Width=1024, toolbars=no, toolbar=no, manubars=no");
                }
            }
            catch (Exception exc)
            {                
                throw exc;
            }
        }


        #endregion

        #endregion Eventos

        #region Metodos

        #region Grafico

        /// <summary>
        /// Metodo que atualiza o grafico
        /// </summary>
        private void AtualizaGrafico(bool graficoInicial)
        {
          
            try
            {
                ModuloCarregando(true, "Recarregando gráfico");

                int index = -1;
                if (graficoInicial)
                    index = 0;
                else
                    index = indexGraficoSelecionado;

                listaGraficos[index].Carregado = true;

                if (RetornaBolsaAtivo(listaGraficos[index].Ativo) == EnumLocal.Bolsa.Bovespa)
                {
                    //Carregar os novos dados
                    baseMarketDataCommon.GetHistoricoBovespaAsync(listaGraficos[index].Ativo,
                        listaGraficos[index].Periodicidade.Value, listaGraficos[index].Periodo.Value,
                        true, false, !ServiceWCF.BovespaRT, DateTime.MinValue, ServiceWCF.Usuario.Guid);
                }
                else
                {
                    //Carregar os novos dados
                    baseMarketDataCommon.GetHistoricoBMFAsync(listaGraficos[index].Ativo,
                        listaGraficos[index].Periodicidade.Value, listaGraficos[index].Periodo.Value,
                        true, false, !ServiceWCF.BMFRT, DateTime.MinValue, ServiceWCF.Usuario.Guid);
                }
                

                //Atualizando o label
                TextBlock txt = listaGraficos[index].Tag as TextBlock;
                if (txt != null)
                    txt.Text = ObtemTituloAba(listaGraficos[index].Ativo, listaGraficos[index].Periodicidade.Value,
                        listaGraficos[index].Periodo.Value);   
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao fazer a chamada de atualização.", "tabAdicionarGrafico_MouseLeftButtonDown", exc);
                ModuloCarregando(false);
            }
        }

        /// <summary>
        /// Metodo que carrega um novo grafico
        /// </summary>
        /// <param name="novoGraficoObj"></param>
        private void CarregaNovoGraficoBVSP(NovoGrafico novoGraficoObj, List<SoaMD.CotacaoServerDTO> lista)
        {
            try
            {
                ConfiguracaoGraficoDTO configuracaoNovoGraficoDTO 
                        = this.ConvertConfiguracaoServerToComponenteGrafico(this.configuracaoPadraDTO.Configuracao);
                ConfiguracaoPadraoDTO configuracaoPadraoLocalDTO
                    = this.ConvertConfiguracaoPadraoServerToComponente(this.configuracaoPadraDTO);

               
                //Criando a nova instancia de gráfico
                Grafico novoGrafico = new Grafico(configuracaoPadraoLocalDTO, configuracaoNovoGraficoDTO, listaTemplates);
                
                //setando a marca dagua
                novoGrafico.SetMarcaDagua(ServiceWCF.MarcaDagua, ServiceWCF.MarcaDaguaLeft, ServiceWCF.MarcaDaguaTop, 
                    ServiceWCF.MarcaDaguaSize, ServiceWCF.MarcaDaguaWidth);

                novoGrafico.ShowHideBotaoAC(ServiceWCF.AnaliseCompartilhada);

                //Setando as opções que o usuário selecionou agora
                if (ServiceWCF.AtivoDireto == "")                    
                    novoGrafico.HabilitaVersaoCompleta(false, "");
                else
                    if (!ServiceWCF.Simpletrader)
                        novoGrafico.HabilitaVersaoCompleta(true, this.linkVersaoCompleta);
                    else
                        novoGrafico.HabilitaVersaoCompleta(false, "");

                novoGrafico.Ativo = novoGraficoObj.Ativo;
                novoGrafico.Periodicidade = novoGraficoObj.Periodicidade;
                novoGrafico.Periodo = novoGraficoObj.Periodo;
                novoGrafico.Bolsa = (int)RetornaBolsaAtivo(novoGrafico.Ativo);                
                
                //Setando os dados
                foreach (SoaMD.CotacaoServerDTO cotacaoObj in lista)
                {
                    if (cotacaoObj.Hora != "")
                        novoGrafico.Dados.Add(new BarraDTO(novoGrafico.Ativo,
                            new DateTime(cotacaoObj.Data.Year, cotacaoObj.Data.Month, cotacaoObj.Data.Day,
                                Convert.ToInt16(cotacaoObj.Hora.Substring(0, 2)), Convert.ToInt16(cotacaoObj.Hora.Substring(2, 2)), 0),
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                    else
                        novoGrafico.Dados.Add(new BarraDTO(novoGrafico.Ativo, cotacaoObj.Data,
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                }

                //Executa a montagem do gráfico
                novoGrafico.Refresh(EnumGeral.TipoRefresh.TudoMantemIndicadoresEObjetos);
                                
                //Adiciona na lista de gráficos
                listaGraficos.Add(novoGrafico);
                
                //Neste caso, estou recebendo as cotacaoes do servidor, portanto não preciso solicitar novamente
                CriaTabPainel(novoGrafico, false, false);
                
                //Verficando se deve aplicar template
                if (novoGraficoObj.TemplateSelecionado.Id > 0)
                {
                    listaGraficos[indexGraficoSelecionado].ConfiguracaoGraficoLocal
                        = this.ConvertConfiguracaoServerToComponenteGrafico(novoGraficoObj.TemplateSelecionado.Configuracao);

                    //Limpando os indicadores
                    listaGraficos[indexGraficoSelecionado].Indicadores.Clear();

                    foreach (TerminalWebSVC.IndicadorDTO indicador in novoGraficoObj.TemplateSelecionado.Indicadores)
                    {
                        listaGraficos[indexGraficoSelecionado].Indicadores.Add(ConverteIndicadorServerToComponente(indicador));
                    }

                    //Exeuctando o refresh
                    //AtualizaGrafico()
                    //novoGrafico.Refresh(EnumGeral.TipoRefresh.Tudo);
                }
                else
                {
                    
                }

                ThreadAtualizacaoGrafico(novoGrafico);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao carregar o gráfico.", "CarregaNovoGrafico", exc);
            }
        }

        /// <summary>
        /// Metodo que carrega um novo grafico
        /// </summary>
        /// <param name="novoGraficoObj"></param>
        private void CarregaNovoGraficoBMF(NovoGrafico novoGraficoObj, List<SoaMD.CotacaoServerDTO> lista)
        {
            try
            {
                ConfiguracaoGraficoDTO configuracaoNovoGraficoDTO
                        = this.ConvertConfiguracaoServerToComponenteGrafico(this.configuracaoPadraDTO.Configuracao);
                ConfiguracaoPadraoDTO configuracaoPadraoLocalDTO
                    = this.ConvertConfiguracaoPadraoServerToComponente(this.configuracaoPadraDTO);

                //Criando a nova instancia de gráfico
                Grafico novoGrafico = new Grafico(configuracaoPadraoLocalDTO, configuracaoNovoGraficoDTO, listaTemplates);

                //setando a marca dagua
                novoGrafico.SetMarcaDagua(ServiceWCF.MarcaDagua, ServiceWCF.MarcaDaguaLeft, ServiceWCF.MarcaDaguaTop,
                    ServiceWCF.MarcaDaguaSize, ServiceWCF.MarcaDaguaWidth);

                novoGrafico.ShowHideBotaoAC(ServiceWCF.AnaliseCompartilhada);

                //Setando as opções que o usuário selecionou agora
                if (ServiceWCF.AtivoDireto == "")
                    novoGrafico.HabilitaVersaoCompleta(false, "");
                else
                    if (!ServiceWCF.Simpletrader)
                        novoGrafico.HabilitaVersaoCompleta(true, this.linkVersaoCompleta);
                    else
                        novoGrafico.HabilitaVersaoCompleta(false, "");

                novoGrafico.Ativo = novoGraficoObj.Ativo;
                novoGrafico.Periodicidade = novoGraficoObj.Periodicidade;
                novoGrafico.Periodo = novoGraficoObj.Periodo;
                novoGrafico.Bolsa = (int)RetornaBolsaAtivo(novoGrafico.Ativo);

                //Setando os dados
                foreach (SoaMD.CotacaoServerDTO cotacaoObj in lista)
                {
                    if (cotacaoObj.Hora != "")
                        novoGrafico.Dados.Add(new BarraDTO(novoGrafico.Ativo,
                            new DateTime(cotacaoObj.Data.Year, cotacaoObj.Data.Month, cotacaoObj.Data.Day,
                                Convert.ToInt16(cotacaoObj.Hora.Substring(0, 2)), Convert.ToInt16(cotacaoObj.Hora.Substring(2, 2)), 0),
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                    else
                        novoGrafico.Dados.Add(new BarraDTO(novoGrafico.Ativo, cotacaoObj.Data,
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                }

                //Executa a montagem do gráfico
                novoGrafico.Refresh(EnumGeral.TipoRefresh.TudoMantemIndicadoresEObjetos);

                //Adiciona na lista de gráficos
                listaGraficos.Add(novoGrafico);

                //Neste caso, estou recebendo as cotacaoes do servidor, portanto não preciso solicitar novamente
                CriaTabPainel(novoGrafico, false, false);

                //Verficando se deve aplicar template
                if (novoGraficoObj.TemplateSelecionado.Id > 0)
                {
                    listaGraficos[indexGraficoSelecionado].ConfiguracaoGraficoLocal
                        = this.ConvertConfiguracaoServerToComponenteGrafico(novoGraficoObj.TemplateSelecionado.Configuracao);

                    //Limpando os indicadores
                    listaGraficos[indexGraficoSelecionado].Indicadores.Clear();

                    foreach (TerminalWebSVC.IndicadorDTO indicador in novoGraficoObj.TemplateSelecionado.Indicadores)
                    {
                        listaGraficos[indexGraficoSelecionado].Indicadores.Add(ConverteIndicadorServerToComponente(indicador));
                    }

                    //Exeuctando o refresh
                    //AtualizaGrafico()
                    //novoGrafico.Refresh(EnumGeral.TipoRefresh.Tudo);
                }
                else
                {

                }

                ThreadAtualizacaoGrafico(novoGrafico);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao carregar o gráfico.", "CarregaNovoGrafico", exc);
            }
        }

        /// <summary>
        /// Metodo que faz a publicação do gráfico
        /// </summary>
        private void PublicarGrafico()
        {
            try
            {
                ModuloCarregando(true, "Publicando a analise selecionada");
                Grafico obj = listaGraficos[indexGraficoSelecionado];
                //foreach (Grafico obj in listaGraficos)
                {
                    TerminalWebSVC.AnaliseCompartilhadaDTO analiseDTO = new TerminalWebSVC.AnaliseCompartilhadaDTO();
                    analiseDTO.Ativo = obj.Ativo;
                    analiseDTO.Comentario = listaGraficos[indexGraficoSelecionado].Comentario;

                    if (listaGraficos[indexGraficoSelecionado].Codigo < 0)
                        analiseDTO.GraficoId = 0;
                    else
                        analiseDTO.GraficoId = listaGraficos[indexGraficoSelecionado].Codigo;

                    analiseDTO.Data = DateTime.Now;
                    analiseDTO.Id = 0;
                    analiseDTO.UsuarioId = ServiceWCF.Usuario.Id;
                    analiseDTO.CaminhoImagem = "";
                    analiseDTO.DispararEmail = "";
                    analiseDTO.PublicoPrivado = "PU";

                    //PublicarAnalise publicarAnalise = new PublicarAnalise(obj, analiseDTO);

                    //publicarAnalise.Closing += (sender1, e1) =>
                    //{
                    //    ModuloCarregando(false);
                    //};

                    //publicarAnalise.Show();
                    TerminalWebSVC.GraficoDTO graficoCorretora = ConvertGraficoComponenteToServer(obj);
                    baseTerminalWebSVC.SalvarAnaliseCompartilhadaAsync(obj.SaveAsImage(), analiseDTO, ServiceWCF.MacroCliente, graficoCorretora);

                }
            }
            catch (Exception exc)
            {                
                throw exc;
            }
            
        }

        /// <summary>
        /// Metodo que altera um grafico existente
        /// </summary>
        /// <param name="novoGraficoObj"></param>
        private void AlteraGraficoBVSP(NovoGrafico novoGraficoObj, List<SoaMD.CotacaoServerDTO> lista)
        {
            try
            {                
                //Setando as opções que o usuário selecionou agora
                listaGraficos[indexGraficoSelecionado].Ativo = novoGraficoObj.Ativo;
                listaGraficos[indexGraficoSelecionado].Bolsa = (int)RetornaBolsaAtivo(novoGraficoObj.Ativo);
                listaGraficos[indexGraficoSelecionado].Periodicidade = novoGraficoObj.Periodicidade;
                listaGraficos[indexGraficoSelecionado].Periodo = novoGraficoObj.Periodo;
                if (tabControl.SelectedItem != null)
                    ((System.Windows.Controls.TabItem)tabControl.SelectedItem).Header = ObtemTituloAba(novoGraficoObj.Ativo, novoGraficoObj.Periodicidade.Value, novoGraficoObj.Periodo.Value);

                //Setando os dados
                listaGraficos[indexGraficoSelecionado].Dados.Clear();
                foreach (SoaMD.CotacaoServerDTO cotacaoObj in lista)
                {
                    if (cotacaoObj.Hora != "")
                        listaGraficos[indexGraficoSelecionado].Dados.Add(new BarraDTO(listaGraficos[indexGraficoSelecionado].Ativo,
                            new DateTime(cotacaoObj.Data.Year, cotacaoObj.Data.Month, cotacaoObj.Data.Day,
                                Convert.ToInt16(cotacaoObj.Hora.Substring(0, 2)), Convert.ToInt16(cotacaoObj.Hora.Substring(2, 2)), 0),
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                    else
                        listaGraficos[indexGraficoSelecionado].Dados.Add(new BarraDTO(listaGraficos[indexGraficoSelecionado].Ativo, cotacaoObj.Data,
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                }

                                
                //Verficando se deve aplicar template
                if (novoGraficoObj.TemplateSelecionado.Id > 0)
                {
                    listaGraficos[indexGraficoSelecionado].ConfiguracaoGraficoLocal
                        = this.ConvertConfiguracaoServerToComponenteGrafico(novoGraficoObj.TemplateSelecionado.Configuracao);

                    //Limpando os indicadores
                    listaGraficos[indexGraficoSelecionado].Indicadores.Clear();

                    foreach (TerminalWebSVC.IndicadorDTO indicador in novoGraficoObj.TemplateSelecionado.Indicadores)
                    {
                        listaGraficos[indexGraficoSelecionado].Indicadores.Add(ConverteIndicadorServerToComponente(indicador));
                    }
                    
                }

                //Exeuctando o refresh
                //AtualizaGrafico();
                listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.Tudo);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao alterar o gráfico.", "AlteraGrafico", exc);
            }
        }

        /// <summary>
        /// Metodo que altera um grafico existente
        /// </summary>
        /// <param name="novoGraficoObj"></param>
        private void AlteraGraficoBMF(NovoGrafico novoGraficoObj, List<SoaMD.CotacaoServerDTO> lista)
        {
            try
            {
                //Setando as opções que o usuário selecionou agora
                listaGraficos[indexGraficoSelecionado].Ativo = novoGraficoObj.Ativo;
                listaGraficos[indexGraficoSelecionado].Bolsa = (int)RetornaBolsaAtivo(novoGraficoObj.Ativo);
                listaGraficos[indexGraficoSelecionado].Periodicidade = novoGraficoObj.Periodicidade;
                listaGraficos[indexGraficoSelecionado].Periodo = novoGraficoObj.Periodo;
                if (tabControl.SelectedItem != null)
                    ((System.Windows.Controls.TabItem)tabControl.SelectedItem).Header = ObtemTituloAba(novoGraficoObj.Ativo, novoGraficoObj.Periodicidade.Value, novoGraficoObj.Periodo.Value);

                //Setando os dados
                listaGraficos[indexGraficoSelecionado].Dados.Clear();
                foreach (SoaMD.CotacaoServerDTO cotacaoObj in lista)
                {
                    if (cotacaoObj.Hora != "")
                        listaGraficos[indexGraficoSelecionado].Dados.Add(new BarraDTO(listaGraficos[indexGraficoSelecionado].Ativo,
                            new DateTime(cotacaoObj.Data.Year, cotacaoObj.Data.Month, cotacaoObj.Data.Day,
                                Convert.ToInt16(cotacaoObj.Hora.Substring(0, 2)), Convert.ToInt16(cotacaoObj.Hora.Substring(2, 2)), 0),
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                    else
                        listaGraficos[indexGraficoSelecionado].Dados.Add(new BarraDTO(listaGraficos[indexGraficoSelecionado].Ativo, cotacaoObj.Data,
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                }


                //Verficando se deve aplicar template
                if (novoGraficoObj.TemplateSelecionado.Id > 0)
                {
                    listaGraficos[indexGraficoSelecionado].ConfiguracaoGraficoLocal
                        = this.ConvertConfiguracaoServerToComponenteGrafico(novoGraficoObj.TemplateSelecionado.Configuracao);

                    //Limpando os indicadores
                    listaGraficos[indexGraficoSelecionado].Indicadores.Clear();

                    foreach (TerminalWebSVC.IndicadorDTO indicador in novoGraficoObj.TemplateSelecionado.Indicadores)
                    {
                        listaGraficos[indexGraficoSelecionado].Indicadores.Add(ConverteIndicadorServerToComponente(indicador));
                    }

                }

                //Exeuctando o refresh
                //AtualizaGrafico();
                listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.Tudo);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao alterar o gráfico.", "AlteraGrafico", exc);
            }
        }


        /// <summary>
        /// Metodo que atualiza um grafico existente
        /// </summary>
        private void CarregaGraficoExistenteBVSP(List<SoaMD.CotacaoServerDTO> lista)
        {
            try
            {
                bool graficoNovo = false;

                if (listaGraficos[indexGraficoSelecionado].Dados.Count == 0)
                    graficoNovo = true;

                //limpandos os dados
                listaGraficos[indexGraficoSelecionado].Dados.Clear();

                //Setando os dados
                foreach (SoaMD.CotacaoServerDTO cotacaoObj in lista)
                {
                    if (cotacaoObj.Hora != "")
                        listaGraficos[indexGraficoSelecionado].Dados.Add(new BarraDTO(listaGraficos[indexGraficoSelecionado].Ativo,
                            new DateTime(cotacaoObj.Data.Year, cotacaoObj.Data.Month, cotacaoObj.Data.Day,
                                Convert.ToInt16(cotacaoObj.Hora.Substring(0, 2)), Convert.ToInt16(cotacaoObj.Hora.Substring(2, 2)), 0),
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                    else
                        listaGraficos[indexGraficoSelecionado].Dados.Add(new BarraDTO(listaGraficos[indexGraficoSelecionado].Ativo, cotacaoObj.Data,
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                }

                listaGraficos[indexGraficoSelecionado].ResetaAlturaPainel();
                               

                //Atualizando o grafico
                if (graficoNovo)
                    listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.Tudo);
                else
                {
                    if (!aplicarTemplate)
                        listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.TudoMantemIndicadoresEObjetos);
                    else
                    {
                        aplicarTemplate = false;
                        listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.Tudo);
                    }
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao carregar um gráfico existente.", "CarregaGraficoExistente", exc);
            }
        }

        /// <summary>
        /// Metodo que atualiza um grafico existente
        /// </summary>
        private void CarregaGraficoExistenteBMF(List<SoaMD.CotacaoServerDTO> lista)
        {
            try
            {
                bool graficoNovo = false;

                if (listaGraficos[indexGraficoSelecionado].Dados.Count == 0)
                    graficoNovo = true;

                //limpandos os dados
                listaGraficos[indexGraficoSelecionado].Dados.Clear();

                //Setando os dados
                foreach (SoaMD.CotacaoServerDTO cotacaoObj in lista)
                {
                    if (cotacaoObj.Hora != "")
                        listaGraficos[indexGraficoSelecionado].Dados.Add(new BarraDTO(listaGraficos[indexGraficoSelecionado].Ativo,
                            new DateTime(cotacaoObj.Data.Year, cotacaoObj.Data.Month, cotacaoObj.Data.Day,
                                Convert.ToInt16(cotacaoObj.Hora.Substring(0, 2)), Convert.ToInt16(cotacaoObj.Hora.Substring(2, 2)), 0),
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                    else
                        listaGraficos[indexGraficoSelecionado].Dados.Add(new BarraDTO(listaGraficos[indexGraficoSelecionado].Ativo, cotacaoObj.Data,
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume, true));
                }

                listaGraficos[indexGraficoSelecionado].ResetaAlturaPainel();


                //Atualizando o grafico
                if (graficoNovo)
                    listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.Tudo);
                else
                {
                    if (!aplicarTemplate)
                        listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.TudoMantemIndicadoresEObjetos);
                    else
                    {
                        aplicarTemplate = false;
                        listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.Tudo);
                    }
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao carregar um gráfico existente.", "CarregaGraficoExistente", exc);
            }
        }

        /// <summary>
        /// Metodo que abre a tela de seleção de novo grafico
        /// </summary>
        /// <param name="listaTemplate"></param>
        private void AbrirDialogNovoGrafico(List<TerminalWebSVC.TemplateDTO> listaTemplate, bool mesmoGrafico)
        {
            try
            {
                mustUpdate = false;
                //Instanciando uma nova tela de novoGraficoDialog passando ativo vazio e periodicidade
                NovoGrafico novoGraficoDialog = new NovoGrafico(listaAtivosLocal, listaTemplate, mesmoGrafico);

                novoGraficoDialog.Closing += (sender1, e1) =>
                {
                    //Carregar dados no objeto do gráfico - termina de carregar no completed do evento
                    if (novoGraficoDialog.DialogResult == true)
                    {
                        //Colocando a tela de carregamento
                        ModuloCarregando(true);

                        if (RetornaBolsaAtivo(novoGraficoDialog.Ativo) == EnumLocal.Bolsa.Bovespa)
                        {
                            baseMarketDataCommon.GetHistoricoBovespaAsync(novoGraficoDialog.Ativo,
                                novoGraficoDialog.Periodicidade.Value, novoGraficoDialog.Periodo.Value,
                                true, false, !ServiceWCF.BovespaRT, DateTime.MinValue,
                                ServiceWCF.Usuario.Guid, novoGraficoDialog);
                        }
                        else
                        {
                            baseMarketDataCommon.GetHistoricoBMFAsync(novoGraficoDialog.Ativo,
                                novoGraficoDialog.Periodicidade.Value, novoGraficoDialog.Periodo.Value,
                                true, false, !ServiceWCF.BMFRT, DateTime.MinValue,
                                ServiceWCF.Usuario.Guid, novoGraficoDialog);
                        }

                        mustUpdate = true;
                    }
                    else
                        ModuloCarregando(false);
                };

                novoGraficoDialog.Show();
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao abrir o dialogo de grafico.", "AbrirDialogNovoGrafico", exc);
            }
        }


        /// <summary>
        /// Metodo que abre um gráfico avulso no simpletrader
        /// </summary>
        private void AbrirGraficoAvulsoSimpleTrader()
        {
            baseTerminalWebSVC.RetornaGraficoAsync(ServiceWCF.AtivoDireto, ServiceWCF.PeriodicidadeDireta, ServiceWCF.IdUsuario);
        }

        /// <summary>
        /// Metodo que abre um gráfico avulso de determinado ativo que foi passado diretamente
        /// </summary>
        private void AbrirGraficoAvulso()
        {
            ConfiguracaoGraficoDTO configuracaoNovoGraficoDTO
                    = this.ConvertConfiguracaoServerToComponenteGrafico(this.configuracaoPadraDTO.Configuracao);
            ConfiguracaoPadraoDTO configuracaoPadraoLocalDTO
                = this.ConvertConfiguracaoPadraoServerToComponente(this.configuracaoPadraDTO);

            Grafico graficoComponente = new Grafico(configuracaoPadraoLocalDTO, configuracaoNovoGraficoDTO, listaTemplates);

            graficoComponente.ShowHideBotaoAC(ServiceWCF.AnaliseCompartilhada);

            graficoComponente.SetMarcaDagua(ServiceWCF.MarcaDagua, ServiceWCF.MarcaDaguaLeft, ServiceWCF.MarcaDaguaTop,
                ServiceWCF.MarcaDaguaSize, ServiceWCF.MarcaDaguaWidth);

            if (ServiceWCF.AtivoDireto == "")
                graficoComponente.HabilitaVersaoCompleta(false, "");
            else
                if (!ServiceWCF.Simpletrader)
                    graficoComponente.HabilitaVersaoCompleta(true, this.linkVersaoCompleta);
                else
                    graficoComponente.HabilitaVersaoCompleta(false, "");

            graficoComponente.Ativo = ServiceWCF.AtivoDireto;
            if (!ServiceWCF.Simpletrader)
            {
                graficoComponente.Periodicidade = EnumPeriodicidade.Diario;
                graficoComponente.Periodo = EnumPeriodo.SeisMeses;
            }
            else
            {
                switch (ServiceWCF.PeriodicidadeDireta)
                {
                    case 1:
                        graficoComponente.Periodicidade = EnumPeriodicidade.UmMinuto;
                        graficoComponente.Periodo = EnumPeriodo.DoisDias;
                        break;
                    case 2:
                        graficoComponente.Periodicidade = EnumPeriodicidade.DoisMinutos;
                        graficoComponente.Periodo = EnumPeriodo.DoisDias;
                        break;
                    case 3:
                        graficoComponente.Periodicidade = EnumPeriodicidade.TresMinutos;
                        graficoComponente.Periodo = EnumPeriodo.CincoDias;
                        break;
                    case 5:
                        graficoComponente.Periodicidade = EnumPeriodicidade.CincoMinutos;
                        graficoComponente.Periodo = EnumPeriodo.CincoDias;
                        break;
                    case 10:
                        graficoComponente.Periodicidade = EnumPeriodicidade.DezMinutos;
                        graficoComponente.Periodo = EnumPeriodo.CincoDias;
                        break;
                    case 15:
                        graficoComponente.Periodicidade = EnumPeriodicidade.QuinzeMinutos;
                        graficoComponente.Periodo = EnumPeriodo.DezDias;
                        break;
                    case 30:
                        graficoComponente.Periodicidade = EnumPeriodicidade.TrintaMinutos;
                        graficoComponente.Periodo = EnumPeriodo.DezDias;
                        break;
                    case 60:
                        graficoComponente.Periodicidade = EnumPeriodicidade.UmaHora;
                        graficoComponente.Periodo = EnumPeriodo.DezDias;
                        break;
                    case 1440:
                        graficoComponente.Periodicidade = EnumPeriodicidade.Diario;
                        graficoComponente.Periodo = EnumPeriodo.SeisMeses;
                        break;
                    case 10080:
                        graficoComponente.Periodicidade = EnumPeriodicidade.Semanal;
                        graficoComponente.Periodo = EnumPeriodo.TresAnos;
                        break;
                    case 43200:
                        graficoComponente.Periodicidade = EnumPeriodicidade.Mensal;
                        graficoComponente.Periodo = EnumPeriodo.CincoAnos;
                        break;
                        
                }
            }

            graficoComponente.OnTrocaTemplate += new Grafico.OnTrocaTemplateDelegate(graficoComponente_OnTrocaTemplate);

            if (listaGraficos.Count == 0)
            {
                //Adicionando na lista de graficos
                listaGraficos.Add(graficoComponente);
            }
            


            graficoAvulso.Children.Add(graficoComponente);
            
            //assinando eventos do componente grafico
            graficoComponente.OnAlteraPeriodicidade += new Grafico.OnAlteraPeriodicidadeDelegate(Principal_OnAlteraPeriodicidade);
            graficoComponente.OnAlteraPeriodo += new Grafico.OnAlteraPeriodoDelegate(Principal_OnAlteraPeriodo);
            graficoComponente.OnAtualizaAtivo += new Grafico.OnAtualizaAtivoDelegate(Principal_OnAtualizaAtivo);
            graficoComponente.OnAtualizaDados += new Grafico.OnAtualizaDadosDelegate(Principal_OnAtualizaDados);
            graficoComponente.OnVerificaAnaliseCompartilhada += new Grafico.OnVerificaAnaliseCompartilhadaDelegate(novoGrafico_OnVerificaAnaliseCompartilhada);
            graficoComponente.OnHelp += new Grafico.OnHelpDelegate(novoGrafico_OnHelp);
            graficoComponente.OnAlteraTipoVolume += new Grafico.OnAlteraTipoVolumeDelegate(novoGrafico_OnAlteraTipoVolume);

            //Assinando tick para novo gráfico
            //AssinaCotacao(graficoComponente.Ativo);
            
            //plotando
            indexGraficoSelecionado = 0;
            AtualizaGrafico(true);            
            
        }

        /// <summary>
        /// Evento disparado ao se trocar o template no componente
        /// </summary>
        /// <param name="grafico"></param>
        void graficoComponente_OnTrocaTemplate(Grafico grafico)
        {
            ModuloCarregando(true, "Aplicando Template");
            baseTerminalWebSVC.RetornaTemplatePorIdAsync(grafico.templateSelecionado);            
        }

        void baseFreeStockChartPlus_RetornaTemplatePorIdCompleted(object sender, TerminalWebSVC.RetornaTemplatePorIdCompletedEventArgs e)
        {
            ////aplicar o template no gráfico selecionado                            
            listaGraficos[indexGraficoSelecionado].ConfiguracaoGraficoLocal = this.ConvertConfiguracaoServerToComponenteGrafico(e.Result.Configuracao);
            listaGraficos[indexGraficoSelecionado].Periodo = EnumPeriodo.GetPeriodo((int)e.Result.Periodo);
            listaGraficos[indexGraficoSelecionado].Periodicidade = EnumPeriodicidade.GetPeriodicidade((int)e.Result.Periodicidade);
            listaGraficos[indexGraficoSelecionado].Indicadores.Clear();

            foreach (TerminalWebSVC.IndicadorDTO indicador in e.Result.Indicadores)
            {
                listaGraficos[indexGraficoSelecionado].Indicadores.Add(ConverteIndicadorServerToComponente(indicador));
            }

            //Exeuctando o refresh
            aplicarTemplate = true;
            AtualizaGrafico(false);
        }

        #endregion Grafico

        #region Templates

        /// <summary>
        /// Metodo salva o template
        /// </summary>
        private void SalvarTemplate(List<TerminalWebSVC.TemplateDTO> listaTemplate)
        {
            try
            {
                if (indexGraficoSelecionado > -1)
                {
                    SalvaTemplate salvaTemplateDialog = new SalvaTemplate(listaTemplate);
                    salvaTemplateDialog.Closing += (sender1, e1) =>
                    {
                        if (salvaTemplateDialog.DialogResult == true)
                        {
                            if (salvaTemplateDialog.Novo)
                                InserirTemplate(salvaTemplateDialog.Nome);
                            else
                                EditarTemplate(salvaTemplateDialog.TemplateSelecionado);

                        }
                        else
                            ModuloCarregando(false);
                    };
                    salvaTemplateDialog.Show();
                }
                else
                {
                    MessageBox.Show("Você não possui gráfico ativo. Por favor abra um gráfico antes de salvar o template.");
                    ModuloCarregando(false);
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao salvar o template.", "SalvarTemplate", exc);
            }
            
        }

        /// <summary>
        /// Metodo que insere um novo template
        /// </summary>
        /// <param name="nome"></param>
        private void InserirTemplate(string nome)
        {
            TerminalWebSVC.TemplateDTO templateLocalDTO = new TerminalWebSVC.TemplateDTO();
            
            try
            {
                //Salvando o template novo com nome
                templateLocalDTO.Nome = nome;
                templateLocalDTO.Configuracao = ConvertConfiguracaoComponenteGraficoToServer(listaGraficos[indexGraficoSelecionado].ConfiguracaoGraficoLocal);
                //
                templateLocalDTO.Id = 0;
                templateLocalDTO.ClienteId = ServiceWCF.IdUsuario;
                templateLocalDTO.Indicadores = new List<TerminalWebSVC.IndicadorDTO>();
                listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.TudoMantemIndicadoresEObjetos);
                foreach (IndicadorDTO indicadorGrafico in listaGraficos[indexGraficoSelecionado].Indicadores)
                {
                    templateLocalDTO.Indicadores.Add(this.ConverteIndicadorComponenteToServer(indicadorGrafico));
                }
                
                templateLocalDTO.Periodicidade = listaGraficos[indexGraficoSelecionado].Periodicidade.Value;
                templateLocalDTO.Periodo = listaGraficos[indexGraficoSelecionado].Periodo.Value;

                baseTerminalWebSVC.SalvaTemplateAsync(templateLocalDTO);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao inserir o template.", "InserirTemplate", exc);
            }
        }

        /// <summary>
        /// Metodo que edita um template existente
        /// </summary>
        /// <param name="nome"></param>
        private void EditarTemplate(TerminalWebSVC.TemplateDTO templateLocalDTO)
        {
            
            try
            {
                //Salvando o template novo com nome
                listaGraficos[indexGraficoSelecionado].Refresh(EnumGeral.TipoRefresh.TudoMantemIndicadoresEObjetos);
                templateLocalDTO.Configuracao = ConvertConfiguracaoComponenteGraficoToServer(listaGraficos[indexGraficoSelecionado].ConfiguracaoGraficoLocal);
                templateLocalDTO.Indicadores.Clear();
                foreach (IndicadorDTO indicadorGrafico in listaGraficos[indexGraficoSelecionado].Indicadores)
                {
                    templateLocalDTO.Indicadores.Add(this.ConverteIndicadorComponenteToServer(indicadorGrafico));
                }

                templateLocalDTO.Periodicidade = listaGraficos[indexGraficoSelecionado].Periodicidade.Value;
                templateLocalDTO.Periodo = listaGraficos[indexGraficoSelecionado].Periodo.Value;

                baseTerminalWebSVC.SalvaTemplateAsync(templateLocalDTO);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao editar o template.", "EditarTemplate", exc);
            }
        }

        /// <summary>
        /// Metodo que dispara a tela de exclusão de tempalte
        /// </summary>
        /// <param name="listaTemplate"></param>
        private void ExcluirTemplate(List<TerminalWebSVC.TemplateDTO> listaTemplate)
        {
            
            try
            {
                if (listaTemplate.Count > 0)
                {
                    SelecaoTemplate dialogSelecaoTemplate = new SelecaoTemplate(listaTemplate);

                    dialogSelecaoTemplate.Closing += (sender1, e1) =>
                    {
                        if (dialogSelecaoTemplate.DialogResult == true)
                        {
                            //Realizando a ordem para exclusão do template
                            baseTerminalWebSVC.ExcluiTemplateAsync(dialogSelecaoTemplate.TemplateLocal);
                        }
                        else
                            ModuloCarregando(false);
                    };

                    dialogSelecaoTemplate.Show();
                }
                else
                {
                    ModuloCarregando(false);
                    MessageBox.Show("Você não possui nenhum template para excluir.");
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao excluir o template.", "ExcluirTemplate", exc);
            }
        }

        /// <summary>
        /// Metodo que deve aplicar um template sobre o gráfico selecionado
        /// </summary>
        /// <param name="templateLocalDTO"></param>
        private void AplicarTemplate(List<TerminalWebSVC.TemplateDTO> listaTemplate)
        {
            try
            {
                if (listaTemplate.Count > 0)
                {
                    SelecaoTemplate dialogSelecaoTemplate = new SelecaoTemplate(listaTemplate);

                    dialogSelecaoTemplate.Closing += (sender1, e1) =>
                    {
                        if (dialogSelecaoTemplate.DialogResult == true)
                        {

                            ////aplicar o template no gráfico selecionado                            
                            listaGraficos[indexGraficoSelecionado].ConfiguracaoGraficoLocal = this.ConvertConfiguracaoServerToComponenteGrafico(dialogSelecaoTemplate.TemplateLocal.Configuracao);
                            listaGraficos[indexGraficoSelecionado].Periodo = EnumPeriodo.GetPeriodo((int)dialogSelecaoTemplate.TemplateLocal.Periodo);
                            listaGraficos[indexGraficoSelecionado].Periodicidade = EnumPeriodicidade.GetPeriodicidade((int)dialogSelecaoTemplate.TemplateLocal.Periodicidade);
                            listaGraficos[indexGraficoSelecionado].Indicadores.Clear();

                            foreach (TerminalWebSVC.IndicadorDTO indicador in dialogSelecaoTemplate.TemplateLocal.Indicadores)
                            {
                                listaGraficos[indexGraficoSelecionado].Indicadores.Add(ConverteIndicadorServerToComponente(indicador));
                            }

                            //Exeuctando o refresh
                            aplicarTemplate = true;
                            AtualizaGrafico(false);
                        }
                    };

                    dialogSelecaoTemplate.Show();
                }
                else
                {
                    MessageBox.Show("Você não possui nenhum template para excluir.");
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao aplicar o template.", "AplicarTemplate", exc);
            }
            finally
            {
                ModuloCarregando(false);
            }
        }

        /// <summary>
        /// Metodo que vai retorna uma lista de indicadores padroes que devem ser aplicados no 
        /// gráfico
        /// </summary>
        private List<TerminalWebSVC.IndicadorDTO> RetornaIndicadoresPadrao()
        {
            try
            {
                TerminalWebSVC.IndicadorDTO indicador1 = new TerminalWebSVC.IndicadorDTO();
                return null;
            }
            catch (Exception exc)
            {                
                throw exc;
            }
        }

        #endregion Templates

        #region Conversores

        /// <summary>
        /// Converte o objeto do servidor para o tipo no componente
        /// </summary>
        /// <param name="configuracaoCliente"></param>
        /// <returns></returns>
        private ConfiguracaoPadraoDTO ConvertConfiguracaoPadraoServerToComponente(TerminalWebSVC.ConfiguracaoPadraoDTO configuracaoServer)
        {
            ConfiguracaoPadraoDTO configuracaoClient = new ConfiguracaoPadraoDTO();
            configuracaoClient.PeriodicidadeDiaria = EnumPeriodicidade.Diario;
            configuracaoClient.PeriodicidadeIntraday = EnumPeriodicidade.CincoMinutos;

            //Retornando o objeto no padrao do componente
            return configuracaoClient;
        }

        /// <summary>
        /// Retorna um objeto de configuração de gráfico a partir da configuração padrão
        /// </summary>
        /// <param name="configuracaoCliente"></param>
        /// <returns></returns>
        private ConfiguracaoGraficoDTO ConvertConfiguracaoServerToComponenteGrafico(TerminalWebSVC.ConfiguracaoGraficoDTO configuracaoServer)
        {
            ConfiguracaoGraficoDTO configuracaoClient = new ConfiguracaoGraficoDTO();
                        
            //Convertendo e sobrepondo as propriedades
            configuracaoClient.CorBordaCandleAlta = ConvertUtil.ConvertFromStringToColor(configuracaoServer.CorBordaCandleAlta);
            configuracaoClient.CorBordaCandleBaixa = ConvertUtil.ConvertFromStringToColor(configuracaoServer.CorBordaCandleBaixa);
            configuracaoClient.CorCandleAlta = ConvertUtil.ConvertFromStringToColor(configuracaoServer.CorCandleAlta);
            configuracaoClient.CorCandleBaixa = ConvertUtil.ConvertFromStringToColor(configuracaoServer.CorCandleBaixa);
            configuracaoClient.CorFundo = ConvertUtil.ConvertFromStringToBrush(configuracaoServer.CorFundo);
            configuracaoClient.CorIndicadorPadrao = ConvertUtil.ConvertFromStringToColor(configuracaoServer.CorIndicador);
            configuracaoClient.CorObjetoDefault = ConvertUtil.ConvertFromStringToBrush(configuracaoServer.CorObjeto);
            configuracaoClient.CorSerieFilha1Padrao = ConvertUtil.ConvertFromStringToColor(configuracaoServer.CorIndicadorAux1);
            configuracaoClient.CorSerieFilha2Padrao = ConvertUtil.ConvertFromStringToColor(configuracaoServer.CorIndicadorAux2);
            configuracaoClient.DarvaBox = (bool)configuracaoServer.DarvaBox;
            configuracaoClient.EspacoADireitaGrafico = (double)configuracaoServer.EspacoADireitaDoGrafico;
            configuracaoClient.EstiloBarra = (EnumGeral.TipoSeriesEnum)configuracaoServer.EstiloBarra;
            configuracaoClient.EstiloPreco = (EnumGeral.EstiloPrecoEnum)configuracaoServer.EstiloPreco;
            configuracaoClient.EstiloPrecoParam1 = (double)configuracaoServer.EstiloPrecoParam1;
            configuracaoClient.EstiloPrecoParam2 = (double)configuracaoServer.EstiloPrecoParam2;
            configuracaoClient.GradeHorizontal = (bool)configuracaoServer.GradeHorizontal;
            configuracaoClient.GradeVertical = (bool)configuracaoServer.GradeVertical;
            configuracaoClient.GrossuraLinhaDefault = (int)configuracaoServer.GrossuraObjeto;
            configuracaoClient.GrossuraSerieFilha1Padrao = (int)configuracaoServer.GrossuraIndicadorAux1;
            configuracaoClient.GrossuraSerieFilha2Padrao = (int)configuracaoServer.GrossuraIndicadorAux2;
            configuracaoClient.NivelZoom = 15;
            configuracaoClient.PainelInfo = (bool)configuracaoServer.PainelInfo;
            configuracaoClient.PosicaoEscala = (EnumGeral.TipoAlinhamentoEscalaEnum)configuracaoServer.PosicaoEscala;
            configuracaoClient.PrecisaoEscala = (int)configuracaoServer.PrecisaoEscala;
            //configuracaoClient.TipoEscala = (EnumGeral.TipoEscala)configuracaoServer.TipoEscala;
            configuracaoClient.TipoEscala = EnumGeral.TipoEscala.Semilog;
            configuracaoClient.TipoLinhaDefault = (EnumGeral.TipoLinha) configuracaoServer.TipoLinhaIndicador;
            configuracaoClient.TipoLinhaSerieFilha1Padrao = (EnumGeral.TipoLinha) configuracaoServer.TipoLinhaIndicadorAux1;
            configuracaoClient.TipoLinhaSerieFilha2Padrao = (EnumGeral.TipoLinha)configuracaoServer.TipoLinhaIndicadorAux2;
            configuracaoClient.TipoLinhaDefault = (EnumGeral.TipoLinha)configuracaoServer.TipoLinhaObjeto;
            configuracaoClient.TipoVolume = configuracaoServer.TipoVolume;
            configuracaoClient.UsarCoresAltaBaixaVolume = (bool)configuracaoServer.UsarCoresAltaBaixaVolume;
            //configuracaoClient.PainelLateralIndicadores = true;


            //Retornando o objeto no padrao do componente
            return configuracaoClient;
        }

        /// <summary>
        /// Retorna um objeto de configuração de gráfico a partir da configuração padrão
        /// </summary>
        /// <param name="configuracaoCliente"></param>
        /// <returns></returns>
        private TerminalWebSVC.ConfiguracaoGraficoDTO ConvertConfiguracaoComponenteGraficoToServer(Traderdata.Client.Componente.GraficoSL.DTO.ConfiguracaoGraficoDTO configuracaoCliente)
        {
            try
            {
                TerminalWebSVC.ConfiguracaoGraficoDTO configuracaoServer = new TerminalWebSVC.ConfiguracaoGraficoDTO();
                configuracaoServer.CorBordaCandleAlta = ConvertUtil.ConvertFromColorToString((Color)configuracaoCliente.CorBordaCandleAlta);
                configuracaoServer.CorBordaCandleBaixa = ConvertUtil.ConvertFromColorToString((Color)configuracaoCliente.CorBordaCandleBaixa);
                configuracaoServer.CorCandleAlta = ConvertUtil.ConvertFromColorToString((Color)configuracaoCliente.CorCandleAlta);
                configuracaoServer.CorCandleBaixa = ConvertUtil.ConvertFromColorToString((Color)configuracaoCliente.CorCandleBaixa);
                configuracaoServer.CorFundo = ConvertUtil.ConvertFromBrushToString(configuracaoCliente.CorFundo);
                configuracaoServer.CorIndicador = ConvertUtil.ConvertFromColorToString((Color)configuracaoCliente.CorIndicadorPadrao);
                configuracaoServer.CorObjeto = ConvertUtil.ConvertFromBrushToString(configuracaoCliente.CorObjetoDefault);
                configuracaoServer.CorIndicadorAux1 = ConvertUtil.ConvertFromColorToString(configuracaoCliente.CorSerieFilha1Padrao);
                configuracaoServer.CorIndicadorAux2 = ConvertUtil.ConvertFromColorToString(configuracaoCliente.CorSerieFilha2Padrao);
                configuracaoServer.DarvaBox = (bool)configuracaoCliente.DarvaBox;
                configuracaoServer.EspacoADireitaDoGrafico = (double)configuracaoCliente.EspacoADireitaGrafico;
                configuracaoServer.EstiloBarra = (int)configuracaoCliente.EstiloBarra;
                configuracaoServer.EstiloPreco = (int)configuracaoCliente.EstiloPreco;
                configuracaoServer.EstiloPrecoParam1 = (double)configuracaoCliente.EstiloPrecoParam1;
                configuracaoServer.EstiloPrecoParam2 = (double)configuracaoCliente.EstiloPrecoParam2;
                configuracaoServer.GradeHorizontal = (bool)configuracaoCliente.GradeHorizontal;
                configuracaoServer.GradeVertical = (bool)configuracaoCliente.GradeVertical;
                configuracaoServer.GrossuraObjeto = (int)configuracaoCliente.GrossuraLinhaDefault;
                configuracaoServer.GrossuraIndicadorAux1 = (int)configuracaoCliente.GrossuraSerieFilha1Padrao;
                configuracaoServer.GrossuraIndicadorAux2 = (int)configuracaoCliente.GrossuraSerieFilha2Padrao;
                //configuracaoServer.NivelZoom = 5;
                configuracaoServer.PainelInfo = (bool)configuracaoCliente.PainelInfo;
                configuracaoServer.PosicaoEscala = (int)configuracaoCliente.PosicaoEscala;
                configuracaoServer.PrecisaoEscala = (int)configuracaoCliente.PrecisaoEscala;
                configuracaoServer.TipoEscala = (int)configuracaoCliente.TipoEscala;
                configuracaoServer.TipoLinhaIndicador = (int)configuracaoCliente.TipoLinhaDefault;
                configuracaoServer.TipoLinhaIndicadorAux1 = (int)configuracaoCliente.TipoLinhaSerieFilha1Padrao;
                configuracaoServer.TipoLinhaIndicadorAux2 = (int)configuracaoCliente.TipoLinhaSerieFilha2Padrao;
                configuracaoServer.TipoLinhaObjeto = (int)configuracaoCliente.TipoLinhaDefault;
                configuracaoServer.UsarCoresAltaBaixaVolume = configuracaoCliente.UsarCoresAltaBaixaVolume;
                configuracaoServer.LinhaMagnetica = configuracaoCliente.LinhaMagnetica;
                configuracaoServer.LinhaTendenciaInfinita = configuracaoCliente.UsarLinhaInfinita;
                configuracaoServer.ConfigFiboRetracements = configuracaoCliente.ConfiguracaoRetracement;
                configuracaoServer.TipoVolume = configuracaoCliente.TipoVolume;

                //Retornando o objeto no padrao do componente
                return configuracaoServer;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao converter o tipo de configuração.", "ConvertConfiguracaoComponenteGraficoToServer", exc);
                throw exc;
            }
        }

        /// <summary>
        /// Metodo converte o indicadorDTO do componente para o formato no servidor
        /// </summary>
        /// <param name="indicadorGrafico"></param>
        /// <returns></returns>
        private TerminalWebSVC.IndicadorDTO ConverteIndicadorComponenteToServer(Traderdata.Client.Componente.GraficoSL.DTO.IndicadorDTO indicadorGrafico)
        {
            TerminalWebSVC.IndicadorDTO indicadorServer = new TerminalWebSVC.IndicadorDTO();
            try
            {
                indicadorServer.Cor = Util.ConvertUtil.ConvertFromColorToString(indicadorGrafico.CorAlta);
                indicadorServer.CorFilha1 = Util.ConvertUtil.ConvertFromColorToString(indicadorGrafico.CorSerieFilha1);
                indicadorServer.CorFilha2 = Util.ConvertUtil.ConvertFromColorToString(indicadorGrafico.CorSerieFilha2);

                indicadorServer.Espessura = indicadorGrafico.Grossura;
                indicadorServer.EspessuraFilha1 = indicadorGrafico.GrossuraSerieFilha1;
                indicadorServer.EspessuraFilha2 = indicadorGrafico.GrossuraSerieFilha2;

                indicadorServer.TipoLinha = (int)indicadorGrafico.TipoLinha;
                indicadorServer.TipoLinhaFilha1 = (int)indicadorGrafico.TipoLinhaSerieFilha1;
                indicadorServer.TipoLinhaFilha2 = (int)indicadorGrafico.TipoLinhaSerieFilha2;

                indicadorServer.IndexPainel = indicadorGrafico.IndexPainel;
                indicadorServer.Parametros = indicadorGrafico.Parametros;
                indicadorServer.TipoIndicador = indicadorGrafico.TipoIndicador;

                indicadorServer.AlturaPainel = indicadorGrafico.AlturaPainel;
                indicadorServer.StatusPainel = indicadorGrafico.StatusPainel;

                indicadorServer.PainelIndicador = indicadorGrafico.PainelIndicadoresLateral;
                indicadorServer.PainelPreco = indicadorGrafico.PainelPreco;
                indicadorServer.PainelVolume = indicadorGrafico.PainelVolume;
                indicadorServer.PainelAbaixoPreco = indicadorGrafico.PainelIndicadoresAbaixo;

                return indicadorServer;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao converter o tipo indicador.", "ConverteIndicadorComponenteToServer", exc);
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que converte o objeto gráfico plotado no gráfico para o objeto do servidor
        /// </summary>
        /// <param name="objetoGrafico"></param>
        /// <returns></returns>
        private TerminalWebSVC.ObjetoEstudoDTO ConverteObjetoComponenteToServer(Traderdata.Client.Componente.GraficoSL.DTO.ObjetoEstudoDTO objetoGrafico)
        {
            TerminalWebSVC.ObjetoEstudoDTO objetoServer = new TerminalWebSVC.ObjetoEstudoDTO();
            try
            {
                objetoServer.CorObjeto = Util.ConvertUtil.ConvertFromBrushToString(objetoGrafico.Cor);
                objetoServer.DataFinal = objetoGrafico.DataFinal;
                objetoServer.DataInicial = objetoGrafico.DataInicial;
                objetoServer.Espessura = objetoGrafico.Grossura;
                objetoServer.Id = 0;
                objetoServer.IndexPainel = objetoGrafico.IndexPainel;
                objetoServer.Suporte = objetoGrafico.Suporte;
                objetoServer.Resistencia = objetoGrafico.Resistencia;                
                objetoServer.InfinitaADireita = objetoGrafico.InfinitaADireita;
                objetoServer.Magnetica = objetoGrafico.Magnetica;
                objetoServer.Parametros = objetoGrafico.Parametros;
                objetoServer.RecordFinal = objetoGrafico.RecordFinal;
                objetoServer.RecordInicial = objetoGrafico.RecordInicial;
                //TODO: não há correspondente
                objetoServer.TamanhoTexto = 0;
                objetoServer.Texto = "";
                objetoServer.TipoLinha = (int)objetoGrafico.TipoLinha;
                objetoServer.TipoObjeto = objetoGrafico.TipoObjeto;
                objetoServer.ValorErrorChannel = 0;
                objetoServer.ValorFinal = objetoGrafico.ValorFinal;
                objetoServer.ValorInicial = objetoGrafico.ValorInicial;
                objetoServer.X1 = objetoGrafico.X1;
                objetoServer.X2 = objetoGrafico.X2;
                objetoServer.Y1 = objetoGrafico.Y1;
                objetoServer.Y2 = objetoGrafico.Y2;
                
                objetoServer.PainelIndicador = objetoGrafico.PainelIndicadores;
                
                return objetoServer;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao converter o tipo objeto.", "ConverteObjetoComponenteToServer", exc);
                throw exc;
            }
        }

        /// <summary>
        /// Metodo converte o objeto server to componente
        /// </summary>
        /// <returns></returns>
        private ObjetoEstudoDTO ConverteObjetoServerToComponente(TerminalWebSVC.ObjetoEstudoDTO objetoGrafico)
        {

            Traderdata.Client.Componente.GraficoSL.DTO.ObjetoEstudoDTO objetoComponente = new ObjetoEstudoDTO();
            try
            {
                objetoComponente.Cor = Util.ConvertUtil.ConvertFromStringToBrush(objetoGrafico.CorObjeto);
                objetoComponente.DataFinal = (DateTime)objetoGrafico.DataFinal;
                objetoComponente.DataInicial = (DateTime)objetoGrafico.DataInicial;
                objetoComponente.Grossura = (int)objetoGrafico.Espessura;
                objetoComponente.IndexPainel = (int)objetoGrafico.IndexPainel;
                objetoComponente.InfinitaADireita = (bool)objetoGrafico.InfinitaADireita;
                objetoComponente.Magnetica = (bool)objetoGrafico.Magnetica;
                objetoComponente.Parametros = objetoGrafico.Parametros;
                objetoComponente.RecordFinal = (int)objetoGrafico.RecordFinal;
                objetoComponente.RecordInicial = (int)objetoGrafico.RecordInicial;
                objetoComponente.TipoLinha = (EnumGeral.TipoLinha)objetoGrafico.TipoLinha;
                objetoComponente.TipoObjeto = (int)objetoGrafico.TipoObjeto;
                objetoComponente.Suporte = objetoGrafico.Suporte;
                objetoComponente.Resistencia = objetoGrafico.Resistencia;
                objetoComponente.ValorFinal = (double)objetoGrafico.ValorFinal;
                objetoComponente.ValorInicial = (double)objetoGrafico.ValorInicial;
                objetoComponente.X1 = (double)objetoGrafico.X1;
                objetoComponente.X2 = (double)objetoGrafico.X2;
                objetoComponente.Y1 = (double)objetoGrafico.Y1;
                objetoComponente.Y2 = (double)objetoGrafico.Y2;
                objetoComponente.PainelIndicadores = objetoGrafico.PainelIndicador;

                return objetoComponente;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao converter o tipo objeto.", "ConverteObjetoServerToComponente", exc);
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que converte um grafico do componente para o servidor
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private TerminalWebSVC.GraficoDTO ConvertGraficoComponenteToServer(Grafico obj)
        {
            try
            {
                TerminalWebSVC.GraficoDTO graficoServer = new TerminalWebSVC.GraficoDTO();
                graficoServer.Ativo = obj.Ativo;
                graficoServer.Configuracao = ConvertConfiguracaoComponenteGraficoToServer(obj.ConfiguracaoGraficoLocal);
                graficoServer.Id = 0;
                graficoServer.Periodicidade = obj.Periodicidade.Value;
                graficoServer.Periodo = obj.Periodo.Value;
                graficoServer.Indicadores = new List<TerminalWebSVC.IndicadorDTO>();
                graficoServer.Objetos = new List<TerminalWebSVC.ObjetoEstudoDTO>();
                graficoServer.Comentario = obj.Comentario;
                
                foreach(IndicadorDTO indicador in obj.Indicadores)
                {
                    graficoServer.Indicadores.Add(ConverteIndicadorComponenteToServer(indicador));
                }

                foreach (ObjetoEstudoDTO objeto in obj.Objetos)
                {
                    graficoServer.Objetos.Add(ConverteObjetoComponenteToServer(objeto));
                }

                return graficoServer;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao converter o tipo grafico.", "ConvertGraficoComponenteToServer", exc);
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que converte do servidor para grafico componente
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        private Grafico ConvertGraficoServerToComponente(TerminalWebSVC.GraficoDTO obj)
        {
            try
            {
                Grafico graficoComponente = new Grafico(ConvertConfiguracaoPadraoServerToComponente(this.configuracaoPadraDTO),
                    this.ConvertConfiguracaoServerToComponenteGrafico(obj.Configuracao), listaTemplates);

                graficoComponente.SetMarcaDagua(ServiceWCF.MarcaDagua, ServiceWCF.MarcaDaguaLeft, ServiceWCF.MarcaDaguaTop,
                    ServiceWCF.MarcaDaguaSize, ServiceWCF.MarcaDaguaWidth);

                if (ServiceWCF.AtivoDireto == "")
                    graficoComponente.HabilitaVersaoCompleta(false, "");
                else
                    graficoComponente.HabilitaVersaoCompleta(true, this.linkVersaoCompleta);

                graficoComponente.SetStockchartSymbol(obj.Ativo);
                graficoComponente.Ativo = obj.Ativo;
                graficoComponente.Bolsa = (int)RetornaBolsaAtivo(obj.Ativo);
                graficoComponente.Codigo = obj.Id;
                graficoComponente.ConfiguracaoGraficoLocal = ConvertConfiguracaoServerToComponenteGrafico(obj.Configuracao);
                graficoComponente.Periodicidade = EnumPeriodicidade.GetPeriodicidade(obj.Periodicidade);
                graficoComponente.Periodo = EnumPeriodo.GetPeriodo(obj.Periodo);
                graficoComponente.Comentario = obj.Comentario;

                List<IndicadorDTO> indicadoresAux = graficoComponente.Indicadores;
                foreach (TerminalWebSVC.IndicadorDTO indicador in obj.Indicadores)
                {
                    indicadoresAux.Add(ConverteIndicadorServerToComponente(indicador));
                }

                List<ObjetoEstudoDTO> objetosAux = graficoComponente.Objetos;
                foreach (TerminalWebSVC.ObjetoEstudoDTO objeto in obj.Objetos)
                {
                    objetosAux.Add(ConverteObjetoServerToComponente(objeto));
                }

                return graficoComponente;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao converter o tipo grafico.", "ConvertGraficoServerToComponente", exc);
                throw exc;
            }
        }

        /// <summary>
        /// Metodo converte o indicadorDTO do componente para o formato no servidor
        /// </summary>
        /// <param name="indicadorGrafico"></param>
        /// <returns></returns>
        private Traderdata.Client.Componente.GraficoSL.DTO.IndicadorDTO ConverteIndicadorServerToComponente(TerminalWebSVC.IndicadorDTO indicadorServer)
        {
            IndicadorDTO indicadorGrafico = new IndicadorDTO();
            try
            {
                indicadorGrafico.CorAlta = Util.ConvertUtil.ConvertFromStringToColor(indicadorServer.Cor);
                indicadorGrafico.CorSerieFilha1 = Util.ConvertUtil.ConvertFromStringToColor(indicadorServer.CorFilha1);
                indicadorGrafico.CorSerieFilha2 = Util.ConvertUtil.ConvertFromStringToColor(indicadorServer.CorFilha2);     

                indicadorGrafico.Grossura = (int)indicadorServer.Espessura;
                indicadorGrafico.GrossuraSerieFilha1 = (int)indicadorServer.EspessuraFilha1;
                indicadorGrafico.GrossuraSerieFilha2 = (int)indicadorServer.EspessuraFilha2;

                indicadorGrafico.IndexPainel = (int)indicadorServer.IndexPainel;
                indicadorGrafico.Parametros = indicadorServer.Parametros;
                indicadorGrafico.TipoIndicador = (int)indicadorServer.TipoIndicador;

                indicadorGrafico.TipoLinha = (EnumGeral.TipoLinha)indicadorServer.TipoLinha;

                if (indicadorServer.TipoLinhaFilha1 != null)
                    indicadorGrafico.TipoLinhaSerieFilha1 = (EnumGeral.TipoLinha)indicadorServer.TipoLinhaFilha1;
                else
                    indicadorGrafico.TipoLinhaSerieFilha1 = EnumGeral.TipoLinha.Solido; //se nao tiver nada cadastrado colocar como sólido

                if (indicadorServer.TipoLinhaFilha2 != null)
                    indicadorGrafico.TipoLinhaSerieFilha2 = (EnumGeral.TipoLinha)indicadorServer.TipoLinhaFilha2;
                else
                    indicadorGrafico.TipoLinhaSerieFilha2 = EnumGeral.TipoLinha.Solido;

                indicadorGrafico.AlturaPainel = indicadorServer.AlturaPainel;
                indicadorGrafico.StatusPainel = indicadorServer.StatusPainel;
                indicadorGrafico.PainelIndicadoresLateral = indicadorServer.PainelIndicador;
                indicadorGrafico.PainelPreco = indicadorServer.PainelPreco;
                indicadorGrafico.PainelVolume= indicadorServer.PainelVolume;
                indicadorGrafico.PainelIndicadoresAbaixo= indicadorServer.PainelAbaixoPreco;

                return indicadorGrafico;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao converter o tipo idnicador.", "ConverteIndicadorServerToComponente", exc);
                throw exc;
            }
        }

        #endregion Conversores

        #region Metodos Auxiliares

        #region CriaTabPainel()
        /// <summary>
        /// O parametro atualizagrafico forca a atualizacao do grafico, caso passe false, apenas criará a aba com o grafico sm dados.
        /// </summary>
        /// <param name="novoGrafico"></param>
        /// <param name="forcaAtualizacaoGrafico"></param>
        private void CriaTabPainel(Grafico novoGrafico, bool forcaAtualizacaoGrafico, bool inicializacao)
        {
            try
            {
                //Criando taba para este gráfico
                TabItem tabItem = new TabItem();
                tabItem.Height = 23;
                tabItem.Background = Brushes.Black;
                tabItem.UseLayoutRounding = true;
                tabControl.Items.Add(tabItem);
                tabItem.InvalidateMeasure();

                //Criando grid interna do tabitem
                Grid grid = new Grid();
                grid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                
                grid.Children.Add(novoGrafico);
                
                //Associando a grid ao tabitem
                tabItem.Content = grid;

                //Criando Header 
                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Horizontal;
                TextBlock txt = new TextBlock();
                txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                txt.Text = ObtemTituloAba(novoGrafico.Ativo, novoGrafico.Periodicidade.Value, novoGrafico.Periodo.Value);
                Button botaoFechar = new Button();
                botaoFechar.Margin = new Thickness(5, 0, 0, 0);
                botaoFechar.Height = 15;
                botaoFechar.Width = 15;
                botaoFechar.Padding = new Thickness(1,0,0,1);
                botaoFechar.Foreground = Brushes.Red;
                botaoFechar.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                botaoFechar.Content = "x";
                botaoFechar.Click += (e1, s1) =>
                    {
                        FechaGrafico(ref novoGrafico);
                    };

                stack.Children.Add(txt);
                stack.Children.Add(botaoFechar);
                tabItem.Header = stack;

                novoGrafico.Tag = txt;


                //assinando eventos do componente grafico
                novoGrafico.OnAlteraPeriodicidade += new Grafico.OnAlteraPeriodicidadeDelegate(Principal_OnAlteraPeriodicidade);
                novoGrafico.OnAlteraPeriodo += new Grafico.OnAlteraPeriodoDelegate(Principal_OnAlteraPeriodo);
                novoGrafico.OnAtualizaAtivo += new Grafico.OnAtualizaAtivoDelegate(Principal_OnAtualizaAtivo);
                novoGrafico.OnAtualizaDados += new Grafico.OnAtualizaDadosDelegate(Principal_OnAtualizaDados);
                novoGrafico.OnVerificaAnaliseCompartilhada += new Grafico.OnVerificaAnaliseCompartilhadaDelegate(novoGrafico_OnVerificaAnaliseCompartilhada);
                novoGrafico.OnHelp += new Grafico.OnHelpDelegate(novoGrafico_OnHelp);
                novoGrafico.OnTrocaTemplate += new Grafico.OnTrocaTemplateDelegate(graficoComponente_OnTrocaTemplate);
                novoGrafico.OnInsereAtivo += new Grafico.OnInsereAtivoDelegate(novoGrafico_OnInsereAtivo);
                novoGrafico.OnAlteraTipoVolume += new Grafico.OnAlteraTipoVolumeDelegate(novoGrafico_OnAlteraTipoVolume);
                
                //Assinando tick para novo gráfico
                //AssinaCotacao(novoGrafico.Ativo);

                //Verifica se é a inicialização
                if (!inicializacao)
                {
                    indexGraficoSelecionado = listaGraficos.Count - 1;
                    tabControl.SelectedIndex = tabControl.Items.Count - 1;
                }


                if (forcaAtualizacaoGrafico)
                {
                    AtualizaGrafico(true);
                    //AssinaCotacao(novoGrafico.Ativo);                    
                }
                
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao criar o tab panel.", "CriaTabPainel", exc);
            }
        }

        void novoGrafico_OnAlteraTipoVolume(Grafico grafico)
        {
            ModuloCarregando(true, "Alterando Volume");
            AtualizaGrafico(false);
        }

        void novoGrafico_OnInsereAtivo(Grafico grafico)
        {
            try
            {
                //Instanciando uma nova tela de novoGraficoDialog passando ativo vazio e periodicidade
                NovoAtivoMesmoGrafico novoAtivo = new NovoAtivoMesmoGrafico(listaAtivosLocal);

                novoAtivo.Closing += (sender1, e1) =>
                {
                    //Carregar dados no objeto do gráfico - termina de carregar no completed do evento
                    if (novoAtivo.DialogResult == true)
                    {
                        //Colocando a tela de carregamento
                        ModuloCarregando(true);

                        if (RetornaBolsaAtivo(novoAtivo.Ativo) == EnumLocal.Bolsa.Bovespa)
                        {
                            baseMarketDataCommon.GetHistoricoBovespaAsync(novoAtivo.Ativo,
                                listaGraficos[indexGraficoSelecionado].Periodicidade.Value, listaGraficos[indexGraficoSelecionado].Periodo.Value,
                                true, false, !ServiceWCF.BovespaRT, DateTime.MinValue,
                                ServiceWCF.Usuario.Guid, novoAtivo);
                        }
                        else
                        {
                            baseMarketDataCommon.GetHistoricoBMFAsync(novoAtivo.Ativo,
                                listaGraficos[indexGraficoSelecionado].Periodicidade.Value,
                                listaGraficos[indexGraficoSelecionado].Periodo.Value,
                                true, false, !ServiceWCF.BMFRT, DateTime.MinValue,
                                ServiceWCF.Usuario.Guid, novoAtivo);
                        }


                    }
                    else
                        ModuloCarregando(false);
                };

                novoAtivo.Show();
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao abrir o dialogo de grafico.", "AbrirDialogNovoGrafico", exc);
            }

        }
               

        void novoGrafico_OnHelp(Grafico grafico)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(ServiceWCF.LinkManual), "");
        }

        #endregion CriaTabPainel()

        #region ObtemTituloAba(+1)
        /// <summary>
        /// Método seta o título da aba com o Ativo e periodicidade
        /// </summary>
        /// <param name="ativo"></param>
        /// <param name="periodicidade"></param>
        /// <returns></returns>
        private string ObtemTituloAba(string ativo, int periodicidade, int periodo)
        {
            string titulo = "";

            // Sabendo o ativo selecionado, o periodo e periodicidade, posso dar nome a tab que criei
            switch (periodicidade)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 10:
                case 15:
                case 30:
                case 60:
                    titulo = ativo.ToUpper() + " (I" + periodicidade.ToString() + " - ";
                    break;
                case 1440:
                    titulo = ativo.ToUpper() + " (D - ";
                    break;
                case 10080:
                    titulo = ativo.ToUpper() + " (S - ";
                    break;
                case 43200:
                    titulo = ativo.ToUpper() + " (M - ";
                    break;
                default:
                    titulo = "";
                    break;
            }

            switch (periodo)
            {
                case 1:
                    titulo += periodo.ToString() + " dia";
                    break;

                case 30:
                    titulo += "1 mês";
                    break;

                case 180:
                    titulo += "6 meses";
                    break;

                case 365:
                    titulo += "1 ano";
                    break;

                case 1095:
                    titulo += "3 anos";
                    break;

                case 1825:
                    titulo += "5 anos";
                    break;

                case 3650:
                    titulo += "10 anos";
                    break;

                default:
                    if (periodo < 30)
                        titulo += periodo.ToString() + " dias";
                    else
                    {
                        if (periodo % 30 == 0)
                            titulo += Convert.ToInt32(periodo / 30).ToString() + " meses";
                        else
                            titulo += Convert.ToInt32(periodo / 30).ToString() + " meses" + "/" + (periodo % 30).ToString() + " dias";
                    }
                    break;
            }

            return titulo + ")";
        }
        #endregion ObtemTituloAba(+1)

        #region WindowsVistaGradiente()
        /// <summary>
        /// Cria gradiente do Windows Vista.
        /// </summary>
        /// <returns></returns>
        private LinearGradientBrush WindowsVistaGradiente(int offset1)
        {
            //Criando BackGround para o stackPanel
            GradientStop gs1 = new GradientStop();
            gs1.Color = Color.FromArgb(155, 76, 76, 76);

            GradientStop gs2 = new GradientStop();
            gs2.Color = Color.FromArgb(155, 51, 53, 56);
            gs2.Offset = offset1;

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

        #region ModuloCarregando()
        /// <summary>
        /// Mostra ou esconde a tela de carregamento.
        /// </summary>
        /// <param name="ativo"></param>
        private void ModuloCarregando(bool ativo)
        {
            ModuloCarregando(ativo, "Carregando dados");
        }
        #endregion ModuloCarregando()

        #region ModuloCarregando(+1)
        /// <summary>
        /// Mostra ou esconde a tela de carregamento.
        /// </summary>
        /// <param name="ativo"></param>
        private void ModuloCarregando(bool ativo, string texto)
        {
            try
            {
                lblCarregando.Text = texto;

                if (ativo)
                    pnlCarregando.Visibility = System.Windows.Visibility.Visible;
                else
                    pnlCarregando.Visibility = System.Windows.Visibility.Collapsed;

                tabControl.IsEnabled = !ativo;
                menu.IsEnabled = !ativo;

                foreach (Grafico gf in listaGraficos)
                {
                    if (gf != null)
                        gf.IsEnabled = !ativo;
                }

                if (listaGraficos.Count == 0)
                    bkcImage.Visibility = System.Windows.Visibility.Visible;
                else
                    bkcImage.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro no carregamento.", "ModuloCarregando", exc);
            }
        }
        #endregion ModuloCarregando(+1)

        #region Tela de Venda

        private void CarregarTelaVenda()
        {
            MessageBox.Show("Você não tem autorização para visualizar esta funcionalidade!");
            ModuloCarregando(false);
        }

        #endregion

        #region FechaGrafico
        /// <summary>
        /// Fecha grafico desejado.
        /// </summary>
        /// <param name="grafico"></param>
        private void FechaGrafico(ref Grafico grafico)
        {
            try
            {
                //grafico.ConfiguracaoGraficoLocal.PainelInfo = false;
                //grafico.Refresh(EnumGeral.TipoRefresh.Layout);

                int indice = listaGraficos.IndexOf(grafico);
                
                listaGraficos.Remove(grafico);

                if (tabControl.Items.Count == 1)
                {
                    try
                    {
                        tabControl.Items.Clear();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    tabControl.Items.RemoveAt(indice);                    
                }

                if (listaGraficos.Count == 0)
                    bkcImage.Visibility = System.Windows.Visibility.Visible;
                else
                    bkcImage.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao fechar o gráfico.", "FechaGrafico", exc);
            }
        }
        #endregion FechaGrafico

        #endregion Metodos Auxiliares

        #region Configuração

        /// <summary>
        /// Metodo que retorna a configuracao padrao para todos os clientes
        /// </summary>
        /// <returns></returns>
        private TerminalWebSVC.ConfiguracaoPadraoDTO RetornaConfiguracaoDefault()
        {
            TerminalWebSVC.ConfiguracaoPadraoDTO configuracaoPadrao = new TerminalWebSVC.ConfiguracaoPadraoDTO();
            TerminalWebSVC.ClienteDTO clienteTemp = new TerminalWebSVC.ClienteDTO();
            TerminalWebSVC.ConfiguracaoGraficoDTO configuracaoGrafico = new TerminalWebSVC.ConfiguracaoGraficoDTO();

            //Criando umn cliente ficticio
            //clienteTemp.Ativo = true;
            clienteTemp.Id = 1;
            clienteTemp.Codigo = "1";
            //clienteTemp.Nome = "DEMO CLIENTE";
            
            //Criando uma configuração ficticia
            configuracaoGrafico.ConfigFiboRetracements = "";
            configuracaoGrafico.CorBordaCandleAlta = "0;0;0;0";
            configuracaoGrafico.CorBordaCandleBaixa = "0;0;0;0";
            configuracaoGrafico.CorCandleAlta = "255;75;243;11";
            configuracaoGrafico.CorCandleBaixa = "255;251;9;3";
            configuracaoGrafico.CorFundo = "255;6;7;11";
            configuracaoGrafico.CorIndicador = "255;17;34;237";
            configuracaoGrafico.CorIndicadorAux1 = "255;121;220;34";
            configuracaoGrafico.CorIndicadorAux2 = "255;253;1;6";
            configuracaoGrafico.CorObjeto = "255;252;254;254";
            configuracaoGrafico.DarvaBox = false;
            configuracaoGrafico.EspacoADireitaDoGrafico = 10;
            configuracaoGrafico.EstiloBarra = 4;
            configuracaoGrafico.EstiloPreco = 0;
            configuracaoGrafico.EstiloPrecoParam1 = 0;
            configuracaoGrafico.EstiloPrecoParam2 = 0;
            configuracaoGrafico.GradeHorizontal = false;
            configuracaoGrafico.GradeVertical = false;
            configuracaoGrafico.GrossuraIndicadorAux1 = 1;
            configuracaoGrafico.GrossuraIndicadorAux2 = 1;
            configuracaoGrafico.GrossuraObjeto = 1;
            configuracaoGrafico.LinhaMagnetica = false;
            configuracaoGrafico.LinhaTendenciaInfinita = false;
            configuracaoGrafico.PainelInfo = true;
            configuracaoGrafico.PosicaoEscala = 1;
            configuracaoGrafico.PrecisaoEscala = 2;
            configuracaoGrafico.TipoEscala = 0;
            configuracaoGrafico.TipoLinhaIndicador = 1;
            configuracaoGrafico.TipoLinhaIndicadorAux1 = 1;
            configuracaoGrafico.TipoLinhaIndicadorAux2 = 1;
            configuracaoGrafico.TipoLinhaObjeto = 1;
            configuracaoGrafico.UsarCoresAltaBaixaVolume = false;

            //Fazendo as associações principais
            configuracaoPadrao.Cliente = clienteTemp;
            configuracaoPadrao.Configuracao = configuracaoGrafico;

            //retornando po objeto defsault
            return configuracaoPadrao;
        }

        #endregion Configuração

        #region Ativos

        /// <summary>
        /// Metodo retorna a bolsa de determinado ativo
        /// </summary>
        /// <param name="ativo"></param>
        /// <returns></returns>
        private EnumLocal.Bolsa RetornaBolsaAtivo(string ativo)
        {
            foreach (AtivoLocalDTO obj in listaAtivosLocal)
            {
                if (obj.Ativo.ToUpper().Trim() == ativo.ToUpper().Trim())
                    return obj.Bolsa;
            }

            //Por default retorna Bovespa
            return EnumLocal.Bolsa.Bovespa;
        }

        #endregion

        #region MostraMensagemErro()
        /// <summary>
        /// Usar este método para mostrar erros.
        /// </summary>
        /// <param name="msgPrincipal"></param>
        /// <param name="metodoEvento"></param>
        /// <param name="exc"></param>
        private void MostraMensagemErro(string msgPrincipal, string metodoEvento, Exception exc)
        {
            if (MessageBox.Show(msgPrincipal + "\r\nClique em OK para visualizar detalhes do erro.", "Atenção", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                MessageBox.Show("Método/Evento: " + metodoEvento + "\r\nErro:" + exc.ToString());
        }
        #endregion MostraMensagemErro()

        #region Auxiliares

        /// <summary>
        /// Converte a cor em hexa para cor solidcolorbrush
        /// </summary>
        /// <param name="hexaColor"></param>
        /// <returns></returns>
        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                )
            );
        }

        /// <summary>
        /// Metodo que retorna um parametro
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string GetParam(string p)
        {
            if (App.Current.Resources[p] != null)
                return App.Current.Resources[p].ToString();
            else
                return string.Empty;
        }

        //function to convert string to byte array 
        public static byte[] ConvertStringToByteArray(string stringToConvert)
        {
            return (new UnicodeEncoding()).GetBytes(stringToConvert);
        }

        #endregion

        #region RT

        private void AssinaCotacao(string symbol)
        {
            //codigo que bloqueava a assinatura do ativo
            foreach (AtivoLocalDTO obj in listaAtivosLocal)
            {
                if (obj.Ativo == symbol)
                {
                    if (obj.Bolsa == EnumLocal.Bolsa.BMF)
                        if (!ServiceWCF.BMFRT)
                            return;
                    if (obj.Bolsa == EnumLocal.Bolsa.Bovespa)
                        if (!ServiceWCF.BovespaRT)
                            return;
                    break;
                }
            }

            client.Subscribe(new SubscribeArgs
            {
                Channel = "/" + symbol,
                OnSuccess = (args) =>
                {
                   
                },
                OnFailure = (args) =>
                {
                    //codigo de falha de assinatura
                },
                OnReceive = (args) =>
                {
                    //codigo de recebimento e processamento de cotações
                    //Disparando evento atraves do asyncOperation (permitindo que não haja a necessidade de usar invoke, por parte do usuario)
                    this.asyncOperation.Post(GenericEventHandler, args.DataJson);                    
                }
            });

        }

        #endregion
        #endregion Metodos

        #region Eventos de Menu

        private void mnuNovoGrafico_Click(object sender, SourcedEventArgs e)
        {
            try
            {
                if (!Util.ServiceWCF.VersaoDEMO)
                {                    
                    //Variavel de retorno de mensagem
                    string mensagemRetorno = "";
                    ModuloCarregando(true, "Carregando gráfico");
                    baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.CarregarNovoGrafico);
                }
                else
                    CarregarTelaVenda();
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao adicionar um gráfico.", "mnuNovoGrafico_Click", exc);
            }
        }

        private void mnuSalvarAT_Click(object sender, SourcedEventArgs e)
        {
            SalvaAT(false, false);
        }

        private void mnuSalvarTemplate_Click(object sender, SourcedEventArgs e)
        {
            
            try
            {
                if (!Util.ServiceWCF.VersaoDEMO)
                {
                    ModuloCarregando(true, "Salvando template");
                    baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.Salvar);
                }
                else
                {
                    CarregarTelaVenda();
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao salvar seu template.", "mnuSalvarTemplate_Click", exc);
            }
        }

        private void mnuAplicarTemplate_Click(object sender, SourcedEventArgs e)
        {
           
            try
            {
                if (!ServiceWCF.VersaoDEMO)
                {
                    ModuloCarregando(true, "Aplicando template");
                    baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.Aplicar);
                }
                else
                    CarregarTelaVenda();
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao aplicar seu template.", "mnuAplicarTemplate_Click", exc);
            }
        }

        private void mnuExcluirTemplate_Click(object sender, SourcedEventArgs e)
        {
            //Variavel de retorno de mensagem
            string mensagemRetorno = "";

            try
            {
                if (!ServiceWCF.VersaoDEMO)
                {
                    ModuloCarregando(true, "Excluindo template");
                    baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.Excluir);
                }
                else
                    CarregarTelaVenda();
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao excluir seu template.", "mnuExcluirTemplate_Click", exc);
            }
        }

        private void mnuAjuda_Click(object sender, SourcedEventArgs e)
        {
            //listaGraficos[0].SetFormatoLinha();

            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(ServiceWCF.LinkManual), "_new");
        }

        private void mnuConfiguracaoPadrao_Click(object sender, SourcedEventArgs e)
        {

            try
            {
                if (!ServiceWCF.VersaoDEMO)
                {
                    //Variavel onde serão controladas possiveis mensagens de retorno
                    string mensagemRetorno = "";

                    ConfiguracaoPadrao dialogConfiguracaoPadrao = new ConfiguracaoPadrao(this.configuracaoPadraDTO);

                    dialogConfiguracaoPadrao.Closing += (sender1, e1) =>
                    {
                        if (dialogConfiguracaoPadrao.DialogResult == true)
                        {
                            ModuloCarregando(true, "Salvando configurações padrões");
                            this.configuracaoPadraDTO = dialogConfiguracaoPadrao.ConfiguracaoPadraoDTO;
                            baseTerminalWebSVC.SalvaConfiguracaoPadraoAsync(this.configuracaoPadraDTO);
                        }
                    };

                    dialogConfiguracaoPadrao.Show();
                }
                else
                    CarregarTelaVenda();
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao iniciar as configurações padrões.", "mnuConfiguracaoPadrao_Click", exc);
            }
        }

        private void mnuSair_Click(object sender, SourcedEventArgs e)
        {
            try
            {
                SalvaAT(true, false);

                System.Windows.Browser.HtmlPage.Window.Invoke("CloseWindow");
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Erro ao finalizar o aplicativo.", "mnuSair_Click", exc);
            }
        }

        private void mnuPublicar_Click(object sender, SourcedEventArgs e)
        {
            if (listaGraficos.Count > 0)
            {
                //if (MessageBox.Show("A área de trabalho precisa ser salva, antes de ser feita a publicação, deseja continuar?", "Aviso", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                //    SalvaAT(false, true);
                    
                //}
                PublicarGrafico();
            }
            else
                MessageBox.Show("Não existe nenhum gráfico a ser publicado.");
        }

        #endregion Eventos de Menu

        #region Atualização de Layout do Gráfico

        private void ThreadAtualizacaoGrafico(Grafico grafico)
        {
            try
            {
                Thread threadAtualizacao = new Thread(new ParameterizedThreadStart(ForcaAtualizacaoLayoutThread));
                threadAtualizacao.IsBackground = true;
                threadAtualizacao.Start(grafico);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao iniciar a atualização de layout.", "ThreadAtualizacaoGrafico", exc);
            }
        }
        
        private void ForcaAtualizacaoLayoutThread(object e)
        {
            this.Dispatcher.BeginInvoke(new DelegateGrafico(ForcaAtualizacaoLayout), (Grafico)e);
        }

        private void ForcaAtualizacaoLayout(Grafico grafico)
        {
            try
            {
                grafico.Refresh(EnumGeral.TipoRefresh.Tudo);
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao atualizar o layout.", "ForcaAtualizacaoLayout", exc);
            }
        }

        #endregion Atualização de Layout do Gráfico

        #region Clique na Logo da Corretora

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.interbolsa.com.br/"), "");
        }

        #endregion Clique na Logo da Corretora

        public void SalvaAT(bool saidaDoAPP, bool publicacao)
        {
            try
            {
                if (listaGraficos.Count == 0)
                {
                    if (!saidaDoAPP)
                        MessageBox.Show("Não há gráficos na área de trabalho.", "Atenção", MessageBoxButton.OK);
                    else
                        ATSalvaSaidaAPP = true;

                    return;
                }

                //Carregando a tela de espera
                ModuloCarregando(true, "Salvando área de trabalho");

                //Limpando a lista de graficos na area de trabalho
                this.areaTrabalhoDTO.Graficos.Clear();

                
                

                //Ler a configuração dos gráficos presentes e suas configurações
                for (int i = 0; i < listaGraficos.Count; i++ )
                {

                    if (!listaGraficos[i].Carregado)
                        this.areaTrabalhoDTO.Graficos.Add(ConvertGraficoComponenteToServer(listaGraficos[i]));
                    else
                    {
                        listaGraficos[i].AtualizaObjetos();
                        listaGraficos[i].AtualizaIndicadores();

                        this.areaTrabalhoDTO.Graficos.Add(ConvertGraficoComponenteToServer(listaGraficos[i]));
                    
                    }
                     
                }

                //Salvar as configurações
                baseTerminalWebSVC.SalvaAreaTrabalhoAsync(this.areaTrabalhoDTO, publicacao);


            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao salvar a área de trabalho.", "mnuNovoGrafico_Click", exc);
            }
        }

        private void imgLogoMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.interbolsa.com.br/"), "");
        }

        private void imgLogoTD_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.traderdata.com.br/"), "");
        }

        private void mnuChat_Click(object sender, SourcedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://suporte.traderdata.com.br/"), "");
        }

        private void mnuemailsuporte_Click(object sender, SourcedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("mailto:suporte@traderdata.com.br"), "");
        }
                
        private Traderdata.Client.Componente.GraficoSL.DTO.TickDTO ConverteFreeStockParaComponente(Traderdata.Client.TerminalWEB.RT.DTO.TickDTO tick)
        {
            Traderdata.Client.Componente.GraficoSL.DTO.TickDTO tickComponente = new Componente.GraficoSL.DTO.TickDTO();
            tickComponente.Abertura = tick.Abertura;
            tickComponente.Ativo = tick.Ativo;
            tickComponente.Bolsa = tick.Bolsa;
            tickComponente.Data = tick.Data;
            tickComponente.FechamentoAnterior = tick.FechamentoAnterior;
            tickComponente.Hora = tick.Hora;
            tickComponente.Maximo = tick.Maximo;
            tickComponente.Media = tick.Media;
            tickComponente.MelhorOfertaCompra = tick.MelhorOfertaCompra;
            tickComponente.MelhorOfertaVenda = tick.MelhorOfertaVenda;
            tickComponente.Minimo = tick.Minimo;
            tickComponente.NumeroNegocio = tick.NumeroNegocio;
            tickComponente.Quantidade = tick.Quantidade;
            tickComponente.QuantidadeMelhorOfertaCompra = tick.QuantidadeMelhorOfertaCompra;
            tickComponente.QuantidadeMelhorOfertaVenda = tick.QuantidadeMelhorOfertaVenda;
            tickComponente.QuantidadeUltimoNegocio = tick.QuantidadeUltimoNegocio;
            tickComponente.Ultimo = tick.Ultimo;
            tickComponente.Variacao = tick.Variacao;
            tickComponente.Volume = tick.Volume;
            tickComponente.VolumeMinuto = tick.VolumeMinuto;
            tickComponente.VolumeIncremento = tick.VolumeIncremento;

            return tickComponente;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        #region Processamento de Tick
        /// <summary>
        /// Parsea o dados de um pacote tick e dispara o evento OnTick.
        /// </summary>
        /// <param name="tickString">Dado a ser parseado.</param>
        internal Traderdata.Client.TerminalWEB.RT.DTO.TickDTO ConverteStringParaTick(string tickString)
        {
            try
            {
                tickString = tickString.Replace("\"", "");
                string[] tick = tickString.Split(':');

                Traderdata.Client.TerminalWEB.RT.DTO.TickDTO tickDTO = new Traderdata.Client.TerminalWEB.RT.DTO.TickDTO();
                if (tick.Length > 2)
                {
                    if (tick[0] == "T")
                    {
                        tickDTO.Ativo = tick[1];
                        tickDTO.Bolsa = Convert.ToInt32(tick[2]);
                        tickDTO.Hora = tick[3];//ConvertFromUnixTimestamp(Convert.ToDouble(tick[3])).ToString("HHmm");
                        tickDTO.Abertura = Convert.ToDouble(tick[4], provider);
                        tickDTO.FechamentoAnterior = Convert.ToDouble(tick[5], provider);
                        tickDTO.Ultimo = Convert.ToDouble(tick[6], provider);
                        tickDTO.Variacao = Convert.ToDouble(tick[7], provider);
                        tickDTO.Maximo = Convert.ToDouble(tick[8], provider);
                        tickDTO.Minimo = Convert.ToDouble(tick[9], provider);
                        tickDTO.Media = Convert.ToDouble(tick[10], provider);
                        tickDTO.NumeroNegocio = Convert.ToInt32(tick[11], provider);
                        tickDTO.QuantidadeUltimoNegocio = Convert.ToDouble(tick[12], provider);
                        tickDTO.Quantidade = Convert.ToDouble(tick[13], provider);
                        tickDTO.MelhorOfertaCompra = Convert.ToDouble(tick[14], provider);
                        tickDTO.QuantidadeMelhorOfertaCompra = Convert.ToDouble(tick[15], provider);
                        tickDTO.MelhorOfertaVenda = Convert.ToDouble(tick[16], provider);
                        tickDTO.QuantidadeMelhorOfertaVenda = Convert.ToDouble(tick[17], provider);
                        //tickDTO.VolumeMinuto = Convert.ToDouble(tick[18], provider);
                        tickDTO.Volume = Convert.ToDouble(tick[18], provider);

                        //Acertando hora se necessario
                        if (tickDTO.Hora.Length == 3)
                            tickDTO.Hora = "0" + tickDTO.Hora;

                        //Demais dados
                        //tickDTO.Volume = Convert.ToDouble(tickDTO.Quantidade) * tickDTO.Media;
                        if (tickDTO.Hora.Length == 6)
                            tickDTO.Data = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, Convert.ToInt32(tickDTO.Hora.Substring(0, 2)), Convert.ToInt32(tickDTO.Hora.Substring(2, 2)), 0);
                        else if (tickDTO.Hora.Length == 4)
                            tickDTO.Data = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, Convert.ToInt32(tickDTO.Hora.Substring(0, 2)), Convert.ToInt32(tickDTO.Hora.Substring(2, 2)), 0);
                        else
                            tickDTO.Data = DateTime.Now;
                    }
                    else if (tick[0] == "I")
                    {
                        tickDTO.Ativo = tick[1];
                        tickDTO.Bolsa = 1;
                        tickDTO.Hora = tick[12];
                        tickDTO.Abertura = Convert.ToDouble(tick[15], provider);
                        tickDTO.FechamentoAnterior = Convert.ToDouble(tick[18], provider);
                        tickDTO.Ultimo = Convert.ToDouble(tick[3], provider);
                        tickDTO.Variacao = Convert.ToDouble(tick[7], provider);
                        tickDTO.Maximo = Convert.ToDouble(tick[4], provider);
                        tickDTO.Minimo = Convert.ToDouble(tick[6], provider);
                        tickDTO.Media = Convert.ToDouble(tick[16], provider);
                        tickDTO.NumeroNegocio = 0;
                        tickDTO.QuantidadeUltimoNegocio = 0;
                        tickDTO.Quantidade = 0;
                        tickDTO.MelhorOfertaCompra = 0;
                        tickDTO.QuantidadeMelhorOfertaCompra = 0;
                        tickDTO.MelhorOfertaVenda = 0;
                        tickDTO.QuantidadeMelhorOfertaVenda = 0;
                        tickDTO.VolumeMinuto = Convert.ToDouble(tick[17], provider);

                        //Acertando hora se necessario
                        if (tickDTO.Hora.Length == 3)
                            tickDTO.Hora = "0" + tickDTO.Hora;

                        //Demais dados
                        tickDTO.Volume = Convert.ToDouble(tick[2]);
                        if (tickDTO.Hora.Length == 4)
                            tickDTO.Data = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, Convert.ToInt32(tickDTO.Hora.Substring(0, 2)), Convert.ToInt32(tickDTO.Hora.Substring(2, 2)), 0);
                        else
                            tickDTO.Data = DateTime.Now;
                    }
                }


                return tickDTO;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void ProcessaTick(object parameters)
        {
            try
            {
                Traderdata.Client.TerminalWEB.RT.DTO.TickDTO tick = new RT.DTO.TickDTO();

                tick = ConverteStringParaTick(Convert.ToString(parameters));
                if (tick.Ativo == listaGraficos[indexGraficoSelecionado].Ativo)
                {
                    tick.Variacao = Math.Round(tick.Variacao,2);// *100;
                    listaGraficos[indexGraficoSelecionado].HabilitaTickBox();
                    dataUltimaDado = DateTime.Now;
                    txtStatus.Text = dataUltimaDado.ToString("HH:mm");
                    listaGraficos[indexGraficoSelecionado].SetUltimaVariacao(tick.Variacao);

                    penultimoVolumeDia = ultimoVolumeDia;
                    ultimoVolumeDia = tick.Volume;

                    if (penultimoVolumeDia > 0)
                        incrementeVolume = ultimoVolumeDia - penultimoVolumeDia;
                    else
                        incrementeVolume = 0;

                    Grafico obj = listaGraficos[indexGraficoSelecionado];
                    if ((tick.Ativo == obj.Ativo) && (tick.Ultimo > 0))
                    //if(false)
                    {

                        //Calcula a variação da ultima barra
                        double ultimo = obj.Dados[obj.Dados.Count - 2].ClosePrice;
                        double variacaoUltimaBarra = ((tick.Ultimo - ultimo) / ultimo) * 100;

                        if (tick.Minimo == 0)
                            tick.Minimo = tick.Ultimo;

                        if (tick.Maximo == 0)
                            tick.Maximo = tick.Ultimo;

                        if (tick.Abertura == 0)
                            tick.Abertura = tick.Ultimo;

                        

                        //Periodicidades diarias
                        switch (obj.Periodicidade.Value)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 5:
                            case 10:
                            case 15:
                            case 30:
                            case 60:
                                tick.VolumeIncremento = incrementeVolume;
                                obj.AtualizaBarrasIntraday(ConverteFreeStockParaComponente(tick), variacaoUltimaBarra);
                                break;

                            case 1440:
                                obj.AtualizaBarraDiaria(tick.Data, tick.Ultimo,
                                    tick.Maximo, tick.Minimo, tick.Abertura, tick.Volume, tick.Variacao, variacaoUltimaBarra);
                                break;
                            case 10080:
                                obj.AtualizaBarraDASEMANA(tick.Data, tick.Ultimo,
                                    tick.Maximo, tick.Minimo, tick.Abertura, tick.Volume + volumeUltimaBarraBanco,
                                    tick.Variacao, variacaoUltimaBarra);
                                break;
                            case 43200:
                                obj.AtualizaBarraDOMES(tick.Data, tick.Ultimo,
                                    tick.Maximo, tick.Minimo, tick.Abertura, tick.Volume + volumeUltimaBarraBanco,
                                    tick.Variacao, variacaoUltimaBarra);
                                break;
                        }
                    }
                }
            }
            catch { }
        }
        #endregion Processamento de Tick

        void timerAssinatura_Tick(object sender, EventArgs e)
        {
            //checando se tem gráfico
            //if (listaGraficos.Count > 0)
            //{
            //    //checando se já encerrou o carregamento
            //    if (pnlCarregando.Visibility == System.Windows.Visibility.Collapsed)
            //    {
            //        if (listaGraficos.Count >= indexGraficoSelecionado)
            //        {
            //            if ( ((ServiceWCF.BovespaRT) && (listaGraficos[indexGraficoSelecionado].Bolsa == (int) EnumLocal.Bolsa.Bovespa)) ||
            //                  ((ServiceWCF.BMFRT) && (listaGraficos[indexGraficoSelecionado].Bolsa == (int)EnumLocal.Bolsa.BMF)) )
            //                if (!realtime.CotacoesAssinadas.Contains(listaGraficos[indexGraficoSelecionado].Ativo))                        
            //                    realtime.AssinaCotacao(listaGraficos[indexGraficoSelecionado].Ativo);
                        

            //            listaGraficos[indexGraficoSelecionado].HabilitaTickBox();
            //        }
            //    }
                                
            //}
        
            
        }

        private void mnuRegistrarInteresse_Click(object sender, SourcedEventArgs e)
        {
            AlertaAnalises alertaAnalises = new AlertaAnalises(listaAtivosLocal);
            alertaAnalises.Show();
        }

        private void mnuCentralAnalise_Click(object sender, SourcedEventArgs e)
        {
            CentralAnalise centralAnalise = new CentralAnalise(listaAtivosLocal);
            centralAnalise.Show();
        }

        private void mnuAlertas_Click(object sender, SourcedEventArgs e)
        {
            CentralAlertas frmAlerta = new CentralAlertas(listaAtivosLocal);
            frmAlerta.Show();
        }

        private void mnuSobre_Click(object sender, SourcedEventArgs e)
        {
            Sobre sobre = new Sobre();
            sobre.Show();
        }

        private void mnuintroducao_Click(object sender, SourcedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.youtube.com/watch?v=UOJpRsPxfa4"), "_new");
        }

        private void mnuindicadores_Click(object sender, SourcedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.youtube.com/watch?v=NjuPIULKgyg"), "_new");
        }

        private void mnuSuporteResistencia_Click(object sender, SourcedEventArgs e)
        {
            List<SuporteResistencia> listaAux = new List<SuporteResistencia>();

            foreach (Grafico obj in listaGraficos)
            {
                SuporteResistencia suporteResistencia = new SuporteResistencia();
                suporteResistencia.Ativo = obj.Ativo;
                suporteResistencia.Resistencias = new List<double>();
                suporteResistencia.Suportes = new List<double>();
                //suporteResistencia.Fechamento = obj.Dados[obj.Dados.Count - 1].ClosePrice;

                foreach (ObjetoEstudoDTO objetos in obj.Objetos)
                {
                    if (objetos.Resistencia)
                    {
                        //nesse caso este objeto é uma resistencia
                        suporteResistencia.Resistencias.Add(objetos.Y1);
                    }

                    suporteResistencia.Resistencias.Sort();
                    
                    if (objetos.Suporte)
                    {
                        //nesse caso este objeto é uma resistencia
                        suporteResistencia.Suportes.Add(objetos.Y1);
                    }

                    suporteResistencia.Suportes.Sort();
                    suporteResistencia.Suportes.Reverse();
                }

                if ( (suporteResistencia.Resistencias.Count > 0) || (suporteResistencia.Suportes.Count>0))
                    listaAux.Add(suporteResistencia);
            }

            SaveFileDialog file = new SaveFileDialog();

            file.DefaultExt = "*.csv";
            file.Filter = "Excel |*.csv";
            if (file.ShowDialog() == false)
                return;

            using (StreamWriter sw = new StreamWriter(file.OpenFile()))
            {
                sw.WriteLine("Ativo;Tendência;Data;Fechamento;sup1;sup2;sup3;res1;res2;res3");
                sw.Flush();

                foreach (SuporteResistencia obj in listaAux)
                {
                    string temp = obj.Ativo;
                    temp += ";;";
                    temp += DateTime.Today.Date;
                    temp += ";" + obj.Fechamento;

                    foreach (double objSuporte in obj.Suportes)
                    {
                        temp += ";" + objSuporte;
                    }

                    switch (obj.Suportes.Count)
                    { 
                        case 0:
                            temp += ";;;";
                            break;
                        case 1:
                            temp += ";;";
                            break;
                        case 2:
                            temp += ";";
                            break;                        
                    }

                    foreach (double objResistencia in obj.Resistencias)
                    {
                        temp += ";" + objResistencia;
                    }

                    switch (obj.Resistencias.Count)
                    {
                        case 0:
                            temp += ";;;";
                            break;
                        case 1:
                            temp += ";;";
                            break;
                        case 2:
                            temp += ";";
                            break;
                    }

                    sw.WriteLine(temp);
                    sw.Flush();
                }
            }
            
        }

        private void btnAplicarTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ServiceWCF.VersaoDEMO)
                {
                    ModuloCarregando(true, "Aplicando template");
                    baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.Aplicar);
                }
                else
                    CarregarTelaVenda();
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao aplicar seu template.", "mnuAplicarTemplate_Click", exc);
            }
        }

        private void btnExcluirTemplate_Click(object sender, RoutedEventArgs e)
        {
            //Variavel de retorno de mensagem
            string mensagemRetorno = "";

            try
            {
                if (!ServiceWCF.VersaoDEMO)
                {
                    ModuloCarregando(true, "Excluindo template");
                    baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.Excluir);
                }
                else
                    CarregarTelaVenda();
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao excluir seu template.", "mnuExcluirTemplate_Click", exc);
            }
        }

        private void btnSalvarTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Util.ServiceWCF.VersaoDEMO)
                {
                    ModuloCarregando(true, "Salvando template");
                    baseTerminalWebSVC.RetornaTemplatesPorClientIdAsync(ServiceWCF.IdUsuario, EnumLocal.AcaoTemplate.Salvar);
                }
                else
                {
                    CarregarTelaVenda();
                }
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao salvar seu template.", "mnuSalvarTemplate_Click", exc);
            }
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            baseTerminalWebSVC.SalvarGraficoAsync(ConvertGraficoComponenteToServer(listaGraficos[indexGraficoSelecionado]), ServiceWCF.IdUsuario);
        }

        void baseFreeStockChartPlus_RetornaGraficoCompleted(object sender, TerminalWebSVC.RetornaGraficoCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                ConfiguracaoGraficoDTO configuracaoNovoGraficoDTO
                        = this.ConvertConfiguracaoServerToComponenteGrafico(e.Result.Configuracao);
                ConfiguracaoPadraoDTO configuracaoPadraoLocalDTO
                    = this.ConvertConfiguracaoPadraoServerToComponente(this.configuracaoPadraDTO);

                Grafico graficoComponente = this.ConvertGraficoServerToComponente(e.Result);
                //new Grafico(configuracaoPadraoLocalDTO, configuracaoNovoGraficoDTO, listaTemplates);

                graficoComponente.ShowHideBotaoAC(ServiceWCF.AnaliseCompartilhada);

                graficoComponente.SetMarcaDagua(ServiceWCF.MarcaDagua, ServiceWCF.MarcaDaguaLeft, ServiceWCF.MarcaDaguaTop,
                    ServiceWCF.MarcaDaguaSize, ServiceWCF.MarcaDaguaWidth);

                if (ServiceWCF.AtivoDireto == "")
                    graficoComponente.HabilitaVersaoCompleta(false, "");
                else
                    if (!ServiceWCF.Simpletrader)
                        graficoComponente.HabilitaVersaoCompleta(true, this.linkVersaoCompleta);
                    else
                        graficoComponente.HabilitaVersaoCompleta(false, "");


                graficoComponente.OnTrocaTemplate += new Grafico.OnTrocaTemplateDelegate(graficoComponente_OnTrocaTemplate);

                if (listaGraficos.Count == 0)
                {
                    //Adicionando na lista de graficos
                    listaGraficos.Add(graficoComponente);
                }



                graficoAvulso.Children.Add(graficoComponente);

                //assinando eventos do componente grafico
                graficoComponente.OnAlteraPeriodicidade += new Grafico.OnAlteraPeriodicidadeDelegate(Principal_OnAlteraPeriodicidade);
                graficoComponente.OnAlteraPeriodo += new Grafico.OnAlteraPeriodoDelegate(Principal_OnAlteraPeriodo);
                graficoComponente.OnAtualizaAtivo += new Grafico.OnAtualizaAtivoDelegate(Principal_OnAtualizaAtivo);
                graficoComponente.OnAtualizaDados += new Grafico.OnAtualizaDadosDelegate(Principal_OnAtualizaDados);
                graficoComponente.OnVerificaAnaliseCompartilhada += new Grafico.OnVerificaAnaliseCompartilhadaDelegate(novoGrafico_OnVerificaAnaliseCompartilhada);
                graficoComponente.OnHelp += new Grafico.OnHelpDelegate(novoGrafico_OnHelp);
                graficoComponente.OnAlteraTipoVolume += new Grafico.OnAlteraTipoVolumeDelegate(novoGrafico_OnAlteraTipoVolume);

                //Assinando tick para novo gráfico
                //AssinaCotacao(graficoComponente.Ativo);

                //plotando
                indexGraficoSelecionado = 0;
                AtualizaGrafico(true);
            }
            else
                AbrirGraficoAvulso();
        }

        void baseFreeStockChartPlus_SalvarGraficoCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ModuloCarregando(false);
            if (e.UserState.ToString() != "AUTO")
                MessageBox.Show("Gráfico Salvo com sucesso!");
        }

        private void btnConfiguracao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ServiceWCF.VersaoDEMO)
                {
                    //Variavel onde serão controladas possiveis mensagens de retorno
                    string mensagemRetorno = "";

                    ConfiguracaoPadrao dialogConfiguracaoPadrao = new ConfiguracaoPadrao(this.configuracaoPadraDTO);

                    dialogConfiguracaoPadrao.Closing += (sender1, e1) =>
                    {
                        if (dialogConfiguracaoPadrao.DialogResult == true)
                        {
                            ModuloCarregando(true, "Salvando configurações padrões");
                            this.configuracaoPadraDTO = dialogConfiguracaoPadrao.ConfiguracaoPadraoDTO;
                            baseTerminalWebSVC.SalvaConfiguracaoPadraoAsync(this.configuracaoPadraDTO);
                        }
                    };

                    dialogConfiguracaoPadrao.Show();
                }
                else
                    CarregarTelaVenda();
            }
            catch (Exception exc)
            {
                MostraMensagemErro("Ocorreu um erro ao iniciar as configurações padrões.", "mnuConfiguracaoPadrao_Click", exc);
            }
        }

        private void mnuProventos_Click(object sender, SourcedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://wiki.traderdata.com.br/analise-fundamentalista/dividendos"), "_new");
        }

        private void mnuGrafico2_Click(object sender, SourcedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(ServiceWCF.BaseAddress + "/tw20/Default.aspx?codcliente=" + ServiceWCF.ID + "&sessid=" + ServiceWCF.SessID), "_new");
        }

        private void mnuTerminalDesktop_Click(object sender, SourcedEventArgs e)
        {
            TerminalDesktop dialog = new TerminalDesktop();

            dialog.Closing += (sender1, e1) =>
            {
                if (dialog.DialogResult == true)
                {
                    
                }
            };

            dialog.Show();
        }

        private void mnuScannerDiario_Click(object sender, SourcedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(ServiceWCF.BaseAddress + "/DefaultScannerAgora.aspx?usr=" + ServiceWCF.ID + "&crc=" + ServiceWCF.AgoraCRC), "_new");
        }

        private void C1MenuItem_Click(object sender, SourcedEventArgs e)
        {

        }
    }

    class SuporteResistencia
    {
        public string Ativo { get; set; }
        public double Fechamento { get; set; }
        public List<double> Suportes { get; set; }
        public List<double> Resistencias { get; set; }
    }
}
