using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Classe de armazenamento de dados de índice bovespa
    /// </summary>
    public class IndiceBovespaDTO
    {
        #region Campos

        private List<string> ativos = new List<string>();
        private double volume = 0;
        private double quantidade = 0;
        private double abertura = 0;
        private double negocios = 0;
        private string hora = "0000";
        private string index = "";
        private double media = 0;
        private double fechamento = 0;

        private double ultimoIndiceDia;
        private double maisAltoIndiceDia;
        private string horaMaisAltoIndiceDia = "";
        private double maisBaixoIndiceDia;
        private string horaMaisBaixoIndiceDia = "";
        private double percVarIndiceEmRelacaoUltimoAnoAnterior;
        private int numPapeisEmBaixaCarteiraIndice;
        private int numPapeisEmAltaCarteiraIndice;
        private int numPapeisSemVariacaoCarteiraIndice;
        private double variacao = 0;
        private double volumeMinuto = 0;
        private double ultimoVolumeMinuto = 0;
        private string horaMinuto = "";

        private bool pendenteAtualizacao = false;

        #endregion Campos

        #region Construtores

        /// <summary>
        /// Construtor de indiceBovespaDTO
        /// </summary>
        /// <param name="ativos"></param>
        /// <param name="volume"></param>
        /// <param name="index"></param>
        public IndiceBovespaDTO(List<string> ativos, double volume, string index)
        {
            this.ativos = ativos;
            this.volume = volume;
            this.index = index;
        }

        #endregion Construtores

        #region Propriedades

        /// <summary>
        /// Volume do índice no momento.
        /// </summary>
        public double Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        /// <summary>
        /// Ativos que compõem o índice.
        /// </summary>
        public List<string> Ativos
        {
            get { return ativos; }
            set { ativos = value; }
        }

        /// <summary>
        /// Nome do índice.
        /// </summary>
        public string Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Ultimo índice do dia.
        /// </summary>
        public double UltimoIndiceDia
        {
            get { return ultimoIndiceDia; }
            set { ultimoIndiceDia = value; }
        }

        /// <summary>
        /// Mais alto índice do dia.
        /// </summary>
        public double MaisAltoIndiceDia
        {
            get { return maisAltoIndiceDia; }
            set { maisAltoIndiceDia = value; }
        }

        /// <summary>
        /// Hora do mais alto indice do dia.
        /// </summary>
        public string HoraMaisAltoIndiceDia
        {
            get { return horaMaisAltoIndiceDia; }
            set { horaMaisAltoIndiceDia = value; }
        }

        /// <summary>
        /// Mais baixo índice do dia.
        /// </summary>
        public double MaisBaixoIndiceDia
        {
            get { return maisBaixoIndiceDia; }
            set { maisBaixoIndiceDia = value; }
        }

        /// <summary>
        /// Hora do mais baixo indice do dia.
        /// </summary>
        public string HoraMaisBaixoIndiceDia
        {
            get { return horaMaisBaixoIndiceDia; }
            set { horaMaisBaixoIndiceDia = value; }
        }

        /// <summary>
        /// Número de papéis, da carteira deste índice, que estão em baixa.
        /// </summary>
        public int NumPapeisEmBaixaCarteiraIndice
        {
            get { return numPapeisEmBaixaCarteiraIndice; }
            set { numPapeisEmBaixaCarteiraIndice = value; }
        }

        /// <summary>
        /// Número de papéis, da carteira deste índice, que estão em alta.
        /// </summary>
        public int NumPapeisEmAltaCarteiraIndice
        {
            get { return numPapeisEmAltaCarteiraIndice; }
            set { numPapeisEmAltaCarteiraIndice = value; }
        }

        /// <summary>
        /// Percentual de vairação do indice em relacao ao ultimo do ano anterior.
        /// </summary>
        public double PercVarIndiceEmRelacaoUltimoAnoAnterior
        {
            get { return percVarIndiceEmRelacaoUltimoAnoAnterior; }
            set { percVarIndiceEmRelacaoUltimoAnoAnterior = value; }
        }

        /// <summary>
        /// Numero de papeis, da carteira deste indice, que não tiveram variações.
        /// </summary>
        public int NumPapeisSemVariacaoCarteiraIndice
        {
            get { return numPapeisSemVariacaoCarteiraIndice; }
            set { numPapeisSemVariacaoCarteiraIndice = value; }
        }

        /// <summary>
        /// Quantidade negociada no indice.
        /// </summary>
        public double Quantidade
        {
            get { return quantidade; }
            set { quantidade = value; }
        }

        /// <summary>
        /// Abertura do indice.
        /// </summary>
        public double Abertura
        {
            get { return abertura; }
            set { abertura = value; }
        }

        /// <summary>
        /// Negocios do indice.
        /// </summary>
        public double Negocios
        {
            get { return negocios; }
            set { negocios = value; }
        }

        /// <summary>
        /// Hora do último tick de indice.
        /// </summary>
        public string Hora
        {
            get { return hora; }
            set { hora = value; }
        }

        /// <summary>
        /// Média do índice.
        /// </summary>
        public double Media
        {
            get { return media; }
            set { media = value; }
        }

        /// <summary>
        /// Variação
        /// </summary>
        public double Variacao
        {
            get { return variacao; }
            set { variacao = value; }
        }

        /// <summary>
        /// Hora minuto
        /// </summary>
        public string HoraMinuto
        {
            get { return horaMinuto; }
            set { horaMinuto = value; }
        }

        /// <summary>
        /// Volume minuto
        /// </summary>
        public double VolumeMinuto
        {
            get { return volumeMinuto; }
            set { volumeMinuto = value; }
        }

        /// <summary>
        /// Último volume minuto
        /// </summary>
        public double UltimoVolumeMinuto
        {
            get { return ultimoVolumeMinuto; }
            set { ultimoVolumeMinuto = value; }
        }

        /// <summary>
        /// Valor fechamento
        /// </summary>
        public double Fechamento
        {
            get { return fechamento; }
            set { fechamento = value; }
        }

        /// <summary>
        /// Pendente de atualização
        /// </summary>
        public bool PendenteAtualizacao
        {
            get { return pendenteAtualizacao; }
            set { pendenteAtualizacao = value; }
        }

        #endregion Propriedades
    }
}
