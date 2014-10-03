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
using Traderdata.Client.TerminalWEB.DTO;
using Traderdata.Client.TerminalWEB.Util;
using System.Windows.Data;
using System.Globalization;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class CentralAnalise : ChildWindow
    {
        TerminalWebSVC.TerminalWebClient baseFreeStockChartPlus;

        private List<AtivoLocalDTO> listaAtivos = new List<AtivoLocalDTO>();
        private List<TerminalWebSVC.AnaliseCompartilhadaDTO> listaAnalise = new List<TerminalWebSVC.AnaliseCompartilhadaDTO>();
        private List<TerminalWebSVC.UsuarioDTO> listaPublicadores = new List<TerminalWebSVC.UsuarioDTO>();

        private List<AnaliseCentro> listaAnaliseCentro;

        public CentralAnalise(List<AtivoLocalDTO> lAtivos)
        {
            try
            {
                baseFreeStockChartPlus = new TerminalWebSVC.TerminalWebClient(ServiceWCF.basicBind, ServiceWCF.endPointTerminalWebSVC);

                //Associando o novo behaviuour aos serviços
                baseFreeStockChartPlus.ChannelFactory.Endpoint.Behaviors.Add(new Util.ClassBehaviour());

                baseFreeStockChartPlus.RetornaTodosPublicadoresCompleted += new EventHandler<TerminalWebSVC.RetornaTodosPublicadoresCompletedEventArgs>(baseFreeStockChartPlus_RetornaTodosPublicadoresCompleted);
                baseFreeStockChartPlus.RetornaTodosPublicadoresAsync();

                InitializeComponent();

                listaAtivos = lAtivos;

                (dtgAnalises.Columns[1] as DataGridBoundColumn).Binding.Converter = new DateTimeConverter();

                ModoAguarde(true);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        void baseFreeStockChartPlus_RetornaTodosPublicadoresCompleted(object sender, TerminalWebSVC.RetornaTodosPublicadoresCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    listaPublicadores = e.Result;

                    ModoAguarde(false);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            ModoAguarde(true);

            if (rdbInteresses.IsChecked == true)
            {
                baseFreeStockChartPlus.RetornaAnaliseComInteresseCompleted += new EventHandler<TerminalWebSVC.RetornaAnaliseComInteresseCompletedEventArgs>(baseFreeStockChartPlus_RetornaAnaliseComInteresseCompleted);
                baseFreeStockChartPlus.RetornaAnaliseComInteresseAsync(ServiceWCF.Usuario.Id);
            }
            else if (rdbTodas.IsChecked == true)
            {
                baseFreeStockChartPlus.RetornaTodasAnalisesCompleted += new EventHandler<TerminalWebSVC.RetornaTodasAnalisesCompletedEventArgs>(baseFreeStockChartPlus_RetornaTodasAnalisesCompleted);
                baseFreeStockChartPlus.RetornaTodasAnalisesAsync();
            }
        }

        void baseFreeStockChartPlus_RetornaTodasAnalisesCompleted(object sender, TerminalWebSVC.RetornaTodasAnalisesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                listaAnalise = e.Result;
            }

            OrganizaLista();
        }

        void baseFreeStockChartPlus_RetornaAnaliseComInteresseCompleted(object sender, TerminalWebSVC.RetornaAnaliseComInteresseCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                listaAnalise = e.Result;
            }

            OrganizaLista();
        }

        private void OrganizaLista()
        {
            AnaliseCentro temp;

            listaAnaliseCentro = new List<AnaliseCentro>();

            foreach (TerminalWebSVC.AnaliseCompartilhadaDTO analise in listaAnalise)
            {
                temp = new AnaliseCentro();

                temp.Id = analise.Id;
                temp.Ativo = analise.Ativo;
                if (analise.Comentario != null)
                {
                    if (analise.Comentario.Length > 50)
                        temp.Comentario = analise.Comentario.Substring(0, 50) + "...";
                    else
                        temp.Comentario = analise.Comentario;
                }

                temp.Data = analise.Data;
                temp.Guid = analise.CaminhoImagem;

                foreach(TerminalWebSVC.UsuarioDTO publicador in listaPublicadores)
                {
                    if (publicador.Id == analise.UsuarioId)
                    {
                        temp.Publicador = publicador.Nome;
                    }
                }

                listaAnaliseCentro.Add(temp);
            }

            dtgAnalises.ItemsSource = null;
            dtgAnalises.ItemsSource = listaAnaliseCentro;

            ModoAguarde(false);
        }

        private void btnVisualiarAnalise_Click(object sender, RoutedEventArgs e)
        {
            if ((AnaliseCentro)dtgAnalises.SelectedItem != null)
            {
                System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(ServiceWCF.LinkVisualizacao + geraParametro()),
                    "_new", "menubar=no, scrollbar=yes, scrollbars=yes, Width=1024, toolbars=no, toolbar=no, manubars=no");
            }
        }

        private string geraParametro()
        {
            int idAnalise = ((AnaliseCentro)dtgAnalises.SelectedItem).Id;
            string guidAnalise = ((AnaliseCentro)dtgAnalises.SelectedItem).Guid;

            return "?i=" + idAnalise + "&g=" + guidAnalise;
        }

        private void btnImportarAnalise_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((AnaliseCentro)dtgAnalises.SelectedItem != null)
                {
                    ModoAguarde(true);

                    //chamando imnport de analise
                    baseFreeStockChartPlus.ImportarAnaliseCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(baseFreeStockChartPlus_ImportarAnaliseCompleted);
                    baseFreeStockChartPlus.ImportarAnaliseAsync(((AnaliseCentro)dtgAnalises.SelectedItem).Id, ServiceWCF.IdUsuario);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ocorreu um erro ao tentar importar a analise para sua área de trabalho");
                MessageBox.Show(exc.ToString());
            }
            finally
            {
                ModoAguarde(false);
            }
            
            
        }

        void baseFreeStockChartPlus_ImportarAnaliseCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Analise importada com sucesso. Será necessário sair da ferramenta e abri-la novamente para que o gráfico importado seja apresentado em sua área de trabalho.");
            ModoAguarde(false);
        }

        private void ModoAguarde(bool aguarde)
        {
            try
            {
                if (aguarde)
                {
                    rdbInteresses.IsEnabled = false;
                    rdbTodas.IsEnabled = false;
                    btnBuscar.IsEnabled = false;

                    dtgAnalises.IsEnabled = false;

                    btnVisualiarAnalise.IsEnabled = false;
                    btnImportarAnalise.IsEnabled = false;

                    pnlCarregando.Visibility = System.Windows.Visibility.Visible;
                    this.Cursor = Cursors.Wait;
                }
                else
                {
                    rdbInteresses.IsEnabled = true;
                    rdbTodas.IsEnabled = true;
                    btnBuscar.IsEnabled = true;

                    dtgAnalises.IsEnabled = true;

                    btnVisualiarAnalise.IsEnabled = true;
                    btnImportarAnalise.IsEnabled = true;                    

                    pnlCarregando.Visibility = System.Windows.Visibility.Collapsed;
                    this.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }
    }

    public class AnaliseCentro
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string guid;
        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        private string ativo;
        public string Ativo
        {
            get { return ativo; }
            set { ativo = value; }
        }

        private string publicador;
        public string Publicador
        {
            get { return publicador; }
            set { publicador = value; }
        }

        private string comentario;
        public string Comentario
        {
            get { return comentario; }
            set { comentario = value; }
        }

        private DateTime data;
        public DateTime Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}

