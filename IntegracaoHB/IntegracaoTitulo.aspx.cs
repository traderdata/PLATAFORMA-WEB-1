using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.Collections.Specialized;

namespace IntegracaoHB
{
    public partial class IntegracaoTitulo : System.Web.UI.Page
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
                        
            CookieContainer cookies = new CookieContainer();
            NameValueCollection postData = new NameValueCollection();
            postData.Add("sessid", stringHash);
            postData.Add("codcliente", codigoCliente);
            postData.Add("ativo", ativo);
            string resultPage = HttpPost("https://titulo.traderdata.com.br/DefaultGraficoIntegracaoSTech.aspx", postData, cookies);

            Response.Write(resultPage);
        }

        //function to convert string to byte array 
        private byte[] ConvertStringToByteArray(string stringToConvert)
        {
            return (new UnicodeEncoding()).GetBytes(stringToConvert);
        }

        private string HttpPost(string uri, NameValueCollection postData, CookieContainer cookies)
        {
            // request to web page
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1)";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // Set Cookies
            request.CookieContainer = cookies;

            // encode the data to POST:  
            string strPost = FormatPostString(postData);
            byte[] encodedData = Encoding.ASCII.GetBytes(strPost);
            request.ContentLength = encodedData.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(encodedData, 0, encodedData.Length);

            // execute request
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // get data via stream
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
            string output = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();

            // save cookies
            foreach (Cookie ck in response.Cookies)
                cookies.Add(ck);

            // register new cookies
            string setCookie = response.GetResponseHeader("Set-Cookie");
            if (!string.IsNullOrEmpty(setCookie))
            {
                string[] cook = setCookie.Split(new char[] { '=', ';' });

                Cookie cookie = new Cookie();
                cookie.Name = cook[0];
                cookie.Value = cook[1];
                cookie.Path = "/";
                cookie.Domain = new Uri(uri).Host;

                cookies.Add(cookie);
            }

            return output;
        }

        private string FormatPostString(NameValueCollection postData)
        {
            string strPost = string.Empty;
            for (int i = 0; i < postData.Count; i++)
            {
                if (i == 0)
                    strPost += string.Format("{0}={1}", postData.Keys[i], postData[i]);
                else
                    strPost += string.Format("&{0}={1}", postData.Keys[i], postData[i]);
            }
            return strPost;
        }
    }
    

}