using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using Traderdata.Client.TerminalWEB.RT.DTO;

namespace Traderdata.Client.TerminalWEB.RT
{
    public partial class RealTime
    {
        /**********************************************************************************************************
         *                                          PENDENCIAS
         *                                          
         * - Criar processo de reconexao (Classes Real Time e Real Time Bruto)
         * - Arrumar o dado que vem no socket do sinal bruto, pois esta vindo dado truncado
         * 
         ***********************************************************************************************************/

        #region Campos

        internal DispatcherTimer timerMonitoracao;

        internal NumberFormatInfo provider = Util.NumberProvider;

        //Variável que permitirá executar um método no contexto da thread principal, evitando a necessidade de invoke pela aplicacao cliente
        internal AsyncOperation asyncOperation;

        //Variavies de assinatura
        internal List<string> booksCompletosAssinados = new List<string>();
        internal List<string> cotacoesAssinadas = new List<string>();
        internal List<string> negociosAssinados = new List<string>();
        internal List<string> indicesBovespaAssinados = new List<string>();
        internal bool noticiasBovespaAssinadas = false;

        internal bool isLogado = false;
        internal string dadoAnterior = "";
        internal DateTime horaSincronizacao = DateTime.Now;

        internal string login = "";
        internal string senha = "";

        #endregion Campos

        #region Delegates da Classe

        /// <summary>
        /// Delegate do evento OnError()
        /// </summary>
        /// <param name="exc">Execeção</param>
        public delegate void OnErrorDelegate(Exception exc);
        /// <summary>
        /// Delegate do evento OnNegocio()
        /// </summary>
        /// <param name="negocio">NegocioDTO</param>
        public delegate void OnNegocioDelegate(NegocioDTO negocio);
        /// <summary>
        /// Delegate do evento OnComandoBookCompleto()
        /// </summary>
        /// <param name="comando">ComandoBookDTO</param>
        public delegate void OnComandoBookCompletoDelegate(ComandoBookDTO comando);
        /// <summary>
        /// Delegate do evento OnHora()
        /// </summary>
        /// <param name="hora">HoraDTO</param>
        public delegate void OnHoraDelegate(HoraDTO hora);
        /// <summary>
        /// Delegate do evento OnNoticia()
        /// </summary>
        public delegate void OnNoticiaDelegate(NoticiaDTO noticia);
        /// <summary>
        /// Delegate do evento OnTick()
        /// </summary>
        /// <param name="e">TickDTO</param>
        public delegate void OnTickDelegate(TickDTO tick);
        /// <summary>
        /// Delegate do evento OnIndiceBovespa()
        /// </summary>
        /// <param name="e">Indice Bovespa</param>
        public delegate void OnIndiceBovespaDelegate(IndiceBovespaDTO indiceBovespa);
        /// <summary>
        /// Delegate do evento OnPacoteTraderData()
        /// </summary>
        /// <param name="pacote">string Pacote</param>
        public delegate void OnPacoteTraderDataDelegate(string pacote);
        /// <summary>
        /// Delegate do evento OnConnect()
        /// </summary>
        public delegate void OnConnectDelegate(ConexaoDTO parametroConexao);
        /// <summary>
        /// Delegate do evento OnDisconnect()
        /// </summary>
        public delegate void OnDisconnectDelegate();
        /// <summary>
        /// Delegate do evento OnAssinatura()
        /// </summary>
        public delegate void OnAssinaturaDelegate(AssinaturaDesassinaturaDTO assinatura);
        /// <summary>
        /// Delegate do evento OnDesassinatura()
        /// </summary>
        public delegate void OnDesassinaturaDelegate(AssinaturaDesassinaturaDTO assinatura);

        #endregion Delegates da Classe

        #region Eventos da Classe

        /// <summary> Evento genérico, que participa do processo de alternativa ao uso de invoke.</summary>
        internal event SendOrPostCallback GenericEventHandler;

        /// <summary>Evento disparado quando ocorre um erro interno.</summary>
        public event OnErrorDelegate OnError;

        /// <summary>Evento disparado no recebimento de um pacote de negócio.</summary>
        public event OnNegocioDelegate OnNegocio;

        /// <summary>Evento disparado na modificação do book de ofertas. Neste evento será retornada o comando de modificação do book.</summary>
        public event OnComandoBookCompletoDelegate OnComandoBookCompleto;

        /// <summary>Evento disparado quando uma notícia é recebida.</summary>
        public event OnNoticiaDelegate OnNoticia;

        /// <summary>Evento disparado no recebimento de um pacote hora.</summary>
        public event OnHoraDelegate OnHora;

        /// <summary>Evento disparado no recebimento de um pacote tick.</summary>
        public event OnTickDelegate OnTick;

        /// <summary>Evento disparado no recebimento de um pacote de índice da bovespa.</summary>
        public event OnIndiceBovespaDelegate OnIndiceBovespa;

        /// <summary>Evento disparado no recebimento de um pacote TraderData.</summary>
        public event OnPacoteTraderDataDelegate OnPacoteTraderData;

        /// <summary>Evento disparado quando a conexão é realizada  com sucesso.</summary>
        public event OnConnectDelegate OnConnect;

        /// <summary>Evento disparado quando a conexão é desfeita.</summary>
        public event OnDisconnectDelegate OnDisconnect;

        /// <summary>Evento disparado quando uma resposta a uma solicitação de assinatura é recebida.</summary>
        public event OnAssinaturaDelegate OnAssinatura;

        /// <summary>Evento disparado quando uma resposta a uma solicitação de desassinatura é recebida.</summary>
        public event OnDesassinaturaDelegate OnDesassinatura;

        #endregion Eventos da Classe

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public RealTime()
        {
            timerMonitoracao = new DispatcherTimer();
            timerMonitoracao.Interval = new TimeSpan(0,0,30);
            timerMonitoracao.Tick += new EventHandler(timerMonitoracao_Tick);

            //Assinando o evento de disparo de dados
            GenericEventHandler = new SendOrPostCallback(RealTime_GenericEventHandler);

            // Cria uma instância de uma AsyncOperation para gerenciar o contexto
            this.asyncOperation = AsyncOperationManager.CreateOperation(null);
        }


        #endregion Construtores

        #region Destrutor

        ~RealTime()
        {
            try
            {
                Desconectar(false);
            }
            catch
            {
            }
        }
        #endregion Destrutor

        #region Propriedades

        #region IsLogado
        /// <summary>
        /// Informa se está logado.
        /// </summary>
        public bool IsLogado
        {
            get { return isLogado; }
            set { isLogado = value; }
        }
        #endregion IsLogado

        #region CotacoesAssinadas
        /// <summary>
        /// Lista de ativos assinados para o recebimento de cotações.
        /// </summary>
        public List<string> CotacoesAssinadas
        {
            get { return cotacoesAssinadas; }
        }
        #endregion CotacoesAssinadas

        #region BooksAssinados
        /// <summary>
        /// Lista de ativos assinados para o recebimento de book.
        /// </summary>
        public List<string> BooksAssinados
        {
            get { return booksCompletosAssinados; }
        }
        #endregion BooksAssinados

        #region NegocioAssinados
        /// <summary>
        /// Lista de ativos assinados para o recebimento de negócio.
        /// </summary>
        public List<string> NegocioAssinados
        {
            get { return negociosAssinados; }
        }
        #endregion NegocioAssinados

        #region IndicesBovespaAssinados
        /// <summary>
        /// Índices da Bovespa assinados.
        /// </summary>
        public List<string> IndicesBovespaAssinados
        {
            get { return indicesBovespaAssinados; }
        }
        #endregion IndicesBovespaAssinados

        #region Login
        /// <summary>
        /// Login
        /// </summary>
        public string Login
        {
            get { return login; }
        }
        #endregion Login

        #region Senha
        /// <summary>
        /// Senha
        /// </summary>
        public string Senha
        {
            get { return senha; }
        }
        #endregion Senha

        #region HoraServidor
        /// <summary>
        /// Indica a hora do servidor.
        /// </summary>
        public DateTime HoraServidor
        {
            get { return horaSincronizacao; }
        }
        #endregion HoraServidor

        #endregion Propriedades

        #region Timer de Monitoramento
        /// <summary>
        /// Timer responsavel pelo envio de pacotes de sincronizacao de hora e monitoracao da conexão.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void timerMonitoracao_Tick(object sender, EventArgs e)
        {
            if (mainSocket.Connected)
            {
                EnviaSolicitacaoAoServidor("F:");
            }
        }
               

        #endregion Timer de Monitoramento

        #region Métodos de Disparo de Eventos

        #region Disparador de Eventos de Dados usando AsyncOperation
        /// <summary>
        /// Método responsável pelo disparo de eventos de dados. Deve receber uma lista com o enumerado de tipo de dado e 
        /// o handle do dado, nesta ordem!
        /// </summary>
        /// <param name="parametros">Qualquer valor, pois o método utiliza a variável "parametros".</param>
        internal void RealTime_GenericEventHandler(object parameters)
        {
            try
            {
                List<object> param = (List<object>)parameters;

                if (param.Count > 0)
                {
                    //Obtendo parâmetros
                    EnumLocalRT.TipoDado tipo = (EnumLocalRT.TipoDado)param[0];

                    //Disparando dado de acordo com o tipo
                    switch (tipo)
                    {
                        case EnumLocalRT.TipoDado.IndiceBovespa:
                            if (OnIndiceBovespa != null)
                                OnIndiceBovespa((IndiceBovespaDTO)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.Assinatura:
                            if (OnAssinatura != null)
                                OnAssinatura((AssinaturaDesassinaturaDTO)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.Desassinatura:
                            if (OnDesassinatura != null)
                                OnDesassinatura((AssinaturaDesassinaturaDTO)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.Tick:
                            if (OnTick != null)
                                OnTick((TickDTO)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.ComandoBookCompleto:
                            if (OnComandoBookCompleto != null)
                                OnComandoBookCompleto((ComandoBookDTO)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.Negocio:
                            if (OnNegocio != null)
                                OnNegocio((NegocioDTO)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.Hora:
                            if (OnHora != null)
                                OnHora((HoraDTO)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.Conexao:
                            if (OnConnect != null)
                                OnConnect((ConexaoDTO)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.Desconexao:
                            if (OnDisconnect != null)
                                OnDisconnect();
                            break;

                        case EnumLocalRT.TipoDado.Erro:
                            if (OnError != null)
                                OnError((Exception)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.PacoteTraderData:
                            if (OnPacoteTraderData != null)
                                OnPacoteTraderData((string)param[1]);
                            break;

                        case EnumLocalRT.TipoDado.Noticia:
                            if (OnNoticia != null)
                                OnNoticia((NoticiaDTO)param[1]);
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
            }
        }
        #endregion Disparador de Eventos de Dados usando AsyncOperation

        #region DisparaEvento()
        /// <summary>
        /// Método responsável por disparar um evento utilizando AsyncOperation, que retira a necessidade de usar invoke por parte do usuario.
        /// </summary>
        /// <param name="tipoEvento">Tipo do evento que será disparado.</param>
        /// <param name="parametros">Parâmetros do evento.</param>
        internal void DisparaEvento(EnumLocalRT.TipoDado tipoEvento, params object[] parametros)
        {
            List<object> parametrosAux = new List<object>();
            parametrosAux.Add(tipoEvento);

            foreach (object obj in parametros)
            {
                parametrosAux.Add(obj);
            }

            #region Verificando se o evento está assinando por alguem

            switch (tipoEvento)
            {
                case EnumLocalRT.TipoDado.IndiceBovespa:
                    if (OnIndiceBovespa == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.Assinatura:
                    if (OnAssinatura == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.Desassinatura:
                    if (OnDesassinatura == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.ComandoBookCompleto:
                    if (OnComandoBookCompleto == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.Conexao:
                    if (OnConnect == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.Desconexao:
                    if (OnDisconnect == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.Erro:
                    if (OnError == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.Hora:
                    if (OnHora == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.Negocio:
                    if (OnNegocio == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.PacoteTraderData:
                    if (OnPacoteTraderData == null)
                        return;
                    break;

                case EnumLocalRT.TipoDado.Tick:
                    if (OnTick == null)
                        return;
                    break;

                default:
                    return;
            }

            #endregion Verificando se o evento está assinando por alguem

            //Disparando evento atraves do asyncOperation (permitindo que não haja a necessidade de usar invoke, por parte do usuario)
            this.asyncOperation.Post(GenericEventHandler, parametrosAux);
        }
        #endregion DisparaEvento()

        #region DisparaEventoAssinatura()
        /// <summary>
        /// Dispara o evento de assinatura.
        /// </summary>
        /// <param name="parametro">Parâmetro de assinatura.</param>
        internal void DisparaEventoAssinatura(AssinaturaDesassinaturaDTO parametro)
        {
            DisparaEvento(EnumLocalRT.TipoDado.Assinatura, parametro);
        }
        #endregion DisparaEventoAssinatura()

        #region DisparaEventoDesassinatura()
        /// <summary>
        /// Dispara o evento de Desassinatura.
        /// </summary>
        /// <param name="parametro">Parâmetro de desassinatura.</param>
        internal void DisparaEventoDesassinatura(AssinaturaDesassinaturaDTO parametro)
        {
            DisparaEvento(EnumLocalRT.TipoDado.Desassinatura, parametro);
        }
        #endregion DisparaEventoDesassinatura()

        #region DisparaEventoIndiceBovespa()
        /// <summary>
        /// Dispara o evento de indice bovespa.
        /// </summary>
        /// <param name="parametro">Parâmetro de indice.</param>
        internal void DisparaEventoIndiceBovespa(IndiceBovespaDTO parametro)
        {
            DisparaEvento(EnumLocalRT.TipoDado.IndiceBovespa, parametro);
        }
        #endregion DisparaEventoIndiceBovespa()

        #region DisparaEventoComandoBookCompleto()
        /// <summary>
        /// Dispara o evento de comando de book completo.
        /// </summary>
        /// <param name="parametro">Parâmetro do book.</param>
        internal void DisparaEventoComandoBookCompleto(ComandoBookDTO parametro)
        {
            DisparaEvento(EnumLocalRT.TipoDado.ComandoBookCompleto, parametro);
        }
        #endregion DisparaEventoComandoBookCompleto()

        #region DisparaEventoConexao()
        /// <summary>
        /// Dispara o evento de conexão.
        /// </summary>
        internal void DisparaEventoConexao(ConexaoDTO parametroConexao)
        {
            DisparaEvento(EnumLocalRT.TipoDado.Conexao, parametroConexao);
        }
        #endregion DisparaEventoConexao()

        #region DisparaEventoDesconexao()
        /// <summary>
        /// Dispara o evento de desconexão.
        /// </summary>
        internal void DisparaEventoDesconexao()
        {
            DisparaEvento(EnumLocalRT.TipoDado.Desconexao);
        }
        #endregion DisparaEventoDesconexao()

        #region DisparaEventoErro()
        /// <summary>
        /// Dispara o evento de erro.
        /// </summary>
        /// <param name="parametro">Parâmetro do erro (exception).</param>
        internal void DisparaEventoErro(Exception parametro)
        {            
            DisparaEvento(EnumLocalRT.TipoDado.Erro, parametro);
        }
        #endregion DisparaEventoErro()

        #region DisparaEventoHora()
        /// <summary>
        /// Dispara o evento de hora.
        /// </summary>
        /// <param name="parametro">Parâmetro da hora.</param>
        internal void DisparaEventoHora(HoraDTO parametro)
        {
            DisparaEvento(EnumLocalRT.TipoDado.Hora, parametro);
        }
        #endregion DisparaEventoHora()

        #region DisparaEventoNegocio()
        /// <summary>
        /// Dispara o evento de negocio.
        /// </summary>
        /// <param name="parametro">Parâmetro do negocio.</param>
        internal void DisparaEventoNegocio(NegocioDTO parametro)
        {
            DisparaEvento(EnumLocalRT.TipoDado.Negocio, parametro);
        }
        #endregion DisparaEventoNegocio()

        #region DisparaEventoPacoteTraderData()
        /// <summary>
        /// Dispara o evento do pacote TraderData.
        /// </summary>
        /// <param name="parametro">Parâmetro do pacote TraderData.</param>
        internal void DisparaEventoPacoteTraderData(string parametro)
        {
            DisparaEvento(EnumLocalRT.TipoDado.PacoteTraderData, parametro);
        }
        #endregion DisparaEventoPacoteTraderData()

        #region DisparaEventoTick()
        /// <summary>
        /// Dispara o evento de tick.
        /// </summary>
        /// <param name="parametro">Parâmetro de tick.</param>
        internal void DisparaEventoTick(TickDTO parametro)
        {
            //OnTick(parametro);
            DisparaEvento(EnumLocalRT.TipoDado.Tick, parametro);
        }
        #endregion DisparaEventoTick()

        #region DisparaEventoNoticia()
        /// <summary>
        /// Dispara o evento de noticia.
        /// </summary>
        /// <param name="parametro">Parâmetro de noticia.</param>
        internal void DisparaEventoNoticia(NoticiaDTO parametro)
        {
            DisparaEvento(EnumLocalRT.TipoDado.Noticia, parametro);
        }
        #endregion DisparaEventoNoticia()

        #endregion Métodos de Disparo de Eventos
    }

    #region Classes Auxiliares

    internal class SocketPacket
    {
        public System.Net.Sockets.Socket thisSocket;
        public byte[] dataBuffer = new byte[1024];
    }

    #endregion Classes Auxiliares
}
