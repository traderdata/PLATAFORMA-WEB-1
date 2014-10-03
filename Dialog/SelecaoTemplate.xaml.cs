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

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class SelecaoTemplate : ChildWindow
    {
        #region Variaveis Privadas

        private TerminalWebSVC.TemplateDTO templateLocal;

        #endregion

        #region Propriedades

        public TerminalWebSVC.TemplateDTO TemplateLocal 
        {
            get { return this.templateLocal; }
        }

        #endregion

        #region Construtor

        /// <summary>
        /// Construtor basico
        /// </summary>
        public SelecaoTemplate(List<TerminalWebSVC.TemplateDTO> listaTemplate)
        {
            InitializeComponent();

            cmbTemplate.ItemsSource = listaTemplate;
            cmbTemplate.DisplayMemberPath = "Nome";
            cmbTemplate.SelectedIndex = 0;            
        }

        #endregion

        #region Eventos

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
            templateLocal = (TerminalWebSVC.TemplateDTO)cmbTemplate.SelectedItem;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion

    }
}

