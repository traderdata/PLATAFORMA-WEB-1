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
using Traderdata.Client.TerminalWEB.DTO;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class AlertaAnalises : ChildWindow
    {
        TerminalWebSVC.TerminalWebClient baseFreeStockChartPlus;
        private List<AtivoLocalDTO> listaAtivos = new List<AtivoLocalDTO>();
        //private List<AtivoLocalDTO> listaAux = new List<AtivoLocalDTO>();
        private List<AtivoLocalDTO> listaAux;
        private List<AtivoLocalDTO> listaAtivosSelecionados = new List<AtivoLocalDTO>();

        private List<TerminalWebSVC.UsuarioDTO> listaPublicadores = new List<TerminalWebSVC.UsuarioDTO>();
        private List<TerminalWebSVC.UsuarioDTO> listaPublicadoresSelecionados = new List<TerminalWebSVC.UsuarioDTO>();

        private List<TerminalWebSVC.InteresseAnaliseDTO> listaInteresse = new List<TerminalWebSVC.InteresseAnaliseDTO>();

        public AlertaAnalises(List<AtivoLocalDTO> lAtivos)
        {
            try
            {
                
                baseFreeStockChartPlus = new TerminalWebSVC.TerminalWebClient(ServiceWCF.basicBind, ServiceWCF.endPointTerminalWebSVC);

                //Associando o novo behavior aos serviços
                baseFreeStockChartPlus.ChannelFactory.Endpoint.Behaviors.Add(new Util.ClassBehaviour());

                baseFreeStockChartPlus.RetornaTodosPublicadoresCompleted += new EventHandler<TerminalWebSVC.RetornaTodosPublicadoresCompletedEventArgs>(baseFreeStockChartPlus_RetornaTodosPublicadoresCompleted);
                baseFreeStockChartPlus.RetornaTodosPublicadoresAsync();

                listaAtivos = lAtivos;
                listaAux = listaAtivos;

                InitializeComponent();

                if (ServiceWCF.DeployCorretora)
                {
                    this.Title = "Registro de Interesse em Analise";
                    tbkTitulo.Text = "Registro de Interesse em Analise";

                    gridPublicadoresDisponiveis.IsEnabled = false;
                    gridPublicadoresSelecionados.IsEnabled = false;

                    btnPublicadoresAdicionar.IsEnabled = false;
                    btnPublicadoresAdicionarTodos.IsEnabled = false;

                    btnPublicadoresRemover.IsEnabled = false;
                    btnPublicadoresRemoverTodos.IsEnabled = false;

                    stackPublicador.Visibility = System.Windows.Visibility.Collapsed;
                    this.Height = this.Height - 205;
                }

                gridAtivosDisponiveis.ItemsSource = null;
                gridAtivosDisponiveis.ItemsSource = listaAtivos;

                ModoAguarde(true);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        /// <summary>
        /// Carrega lista de publicadores disponíveis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseFreeStockChartPlus_RetornaTodosPublicadoresCompleted(object sender, TerminalWebSVC.RetornaTodosPublicadoresCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    listaPublicadores = e.Result;

                    gridPublicadoresDisponiveis.ItemsSource = null;
                    gridPublicadoresDisponiveis.ItemsSource = listaPublicadores;

                    if (ServiceWCF.DeployCorretora)
                    {
                        listaPublicadoresSelecionados = listaPublicadores;
                        gridPublicadoresSelecionados.ItemsSource = null;
                        gridPublicadoresSelecionados.ItemsSource = listaPublicadoresSelecionados;
                    }

                    baseFreeStockChartPlus.RetornaInteressePorUsuarioIdCompleted += new EventHandler<TerminalWebSVC.RetornaInteressePorUsuarioIdCompletedEventArgs>(baseFreeStockChartPlus_RetornaInteressePorUsuarioIdCompleted);
                    baseFreeStockChartPlus.RetornaInteressePorUsuarioIdAsync(ServiceWCF.Usuario.Id);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        void baseFreeStockChartPlus_RetornaInteressePorUsuarioIdCompleted(object sender, TerminalWebSVC.RetornaInteressePorUsuarioIdCompletedEventArgs e)
        {
            ModoAguarde(false);

            try
            {
                if (e.Error == null)
                {
                    listaInteresse = e.Result;
                    
                    foreach (TerminalWebSVC.InteresseAnaliseDTO interesse in listaInteresse)
                    {
                        txtEmail.Text = interesse.Email;
                        foreach (TerminalWebSVC.UsuarioDTO publicador in listaPublicadores)
                        {
                            if ((publicador.Id == interesse.PublicadorId) && (!listaPublicadoresSelecionados.Contains(publicador)))
                            {
                                listaPublicadoresSelecionados.Add(publicador);
                                gridPublicadoresSelecionados.ItemsSource = null;
                                gridPublicadoresSelecionados.ItemsSource = listaPublicadoresSelecionados;
                            }
                        }

                        foreach (AtivoLocalDTO ativo in listaAtivos)
                        {
                            if ((ativo.Ativo == interesse.Ativo) && (!listaAtivosSelecionados.Contains(ativo)))
                            {
                                listaAtivosSelecionados.Add(ativo);
                                gridAtivosSelecionados.ItemsSource = null;
                                gridAtivosSelecionados.ItemsSource = listaAtivosSelecionados;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        /// <summary>
        /// Adiciona um Publicador na lista de selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPublicadoresAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (gridPublicadoresDisponiveis.SelectedIndex >= 0)
            {
                foreach (TerminalWebSVC.UsuarioDTO publicadorSelecionado in gridAtivosDisponiveis.SelectedItems)
                {
                    if (!listaPublicadoresSelecionados.Contains(publicadorSelecionado)) //listaPublicadores[gridPublicadoresDisponiveis.SelectedIndex]
                    {
                        listaPublicadoresSelecionados.Add(publicadorSelecionado);
                    }
                }

                gridPublicadoresSelecionados.ItemsSource = null;
                gridPublicadoresSelecionados.ItemsSource = listaPublicadoresSelecionados;
            }
        }

        /// <summary>
        /// Remove um Publicador da lista de selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPublicadoresRemover_Click(object sender, RoutedEventArgs e)
        {
            if (gridPublicadoresSelecionados.SelectedIndex >= 0)
            {
                List<TerminalWebSVC.UsuarioDTO> publicadorTemp = new List<TerminalWebSVC.UsuarioDTO>();

                foreach (TerminalWebSVC.UsuarioDTO publicadorSelecionado in gridPublicadoresSelecionados.SelectedItems)
                {
                    publicadorTemp.Add(publicadorSelecionado);
                }

                foreach (TerminalWebSVC.UsuarioDTO publicadorSelecionado in publicadorTemp)
                {
                    listaPublicadoresSelecionados.Remove(publicadorSelecionado);
                }
                gridPublicadoresSelecionados.ItemsSource = null;
                gridPublicadoresSelecionados.ItemsSource = listaPublicadoresSelecionados;
            }
        }

        /// <summary>
        /// Copia toda a lista de Publicadores para a de selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPublicadoresAdicionarTodos_Click(object sender, RoutedEventArgs e)
        {
            foreach (TerminalWebSVC.UsuarioDTO publicador in listaPublicadores)
            {
                if(!listaPublicadoresSelecionados.Contains(publicador))
                    listaPublicadoresSelecionados.Add(publicador);
            }

            gridPublicadoresSelecionados.ItemsSource = null;
            gridPublicadoresSelecionados.ItemsSource = listaPublicadoresSelecionados;
        }

        /// <summary>
        /// Limpa a lista de Publicadores selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPublicadoresRemoverTodos_Click(object sender, RoutedEventArgs e)
        {
            listaPublicadoresSelecionados = new List<TerminalWebSVC.UsuarioDTO>();
            gridPublicadoresSelecionados.ItemsSource = null;
            gridPublicadoresSelecionados.ItemsSource = listaPublicadoresSelecionados;
        }

        /// <summary>
        /// Adiciona um ativo na lista de selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAtivosAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (gridAtivosDisponiveis.SelectedIndex >= 0)
            {
                foreach (AtivoLocalDTO itemSelecionado in gridAtivosDisponiveis.SelectedItems)
                {
                    if (!listaAtivosSelecionados.Contains(itemSelecionado)) //(AtivoLocalDTO)gridAtivosDisponiveis.SelectedItem
                    {
                        listaAtivosSelecionados.Add(itemSelecionado); //(AtivoLocalDTO)gridAtivosDisponiveis.SelectedItem
                    }
                }

                gridAtivosSelecionados.ItemsSource = null;
                gridAtivosSelecionados.ItemsSource = listaAtivosSelecionados;
            }
        }

        /// <summary>
        /// Remove um ativo da lista de selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAtivosRemover_Click(object sender, RoutedEventArgs e)
        {
            if (gridAtivosSelecionados.SelectedIndex >= 0)
            {
                List<AtivoLocalDTO> ativosTemp = new List<AtivoLocalDTO>();

                foreach (AtivoLocalDTO itemSelecionado in gridAtivosSelecionados.SelectedItems)
                {
                    ativosTemp.Add(itemSelecionado);
                }

                foreach (AtivoLocalDTO itemSelecionado in ativosTemp)
                {
                    listaAtivosSelecionados.Remove(itemSelecionado);
                }

                gridAtivosSelecionados.ItemsSource = null;
                gridAtivosSelecionados.ItemsSource = listaAtivosSelecionados;
            }
        }

        /// <summary>
        /// Copia toda a lista de ativos para a de selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAtivosAdicionarTodos_Click(object sender, RoutedEventArgs e)
        {
            foreach (AtivoLocalDTO ativo in listaAux)
            {
                if(!listaAtivosSelecionados.Contains(ativo))
                    listaAtivosSelecionados.Add(ativo);
            }

            gridAtivosSelecionados.ItemsSource = null;
            gridAtivosSelecionados.ItemsSource = listaAtivosSelecionados;
        }

        /// <summary>
        /// Limpa a lista de ativos selecionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAtivosRemoverTodos_Click(object sender, RoutedEventArgs e)
        {
            listaAtivosSelecionados = new List<AtivoLocalDTO>();
            gridAtivosSelecionados.ItemsSource = null;
            gridAtivosSelecionados.ItemsSource = listaAtivosSelecionados;
        }

        private void btnConcluirInteresse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ModoAguarde(true);

                
                if ((listaAtivosSelecionados.Count > 0) && (listaPublicadoresSelecionados.Count > 0))
                {
                    listaInteresse = new List<TerminalWebSVC.InteresseAnaliseDTO>();

                    int i, j = 0;

                    for (i = 0; i < listaPublicadoresSelecionados.Count; i++)
                    {
                        for (j = 0; j < listaAtivosSelecionados.Count; j++)
                        {
                            TerminalWebSVC.InteresseAnaliseDTO interesse = new TerminalWebSVC.InteresseAnaliseDTO();
                            interesse.UsuarioId = ServiceWCF.Usuario.Id;
                            interesse.Email = txtEmail.Text;
                            interesse.PublicadorId = listaPublicadoresSelecionados[i].Id;
                            interesse.Ativo = listaAtivosSelecionados[j].Ativo;

                            listaInteresse.Add(interesse);
                        }
                    }

                    baseFreeStockChartPlus.SalvaInteresseAnaliseCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(baseFreeStockChartPlus_SalvaInteresseAnaliseCompleted);
                    baseFreeStockChartPlus.SalvaInteresseAnaliseAsync(listaInteresse);
                }
                else
                    MessageBox.Show("Você deve selecionar ao menos 1 (um) Publicador e 1 (um) Ativo.");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        void baseFreeStockChartPlus_SalvaInteresseAnaliseCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ModoAguarde(false);
                MessageBox.Show("Dados cadastrados com sucesso.");
                this.DialogResult = true;
            }
        }

        private void ModoAguarde(bool aguarde)
        {
            try
            {
                if (aguarde)
                {
                    if (!ServiceWCF.DeployCorretora)
                    {
                        gridPublicadoresDisponiveis.IsEnabled = false;
                        gridPublicadoresSelecionados.IsEnabled = false;
                        btnPublicadoresAdicionar.IsEnabled = false;
                        btnPublicadoresRemover.IsEnabled = false;
                        btnPublicadoresAdicionarTodos.IsEnabled = false;
                        btnPublicadoresRemoverTodos.IsEnabled = false;
                    }

                    gridAtivosDisponiveis.IsEnabled = false;
                    gridAtivosSelecionados.IsEnabled = false;
                    btnAtivosAdicionar.IsEnabled = false;
                    btnAtivosRemover.IsEnabled = false;
                    btnAtivosAdicionarTodos.IsEnabled = false;
                    btnAtivosRemoverTodos.IsEnabled = false;

                    btnConcluirInteresse.IsEnabled = false;
                    pnlCarregando.Visibility = System.Windows.Visibility.Visible;
                    this.Cursor = Cursors.Wait;
                }
                else
                {
                    if (!ServiceWCF.DeployCorretora)
                    {
                        gridPublicadoresDisponiveis.IsEnabled = true;
                        gridPublicadoresSelecionados.IsEnabled = true;
                        btnPublicadoresAdicionar.IsEnabled = true;
                        btnPublicadoresRemover.IsEnabled = true;
                        btnPublicadoresAdicionarTodos.IsEnabled = true;
                        btnPublicadoresRemoverTodos.IsEnabled = true;
                    }

                    gridAtivosDisponiveis.IsEnabled = true;
                    gridAtivosSelecionados.IsEnabled = true;
                    btnAtivosAdicionar.IsEnabled = true;
                    btnAtivosRemover.IsEnabled = true;
                    btnAtivosAdicionarTodos.IsEnabled = true;
                    btnAtivosRemoverTodos.IsEnabled = true;

                    btnConcluirInteresse.IsEnabled = true;
                    pnlCarregando.Visibility = System.Windows.Visibility.Collapsed;
                    this.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        /// <summary>
        /// Leva para a filtragem da busca.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAtivoDigitando_TextChanged(object sender, TextChangedEventArgs e)
        {
            BuscarAtivos();
        }

        // Código antigo que fazia exatamente a mesma coisa com muito mais linhas:

        //private void txtAtivoDigitando_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (!(sender is TextBox)) return;

        //    //do not handle ModifierKeys 
        //    if (Keyboard.Modifiers != ModifierKeys.None) return;
        //    if ((e.Key < Key.A) || (e.Key > Key.Z))
        //    {
        //        if (e.Key != Key.Back)
        //            return;
        //        else
        //        {
        //            //Limpando o grid
        //            gridAtivosDisponiveis.ItemsSource = null;

        //            //Resetando o grid            
        //            gridAtivosDisponiveis.ItemsSource = listaAtivos;
        //        }
        //    }

        //    TextBox tb = (TextBox)sender;

        //    string s = new string(new char[] { (char)e.PlatformKeyCode });
        //    int selstart = tb.SelectionStart;
        //    tb.Text = tb.Text.Remove(selstart, tb.SelectionLength);
        //    tb.Text = tb.Text.Insert(selstart, s);

        //    tb.Select(selstart + 1, 0);
        //    e.Handled = true;

        //    //Carregar a busca
        //    BuscarAtivos();
        //}

        /// <summary>
        /// Metodo que atualiza a lista
        /// </summary>
        private void BuscarAtivos()
        {
            listaAux = new List<AtivoLocalDTO>();

            foreach (AtivoLocalDTO obj in listaAtivos)
            {
                if ((obj.Ativo.Contains(txtAtivoDigitando.Text.ToUpper()) ||
                    (obj.Empresa.Contains(txtAtivoDigitando.Text.ToUpper()))))
                    listaAux.Add(obj);
            }

            //Limpando o grid
            gridAtivosDisponiveis.ItemsSource = null;

            //Resetando o grid            
            gridAtivosDisponiveis.ItemsSource = listaAux;
        }
    }
}

