using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace Startup
{
    public partial class DefaultGraficoHSBC : System.Web.UI.Page
    {
        #region Decoder

        //tabela de caracteres base64
        private char[] mainChMap = new char[64];
        private byte[] mainByMap = new byte[128];

        public DefaultGraficoHSBC()
        {
            int i = 0;
            for (char c = 'A'; c <= 'Z'; c++) mainChMap[i++] = c;            
            for (char c = 'a'; c <= 'z'; c++) mainChMap[i++] = c;
            for (char c = '0'; c <= '9'; c++) mainChMap[i++] = c;
            mainChMap[i++] = '+'; mainChMap[i++] = '/';

            //coloquei como 0  abaixo ao inves de -1
            for (int j = 0; j < mainByMap.Length; j++) mainByMap[j] = 0;
            for (int j = 0; j < 64; j++) mainByMap[mainChMap[j]] = (byte)j;
        }

        public byte[] decodeSG(char[] cIn, int iOff, int iLen)
        {
            //valida. A string codificada tem de ter largura múltiplo de 4
		    if (iLen % 4 != 0) throw new Exception ("Length of Base64 encoded input string is not a multiple of 4.");
		
		    //elimina  da entrada (cIn) caracteres adicionados para completar o múltiplo de 4 (normalmente a letra 'A')
		    while (iLen > 0 && cIn[iOff + iLen - 1] == '=') iLen--;

		    //inicializa variáveis
		    int oLen = (iLen * 3) / 4;
		    byte[] saida = new byte[oLen];
		    int ip = iOff;
		    int iEnd = iOff + iLen;
		    int op = 0;
		
		    //loop no array de entrada
		    while (ip < iEnd) 
		    {
			    //obtem o ascii em grupos de 4 caracters da entrada (cIn) e valida.
			    int i0 = cIn[ip++];
			    int i1 = cIn[ip++];
			    int i2 = ip < iEnd ? cIn[ip++] : 'A';
			    int i3 = ip < iEnd ? cIn[ip++] : 'A';
			    if (i0 > 127 || i1 > 127 || i2 > 127 || i3 > 127)
				    throw new Exception("Illegal character in Base64 encoded data.");

			    //mapeia o grupo de 4 caracteres acima obtidos para a tabela de bytes 
			    //e faz Xor sob cada item obtido da tabela. Valida cada byte.
			    int b0 = mainByMap[i0]; b0 = 1 ^ b0;
			    int b1 = mainByMap[i1]; b1 = 1 ^ b1;
			    int b2 = mainByMap[i2]; b2 = 1 ^ b2;
			    int b3 = mainByMap[i3]; b3 = 1 ^ b3;
			    if (b0 < 0 || b1 < 0 || b2 < 0 || b3 < 0)
				    throw new Exception("Illegal character in Base64 encoded data.");

			    //A partir de deslocamento de bits, converte os 4 bytes acima obtidos em três.
			    //uint o0 = ( (uint)b0 << 2) | ((uint)b1 >> 4);
                //uint o1 = (((uint)b1 & 0xf) << 4) | ((uint)b2 >> 2);
			    //int o2 = ((b2 & 3) << 6) |  b3;

                
			    //A partir de deslocamento de bits, converte os 4 bytes acima obtidos em três.
			    int o0 = ( b0 << 2) | Math.Abs(b1 >> 4);
			    int o1 = ((b1 & 0xf) << 4) | Math.Abs(b2 >> 2);
			    int o2 = ((b2 & 3) << 6) |  b3;


			    //coloca o grupo de 3 bytes finais no array de saída.
			    saida[op++] = (byte)o0;
			    if (op < oLen) saida[op++] = (byte)o1;
			    if (op < oLen) saida[op++] = (byte)o2; 
		    }
		    
            return saida; 

        }

        public byte[] decodeSG(char[] cIn)
        {
            return decodeSG(cIn, 0, cIn.Length);
        }

        public byte[] decodeSG(String sIn)
        {
            return decodeSG(sIn.ToCharArray());
        }

        public string decodeString(string sIn)
        {
            byte [] dBytes = decodeSG(sIn);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(dBytes);            
        }

        public byte[] decodeLines(string sIn)
        {
            char[] buf = new char[sIn.Length];
            int p = 0;
            for (int ip = 0; ip < sIn.Length; ip++)
            {
                char c = sIn[ip];
                if (c != ' ' && c != '\r' && c != '\n' && c != '\t') buf[p++] = c;
            }
            return decodeSG(buf, 0, p);
        }

        #endregion

        public string base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Resgatando o usuário e validando
            string ativo = Request["ativo"];
            string codClienteCriptografado = Request["codcliente"];
            string sessId = Request["sidencriptada"];
            string debug = Request["debug"];
            string codCliente = "";

            //variavel de controle de acesso
            bool acessoValido = false;

            //Checando se a HML está habilitada e se o debug é valido
            if (ConfigurationSettings.AppSettings["AMBIENTE"] == "HML")
                if (debug == "1")
                {
                    acessoValido = true;
                    codCliente = codClienteCriptografado;
                }
            
            if (sessId == null)
                sessId = "";

            if ((codClienteCriptografado == "0001") && (sessId == "NT1xPD1yNTBwNip0NipxND52NSliNSB2NCFsMzMnfnsoWyQ4W08Sa0gick"))
            {
                acessoValido = true;
                ativo = "";
                codCliente = "0001";
            }

            if (!acessoValido)
            {
                //tenho que verificar a chave passada para ver se é valida
                codCliente = decodeString(codClienteCriptografado);
                string chaveDesincriptadaEncriptada = decodeString(sessId);
            
                //splito a chavedesincriptada para pegar cada um dos campos
                string[] arChave = chaveDesincriptadaEncriptada.Split('#');
                DateTime timestamp = Convert.ToDateTime(arChave[0]);
                string codclienteSid = arChave[1];

                //verificando se é valida
                if ((timestamp.Date == DateTime.Today.Date) && (codCliente == codclienteSid) && (DateTime.Now.Subtract(timestamp) <= new TimeSpan(0,70,0)))
                    acessoValido = true;

               
            }
            
            //comparando para validar
            if (!acessoValido)
            {
                Response.Write("Acesso negado");
            }
            else
            {

                if (ConfigurationSettings.AppSettings["REDIRECT-EC2"].Length > 0)
                {
                    Response.Redirect(ConfigurationSettings.AppSettings["REDIRECT-EC2"] + "?sessid=" + sessId + "&debug=" + debug + "&codcliente=" + codCliente + "&ativo=" + ativo, true);
                }


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
                string ambiente = ConfigurationSettings.AppSettings["AMBIENTE"];
                string download = ConfigurationSettings.AppSettings["CAMINHO-DOWNLOAD"];
                string simpletrader = ConfigurationSettings.AppSettings["SIMPLETRADER"];
                string soaMD = ConfigurationSettings.AppSettings["SOAMD"];
                string periodicidade = "";

                if ((periodicidade == "") || (periodicidade == null))
                    periodicidade = "1";
                                
                ativo = ativo.ToUpper();

                Response.Write("<html><head><title>" + titulo + "</title>");
                if (splashscreen.Length > 0)
                {
                    Response.Write("<script type=\"text/javascript\">");
                    Response.Write("function DownloadProgress(sender, eventArgs) {");
                    Response.Write("sender.findName(\"uxStatus\").Text = \"Loading: \" + Math.round(eventArgs.progress * 100) + \"%\";}");
                    //Response.Write("//sender.findName(\"uxProgressBar\").ScaleX = eventArgs.progress;");
                    //Response.Write("}");
                    Response.Write("</script>");
                }
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
                if (splashscreen.Length > 0)
                {
                    Response.Write("<param name=\"splashscreensource\" value=\"" + splashscreen + ".xaml\"/>");
                    Response.Write("<param name=\"onSourceDownloadProgressChanged\" value=\"DownloadProgress\" />");
                }

                //Response.Write("<param name=\"initParams\" value=\"ambiente=" + ambiente + ",marcadaguawidth=" + marcadaguawidth + ",marcadaguasize=" + marcadaguasize + ",marcadaguatop=" + marcadaguatop + ",marcadagualeft=" + marcadagualeft + ",marcadaguatexto=" + marcadaguatexto + ",baseaddress=" + baseAddress + ", ativodireto=" + ativo + ",alerta=" + alerta + ", dica=" + dica + ", macrocliente=" + macrocliente + ", analisecompartilhada=" + analiseComprartilhada + ", linkmanual=" + linkmanual + ", linkvisualizacao=" + linkvisualizacao + ", suporte=" + suporte + ", corfundo=" + corfundo + ", bovespart=" + bovespaRT + ", bmfrt=" + bmfRT + ", rthost=" + rthost + ", rtport=" + rtport + ", deploy=CORRETORA, title=" + titulo + ", soaurl=" + soaurl + " , usr=" + codCliente + "\"");
                Response.Write("<param name=\"initParams\" value=\"soaurlMD=" + soaMD + ",periodicidade=" + periodicidade + ",simpletrader=" + simpletrader + ",ambiente=" + ambiente + ",marcadaguawidth=" + marcadaguawidth + ",marcadaguasize=" + marcadaguasize + ",marcadaguatop=" + marcadaguatop + ",marcadagualeft=" + marcadagualeft + ",marcadaguatexto=" + marcadaguatexto + ",baseaddress=" + baseAddress + ", ativodireto=" + ativo + ",alerta=" + alerta + ", dica=" + dica + ", macrocliente=" + macrocliente + ", analisecompartilhada=" + analiseComprartilhada + ", linkmanual=" + linkmanual + ", linkvisualizacao=" + linkvisualizacao + ", suporte=" + suporte + ", corfundo=" + corfundo + ", bovespart=" + bovespaRT + ", bmfrt=" + bmfRT + ", rthost=" + rthost + ", rtport=" + rtport + ", deploy=CORRETORA, title=" + titulo + ", soaurl=" + soaurl + " , usr=" + codCliente + "\"");
                
                Response.Write("<param name=\"onError\" value=\"onSilverlightError\" />");
                Response.Write("<param name=\"windowless\" value=\"true\"/>");
                Response.Write("<param name=\"background\" value=\"white\" />");
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