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
using Traderdata.Client.TerminalWEB.Util;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class Cadastro : ChildWindow
    {
        #region Campos e Construtores

        private TerminalWebSVC.TerminalWebClient webClient;

        private TerminalWebSVC.UsuarioDTO novoUsuario;

        private bool clienteNovo = false;

        bool devePreencher = false;

        public Cadastro(bool clienteNovo)
        {
            try
            {
                webClient = new TerminalWebSVC.TerminalWebClient(ServiceWCF.basicBind, ServiceWCF.endPointTerminalWebSVC);
                
                //Associando o novo behaviuour aos serviços
                webClient.SalvarUsuarioCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(baseFreeStockChartPlus_SalvarUsuarioCompleted);
                
                InitializeComponent();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }

            if (ServiceWCF.Usuario != null)
            {
                txtNome.Text = ServiceWCF.Usuario.Nome;

                txtCPF.IsEnabled = false;
                txtCPF.Text = ServiceWCF.Usuario.CPF;
                txtCPF.Background = new SolidColorBrush(Colors.Gray);
                txtCPF.Foreground = new SolidColorBrush(Colors.Black);

                txtEmail.IsEnabled = false;
                txtEmail.Text = ServiceWCF.Usuario.Email;
                txtEmail.Background = new SolidColorBrush(Colors.Gray);
                txtEmail.Foreground = new SolidColorBrush(Colors.Black);

                txtSenha.Password = ServiceWCF.Usuario.Senha;
                txtSenha2.Password = ServiceWCF.Usuario.Senha;

                if (!String.IsNullOrEmpty(ServiceWCF.Usuario.Endereco))
                {
                    txtCidade.Text = ServiceWCF.Usuario.Cidade;
                    txtPais.Text = ServiceWCF.Usuario.Pais;
                    txtTelefone.Text = ServiceWCF.Usuario.Telefone;

                    if (ServiceWCF.Usuario.Sexo == "M")
                        rdbSexoM.IsChecked = true;
                    else
                        rdbSexoF.IsChecked = true;

                    txtRG.Text = ServiceWCF.Usuario.Identidade;
                    txtBairro.Text = ServiceWCF.Usuario.Bairro;
                    txtCEP.Text = ServiceWCF.Usuario.CEP;
                    txtEndereco.Text = ServiceWCF.Usuario.Endereco;

                    foreach (ComboBoxItem item in cbxUF.Items)
                    {
                        if (item.Tag.ToString() == ServiceWCF.Usuario.Estado)
                        {
                            cbxUF.SelectedItem = item;
                            break;
                        }
                    }

                    txtCel.Text = ServiceWCF.Usuario.Celular;
                }
                else
                {
                    this.HasCloseButton = false;
                    devePreencher = true;
                }
            }
        }

        
        #endregion Campos e Construtores

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
        /// Executa operações pós cadastro do usuário.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void baseFreeStockChartPlus_SalvarUsuarioCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    ModoAguarde(false);

                    if (clienteNovo)
                    {
                        CadastroConcluido telaCadastroConcluido = new CadastroConcluido();

                        telaCadastroConcluido.Closing += (sender1, e1) =>
                        {
                            if (telaCadastroConcluido.DialogResult == true)
                            {
                                this.DialogResult = true;
                            }
                            else
                                e1.Cancel = true;
                        };

                        telaCadastroConcluido.Show();
                    }
                    else
                    {
                        this.DialogResult = true;
                    }
                }
                else
                {
                    MessageBox.Show("Erro ao fazer cadastro. Contate o suporte. Detalhes do erro: " + e.Error.Message.ToString(), "Atenção", MessageBoxButton.OK);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        /// <summary>
        /// Faz a validação dos campos e chama o método de cadastro de usuário.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConcluirCadastro_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)chkInvestidor.IsChecked)
                {
                    if (ValidaCampos())
                    {
                        CriaUsuarioDTO();

                        Cadastrar();
                    }
                }
                else
                    if (MessageBox.Show("Esta aplicação é direcionada a investidores individuais, o sr(a). é investidor individual?", "Confirmação", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        chkInvestidor.IsChecked = true;
                        if (ValidaCampos())
                        {
                            CriaUsuarioDTO();

                            Cadastrar();
                        }
                    }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Fecha a janela de cadastro pelo botão de fechar da janela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (devePreencher == false)
                this.DialogResult = true;
        }

        #endregion Eventos

        #region Métodos

        /// <summary>
        /// Cria um usuárioDTO para ser enviado no método de cadastro.
        /// </summary>
        private void CriaUsuarioDTO()
        {
            try
            {
                if (devePreencher)
                {
                    ServiceWCF.Usuario.Identidade = txtRG.Text.Trim();
                    ServiceWCF.Usuario.Telefone = txtTelefone.Text.Trim();
                    ServiceWCF.Usuario.Celular = txtCel.Text.Trim();

                    ServiceWCF.Usuario.Senha = txtSenha.Password.Trim();

                    if (rdbSexoM.IsChecked == true)
                        ServiceWCF.Usuario.Sexo = "M";
                    else
                        ServiceWCF.Usuario.Sexo = "F";

                    ServiceWCF.Usuario.Endereco = txtEndereco.Text.Trim();
                    ServiceWCF.Usuario.Bairro = txtBairro.Text.Trim();
                    ServiceWCF.Usuario.Cidade = txtCidade.Text.Trim();
                    ServiceWCF.Usuario.Estado = ((ComboBoxItem)cbxUF.SelectedItem).Tag.ToString();
                    ServiceWCF.Usuario.CEP = txtCEP.Text.Trim();
                    ServiceWCF.Usuario.Pais = txtPais.Text.Trim();
                }
                else
                {
                    novoUsuario = new TerminalWebSVC.UsuarioDTO();

                    novoUsuario.Nome = txtNome.Text.Trim();

                    if (rdbSexoM.IsChecked == true)
                        novoUsuario.Sexo = "M";
                    else
                        novoUsuario.Sexo = "F";

                    novoUsuario.Perfil = "U";

                    novoUsuario.CPF = txtCPF.Text.Trim();
                    novoUsuario.Identidade = txtRG.Text.Trim();
                    novoUsuario.Email = txtEmail.Text.Trim();
                    novoUsuario.Bairro = txtBairro.Text.Trim();
                    novoUsuario.Endereco = txtEndereco.Text.Trim();
                    novoUsuario.CEP = txtCEP.Text.Trim();
                    novoUsuario.Cidade = txtCidade.Text.Trim();

                    novoUsuario.Estado = ((ComboBoxItem)cbxUF.SelectedItem).Tag.ToString();

                    novoUsuario.Pais = txtPais.Text.Trim();
                    novoUsuario.Telefone = txtTelefone.Text.Trim();
                    novoUsuario.Celular = txtCel.Text.Trim();
                    novoUsuario.Senha = txtSenha.Password.Trim();
                    novoUsuario.Ativo = 1;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        /// <summary>
        /// Chama o método assíncrono de cadastro de usuário.
        /// </summary>
        private void Cadastrar()
        {
            ModoAguarde(true);

            if (devePreencher)
                webClient.SalvarUsuarioAsync(ServiceWCF.Usuario);
            else
                webClient.SalvarUsuarioAsync(novoUsuario);
        }

        #region Validações

        /// <summary>
        /// Método que valida os campos do formulário
        /// </summary>
        /// <returns></returns>
        private bool ValidaCampos()
        {
            try
            {
                bool retorno = true;

                ReiniciaCampos();

                ValidaNome(ref retorno);

                ValidaCPF(ref retorno);

                validaIdentidade(ref retorno);

                validaEndereco(ref retorno);

                validaCidade(ref retorno);

                validaBairro(ref retorno);

                validaCep(ref retorno);

                validaEstado(ref retorno);

                ValidaEMail(ref retorno);

                ValidaSenhaContraSenha(ref retorno);

                validaTelefone(ref retorno);


                if (!retorno)
                    stackErro.Visibility = System.Windows.Visibility.Visible;

                return retorno;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Faz a reinicialização dos campos (desabilita campos de erro)
        /// </summary>
        private void ReiniciaCampos()
        {
            try
            {
                stackErro.Visibility = System.Windows.Visibility.Collapsed;

                lblNome.Visibility = System.Windows.Visibility.Collapsed;
                lblCPF.Visibility = System.Windows.Visibility.Collapsed;
                lblEmail.Visibility = System.Windows.Visibility.Collapsed;
                lblSenha.Visibility = System.Windows.Visibility.Collapsed;
                lblSenha2.Visibility = System.Windows.Visibility.Collapsed;
                lblTelefone.Visibility = System.Windows.Visibility.Collapsed;
                lblRG.Visibility = System.Windows.Visibility.Collapsed;
                lblEndereco.Visibility = System.Windows.Visibility.Collapsed;
                lblCidade.Visibility = System.Windows.Visibility.Collapsed;
                lblBairro.Visibility = System.Windows.Visibility.Collapsed;
                lblCEP.Visibility = System.Windows.Visibility.Collapsed;
                lblEmail.Visibility = System.Windows.Visibility.Collapsed;
                lblUF.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Faz a validacao do nome
        /// </summary>
        /// <param name="retorno"></param>
        private void ValidaNome(ref bool retorno)
        {
            //if (txtNome.Text.Trim().Length < 8)
            //{
            //    lblNome.Visibility = System.Windows.Visibility.Visible;
            //    retorno = false;
            //}

            if (string.IsNullOrEmpty(txtNome.Text))
            {
                lblNome.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retorno"></param>
        public void validaTelefone(ref bool retorno)
        {
            try
            {
                //if (!String.IsNullOrEmpty(txtTelefone.Text.Trim()))
                //{
                //    if (txtTelefone.Text.Trim().Length > 12)
                //    {
                //        lblTelefone.Visibility = System.Windows.Visibility.Visible;
                //        retorno = false;
                //    }
                //    Int32 verificador = 0;
                //    if (!Int32.TryParse(txtTelefone.Text.Trim(), out verificador))
                //    {
                //        lblTelefone.Visibility = System.Windows.Visibility.Visible;
                //        retorno = false;
                //    }
                //}
                //else 
                //{
                //    lblTelefone.Visibility = System.Windows.Visibility.Visible;
                //}

                if (String.IsNullOrEmpty(txtTelefone.Text))
                {
                    lblTelefone.Visibility = System.Windows.Visibility.Visible;
                    retorno = false;
                }
            }
            catch(Exception exp)
            {
                //MessageBox.Show(exp.Message.ToString());
                retorno = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retorno"></param>
        public void validaIdentidade(ref bool retorno)
        {
            if (String.IsNullOrEmpty(txtRG.Text.Trim()))
            {
                lblRG.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retorno"></param>
        public void validaEndereco(ref bool retorno)
        {
            if (String.IsNullOrEmpty(txtEndereco.Text.Trim()))
            {
                lblEndereco.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retorno"></param>
        public void validaCidade(ref bool retorno)
        {
            if (String.IsNullOrEmpty(txtCidade.Text.Trim()))
            {
                lblCidade.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retorno"></param>
        public void validaBairro(ref bool retorno)
        {
            if (String.IsNullOrEmpty(txtBairro.Text.Trim()))
            {
                lblBairro.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retorno"></param>
        public void validaCep(ref bool retorno)
        {
            //if (txtCEP.Text.Trim().Length != 8)
            if (string.IsNullOrEmpty(txtCEP.Text))
            {
                 lblCEP.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retorno"></param>
        public void validaEstado(ref bool retorno)
        {
            if (cbxUF.SelectedIndex == 0)
            {
                lblUF.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
        }

        /// <summary>
        /// Faz a validação do CPF
        /// </summary>
        /// <param name="retorno"></param>
        private void ValidaCPF(ref bool retorno)
        {
            //if (!ValidadorCPF(txtCPF.Text))
            if (string.IsNullOrEmpty(txtCPF.Text))
            {
                lblCPF.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
        }

        /// <summary>
        /// Metodo que efetua a validação de CPF
        /// </summary>
        /// <param name="vrCPF"></param>
        /// <returns></returns>
        public static bool ValidadorCPF(string vrCPF)
        {
            string valor = vrCPF.Replace(".", "");
            valor = valor.Replace("-", "");

            if (valor.Length != 11)
                return false;

            bool igual = true;

            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(valor[i].ToString());

            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                    return false;
            return true;
        }

        /// <summary>
        /// Faz a validação do email informado
        /// </summary>
        /// <param name="erros"></param>
        /// <param name="retorno"></param>
        private void ValidaEMail(ref bool retorno)
        {
            //if (!ValidadorEmail(txtEmail.Text.Trim()))
            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                lblEmail.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
        }

        /// <summary>
        /// Metodo que efetua a validação de Email
        /// </summary>
        /// <param name="emailInformado"></param>
        /// <returns></returns>
        public static bool ValidadorEmail(string emailInformado)
        {
            //Verifica se tem @ e . no e-mail
            if (!emailInformado.Contains("@") || !emailInformado.Contains("."))
                return false;

            //Divide em antes e depois do @
            string[] campos = emailInformado.Split('@');

            //se tiver mais que 1 arroba, não é valido
            if (campos.Length != 2)
                return false;

            //se for menor que 4 caracteres, tá errado
            if (campos[0].Length < 3)
                return false;

            //Agora eu pego depois do arroba e divido os pontos
            if (!campos[1].Contains("."))
                return false;
            campos = campos[1].Split('.');

            //se for menor que 4, é falso
            if (campos[0].Length < 2)
                return false;

            //se chegou aqui, pode ser verdadeiro
            return true;
        }

        /// <summary>
        /// Faz a validação da senha e confirmação de senha
        /// </summary>
        /// <param name="retorno"></param>
        private void ValidaSenhaContraSenha(ref bool retorno)
        {
            if (txtSenha.Password.Trim() == String.Empty)
            {
                lblSenha.Visibility = System.Windows.Visibility.Visible;
                retorno = false;
            }
            else
            {
                if (txtSenha2.Password.Trim() == String.Empty)
                {
                    lblSenha2.Visibility = System.Windows.Visibility.Visible;
                    retorno = false;
                }
                else
                {
                    if (txtSenha.Password.Trim() != txtSenha2.Password.Trim())
                    {
                        lblSenha.Visibility = System.Windows.Visibility.Visible;
                        lblSenha2.Visibility = System.Windows.Visibility.Visible;

                        MessageBox.Show("Senha e Confirmação de Senha não conferem");
                        retorno = false;
                    }
                }
            }
        }

        #endregion Validações

        private void ModoAguarde(bool aguarde)
        {
            try
            {
                if (aguarde)
                {
                    txtNome.IsEnabled = false;
                    txtEmail.IsEnabled = false;
                    txtCPF.IsEnabled = false;
                    txtSenha.IsEnabled = false;
                    txtSenha2.IsEnabled = false;

                    btnConcluirCadastro.IsEnabled = false;
                    this.Cursor = Cursors.Wait;
                }
                else
                {
                    txtNome.IsEnabled = true;
                    txtEmail.IsEnabled = true;
                    txtCPF.IsEnabled = true;
                    txtSenha.IsEnabled = true;
                    txtSenha2.IsEnabled = true;

                    btnConcluirCadastro.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        #endregion Métodos

        private void cbxUF_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is ComboBox)) return;

            //do not handle ModifierKeys 
            if (Keyboard.Modifiers != ModifierKeys.None) return;
            if ((e.Key < Key.A) || (e.Key > Key.Z)) return;

            //Carregar a busca
            int aux = 0;

            foreach (ComboBoxItem item in cbxUF.Items)
            {
                if (item.Tag.ToString() != "XX")
                {
                    if (item.Content.ToString().StartsWith(e.Key.ToString()))
                    {
                        cbxUF.SelectedIndex = aux;
                        break;
                    }
                }
                aux++;
            }
        }

        private void txtCPF_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtCPF.Text == "000.000.000-00")
            {
                txtCPF.Text = "";
            }
        }

        private void txtCPF_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCPF.Text.Trim() == "")
            {
                txtCPF.Text = "000.000.000-00";
            }
            else
            {
                txtCPF.Text = ConverteCPF(txtCPF.Text);
            }
        }

        private void txtTelefone_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtTelefone.Text == "(00)0000-0000")
            {
                txtTelefone.Text = "";
            }
        }

        private void txtTelefone_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtTelefone.Text.Trim() == "")
            {
                txtTelefone.Text = "(00)0000-0000";
            }
            else
            {
                txtTelefone.Text = ConverteTelefone(txtTelefone.Text);
            }
        }

        private void txtCel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtCel.Text == "(00)0000-0000")
            {
                txtCel.Text = "";
            }
        }

        private void txtCel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCel.Text.Trim() == "")
            {
                txtCel.Text = "(00)0000-0000";
            }
            else
            {
                txtCel.Text = ConverteTelefone(txtCel.Text);
            }
        }

        private void txtCEP_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtCEP.Text == "00000-000")
            {
                txtCEP.Text = "";
            }
        }

        private void txtCEP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCEP.Text.Trim() == "")
            {
                txtCEP.Text = "00000-000";
            }
            else
            {
                txtCEP.Text = ConverteCEP(txtCEP.Text);
            }
        }

        private string ConverteCPF(string valor)
        {
            return valor;
        }

        private string ConverteTelefone(string valor)
        {
            return valor;
        }

        private string ConverteCEP(string valor)
        {
            return valor;
        }
    
    }
}

