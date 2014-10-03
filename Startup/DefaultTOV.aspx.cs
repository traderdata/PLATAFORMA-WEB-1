using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Startup
{
    public partial class DefaultTOV : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Resgatando o usuário e validando
            string sessId = Request["sessid"];
            string debug = Request["debug"];
            string codCliente = Request["codcliente"];
            string ativo = Request["ativo"];
            if (debug != "")
                Response.Redirect("DefaultGraficoIntegracaoSTech.aspx?sessid=" + sessId + "&debug=" + debug + "&codcliente=" + codCliente + "&ativo=" + ativo);
            else
                Response.Redirect("DefaultGraficoIntegracaoSTech.aspx?sessid=" + sessId + "&codcliente=" + codCliente + "&ativo=" + ativo);
        }
    }
}