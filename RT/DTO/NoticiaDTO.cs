using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Classe de armazenamento de dados de notícia
    /// </summary>
    public class NoticiaDTO
    {
        public string Agencia { get; set; }
        public DateTime Data { get; set; }
        public string Titulo { get; set; }
        public string Prioridade { get; set; }
        public string Texto { get; set; }

        /// <summary>
        /// Construtor noticiaDTO
        /// </summary>
        public NoticiaDTO()
        {
            Agencia = "";
            Data = new DateTime();
            Titulo = "";
            Prioridade = "";
            Texto = "";
        }
    }
}
