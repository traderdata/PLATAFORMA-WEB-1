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
    public partial class TerminalDesktop : ChildWindow
    {

        private TraderdataDesktop.ServiceClient client = new TraderdataDesktop.ServiceClient(ServiceWCF.basicBindHTTP, ServiceWCF.endPointTerminalDesktop);

        public TerminalDesktop()
        {
            InitializeComponent();
        }

        public string CreatePassword(int length)
        {
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string res = "";
            Random rnd = new Random();
            while (0 < length--)
                res += valid[rnd.Next(valid.Length)];
            return res;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //salvando cliente
            if ((txtEmail.Text.Length == 0) || (txtNome.Text.Length == 0))
            {
                MessageBox.Show("Os dois campos são obrigatorios");
                return;
            }
            

            TraderdataDesktop.UsuarioDTO user = new TraderdataDesktop.UsuarioDTO();
            user.Bairro = "";
            user.Celular = "";
            user.CEP = "";
            user.Cidade = "";
            user.CPF = "";
            user.Endereco = "";
            user.Estado = "";
            user.Guid = "";
            user.RG = "";
            user.Telefone = "";            
            user.BMFRT = DateTime.Today.AddDays(7);
            user.BovespaRT = DateTime.Today.AddDays(30);
            user.Corretora = ServiceWCF.MacroCliente;
            user.Email = txtEmail.Text;
            user.Nome = txtNome.Text;
            user.Pais = "BR";
            user.Perfil = "B";
            user.Roteamento = false;
            user.TipoUsuario = "NP";
            user.Trial = DateTime.Today.AddDays(7);
            user.Senha = CreatePassword(6);

            client.InserirUsuarioIntegracaoCorretoraCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_InserirUsuarioIntegracaoCorretoraCompleted);
            client.InserirUsuarioIntegracaoCorretoraAsync(user);

            this.DialogResult = true;
            Close();
        }

        void client_InserirUsuarioIntegracaoCorretoraCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Enviamos uma mensagem para o e-mail informado lhe passando seu login e senha de acesso.");
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}

