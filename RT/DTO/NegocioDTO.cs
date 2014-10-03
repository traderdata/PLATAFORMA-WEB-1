using System;
using System.Collections.Generic;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Fornece estrutura de armazenamento para negócio.
    /// </summary>
    public class NegocioDTO
    {
        public string Ativo { get; set; }
        public DateTime TimeStamp { get; set; }
        public string HoraBolsa { get; set; }
        public double Quantidade { get; set; }
        public double Valor { get; set; }
        public int Numero { get; set; }
        public int CorretoraCompradora { get; set; }
        public int CorretoraVendedora { get; set; }
        public int Bolsa { get; set; }

        /// <summary>
        /// Contrutor negocioDTO
        /// </summary>
        public NegocioDTO()
        {
            Ativo = "";
            TimeStamp = new DateTime();
            HoraBolsa = "";
            Quantidade = 0;
            Valor = 0;
            Numero = 0;
            CorretoraCompradora = 0;
            CorretoraVendedora = 0;
            Bolsa = 1;
        }
    }
}
