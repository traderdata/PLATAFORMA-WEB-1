using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IntegracaoHB
{
    public partial class IntegracaoHSBC : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class decode
    {
        //tabela de caracteres base64
        private static char[] mainChMap = new char[64];
        private static byte[] mainByMap = new byte[128];

        int i = 0;

        private decode()
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {
                mainChMap[i++] = c;
            }
            for (char c = 'a'; c <= 'z'; c++) mainChMap[i++] = c;
            for (char c = '0'; c <= '9'; c++) mainChMap[i++] = c;
            mainChMap[i++] = '+'; mainChMap[i++] = '/';


            //coloquei como 0  abaixo ao inves de -1
            for (int j = 0; j < mainByMap.Length; j++) mainByMap[j] = 0;
            for (int j = 0; j < 64; j++) mainByMap[mainChMap[j]] = (byte)i;
        }

        public byte[] decodeSG(char[] cIn, int iOff, int iLen)
        {
            //valida. A string codificada tem de ter largura múltiplo de 4
            if (iLen % 4 != 0) throw new Exception("Length of Base64 encoded input string is not a multiple of 4.");

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
                int o0 = (b0 << 2) | (b1 >> 4);
                int o1 = ((b1 & 0xf) << 4) | (b2 >> 2);
                int o2 = ((b2 & 3) << 6) | b3;

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
            return decodeSG(sIn).ToString();
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

    }

}