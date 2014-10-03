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
    public partial class SalvaTemplate : ChildWindow
    {
        #region Campos e Construtores

        private string nome;
        private bool novo = true;
        private TerminalWebSVC.TemplateDTO templateSelecionado;


        public string Nome
        {
            get { return nome; }            
        }

        public bool Novo
        {
            get { return novo; }            
        }

        public TerminalWebSVC.TemplateDTO TemplateSelecionado
        {
            get { return templateSelecionado; }
        }

        public SalvaTemplate(List<TerminalWebSVC.TemplateDTO> listaTemplate)
        {
            InitializeComponent();
            cmbTemplate.DisplayMemberPath = "Nome";
            cmbTemplate.SelectedValuePath = "Id";
            if (listaTemplate.Count > 0)
            {
                cmbTemplate.ItemsSource = listaTemplate;
                cmbTemplate.SelectedIndex = 0;
                cmbTemplate.SelectionChanged += new SelectionChangedEventHandler(cmbTemplate_SelectionChanged);
                this.templateSelecionado = (TerminalWebSVC.TemplateDTO)cmbTemplate.SelectedItem;
            }
            else
                radioButton2.IsEnabled = false;
        }

        
        #endregion Campos e Construtores

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
            nome = txtNome.Text;

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

      
        private void txtNome_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    if (txtNome.Text != "")
                    {
                        nome = txtNome.Text;
                        this.DialogResult = true;
                    }
                    else
                        MessageBox.Show("Digite um nome para o seu template.", "Aviso", MessageBoxButton.OK);
                }
            }
            catch
            {
                MessageBox.Show("Erro ao salvar template.", "Atenção", MessageBoxButton.OK);
            }
        }

        private void radioButton1_Click(object sender, RoutedEventArgs e)
        {
            txtNome.IsEnabled = (bool)radioButton1.IsChecked;
            cmbTemplate.IsEnabled = (bool)radioButton2.IsChecked;
            novo = (bool)radioButton1.IsChecked;
        }

        private void radioButton2_Click(object sender, RoutedEventArgs e)
        {
            txtNome.IsEnabled = (bool)radioButton1.IsChecked;
            cmbTemplate.IsEnabled = (bool)radioButton2.IsChecked;
            novo = (bool)radioButton1.IsChecked;
        }

        private void cmbTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.templateSelecionado = (TerminalWebSVC.TemplateDTO)cmbTemplate.SelectedItem;
        }

    }
}

