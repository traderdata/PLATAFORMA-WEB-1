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
    public partial class RecuperaSenha : ChildWindow
    {
        #region Campos e Construtores

        private TerminalWebSVC.TerminalWebClient baseFreeStockChartPlus;

        public RecuperaSenha()
        {
            try
            {
                InitializeComponent();

                baseFreeStockChartPlus = new TerminalWebSVC.TerminalWebClient(ServiceWCF.basicBind, ServiceWCF.endPointTerminalWebSVC);

                baseFreeStockChartPlus.RecuperaSenhaPorEmailCompleted += new EventHandler<TerminalWebSVC.RecuperaSenhaPorEmailCompletedEventArgs>(baseFreeStockChartPlus_RecuperaSenhaPorEmailCompleted);
                baseFreeStockChartPlus.RecuperaSenhaPorCPFCompleted += new EventHandler<TerminalWebSVC.RecuperaSenhaPorCPFCompletedEventArgs>(baseFreeStockChartPlus_RecuperaSenhaPorCPFCompleted);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        
    
        

        

        void baseFreeStockChartPlus_RecuperaSenhaPorCPFCompleted(object sender, TerminalWebSVC.RecuperaSenhaPorCPFCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    if (e.Result != null)
                    {
                        ReenvioConcluido(e.Result.Email);
                    }
                    else
                    {
                        txtInfo.Text = "CPF não cadastrado";
                        stackInfo.Visibility = System.Windows.Visibility.Visible;
                        ModoAguarde(false);
                    }
                }
                else
                {
                    MessageBox.Show("Ocorreu um erro ao reenviar sua senha. Detalhes do erro: " + e.Error.ToString(), "Atenção", MessageBoxButton.OK);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ocorreu um erro ao reenviar sua senha. Detalhes do erro: " + exc.Message.ToString(), "Atenção", MessageBoxButton.OK);
            }
        }

        void baseFreeStockChartPlus_RecuperaSenhaPorEmailCompleted(object sender, TerminalWebSVC.RecuperaSenhaPorEmailCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    if (e.Result != null)
                    {
                        ReenvioConcluido(e.Result.Email);
                    }
                    else
                    {
                        txtInfo.Text = "Email não cadastrado";
                        stackInfo.Visibility = System.Windows.Visibility.Visible;
                        ModoAguarde(false);
                    }
                }
                else
                {
                    MessageBox.Show("Ocorreu um erro ao reenviar sua senha. Detalhes do erro: " + e.Error.ToString(), "Atenção", MessageBoxButton.OK);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ocorreu um erro ao reenviar sua senha. Detalhes do erro: " + exc.Message.ToString(), "Atenção", MessageBoxButton.OK);
            }
        }

        
        private void ReenvioConcluido(string resposta)
        {
            try
            {
                if (resposta != null)
                {
                    txtInfo.Text = "Senha foi enviada para: " + resposta;
                    stackInfo.Visibility = System.Windows.Visibility.Visible;

                    btnReenviarSenha.Visibility = System.Windows.Visibility.Collapsed;
                    btnRetornaLogin.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Erro ao fazer cadastro. Contate o suporte.", "Atenção", MessageBoxButton.OK);
                }

                ModoAguarde(false);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
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
        /// Faz a validação dos campos e chama o reenvio de senha.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReenviarSenha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidaCampos())
                {
                    stackInfo.Visibility = System.Windows.Visibility.Collapsed;
                    ReenviarSenha();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Evento Key Press txtSenha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void txtSenha_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Key == System.Windows.Input.Key.Enter)
        //        {
        //            if ((txtSenha.Password != "") && (txtUser.Text != ""))
        //                Logar();
        //            else
        //                MessageBox.Show("O(s) campo(s) usuário ou senha não foram preenchidos.", "Aviso", MessageBoxButton.OK);
        //        }
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Erro ao logar-se.Contate o suporte.", "Atenção", MessageBoxButton.OK);
        //        ModoAguarde(false);
        //    }
        //}

        /// <summary>
        /// Retorna para a janela de login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRetornaLogin_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// Fecha a janela de cadastro pelo botão de fechar da janela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion Eventos

        #region Métodos

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

                if (txtEmail.Text.Trim() != String.Empty)
                {
                    ValidaEMail(ref retorno);
                }
                else if (txtCPF.Text.Trim() != String.Empty)
                {
                    ValidaCPF(ref retorno);
                }
                else
                {
                    txtInfo.Text = "Pelo menos um dos campos deve ser preenchido.";
                    retorno = false;
                }

                if (!retorno)
                    stackInfo.Visibility = System.Windows.Visibility.Visible;

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
                stackInfo.Visibility = System.Windows.Visibility.Collapsed;

                lblEmail.Visibility = System.Windows.Visibility.Collapsed;
                lblCPF.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// Faz a validação do email informado
        /// </summary>
        /// <param name="erros"></param>
        /// <param name="retorno"></param>
        private void ValidaEMail(ref bool retorno)
        {
            if (!ValidadorEmail(txtEmail.Text.Trim()))
            {
                lblEmail.Visibility = System.Windows.Visibility.Visible;
                txtInfo.Text = "Campo 'Email' preenchido de forma inválida.";
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
        /// Faz a validação do CPF
        /// </summary>
        /// <param name="retorno"></param>
        private void ValidaCPF(ref bool retorno)
        {
            if (!ValidadorCPF(txtCPF.Text))
            {
                lblCPF.Visibility = System.Windows.Visibility.Visible;
                txtInfo.Text = "Campo 'CPF' preenchido de forma inválida.";
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

        #endregion Validações

        /// <summary>
        /// Chama o método assíncrono de reenvio de senhas por email ou cpf.
        /// </summary>
        private void ReenviarSenha()
        {
            try
            {
                ModoAguarde(true);

                if (txtEmail.Text.Trim() != "")
                {
                    baseFreeStockChartPlus.RecuperaSenhaPorEmailAsync(txtEmail.Text.Trim());
                }
                else
                {
                    baseFreeStockChartPlus.RecuperaSenhaPorCPFAsync(txtCPF.Text.Trim());
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        private void ModoAguarde(bool aguarde)
        {
            try
            {
                if (aguarde)
                {
                    txtEmail.IsEnabled = false;
                    txtCPF.IsEnabled = false;
                    btnReenviarSenha.IsEnabled = false;
                    this.Cursor = Cursors.Wait;
                }
                else
                {
                    txtEmail.IsEnabled = true;
                    txtCPF.IsEnabled = true;
                    btnReenviarSenha.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        #endregion Métodos
    }
}

