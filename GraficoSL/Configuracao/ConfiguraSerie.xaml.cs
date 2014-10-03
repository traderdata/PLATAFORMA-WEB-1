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
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.Configuracao
{
    public partial class ConfiguraSerie : ChildWindow
    {
      #region Campos e Construtrores
		
		private Color? corAltaSerie;
        private Color? corBaixaSerie;
        private Color? strokeColor;
		private double grossuraSerie;
		private EnumGeral.TipoLinha tipoLinhaSerie;
		
        public ConfiguraSerie()
        {
            InitializeComponent();
			
			cmbTipo.Items.Add("Pontilhado");
			cmbTipo.Items.Add("Pontilhado e Tracejado");
			cmbTipo.Items.Add("Tracejado");
			cmbTipo.Items.Add("Solido");
			cmbTipo.SelectedIndex = 0;
        }

        public ConfiguraSerie(Color? corAlta, Color? corBaixa, double grossura, EnumGeral.TipoLinha tipoLinha)
        {
            InitializeComponent();
			
			this.corAltaSerie = corAlta;
            this.corBaixaSerie = corBaixa;
			this.grossuraSerie = grossura;
			this.tipoLinhaSerie = tipoLinha;
			
			cmbTipo.Items.Add("Pontilhado");
			cmbTipo.Items.Add("Pontilhado e Tracejado");
			cmbTipo.Items.Add("Tracejado");
			cmbTipo.Items.Add("Solido");
			cmbTipo.SelectedIndex = 0;

            if (corAltaSerie != null)
                rectCorAlta.Fill = new SolidColorBrush((Color)corAltaSerie);
            else
                rectCorAlta.Fill = new SolidColorBrush(Colors.Blue);

            if (corBaixaSerie != null)
                rectCorBaixa.Fill = new SolidColorBrush((Color)corBaixaSerie);
            else
                rectCorBaixa.Fill = new SolidColorBrush(Colors.Blue);

            numGrossuraObjeto.Value = grossura;

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

        public ConfiguraSerie(Color? strokeColor, double grossura, EnumGeral.TipoLinha tipoLinha)
        {
            InitializeComponent();

            this.strokeColor = strokeColor;
            this.grossuraSerie = grossura;
            this.tipoLinhaSerie = tipoLinha;

            cmbTipo.Items.Add("Pontilhado");
            cmbTipo.Items.Add("Pontilhado e Tracejado");
            cmbTipo.Items.Add("Tracejado");
            cmbTipo.Items.Add("Solido");
            cmbTipo.SelectedIndex = 0;

            if (strokeColor == null)
                strokeColor = Colors.Blue;

            rectStrokeColor.Fill = new SolidColorBrush((Color)strokeColor);

            pnlStrokeColor.Visibility = Visibility.Visible;
            pnlCorAlta.Visibility = Visibility.Collapsed;
            pnlCorBaixa.Visibility = Visibility.Collapsed;

            numGrossuraObjeto.Value = grossura;

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
		
        #endregion Campos e Construtrores
		
		#region Propriedades
		
		public Color? CorAltaSerie
		{
			get {return corAltaSerie;}
		}

        public Color? CorBaixaSerie
        {
            get { return corBaixaSerie; }
        }

        public Color? StrokeColor
        {
            get { return strokeColor; }
        }
		
		public double GrossuraSerie
		{
			get {return grossuraSerie;}
		}

        public EnumGeral.TipoLinha TipoLinhaSerie
		{
			get {return tipoLinhaSerie;}
		}
		
		#endregion Propriedades

		#region Eventos
		
		/// <summary>
		/// Clique no botão de cor.
		/// </summary>
        private void btnCorAlta_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColorDialog corDialog = new ColorDialog((Color)corAltaSerie);
			corDialog.Show();
			
			corDialog.Closing += (sender1, e1) =>{corAltaSerie = corDialog.Cor; rectCorAlta.Fill = new SolidColorBrush(corDialog.Cor);};
        }

        /// <summary>
        /// Clique no botão de cor.
        /// </summary>
        private void btnCorBaixa_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColorDialog corDialog = new ColorDialog((Color)corBaixaSerie);
            corDialog.Show();

            corDialog.Closing += (sender1, e1) => { corBaixaSerie = corDialog.Cor; rectCorBaixa.Fill = new SolidColorBrush(corDialog.Cor); };
        }
		
		
		/// <summary>
		/// Clique no botão ok.
		/// </summary>
		private void OKButton_Click(object sender, RoutedEventArgs e)
        {
			//Atualizando variáveis
			grossuraSerie = (double)numGrossuraObjeto.Value;
			tipoLinhaSerie = ObtemLinhaSelecionada();
			
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

        private void btnStrokeColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog corDialog = new ColorDialog((Color)strokeColor);
            corDialog.Show();

            corDialog.Closing += (sender1, e1) => { strokeColor = corDialog.Cor; rectStrokeColor.Fill = new SolidColorBrush(corDialog.Cor); };
        }
    }
}

