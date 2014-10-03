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
using Traderdata.Client.Componente.GraficoSL.Main;
using Traderdata.Client.TerminalWEB.Util;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class PublicarAnalise : ChildWindow
    {
        TerminalWebSVC.TerminalWebClient baseFreeStockChartPlus;
        Grafico grafico;
        TerminalWebSVC.AnaliseCompartilhadaDTO analiseCompartilhada;

        public PublicarAnalise(Grafico obj, TerminalWebSVC.AnaliseCompartilhadaDTO analiseDTO)
        {
            baseFreeStockChartPlus = new TerminalWebSVC.TerminalWebClient(ServiceWCF.basicBind, ServiceWCF.endPointTerminalWebSVC);

            //Associando o novo behavior aos serviços
            baseFreeStockChartPlus.ChannelFactory.Endpoint.Behaviors.Add(new Util.ClassBehaviour());

            grafico = obj;
            analiseCompartilhada = analiseDTO;

            InitializeComponent();
        }


        private void btn_Enter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Hand;
        }

        private void btn_Leave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Arrow;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ModoAguarde(true);

            if (rdbPublico.IsChecked == true)
                analiseCompartilhada.PublicoPrivado = "PU";
            else
                analiseCompartilhada.PublicoPrivado = "PR";

            if (!String.IsNullOrEmpty(txtEmails.Text.Trim()))
            {
                analiseCompartilhada.DispararEmail = txtEmails.Text.Trim();
            }
            else
                analiseCompartilhada.DispararEmail = "";

            baseFreeStockChartPlus.SalvarAnaliseCompartilhadaCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(baseFreeStockChartPlus_SalvarAnaliseCompartilhadaCompleted);
            //baseFreeStockChartPlus.SalvarAnaliseCompartilhadaAsync(grafico.SaveAsImage(), analiseCompartilhada,ServiceWCF.MacroCliente);
            
        }

        void baseFreeStockChartPlus_SalvarAnaliseCompartilhadaCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                MessageBox.Show("Analise Publicada com sucesso.");
                ModoAguarde(false);
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Erro ao publicar analise. Por favor, tente novamente.");
                ModoAguarde(false);
                this.DialogResult = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ModoAguarde(bool aguarde)
        {
            try
            {
                if (aguarde)
                {
                    rdbPrivado.IsEnabled = false;
                    rdbPublico.IsEnabled = false;
                    txtEmails.IsEnabled = false;
                    OKButton.IsEnabled = false;
                    CancelButton.IsEnabled = false;
                    this.Cursor = Cursors.Wait;
                }
                else
                {
                    rdbPrivado.IsEnabled = true;
                    rdbPublico.IsEnabled = true;
                    txtEmails.IsEnabled = true;
                    OKButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }
    }
}

