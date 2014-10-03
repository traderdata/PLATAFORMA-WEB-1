using System;
using System.Collections.Generic;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Fornece estrutura para armazenar um tick.
    /// </summary>
    public class TickDTO : EventArgs
    {
        //Propriedades principais
        public string Ativo { get; set; }        
        public double Abertura { get; set; }        
        public double Maximo { get; set; }        
        public double Minimo { get; set; }        
        public double Ultimo { get; set; }        
        public double Media { get; set; }
        public double Quantidade { get; set; }
        public double Volume { get; set; }        
        public DateTime Data { get; set; }
        public string Hora { get; set; }
        public double FechamentoAnterior { get; set; }
        public double Variacao { get; set; }
        public double MelhorOfertaCompra { get; set; }
        public double MelhorOfertaVenda { get; set; }
        public int NumeroNegocio { get; set; }
        public double QuantidadeMelhorOfertaCompra { get; set; }
        public double QuantidadeMelhorOfertaVenda { get; set; }
        public double QuantidadeUltimoNegocio { get; set; }
        public int Bolsa { get; set; }
        public double VolumeMinuto { get; set; }
        public double VolumeIncremento { get; set; }
        public bool Dirty { get; set; }

        //Status das cores
        public int StatusAtivo { get; set; }
        public int StatusAbertura { get; set; }
        public int StatusMaximo { get; set; }
        public int StatusMinimo { get; set; }
        public int StatusUltimo { get; set; }
        public int StatusMedia { get; set; }
        public int StatusQuantidade { get; set; }
        public int StatusVolume { get; set; }
        public int StatusData { get; set; }
        public int StatusHora { get; set; }
        public int StatusFechamentoAnterior { get; set; }
        public int StatusVariacao { get; set; }
        public int StatusMelhorOfertaCompra { get; set; }
        public int StatusMelhorOfertaVenda { get; set; }
        public int StatusNumeroNegocio { get; set; }
        public int StatusQuantidadeMelhorOfertaCompra { get; set; }
        public int StatusQuantidadeMelhorOfertaVenda { get; set; }
        public int StatusQuantidadeUltimoNegocio { get; set; }
        public int StatusBolsa { get; set; }
        public int StatusVolumeMinuto { get; set; }

        
        //Campos que armazenam a hora da alteração
        public DateTime TimeStampAlteracaoAbertura { get; set; }
        public DateTime TimeStampAlteracaoMaximo { get; set; }
        public DateTime TimeStampAlteracaoMinimo { get; set; }
        public DateTime TimeStampAlteracaoUltimo { get; set; }
        public DateTime TimeStampAlteracaoMedia { get; set; }
        public DateTime TimeStampAlteracaoQuantidade { get; set; }
        public DateTime TimeStampAlteracaoVolume { get; set; }
        public DateTime TimeStampAlteracaoFechamentoAnterior { get; set; }
        public DateTime TimeStampAlteracaoVariacao { get; set; }
        public DateTime TimeStampAlteracaoMelhorOfertaCompra { get; set; }
        public DateTime TimeStampAlteracaoMelhorOfertaVenda { get; set; }
        public DateTime TimeStampAlteracaoNumeroNegocio { get; set; }
        public DateTime TimeStampAlteracaoQuantidadeMelhorOfertaCompra { get; set; }
        public DateTime TimeStampAlteracaoQuantidadeMelhorOfertaVenda { get; set; }
        public DateTime TimeStampAlteracaoQuantidadeUltimoNegocio { get; set; }
        public DateTime TimeStampAlteracaoVolumeMinuto { get; set; }
        
        /// <summary>
        /// Contrutor tickDTO
        /// </summary>
        public TickDTO()
        {
            Ativo = "";
            Abertura = 0;
            Maximo = 0;
            Minimo = 0;
            Ultimo = 0;
            Media = 0;
            Quantidade = 0;
            Volume = 0;
            Data = new DateTime();
            Hora = "";
            FechamentoAnterior = 0;
            Variacao = 0;
            MelhorOfertaCompra = 0;
            MelhorOfertaVenda = 0;
            NumeroNegocio = 0;
            QuantidadeMelhorOfertaCompra = 0;
            QuantidadeMelhorOfertaVenda = 0;
            QuantidadeUltimoNegocio = 0;
            Bolsa = 1;
            VolumeMinuto = 0;
            Dirty = false;

            //Status das cores de cada propriedades
             StatusAtivo = 0;
             StatusAbertura = 0;
             StatusMaximo = 0;
             StatusMinimo = 0;
             StatusUltimo = 0;
             StatusMedia = 0;
             StatusQuantidade = 0;
             StatusVolume = 0;
             StatusData = 0;
             StatusHora = 0;
             StatusFechamentoAnterior = 0;
             StatusVariacao = 0;
             StatusMelhorOfertaCompra = 0;
             StatusMelhorOfertaVenda = 0;
             StatusNumeroNegocio = 0;
             StatusQuantidadeMelhorOfertaCompra = 0;
             StatusQuantidadeMelhorOfertaVenda = 0;
             StatusQuantidadeUltimoNegocio = 0;
             StatusBolsa = 0;
             StatusVolumeMinuto = 0;



            //Setando os Datetimes Para Now
            TimeStampAlteracaoAbertura = DateTime.Now;
            TimeStampAlteracaoFechamentoAnterior = DateTime.Now;
            TimeStampAlteracaoMaximo = DateTime.Now;
            TimeStampAlteracaoMedia = DateTime.Now;
            TimeStampAlteracaoMelhorOfertaCompra = DateTime.Now;
            TimeStampAlteracaoMelhorOfertaVenda = DateTime.Now;
            TimeStampAlteracaoMinimo = DateTime.Now;
            TimeStampAlteracaoNumeroNegocio = DateTime.Now;
            TimeStampAlteracaoQuantidade = DateTime.Now;
            TimeStampAlteracaoQuantidadeMelhorOfertaCompra = DateTime.Now;
            TimeStampAlteracaoQuantidadeMelhorOfertaVenda = DateTime.Now;
            TimeStampAlteracaoQuantidadeUltimoNegocio = DateTime.Now;
            TimeStampAlteracaoUltimo = DateTime.Now;
            TimeStampAlteracaoVariacao = DateTime.Now;
            TimeStampAlteracaoVolume = DateTime.Now;
            TimeStampAlteracaoVolumeMinuto = DateTime.Now;
            
        }

        public TickDTO Clone()
        {
            TickDTO tickDTO = new TickDTO();

            //Propriedades principais
            tickDTO.Ativo = this.Ativo;        
            tickDTO.Abertura = this.Abertura;      
            tickDTO.Maximo = this.Maximo;        
            tickDTO.Minimo = this.Minimo;        
            tickDTO.Ultimo = this.Ultimo;        
            tickDTO.Media = this.Media;
            tickDTO.Quantidade = this.Quantidade;
            tickDTO.Volume = this.Volume;        
            tickDTO.Data = this.Data;
            tickDTO.Hora = this.Hora;
            tickDTO.FechamentoAnterior = this.FechamentoAnterior;
            tickDTO.Variacao = this.Variacao;
            tickDTO.MelhorOfertaCompra = this.MelhorOfertaCompra;
            tickDTO.MelhorOfertaVenda = this.MelhorOfertaVenda;
            tickDTO.NumeroNegocio = this.NumeroNegocio;
            tickDTO.QuantidadeMelhorOfertaCompra = this.QuantidadeMelhorOfertaCompra;
            tickDTO.QuantidadeMelhorOfertaVenda = this.QuantidadeMelhorOfertaVenda;
            tickDTO.QuantidadeUltimoNegocio = this.QuantidadeUltimoNegocio;
            tickDTO.Bolsa = this.Bolsa;
            tickDTO.VolumeMinuto = this.VolumeMinuto;
            tickDTO.Dirty = this.Dirty;

            //Status das cores
            tickDTO.StatusAtivo = this.StatusAtivo;
            tickDTO.StatusAbertura = this.StatusAbertura;
            tickDTO.StatusMaximo = this.StatusMaximo;
            tickDTO.StatusMinimo = this.StatusMinimo;
            tickDTO.StatusUltimo = this.StatusUltimo;
            tickDTO.StatusMedia = this.StatusMedia;
            tickDTO.StatusQuantidade = this.StatusQuantidade;
            tickDTO.StatusVolume = this.StatusVolume;
            tickDTO.StatusData = this.StatusData;
            tickDTO.StatusHora = this.StatusHora;
            tickDTO.StatusFechamentoAnterior = this.StatusFechamentoAnterior;
            tickDTO.StatusVariacao = this.StatusVariacao;
            tickDTO.StatusMelhorOfertaCompra = this.StatusMelhorOfertaCompra;
            tickDTO.StatusMelhorOfertaVenda = this.StatusMelhorOfertaVenda;
            tickDTO.StatusNumeroNegocio = this.StatusNumeroNegocio;
            tickDTO.StatusQuantidadeMelhorOfertaCompra = this.StatusQuantidadeMelhorOfertaCompra;
            tickDTO.StatusQuantidadeMelhorOfertaVenda = this.StatusQuantidadeMelhorOfertaVenda;
            tickDTO.StatusQuantidadeUltimoNegocio = this.StatusQuantidadeUltimoNegocio;
            tickDTO.StatusBolsa = this.StatusBolsa;
            tickDTO.StatusVolumeMinuto = this.StatusVolumeMinuto;
                
            //Campos que armazenam a hora da alteração
            tickDTO.TimeStampAlteracaoAbertura = this.TimeStampAlteracaoAbertura;
            tickDTO.TimeStampAlteracaoMaximo = this.TimeStampAlteracaoMaximo;
            tickDTO.TimeStampAlteracaoMinimo = this.TimeStampAlteracaoMinimo;
            tickDTO.TimeStampAlteracaoUltimo = this.TimeStampAlteracaoUltimo;
            tickDTO.TimeStampAlteracaoMedia = this.TimeStampAlteracaoMedia;
            tickDTO.TimeStampAlteracaoQuantidade = this.TimeStampAlteracaoQuantidade;
            tickDTO.TimeStampAlteracaoVolume = this.TimeStampAlteracaoVolume;
            tickDTO.TimeStampAlteracaoFechamentoAnterior = this.TimeStampAlteracaoFechamentoAnterior;
            tickDTO.TimeStampAlteracaoVariacao = this.TimeStampAlteracaoVariacao;
            tickDTO.TimeStampAlteracaoMelhorOfertaCompra = this.TimeStampAlteracaoMelhorOfertaCompra;
            tickDTO.TimeStampAlteracaoMelhorOfertaVenda = this.TimeStampAlteracaoMelhorOfertaVenda;
            tickDTO.TimeStampAlteracaoNumeroNegocio = this.TimeStampAlteracaoNumeroNegocio;
            tickDTO.TimeStampAlteracaoQuantidadeMelhorOfertaCompra = this.TimeStampAlteracaoQuantidadeMelhorOfertaCompra;
            tickDTO.TimeStampAlteracaoQuantidadeMelhorOfertaVenda = this.TimeStampAlteracaoQuantidadeMelhorOfertaVenda;
            tickDTO.TimeStampAlteracaoQuantidadeUltimoNegocio = this.TimeStampAlteracaoQuantidadeUltimoNegocio;
            tickDTO.TimeStampAlteracaoVolumeMinuto = this.TimeStampAlteracaoVolumeMinuto;


            return tickDTO;
        }
    }
}