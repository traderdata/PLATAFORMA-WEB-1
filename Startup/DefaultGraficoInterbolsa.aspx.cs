using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Startup
{
    public partial class DefaultGraficoInterbolsa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Resgatando o usuário e validando
            string sessId = Request["sessid"];
            string debug = Request["debug"];
            string codCliente = Request["shadowCode"];
            string ativo = Request["ativo"];
            Response.Redirect("DefaultGraficoIntegracao.aspx?sessid=" + sessId + "&debug=1&codcliente=" + codCliente + "&ativo=" + ativo);
            
        }
    }
}