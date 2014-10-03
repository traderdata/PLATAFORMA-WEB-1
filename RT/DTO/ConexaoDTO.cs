using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Classe de armazenamento de dados da conexão
    /// </summary>
    public class ConexaoDTO
    {
        public string Login { get; set; }
        public string Senha { get; set; }
        public DateTime HoraConexao { get; set; }
        public bool ConexaoBemSucedida { get; set; }
        public string Mensagem { get; set; }

        /// <summary>
        /// Construtor de conexaoDTO
        /// </summary>
        public ConexaoDTO()
        {
            Login = "";
            Senha = "";
            HoraConexao = new DateTime();
            ConexaoBemSucedida = false;
            Mensagem = "";
        }
    }
}
