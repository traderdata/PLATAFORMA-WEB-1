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
using System.ServiceModel;
using System.Windows.Navigation;
using Traderdata.Client.Componente.GraficoSL.Main;
using Traderdata.Client.GraficoWEB.Dialog;
using Traderdata.Client.GraficoWEB.Util;
using Traderdata.Client.Componente.GraficoSL.DTO;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.GraficoWEB
{
    public partial class Principal : UserControl
    {

        #region Variaveis Privadas

        
        
        //TODO:Retirar
        private const int IdUsuario = 198;
        private const int idProdutoDDF = 3;

        //Configuração padrao
        private GraficoSVC.ConfiguracaoPadraoDTO configuracaoPadraDTO;

        //Area de trabalho do cliente logado
        private GraficoSVC.AreaTrabalhoDTO areaTrabalhoDTO; 

        //Variavel de controle de grafico selecionado
        private Grafico graficoSelecionado;
        private int indexGraficoSelecionado = -1;

        //Lista de controle dos graifcos abertos na tela
        private List<Grafico> listaGraficos = new List<Grafico>();

        //Varialve de serviço
        private GraficoSVC.GraficoWebClient baseGrafico;
        private DDFSVC.DDFClient baseDDF;

        //TODO:Retirar a lista abaixo, pois posso pegar direto da propriedade do DDF
        private List<string> listaAtivosLocal = new List<string>();
        
        
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Principal()
        {
            InitializeComponent();

            //Iniciando os serviços
            baseGrafico = new GraficoSVC.GraficoWebClient(ServiceWCF.basicBind, ServiceWCF.endPointAddressGrafico);
            baseDDF = new DDFSVC.DDFClient(ServiceWCF.basicBind, ServiceWCF.endPointAddressDDF);

            //Associando o novo behaviuour aos serviços
            baseDDF.ChannelFactory.Endpoint.Behaviors.Add(new Util.ClassBehaviour());
            
            //Assinando os serviços de DDF
            baseDDF.GetAtivosBovespaCompleted += new EventHandler<DDFSVC.GetAtivosBovespaCompletedEventArgs>(baseDDF_GetAtivosBovespaCompleted);
            baseDDF.GetAtivosBMFCompleted += new EventHandler<DDFSVC.GetAtivosBMFCompletedEventArgs>(baseDDF_GetAtivosBMFCompleted);
            baseDDF.ConnectCompleted += new EventHandler<DDFSVC.ConnectCompletedEventArgs>(baseDDF_ConnectCompleted);
            baseDDF.GetHistoricoBovespaCompleted += new EventHandler<DDFSVC.GetHistoricoBovespaCompletedEventArgs>(baseDDF_GetHistoricoBovespaCompleted);

            //Assinando os serviços de Grafico
            baseGrafico.RetornaAreaTrabalhoPorIdCompleted += new EventHandler<GraficoSVC.RetornaAreaTrabalhoPorIdCompletedEventArgs>(baseGrafico_RetornaAreaTrabalhoPorIdCompleted);
            baseGrafico.RetornaConfiguracaoPadraoPorIdCompleted += new EventHandler<GraficoSVC.RetornaConfiguracaoPadraoPorIdCompletedEventArgs>(baseGrafico_RetornaConfiguracaoPadraoPorIdCompleted);
            baseGrafico.RetornaTemplatesPorClientIdCompleted +=new EventHandler<GraficoSVC.RetornaTemplatesPorClientIdCompletedEventArgs>(baseGrafico_RetornaTemplatesPorClientIdCompleted);
            baseGrafico.SalvaAreaTrabalhoCompleted += new EventHandler<GraficoSVC.SalvaAreaTrabalhoCompletedEventArgs>(baseGrafico_SalvaAreaTrabalhoCompleted);
            baseGrafico.SalvaConfiguracaoPadraoCompleted += new EventHandler<GraficoSVC.SalvaConfiguracaoPadraoCompletedEventArgs>(baseGrafico_SalvaConfiguracaoPadraoCompleted);
            baseGrafico.SalvaTemplateCompleted += new EventHandler<GraficoSVC.SalvaTemplateCompletedEventArgs>(baseGrafico_SalvaTemplateCompleted);
            baseGrafico.ExcluiTemplateCompleted += new EventHandler<GraficoSVC.ExcluiTemplateCompletedEventArgs>(baseGrafico_ExcluiTemplateCompleted);

        }


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
            //Variavel que armazena um possivel retorno logico do metodo
            string mensagemRetorno = "";

            //try
            //{
            //    Login telaLogin = new Login();

            //    telaLogin.Closing += (sender1, e1) =>
            //    {
            //        if (telaLogin.DialogResult == true)
            //        {
            //            ModoCarregando(true);

            //            loginUsuario = telaLogin.ClienteDTO.email;
            //            guidUsuario = telaLogin.ClienteDTO.guid;

            //            //Chamando metodo responsavel por obter ativos assincronamente
            //            //OBS: O GRAFICO SÓ SERÁ INICIADO NA RESPOSTA DOS ATIVOS
            //            classeDados.ObtemAtivosAync(guidUsuario);

            //        }
            //    };
            //    telaLogin.Show();
            //}
            //catch
            //{
            //    MessageBox.Show("Não foi possível carregar sua área de trabalho.", "Aviso", MessageBoxButton.OK);
            //}
            //finally
            //{
            //    ModoCarregando(false);
            //}

            //Conectando: TODO: retirar e colocar na tela de login
            baseDDF.ConnectAsync(ServiceWCF.LoginUsuario, "monica00", ServiceWCF.LoginSistema, ServiceWCF.SenhaSistema);

            //Setando o login
            Util.ServiceWCF.LoginUsuario = "felsoares";
                      

            //Retornando a configuração padrao deste usuário
            baseGrafico.RetornaConfiguracaoPadraoPorIdAsync(IdUsuario, mensagemRetorno);

            //Retornando a área de trabalho deste cliente
            baseGrafico.RetornaAreaTrabalhoPorIdAsync(IdUsuario, mensagemRetorno);

        }
        #endregion LayoutRoot

        #endregion

        #region Visualização

        #region Mudança de Tamanho da Página
        /// <summary>
        /// Evento Resize: Quando o browser fopr redimensionado para um  tamanho menor que 773, "Powered by Traderdata" some.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (graficoSelecionado != null)
            {
                if (e.NewSize.Width < 773)
                    graficoSelecionado.VisibilidadeLinkTD = false;
                else
                    graficoSelecionado.VisibilidadeLinkTD = true;
            }
        }
        #endregion Mudança de Tamanho da Página

        #region Expander

        #region Colapse
        /// <summary>
        /// Escondendo conteudo do expander.
        /// </summary>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            pnlGraficoContainer.Margin = new Thickness(25, 0, 0, 0);
        }
        #endregion Colapse

        #region Expand
        /// <summary>
        /// Mostrando conteudo do expander.
        /// </summary>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            pnlGraficoContainer.Margin = new Thickness(245, 0, 0, 0);
        }
        #endregion Expand

        #endregion Expander

        #region SubMenus

        #region Mouse Enter
        /// <summary>
        /// Mouse Enter
        /// Seta gradiente vista com offset 2
        /// </summary>
        private void subMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            Border borda = sender as Border;

            if (borda != null)
                borda.Background = WindowsVistaGradiente(2);
        }
        #endregion Mouse Enter

        #region MouseLeave
        /// <summary>
        /// Mouse Enter
        /// Seta gradiente vista com offset 1
        /// </summary>
        private void subMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            Border borda = sender as Border;

            if (borda != null)
                borda.Background = WindowsVistaGradiente(1);
        }
        #endregion MouseLeave

        #endregion SubMenus

        #endregion

        #region Area de Trabalho

        #region SalvarAT
        /// <summary>
        /// Salva área de trabalho.
        /// </summary>
        private void btnSalvarATAux_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Variavel de controle de mensagem de retorno
            string mensagemRetorno = "";

            try
            {
                //Ler a configuração dos gráficos presentes e suas configurações
                foreach (Grafico obj in listaGraficos)
                {
                    this.areaTrabalhoDTO.Graficos.Add(ConvertGraficoComponenteToServer(obj));
                }
                                
                //Salvar as configurações
                baseGrafico.SalvaAreaTrabalhoAsync(this.areaTrabalhoDTO, mensagemRetorno);
            }
            catch
            {
                MessageBox.Show("Erro ao salvar a área de trabalho.");
            }
            finally
            {
                //ModoCarregando(false);
            }
        }
        #endregion SalvarAT

        #region Completed

        private void baseGrafico_SalvaAreaTrabalhoCompleted(object sender, GraficoSVC.SalvaAreaTrabalhoCompletedEventArgs e)
        {
            MessageBox.Show("Área de trabalho salva com sucesso!");
        }

        /// <summary>
        /// Evento disparado quando após se carregar a área de trabalho do cliente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseGrafico_RetornaAreaTrabalhoPorIdCompleted(object sender, GraficoSVC.RetornaAreaTrabalhoPorIdCompletedEventArgs e)
        {
            this.areaTrabalhoDTO = (GraficoSVC.AreaTrabalhoDTO)e.Result;
        }


        #endregion

        #endregion

        #region Template

        #region Clique Titulo
        /// <summary>
        /// Expande ou esconde o painel de opcoes de template.
        /// </summary>
        private void bordaTituloTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (stpTemplate.Visibility == Visibility.Visible)
                stpTemplate.Visibility = Visibility.Collapsed;
            else
                stpTemplate.Visibility = Visibility.Visible;
        }
        #endregion Clique Titulo

        #region Aplicar Template
        /// <summary>
        /// Aplica o template desejado ao gráfico seleiconado.
        /// </summary>
        private void btnAplicarTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Variavel de retorno de mensagem
            string mensagemRetorno = "";

            try
            {
                baseGrafico.RetornaTemplatesPorClientIdAsync(IdUsuario, mensagemRetorno, "APLICAR");
            }
            catch
            {
                MessageBox.Show("Não foi possível salvar seu template.", "Aviso", MessageBoxButton.OK);
            }
        }
        #endregion Aplicar Template

        #region Salvar Template
        /// <summary>
        /// Salva o template do gráfico selecionado.
        /// </summary>
        private void btnSalvarTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string mensagemRetorno = "";

            try
            {
                baseGrafico.RetornaTemplatesPorClientIdAsync(IdUsuario, mensagemRetorno, "SALVAR");                
            }
            catch
            {
                MessageBox.Show("Não foi possível salvar seu template.", "Aviso", MessageBoxButton.OK);
            }

        }
        #endregion Salvar Template

        #region Exclui Template
        /// <summary>
        /// Exclui o template desejado.
        /// </summary>
        private void btnExcluirTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Variavel de retorno de mensagem
            string mensagemRetorno = "";

            try
            {
                baseGrafico.RetornaTemplatesPorClientIdAsync(IdUsuario, mensagemRetorno, "EXCLUIR");
            }
            catch
            {
                MessageBox.Show("Não foi possível salvar seu template.", "Aviso", MessageBoxButton.OK);
            }
        }
        
        #endregion Exclui Template
        
        #region Completed

        /// <summary>
        /// Evento disparado ao terminar de trazer os templates de determinado cliente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseGrafico_RetornaTemplatesPorClientIdCompleted(object sender, GraficoSVC.RetornaTemplatesPorClientIdCompletedEventArgs e)
        {
            try
            {
                switch ((string)e.UserState)
                {
                    case "EXCLUIR":
                        //Realizando a ordem para exclusão do template
                        //baseGrafico.ExcluiTemplateAsync(dialogSelecaoTemplate.TemplateLocal, mensagemRetorno);
                        ExcluirTemplate((List<GraficoSVC.TemplateDTO>)e.Result);
                        break;
                    case "APLICAR":
                        //Aplicando o template no gráfico selecionado
                        AplicarTemplate((List<GraficoSVC.TemplateDTO>)e.Result);
                        break;
                    case "SALVAR":
                        SalvarTemplate((List<GraficoSVC.TemplateDTO>)e.Result);
                        break;
                }

            }
            catch (Exception exc)
            {
                throw exc;
            }

        }

        /// <summary>
        /// Evento disparado ao terminar de excluir o template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseGrafico_ExcluiTemplateCompleted(object sender, GraficoSVC.ExcluiTemplateCompletedEventArgs e)
        {
            MessageBox.Show("Template excluído com sucesso.");
        }

        /// <summary>
        /// Evento disparado após salvar um template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseGrafico_SalvaTemplateCompleted(object sender, GraficoSVC.SalvaTemplateCompletedEventArgs e)
        {            
            MessageBox.Show("Template salvo com sucesso.");
        }

        #endregion

        #endregion Template

        #region Configuração Padrão

        #region Dialog Configuracao Padrao

        /// <summary>
        /// Evento executado ao se clicar no link de configuração padrão
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfigPadrao_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {   
         
            //Variavel onde serão controladas possiveis mensagens de retorno
            string mensagemRetorno = "";

            ConfiguracaoPadrao dialogConfiguracaoPadrao = new ConfiguracaoPadrao(this.configuracaoPadraDTO);

            dialogConfiguracaoPadrao.Closing += (sender1, e1) =>
            {
                if (dialogConfiguracaoPadrao.DialogResult == true)
                {
                    this.configuracaoPadraDTO = dialogConfiguracaoPadrao.ConfiguracaoPadraoDTO;                        
                    baseGrafico.SalvaConfiguracaoPadraoAsync(this.configuracaoPadraDTO, mensagemRetorno);
                }
            };

            dialogConfiguracaoPadrao.Show();
            
        }

        #endregion

        #region Completed

        /// <summary>
        /// Evento disparado quando encerra o carregamento da configuracao padrao
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseGrafico_RetornaConfiguracaoPadraoPorIdCompleted(object sender, GraficoSVC.RetornaConfiguracaoPadraoPorIdCompletedEventArgs e)
        {
            try
            {
                this.configuracaoPadraDTO = (GraficoSVC.ConfiguracaoPadraoDTO)e.Result;
            }
            catch (Exception exc)
            {                
                throw exc;
            }

        }

        /// <summary>
        /// Evento disparado quando encerra o processamento de salvar configurações padroes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseGrafico_SalvaConfiguracaoPadraoCompleted(object sender, GraficoSVC.SalvaConfiguracaoPadraoCompletedEventArgs e)
        {
            MessageBox.Show("Configurações salvas com sucesso!");
        }


        #endregion

        #endregion

        #region Eventos do Componente Grafico

        /// <summary>
        /// Evento disparado quando o usuario clica em alterar periodo dentro do grafico
        /// </summary>
        /// <param name="configuracaoGraficoLocal"></param>
        private void graficoSelecionado_OnAlteraPeriodo(ConfiguracaoGraficoDTO configuracaoGraficoLocal)
        {
            try
            {
                AtualizaGrafico();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Evento disparado quandoo o usuario altera a periodicidade
        /// </summary>
        /// <param name="configuracaoGraficoLocal"></param>
        private void graficoSelecionado_OnAlteraPeriodicidade(ConfiguracaoGraficoDTO configuracaoGraficoLocal)
        {            
            try
            {
                AtualizaGrafico();
            }
            catch (Exception exc)
            {
                throw exc;
            }

        }

        /// <summary>
        /// Evento que ocorre quando atualiza os dados
        /// </summary>
        /// <param name="configuracaoGraficoLocal"></param>
        private void graficoSelecionado_OnAtualizaDados(ConfiguracaoGraficoDTO configuracaoGraficoLocal)
        {
            AtualizaGrafico();
        }

        /// <summary>
        /// Evento que roda quando se clica sobre o nome do ativo
        /// </summary>
        /// <param name="configuracaoGraficoLocal"></param>
        private void graficoSelecionado_OnAtualizaAtivo(ConfiguracaoGraficoDTO configuracaoGraficoLocal)
        {
            //Instanciando uma nova tela de novoGraficoDialog passando ativo vazio e periodicidade
            NovoGrafico novoGraficoDialog = new NovoGrafico(listaAtivosLocal);

            novoGraficoDialog.Closing += (sender1, e1) =>
            {
                if (novoGraficoDialog.DialogResult == true)
                {
                    //Carregar dados no objeto do gráfico - termina de carregar no completed do evento
                    baseDDF.GetHistoricoBovespaAsync(novoGraficoDialog.Ativo, novoGraficoDialog.Periodicidade.Value, novoGraficoDialog.Periodo.Value, false, false, novoGraficoDialog);

                }
            };

            novoGraficoDialog.Show();
       
        }

        #endregion

        #region DDF

        /// <summary>
        /// Evento executado quando o metodo getAtivoBMF termina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseDDF_GetAtivosBMFCompleted(object sender, DDFSVC.GetAtivosBMFCompletedEventArgs e)
        {
            try
            {
                foreach (string obj in e.Result)
                {
                    listaAtivosLocal.Add(obj.Split(';')[1]);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Evento executado quando o metodo asyn getAtivoBovespa termina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseDDF_GetAtivosBovespaCompleted(object sender, DDFSVC.GetAtivosBovespaCompletedEventArgs e)
        {
            try
            {
                foreach (string obj in e.Result)
                {
                    listaAtivosLocal.Add(obj.Split(';')[1]);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

        }

        /// <summary>
        /// Evento disparado após a tentiva de conexão
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseDDF_ConnectCompleted(object sender, DDFSVC.ConnectCompletedEventArgs e)
        {
            //Setando a Guid
            ServiceWCF.Guids = (string)e.Result;

            //Retornando os ativos presentes no banco de bovespa
            baseDDF.GetAtivosBovespaAsync();

            //Retornando os ativos presentes no banco de bovespa
            baseDDF.GetAtivosBMFAsync();
        }

        /// <summary>
        /// Evento disparado ao terminar de carregar as cotações do BD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void baseDDF_GetHistoricoBovespaCompleted(object sender, DDFSVC.GetHistoricoBovespaCompletedEventArgs e)
        {
            List<DDFSVC.CotacaoDTO> lista = (List<DDFSVC.CotacaoDTO>)e.Result;
            
            //Criando o objeto de configuração a partir do padrão do usuário
            NovoGrafico novoGraficoObj = (NovoGrafico) e.UserState;

            if (novoGraficoObj != null)
                CarregaNovoGrafico(novoGraficoObj, lista);
            else
                CarregaGraficoExistente(lista);
            
        }

        
        #endregion

        #region Seleção Gráficos

        #region TituloSelecao
        /// <summary>
        /// Clique no titulo da selecao.
        /// Esconde subopcoes.
        /// </summary>
        private void bordaTituloSelecao_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (stpSelecaoGraficos.Visibility == Visibility.Visible)
                stpSelecaoGraficos.Visibility = Visibility.Collapsed;
            else
                stpSelecaoGraficos.Visibility = Visibility.Visible;
        }
        #endregion TituloSelecao

        #region Adiciona Grafico
        private void tabAdicionarGrafico_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Instanciando uma nova tela de novoGraficoDialog passando ativo vazio e periodicidade
            NovoGrafico novoGraficoDialog = new NovoGrafico(listaAtivosLocal);

            novoGraficoDialog.Closing += (sender1, e1) =>
            {
                if (novoGraficoDialog.DialogResult == true)
                {
                    //Carregar dados no objeto do gráfico - termina de carregar no completed do evento
                    baseDDF.GetHistoricoBovespaAsync(novoGraficoDialog.Ativo, novoGraficoDialog.Periodicidade.Value, novoGraficoDialog.Periodo.Value, false, false, novoGraficoDialog);
                    
                }
            };

            novoGraficoDialog.Show();
        }
        #endregion Adiciona Grafico

        #endregion Seleção Gráficos

        #endregion

        #region Metodos

        #region Grafico

        /// <summary>
        /// Metodo que atualiza o grafico
        /// </summary>
        private void AtualizaGrafico()
        {
            try
            {
                //Carregar os novos dados
                baseDDF.GetHistoricoBovespaAsync(graficoSelecionado.ConfiguracaoGraficoLocal.Ativo,
                    graficoSelecionado.ConfiguracaoGraficoLocal.Periodicidade.Value, graficoSelecionado.ConfiguracaoGraficoLocal.Periodo.Value,
                    true, true);

                //Atualizando o label
                ObtemTituloAba(graficoSelecionado.ConfiguracaoGraficoLocal.Ativo, graficoSelecionado.ConfiguracaoGraficoLocal.Periodicidade.Value,
                    graficoSelecionado.ConfiguracaoGraficoLocal.Periodo.Value);
            }
            catch (Exception exc)
            {                
                throw exc;
            }
            
        }

        /// <summary>
        /// Metodo que carrega um novo grafico
        /// </summary>
        /// <param name="novoGraficoObj"></param>
        private void CarregaNovoGrafico(NovoGrafico novoGraficoObj, List<DDFSVC.CotacaoDTO> lista)
        {
            try
            {
                ConfiguracaoGraficoDTO configuracaoNovoGraficoDTO 
                        = this.ConvertConfiguracaoServerToComponenteGrafico(this.configuracaoPadraDTO.Configuracao);
                ConfiguracaoPadraoDTO configuracaoPadraoLocalDTO
                    = this.ConvertConfiguracaoPadraoServerToComponente(this.configuracaoPadraDTO);

                //Setando as opções que o usuário selecionou agora
                configuracaoNovoGraficoDTO.Ativo = novoGraficoObj.Ativo;
                configuracaoNovoGraficoDTO.Periodicidade = novoGraficoObj.Periodicidade;
                configuracaoNovoGraficoDTO.Periodo = novoGraficoObj.Periodo;

                //Criando a nova instancia de gráfico
                Grafico novoGrafico = new Grafico(configuracaoPadraoLocalDTO, configuracaoNovoGraficoDTO);

                //Setando os dados
                foreach (DDFSVC.CotacaoDTO cotacaoObj in lista)
                {
                    if (cotacaoObj.Hora != "")
                        novoGrafico.Dados.Add(new BarraDTO(configuracaoNovoGraficoDTO.Ativo,
                            new DateTime(cotacaoObj.Data.Year, cotacaoObj.Data.Month, cotacaoObj.Data.Day,
                                Convert.ToInt16(cotacaoObj.Hora.Substring(0, 2)), Convert.ToInt16(cotacaoObj.Hora.Substring(2, 2)), 0),
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume));
                    else
                        novoGrafico.Dados.Add(new BarraDTO(configuracaoNovoGraficoDTO.Ativo, cotacaoObj.Data,
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume));
                }

                //Carregando os dados deste grafico
                pnlGraficoContainer.Children.Add(novoGrafico);

                //Executa a montagem do gráfico
                novoGrafico.Refresh(EnumGeral.TipoRefresh.All);
                                
                //Adiciona na lista de gráficos
                listaGraficos.Add(novoGrafico);
                CriaEsquemaPainel(listaGraficos.Count - 1, true);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        
        /// <summary>
        /// Metodo que atualiza um grafico existente
        /// </summary>
        private void CarregaGraficoExistente(List<DDFSVC.CotacaoDTO> lista)
        {
            try
            {
                //limpandos os dados
                graficoSelecionado.Dados.Clear();

                //Setando os dados
                foreach (DDFSVC.CotacaoDTO cotacaoObj in lista)
                {
                    if (cotacaoObj.Hora != "")
                        graficoSelecionado.Dados.Add(new BarraDTO(graficoSelecionado.ConfiguracaoGraficoLocal.Ativo,
                            new DateTime(cotacaoObj.Data.Year, cotacaoObj.Data.Month, cotacaoObj.Data.Day,
                                Convert.ToInt16(cotacaoObj.Hora.Substring(0, 2)), Convert.ToInt16(cotacaoObj.Hora.Substring(2, 2)), 0),
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume));
                    else
                        graficoSelecionado.Dados.Add(new BarraDTO(graficoSelecionado.ConfiguracaoGraficoLocal.Ativo, cotacaoObj.Data,
                            cotacaoObj.Abertura, cotacaoObj.Maximo, cotacaoObj.Minimo, cotacaoObj.Ultimo, cotacaoObj.Volume));
                }

                //Atualizando o grafico
                graficoSelecionado.Refresh(EnumGeral.TipoRefresh.All);
            }
            catch (Exception exc)
            {                
                throw exc;
            }
        }

        #endregion

        #region Templates

        /// <summary>
        /// Metodo salva o template
        /// </summary>
        private void SalvarTemplate(List<GraficoSVC.TemplateDTO> listaTemplate)
        {
            
            try
            {
                if (graficoSelecionado != null)
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
                    };
                    salvaTemplateDialog.Show();
                }
                else
                    MessageBox.Show("Você não possui gráfico ativo. Por favor abra um gráfico antes de salvar o template.");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que insere um novo template
        /// </summary>
        /// <param name="nome"></param>
        private void InserirTemplate(string nome)
        {
            GraficoSVC.TemplateDTO templateLocalDTO = new GraficoSVC.TemplateDTO();
            string mensagemRetorno = "";

            try
            {
                //Salvando o template novo com nome
                templateLocalDTO.Nome = nome;
                templateLocalDTO.Configuracao = ConvertConfiguracaoComponenteGraficoToServer(graficoSelecionado.ConfiguracaoGraficoLocal);
                templateLocalDTO.Id = 0;
                templateLocalDTO.ClienteId = IdUsuario;
                templateLocalDTO.Indicadores = new List<GraficoSVC.IndicadorDTO>();
                foreach (IndicadorDTO indicadorGrafico in graficoSelecionado.Indicadores)
                {
                    templateLocalDTO.Indicadores.Add(this.ConverteIndicadorComponenteToServer(indicadorGrafico));
                }

                templateLocalDTO.Periodicidade = graficoSelecionado.ConfiguracaoGraficoLocal.Periodicidade.Value;
                templateLocalDTO.Periodo = graficoSelecionado.ConfiguracaoGraficoLocal.Periodo.Value;

                baseGrafico.SalvaTemplateAsync(templateLocalDTO, mensagemRetorno);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que edita um template existente
        /// </summary>
        /// <param name="nome"></param>
        private void EditarTemplate(GraficoSVC.TemplateDTO templateLocalDTO)
        {
            string mensagemRetorno = "";

            try
            {
                //Salvando o template novo com nome
                templateLocalDTO.Configuracao = ConvertConfiguracaoComponenteGraficoToServer(graficoSelecionado.ConfiguracaoGraficoLocal);
                foreach (IndicadorDTO indicadorGrafico in graficoSelecionado.Indicadores)
                {
                    templateLocalDTO.Indicadores.Add(this.ConverteIndicadorComponenteToServer(indicadorGrafico));
                }

                templateLocalDTO.Periodicidade = graficoSelecionado.ConfiguracaoGraficoLocal.Periodicidade.Value;
                templateLocalDTO.Periodo = graficoSelecionado.ConfiguracaoGraficoLocal.Periodo.Value;

                baseGrafico.SalvaTemplateAsync(templateLocalDTO, mensagemRetorno);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que dispara a tela de exclusão de tempalte
        /// </summary>
        /// <param name="listaTemplate"></param>
        private void ExcluirTemplate(List<GraficoSVC.TemplateDTO> listaTemplate)
        {
            string mensagemRetorno = "";

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
                            baseGrafico.ExcluiTemplateAsync(dialogSelecaoTemplate.TemplateLocal, mensagemRetorno);
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
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que deve aplicar um template sobre o gráfico selecionado
        /// </summary>
        /// <param name="templateLocalDTO"></param>
        private void AplicarTemplate(List<GraficoSVC.TemplateDTO> listaTemplate)
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
                            graficoSelecionado.ConfiguracaoGraficoLocal = this.ConvertConfiguracaoServerToComponenteGrafico(dialogSelecaoTemplate.TemplateLocal.Configuracao);
                            graficoSelecionado.ConfiguracaoGraficoLocal.Periodo = EnumPeriodo.GetPeriodo((int)dialogSelecaoTemplate.TemplateLocal.Periodo);
                            graficoSelecionado.ConfiguracaoGraficoLocal.Periodicidade = EnumPeriodicidade.GetPeriodicidade((int)dialogSelecaoTemplate.TemplateLocal.Periodicidade);
                            List<IndicadorDTO> listaIndicador = new List<IndicadorDTO>();
                            foreach (GraficoSVC.IndicadorDTO indicador in dialogSelecaoTemplate.TemplateLocal.Indicadores)
                            {
                                
                                listaGraficos[listaGraficos.Count-1].AddIndicador(ConverteIndicadorServerToComponente(indicador));
                                //listaIndicador.Add(ConverteIndicadorServerToComponente(indicador));
                            }
//                            graficoSelecionado.Indicadores = listaIndicador;

                            //Exeuctando o refresh
                            AtualizaGrafico();
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
                throw exc;
            }
        }

        #endregion

        #region Conversores

        /// <summary>
        /// Converte o objeto do servidor para o tipo no componente
        /// </summary>
        /// <param name="configuracaoCliente"></param>
        /// <returns></returns>
        private ConfiguracaoPadraoDTO ConvertConfiguracaoPadraoServerToComponente(GraficoSVC.ConfiguracaoPadraoDTO configuracaoServer)
        {
            Traderdata.Client.Componente.GraficoSL.DTO.ConfiguracaoPadraoDTO configuracaoClient = new ConfiguracaoPadraoDTO();
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
        private ConfiguracaoGraficoDTO ConvertConfiguracaoServerToComponenteGrafico(GraficoSVC.ConfiguracaoGraficoDTO configuracaoServer)
        {
            ConfiguracaoGraficoDTO configuracaoClient;

            //Inicio com as configurações do gráfico selecionado
            if (graficoSelecionado != null)
                configuracaoClient = graficoSelecionado.ConfiguracaoGraficoLocal;
            else
                configuracaoClient = new ConfiguracaoGraficoDTO();
            
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
            configuracaoClient.NivelZoom = 5;
            configuracaoClient.PainelInfo = (bool)configuracaoServer.PainelInfo;
            configuracaoClient.PosicaoEscala = (EnumGeral.TipoAlinhamentoEscalaEnum)configuracaoServer.PosicaoEscala;
            configuracaoClient.PrecisaoEscala = (int)configuracaoServer.PrecisaoEscala;
            configuracaoClient.TipoEscala = (EnumGeral.TipoEscala)configuracaoServer.TipoEscala;
            configuracaoClient.TipoLinhaDefault = (EnumGeral.TipoLinha) configuracaoServer.TipoLinhaIndicador;
            configuracaoClient.TipoLinhaSerieFilha1Padrao = (EnumGeral.TipoLinha) configuracaoServer.TipoLinhaIndicadorAux1;
            configuracaoClient.TipoLinhaSerieFilha2Padrao = (EnumGeral.TipoLinha)configuracaoServer.TipoLinhaIndicadorAux2;
            configuracaoClient.TipoLinhaDefault = (EnumGeral.TipoLinha)configuracaoServer.TipoLinhaObjeto;

            //Retornando o objeto no padrao do componente
            return configuracaoClient;
        }

        /// <summary>
        /// Retorna um objeto de configuração de gráfico a partir da configuração padrão
        /// </summary>
        /// <param name="configuracaoCliente"></param>
        /// <returns></returns>
        private GraficoSVC.ConfiguracaoGraficoDTO ConvertConfiguracaoComponenteGraficoToServer(Traderdata.Client.Componente.GraficoSL.DTO.ConfiguracaoGraficoDTO configuracaoCliente)
        {
            try
            {
                GraficoSVC.ConfiguracaoGraficoDTO configuracaoServer = new GraficoSVC.ConfiguracaoGraficoDTO();
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

                //Retornando o objeto no padrao do componente
                return configuracaoServer;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Metodo converte o indicadorDTO do componente para o formato no servidor
        /// </summary>
        /// <param name="indicadorGrafico"></param>
        /// <returns></returns>
        private GraficoSVC.IndicadorDTO ConverteIndicadorComponenteToServer(Traderdata.Client.Componente.GraficoSL.DTO.IndicadorDTO indicadorGrafico)
        {
            GraficoSVC.IndicadorDTO indicadorServer = new GraficoSVC.IndicadorDTO();
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
                

                return indicadorServer;
            }
            catch (Exception exc)
            {                
                throw exc;
            }
        }

        /// <summary>
        /// Metodo que converte um grafico do componente para o servidor
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private GraficoSVC.GraficoDTO ConvertGraficoComponenteToServer(Grafico obj)
        {
            try
            {
                GraficoSVC.GraficoDTO graficoServer = new GraficoSVC.GraficoDTO();
                graficoServer.Ativo = obj.ConfiguracaoGraficoLocal.Ativo;
                graficoServer.Configuracao = ConvertConfiguracaoComponenteGraficoToServer(obj.ConfiguracaoGraficoLocal);
                graficoServer.Id = 0;
                graficoServer.Periodicidade = obj.ConfiguracaoGraficoLocal.Periodicidade.Value;
                graficoServer.Periodo = obj.ConfiguracaoGraficoLocal.Periodo.Value;
                foreach(IndicadorDTO indicador in obj.Indicadores)
                {
                    graficoServer.Indicadores.Add(ConverteIndicadorComponenteToServer(indicador));
                }
                foreach (ObjetoEstudoDTO objeto in obj.Objetos)
                {
                    //graficoServer.Objetos.Add(ConverteObjetoComponenteToServer(objeto));
                }

                return graficoServer;
            }
            catch (Exception exc)
            {                
                throw exc;
            }
        }

        /// <summary>
        /// Metodo converte o indicadorDTO do componente para o formato no servidor
        /// </summary>
        /// <param name="indicadorGrafico"></param>
        /// <returns></returns>
        private Traderdata.Client.Componente.GraficoSL.DTO.IndicadorDTO ConverteIndicadorServerToComponente(GraficoSVC.IndicadorDTO indicadorServer)
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

                return indicadorGrafico;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

       
        #endregion

        #region Metodos Auxiliares

        #region CriaEsquemaPainel()
        /// <summary>
        /// Cria bloco a bloco de selecao de listaGraficos.
        /// O parametro basea-se na posição do gráfico na lista.
        /// </summary>
        /// <param name="i"></param>
        private void CriaEsquemaPainel(int i, bool usaSelecao)
        {            
            Grafico grafico = listaGraficos[i];
                        
            Border borda = new Border();
            borda.BorderThickness = new Thickness(0.3);
            borda.BorderBrush = new SolidColorBrush(Colors.White);
            borda.Background = WindowsVistaGradiente(1);

            borda.MouseEnter += (sender, e) =>
            {
                if ((borda.Tag == null) || ((bool)borda.Tag == false))
                    borda.Background = WindowsVistaGradiente(2);
                else
                    borda.Background = new SolidColorBrush(Colors.Black);
            };

            borda.MouseLeave += (sender, e) =>
            {
                if ((borda.Tag == null) || ((bool)borda.Tag == false))
                    borda.Background = WindowsVistaGradiente(1);
                else
                    borda.Background = new SolidColorBrush(Colors.Black);
            };

            borda.Height = 30;

            grafico.Tag = borda;

            StackPanel painelClose = new StackPanel();
            painelClose.Orientation = Orientation.Horizontal;

            TextBlock lbl = new TextBlock();
            lbl.Foreground = new SolidColorBrush(Colors.White);
            lbl.TextAlignment = TextAlignment.Left;
            lbl.FontSize = 10;
            lbl.FontWeight = FontWeights.Bold;
            lbl.FontFamily = new FontFamily("Verdana");
            lbl.VerticalAlignment = VerticalAlignment.Center;
            lbl.Margin = new Thickness(20, 0, 0, 0);

            Image imgBtn = new Image();
            imgBtn.Source = (ImageSource)(new ImageSourceConverter()).ConvertFrom("Images/btnFechar.png");

            Button btnFechar = new Button();
            btnFechar.Margin = new Thickness(15, 0, 0, 0);
            btnFechar.Content = imgBtn;
            btnFechar.Height = 15;
            btnFechar.VerticalContentAlignment = VerticalAlignment.Top;
            btnFechar.VerticalAlignment = VerticalAlignment.Center;

            #region Eventos de Fechamento de Grafico

            btnFechar.Click += (sender, e) =>
            {
                //Ao fechar um gráfico, devo seleiconar um outro gráfico
                int index = listaGraficos.IndexOf(grafico);

                if (index != -1)
                {
                    if (index + 1 < listaGraficos.Count)
                        SelecionaGrafico((Border)listaGraficos[index + 1].Tag, listaGraficos[index + 1]);
                    else if ((index - 1 < listaGraficos.Count) && (listaGraficos.Count > 1))
                        SelecionaGrafico((Border)listaGraficos[index - 1].Tag, listaGraficos[index - 1]);
                    else if (listaGraficos.Count > 1)
                        SelecionaGrafico((Border)listaGraficos[0].Tag, listaGraficos[0]);
                }

                //TODO:Rever a necessidade                
                grafico.ConfiguracaoGraficoLocal.PainelInfo = false;
                grafico.Refresh(EnumGeral.TipoRefresh.Layout);


                stpSelecaoGraficos.Children.Remove(borda);
                pnlGraficoContainer.Children.Remove(grafico);
                listaGraficos.Remove(grafico);

                //if ((listaGraficos.Count <= 0) && (!adicionandoGrafico))
                //{
                //    adicionandoGrafico = true;

                //    //Instanciando uma nova tela de graficoLoadDialog passando ativo vazio e periodicidade
                //    GraficoLoadDialog graficoLoadDialog;
                //    graficoLoadDialog = new GraficoLoadDialog("", 180, 1440, true, ativos);

                //    graficoLoadDialog.Closing += (sender1, e1) =>
                //    {
                //        if (graficoLoadDialog.DialogResult == true)
                //        {
                //            GraficoSL novoGrafico = new GraficoSL(ativos);
                //            novoGrafico.Ativo = graficoLoadDialog.Ativo;
                //            novoGrafico.Periodicidade = graficoLoadDialog.Periodicidade;
                //            novoGrafico.Periodo = graficoLoadDialog.Periodo;

                //            novoGrafico.ConfigPadrao = this.configPadraoDTO;
                //            novoGrafico.PendenteConfigPadroes = true;

                //            listaGraficos.Add(novoGrafico);
                //            novoGrafico.AguardandoCarregamento = false;

                //            CriaEsquemaPainel(listaGraficos.Count - 1, true);
                //        }

                //        adicionandoGrafico = false;
                //    };

                //    graficoLoadDialog.Show();
                //}
            };

            imgBtn.MouseLeftButtonDown += (sender, e) =>
            {
                //Ao fechar um gráfico, devo seleiconar um outro gráfico
                int index = listaGraficos.IndexOf(grafico);

                if (index != -1)
                {
                    if (index + 1 < listaGraficos.Count)
                        SelecionaGrafico((Border)listaGraficos[index + 1].Tag, listaGraficos[index + 1]);
                    else if ((index - 1 < listaGraficos.Count) && (listaGraficos.Count > 1))
                        SelecionaGrafico((Border)listaGraficos[index - 1].Tag, listaGraficos[index - 1]);
                    else if (listaGraficos.Count > 1)
                        SelecionaGrafico((Border)listaGraficos[0].Tag, listaGraficos[0]);
                }

                //TODO:Rever a necessidade                
                grafico.ConfiguracaoGraficoLocal.PainelInfo = false;
                grafico.Refresh(EnumGeral.TipoRefresh.Layout);

                stpSelecaoGraficos.Children.Remove(borda);
                pnlGraficoContainer.Children.Remove(grafico);
                listaGraficos.Remove(grafico);

                //if ((listaGraficos.Count <= 0) && (!adicionandoGrafico))
                //{
                //    adicionandoGrafico = true;

                //    //Instanciando uma nova tela de graficoLoadDialog passando ativo vazio e periodicidade
                //    GraficoLoadDialog graficoLoadDialog;
                //    graficoLoadDialog = new GraficoLoadDialog("", 180, 1440, true, ativos);

                //    graficoLoadDialog.Closing += (sender1, e1) =>
                //    {
                //        if (graficoLoadDialog.DialogResult == true)
                //        {
                //            GraficoSL novoGrafico = new GraficoSL(ativos);
                //            novoGrafico.Ativo = graficoLoadDialog.Ativo;
                //            novoGrafico.Periodicidade = graficoLoadDialog.Periodicidade;
                //            novoGrafico.Periodo = graficoLoadDialog.Periodo;

                //            novoGrafico.ConfigPadrao = this.configPadraoDTO;
                //            novoGrafico.PendenteConfigPadroes = true;

                //            listaGraficos.Add(novoGrafico);
                //            novoGrafico.AguardandoCarregamento = false;

                //            CriaEsquemaPainel(listaGraficos.Count - 1, true);
                //        }

                //        adicionandoGrafico = false;
                //    };

                //    graficoLoadDialog.Show();
                //}
            };

            #endregion Eventos de Fechamento de Grafico

            painelClose.Children.Add(lbl);
            painelClose.Children.Add(btnFechar);

            borda.Child = painelClose;
            stpSelecaoGraficos.Children.Insert(stpSelecaoGraficos.Children.Count - 1, borda);

            //Criando painel do gráfico
            listaGraficos[i].Margin = new Thickness(0);

            lbl.Text = ObtemTituloAba(listaGraficos[i].ConfiguracaoGraficoLocal.Ativo, listaGraficos[i].ConfiguracaoGraficoLocal.Periodicidade.Value, listaGraficos[i].ConfiguracaoGraficoLocal.Periodo.Value);

            Canvas.SetZIndex(listaGraficos[i], 1);

            //Setando o zindex do gráfico anterior como 0
            if (listaGraficos.Count - 1 > 0)
                Canvas.SetZIndex(listaGraficos[listaGraficos.Count - 1], 0);

            //TODO:Eventos que alteram periodo e periodicidade
            //assinando eventos do componente grafico
            listaGraficos[i].OnAlteraPeriodicidade += new Grafico.OnAlteraPeriodicidadeDelegate(graficoSelecionado_OnAlteraPeriodicidade);
            listaGraficos[i].OnAlteraPeriodo += new Grafico.OnAlteraPeriodoDelegate(graficoSelecionado_OnAlteraPeriodo);
            listaGraficos[i].OnAtualizaAtivo += new Grafico.OnAtualizaAtivoDelegate(graficoSelecionado_OnAtualizaAtivo);
            listaGraficos[i].OnAtualizaDados += new Grafico.OnAtualizaDadosDelegate(graficoSelecionado_OnAtualizaDados);

            //listaGraficos[i].OnGraficoChange += (sender) => lbl.Text = ObtemTituloAba(((GraficoSL)sender).Ativo, ((GraficoSL)sender).Periodicidade, ((GraficoSL)sender).Periodo);


            //Criando evento de clique na borda
            borda.MouseLeftButtonDown += (sender, e) =>
            {
                //Colocando os gráficos atras do grafico selecionado
                foreach (Grafico obj in listaGraficos)
                {
                    Canvas.SetZIndex(obj, 0);
                }

                //Selecionando tab do gráfico
                foreach (object obj in stpSelecaoGraficos.Children)
                {
                    Border bord = obj as Border;

                    if (bord != null)
                    {
                        bord.Tag = false;
                        bord.Background = WindowsVistaGradiente(1);
                    }
                }

                //Dando foco ao gráfico
                Canvas.SetZIndex(grafico, 1);
                borda.Tag = true;
                borda.Background = new SolidColorBrush(Colors.Black);

                graficoSelecionado = grafico;

                //if (graficoSelecionado.AguardandoCarregamento)
                //{
                //    graficoSelecionado.AtualizaChart(graficoSelecionado.Ativo, DateTime.Today.Subtract(new TimeSpan(graficoSelecionado.Periodo, 0, 0, 0, 0)), DateTime.Now, graficoSelecionado.Ajuste, "AGORA", graficoSelecionado.Periodicidade, true);
                //    graficoSelecionado.AguardandoCarregamento = false;
                //}
            };

            graficoSelecionado = grafico;

            if (usaSelecao)
                SelecionaGrafico(borda, grafico);
        }
        #endregion CriaEsquemaPainel()

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

        #region SelecionaGrafico
        /// <summary>
        /// Seleciona o gráfico desejado.
        /// </summary>
        /// <param name="borda"></param>
        /// <param name="grafico"></param>
        private void SelecionaGrafico(Border borda, Grafico grafico)
        {
            //Selecionando este item
            foreach (Grafico obj in listaGraficos)
            {
                Canvas.SetZIndex(obj, 0);
            }

            foreach (object obj in stpSelecaoGraficos.Children)
            {
                Border bord = obj as Border;

                if (bord != null)
                {
                    bord.Tag = false;
                    bord.Background = WindowsVistaGradiente(1);
                }
            }

            Canvas.SetZIndex(grafico, 1);
            borda.Tag = true;
            borda.Background = new SolidColorBrush(Colors.Black);

            graficoSelecionado = grafico;

            //if (grafico.AguardandoCarregamento)
            //{
            //    grafico.AtualizaChart(grafico.Ativo, DateTime.Today.Subtract(new TimeSpan(grafico.Periodo, 0, 0, 0, 0)), DateTime.Now, grafico.Ajuste, "AGORA", grafico.Periodicidade, true);
            //    grafico.AguardandoCarregamento = false;
            //}
        }
        #endregion SelecionaGrafico

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

        #endregion

        #endregion

    }
}
