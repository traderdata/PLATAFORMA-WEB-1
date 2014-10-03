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
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.Util;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;

namespace Traderdata.Client.Componente.GraficoSL.Configuracao
{
    public partial class ConfiguraObjeto : ChildWindow
    {
		#region Campos e Construtrores
		
		private Color corObjeto;
		private double grossuraObjeto;
		private EnumGeral.TipoLinha tipoLinhaObjeto;
        private bool linhaTendenciaInfinita = false;
        private bool linhaMagnetica = false;

		
        public ConfiguraObjeto()
        {
            InitializeComponent();

            this.Height -= 84;

			cmbTipo.Items.Add("Pontilhado");
			cmbTipo.Items.Add("Pontilhado e Tracejado");
			cmbTipo.Items.Add("Tracejado");
			cmbTipo.Items.Add("Solido");
			cmbTipo.SelectedIndex = 0;
        }

        public ConfiguraObjeto(Brush cor, double grossura, EnumGeral.TipoLinha tipoLinha)
        {
            InitializeComponent();
			
			SolidColorBrush corAux = (SolidColorBrush)cor;

            this.Height -= 84;

			this.corObjeto = corAux.Color;
			this.grossuraObjeto = grossura;
			this.tipoLinhaObjeto = tipoLinha;

			cmbTipo.Items.Add("Pontilhado");
			cmbTipo.Items.Add("Pontilhado e Tracejado");
			cmbTipo.Items.Add("Tracejado");
			cmbTipo.Items.Add("Solido");
			cmbTipo.SelectedIndex = 0;

            rectCorFundo.Fill = cor;
            numGrossuraObjeto.Value = grossura;

            chkLinhaMagnetica.Visibility = Visibility.Collapsed;

            switch (tipoLinha)
            {
                case EnumGeral.TipoLinha.Pontilhado:
                    cmbTipo.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoLinha.TracejadoPontilhado:
                    cmbTipo.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoLinha.Tracejado:
                    cmbTipo.SelectedIndex = 2;
                    break;

                default:
                    cmbTipo.SelectedIndex = 3;
                    break;
            }
        }


        /// <summary>
        /// Usar para configurar linha de tendencia infinita.
        /// </summary>
        /// <param name="cor"></param>
        /// <param name="grossura"></param>
        /// <param name="EnumGeral.tipoLinha"></param>
        /// <param name="linhaTendencia"></param>
        public ConfiguraObjeto(Brush cor, double grossura, EnumGeral.TipoLinha tipoLinha, TrendLine linhaTendencia, bool linhaMagnetica, bool permiteLinhaMagnetica, bool permiteLinhaInfinita, bool suporte, bool resistencia)
        {
            InitializeComponent();

            SolidColorBrush corAux = (SolidColorBrush)cor;

            this.corObjeto = corAux.Color;
            this.grossuraObjeto = grossura;
            this.tipoLinhaObjeto = tipoLinha;
            this.linhaMagnetica = linhaMagnetica;

            cmbTipo.Items.Add("Pontilhado");
            cmbTipo.Items.Add("Pontilhado e Tracejado");
            cmbTipo.Items.Add("Tracejado");
            cmbTipo.Items.Add("Solido");
            cmbTipo.SelectedIndex = 0;

            rectCorFundo.Fill = cor;
            numGrossuraObjeto.Value = grossura;
            chkLinhaMagnetica.IsChecked = linhaMagnetica;
            rdbResistencia.IsChecked = resistencia;
            rdbSuporte.IsChecked = suporte;
            switch (tipoLinha)
            {
                case EnumGeral.TipoLinha.Pontilhado:
                    cmbTipo.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoLinha.TracejadoPontilhado:
                    cmbTipo.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoLinha.Tracejado:
                    cmbTipo.SelectedIndex = 2;
                    break;

                default:
                    cmbTipo.SelectedIndex = 3;
                    break;
            }

            chkLinhaInfinita.Visibility = Visibility.Collapsed;
            chkLinhaInfinita.IsChecked = linhaTendencia.LinhaInfinitaADireita;

            //Checando se é possível alterar para linha magnetica ou inifinita
            chkLinhaInfinita.IsEnabled = permiteLinhaInfinita;
            chkLinhaMagnetica.IsEnabled = permiteLinhaMagnetica;

            rdbSuporte.IsChecked = suporte;
            rdbResistencia.IsChecked = resistencia;
        }

        /// <summary>
        /// Usar para configurar linha de tendencia infinita.
        /// </summary>
        /// <param name="cor"></param>
        /// <param name="grossura"></param>
        /// <param name="EnumGeral.tipoLinha"></param>
        /// <param name="linhaTendencia"></param>
        public ConfiguraObjeto(Brush cor, double grossura, EnumGeral.TipoLinha tipoLinha, bool linhaMagnetica, bool permiteLinhaMagnetica, bool permiteLinhaInfinita, bool suporte, bool resistencia)
        {
            InitializeComponent();

            this.Height -= 42;

            SolidColorBrush corAux = (SolidColorBrush)cor;

            this.corObjeto = corAux.Color;
            this.grossuraObjeto = grossura;
            this.tipoLinhaObjeto = tipoLinha;
            this.linhaMagnetica = linhaMagnetica;

            cmbTipo.Items.Add("Pontilhado");
            cmbTipo.Items.Add("Pontilhado e Tracejado");
            cmbTipo.Items.Add("Tracejado");
            cmbTipo.Items.Add("Solido");
            cmbTipo.SelectedIndex = 0;

            rectCorFundo.Fill = cor;
            numGrossuraObjeto.Value = grossura;
            chkLinhaMagnetica.IsChecked = linhaMagnetica;
            rdbResistencia.IsChecked = resistencia;
            rdbSuporte.IsChecked = suporte;
            switch (tipoLinha)
            {
                case EnumGeral.TipoLinha.Pontilhado:
                    cmbTipo.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoLinha.TracejadoPontilhado:
                    cmbTipo.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoLinha.Tracejado:
                    cmbTipo.SelectedIndex = 2;
                    break;

                default:
                    cmbTipo.SelectedIndex = 3;
                    break;
            }

            //Checando se é possível alterar para linha magnetica ou inifinita
            chkLinhaInfinita.IsEnabled = permiteLinhaInfinita;
            chkLinhaMagnetica.IsEnabled = permiteLinhaMagnetica;
            
        }

        #endregion Campos e Construtrores
		
		#region Propriedades

        public bool LinhaMagnetica
        {
            get { return linhaMagnetica; }
            set { linhaMagnetica = value; }
        }

        public bool LinhaTendenciaInfinita
        {
            get { return linhaTendenciaInfinita; }
            set { linhaTendenciaInfinita = value; }
        }

		public Color CorObjeto
		{
			get {return corObjeto;}
		}
		
		public Brush BrushObjeto
		{
			get {return new SolidColorBrush(corObjeto);}
		}
		
		public double GrossuraObjeto
		{
			get {return grossuraObjeto;}
		}

        public EnumGeral.TipoLinha TipoLinhaObjeto
		{
			get {return tipoLinhaObjeto;}
		}
		
		#endregion Propriedades

		#region Eventos
		
		/// <summary>
		/// Clique no botão de cor.
		/// </summary>
        private void btnCor_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColorDialog corDialog = new ColorDialog((Color)corObjeto);
			corDialog.Show();
			
			corDialog.Closing += (sender1, e1) =>{corObjeto = corDialog.Cor; rectCorFundo.Fill = new SolidColorBrush(corDialog.Cor);};
        }
		
		
		/// <summary>
		/// Clique no botão ok.
		/// </summary>
		private void OKButton_Click(object sender, RoutedEventArgs e)
        {
			//Atualizando variáveis
			grossuraObjeto = (double)numGrossuraObjeto.Value;
			tipoLinhaObjeto = ObtemLinhaSelecionada();
            linhaMagnetica = (bool)chkLinhaMagnetica.IsChecked;

            if (chkLinhaInfinita.Visibility == Visibility.Visible)
                linhaTendenciaInfinita = (bool)chkLinhaInfinita.IsChecked;
			
            this.DialogResult = true;
        }

		
		/// <summary>
		/// Clique no botão cancelar.
		/// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
		
		#endregion Eventos
		
		#region Métodos
		
		/// <summary>
		/// Retorna a linha selecionada na combo.
		/// </summary>
		/// <returns></returns>
		private EnumGeral.TipoLinha ObtemLinhaSelecionada()
		{
			switch (cmbTipo.SelectedIndex)
			{
				case 0:
                    return EnumGeral.TipoLinha.Pontilhado;
					
				case 1:
                    return EnumGeral.TipoLinha.TracejadoPontilhado;
					
				case 2:
                    return EnumGeral.TipoLinha.Tracejado;
					
				default:
                    return EnumGeral.TipoLinha.Solido;
			}
		}
		
		#endregion Métodos
    }
}

