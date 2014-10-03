using System;
using System.Collections.Generic;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Fornece estrutura para armazenar uma oferta.
    /// </summary>
    public class OfertaDTO : EventArgs
    {
        #region Propriedades

        /// <summary>
        /// Enumerador direção oferta
        /// </summary>
        public enum DirecaoOfertaEnum { Compra, Venda, Indefinido };

        public string Ativo { get; set; }
        public double Valor { get; set; }
        public double ValorTeoricoAbertura { get; set; }
        public double Quantidade { get; set; }
        public DirecaoOfertaEnum DirecaoOferta { get; set; }
        public int Corretora { get; set; }
        public DateTime Data { get; set; }
        public string NomeCorretora { get; set; }
        public OfertaDTO OfertaSuperior { get; set; }
        public OfertaDTO OfertaInferior { get; set; }
        public System.Windows.Media.Color Cor { get; set; }
        public bool Sujo { get; set; }

        #endregion Propriedades

        #region Construtores

        /// <summary>
        /// Construtor ofertaDTO
        /// </summary>
        public OfertaDTO()
            : this(string.Empty, 0, 0, 0, DirecaoOfertaEnum.Compra, 0, new DateTime(), string.Empty, false)
        {
        }

        /// <summary>
        /// Construtor ofertaDTO+1
        /// </summary>
        /// <param name="ativo">Nome do ativo</param>
        /// <param name="valor">Valor da oferta</param>
        /// <param name="valorTeoricoAbertura">Valor teórico da abertura</param>
        /// <param name="quantidade">Quantidade</param>
        /// <param name="sentido">Sentido</param>
        /// <param name="corretora">Corretora</param>
        /// <param name="data">Data</param>
        public OfertaDTO(string ativo, double valor, double valorTeoricoAbertura, double quantidade,
                         DirecaoOfertaEnum sentido, int corretora, DateTime data, string nomeCorretora, bool sujo)
        {
            this.Ativo = ativo;
            this.Valor = valor;
            this.ValorTeoricoAbertura = valorTeoricoAbertura;
            this.Quantidade = quantidade;
            this.DirecaoOferta = sentido;
            this.Corretora = corretora;
            this.Data = data;
            this.NomeCorretora = nomeCorretora;
            this.Sujo = sujo;
        }

        #endregion Construtores


        public OfertaDTO Clone()
        {
            OfertaDTO clone  = new OfertaDTO();
            clone.Ativo = this.Ativo;
            clone.Cor = this.Cor;
            clone.Corretora = this.Corretora;
            clone.Data = this.Data;
            clone.DirecaoOferta = this.DirecaoOferta;
            clone.NomeCorretora = this.NomeCorretora;
            clone.OfertaInferior = this.OfertaInferior;
            clone.OfertaSuperior = this.OfertaSuperior;
            clone.Quantidade = this.Quantidade;
            clone.Valor = this.Valor;
            clone.ValorTeoricoAbertura = this.ValorTeoricoAbertura;
            clone.Sujo = false;

            return clone;
        }
    }
}

