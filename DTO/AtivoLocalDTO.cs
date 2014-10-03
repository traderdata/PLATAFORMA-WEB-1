using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Traderdata.Client.TerminalWEB.DTO
{
    public class AtivoLocalDTO
    {
        public string Ativo { get; set; }
        public string Empresa { get; set; }
        public EnumLocal.Bolsa Bolsa { get; set; }

        public AtivoLocalDTO(string ativo, string empresa, EnumLocal.Bolsa bolsa)
        {
            this.Ativo = ativo;
            this.Empresa = empresa;
            this.Bolsa = bolsa;
        }
    }
}
