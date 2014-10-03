using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Enumerador local interno
    /// </summary>
    public class EnumLocalRT
    {
        /// <summary>
        /// Enumerador de bolsa
        /// </summary>
        public enum Bolsa
        {
            Bovespa = 1, BMF = 2
        }

        /// <summary>
        /// Enumerador de Periodicidade Diaria
        /// </summary>
        public enum PeriodicidadeDiaria
        {
            Diaria = 1440, Semanal = 10080, Mensal = 43200
        }

        /// <summary>
        /// Enummerador de tipos de dados
        /// </summary>
        public enum TipoDado
        {
            Assinatura, 
            Desassinatura,
            IndiceBovespa,
            /// <summary>
            /// Tick
            /// </summary>
            Tick,
            /// <summary>
            /// Negocio
            /// </summary>
            Negocio,
            /// <summary>
            /// Hora
            /// </summary>
            Hora,
            /// <summary>
            /// ComandoBookCompleto
            /// </summary>
            ComandoBookCompleto,
            /// <summary>
            /// Desconexão
            /// </summary>
            Desconexao,
            /// <summary>
            /// Conexão
            /// </summary>
            Conexao,
            /// <summary>
            /// Erro
            /// </summary>
            Erro,
            /// <summary>
            /// PacoteTraderData
            /// </summary>
            PacoteTraderData,
            /// <summary>
            /// Noticia
            /// </summary>
            Noticia
        };
    }
}
