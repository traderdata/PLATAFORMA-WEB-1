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
using System.Windows.Browser;
using Traderdata.Client.TerminalWEB.Dialog;

namespace Traderdata.Client.TerminalWEB
{
    public partial class App : Application
    {

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //setando a soa de log
            ServiceWCF.SoaLogService = "https://clientdata.traderdata.com.br/log/service.svc";
            ServiceWCF.SoaTerminalDesktop = "https://md-us/traderdatalite/TraderdataLiteService.svc";

            if (e.InitParams.Count == 0)
            {
                #region Teste Interbolsa
                ServiceWCF.DeployCorretora = true;
                ServiceWCF.userHB = "58122-3";
                ServiceWCF.Title = "HSBC";
                ServiceWCF.SoaTerminalWeb = "https://webservice.traderdata.com.br/tw-HSBC/service.svc";
                ServiceWCF.SoaMarketData = "https://app-ext.traderdata.com.br/terminal-web-sl1/service.svc";

                //ServiceWCF.RTURLHost = "https://lbrt.traderdata.inf.br/tw-realtime/request.ashx";
                //ServiceWCF.RTURLHost = "https://tickserver.traderdata.inf.br/bvsp.realtime/request.ashx";
                ServiceWCF.RTURLHost = "https://app-ext.traderdata.com.br/tw10.tick.bmfbovespa.rt/request.ashx";
                
                ServiceWCF.HS = "";
                ServiceWCF.ID = "58122-3";
                ServiceWCF.AbrirCompra = false;
                ServiceWCF.BovespaRT = false;
                ServiceWCF.BMFRT = false;
                ServiceWCF.PossuiSuporte = false;
                ServiceWCF.LinkManual = "http://www.agorainvest.com.br";
                ServiceWCF.CorFundo = "#FFA6A8AB";
                ServiceWCF.MacroCliente = "HSBC";
                ServiceWCF.LinkVisualizacao = "https://hsbc.traderdata.com.br/VisualizarAnalise.aspx";
                ServiceWCF.AnaliseCompartilhada = true;
                ServiceWCF.ApresentarDica = false;
                ServiceWCF.BMFRT = true;
                ServiceWCF.BovespaRT = true;
                ServiceWCF.Alerta = false;
                ServiceWCF.AtivoDireto = "";
                ServiceWCF.MarcaDagua = "";
                ServiceWCF.MarcaDaguaLeft = -1;
                ServiceWCF.MarcaDaguaWidth = 100;
                ServiceWCF.MarcaDaguaTop = 370;
                ServiceWCF.MarcaDaguaSize = 20;
                ServiceWCF.BaseAddress = "https://walpires.traderdata.com.br";
                ServiceWCF.Simpletrader = false;
                ServiceWCF.PeriodicidadeDireta = 1;

                //ServiceWCF.DeployCorretora = false;
                //ServiceWCF.Title = "FreeStockChart Plus";
                //ServiceWCF.SOAURL = "https://soaout.traderdata.com.br/terminal-web-varejo";
                //ServiceWCF.RTURLHost = "https://rt1.traderdata.com.br/request.ashx";
                //ServiceWCF.RTURLPort = "4502";
                //ServiceWCF.HS = "";
                //ServiceWCF.ID = "";
                //ServiceWCF.AbrirCompra = false;
                //ServiceWCF.BovespaRT = true;
                //ServiceWCF.CorFundo = "#FFA6A8AB";
                //ServiceWCF.BMFRT = true;
                //ServiceWCF.AtivoDireto = "";
                #endregion
                
            }
            else
            switch (e.InitParams["deploy"])
            {
                case "FREESTOCKPLUS":
                    ServiceWCF.DeployCorretora = false;
                    ServiceWCF.Title = "FreeStockChart Plus";
                    ServiceWCF.SoaTerminalWeb = e.InitParams["soaurl"].ToString();
                    ServiceWCF.RTURLHost = e.InitParams["rthost"].ToString();
                    ServiceWCF.HS = "";
                    ServiceWCF.ID = "a";
                    ServiceWCF.CorFundo = e.InitParams["corfundo"].ToString();
                    ServiceWCF.AbrirCompra = false;
                    ServiceWCF.BovespaRT = true;
                    ServiceWCF.BMFRT = true;
                    ServiceWCF.ApresentarDica = Convert.ToBoolean(e.InitParams["dica"].ToString());
                    if (e.InitParams.Count > 1)
                    {
                        ServiceWCF.HS = e.InitParams["hs"].ToString();
                        ServiceWCF.ID = e.InitParams["id"].ToString();

                        if (e.InitParams.Count > 3)
                        {
                            if (e.InitParams["acao"].ToString() == "C")
                                ServiceWCF.AbrirCompra = true;
                            else
                                ServiceWCF.AbrirCompra = false;
                        }
                        else
                            ServiceWCF.AbrirCompra = false;
                    }
                    else
                    {
                        ServiceWCF.HS = "";
                        ServiceWCF.ID = "";
                        ServiceWCF.AbrirCompra = false;
                    }
                    break;
                case "CORRETORA":

                    //ServiceWCF.SessID = e.InitParams["sessid"].ToString();
                    ServiceWCF.DeployCorretora = true;
                    ServiceWCF.userHB = e.InitParams["usr"].ToString();
                    ServiceWCF.Title = e.InitParams["title"].ToString();
                    ServiceWCF.SoaTerminalWeb = e.InitParams["soaurl"].ToString();
                    ServiceWCF.SoaMarketData = e.InitParams["soaurlMD"].ToString();
                    ServiceWCF.RTURLHost = e.InitParams["rthost"].ToString();
                    ServiceWCF.HS = "";
                    ServiceWCF.ID = e.InitParams["usr"].ToString();
                    
                    ServiceWCF.AbrirCompra = false;
                    ServiceWCF.BovespaRT = Convert.ToBoolean(e.InitParams["bovespart"].ToString());
                    ServiceWCF.BMFRT = Convert.ToBoolean(e.InitParams["bmfrt"].ToString());
                    ServiceWCF.PossuiSuporte = Convert.ToBoolean(e.InitParams["suporte"].ToString());
                    ServiceWCF.LinkManual = e.InitParams["linkmanual"].ToString();
                    ServiceWCF.CorFundo = e.InitParams["corfundo"].ToString();
                    ServiceWCF.LinkVisualizacao = e.InitParams["linkvisualizacao"].ToString();
                    ServiceWCF.AnaliseCompartilhada = Convert.ToBoolean(e.InitParams["analisecompartilhada"].ToString());
                    ServiceWCF.MacroCliente = e.InitParams["macrocliente"].ToString();
                    
                    ServiceWCF.ApresentarDica = Convert.ToBoolean(e.InitParams["dica"].ToString());
                    ServiceWCF.Alerta = Convert.ToBoolean(e.InitParams["alerta"].ToString());
                    ServiceWCF.AtivoDireto = e.InitParams["ativodireto"].ToString();
                    ServiceWCF.BaseAddress = e.InitParams["baseaddress"].ToString();
                    ServiceWCF.MarcaDagua = e.InitParams["marcadaguatexto"].ToString();
                    
                    ServiceWCF.MarcaDaguaLeft = Convert.ToInt32(e.InitParams["marcadagualeft"].ToString());
                    ServiceWCF.MarcaDaguaTop = Convert.ToInt32(e.InitParams["marcadaguatop"].ToString());
                    ServiceWCF.MarcaDaguaSize = Convert.ToInt32(e.InitParams["marcadaguasize"].ToString());
                    ServiceWCF.MarcaDaguaWidth = Convert.ToInt32(e.InitParams["marcadaguawidth"].ToString());
                    ServiceWCF.Ambiente = e.InitParams["ambiente"].ToString();

                    ServiceWCF.Simpletrader = Convert.ToBoolean(e.InitParams["simpletrader"].ToString());
                    ServiceWCF.PeriodicidadeDireta = Convert.ToInt32(e.InitParams["periodicidade"].ToString());
                    //ServiceWCF.Simpletrader = false;
                    //ServiceWCF.PeriodicidadeDireta = Convert.ToInt32(e.InitParams["periodicidade"].ToString());

                    break;
            }


            ServiceWCF.InicializaServiceWCF(ServiceWCF.SoaTerminalWeb, ServiceWCF.SoaMarketData, ServiceWCF.SoaLogService);

            

            //Iniciando o visual
            //checando se o serviço é o serviço do site....
            //isso nao ficou bom
            if (ServiceWCF.SoaTerminalWeb.Contains("site"))
                ServiceWCF.Site = true;
            else
                ServiceWCF.Site = false;

            
            //checando a pagina inicial
            if (e.InitParams.ContainsKey("paginainicial"))
            {
                
                if (e.InitParams["paginainicial"].ToString() == "SITE")
                {
                    ServiceWCF.ID = "a";
                    ServiceWCF.Site = false;
                    //this.RootVisual = new Site();
                }
            }
            else
            {
                
                this.RootVisual = new Principal();
            }
        }

        private void Application_Exit(object sender, EventArgs e)
        {
            //if (ServiceWCF.Usuario != null)
            //    HtmlPage.Window.Invoke("NotifyWebserviceOnClose",
            //                new[] { ServiceWCF.Usuario.Id.ToString() });

            
        }

        void client_DesconectarCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
