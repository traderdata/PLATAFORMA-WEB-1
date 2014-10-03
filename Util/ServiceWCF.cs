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

namespace Traderdata.Client.TerminalWEB.Util
{
    public class ServiceWCF
    {
        #region Variaveis static

        public static string SessID { get; set; }
        public static bool Site { get; set; }
        public static string Ambiente { get; set; }
        public static int MarcaDaguaWidth { get; set; }
        public static string MarcaDagua { get; set; }
        public static int MarcaDaguaLeft { get; set; }
        public static int MarcaDaguaTop { get; set; }
        public static int MarcaDaguaSize { get; set; }
        public static string BaseAddress { get; set; }
        public static bool Alerta { get; set; }
        public static bool ApresentarDica { get; set; }
        public static bool AnaliseCompartilhada { get; set; }
        public static string  MacroCliente { get; set; }
        public static string CorFundo { get; set; }
        public static bool DeployCorretora { get; set; }
        public static string Title { get; set; }

        public static string SoaTerminalDesktop { get; set; }
        public static string SoaLogService { get; set; }
        public static string SoaTerminalWeb { get; set; }
        public static string SoaMarketDataBVSP { get; set; }
        public static string SoaMarketDataBMF { get; set; }
        public static string SoaMarketData { get; set; }
        public static string RTURLHost { get; set; }
        public static TerminalWebSVC.UsuarioDTO Usuario { get; set; }
        public static int IdUsuario = -1;
        public static string LoginSistema = "TD-GRAFICO-WEB";
        public static string SenhaSistema = "tdgraficoweb00";
        public static bool VersaoDEMO = false;
        public static string LoginUsuario = "";
        public static string userHB = "";
        public static string Guids = "";

        public static EndpointAddress endPointTerminalDesktop;
        public static EndpointAddress endPointLogService;
        public static EndpointAddress endPointTerminalWebSVC;
        public static EndpointAddress endPointMarketDataBVSP;
        public static EndpointAddress endPointMarketDataBMF;
        public static EndpointAddress endPointMarketDataCommon;
        public static BasicHttpBinding basicBind;
        public static BasicHttpBinding basicBindHTTP;
        public static CustomBinding customBind;
        public static bool BovespaRT { get; set; }
        public static bool BMFRT { get; set; }
        public static string HS { get; set; }
        public static string ID { get; set; }
        public static bool AbrirCompra { get; set; }
        public static bool PossuiSuporte { get; set; }
        public static string LinkManual { get; set; }
        public static string LinkVisualizacao { get; set; }
        public static string AtivoDireto { get; set; }
        public static bool Simpletrader { get; set; }
        public static int PeriodicidadeDireta { get; set; }
        #endregion

        //Constante com o endpointGrafico
        private static string endpointFreeStockChartPlus;



        public static void InicializaServiceWCF(string hostTerminalWeb, string hostMarketDataService, string hostLog)
        {
            
            //Inicializa serviços
            endPointTerminalDesktop = new EndpointAddress(SoaTerminalDesktop);
            endPointLogService = new EndpointAddress(hostLog);
            endPointTerminalWebSVC = new EndpointAddress(hostTerminalWeb);
            endPointMarketDataCommon = new EndpointAddress(hostMarketDataService);
            basicBind = new BasicHttpBinding();
            basicBind.SendTimeout = new TimeSpan(1, 0, 0);
            basicBind.MaxReceivedMessageSize = 99999999;
            basicBind.Security.Mode = BasicHttpSecurityMode.Transport;

            basicBindHTTP = new BasicHttpBinding();
            basicBindHTTP.SendTimeout = new TimeSpan(1, 0, 0);
            basicBindHTTP.MaxReceivedMessageSize = 99999999;
            

            customBind = new CustomBinding();
            customBind.SendTimeout = new TimeSpan(1, 0, 0);
            customBind.ReceiveTimeout = new TimeSpan(1, 0, 0);
            customBind.Elements.Add(new Microsoft.Samples.GZipEncoder.GZipMessageEncodingBindingElement());
            HttpTransportBindingElement transportBindingElement = new HttpTransportBindingElement();
            transportBindingElement.MaxReceivedMessageSize = 99999999;
                        
            
            customBind.Elements.Add(transportBindingElement);
        }
            

        
    }
}
