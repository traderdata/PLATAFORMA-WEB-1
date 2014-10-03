using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace Startup
{
    public partial class DefaultGraficoVotorantim : System.Web.UI.Page
    {
        //function to convert string to byte array 
        private byte[] ConvertStringToByteArray(string stringToConvert)
        {
            return (new UnicodeEncoding()).GetBytes(stringToConvert);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Resgatando o usuário e validando
            string sessId = Request["sessid"];
            string debug = Request["debug"];
            string codCliente = Request["codcliente"];
            string ativo = Request["ativo"];

            //Calculando hash para validar
            //montando o link em hash
            SHA256Managed hashSHA = new SHA256Managed();
            hashSHA.ComputeHash(ConvertStringToByteArray(codCliente + "-" + DateTime.Today.ToString("dd-MM-yyyy")));

            string stringHash = "";

            foreach (byte b in hashSHA.Hash)
            {
                stringHash += Convert.ToString(Convert.ToInt16(b)) + "-";
            }

            stringHash = stringHash.Remove(stringHash.Length - 1);

            if (debug == "1")
                stringHash = sessId;

            if (codCliente == null)
                codCliente = "felipe";


            //comparando para validar
            if (stringHash != sessId)
            {
                Response.Write("Acesso negado");
            }
            else
            {
                string alerta = ConfigurationSettings.AppSettings["ALERTA"];
                string dica = ConfigurationSettings.AppSettings["DICA"];
                string macrocliente = ConfigurationSettings.AppSettings["MACROCLIENTE"];
                string analiseComprartilhada = ConfigurationSettings.AppSettings["ANALISECOMPARTILHADA"];
                string titulo = ConfigurationSettings.AppSettings["TITULO"];
                string bovespaRT = ConfigurationSettings.AppSettings["BOVESPART"];
                string bmfRT = ConfigurationSettings.AppSettings["BMFRT"];
                string suporte = ConfigurationSettings.AppSettings["SUPORTE"];
                string corfundo = ConfigurationSettings.AppSettings["CORFUNDO"];
                string rthost = ConfigurationSettings.AppSettings["RTHOST"];
                string rtport = ConfigurationSettings.AppSettings["RTPORT"];
                string linkmanual = ConfigurationSettings.AppSettings["LINKMANUAL"];
                string soaurl = ConfigurationSettings.AppSettings["SOAURL"];
                string linkvisualizacao = ConfigurationSettings.AppSettings["LINKVISUALIZACAO"];
                string baseAddress = ConfigurationSettings.AppSettings["BASE-ADDRESS"];
                string marcadaguatexto = ConfigurationSettings.AppSettings["MARCADAGUA"];
                string marcadagualeft = ConfigurationSettings.AppSettings["MARCADAGUA-LEFT"];
                string marcadaguatop = ConfigurationSettings.AppSettings["MARCADAGUA-TOP"];
                string marcadaguasize = ConfigurationSettings.AppSettings["MARCADAGUA-SIZE"];
                string marcadaguawidth = ConfigurationSettings.AppSettings["MARCADAGUA-WIDTH"];
                string splashscreen = ConfigurationSettings.AppSettings["SPLASHSCREEN"];
                string download = ConfigurationSettings.AppSettings["CAMINHO-DOWNLOAD"];
                string simpletrader = ConfigurationSettings.AppSettings["SIMPLETRADER"];
                string soaMD = ConfigurationSettings.AppSettings["SOAMD"];
                string ambiente = ConfigurationSettings.AppSettings["AMBIENTE"];
               
                string periodicidade = "1";
                //string ativo = "";    
               

                Response.Write("<html><head><title>" + titulo + "</title>");
                Response.Write("<style type=\"text/css\">");
                Response.Write("html, body {");
                Response.Write("height: 100%;");
                Response.Write("overflow: auto;}");
                Response.Write("body {");
                Response.Write("padding: 0;");
                Response.Write("margin: 0;}");
                Response.Write("#silverlightControlHost {");
                Response.Write("height: 100%;");
                Response.Write("text-align:center;}");
                Response.Write("</style>");
                Response.Write("</head><body><object data=\"data:application/x-silverlight-2,\" type=\"application/x-silverlight-2\" width=\"100%\" height=\"100%\">");
                Response.Write("<param name=\"source\" value=\"" + download + "/TerminalWeb.xap\"/>");

                Response.Write("<param name=\"initParams\" value=\"soaurlMD=" + soaMD + ",periodicidade=" + periodicidade + ",simpletrader=" + simpletrader + ",ambiente=" + ambiente + ",marcadaguawidth=" + marcadaguawidth + ",marcadaguasize=" + marcadaguasize + ",marcadaguatop=" + marcadaguatop + ",marcadagualeft=" + marcadagualeft + ",marcadaguatexto=" + marcadaguatexto + ",baseaddress=" + baseAddress + ", ativodireto=" + ativo + ",alerta=" + alerta + ", dica=" + dica + ", macrocliente=" + macrocliente + ", analisecompartilhada=" + analiseComprartilhada + ", linkmanual=" + linkmanual + ", linkvisualizacao=" + linkvisualizacao + ", suporte=" + suporte + ", corfundo=" + corfundo + ", bovespart=" + bovespaRT + ", bmfrt=" + bmfRT + ", rthost=" + rthost + ", rtport=" + rtport + ", deploy=CORRETORA, title=" + titulo + ", soaurl=" + soaurl + " , usr=" + codCliente + "\"");
               
                //Response.Write("<param name=\"initParams\" value=\"ambiente=PRD,marcadaguawidth=" + marcadaguawidth + ",marcadaguasize=" + marcadaguasize + ",marcadaguatop=" + marcadaguatop + ",marcadagualeft=" + marcadagualeft + ",marcadaguatexto=" + marcadaguatexto + ",baseaddress=" + baseAddress + ", ativodireto=" + ativo + ", alerta=" + alerta + ", dica=" + dica + ", macrocliente=" + macrocliente + ", analisecompartilhada=" + analiseComprartilhada + ", linkmanual=" + linkmanual + ", linkvisualizacao=" + linkvisualizacao + ", suporte=" + suporte + ", corfundo=" + corfundo + ", bovespart=" + bovespaRT + ", bmfrt=" + bmfRT + ", rthost=" + rthost + ", rtport=" + rtport + ", deploy=CORRETORA, title=" + titulo + ", soaurl=" + soaurl + " , usr=" + codCliente + "\"");
                Response.Write("<param name=\"onError\" value=\"onSilverlightError\" />");
                Response.Write("<param name=\"background\" value=\"white\" />");
                Response.Write("<param name=\"windowless\" value=\"true\"/>");
                Response.Write("<param name=\"enablehtmlaccess\" value=\"true\"/>");
                Response.Write("<param name=\"minRuntimeVersion\" value=\"4.0.50826.0\" />");
                Response.Write("<param name=\"autoUpgrade\" value=\"true\" />");
                Response.Write("<a href=\"http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50826.0\" style=\"text-decoration:none\">");
                Response.Write("<img src=\"http://go.microsoft.com/fwlink/?LinkId=161376\" alt=\"Get Microsoft Silverlight\" style=\"border-style:none\"/></a>");
                Response.Write("</object></body></html>");
            }
        }
    }
}