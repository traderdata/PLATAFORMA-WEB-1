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
using Traderdata.Client.Componente.GraficoSL.Util;


namespace Traderdata.Client.Componente.GraficoSL.Configuracao
{
    public partial class ConfiguraTexto : ChildWindow
    {
        private double tamanhoFonte;
        private ColorDialog corDialog;
        private Brush cor;

        private string texto;

        /// <summary>
        /// Obtém ou seta o texto
        /// </summary>
        public string Texto
        {
            get { return texto; }
            set { texto = value; }
        }

        /// <summary>
        /// Obtém ou seta Tamanho da fonte
        /// </summary>
        public double TamanhoFonte
        {
            get { return tamanhoFonte; }
            set { tamanhoFonte = value; }
        }

        /// <summary>
        /// Obtém ou seta a cor do fundo
        /// </summary>
        public Brush Cor
        {
            get { return cor; }
            set { cor = value; }
        }

        public ConfiguraTexto(string texto, double tamanhoFonte, Brush cor)
        {
            InitializeComponent();
            this.TamanhoFonte = tamanhoFonte;
            this.Texto = texto;
            this.cor = cor;

            numTamanhoFonte.Value = tamanhoFonte;
            txtTexto.Text = texto;
            rectCor.Fill = cor;
            if(txtTexto.Text.Trim() == "")
                OKButton.IsEnabled = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtTexto.Text.Trim() != "")
            {
                texto = txtTexto.Text;
                tamanhoFonte = numTamanhoFonte.Value;
                this.DialogResult = true;
            }
            else
                MessageBox.Show("Digite um texto antes de aplicar o objeto.", "Aviso", MessageBoxButton.OK);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

        }

        private void rectCor_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SolidColorBrush corAux = (SolidColorBrush)cor;

            corDialog = new ColorDialog(corAux.Color);
            corDialog.Closing += (sender1, e1) =>
            { cor = new SolidColorBrush(corDialog.Cor); rectCor.Fill = cor; };

            corDialog.Show();
        }

        private void text_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtTexto.Text.Trim() != "")
                OKButton.IsEnabled = true;
            else 
                OKButton.IsEnabled = false;
        }

        private void txtTexto_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    if (txtTexto.Text != "")
                    {
                        texto = txtTexto.Text;
                        tamanhoFonte = numTamanhoFonte.Value;
                        this.DialogResult = true;
                    }
                    else
                        MessageBox.Show("Digite um texto para inserir.", "Aviso", MessageBoxButton.OK);
                }
            }
            catch
            {
                MessageBox.Show("Erro inserir texto.", "Atenção", MessageBoxButton.OK);
            }
        }
    }
}

