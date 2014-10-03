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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Browser;
using System.ServiceModel;
using Traderdata.Client.Componente.GraficoSL.StockChart;
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.Main;

namespace Traderdata.Client.GraficoWEB
{
    public partial class MainPage : UserControl
    {
        #region Campos e Contrutores
        
        //Variaveis de controle de usuario
        private string loginUsuario = "";
        private string loginCorretora = "agora";
        private string guidUsuario = "";
        private bool adicionandoGrafico = false;

        //Armazenamento
        private bool AplicaTemplate = false;
        private bool RetornaTemplate = false;
        private List<Grafico> listaGraficos = new List<Grafico>();        
        private GraficoSVC.AreaTrabalhoDTO areaTrabalho = new GraficoSVC.AreaTrabalhoDTO();                
        private List<string> ativos = new List<string>();        
        private Grafico graficoSelecionado = null;
                
        //Inicializando o webservice wcf de grafico
        private GraficoSVC.GraficoWebClient baseGrafico;
        private BasicHttpBinding bindGrafico = new BasicHttpBinding();
        private EndpointAddress endpointGrafico = new EndpointAddress("http://201.49.223.90/GraficoWeb");

        private bool suporteAMultiplasATs = false;

        public MainPage()
        {
            InitializeComponent();
                        
            //Iniciando Client WCF Grafico
            bindGrafico.MaxReceivedMessageSize = 10000000;
            baseGrafico = new GraficoWeb.GraficoSVC.GraficoWebClient(bindGrafico, endpointGrafico);
            baseGrafico.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 0, 900000);

            //Assinando evento de web para grafico e template
            baseGrafico.RetornaAreaTrabalhoPorLoginCompleted += new EventHandler<GraficoWeb.GraficoSVC.RetornaAreaTrabalhoPorLoginCompletedEventArgs>(baseGrafico_RetornaAreaTrabalhoPorLoginCompleted);
            baseGrafico.RetornaConfiguracaoPadraoCompleted += new EventHandler<GraficoWeb.GraficoSVC.RetornaConfiguracaoPadraoCompletedEventArgs>(baseGrafico_RetornaConfiguracaoPadraoCompleted);
            baseGrafico.RetornaTemplatePorIdCompleted += new EventHandler<GraficoWeb.GraficoSVC.RetornaTemplatePorIdCompletedEventArgs>(baseGrafico_RetornaTemplatePorIdCompleted);
            baseGrafico.SalvaAreaTrabalhoCompleted += new EventHandler<GraficoWeb.GraficoSVC.SalvaAreaTrabalhoCompletedEventArgs>(baseGrafico_SalvaAreaTrabalhoCompleted);
            baseGrafico.SalvaConfiguracaoPadraoCompleted += new EventHandler<GraficoWeb.GraficoSVC.SalvaConfiguracaoPadraoCompletedEventArgs>(baseGrafico_SalvaConfiguracaoPadraoCompleted);
            baseGrafico.SalvaTemplateCompleted += new EventHandler<GraficoWeb.GraficoSVC.SalvaTemplateCompletedEventArgs>(baseGrafico_SalvaTemplateCompleted);
                        
            //Configurações inicias de interface
            btnSalvarAreaTrabalho.Background = WindowsVistaGradiente(1);
            btnEditarAreaTrabalho.Background = WindowsVistaGradiente(1);
            btnExcluirAreaTrabalho.Background = WindowsVistaGradiente(1);

            btnSalvarTemplate.Background = WindowsVistaGradiente(1);
            btnEditarTemplate.Background = WindowsVistaGradiente(1);
            btnExcluirTemplate.Background = WindowsVistaGradiente(1);
                        
        }

        void baseGrafico_SalvaTemplateCompleted(object sender, GraficoSVC.SalvaTemplateCompletedEventArgs e)
        {
            
            throw new NotImplementedException();
        }

        void baseGrafico_SalvaConfiguracaoPadraoCompleted(object sender, GraficoSVC.SalvaConfiguracaoPadraoCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void baseGrafico_SalvaAreaTrabalhoCompleted(object sender, GraficoSVC.SalvaAreaTrabalhoCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void baseGrafico_RetornaTemplatePorIdCompleted(object sender, GraficoSVC.RetornaTemplatePorIdCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void baseGrafico_RetornaConfiguracaoPadraoCompleted(object sender, GraficoSVC.RetornaConfiguracaoPadraoCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void baseGrafico_RetornaAreaTrabalhoPorLoginCompleted(object sender, GraficoSVC.RetornaAreaTrabalhoPorLoginCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion Campos e Contrutores

        #region Eventos

        #region LayoutRoot
        /// <summary>
        /// Evento dispara quando a grid LayoutRoot (grid principal) e carregando a tela para adicionar um novo gráfico.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
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
        }
        #endregion LayoutRoot

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

        #endregion Eventos

        

        #region Eventos de Inteface

        #region Área de Trabalho

        #region Clique Titulo AT
        /// <summary>
        /// Clique no titulo da area de trabalho.
        /// </summary>
        private void bordaTituloAreaTrabalho_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (stpAreaTrabalho.Visibility == Visibility.Visible)
                stpAreaTrabalho.Visibility = Visibility.Collapsed;
            else
                stpAreaTrabalho.Visibility = Visibility.Visible;
        }
        #endregion Clique Titulo AT

        #region SalvarAT
        /// <summary>
        /// Salva área de trabalho.
        /// </summary>
        private void btnSalvarAT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TDGraficoSL.Telas.SalvaAreaTrabalho frmSalvar = new TDGraficoSL.Telas.SalvaAreaTrabalho();
                frmSalvar.Closing += (sender1, e1) =>
                {
                    if (frmSalvar.DialogResult == true)
                    {
                        TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO atDTO = GeraAreaTrabalhoDTO();

                        //Obter nome
                        atDTO.Nome = frmSalvar.Nome;

                        baseGrafico.SalvaAreaTrabalhoAsync(atDTO);
                    }
                };

                frmSalvar.Show();
            }
            catch
            {
                MessageBox.Show("Erro ao salvar a área de trabalho.");
            }
            finally
            {
                ModoCarregando(false);
            }
        }
        #endregion SalvarAT

        #region Aplica AT
        /// <summary>
        /// Aplica área de trabalho desejada.
        /// </summary>
        private void btnAplicarAT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            baseGrafico.RetornaAreaTrabalhoPorLoginAsync(loginUsuario, loginCorretora);
        }
        #endregion Aplica AT

        #region Editar AT
        /// <summary>
        /// Permite editar a area de trabalho.
        /// </summary>
        private void btnEditarAT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                 TDGraficoSL.Telas.SelecaoAreaTrabalho selecaoAreaTrabalho = new TDGraficoSL.Telas.SelecaoAreaTrabalho(areasTrabalhos);

                 selecaoAreaTrabalho.Closing += (sender1, e1) =>
                 {
                     if (selecaoAreaTrabalho.DialogResult == true)
                     {
                         TDGraficoSL.Telas.SalvaAreaTrabalho frmSalvar = new TDGraficoSL.Telas.SalvaAreaTrabalho();
                         frmSalvar.Closing += (sender2, e2) =>
                         {
                             if (frmSalvar.DialogResult == true)
                             {
                                 TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO atDTO = GeraAreaTrabalhoDTO();
                                 TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO atVelhoDTO = new TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO();

                                 //Obter nome
                                 atDTO.Nome = frmSalvar.Nome;

                                 foreach (TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO obj in areasTrabalhos)
                                 {
                                     if (obj.Id == selecaoAreaTrabalho.IdArea)
                                         atVelhoDTO = obj;
                                 }

                                 baseGrafico.EditarAreaTrabalhoAsync(atVelhoDTO, atDTO);
                             }
                         };

                         frmSalvar.Show();
                     }
                 };

                 selecaoAreaTrabalho.Show();
            }
            catch
            {
                MessageBox.Show("Erro ao salvar a área de trabalho.");
            }
        }
        #endregion Editar AT

        #region Excluir AT
        /// <summary>
        /// Exclui area de trabalho.
        /// </summary>
        private void btnExcluirAT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TDGraficoSL.Telas.SelecaoAreaTrabalho selecaoAreaTrabalho = new TDGraficoSL.Telas.SelecaoAreaTrabalho(areasTrabalhos);

                selecaoAreaTrabalho.Closing += (sender1, e1) =>
                {
                    if (selecaoAreaTrabalho.DialogResult == true)
                    {
                        TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO atDTO = new TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO();
                        foreach (TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO obj in areasTrabalhos)
                        {
                            if (obj.Id == selecaoAreaTrabalho.IdArea)
                                atDTO = obj;
                        }

                        baseGrafico.ExcluiAreaTrabalhoAsync(atDTO);
                    }
                };

                selecaoAreaTrabalho.Show();
            }
            catch
            {
                MessageBox.Show("Erro ao salvar a área de trabalho.");
            }
        }
        #endregion Excluir AT

        #endregion Área de Trabalho

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
            try
            {
                AplicaTemplate = true;
                baseGrafico.RetornaTemplateByLoginAsync(loginUsuario, loginCorretora, false);
            }
            catch
            {
                MessageBox.Show("Não foi possível aplicar o template.", "Aviso", MessageBoxButton.OK);
            }
        }
        #endregion Aplicar Template

        #region Salvar Template
        /// <summary>
        /// Salva o template do gráfico selecionado.
        /// </summary>
        private void btnSalvarTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TDGraficoSL.GraficoWS.TemplateDTO templateDTO = (TDGraficoSL.GraficoWS.TemplateDTO)graficoSelecionado.GeraTemplateDTO();
 
                //Atenção: Retirando objetos!!
                if (templateDTO != null)
                    templateDTO.Objetos = new List<TDGraficoSL.GraficoWS.ObjetoEstudoDTO>();

                TDGraficoSL.Telas.SalvaTemplate salvaTemplate = new TDGraficoSL.Telas.SalvaTemplate(templateDTO.Periodo, templateDTO.Periodicidade);
                salvaTemplate.Closing += (sender1, e1) =>
                {
                    if (salvaTemplate.DialogResult == true)
                    {
                        //Salvando o template com nome, periodo e periodicidade aplicado.
                        templateDTO.Nome = salvaTemplate.Nome;
                        templateDTO.Periodo = salvaTemplate.Periodo;
                        templateDTO.Periodicidade = salvaTemplate.Periodicidade;
                        templateDTO.LoginCliente = loginUsuario;
                        templateDTO.LoginCorretora = loginCorretora;

                        templateDTO.AreaTrabalho = false;

                        baseGrafico.SalvaTemplateAsync(templateDTO);
                    }
                };
                salvaTemplate.Show();
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
            try
            {
                AplicaTemplate = false;
                baseGrafico.RetornaTemplateByLoginAsync(loginUsuario, loginCorretora, false);
            }
            catch
            {
                MessageBox.Show("Não foi possível salvar seu template.", "Aviso", MessageBoxButton.OK);
            }
        }
        #endregion Exclui Template

        #region Edita Template
        /// <summary>
        /// Edita o template desejado.
        /// </summary>
        private void btnEditarTemplate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            baseGrafico.RetornaTemplateByLoginAsync(loginUsuario, loginCorretora, false);
        }
        #endregion Edita Template

        #endregion Template

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
            //Instanciando uma nova tela de graficoLoadDialog passando ativo vazio e periodicidade
            GraficoLoadDialog graficoLoadDialog;

            if (listaGraficos.Count >= 1)
                graficoLoadDialog = new GraficoLoadDialog("", 180, 1440, false, ativos);
            else
                graficoLoadDialog = new GraficoLoadDialog("", 180, 1440, true, ativos);

            graficoLoadDialog.Closing += (sender1, e1) =>
            {
                if (graficoLoadDialog.DialogResult == true)
                {
                    GraficoSL novoGrafico = new GraficoSL(ativos);
                    novoGrafico.Ativo = graficoLoadDialog.Ativo;
                    novoGrafico.Periodicidade = graficoLoadDialog.Periodicidade;
                    novoGrafico.Periodo = graficoLoadDialog.Periodo;

                    listaGraficos.Add(novoGrafico);
                    novoGrafico.AguardandoCarregamento = false;

                    listaGraficos[listaGraficos.Count - 1].PendenteConfigPadroes = true;

                    CriaEsquemaPainel(listaGraficos.Count - 1, true);
                }
            };

            graficoLoadDialog.Show();
        }
        #endregion Adiciona Grafico

        #endregion Seleção Gráficos

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

        #endregion Eventos de Inteface

        #region Métodos

        #region CriaEsquemaPainel()
        /// <summary>
        /// Cria bloco a bloco de selecao de listaGraficos.
        /// O parametro basea-se na posição do gráfico na lista.
        /// </summary>
        /// <param name="i"></param>
        private void CriaEsquemaPainel(int i, bool usaSelecao)
        {
            GraficoSL grafico = listaGraficos[i];

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
            imgBtn.Source = (ImageSource)(new ImageSourceConverter()).ConvertFrom("btnFechar.png");

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

                if (grafico.StockChart.InfoPanelPosicao == InfoPanelPosicaoEnum.Fixo)
                    grafico.StockChart.InfoPanelPosicao = InfoPanelPosicaoEnum.SeguindoMouse;

                stpSelecaoGraficos.Children.Remove(borda);
                pnlGraficoContainer.Children.Remove(grafico);
                listaGraficos.Remove(grafico);

                if ((listaGraficos.Count <= 0) && (!adicionandoGrafico))
                {
                    adicionandoGrafico = true;

                    //Instanciando uma nova tela de graficoLoadDialog passando ativo vazio e periodicidade
                    GraficoLoadDialog graficoLoadDialog;
                    graficoLoadDialog = new GraficoLoadDialog("", 180, 1440, true, ativos);

                    graficoLoadDialog.Closing += (sender1, e1) =>
                    {
                        if (graficoLoadDialog.DialogResult == true)
                        {
                            GraficoSL novoGrafico = new GraficoSL(ativos);
                            novoGrafico.Ativo = graficoLoadDialog.Ativo;
                            novoGrafico.Periodicidade = graficoLoadDialog.Periodicidade;
                            novoGrafico.Periodo = graficoLoadDialog.Periodo;

                            novoGrafico.ConfigPadrao = this.configPadraoDTO;
                            novoGrafico.PendenteConfigPadroes = true;

                            listaGraficos.Add(novoGrafico);
                            novoGrafico.AguardandoCarregamento = false;

                            CriaEsquemaPainel(listaGraficos.Count - 1, true);
                        }

                        adicionandoGrafico = false;
                    };

                    graficoLoadDialog.Show();
                }
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

                if (grafico.StockChart.InfoPanelPosicao == InfoPanelPosicaoEnum.Fixo)
                    grafico.StockChart.InfoPanelPosicao = InfoPanelPosicaoEnum.SeguindoMouse;

                stpSelecaoGraficos.Children.Remove(borda);
                pnlGraficoContainer.Children.Remove(grafico);
                listaGraficos.Remove(grafico);

                if ((listaGraficos.Count <= 0) && (!adicionandoGrafico))
                {
                    adicionandoGrafico = true;

                    //Instanciando uma nova tela de graficoLoadDialog passando ativo vazio e periodicidade
                    GraficoLoadDialog graficoLoadDialog;
                    graficoLoadDialog = new GraficoLoadDialog("", 180, 1440, true, ativos);

                    graficoLoadDialog.Closing += (sender1, e1) =>
                    {
                        if (graficoLoadDialog.DialogResult == true)
                        {
                            GraficoSL novoGrafico = new GraficoSL(ativos);
                            novoGrafico.Ativo = graficoLoadDialog.Ativo;
                            novoGrafico.Periodicidade = graficoLoadDialog.Periodicidade;
                            novoGrafico.Periodo = graficoLoadDialog.Periodo;

                            novoGrafico.ConfigPadrao = this.configPadraoDTO;
                            novoGrafico.PendenteConfigPadroes = true;

                            listaGraficos.Add(novoGrafico);
                            novoGrafico.AguardandoCarregamento = false;

                            CriaEsquemaPainel(listaGraficos.Count - 1, true);
                        }

                        adicionandoGrafico = false;
                    };

                    graficoLoadDialog.Show();
                }
            };

            #endregion Eventos de Fechamento de Grafico

            painelClose.Children.Add(lbl);
            painelClose.Children.Add(btnFechar);

            borda.Child = painelClose;
            stpSelecaoGraficos.Children.Insert(stpSelecaoGraficos.Children.Count - 1, borda);

            listaGraficos[i].ConfigPadrao = configPadraoDTO;

            //Criando painel do gráfico
            pnlGraficoContainer.Children.Add(listaGraficos[i]);
            listaGraficos[i].Margin = new Thickness(0);

            //Seleciona o último gráfico e dispara o AtualizaChart()
            if (i == listaGraficos.Count - 1)
                listaGraficos[i].AtualizaChart(listaGraficos[i].Ativo, DateTime.Today.Subtract(new TimeSpan(listaGraficos[i].Periodo, 0, 0, 0, 0)), DateTime.Now, listaGraficos[i].Ajuste, guidUsuario, listaGraficos[i].Periodicidade, true);
            else
                listaGraficos[i].AguardandoCarregamento = true;

            lbl.Text = ObtemTituloAba(listaGraficos[i].Ativo, listaGraficos[i].Periodicidade, listaGraficos[i].Periodo);

            Canvas.SetZIndex(listaGraficos[i], 1);

            //Setando o zindex do gráfico anterior como 0
            if (listaGraficos.Count - 1 > 0)
                Canvas.SetZIndex(listaGraficos[listaGraficos.Count - 1], 0);

            listaGraficos[i].OnGraficoChange += (sender) => lbl.Text = ObtemTituloAba(((GraficoSL)sender).Ativo, ((GraficoSL)sender).Periodicidade, ((GraficoSL)sender).Periodo);


            //Criando evento de clique na borda
            borda.MouseLeftButtonDown += (sender, e) =>
            {
                //Colocando os gráficos atras do grafico selecionado
                foreach (GraficoSL obj in listaGraficos)
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

                if (graficoSelecionado.AguardandoCarregamento)
                {
                    graficoSelecionado.AtualizaChart(graficoSelecionado.Ativo, DateTime.Today.Subtract(new TimeSpan(graficoSelecionado.Periodo, 0, 0, 0, 0)), DateTime.Now, graficoSelecionado.Ajuste, "AGORA", graficoSelecionado.Periodicidade, true);
                    graficoSelecionado.AguardandoCarregamento = false;
                }
            };

            graficoSelecionado = grafico;

            if (usaSelecao)
                SelecionaGrafico(borda, grafico);
        }
        #endregion CriaEsquemaPainel()

        #region CriaPainelControleGraficos()
        /// <summary>
        /// Cria paineis de selecao de gráficos baseada na lista "listaGraficos".
        /// </summary>
        private void CriaPainelControleGraficos()
        {
            for (int i = 0; i < listaGraficos.Count; i++)
            {
                if (i == listaGraficos.Count - 1)
                    CriaEsquemaPainel(i, true);
                else
                    CriaEsquemaPainel(i, false);
            }
        }
        #endregion CriaPainelControleGraficos()

        #region GeraAreaTrabalhoDTO()
        /// <summary>
        /// Gera template da área de trabalho.
        /// </summary>
        /// <returns></returns>
        private TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO GeraAreaTrabalhoDTO()
        {
            TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO areaTrabalhoDTO = new TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO();

            areaTrabalhoDTO.LoginCliente = loginUsuario;
            areaTrabalhoDTO.LoginCorretora = loginCorretora;
            areaTrabalhoDTO.ListaGrafico = RetornaListaGraficoDTO(areaTrabalhoDTO);

            return areaTrabalhoDTO;
        }
        #endregion GeraAreaTrabalhoDTO()

        #region ObtemTituloAba()
        /// <summary>
        /// Método seta o título da aba com o Ativo e periodicidade
        /// </summary>
        /// <param name="ativo"></param>
        /// <param name="periodicidade"></param>
        /// <returns></returns>
        private string ObtemTituloAba(string ativo, int periodicidade)
        {
            // Sabendo o ativo selecionado, o periodo e periodicidade, posso dar nome a tab que criei
            switch (periodicidade)
            {
                case 1:
                case 5:
                case 10:
                case 15:
                case 30:
                case 60:
                    return ativo.ToUpper() + "(I" + periodicidade.ToString() + ")";
                case 1440:
                    return ativo.ToUpper() + "(D)";
                case 10080:
                    return ativo.ToUpper() + "(S)";
                case 43200:
                    return ativo.ToUpper() + "(M)";
                default:
                    return "";
            }
        }
        #endregion ObtemTituloAba()

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

        #region RetornaListaGraficoDTO()
        /// <summary>
        /// Retorna uma lista de gráficos da area de trabalho atual.
        /// </summary>
        /// <param name="areaTrabalhoDTO"></param>
        /// <returns></returns>
        private List<TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO> RetornaListaGraficoDTO(TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO areaTrabalhoDTO)
        {
            List<TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO> listaGrafico = new List<TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO>();

            int index = 0;
            foreach (GraficoSL obj in listaGraficos)
            {
                TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO grafico = new TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO();
                grafico.Ativo = obj.Ativo;

                //Atenção: só devo gerar NOVOS template dos listaGraficos que NÃO estão aguardando carregamento
                if (obj.AguardandoCarregamento)
                {
                    //Localizando o template antigo deste grafico
                    TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO antigaAT = ObtemAntigaAreaTrabalho();

                    if (antigaAT != null)
                    {
                        //Obtendo clone da area de trabalho antiga
                        grafico = ClonaGrafico(antigaAT.ListaGrafico[index]);

                        //Zerando Ids
                        grafico.Id = 0;
                        grafico.TemplateDTO.Id = 0;

                        foreach (TDGraficoSL.GraficoWS.IndicadorDTO ind in grafico.TemplateDTO.Indicadores)
                        {
                            ind.Id = 0;
                        }

                        foreach (TDGraficoSL.GraficoWS.ObjetoEstudoDTO objeto in grafico.TemplateDTO.Objetos)
                        {
                            objeto.Id = 0;
                        }

                    }
                    else
                        continue;
                }
                else
                    grafico.TemplateDTO = obj.GeraTemplateDTO();

                listaGrafico.Add(grafico);

                index++;
            }

            return listaGrafico;
        }
        #endregion RetornaListaGraficoDTO()

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

        #region LimpaAreaTrabalho
        /// <summary>
        /// Limpa a área de trabalho.
        /// </summary>
        private void LimpaAreaTrabalho()
        {
            try
            {
                int count = stpSelecaoGraficos.Children.Count;
                int aux = 0;
                for (int i = 0; i < count; i++)
                {
                    if ((stpSelecaoGraficos.Children[aux] is Border) && (((Border)stpSelecaoGraficos.Children[aux]).Name == "tabAdicionaGrafico"))
                    {
                        aux++;
                        continue; 
                    }

                    stpSelecaoGraficos.Children.Remove((UIElement)stpSelecaoGraficos.Children[aux]);
                }

                pnlGraficoContainer.Children.Clear();
                listaGraficos.Clear();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ocorreu um erro ao tentar limpar a área  de trabalho.");
            }
        }
        #endregion LimpaAreaTrabalho

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

            if (grafico.AguardandoCarregamento)
            {
                grafico.AtualizaChart(grafico.Ativo, DateTime.Today.Subtract(new TimeSpan(grafico.Periodo, 0, 0, 0, 0)), DateTime.Now, grafico.Ajuste, "AGORA", grafico.Periodicidade, true);
                grafico.AguardandoCarregamento = false;
            }
        }
        #endregion SelecionaGrafico

        #region ModoCarregando()
        /// <summary>
        /// Abre tela de espera/carregamento do gráfico.
        /// </summary>
        /// <param name="ativar">Ativa ou desativa a tela.</param>
        public void ModoCarregando(bool ativar)
        {
            try
            {
                if (ativar)
                {
                    this.IsEnabled = false;
                    expander.IsEnabled = false;
                    this.Opacity = 0.8;
                    pnlCarregando.Visibility = Visibility.Visible;
                }
                else
                {
                    this.IsEnabled = true;
                    expander.IsEnabled = true;
                    this.Opacity = 1;
                    pnlCarregando.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion ModoCarregando()

        #endregion Métodos

        private void btnSalvarATAux_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ModoCarregando(true);

                TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO areaAntiga = null;
                TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO area = GeraAreaTrabalhoDTO();

                foreach (TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO obj in areasTrabalhos)
                {
                    if (obj.Nome.Contains("TDSGFSL-"))
                    {
                        areaAntiga = obj;
                    }
                }

                area.Nome = "TDSGFSL-" + loginUsuario;

                int count = 0;
                foreach (TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO grafico in area.ListaGrafico)
                {
                    grafico.TemplateDTO.Nome = "TDSGFSL-" + count.ToString() + "-" + loginUsuario;

                 
                    count++;
                }

                if (areaAntiga == null)
                {
                    baseGrafico.SalvaAreaTrabalhoAsync(area);

                    areasTrabalhos.Add(area);
                }
                else
                {
                    baseGrafico.EditarAreaTrabalhoAsync(areaAntiga, area);

                    areaAntiga = area;
                }
            }
            catch (Exception exc)
            {
                ModoCarregando(false);
                MessageBox.Show("Erro ao tentar salvar a Área de Trabalho.");
            }
        }

        private void baseGrafico_ExcluiTemplateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show("Template excluído com sucesso.", "Aviso", MessageBoxButton.OK);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void baseGrafico_RetornaObjetosIndicadoresByTemplateCompleted(object sender, TDGraficoSL.GraficoWS.RetornaObjetosIndicadoresByTemplateCompletedEventArgs e)
        {
            try
            {
                if (RetornaTemplate)
                {
                    TDGraficoSL.GraficoWS.TemplateDTO templateDTO = e.Result;
                    graficoSelecionado.AplicaTemplate(templateDTO, false, true, true, true);
                }
                else
                {
                    if (templateDTOExclusao != null)
                    {
                        templateDTOExclusao = e.Result;
                        baseGrafico.ExcluiTemplateAsync(templateDTOExclusao);
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void baseGrafico_RetornaTemplateByLoginCompleted(object sender, TDGraficoSL.GraficoWS.RetornaTemplateByLoginCompletedEventArgs e)
        {
            try
            {
                List<TDGraficoSL.GraficoWS.TemplateDTO> listTemplateDTO = e.Result;
                if (listTemplateDTO.Count > 0)
                {
                    if (AplicaTemplate)
                    {
                        TDGraficoSL.Telas.SelecaoTemplate selecaoTemplate = new TDGraficoSL.Telas.SelecaoTemplate(listTemplateDTO, "Selecione um template");
                        selecaoTemplate.Closing += (sender1, e1) =>
                        {
                            if (selecaoTemplate.DialogResult == true)
                            {
                                RetornaTemplate = true;
                                //selecaoTemplate.TemplateDTO.Periodo = selecaoTemplate.Periodo;
                                //selecaoTemplate.TemplateDTO.Periodicidade = selecaoTemplate.Periodicidade;
                                baseGrafico.RetornaObjetosIndicadoresByTemplateAsync(selecaoTemplate.TemplateDTO);
                            }
                        };
                        selecaoTemplate.Show();
                    }
                    else
                    {
                        TDGraficoSL.Telas.SelecaoTemplate selecaoTemplate = new TDGraficoSL.Telas.SelecaoTemplate(listTemplateDTO, "Selecione um template");
                        selecaoTemplate.Closing += (sender1, e1) =>
                        {
                            if (selecaoTemplate.DialogResult == true)
                            {
                                RetornaTemplate = false;
                                templateDTOExclusao = selecaoTemplate.TemplateDTO;
                                baseGrafico.RetornaObjetosIndicadoresByTemplateAsync(templateDTOExclusao);
                            }
                        };
                        selecaoTemplate.Show();
                    }
                }
                else
                    MessageBox.Show("Não há template para executar esta ação.", "Aviso", MessageBoxButton.OK);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Evento de resposta contendo ativos solicitados a classe DadosLIB.
        /// </summary>
        /// <param name="ativos"></param>
        private void classeDados_OnObtemAtivosAyncComplete(List<string> ativos)
        {
            //Populando ativos!
            this.ativos = ativos;

            baseGrafico.RetornaConfiguracaoPadroesAsync(loginUsuario);

            //Usar a linha abaixo para trazedr as areas de trabalho de um cliente
            if (suporteAMultiplasATs)
                baseGrafico.RetornaAreaTrabalhoPorLoginAsync(loginUsuario, loginCorretora);
            else
                baseGrafico.RetornaUltimaAreaTrabalhoAsync(loginUsuario);
        }


        private void baseGrafico_RetornaConfiguracaoPadroesCompleted(object sender, TDGraficoSL.GraficoWS.RetornaConfiguracaoPadroesCompletedEventArgs e)
        {
            configPadraoDTO = new TDGraficoSL.GraficoWS.ConfiguracaoPadraoDTO();

            if (e.Result != null)
            {
                configPadraoDTO.CorBordaCandleAlta = e.Result.CorBordaCandleAlta;
                configPadraoDTO.CorBordaCandleBaixa = e.Result.CorBordaCandleBaixa;
                configPadraoDTO.CorCandleAlta = e.Result.CorCandleAlta;
                configPadraoDTO.CorCandleBaixa = e.Result.CorCandleBaixa;
                configPadraoDTO.CorFundo = e.Result.CorFundo;
                configPadraoDTO.CorObjeto = e.Result.CorObjeto;
                configPadraoDTO.EspacoADireitaGrafico = e.Result.EspacoADireitaGrafico;
                configPadraoDTO.EstiloBarra = e.Result.EstiloBarra;
                configPadraoDTO.EstiloPreco = e.Result.EstiloPreco;
                configPadraoDTO.EstiloPrecoParam1 = e.Result.EstiloPrecoParam1;
                configPadraoDTO.EstiloPrecoParam2 = e.Result.EstiloPrecoParam2;
                configPadraoDTO.GradeHorizontal = e.Result.GradeHorizontal;
                configPadraoDTO.GradeVertical = e.Result.GradeVertical;
                configPadraoDTO.GrossuraObjeto = e.Result.GrossuraObjeto;
                configPadraoDTO.Id = e.Result.Id;
                configPadraoDTO.LinhaMagnetica = e.Result.LinhaMagnetica;
                configPadraoDTO.LinhaTendenciaInfinita = e.Result.LinhaTendenciaInfinita;
                configPadraoDTO.LoginCliente = e.Result.LoginCliente;
                configPadraoDTO.PainelInfo = e.Result.PainelInfo;
                configPadraoDTO.PosicaoEscala = e.Result.PosicaoEscala;
                configPadraoDTO.PrecisaoEscala = e.Result.PrecisaoEscala;
                configPadraoDTO.TipoEscala = e.Result.TipoEscala;
                configPadraoDTO.TipoLinhaObjeto = e.Result.TipoLinhaObjeto;
                configPadraoDTO.UsarCoresAltaBaixaVolume = e.Result.UsarCoresAltaBaixaVolume;

                configPadraoDTO.CorIndicador = e.Result.CorIndicador;
                configPadraoDTO.CorIndicadorAux1 = e.Result.CorIndicadorAux1;
                configPadraoDTO.CorIndicadorAux2 = e.Result.CorIndicadorAux2;

                configPadraoDTO.TipoLinhaIndicador = (int)e.Result.TipoLinhaIndicador;
                configPadraoDTO.TipoLinhaIndicadorAux1 = (int)e.Result.TipoLinhaIndicadorAux1;
                configPadraoDTO.TipoLinhaIndicadorAux2 = (int)e.Result.TipoLinhaIndicadorAux2;

                configPadraoDTO.ConfigFiboRetracements = e.Result.ConfigFiboRetracements;
            }
            //Se for nula, devo popular o DTO com dados padroes
            else
            {
                configPadraoDTO.CorBordaCandleAlta = Colors.Transparent.A.ToString() + ";" + Colors.Transparent.R.ToString() + ";" + Colors.Transparent.G.ToString() + ";" + Colors.Transparent.B.ToString();
                configPadraoDTO.CorBordaCandleBaixa = Colors.Transparent.A.ToString() + ";" + Colors.Transparent.R.ToString() + ";" + Colors.Transparent.G.ToString() + ";" + Colors.Transparent.B.ToString();
                configPadraoDTO.CorCandleAlta = Colors.Green.A.ToString() + ";" + Colors.Green.R.ToString() + ";" + Colors.Green.G.ToString() + ";" + Colors.Green.B.ToString();
                configPadraoDTO.CorCandleBaixa = Colors.Red.A.ToString() + ";" + Colors.Red.R.ToString() + ";" + Colors.Red.G.ToString() + ";" + Colors.Red.B.ToString();
                configPadraoDTO.CorFundo = Colors.Black.A.ToString() + ";" + Colors.Black.R.ToString() + ";" + Colors.Black.G.ToString() + ";" + Colors.Black.B.ToString();
                configPadraoDTO.CorObjeto = Colors.White.A.ToString() + ";" + Colors.White.R.ToString() + ";" + Colors.White.G.ToString() + ";" + Colors.White.B.ToString();
                configPadraoDTO.EspacoADireitaGrafico = 50;
                configPadraoDTO.EstiloBarra = (int)TipoSeriesEnum.Candle;
                configPadraoDTO.EstiloPreco = (int)EstiloPrecoEnum.Padrao;
                configPadraoDTO.EstiloPrecoParam1 = 0;
                configPadraoDTO.EstiloPrecoParam2 = 0;
                configPadraoDTO.GradeHorizontal = false;
                configPadraoDTO.GradeVertical = true;
                configPadraoDTO.GrossuraObjeto = 1;
                configPadraoDTO.Id = 0;
                configPadraoDTO.LinhaMagnetica = false;
                configPadraoDTO.LinhaTendenciaInfinita = false;
                configPadraoDTO.LoginCliente= loginUsuario;
                configPadraoDTO.PainelInfo = false;
                configPadraoDTO.PosicaoEscala = (int)TipoAlinhamentoEscalaEnum.Direita;
                configPadraoDTO.PrecisaoEscala = 2;
                configPadraoDTO.TipoEscala = (int)TipoEscala.Linear;
                configPadraoDTO.TipoLinhaObjeto = (int)TipoLinha.Solido;
                configPadraoDTO.UsarCoresAltaBaixaVolume = false;

                configPadraoDTO.CorIndicador = Colors.Blue.A.ToString() + ";" + Colors.Blue.R.ToString() + ";" + Colors.Blue.G.ToString() + ";" + Colors.Blue.B.ToString();
                configPadraoDTO.CorIndicadorAux1 = Colors.Red.A.ToString() + ";" + Colors.Red.R.ToString() + ";" + Colors.Red.G.ToString() + ";" + Colors.Red.B.ToString();
                configPadraoDTO.CorIndicadorAux2 = Colors.Yellow.A.ToString() + ";" + Colors.Yellow.R.ToString() + ";" + Colors.Yellow.G.ToString() + ";" + Colors.Yellow.B.ToString();

                configPadraoDTO.TipoLinhaIndicador = (int)TipoLinha.Solido;
                configPadraoDTO.TipoLinhaIndicadorAux1 = (int)TipoLinha.Solido;
                configPadraoDTO.TipoLinhaIndicadorAux2 = (int)TipoLinha.Solido;

                configPadraoDTO.ConfigFiboRetracements = "161.8;99.99;61.8;38.89;23.6;14.59;9.01";
            }

            foreach (GraficoSL graf in listaGraficos)
            {
                graf.ConfigPadrao = configPadraoDTO;
            }
        }


        private void btnConfigPadrao_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (configPadraoDTO != null)
            {
                ConfiguracoesPadroes configuracoesPadroes = new ConfiguracoesPadroes(GraficoSL.GeraBrushPeloARGB(configPadraoDTO.CorFundo), GraficoSL.GeraCorPeloARGB(configPadraoDTO.CorBordaCandleAlta), GraficoSL.GeraCorPeloARGB(configPadraoDTO.CorBordaCandleBaixa),
                                                            GraficoSL.GeraCorPeloARGB(configPadraoDTO.CorCandleAlta), GraficoSL.GeraCorPeloARGB(configPadraoDTO.CorCandleBaixa), (TipoEscala)configPadraoDTO.TipoEscala, configPadraoDTO.PrecisaoEscala, (TipoAlinhamentoEscalaEnum)configPadraoDTO.PosicaoEscala,
                                                            configPadraoDTO.GradeHorizontal, configPadraoDTO.GradeVertical, configPadraoDTO.PainelInfo, (EstiloPrecoEnum)configPadraoDTO.EstiloPreco, configPadraoDTO.EstiloPrecoParam1, configPadraoDTO.EstiloPrecoParam2, (TipoSeriesEnum)configPadraoDTO.EstiloBarra,
                                                            configPadraoDTO.EspacoADireitaGrafico, configPadraoDTO.UsarCoresAltaBaixaVolume, GraficoSL.GeraCorPeloARGB(configPadraoDTO.CorObjeto), configPadraoDTO.GrossuraObjeto, configPadraoDTO.LinhaMagnetica, configPadraoDTO.LinhaMagnetica, (TipoLinha)configPadraoDTO.TipoLinhaObjeto, configPadraoDTO.ConfigFiboRetracements,
                                                            GraficoSL.GeraCorPeloARGB(configPadraoDTO.CorIndicador), (TipoLinha)configPadraoDTO.TipoLinhaIndicador, GraficoSL.GeraCorPeloARGB(configPadraoDTO.CorIndicadorAux1), (TipoLinha)configPadraoDTO.TipoLinhaIndicadorAux1, GraficoSL.GeraCorPeloARGB(configPadraoDTO.CorIndicadorAux2), (TipoLinha)configPadraoDTO.TipoLinhaIndicadorAux2);

                configuracoesPadroes.Closing += (sender1, e1) =>
                    {
                        if (configuracoesPadroes.DialogResult == true)
                        {
                            configPadraoDTO.CorBordaCandleAlta = configuracoesPadroes.CorBordaCandleAlta.A.ToString() + ";" + configuracoesPadroes.CorBordaCandleAlta.R.ToString() + ";" + configuracoesPadroes.CorBordaCandleAlta.G.ToString() + ";" + configuracoesPadroes.CorBordaCandleAlta.B.ToString();
                            configPadraoDTO.CorBordaCandleBaixa = configuracoesPadroes.CorBordaCandleBaixa.A.ToString() + ";" + configuracoesPadroes.CorBordaCandleBaixa.R.ToString() + ";" + configuracoesPadroes.CorBordaCandleBaixa.G.ToString() + ";" + configuracoesPadroes.CorBordaCandleBaixa.B.ToString();
                            configPadraoDTO.CorCandleAlta = configuracoesPadroes.CorCandleAlta.A.ToString() + ";" + configuracoesPadroes.CorCandleAlta.R.ToString() + ";" + configuracoesPadroes.CorCandleAlta.G.ToString() + ";" + configuracoesPadroes.CorCandleAlta.B.ToString();
                            configPadraoDTO.CorCandleBaixa = configuracoesPadroes.CorCandleBaixa.A.ToString() + ";" + configuracoesPadroes.CorCandleBaixa.R.ToString() + ";" + configuracoesPadroes.CorCandleBaixa.G.ToString() + ";" + configuracoesPadroes.CorCandleBaixa.B.ToString();

                            SolidColorBrush corFundo = (SolidColorBrush)configuracoesPadroes.CorFundo;
                            configPadraoDTO.CorFundo = corFundo.Color.A.ToString() + ";" + corFundo.Color.R.ToString() + ";" + corFundo.Color.G.ToString() + ";" + corFundo.Color.B.ToString();
                            configPadraoDTO.CorObjeto = configuracoesPadroes.CorObjeto.A.ToString() + ";" + configuracoesPadroes.CorObjeto.R.ToString() + ";" + configuracoesPadroes.CorObjeto.G.ToString() + ";" + configuracoesPadroes.CorObjeto.B.ToString();
                            configPadraoDTO.EspacoADireitaGrafico = configuracoesPadroes.EspacoADireitaGrafico;
                            configPadraoDTO.EstiloBarra = (int)configuracoesPadroes.EstiloBarra;
                            configPadraoDTO.EstiloPreco = (int)configuracoesPadroes.EstiloPreco;
                            configPadraoDTO.EstiloPrecoParam1 = configuracoesPadroes.EstiloPrecoParam1;
                            configPadraoDTO.EstiloPrecoParam2 = configuracoesPadroes.EstiloPrecoParam2;
                            configPadraoDTO.GradeHorizontal = configuracoesPadroes.GradeHorizontal;
                            configPadraoDTO.GradeVertical = configuracoesPadroes.GradeVertical;
                            configPadraoDTO.GrossuraObjeto = configuracoesPadroes.GrossuraObjeto;
                            configPadraoDTO.LinhaMagnetica = configuracoesPadroes.LinhaMagnetica;
                            configPadraoDTO.LinhaTendenciaInfinita = configuracoesPadroes.LinhaTendenciaInfinita;
                            configPadraoDTO.LoginCliente = loginUsuario;
                            configPadraoDTO.PainelInfo = configuracoesPadroes.PainelInfo;
                            configPadraoDTO.PosicaoEscala = (int)configuracoesPadroes.PosicaoEscala;
                            configPadraoDTO.PrecisaoEscala = configuracoesPadroes.PrecisaoEscala;
                            configPadraoDTO.TipoEscala = (int)configuracoesPadroes.TipoEscala;
                            configPadraoDTO.TipoLinhaObjeto = (int)configuracoesPadroes.TipoLinhaObjeto;
                            configPadraoDTO.UsarCoresAltaBaixaVolume = configuracoesPadroes.UsarCoresAltaBaixaVolume;

                            configPadraoDTO.CorIndicador = configuracoesPadroes.CorIndicador.A.ToString() + ";" + configuracoesPadroes.CorIndicador.R.ToString() + ";" + configuracoesPadroes.CorIndicador.G.ToString() + ";" + configuracoesPadroes.CorIndicador.B.ToString();
                            configPadraoDTO.CorIndicadorAux1 = configuracoesPadroes.CorIndicadorAux1.A.ToString() + ";" + configuracoesPadroes.CorIndicadorAux1.R.ToString() + ";" + configuracoesPadroes.CorIndicadorAux1.G.ToString() + ";" + configuracoesPadroes.CorIndicadorAux1.B.ToString();
                            configPadraoDTO.CorIndicadorAux2 = configuracoesPadroes.CorIndicadorAux2.A.ToString() + ";" + configuracoesPadroes.CorIndicadorAux2.R.ToString() + ";" + configuracoesPadroes.CorIndicadorAux2.G.ToString() + ";" + configuracoesPadroes.CorIndicadorAux2.B.ToString();

                            configPadraoDTO.TipoLinhaIndicador = (int)configuracoesPadroes.TipoLinhaIndicador;
                            configPadraoDTO.TipoLinhaIndicadorAux1 = (int)configuracoesPadroes.TipoLinhaIndicadorAux1;
                            configPadraoDTO.TipoLinhaIndicadorAux2 = (int)configuracoesPadroes.TipoLinhaIndicadorAux2;

                            configPadraoDTO.ConfigFiboRetracements = configuracoesPadroes.ConfigRetracements;

                            baseGrafico.SalvaConfiguracaoPadroesAsync(configPadraoDTO);
                        }
                    };

                configuracoesPadroes.Show();
            }
        }

        private void baseGrafico_SalvaConfiguracaoPadroesCompleted(object sender, TDGraficoSL.GraficoWS.SalvaConfiguracaoPadroesCompletedEventArgs e)
        {
            if (e.Result != null)
                configPadraoDTO.Id = e.Result;
        }

        private TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO ObtemAntigaAreaTrabalho()
        {
            foreach (TDGraficoSL.GraficoWS.GraficoAreaTrabalhoDTO obj in areasTrabalhos)
            {
                if (obj.Nome.Contains("TDSGFSL-"))
                    return obj;
            }

            return null;
        }

        private TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO ClonaGrafico(TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO graficoAClonar)
        {
            TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO grafico = new TDGraficoSL.GraficoWS.GraficoAreaTrabalhoGraficoDTO();
            TDGraficoSL.GraficoWS.TemplateDTO template = new TDGraficoSL.GraficoWS.TemplateDTO();
            template.Objetos = new List<TDGraficoSL.GraficoWS.ObjetoEstudoDTO>();
            template.Indicadores = new List<TDGraficoSL.GraficoWS.IndicadorDTO>();

            grafico.Ativo = graficoAClonar.Ativo;
            grafico.Id = graficoAClonar.Id;
            grafico.IdArea = graficoAClonar.IdArea;
            grafico.IdTemplate = graficoAClonar.IdTemplate;
            grafico.ManterGrafico = graficoAClonar.ManterGrafico;
            grafico.TemplateDTO = template;

            template.AreaTrabalho = graficoAClonar.TemplateDTO.AreaTrabalho;
            template.Ativo = graficoAClonar.TemplateDTO.Ativo;
            template.CorBordaCandleAlta = graficoAClonar.TemplateDTO.CorBordaCandleAlta;
            template.CorBordaCandleBaixa = graficoAClonar.TemplateDTO.CorBordaCandleBaixa;
            template.CorCandleAlta = graficoAClonar.TemplateDTO.CorCandleAlta;
            template.CorCandleBaixa = graficoAClonar.TemplateDTO.CorCandleBaixa;
            template.CorFundo = graficoAClonar.TemplateDTO.CorFundo;
            template.CorPadraoObjeto = graficoAClonar.TemplateDTO.CorPadraoObjeto;
            template.CorVolume = graficoAClonar.TemplateDTO.CorVolume;
            template.DarvaBox = graficoAClonar.TemplateDTO.DarvaBox;
            template.DarvaStopPercent = graficoAClonar.TemplateDTO.DarvaStopPercent;
            template.EscalaDireita = graficoAClonar.TemplateDTO.EscalaDireita;
            template.EscalaNormal = graficoAClonar.TemplateDTO.EscalaNormal;
            template.EspacoDireita = graficoAClonar.TemplateDTO.EspacoDireita;
            template.FixarPainelInfo = graficoAClonar.TemplateDTO.FixarPainelInfo;
            template.GradeHorizontal = graficoAClonar.TemplateDTO.GradeHorizontal;
            template.GradeVertical = graficoAClonar.TemplateDTO.GradeVertical;
            template.Id = graficoAClonar.TemplateDTO.Id;
            template.LoginCliente = graficoAClonar.TemplateDTO.LoginCliente;
            template.LoginCorretora = graficoAClonar.TemplateDTO.LoginCorretora;
            template.Nome = graficoAClonar.TemplateDTO.Nome;
            template.Periodicidade = graficoAClonar.TemplateDTO.Periodicidade;
            template.Periodo = graficoAClonar.TemplateDTO.Periodo;
            template.PrecisaoEscala = graficoAClonar.TemplateDTO.PrecisaoEscala;
            template.QtePainel = graficoAClonar.TemplateDTO.QtePainel;
            template.TamanhoPaineis = graficoAClonar.TemplateDTO.TamanhoPaineis;
            template.TipoBarra = graficoAClonar.TemplateDTO.TipoBarra;
            template.TipoPreco = graficoAClonar.TemplateDTO.TipoPreco;
            template.TipoPrecoParam1 = graficoAClonar.TemplateDTO.TipoPrecoParam1;
            template.TipoPrecoParam2 = graficoAClonar.TemplateDTO.TipoPrecoParam2;

            foreach (TDGraficoSL.GraficoWS.IndicadorDTO indicador in graficoAClonar.TemplateDTO.Indicadores)
            {
                TDGraficoSL.GraficoWS.IndicadorDTO obj = new TDGraficoSL.GraficoWS.IndicadorDTO();

                obj.ConfigSerieFilha1 = indicador.ConfigSerieFilha1;
                obj.ConfigSerieFilha2 = indicador.ConfigSerieFilha2;
                obj.CorAlta = indicador.CorAlta;
                obj.CorBaixa = indicador.CorBaixa;
                obj.Espessura = indicador.Espessura;
                obj.Id = indicador.Id;
                obj.IdTemplate = indicador.IdTemplate;
                obj.IndexPainel = indicador.IndexPainel;
                obj.Parametros = indicador.Parametros;
                obj.TipoIndicador = indicador.TipoIndicador;
                obj.TipoLinha = indicador.TipoLinha;

                template.Indicadores.Add(obj);
            }

            foreach (TDGraficoSL.GraficoWS.ObjetoEstudoDTO objeto in graficoAClonar.TemplateDTO.Objetos)
            {
                TDGraficoSL.GraficoWS.ObjetoEstudoDTO obj = new TDGraficoSL.GraficoWS.ObjetoEstudoDTO();

                obj.CorObjeto = objeto.CorObjeto;
                obj.DataFinal = objeto.DataFinal;
                obj.DataInicial = objeto.DataInicial;
                obj.Espessura = objeto.Espessura;
                obj.Id = objeto.Id;
                obj.IdTemplate = objeto.IdTemplate;
                obj.IndexPainel = objeto.IndexPainel;
                obj.InfinitaADireita = objeto.InfinitaADireita;
                obj.Linha1 = objeto.Linha1;
                obj.Linha2 = objeto.Linha2;
                obj.Linha3 = objeto.Linha3;
                obj.Linha4 = objeto.Linha4;
                obj.Linha5 = objeto.Linha5;
                obj.Linha6 = objeto.Linha6;
                obj.Linha7 = objeto.Linha7;
                obj.Magnetica = objeto.Magnetica;
                obj.Parametros = objeto.Parametros;
                obj.RecordFinal = objeto.RecordFinal;
                obj.RecordInicial = objeto.RecordInicial;
                obj.TamanhoTexto = objeto.TamanhoTexto;
                obj.Texto = obj.Texto;
                obj.TipoLinha = objeto.TipoLinha;
                obj.TipoObjeto = objeto.TipoObjeto;
                obj.ValorErrorChannel = objeto.ValorErrorChannel;
                obj.ValorFinal = objeto.ValorFinal;
                obj.ValorInicial = objeto.ValorInicial;
                obj.X1 = objeto.X1;
                obj.X2 = objeto.X2;
                obj.Y1 = objeto.Y1;
                obj.Y2 = objeto.Y2;

                template.Objetos.Add(obj);
            }

            return grafico;
        }

        

        //private void ExpandedOrCollapsed(object sender, RoutedEventArgs e)
        //{
        //    ExpandedOrCollapsed(sender as Expander);
        //}
        //private void ExpandedOrCollapsed(Expander expander)
        //{
        //    var colIndex = Grid.GetColumn(expander);
        //    var col = expanderGrid.ColumnDefinitions[colIndex];
        //    if (expander.IsExpanded)
        //    {
        //        col.Width = starHeight[colIndex];
        //        col.MinWidth = 50;
        //    }
        //    else
        //    {
        //        starHeight[colIndex] = col.Width;
        //        col.Width = GridLength.Auto;
        //        col.MinWidth = 0;
        //    }
        //    var bothExpanded = expander.IsExpanded;
        //    splitter.Visibility = bothExpanded ? Visibility.Visible : Visibility.Collapsed;
        //}


    }
}  