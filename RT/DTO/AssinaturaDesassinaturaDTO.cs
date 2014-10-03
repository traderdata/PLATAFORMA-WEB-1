using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Classe de armazenamento de dados de assinatura ou desassinatura
    /// </summary>
    public class AssinaturaDesassinaturaDTO
    {
        /// <summary>
        /// Enumerador de assinatura
        /// </summary>
        public enum TipoAssinaturaEnum { Assinatura, Desassinatura };

        /// <summary>
        /// Enumerador de dado de assinatura
        /// </summary>
        public enum TipoDadoAssinaturaEnum { BookCompleto, Tick, Negocio, MiniBook, Noticia, IndiceBovespa };

        public TipoAssinaturaEnum Tipo { get; set; }
        public TipoDadoAssinaturaEnum TipoDado { get; set; }
        public bool OperacaoBemSucedida { get; set; }
        public string Mensagem { get; set; }
        public string Ativo { get; set; }

        /// <summary>
        /// Construtor AssinaturaDesassinaturaDTO
        /// </summary>
        public AssinaturaDesassinaturaDTO()
        {
            Tipo = TipoAssinaturaEnum.Assinatura;
            TipoDado = TipoDadoAssinaturaEnum.Negocio;
            OperacaoBemSucedida = false;
            Mensagem = "";
            Ativo = "";
        }
    }
}
