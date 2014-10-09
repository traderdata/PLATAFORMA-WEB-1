using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Configuration;

namespace Startup
{
    public partial class DefaultGraficoAgora : System.Web.UI.Page
    {
        //private FreeStockChartSVC.TerminalWebClient baseWebTerminal;
        protected void Page_Load(object sender, EventArgs e)
        {                        
            try
            {
                
                //ativo passado
                string ativoDireto = Request.QueryString["ativodireto"];
                if (ativoDireto == null)
                    ativoDireto = "";

                //usr = codigo do cliente sem o dígito verificador 
                string usr = Request.QueryString["usr"];

                //crc = este código é obtido após o seu login no Portal da Ágora, dê um View Source e procure por este campo, o conteúdo dele é o que deve ser enviado pelo HomeBroker
                string crc = Request.QueryString["crc"];

                //string que pula a validação da agora
                //string debug = Request.QueryString["debug"];

                //if (debug == "2507")
                //    usr = Request.QueryString["codcliente"];

                string ambiente = ConfigurationSettings.AppSettings["AMBIENTE"];

                if ((ambiente == "HML") ||(usr=="felipe"))
                {
                    //usr = ;

                    //apresentando o gráfico
                    ApresentaGrafico(ativoDireto, usr, crc);
                    return;
                }

                if ((usr == null) || (usr==""))
                {
                    usr = Request.Form["usr"];
                    crc = Request.Form["crc"];
                    ativoDireto = Request.Form["ativodireto"];
                }


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.agorainvest.com.br/forms/autenticacrc.asp?usr=" + usr + "&crc=" + crc);
                request.Method = "GET";
                request.ContentType = "text/xml; encoding='utf-8'";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string resposta = sr.ReadToEnd();

                if ((resposta == "false"))
                {
                    Response.Write("Cliente inválido.");
                }
                else
                {
                    ApresentaGrafico(ativoDireto, usr, crc);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void ApresentaGrafico(string ativoDireto, string usr, string crc)
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
            string marcadaguatexto = ConfigurationSettings.AppSettings["MARCADAGUA"];
            string marcadagualeft = ConfigurationSettings.AppSettings["MARCADAGUA-LEFT"];
            string marcadaguatop = ConfigurationSettings.AppSettings["MARCADAGUA-TOP"];
            string marcadaguasize = ConfigurationSettings.AppSettings["MARCADAGUA-SIZE"];
            string marcadaguawidth = ConfigurationSettings.AppSettings["MARCADAGUA-WIDTH"];
            string splashscreen = ConfigurationSettings.AppSettings["SPLASHSCREEN"];
            string ambiente = ConfigurationSettings.AppSettings["AMBIENTE"];
            string download = ConfigurationSettings.AppSettings["CAMINHO-DOWNLOAD"];
            string simpletrader = ConfigurationSettings.AppSettings["SIMPLETRADER"];
            string soaMD = ConfigurationSettings.AppSettings["SOAMD"];
            string ga = ConfigurationSettings.AppSettings["GA"];

            string periodicidade = "";
            if ((periodicidade == "") || (periodicidade == null))
                periodicidade = "1";
                

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

            Response.Write("<param name=\"initParams\" value=\"ga=" + ga + ",soaurlMD=" + soaMD + ",periodicidade=" + periodicidade + ",simpletrader=" + simpletrader + ",ambiente=" + ambiente + ",marcadaguawidth=" + marcadaguawidth + ",marcadaguasize=" + marcadaguasize + ",marcadaguatop=" + marcadaguatop + ",marcadagualeft=" + marcadagualeft + ",marcadaguatexto=" + marcadaguatexto + ",baseaddress=https://agora.traderdata.com.br, ativodireto=" + ativoDireto + ", alerta=" + alerta + ", dica=" + dica + ", macrocliente=" + macrocliente + ", analisecompartilhada=" + analiseComprartilhada + ", linkmanual=" + linkmanual + ", linkvisualizacao=" + linkvisualizacao + ", suporte=" + suporte + ", corfundo=" + corfundo + ", bovespart=" + bovespaRT + ", bmfrt=" + bmfRT + ", rthost=" + rthost + ", rtport=" + rtport + ", deploy=CORRETORA, title=" + titulo + ", soaurl=" + soaurl + " , usr=" + usr + ",crc=" + crc + "\"");
            Response.Write("<param name=\"onError\" value=\"onSilverlightError\" />");
            Response.Write("<param name=\"background\" value=\"white\" />");
            Response.Write("<param name=\"minRuntimeVersion\" value=\"4.0.50826.0\" />");
            Response.Write("<param name=\"windowless\" value=\"true\"/>");
            Response.Write("<param name=\"enablehtmlaccess\" value=\"true\"/>");
            Response.Write("<param name=\"autoUpgrade\" value=\"true\" />");
            Response.Write("<a href=\"http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50826.0\" style=\"text-decoration:none\">");
            Response.Write("<img src=\"http://go.microsoft.com/fwlink/?LinkId=161376\" alt=\"Get Microsoft Silverlight\" style=\"border-style:none\"/></a>");
            Response.Write("</object></body></html>");
        }
        
    }
}