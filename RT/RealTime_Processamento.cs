using System.IO;
using System;
using System.Collections.Generic;
using Traderdata.Client.TerminalWEB.RT.DTO;

namespace Traderdata.Client.TerminalWEB.RT
{
    /// <summary>
    /// Parte da classe Real Time reponsavel pelo processamento do dado tratado recebido.
    /// </summary>
    public partial class RealTime
    {
        
        #region Redirecionamento de Pacote Recebido
        /// <summary>
        /// Redireciona o pacote recebido para o processador correspondente.
        /// </summary>
        /// <param name="dado">Dado recebido pelo servidor.</param>
        internal bool ProcessaDadoRecebido(string dado)
        {
            try
            {
                bool processamentoRealizado = false;

                //Trabalhando a data
                dado = dadoAnterior + dado;

                //Agora devo fazer um split dos comandos recebidos
                string[] arrayComandos = dado.Split('\n');
                                

                //Todo comando de usuário deve terminar em "\n", portanto, o ultimo item do array de comandos deve ser vazio
                //Para isso temmos os seguintes casos:

                //Caso 1) Se o array de comandos possui apenas um item, significa que este comando não está terminado, portanto devo armazená-lo
                if (arrayComandos.Length == 1)
                {
                    dadoAnterior = arrayComandos[0];
                    return true;
                }

                //Caso 2) Se o array de comando possui mais de um item e o ultimo item não é vazio, devo buferiza-lo
                else if ((arrayComandos.Length > 1) && (arrayComandos[arrayComandos.Length - 1] != ""))
                {
                    dadoAnterior = arrayComandos[arrayComandos.Length - 1];
                    arrayComandos[arrayComandos.Length - 1] = "";
                }

                //Caso 3) Se o array possui mais de um item ou não possuir nenhum, e nenhum dos casos acima for atendido, devo esvaziar o buffer
                else
                    dadoAnterior = "";

                for (int i = 0; i < arrayComandos.Length; i++)
                {
                    if (arrayComandos[i].Length >= 2)
                    {
                        //Limpa a linha retirando caracteres desnecessarios
                        string comando = arrayComandos[i];

                        //Verifica o tipo de pacote para poder parsear e dispaar o evento
                        if (comando.Length > 2)
                        {
                            switch (comando.Substring(0, 2))
                            {
                                //Confirmacao Assinatura
                                case "M:":
                                    processamentoRealizado = ProcessaRespostaAssinatura(comando);
                                    break;

                                //Confirmacao de Desassinatura
                                case "S:":
                                    processamentoRealizado = ProcessaRespostaDesassinatura(comando);
                                    break;

                                //Conexao
                                case "C:":
                                    processamentoRealizado = ProcessaConexao(comando);
                                    break;

                                //Book
                                case "D:":
                                    processamentoRealizado = ProcessaBookCompleto(comando);
                                    break;

                                //Hora
                                case "H:":
                                    processamentoRealizado = ProcessaHora(comando);
                                    break;

                                //Negocio
                                case "N:":
                                    processamentoRealizado = ProcessaNegocio(comando);
                                    break;

                                //Tick
                                case "T:":
                                    processamentoRealizado = ProcessaTick(comando);
                                    break;

                                //Indice Bovespa
                                case "I:":
                                    processamentoRealizado = ProcessaIndiceBovespa(comando);
                                    break;

                                //Noticia Bovespa
                                case "Z:":
                                    ProcessaNoticiaBovespa(comando);
                                    break;

                                //Noticia Bovespa
                                case "U:":
                                    ProcessaNoticiaBMF(comando);
                                    break;

                                //Pacote de monitoramento
                                case "F:":
                                    processamentoRealizado = ProcessaComandoMonitoramento(comando);
                                    break;

                                //Desconexao
                                case "X:":
                                    Desconectar(true);
                                    break;

                                //Outros
                                default:
                                    //Pacote de sincronizacao e atividade do servidor
                                    if (comando.Contains("QOS:"))
                                      horaSincronizacao = Convert.ToDateTime(comando.Replace("QOS:", ""));
                                    break;
                            }

                            //Disparando evento de pacote traderdata
                            DisparaEventoPacoteTraderData(comando);
                        }
                    }
                }

                return processamentoRealizado;
            }
            catch (Exception exc)
            {                
                DisparaEventoErro(exc);
                return false;
            }
        }
        #endregion Redirecionamento de Pacote Recebido

        #region Processamento de Conexao
        /// <summary>
        /// Processa resposta da tentativa de conexao.
        /// </summary>
        /// <param name="dado"></param>
        /// <returns></returns>
        internal bool ProcessaConexao(string dado)
        {
            try
            {
                string[] dados = dado.Replace("C:", "").Split(':');

                ConexaoDTO conexao = new ConexaoDTO();
                conexao.ConexaoBemSucedida = Convert.ToBoolean(dados[0]);
                conexao.Login = login;
                conexao.Senha = senha;

                //Se a conexao foi bem sucedida, devo atualizar a hora com o valor correspondente
                if (conexao.ConexaoBemSucedida)
                {
                    conexao.HoraConexao = new DateTime(Convert.ToInt32(dados[1].Substring(4, 4)), Convert.ToInt32(dados[1].Substring(2, 2)),
                                                       Convert.ToInt32(dados[1].Substring(0, 2)), Convert.ToInt32(dados[1].Substring(8, 2)),
                                                       Convert.ToInt32(dados[1].Substring(10, 2)), Convert.ToInt32(dados[1].Substring(12, 2)));

                }
                else
                    conexao.Mensagem = dados[1];

                //Disparando evento de conexao
                DisparaEventoConexao(conexao);

                return true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion Processamento de Conexao

        #region Processamento da Resposta de Assinatura
        /// <summary>
        /// Processa resposta da assinatura.
        /// </summary>
        internal bool ProcessaRespostaAssinatura(string dado)
        {
            try
            {
                string[] dados = dado.Replace("M:", "").Split(':');
                string tipo = dados[0];
                string ativo = dados[1];
                bool sucesso = Convert.ToBoolean(dados[2]);

                AssinaturaDesassinaturaDTO assinaturaDTO = new AssinaturaDesassinaturaDTO();
                assinaturaDTO.Tipo = AssinaturaDesassinaturaDTO.TipoAssinaturaEnum.Assinatura;
                assinaturaDTO.Ativo = ativo;
                assinaturaDTO.OperacaoBemSucedida = sucesso;

                //Verificando o tipo de dado da assinatura
                switch (tipo)
                {
                    case "T":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.Tick;

                        if ((sucesso) && (!cotacoesAssinadas.Contains(ativo)))
                            cotacoesAssinadas.Add(ativo);
                        break;

                    case "N":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.Negocio;

                        if ((sucesso) && (!negociosAssinados.Contains(ativo)))
                            negociosAssinados.Add(ativo);
                        break;

                    case "D":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.BookCompleto;

                        if ((sucesso) && (!booksCompletosAssinados.Contains(ativo)))
                            booksCompletosAssinados.Add(ativo);
                        break;

                    case "I":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.IndiceBovespa;

                        if ((sucesso) && (!indicesBovespaAssinados.Contains(ativo)))
                            indicesBovespaAssinados.Add(ativo);
                        break;

                    case "Z":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.Noticia;

                        if ((sucesso) && (!noticiasBovespaAssinadas))
                            noticiasBovespaAssinadas = true;
                        break;
                }

                //Obtendo msg de erro se a assinatura não ocorreu como esperado
                if (!sucesso)
                    assinaturaDTO.Mensagem = dados[3];

                //Disparando evento de assinatura
                DisparaEventoAssinatura(assinaturaDTO);

                return true;
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
                return false;
            }
        }
        #endregion Processamento da Resposta de Assinatura

        #region Processamento da Resposta de Desassinatura
        /// <summary>
        /// Processa resposta da desassinatura.
        /// </summary>
        internal bool ProcessaRespostaDesassinatura(string dado)
        {
            try
            {
                string[] dados = dado.Replace("S:", "").Split(':');
                string tipo = dados[0];
                string ativo = dados[1];
                bool sucesso = Convert.ToBoolean(dados[2]);

                AssinaturaDesassinaturaDTO assinaturaDTO = new AssinaturaDesassinaturaDTO();
                assinaturaDTO.Tipo = AssinaturaDesassinaturaDTO.TipoAssinaturaEnum.Desassinatura;
                assinaturaDTO.Ativo = ativo;
                assinaturaDTO.OperacaoBemSucedida = sucesso;

                //Verificando o tipo de dado da assinatura
                switch (tipo)
                {
                    case "T":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.Tick;

                        if ((sucesso) && (cotacoesAssinadas.Contains(ativo)))
                            cotacoesAssinadas.Remove(ativo);
                        break;

                    case "N":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.Negocio;

                        if ((sucesso) && (negociosAssinados.Contains(ativo)))
                            negociosAssinados.Remove(ativo);
                        break;

                    case "D":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.BookCompleto;

                        if ((sucesso) && (booksCompletosAssinados.Contains(ativo)))
                            booksCompletosAssinados.Remove(ativo);
                        break;

                    case "I":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.IndiceBovespa;

                        if ((sucesso) && (indicesBovespaAssinados.Contains(ativo)))
                            indicesBovespaAssinados.Remove(ativo);
                        break;

                    case "Z":
                        assinaturaDTO.TipoDado = AssinaturaDesassinaturaDTO.TipoDadoAssinaturaEnum.Noticia;

                        if ((sucesso) && (noticiasBovespaAssinadas))
                            noticiasBovespaAssinadas = false;
                        break;
                }

                //Obtendo msg de erro se a assinatura não ocorreu como esperado
                if (!sucesso)
                    assinaturaDTO.Mensagem = dados[3];

                //Disparando evento de assinatura
                DisparaEventoDesassinatura(assinaturaDTO);

                return true;
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
                return false;
            }
        }
        #endregion Processamento da Resposta de Desassinatura

        #region Processamento de Tick
        /// <summary>
        /// Parsea o dados de um pacote tick e dispara o evento OnTick.
        /// </summary>
        /// <param name="tickString">Dado a ser parseado.</param>
        internal bool ProcessaTick(string tickString)
        {
            try
            {
                string[] tick = tickString.Split(':');

                //Só devo processar ativos assinados
                if (cotacoesAssinadas.Contains(tick[1]))
                {
                    TickDTO tickDTO = new TickDTO();
                    TickDTO tickDTOAntigo = null;
                    tickDTO.Ativo = tick[1];

                    //Resgatando o tickDTO presente no hub para poder identificar o que exatamente mudou e alterar os timespans
                    foreach (TickDTO obj in DataHub.listaTicks)
                    {
                        if (obj.Ativo == tickDTO.Ativo)
                        {
                            tickDTOAntigo = obj;

                            tickDTO.TimeStampAlteracaoAbertura = obj.TimeStampAlteracaoAbertura;
                            tickDTO.TimeStampAlteracaoFechamentoAnterior = obj.TimeStampAlteracaoFechamentoAnterior;
                            tickDTO.TimeStampAlteracaoMaximo = obj.TimeStampAlteracaoMaximo;
                            tickDTO.TimeStampAlteracaoMedia = obj.TimeStampAlteracaoMedia;
                            tickDTO.TimeStampAlteracaoMelhorOfertaCompra = obj.TimeStampAlteracaoMelhorOfertaCompra;
                            tickDTO.TimeStampAlteracaoMelhorOfertaVenda = obj.TimeStampAlteracaoMelhorOfertaVenda;
                            tickDTO.TimeStampAlteracaoMinimo = obj.TimeStampAlteracaoMinimo;
                            tickDTO.TimeStampAlteracaoNumeroNegocio = obj.TimeStampAlteracaoNumeroNegocio;
                            tickDTO.TimeStampAlteracaoQuantidade = obj.TimeStampAlteracaoQuantidade;
                            tickDTO.TimeStampAlteracaoQuantidadeMelhorOfertaCompra = obj.TimeStampAlteracaoQuantidadeMelhorOfertaCompra;
                            tickDTO.TimeStampAlteracaoQuantidadeMelhorOfertaVenda = obj.TimeStampAlteracaoQuantidadeMelhorOfertaVenda;
                            tickDTO.TimeStampAlteracaoQuantidadeUltimoNegocio = obj.TimeStampAlteracaoQuantidadeUltimoNegocio;
                            tickDTO.TimeStampAlteracaoUltimo = obj.TimeStampAlteracaoUltimo;
                            tickDTO.TimeStampAlteracaoVariacao = obj.TimeStampAlteracaoVariacao;
                            tickDTO.TimeStampAlteracaoVolume = obj.TimeStampAlteracaoVolume;
                            tickDTO.TimeStampAlteracaoVolumeMinuto = obj.TimeStampAlteracaoVolumeMinuto;

                            break;
                        }
                    }
                                        
                    tickDTO.Bolsa = Convert.ToInt32(tick[2]);
                    tickDTO.Hora = tick[3];
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
                    tickDTO.VolumeMinuto = Convert.ToDouble(tick[18], provider);

                    //Acertando hora se necessario
                    if (tickDTO.Hora.Length == 3)
                        tickDTO.Hora = "0" + tickDTO.Hora;

                    //Demais dados
                    tickDTO.Volume = Convert.ToDouble(tickDTO.Quantidade) * tickDTO.Media;
                    if (tickDTO.Hora.Length == 4)
                        tickDTO.Data = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, Convert.ToInt32(tickDTO.Hora.Substring(0, 2)), Convert.ToInt32(tickDTO.Hora.Substring(2, 2)), 0);
                    else
                        tickDTO.Data = DateTime.Now;


                    //Verificando se o ativo existia na lista de ticks
                    if (tickDTOAntigo != null)
                    {
                        //Neste caso devemos setar as datas de acordo com o que fora alterado
                        if (tickDTOAntigo.Abertura != tickDTO.Abertura)
                            tickDTO.TimeStampAlteracaoAbertura = DateTime.Now;

                        if (tickDTOAntigo.FechamentoAnterior != tickDTO.FechamentoAnterior)
                            tickDTO.TimeStampAlteracaoFechamentoAnterior = DateTime.Now;

                        if (tickDTOAntigo.Maximo != tickDTO.Maximo)
                            tickDTO.TimeStampAlteracaoMaximo = DateTime.Now;

                        if (tickDTOAntigo.Media != tickDTO.Media)
                            tickDTO.TimeStampAlteracaoMedia = DateTime.Now;

                        if (tickDTOAntigo.MelhorOfertaCompra != tickDTO.MelhorOfertaCompra)
                            tickDTO.TimeStampAlteracaoMelhorOfertaCompra = DateTime.Now;

                        if (tickDTOAntigo.MelhorOfertaVenda != tickDTO.MelhorOfertaVenda)
                            tickDTO.TimeStampAlteracaoMelhorOfertaVenda = DateTime.Now;

                        if (tickDTOAntigo.Minimo != tickDTO.Minimo)
                            tickDTO.TimeStampAlteracaoMinimo = DateTime.Now;

                        if (tickDTOAntigo.NumeroNegocio != tickDTO.NumeroNegocio)
                            tickDTO.TimeStampAlteracaoNumeroNegocio = DateTime.Now;

                        if (tickDTOAntigo.Quantidade != tickDTO.Quantidade)
                            tickDTO.TimeStampAlteracaoQuantidade = DateTime.Now;

                        if (tickDTOAntigo.QuantidadeMelhorOfertaCompra != tickDTO.QuantidadeMelhorOfertaCompra)
                            tickDTO.TimeStampAlteracaoQuantidadeMelhorOfertaCompra = DateTime.Now;

                        if (tickDTOAntigo.QuantidadeMelhorOfertaVenda != tickDTO.QuantidadeMelhorOfertaVenda)
                            tickDTO.TimeStampAlteracaoQuantidadeMelhorOfertaVenda = DateTime.Now;

                        if (tickDTOAntigo.QuantidadeUltimoNegocio != tickDTO.QuantidadeUltimoNegocio)
                            tickDTO.TimeStampAlteracaoQuantidadeUltimoNegocio = DateTime.Now;

                        if (tickDTOAntigo.Ultimo != tickDTO.Ultimo)
                            tickDTO.TimeStampAlteracaoUltimo = DateTime.Now;

                        if (tickDTOAntigo.Variacao != tickDTO.Variacao)
                            tickDTO.TimeStampAlteracaoVariacao = DateTime.Now;

                        if (tickDTOAntigo.Volume != tickDTO.Volume)
                            tickDTO.TimeStampAlteracaoVolume = DateTime.Now;

                        if (tickDTOAntigo.VolumeMinuto != tickDTO.VolumeMinuto)
                            tickDTO.TimeStampAlteracaoVolumeMinuto = DateTime.Now;

                        //Alterando o tick na listagem
                        for (int i = 0; i <= DataHub.listaTicks.Count - 1; i++)
                        {
                            if (DataHub.listaTicks[i].Ativo == tickDTO.Ativo)
                            {
                                DataHub.listaTicks[i] = tickDTO;
                                break;
                            }
                        }
                    }
                    else
                    {
                        DataHub.listaTicks.Add(tickDTO);
                    }
                    
                    

                    //Disparando evento de tick
                    DisparaEventoTick(tickDTO);
                }

                return true;
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
                return false;
            }
        }
        #endregion Processamento de Tick

        #region Processamento de Negocio
        /// <summary>
        /// Parsea um pacote de negócio e dispara o evento de negócio.
        /// </summary>
        /// <param name="negocioString">Dado a ser parseado.</param>
        internal bool ProcessaNegocio(string negocioString)
        {
            try
            {
                string[] negocio = negocioString.Split(':');

                //Só devo processar ativos assinados
                if (negociosAssinados.Contains(negocio[1]))
                {
                    NegocioDTO negocioDTO = new NegocioDTO();

                    negocioDTO.Ativo = negocio[1];
                    negocioDTO.Bolsa = Convert.ToInt32(negocio[2]);
                    negocioDTO.HoraBolsa = negocio[3];
                    negocioDTO.Valor = Convert.ToDouble(negocio[4], provider);
                    negocioDTO.Quantidade = Convert.ToDouble(negocio[5], provider);
                    negocioDTO.Numero = Convert.ToInt32(negocio[6], provider);

                    //Atenção: Os DTOs de corretoras gerados não refletem as corretora do banco.
                    if (negocio[7] == "null")
                        negocioDTO.CorretoraCompradora = 0;
                    else
                        negocioDTO.CorretoraCompradora = Convert.ToInt32(negocio[7]);

                    if (negocio[8] == "null")
                        negocioDTO.CorretoraVendedora = 0;
                    else
                        negocioDTO.CorretoraVendedora = Convert.ToInt32(negocio[8]);

                    //Acertando hora se necessario
                    if (negocioDTO.HoraBolsa.Length == 3)
                        negocioDTO.HoraBolsa = "0" + negocioDTO.HoraBolsa;

                    negocioDTO.TimeStamp = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, Convert.ToInt32(negocioDTO.HoraBolsa.Substring(0, 2)), Convert.ToInt32(negocioDTO.HoraBolsa.Substring(2, 2)), 0);

                    //Disparando evento de negocio
                    DisparaEventoNegocio(negocioDTO);

                    return true;
                }

                return false;
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
                return false;
            }
        }
        #endregion Processamento de Negocio

        #region Processamento de Hora
        /// <summary>
        /// Processa um pacote do tipo hora e dispara o evento correspondente a este pacote.
        /// </summary>
        /// <param name="data">Dado a ser parseado.</param>
        internal bool ProcessaHora(string data)
        {
            try
            {
                //Criando o objeto Hora
                HoraDTO horaObj = new HoraDTO();

                //Recuperando o dado e splitando para poder transforma-lo em coluna
                horaObj.UltimaHora = data.Substring(2, data.Length - 2);

                //Disparando evento de hora
                DisparaEventoHora(horaObj);

                return true;
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);

                return false;
            }
        }
        #endregion Processamento de Hora

        #region Processamento de Book Completo
        /// <summary>
        /// Processa um comando de book, disparando evento.
        /// </summary>
        /// <param name="dado">Dado a ser parseado.</param>
        internal bool ProcessaBookCompleto(string dado)
        {
            try
            {
                ComandoBookDTO comando = new ComandoBookDTO(dado);
                
                DisparaEventoComandoBookCompleto(comando);

                return true;
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
                return false;
            }
        }
        #endregion Processamento de Book Bovespa

        #region Processamento de Noticia Bovespa
        /// <summary>
        /// Processa uma noticia da bovespa.
        /// </summary>
        /// <param name="data"></param>
        internal void ProcessaNoticiaBovespa(string noticiaString)
        {
            string[] dados = noticiaString.Replace("Z:", "").Split('|');

            NoticiaDTO noticia = new NoticiaDTO();

            noticia.Agencia = dados[0];
            noticia.Prioridade = dados[1];
            noticia.Data = Convert.ToDateTime(dados[3]);
            noticia.Titulo = dados[4];
            noticia.Texto = dados[5];

            DisparaEventoNoticia(noticia);
        }
        #endregion Processamento de Noticia Bovespa

        #region Processamento de Noticia BMF
        /// <summary>
        /// Processa uma noticia da BMF.
        /// </summary>
        /// <param name="data"></param>
        internal void ProcessaNoticiaBMF(string noticiaString)
        {
            string[] dados = noticiaString.Replace("U:", "").Split('|');

            NoticiaDTO noticia = new NoticiaDTO();

            noticia.Agencia = dados[0];
            noticia.Prioridade = dados[1];
            noticia.Data = Convert.ToDateTime(dados[3]);
            noticia.Titulo = dados[4];
            noticia.Texto = dados[5];

            DisparaEventoNoticia(noticia);
        }
        #endregion Processamento de Noticia BMF

        #region Processamento de Indice Bovespa
        /// <summary>
        /// Parsea o dados de um pacote tick e dispara o evento OnTick.
        /// </summary>
        /// <param name="tickString">Dado a ser parseado.</param>
        internal bool ProcessaIndiceBovespa(string indiceString)
        {
            try
            {
                //Formato: I:[1-Indice]:[2-Volume]:[3-Ultimo]:[4-Maximo]:[5-Hora Max]:[6-Minimo]:[7-Hora Min]
                //          :[8-Variacao]:[9-Num Papeis em Alta]:[10-Num Papeis em Baixa]:[11-Num Papeis sem Variacao]
                //          :[12-Hora]:[13-Quantidade]:[14-Negocios]:[15-Abertura]:[16-Media]:[17-Volume Minuto]:[18-Fechamento]

                IndiceBovespaDTO indiceDTO = new IndiceBovespaDTO(new List<string>(), 0, "");
                string[] indice = indiceString.Split(':');

                //Só devo processar ativos assinados
                if (indicesBovespaAssinados.Contains(indice[1]))
                {
                    indiceDTO.Index = indice[1];
                    indiceDTO.Volume = Convert.ToDouble(indice[2], provider);
                    indiceDTO.UltimoIndiceDia = Convert.ToDouble(indice[3], provider);
                    indiceDTO.MaisAltoIndiceDia = Convert.ToDouble(indice[4], provider);
                    indiceDTO.HoraMaisAltoIndiceDia = indice[5];
                    indiceDTO.MaisBaixoIndiceDia = Convert.ToDouble(indice[6], provider);
                    indiceDTO.HoraMaisBaixoIndiceDia = indice[7];
                    indiceDTO.Variacao = Convert.ToDouble(indice[8], provider) / 100;
                    indiceDTO.NumPapeisEmAltaCarteiraIndice = Convert.ToInt32(indice[9], provider);
                    indiceDTO.NumPapeisEmBaixaCarteiraIndice = Convert.ToInt32(indice[10], provider);
                    indiceDTO.NumPapeisSemVariacaoCarteiraIndice = Convert.ToInt32(indice[11], provider);

                    //Hora
                    indiceDTO.Hora = (indice[12].Length > 3) ? indiceDTO.Hora = indice[12].Substring(0, 4) : indiceDTO.Hora = "0000";

                    indiceDTO.Quantidade = Convert.ToDouble(indice[13], provider);
                    indiceDTO.Negocios = Convert.ToDouble(indice[14], provider);
                    indiceDTO.Abertura = Convert.ToDouble(indice[15], provider);
                    indiceDTO.Media = Convert.ToDouble(indice[16], provider);
                    indiceDTO.VolumeMinuto = Convert.ToDouble(indice[17], provider);
                    indiceDTO.Fechamento = Convert.ToDouble(indice[18], provider);

                    DisparaEventoIndiceBovespa(indiceDTO);
                }

                return true;
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);
                return false;
            }
        }
        #endregion Processamento de Indice Bovespa

        #region Processamento de Comando de Monitoramento
        /// <summary>
        /// Processa o comando de monitoramento do servico.
        /// </summary>
        /// <param name="comando"></param>
        internal bool ProcessaComandoMonitoramento(string comando)
        {
            try
            {
                comando = comando.Replace("F:", "");

                int dia = Convert.ToInt32(comando.Substring(0, 2));
                int mes = Convert.ToInt32(comando.Substring(2, 2));
                int ano = Convert.ToInt32(comando.Substring(4, 4));
                int hora = Convert.ToInt32(comando.Substring(8, 2));
                int minuto = Convert.ToInt32(comando.Substring(10, 2));
                int segundo = Convert.ToInt32(comando.Substring(12, 2));

                //Neste passo a msg deve estar com o seguinte formato: "F:[ENVIO SERVICO]
                DateTime horaServidor = new DateTime(ano, mes, dia, hora, minuto, segundo);

                horaSincronizacao = horaServidor;

                EnviaSolicitacaoAoServidor("F:" + comando);

                return true;
            }
            catch (Exception exc)
            {
                DisparaEventoErro(exc);

                return false;
            }
        }
        #endregion Processamento de Comando de Monitoramento
    }
}
