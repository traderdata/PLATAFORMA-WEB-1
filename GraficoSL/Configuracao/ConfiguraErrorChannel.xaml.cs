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
    public partial class ConfiguraErrorChannel : ChildWindow
    {
		
		private double valor;
		private Brush cor;
		private double espessura;
		
		//estudosSelecionados[0].StrokeThickness, estudosSelecionados[0].StrokeType
		private ColorDialog corDialog;

        public ConfiguraErrorChannel(double valor, Brush cor, double espessura)
        {
            InitializeComponent();
			this.valor = valor;
			this.cor = cor;
			this.espessura = espessura;
			
			
			numValor.Value = valor;
            numEspessura.Value = espessura;
			rectCor.Fill = cor;
        }		
		
		
		/// <summary>
        /// Obtém ou seta o valor do estudo
        /// </summary>
        public double Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        /// <summary>
        /// Obtém ou seta a cor das linhas
        /// </summary>
        public Brush Cor
        {
            get { return cor; }
            set { cor = value; }
        }


        /// <summary>
        /// Obtém ou seta a cor das linhas
        /// </summary>
        public double Espessura
        {
            get { return espessura; }
            set { espessura = value; }
        }
		

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
			valor = numValor.Value;
			espessura = numEspessura.Value;
			cor = rectCor.Fill;				
			
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        private void rectCor_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	SolidColorBrush corFundoAux = (SolidColorBrush)cor;
			
            corDialog = new ColorDialog(corFundoAux.Color);
            corDialog.Closing += (sender1, e1) =>
            { cor = new SolidColorBrush(corDialog.Cor); rectCor.Fill = cor; };

            corDialog.Show();
        }

       
    }
}

