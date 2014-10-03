using System;
using System.Collections.Generic;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Fornece estrutura de armazenamento para pacotes de hora.
    /// </summary>
    public class HoraDTO
    {
        #region Fields

        private string ultimaHora;        

        #endregion

        #region Constructor

        /// <summary>
        /// Contrutor de horaDTO
        /// </summary>
        public HoraDTO()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Construtor de horaDTO +1
        /// </summary>
        /// <param name="ultimaHora">Última hora</param>
        public HoraDTO(string ultimaHora)
        {
            this.ultimaHora = ultimaHora;
        }

        #endregion

        #region Properties

        #region UltimaHora
        /// <summary>
        /// UltimaHora
        /// </summary>
        public string UltimaHora
        {
            get { return ultimaHora; }
            set { ultimaHora = value; }
        }
        #endregion UltimaHora

        #endregion
    }
}
