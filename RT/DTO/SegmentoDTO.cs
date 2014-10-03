using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Classe de armazenamento de dados de segmento
    /// </summary>
    public class SegmentoDTO
    {
        #region Campos

        private int id;
        private string nome;

        #endregion Campos

        #region Construtores

        /// <summary>
        /// Construtor segmentoDTO
        /// </summary>
        public SegmentoDTO()
            : this(0, string.Empty)
        {
        }

        /// <summary>
        /// Construtor segmentoDTO
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="nome">Nome do segmento</param>
        public SegmentoDTO(int id, string nome)
        {
            this.id = id;
            this.nome = nome;
        }

        #endregion Construtores

        #region Propriedades

        /// <summary>
        /// Id do segmento
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Nome do segmento
        /// </summary>
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        #endregion Propriedades

       
    }
}
