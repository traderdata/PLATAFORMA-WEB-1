using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.ServiceModel;
using Traderdata.Client.Componente.GraficoSL.Enum;
using System.Windows.Browser;
using System.Windows.Input;
using Traderdata.Client.TerminalWEB.DTO;
using Traderdata.Client.TerminalWEB.Util;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class NovoGrafico : ChildWindow
    {
        #region Campos e Construtores
        //Varialve de serviço
        private TerminalWebSVC.TerminalWebClient baseTerminalWeb;
        private SoaMD.MarketDataTerminalWebClient baseMarketDataCommon;
        private string ativo = "";
        private bool mesmoGrafico = false;
        private Tupla periodicidade;
        private Tupla periodo;
        private List<AtivoLocalDTO> listaAtivo = new List<AtivoLocalDTO>();
        private List<TerminalWebSVC.TemplateDTO> listaTemplate = new List<TerminalWebSVC.TemplateDTO>();

        public NovoGrafico(List<AtivoLocalDTO> listaAtivo, List<TerminalWebSVC.TemplateDTO> listaTemplate, bool mesmoGrafico) 
        {
            InitializeComponent();

            this.listaAtivo = listaAtivo;
            this.listaTemplate = listaTemplate;
            this.mesmoGrafico = mesmoGrafico;
            TerminalWebSVC.TemplateDTO templateNenhum = new TerminalWebSVC.TemplateDTO();
            templateNenhum.Id = -1;
            templateNenhum.Nome = "Nenhum";
            this.listaTemplate.Insert(0, templateNenhum);
            CarregaComboPeriodo();
            CarregaComboPeriodicidade();
            CarregaComboTemplate();
            
            if (this.listaAtivo.Count <= 1)
            {
                ModuloCarregando(true, "Carregando ativos");
                baseMarketDataCommon = new SoaMD.MarketDataTerminalWebClient(ServiceWCF.basicBind, ServiceWCF.endPointMarketDataCommon);
                baseMarketDataCommon.GetAtivosCompactadoCompleted +=new EventHandler<SoaMD.GetAtivosCompactadoCompletedEventArgs>(baseMarketDataCommon_GetAtivosCompactadoCompleted);                    
                baseMarketDataCommon.GetAtivosCompactadoAsync();
            }

        }



        void baseMarketDataCommon_GetAtivosCompactadoCompleted(object sender, SoaMD.GetAtivosCompactadoCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    this.listaAtivo.Clear();
                    foreach (string obj in e.Result)
                    {
                        if (obj.Length > 0)
                            this.listaAtivo.Add(new AtivoLocalDTO(obj.Split(';')[0], obj.Split(';')[1], (EnumLocal.Bolsa)Convert.ToInt16(obj.Split(';')[2])));
                    }

                }

            }
            finally
            {
                ModuloCarregando(false,"");
            }
        }
        
        
        #endregion Campos e Construtores

        #region Propriedades
                
        /// <summary>
        /// Ativo escolhido.
        /// </summary>
        public string Ativo
        {
            get { return ativo; }
            set { ativo = value; }
        }

        /// <summary>
        /// Periodicidade escolhida.
        /// </summary>
        public Tupla Periodicidade
        {
            get { return periodicidade; }
            set { periodicidade = value; }
        }

        /// <summary>
        /// Periodo escolhido.
        /// </summary>
        public Tupla Periodo
        {
            get { return periodo; }
            set { periodo = value; }
        }

        /// <summary>
        /// Propriedade que retorna o template que fora selecionado
        /// </summary>
        public TerminalWebSVC.TemplateDTO TemplateSelecionado
        {
            get { return (TerminalWebSVC.TemplateDTO)cmbTemplate.SelectedItem; }
        }

        /// <summary>
        /// Propriedade que controla se é o mesmo gráfico ou não 
        /// </summary>
        public bool MesmoGrafico
        {
            get { return this.mesmoGrafico; }
        }
               
        #endregion Propriedades

        #region Eventos

        private void btn_Enter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Hand;
        }

        private void btn_Leave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// eventos disparado ao se trocar a periodicidade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPeriodicidade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregaComboPeriodo();

            //selecionando o periodo default
            switch ((int)cmbPeriodicidade.SelectedValue)
            {
                case 1:
                    cmbPeriodo.SelectedItem = EnumPeriodo.DoisDias;
                    break;
                case 2:
                    cmbPeriodo.SelectedItem = EnumPeriodo.DoisDias;
                    break;
                case 3:
                    cmbPeriodo.SelectedItem = EnumPeriodo.TresDias;
                    break;
                case 5:
                    cmbPeriodo.SelectedItem = EnumPeriodo.CincoDias;
                    break;
                case 10:
                    cmbPeriodo.SelectedItem = EnumPeriodo.DezDias;
                    break;
                case 15:
                    cmbPeriodo.SelectedItem = EnumPeriodo.QuinzeDias;
                    break;
                case 30:
                    cmbPeriodo.SelectedItem = EnumPeriodo.UmMes;
                    break;
                case 60:
                    cmbPeriodo.SelectedItem = EnumPeriodo.UmMes;
                    break;
                case 1440:
                    cmbPeriodo.SelectedItem = EnumPeriodo.TresAnos;
                    break;
                case 10080:
                    cmbPeriodo.SelectedItem = EnumPeriodo.DezAnos;
                    break;
                case 43200:
                    cmbPeriodo.SelectedItem = EnumPeriodo.DezAnos;
                    break;
            }
        }

        /// <summary>
        /// Mudança de periodo.
        /// A mudança de periodo deve ajustar a periodicidade.
        /// </summary>
        private void cmbPeriodo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregaComboPeriodicidade();
        }

        /// <summary>
        /// Clique no botao pesquisa.
        /// Abre a tela de pesquisa de ativo.
        /// </summary>
        private void btnPesquisaAtivo_Click(object sender, RoutedEventArgs e)
        {
            PesquisaAtivo pesquisaAtivo = new PesquisaAtivo(txtAtivo.Text, listaAtivo);
            pesquisaAtivo.Closing += (sender1, e1) =>
            {
                if (pesquisaAtivo.DialogResult == true)
                {
                    txtAtivo.Text = pesquisaAtivo.Ativo;
                }
            };
            pesquisaAtivo.Show();
        }
        
        /// <summary>
        /// Tecla pressionada no txtAtivo.
        /// Se for pressionado enter, devo executar o procedimento de clique no botao OK.
        /// </summary>
        private void txtAtivo_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                txtAtivo.Text = txtAtivo.Text.ToUpper();
                Carregar();
            }
        }

        /// <summary>
        /// Clique no botao OK
        /// Realiza verificacao de campos e fecha a janela.
        /// </summary>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            txtAtivo.Text = txtAtivo.Text.ToUpper();
            Carregar();
        }

        /// <summary>
        /// Clique no botao Cancel
        /// Cancela a escolha de ativo, epriodo e periodicidade.
        /// </summary>
        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Evento disparado ao se pressionar qualquer tecla sobre o campo de ativo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAtivo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (!(sender is TextBox)) return;

            ////do not handle ModifierKeys 
            //if (Keyboard.Modifiers != ModifierKeys.None) return;
            //if ((e.Key < Key.A) || (e.Key > Key.Z)) return;

            //TextBox tb = (TextBox)sender;

            //string s = new string(new char[] { (char)e.PlatformKeyCode });
            //int selstart = tb.SelectionStart;
            //tb.Text = tb.Text.Remove(selstart, tb.SelectionLength);
            //tb.Text = tb.Text.Insert(selstart, s);

            //tb.Select(selstart + 1, 0);
            //e.Handled = true;
        }
       
        #endregion Eventos

        #region Métodos

        /// <summary>
        /// Realiza validação de preenchimento de campos.
        /// </summary>
        private void Validacao()
        {
            ativo = txtAtivo.Text;
            periodicidade = (Tupla)cmbPeriodicidade.SelectedItem;
            periodo = (Tupla)cmbPeriodo.SelectedItem;

        }

        /// <summary>
        /// Metodo carrega a combo de periodo
        /// </summary>
        private void CarregaComboPeriodo()
        {
            cmbPeriodo.DisplayMemberPath = "Name";
            cmbPeriodo.SelectedValuePath = "Value";
            cmbPeriodo.ItemsSource = null;

            if (cmbPeriodicidade.SelectedValue != null)
            {
                if (((Tupla)cmbPeriodicidade.SelectedItem).Value < 60)
                {
                    cmbPeriodo.ItemsSource = EnumPeriodo.GetListIntraday();
                    cmbPeriodo.SelectedItem = EnumPeriodo.TresDias;
                }
                else
                {
                    cmbPeriodo.ItemsSource = EnumPeriodo.GetListDiario();
                    cmbPeriodo.SelectedItem = EnumPeriodo.SeisMeses;
                }
            }
        }

        /// <summary>
        /// Metodo carrega as periodicidades
        /// </summary>
        private void CarregaComboPeriodicidade()
        {            
                     
            //populando a combo
            cmbPeriodicidade.DisplayMemberPath = "Name";
            cmbPeriodicidade.SelectedValuePath = "Value";
            cmbPeriodicidade.ItemsSource = null;
            
            
            List<Tupla> listaTotal = new List<Tupla>();
            foreach (Tupla obj in EnumPeriodicidade.GetList(EnumGeral.TipoPeriodicidade.Intraday))
            {
                listaTotal.Add(obj);
            }
            foreach (Tupla obj in EnumPeriodicidade.GetList(EnumGeral.TipoPeriodicidade.Diario))
            {
                listaTotal.Add(obj);
            }

            cmbPeriodicidade.ItemsSource = listaTotal;
            cmbPeriodicidade.SelectedItem = EnumPeriodicidade.Diario;            
                    
            
        }
            
        /// <summary>
        /// Excecuta ação do botão OK, ou seja valida e fecha a tela se estiver tudo correto.
        /// </summary>
        private void Carregar()
        {
            //se for igual a 1 é porque é o gráfico pelado
            if (listaAtivo.Count > 1)
            {
                bool carregou = false;
                if (txtAtivo.Text != "")
                {
                    foreach (AtivoLocalDTO obj in listaAtivo)
                    {
                        if (obj.Ativo == txtAtivo.Text.Trim())
                        {
                            Validacao();
                            this.DialogResult = true;
                            carregou = true;
                            break;
                        }
                    }
                    if (!carregou)
                        MessageBox.Show("Ativo não encontrado, verifique se o código do ativo foi digitado corretamente e tente novamente.", "Aviso", MessageBoxButton.OK);
                }
                else
                    MessageBox.Show("Digite um ativo para carregar o gráfico.", "Aviso", MessageBoxButton.OK);
            }
            else
            {
                
                //neste caso devo buscar pelo ativo para verificar se existe ou nao e caso nao exista carregar IBOV                
                baseMarketDataCommon = new SoaMD.MarketDataTerminalWebClient(ServiceWCF.basicBind, ServiceWCF.endPointMarketDataCommon);
                baseMarketDataCommon.GetAtivoCompleted +=new EventHandler<SoaMD.GetAtivoCompletedEventArgs>(baseMarketDataCommon_GetAtivoCompleted);                    
                baseMarketDataCommon.GetAtivoAsync(txtAtivo.Text.Trim());
                ModuloCarregando(true, "Verificando ativo...");
            }
            
        }

        
        #region ModuloCarregando(+1)
        /// <summary>
        /// Mostra ou esconde a tela de carregamento.
        /// </summary>
        /// <param name="ativo"></param>
        private void ModuloCarregando(bool ativo, string texto)
        {
            
            lblCarregando.Text = texto;

            if (ativo)
                pnlCarregando.Visibility = System.Windows.Visibility.Visible;
            else
                pnlCarregando.Visibility = System.Windows.Visibility.Collapsed;

            this.IsEnabled = !ativo;                
           
        }
        #endregion ModuloCarregando(+1)

        void baseMarketDataCommon_GetAtivoCompleted(object sender, SoaMD.GetAtivoCompletedEventArgs e)
        {
            ModuloCarregando(false,"");
            if (e.Result != null)
            {
                //buscando o ativo direto
                string[] arTempAtivo = e.Result.Split(';');
                if (arTempAtivo.Length > 3)
                    if (txtAtivo.Text.ToUpper() != arTempAtivo[1])
                        CarregaIBOV();
                    else
                    {
                        ativo = txtAtivo.Text.ToUpper();
                        periodicidade = (Tupla)cmbPeriodicidade.SelectedItem;
                        periodo = (Tupla)cmbPeriodo.SelectedItem;
                        this.DialogResult = true;
                    }
                
            }
            else
                CarregaIBOV();            
        }

        private void CarregaIBOV()
        {
            //ativo = "IBOV";
            //periodicidade = (Tupla)cmbPeriodicidade.SelectedItem;
            //periodo = (Tupla)cmbPeriodo.SelectedItem;

            //this.DialogResult = true;
            MessageBox.Show("O ativo informado não é válido");
        }
    

        /// <summary>
        /// Metodo que carrega a combo de templates
        /// </summary>
        private void CarregaComboTemplate()
        {
            if (listaTemplate.Count > 0)
            {
                cmbTemplate.IsEnabled = true;
                cmbTemplate.ItemsSource = null;
                cmbTemplate.DisplayMemberPath = "Nome";
                cmbTemplate.SelectedValuePath = "Id";
                cmbTemplate.ItemsSource = listaTemplate;
                cmbTemplate.SelectedIndex = 0;

            }
            else
                cmbTemplate.IsEnabled = false;
        }

        #endregion Métodos

        

        
    }
}




