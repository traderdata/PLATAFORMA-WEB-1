using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace Startup
{
    public partial class VisualizarAnalise : System.Web.UI.Page
    {
        FreeStockChartSVC.TerminalWebClient baseFreeStockChartPlus;
        string DOMINIO_IMAGENS = ConfigurationSettings.AppSettings["CAMINHO-IMAGEM"];

        protected void Page_Load(object sender, EventArgs e)
        {
            baseFreeStockChartPlus = new FreeStockChartSVC.TerminalWebClient(ServiceWCF.basicBind, new System.ServiceModel.EndpointAddress(ConfigurationSettings.AppSettings["SOAURL"]));

            string guidAnalise = Request.QueryString["g"];
            int idAnalise = Convert.ToInt32(Request.QueryString["i"]);

            FreeStockChartSVC.AnaliseCompartilhadaDTO analiseResposta = baseFreeStockChartPlus.RetornaAnalisePorId(idAnalise);

            if (analiseResposta.CaminhoImagem == guidAnalise)
            {
                imgGrafico.ImageUrl = DOMINIO_IMAGENS + analiseResposta.CaminhoImagem;

                lblMensagem.Text = analiseResposta.Comentario;
                lblPublicadorData.Text = "Publicado por " + RetornaPublicadorNomePorId(analiseResposta.UsuarioId) + " em " + analiseResposta.Data.ToShortDateString();
            }
            else
            {
                lblMensagem.Text = "Esta página não deve ser carregada manualmente";
            }
        }

        private string RetornaPublicadorNomePorId(int p)
        {
            FreeStockChartSVC.UsuarioDTO publicador = baseFreeStockChartPlus.RetornaUsuarioPorId(p);
            return publicador.Nome;
            //return "";
        }
    }
}