using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Classe de armazenamento de dados de ativo
    /// </summary>
    public class AtivoDTO
    {
        public string Ativo { get; set; }
        public string Empresa { get; set; }
        public EnumLocalRT.Bolsa Bolsa { get; set; }

        /// <summary>
        /// Construtor de AtivoDTO
        /// </summary>
        /// <param name="ativo">Ativo</param>
        /// <param name="empresa">Empresa</param>
        /// <param name="bolsa">Bolsa</param>
        public AtivoDTO(string ativo, string empresa, EnumLocalRT.Bolsa bolsa)
        {
            this.Ativo = ativo;
            this.Empresa = empresa;
            this.Bolsa = bolsa;
        }
        
    }
}
