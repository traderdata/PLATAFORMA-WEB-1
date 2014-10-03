using System;
using System.Collections.Generic;
using System.Text;
using Traderdata.Client.TerminalWEB.RT;

namespace Traderdata.Client.TerminalWEB.RT.DTO
{
    /// <summary>
    /// Fornece uma estrutura de armazenamento para comandos do book.
    /// </summary>
    public class ComandoBookDTO
    {
        #region Campos

        public enum TipoComandoEnum { Adicao, Atualizacao, Delecao, NaoDefinido };
        public enum TipoDelecaoEnum { Tipo1, Tipo2, Tipo3, NaoDefinido };

        public TipoComandoEnum TipoComando { get; set; }
        public OfertaDTO.DirecaoOfertaEnum DirecaoComando { get; set; }
        public TipoDelecaoEnum TipoDelecao { get; set; }
        public string Ativo { get; set; }
        public int Posicao { get; set; }
        public double Valor { get; set; }
        public double Quantidade { get; set; }
        public int Corretora { get; set; }
        public DateTime Data { get; set; }
        public EnumLocalRT.Bolsa Bolsa { get; set; }

        #endregion Campos

        #region Construtores

        public ComandoBookDTO()
        {
            IniciaCampos();
        }

        /// <summary>
        /// Inicia a classe com os dados do comando, em formato TraderData, passado por parâmetro.
        /// </summary>
        /// <param name="comandoTD"></param>
        public ComandoBookDTO(string comandoTD)
        {
            IniciaCamposDoComando(comandoTD);
        }

        #endregion Construtores

        #region Métodos

        #region Inicia Campos
        /// <summary>
        /// Inicia campos do DTO com valores padroes.
        /// </summary>
        public void IniciaCampos()
        {
            TipoComando = TipoComandoEnum.NaoDefinido;
            DirecaoComando = OfertaDTO.DirecaoOfertaEnum.Indefinido;
            TipoDelecao = TipoDelecaoEnum.NaoDefinido;
            Ativo = "";
            Posicao = -999;
            Valor = -1;
            Quantidade = -1;
            Corretora = -1;
            Data = new DateTime();
            Bolsa = EnumLocalRT.Bolsa.Bovespa;
        }
        #endregion Inicia Campos

        #region Inicia Campos Do Comando
        /// <summary>
        /// Inicia campos do DTO com valores do pacote desejado.
        /// </summary>
        public void IniciaCamposDoComando(string comandoTD)
        {
            //Limpando demais campos
            IniciaCampos();

            //Obtendo dados do comando
            string[] dadosComando = comandoTD.Split(':');

            //Obtendo ativo e comando
            Ativo = dadosComando[1].Trim().ToUpper();
            string comando = dadosComando[2];

            //Verificando comando
            switch (comando)
            {
                //Adicao ou Atualizacao
                //Obs: Esses dois comandos tem o mesmo formato, apenas o tipo de comando muda
                case "A":
                case "U":
                    //Obtendo Comando
                    if (comando == "A")
                        TipoComando = TipoComandoEnum.Adicao;
                    else
                        TipoComando = TipoComandoEnum.Atualizacao;

                    //Obtendo direcao
                    if (dadosComando[3] == "A")
                        DirecaoComando = OfertaDTO.DirecaoOfertaEnum.Compra;
                    else
                        DirecaoComando = OfertaDTO.DirecaoOfertaEnum.Venda;

                    //Obtendo demais valores
                    Posicao = Convert.ToInt32(dadosComando[4]);
                    Valor = Convert.ToDouble(dadosComando[5], Util.NumberProvider);
                    Quantidade = Convert.ToDouble(dadosComando[6], Util.NumberProvider);
                    Corretora = Convert.ToInt32(dadosComando[7]);

                    int dia = Convert.ToInt32(dadosComando[8].Substring(0, 2));
                    int mes = Convert.ToInt32(dadosComando[8].Substring(2, 2));
                    int ano = Convert.ToInt32(dadosComando[8].Substring(4, 4));
                    int hora = Convert.ToInt32(dadosComando[8].Substring(8, 2));
                    int minuto = Convert.ToInt32(dadosComando[8].Substring(10, 2));
                    int segundo = Convert.ToInt32(dadosComando[8].Substring(12, 2));

                    Data = new DateTime(ano, mes, dia, hora, minuto, segundo);

                    if (dadosComando[9].Trim() == "1")
                        Bolsa = EnumLocalRT.Bolsa.Bovespa;
                    else
                        Bolsa = EnumLocalRT.Bolsa.BMF;

                    break;

                //Delecao
                case "D":
                    TipoComando = TipoComandoEnum.Delecao;

                    switch (dadosComando[3])
                    {
                        case "1":
                        case "2":
                            //Obtendo tipo delecao
                            if (dadosComando[3] == "1")
                                TipoDelecao = TipoDelecaoEnum.Tipo1;
                            else
                                TipoDelecao = TipoDelecaoEnum.Tipo2;

                            //Obtendo direcao
                            if (dadosComando[4] == "A")
                                DirecaoComando = OfertaDTO.DirecaoOfertaEnum.Compra;
                            else
                                DirecaoComando = OfertaDTO.DirecaoOfertaEnum.Venda;

                            //Obtendo posicao
                            Posicao = Convert.ToInt32(dadosComando[5]);

                            if (dadosComando[6].Trim() == "1")
                                Bolsa = EnumLocalRT.Bolsa.Bovespa;
                            else
                                Bolsa = EnumLocalRT.Bolsa.BMF;
                            break;

                        case "3":
                            TipoDelecao = TipoDelecaoEnum.Tipo3;

                            if (dadosComando[4].Trim() == "1")
                                Bolsa = EnumLocalRT.Bolsa.Bovespa;
                            else
                                Bolsa = EnumLocalRT.Bolsa.BMF;
                            break;
                    }
                    break;
            }
        }
        #endregion Inicia Campos Do Comando

        #region GeraOfertaDTO
        /// <summary>
        /// Gera um DTO de oferta a partir deste DTO de comando.
        /// </summary>
        /// <returns></returns>
        public OfertaDTO GeraOfertaDTO()
        {
            OfertaDTO oferta = new OfertaDTO();

            oferta.Ativo = this.Ativo;
            oferta.Corretora = this.Corretora;
            oferta.Data = this.Data;
            oferta.DirecaoOferta = this.DirecaoComando;
            oferta.Quantidade = this.Quantidade;
            oferta.Valor = this.Valor;
            oferta.ValorTeoricoAbertura = -1;

            return oferta;
        }
        #endregion GeraOfertaDTO

        #endregion Métodos
    } 


}