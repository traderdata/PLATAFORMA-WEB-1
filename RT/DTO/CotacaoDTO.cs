using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Classe de armazenamento de dados de cotação
    /// </summary>
    public class CotacaoDTO
    {
        public  string Ativo { get; set; }
        public  double Abertura { get; set; }        
        public  double Maximo { get; set; }        
        public  double Minimo { get; set; }        
        public  double Ultimo { get; set; }        
        public  double Quantidade { get; set; }        
        public  double Volume { get; set; }        
        public  DateTime Data { get; set; }
        public  bool AfterMarket { get; set; }        
        public  string Hora { get; set; }

        /// <summary>
        /// Construtor de cotacaoDTO
        /// </summary>
        /// <param name="abertura">Valor de abertura</param>
        /// <param name="maximo">Valor do máximo</param>
        /// <param name="minimo">Valor do mínimo</param>
        /// <param name="ultimo">Valor do último</param>
        /// <param name="quantidade">Quantidade</param>
        /// <param name="volume">Volume</param>
        /// <param name="data">Data</param>
        /// <param name="afterMarket">True se for afterMarket</param>
        /// <param name="hora">Hora</param>
        public CotacaoDTO(double abertura, double maximo, double minimo, double ultimo, double quantidade,
            double volume, DateTime data, bool afterMarket, string hora)
        {
            this.Abertura = abertura;
            this.Maximo = maximo;
            this.Minimo = minimo;
            this.Quantidade = quantidade;
            this.Ultimo = ultimo;
            this.Volume = volume;
            this.Data = data;
            this.AfterMarket = afterMarket;
            this.Hora = hora;
        }
    }
}
