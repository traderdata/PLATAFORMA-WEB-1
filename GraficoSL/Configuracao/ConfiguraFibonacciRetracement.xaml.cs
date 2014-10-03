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
    public partial class ConfiguraFibonacciRetracement : ChildWindow
    {
        #region Campos e Construtores

        //Lista de double das linhas de parâmetros
        private List<double?> linhas = new List<double?>();
        private double espessura;
		private EnumGeral.TipoLinha tipoLinha;
		private Brush cor;	  
		private ColorDialog corDialog;
        private bool linhaMagnetica = false;

        
        /// <summary>
        /// Contrutor padrão
        /// </summary>
        /// <param name="linhas">Lista de linhas de tipo double.</param>
        /// <param name="espessura">Tamanho da espessura da linha.</param>
        /// <param name="EnumGeral.tipoLinha">Tipo de linha.</param>
        /// <param name="cor">Cor da linha.</param>
        /// <param name="magnetico">Indica se está usando magnético ou não.</param>
        public ConfiguraFibonacciRetracement(List<double?> linhas, double espessura, EnumGeral.TipoLinha TipoLinha, Brush cor, bool magnetico)
        {
            InitializeComponent();

			numEspessura.Value = espessura;
			rectCor.Fill = cor;
            this.linhaMagnetica = magnetico;
		
			cmbTipoLinha.Items.Add("Pontilhado");
			cmbTipoLinha.Items.Add("Pontilhado e Tracejado");
			cmbTipoLinha.Items.Add("Tracejado");
			cmbTipoLinha.Items.Add("Solido");

            this.linhas = linhas;

			switch (TipoLinha)
            {
                case EnumGeral.TipoLinha.Pontilhado:
                    cmbTipoLinha.SelectedIndex = 0;
                    break;

                case EnumGeral.TipoLinha.TracejadoPontilhado:
                    cmbTipoLinha.SelectedIndex = 1;
                    break;

                case EnumGeral.TipoLinha.Tracejado:
                    cmbTipoLinha.SelectedIndex = 2;
                    break;

                default:
                    cmbTipoLinha.SelectedIndex = 3;
                    break;
            }

            if (linhas[0].HasValue)
            {
                chbLinha1.IsChecked = true;
                numValor1.Value = linhas[0].Value * 100;
            }
            if (linhas[1].HasValue)
            {
                chbLinha2.IsChecked = true;
                numValor2.Value = linhas[1].Value * 100;
            }
            if (linhas[2].HasValue)
            {
                chbLinha3.IsChecked = true;
                numValor3.Value = linhas[2].Value * 100;
            }
            if (linhas[3].HasValue)
            {
                chbLinha4.IsChecked = true;
                numValor4.Value = linhas[3].Value * 100;
            }
            if (linhas[4].HasValue)
            {
                chbLinha5.IsChecked = true;
                numValor5.Value = linhas[4].Value * 100;
            }
            if (linhas[5].HasValue)
            {
                chbLinha6.IsChecked = true;
                numValor6.Value = linhas[5].Value * 100;
            }
            if (linhas[6].HasValue)
            {
                chbLinha7.IsChecked = true;
                numValor7.Value = linhas[6].Value * 100;
            }


            chkLinhaMagnetica.IsChecked = magnetico;
        }

        #endregion Campos e Construtores

        #region Propriedades


        public bool LinhaMagnetica
        {
            get { return linhaMagnetica; }
            set { linhaMagnetica = value; }
        }

        public List<double?> Linhas
        {
            get { return linhas; }
            set { linhas = value; }
        }
		
		public double Espessura
        {
            get { return espessura; }
            set { espessura = value; }
        }

        public EnumGeral.TipoLinha TipoLinha
        {
            get { return tipoLinha; }
            set { tipoLinha = value; }
        }
		
		public Brush Cor
        {
            get { return cor; }
            set { cor = value; }
        }
		
		
		#endregion Propriedades

        #region Eventos

        /// <summary>
        /// Evento dispara quando botão OK é clicado passando os valores do campo para respectivas variáveis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            linhaMagnetica = (bool)chkLinhaMagnetica.IsChecked;

            if(chbLinha7.IsChecked == true)
                linhas[6] = numValor7.Value / 100;
            else
                linhas[6] = null;

            if (chbLinha6.IsChecked == true)
                linhas[5] = numValor6.Value / 100;
            else
                linhas[5] = null;

            if (chbLinha5.IsChecked == true)
                linhas[4] = numValor5.Value / 100;
            else
                linhas[4] = null;

            if (chbLinha4.IsChecked == true)
                linhas[3] = numValor4.Value / 100;
            else
                linhas[3] = null;

            if (chbLinha3.IsChecked == true)
                linhas[2] = numValor3.Value / 100;
            else
                linhas[2] = null;

            if (chbLinha2.IsChecked == true)
                linhas[1] = numValor2.Value / 100;
            else
                linhas[1] = null;

            if (chbLinha1.IsChecked == true)
                linhas[0] = numValor1.Value / 100;
            else
                linhas[0] = null;

			tipoLinha = ObtemLinhaSelecionada();
			
			espessura = numEspessura.Value;
			cor = rectCor.Fill;
			
            this.DialogResult = true;
        }


        /// <summary>
        /// Evento dispara quando botão Cancelar é clicado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Evento é disparado para chamar CorDialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rectCor_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	SolidColorBrush corAux = (SolidColorBrush)cor;
			
            corDialog = new ColorDialog(corAux.Color);
            corDialog.Closing += (sender1, e1) =>
            { 
				cor = new SolidColorBrush(corDialog.Cor); 
				rectCor.Fill = cor; 
			};

            corDialog.Show();
        }

        #endregion Eventos

        #region Métodos

        /// <summary>
		/// Retorna a linha selecionada na combo.
		/// </summary>
		/// <returns></returns>
        private EnumGeral.TipoLinha ObtemLinhaSelecionada()
		{
			switch (cmbTipoLinha.SelectedIndex)
			{
				case 0:
                    return EnumGeral.TipoLinha.Pontilhado;//pontilhado
					
				case 1:
                    return EnumGeral.TipoLinha.TracejadoPontilhado; //pontilhado e tracejado
					
				case 2:
                    return EnumGeral.TipoLinha.Tracejado;//tracejado
					
				default:
                    return EnumGeral.TipoLinha.Solido; // solid
			}
        }

        #endregion Métodos

    }
}

