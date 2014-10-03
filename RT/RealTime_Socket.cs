using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using Traderdata.Client.TerminalWEB.Util;

namespace Traderdata.Client.TerminalWEB.RT
{
    /// <summary>
    /// Permite o recebimento de dados em tempo real.
    /// </summary>
    public partial class RealTime
    {
        #region Campos do Socket

        //Variáveis de conexão
        internal static Socket mainSocket;
        internal AsyncCallback m_pfnCallBack;
        internal IAsyncResult m_result;

        #endregion Campos do Socket

        #region Processamento da Conexao Socket

        #region evento disparado após conectar

        /// <summary>
        /// Evento é disparado quando conecta no socket server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnSocketConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
                                
                if (e.SocketError == SocketError.Success)
                {
                    //Enviando comando de login
                    EnviaSolicitacaoAoServidor("L:" + ServiceWCF.userHB + ":" + ServiceWCF.MacroCliente + ":" + ServiceWCF.BovespaRT.ToString() + ":" + ServiceWCF.BMFRT.ToString());

                    //Iniciando a recepção
                    byte[] transferBuffer = new byte[1024];
                    socketArgs.SetBuffer(transferBuffer, 0, transferBuffer.Length);
                    socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSocketReceive);
                    

                    mainSocket.ReceiveAsync(socketArgs);
                }


                
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion

        #region Aguardando Recepcao de Dados pelo Socket

        /// <summary>
        /// Evento dispara evento assincrono que atualiza a tela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSocketReceive(object sender, SocketAsyncEventArgs e)
        {
            StringReader sr = null;
            try
            {
                string data = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                if (data.Length == 0)
                {
                    Desconectar(true);
                }
                else
                {
                    //Retirando caracteres desnecessarios
                    data = data.Replace("\0", "").Replace("\r", "");

                    //Processando dado recebido
                    ProcessaDadoRecebido(data);

                }
               
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                if (sr != null) sr.Close();

                //Prepare to receive more data
                //e.Completed += new EventHandler<SocketAsyncEventArgs>(OnSocketReceive);
                
                mainSocket.ReceiveAsync(e);
            }
            
        }

        #endregion Aguardando Recepcao de Dados pelo Socket
              

        #region Desconectando do Feed
        /// <summary>
        /// Dispara o evento de desconexão do socket.
        /// </summary>
        internal void Desconectar(bool disparaEventoDesconexao)
        {
            try
            {
                //Desconectando
                if (mainSocket != null)
                {
                    mainSocket.Close(5);
                    mainSocket = null;
                }

                //Disparando o evento de desconexao
                if (disparaEventoDesconexao)
                    DisparaEventoDesconexao();
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
            }
        }

        /// <summary>
        /// Dispara o evento de desconexão do socket.
        /// </summary>
        public void Desconectar()
        {
            try
            {
                //Desconectando
                if (mainSocket != null)
                {
                    mainSocket.Close(5);
                    mainSocket = null;
                }

                DisparaEventoDesconexao();
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
            }
        }
        #endregion Desconectando do Feed

        #region Envio de Solicitacao ao Servidor
        /// <summary>
        /// Método padrão para envio de dados ao servidor pelo socket.
        /// </summary>
        /// <param name="dado"></param>
        internal void EnviaSolicitacaoAoServidor(string dado)
        {

            try
            {
                if (mainSocket != null)
                {
                    byte[] bytes = UnicodeEncoding.Unicode.GetBytes(dado + "\r\n");                      

                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.SetBuffer(bytes, 0, bytes.Length);
                    mainSocket.SendAsync(args);
                }
                
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
                Desconectar(true);
            }

        }

        

        #endregion Envio de Solicitacao ao Servidor

        #endregion Processamento da Conexao Socket

        #region Solicitações do Usuário

        #region Assinaturas

        #region Assinatura de Cotacao
        /// <summary>
        /// Assina recebimento de cotação para o ativo desejado.
        /// </summary>
        /// <param name="ativo"></param>
        public void AssinaCotacao(string ativo)
        {
            //Formato: A:T:[ATIVO]:[ASSINATURA(TRUE)/DESASSINATURA(FALSE)]
            ativo = ativo.ToUpper().Trim();

            if (!cotacoesAssinadas.Contains(ativo))
                EnviaSolicitacaoAoServidor("A:T:" + ativo + ":" + "true");
        }
        #endregion Assinatura de Cotacao

        #region Assinatura de Negocio
        /// <summary>
        /// Assina recebimento de negócios para o ativo desejado.
        /// </summary>
        /// <param name="ativo">Ativo desejado.</param>
        public void AssinaNegocios(string ativo)
        {
            //Formato: A:N:[ATIVO]:[ASSINATURA(TRUE)/DESASSINATURA(FALSE)]:[OBTER NEGOCIOS INICIAIS]
            ativo = ativo.ToUpper().Trim();

            if (!negociosAssinados.Contains(ativo))
                EnviaSolicitacaoAoServidor("A:N:" + ativo + ":" + "true" + ":" + "false");
        }
        #endregion Assinatura de Negocio

        #region Assinatura de Book
        /// <summary>
        /// Assina recebimento de book para o ativo desejado.
        /// </summary>
        /// <param name="ativo">Ativo desejado.</param>
        public void AssinaBook(string ativo)
        {
            ativo = ativo.ToUpper();

            if (!booksCompletosAssinados.Contains(ativo))
                EnviaSolicitacaoAoServidor("A:D:" + ativo + ":" + "true");
        }
        #endregion Assinatura de Book

        #region Assinatura de Noticia Bovespa
        /// <summary>
        /// Assina recebimento de book para o ativo desejado.
        /// </summary>
        public void AssinaNoticiaBovespa()
        {
            if (!noticiasBovespaAssinadas)
                EnviaSolicitacaoAoServidor("A:Z:" + "true");
        }
        #endregion Assinatura de Noticia Bovespa

        #region Assinatura de Indice Bovespa
        /// <summary>
        /// Assina recebimento de dados de um índice da Bovespa.
        /// </summary>
        /// <param name="indice">Índice desejado.</param>
        public void AssinaIndiceBovespa(string indice)
        {
            //Formato: A:I:[Indice]:[ASSINATURA(TRUE)/DESASSINATURA(FALSE)]
            indice = indice.ToUpper().Trim();

            if (!indicesBovespaAssinados.Contains(indice))
                EnviaSolicitacaoAoServidor("A:I:" + indice + ":" + "true");
        }
        #endregion Assinatura de Indice Bovespa

        #endregion Assinaturas

        #region Desassinaturas

        #region Desassinatura de Cotacao
        /// <summary>
        /// Desassina recebimento de cotação para o ativo desejado.
        /// </summary>
        /// <param name="ativo"></param>
        public void DesassinaCotacao(string ativo)
        {
            //Formato: A:T:[ATIVO]:[ASSINATURA(TRUE)/DESASSINATURA(FALSE)]
            ativo = ativo.ToUpper().Trim();

            if (cotacoesAssinadas.Contains(ativo))
                EnviaSolicitacaoAoServidor("A:T:" + ativo + ":" + "false");
        }
        #endregion Desassinatura de Cotacao

        #region Desassinatura de Negocio
        /// <summary>
        /// Desassina recebimento de negócios para o ativo desejado.
        /// </summary>
        /// <param name="ativo">Ativo desejado.</param>
        public void DesassinaNegocio(string ativo)
        {
            //Formato: A:N:[ATIVO]:[ASSINATURA(TRUE)/DESASSINATURA(FALSE)]:[OBTER NEGOCIOS INICIAIS]
            ativo = ativo.ToUpper().Trim();

            if (negociosAssinados.Contains(ativo))
                EnviaSolicitacaoAoServidor("A:N:" + ativo + ":" + "false");
        }
        #endregion Desassinatura de Negocio

        #region Desassinatura de Book
        /// <summary>
        /// Desassina recebimento de book para o ativo desejado.
        /// </summary>
        /// <param name="ativo">Ativo desejado.</param>
        public void DesassinaBook(string ativo)
        {
            ativo = ativo.ToUpper();

            if (booksCompletosAssinados.Contains(ativo))
                EnviaSolicitacaoAoServidor("A:D:" + ativo + ":" + "false");
        }
        #endregion Desassinatura de Book

        #region Desassinatura de Noticia Bovespa
        /// <summary>
        /// Desassina recebimento de book para o ativo desejado.
        /// </summary>
        public void DesassinaNoticiaBovespa()
        {
            if (noticiasBovespaAssinadas)
                EnviaSolicitacaoAoServidor("A:Z:" + "false");
        }
        #endregion Desassinatura de Noticia Bovespa

        #region Desassinatura de Indice Bovespa
        /// <summary>
        /// Desassina recebimento de dados de um índice da Bovespa.
        /// </summary>
        /// <param name="indice">Índice desejado.</param>
        public void DesassinaIndiceBovespa(string indice)
        {
            //Formato: A:I:[Indice]:[ASSINATURA(TRUE)/DESASSINATURA(FALSE)]
            indice = indice.ToUpper().Trim();

            if (indicesBovespaAssinados.Contains(indice))
                EnviaSolicitacaoAoServidor("A:I:" + indice + ":" + "false");
        }
        #endregion Desassinatura de Indice Bovespa

        #endregion Desassinatura

        #region IniciaRecepcao

        /// <summary>
        /// Conecta ao servidor de cotações. Em formulários, é desejável que a chamada a este método 
        /// seja feita após o método "InitializeComponent".
        /// </summary>
        /// <param name="login">Login válido para a conexão.</param>
        /// <param name="senha">Senha válida para a conexão.</param>
        /// <param name="partnerId">Id do parceiro.</param>
        public void IniciaRecepcao()
        {
            //Estabelecendo eventos do Telnet Connector
            try
            {
                //Aqui é assinado o evento do socket
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ServiceWCF.RTURLHost), Convert.ToInt32(ServiceWCF.RTURLPort));

                //Criando o socket
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.UserToken = mainSocket;
                args.RemoteEndPoint = ipEnd;
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnSocketConnectCompleted);
                mainSocket.ConnectAsync(args);

                //Assinando evento para evitar a utilizacao de invoke
                GenericEventHandler += new System.Threading.SendOrPostCallback(RealTime_GenericEventHandler); 
            }
            catch (SocketException se)
            {
                DisparaEventoErro(se);
            }
        }

        #endregion IniciaRecepcao

        #endregion Solicitações do Usuário
    }
}
