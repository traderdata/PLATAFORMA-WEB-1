using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;

namespace IntegracaoHB
{
    public partial class IntegracaoInvestbolsa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //A página deve receber 2 parametros
            //CodCliente  - codigo do cliente
            //Ativo - ativo
            string codigoCliente = Request["CodCliente"];
            string ativo = Request["Ativo"];

            //montando o link em hash
            SHA256Managed hashSHA = new SHA256Managed();
            hashSHA.ComputeHash(ConvertStringToByteArray(codigoCliente + "-" + DateTime.Today.ToString("dd-MM-yyyy")));

            string stringHash = "";

            foreach (byte b in hashSHA.Hash)
            {
                stringHash += Convert.ToString(Convert.ToInt16(b)) + "-";
            }

            stringHash = stringHash.Remove(stringHash.Length - 1);

            //Redirecionando
            string link = "https://investbolsa.traderdata.com.br/DefaultGraficoIntegracaoSTech.aspx?sessid=" + stringHash + "&codcliente=" + codigoCliente + "&ativo=" + ativo;

            Response.Redirect(link);
        }

        //function to convert string to byte array 
        private byte[] ConvertStringToByteArray(string stringToConvert)
        {
            return (new UnicodeEncoding()).GetBytes(stringToConvert);
        }
    }
}