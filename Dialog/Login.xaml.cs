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
using Traderdata.Client.Componente.GraficoSL.DTO;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.Main;
using Traderdata.Client.TerminalWEB.Dialog;
using Traderdata.Client.TerminalWEB.DTO;
using System.ServiceModel;

namespace Traderdata.Client.TerminalWEB.Dialog
{
    public partial class Login : ChildWindow
    {
        #region Campos e Construtores

        private TerminalWebSVC.TerminalWebClient baseFreeStockChartPlus;

        public Login()
        {
            try
            {
                InitializeComponent();

                baseFreeStockChartPlus = new TerminalWebSVC.TerminalWebClient(ServiceWCF.basicBind, new EndpointAddress(ServiceWCF.SoaTerminalWeb));

                //Faz a conexão com o servidor.
                baseFreeStockChartPlus.AutenticaLoginAndPasswordCompleted += new EventHandler<TerminalWebSVC.AutenticaLoginAndPasswordCompletedEventArgs>(baseFreeStockChartPlus_AutenticaLoginAndPasswordCompleted);
                
                //Colocando o titulo
                lblTitle.Text = ServiceWCF.Title;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        
        #endregion Campos e Construtores

        #region Eventos

        ///// <summary>
        ///// Operações pós login
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        void baseFreeStockChartPlus_AutenticaLoginAndPasswordCompleted(object sender, TerminalWebSVC.AutenticaLoginAndPasswordCompletedEventArgs e)
        {
            try
            {
                ModoAguarde(false);

                if (e.Result != null)
                {
                    ServiceWCF.Usuario = e.Result;
                    ServiceWCF.userHB = e.Result.Id.ToString();
                    ServiceWCF.ID = e.Result.Email;

                    //Avaliando objeto de usuario para determinar se deve ou nao liberar Bovespa e BMF em RT
                    //Vendo se esta em periodo de trial
                    if (ServiceWCF.Usuario.DataFinalTrial >= DateTime.Today)
                    {
                        ServiceWCF.BovespaRT = true;
                        ServiceWCF.BMFRT = true;
                    }
                    else
                    {
                        if (ServiceWCF.Usuario.DataFinalBovespa >= DateTime.Today)
                            ServiceWCF.BovespaRT = true;
                        else
                            ServiceWCF.BovespaRT = false;

                        if (ServiceWCF.Usuario.DataFinalBMF >= DateTime.Today)
                            ServiceWCF.BMFRT = true;
                        else
                            ServiceWCF.BMFRT = false;

                    }

                    this.Close();
    
                }
                else
                {
                    MessageBox.Show("Login ou senha inválidos.", "Atenção", MessageBoxButton.OK);

                    txtSenha.Password = String.Empty;

                    ModoAguarde(false);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        private void btn_Enter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Hand;
        }

        private void btn_Leave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Arrow;
        }
        
        /// <summary>
        /// Inicia o login do usuário.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logar();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        /// <summary>
        /// Chama a tela de cadastro de novo usuário.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCadastro_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cadastro telaCadastro = new Cadastro(true);

                telaCadastro.Closing += (sender1, e1) =>
                {
                    if (telaCadastro.DialogResult == true)
                    {
                        // Nothing yet.
                    }
                    else
                        e1.Cancel = true;
                };

                telaCadastro.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Erro ao criar login. Contate o suporte. Detalhes do erro: " + exc.Message.ToString(), "Atenção", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Chama a tela de recuperação de senha.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkRecuperaSenha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RecuperaSenha telaRecuperaSenha = new RecuperaSenha();

                telaRecuperaSenha.Closing += (sender1, e1) =>
                {
                    if (telaRecuperaSenha.DialogResult == true)
                    {
                        // Nothing yet.
                    }
                    else
                        e1.Cancel = true;
                };

                telaRecuperaSenha.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Erro ao recuperar senha. Contate o suporte. Detalhes do erro: " + exc.Message.ToString(), "Atenção", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Evento Key Press txtSenha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    if ((txtSenha.Password != "") && (txtUser.Text != ""))
                        Logar();
                    else
                        MessageBox.Show("O(s) campo(s) usuário ou senha não foram preenchidos.", "Aviso", MessageBoxButton.OK);
                }
            }
            catch
            {
                MessageBox.Show("Erro ao logar-se.Contate o suporte.", "Atenção", MessageBoxButton.OK);
                ModoAguarde(false);
            }
        }

        /// <summary>
        /// Evento Key Press para txtUsuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUser_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    if ((txtSenha.Password != "") && (txtUser.Text != ""))
                        Logar();
                    else
                        MessageBox.Show("O(s) campo(s) usuário ou senha não foram preenchidos.", "Aviso", MessageBoxButton.OK);
                }
            }
            catch
            {
                MessageBox.Show("Erro ao logar-se.Contate o suporte.", "Atenção", MessageBoxButton.OK);
                ModoAguarde(false);
            }
        }

        #endregion Eventos

        #region Métodos

        /// <summary>
        /// Chama o método assíncrono de login.
        /// </summary>
        private void Logar()
        {
            try
            {
                ModoAguarde(true);
                baseFreeStockChartPlus.AutenticaLoginAndPasswordAsync(txtUser.Text.Trim(), txtSenha.Password.Trim());                
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
                    txtSenha.IsEnabled = false;
                    txtUser.IsEnabled = false;
                    btnLogar.IsEnabled = false;
                    btnCadastro.IsEnabled = false;
                    this.Cursor = Cursors.Wait;
                }
                else
                {
                    txtSenha.IsEnabled = true;
                    txtUser.IsEnabled = true;
                    btnLogar.IsEnabled = true;
                    btnCadastro.IsEnabled = true;
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

