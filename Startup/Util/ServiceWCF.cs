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
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Startup
{
    public class ServiceWCF
    {
        #region Variaveis static

        public static FreeStockChartSVC.UsuarioDTO Usuario { get; set; }
        public static int IdUsuario = -1;
        public static string LoginSistema = "TD-GRAFICO-WEB";
        public static string SenhaSistema = "tdgraficoweb00";
        public static bool VersaoDEMO = false;
        public static string LoginUsuario = "";
        public static string userHB = "";
        public static string Guids = "";
        public static EndpointAddress endPointAddressFreeStockChartPlus;
        public static BasicHttpBinding basicBind;
        public static CustomBinding customBind;
        public static CustomBinding customBindHttps;
        public static bool BovespaRT { get; set; }
        public static bool BMFRT { get; set; }
        public static string HS { get; set; }
        public static string ID { get; set; }

        #endregion

        //Constante com o endpointGrafico
        private const string endpointFreeStockChartPlus = "http://201.49.223.89/freestockplus";


        static ServiceWCF()
        {
            //Inicializa serviços
            endPointAddressFreeStockChartPlus = new EndpointAddress(endpointFreeStockChartPlus);
            basicBind = new BasicHttpBinding();
            basicBind.SendTimeout = new TimeSpan(1, 0, 0);
            basicBind.MaxReceivedMessageSize = 99999999;
            basicBind.Security.Mode = BasicHttpSecurityMode.Transport;

            customBind = new CustomBinding();
            customBind.Elements.Add(new Microsoft.Samples.GZipEncoder.GZipMessageEncodingBindingElement());
            HttpTransportBindingElement transportBindingElement = new HttpTransportBindingElement();
            transportBindingElement.MaxReceivedMessageSize = 99999999;
            customBind.Elements.Add(transportBindingElement);

            customBindHttps = new CustomBinding();
            customBindHttps.Elements.Add(new Microsoft.Samples.GZipEncoder.GZipMessageEncodingBindingElement());
            HttpsTransportBindingElement transportBindingElementhttps = new HttpsTransportBindingElement();
            transportBindingElementhttps.MaxReceivedMessageSize = 99999999;
            customBindHttps.Elements.Add(transportBindingElementhttps);
        }
            

        
    }
}
