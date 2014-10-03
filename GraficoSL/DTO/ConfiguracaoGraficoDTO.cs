using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.DTO
{
    public class ConfiguracaoGraficoDTO
    {
        public string Comentario { get; set; }
        public bool DarvaBox { get; set; }
        public EnumGeral.TipoLinha TipoLinhaDefault { get; set; }
        public int GrossuraLinhaDefault { get; set; }
        public Brush CorObjetoDefault { get; set; }
        public bool UsarLinhaInfinita { get; set; }
        public int NivelZoom { get; set; }
        public bool LinhaDeComando { get; set; }
        public Color CorIndicadorPadrao { get; set; }
        public Color CorSerieFilha1Padrao { get; set; }
        public Color CorSerieFilha2Padrao { get; set; }
        public int GrossuraSerieFilha1Padrao { get; set; }
        public int GrossuraSerieFilha2Padrao { get; set; }
        public EnumGeral.TipoLinha TipoLinhaSerieFilha1Padrao { get; set; }
        public EnumGeral.TipoLinha TipoLinhaSerieFilha2Padrao { get; set; }
        public Brush CorFundo { get; set; }
        public Color? CorBordaCandleAlta { get; set; }
        public Color? CorBordaCandleBaixa { get; set; }
        public Color CorCandleAlta { get; set; }
        public Color CorCandleBaixa { get; set; }
        public EnumGeral.TipoEscala TipoEscala { get; set; }
        public int PrecisaoEscala { get; set; }
        public EnumGeral.TipoAlinhamentoEscalaEnum PosicaoEscala { get; set; }
        public bool GradeHorizontal { get; set; }
        public bool GradeVertical { get; set; }
        public bool PainelInfo { get; set; }
        public EnumGeral.EstiloPrecoEnum EstiloPreco { get; set; }
        public double EstiloPrecoParam1 { get; set; }
        public double EstiloPrecoParam2 { get; set; }
        public EnumGeral.TipoSeriesEnum EstiloBarra { get; set; }
        public double EspacoADireitaGrafico { get; set; }
        public bool UsarCoresAltaBaixaVolume { get; set; }
        public bool LinhaMagnetica { get; set; }
        public string ConfiguracaoRetracement { get; set; }
        public string TipoVolume { get; set; }
        
        #region Construtor

        public ConfiguracaoGraficoDTO()
        {
            ConfiguracaoRetracement = "";
        }

        #endregion

        //#region Clone

        //public ConfiguracaoGraficoDTO Clone()
        //{
        //    ConfiguracaoGraficoDTO novaConfiguracao = new ConfiguracaoGraficoDTO();
        //    novaConfiguracao.Ativo = this.Ativo;
        //    novaConfiguracao.Periodo = this.Periodo;
        //    novaConfiguracao.DarvaBox = this.DarvaBox;
        //    novaConfiguracao.Periodicidade = this.Periodicidade;
        //    novaConfiguracao.TipoLinhaDefault = this.TipoLinhaDefault;
        //    novaConfiguracao.GrossuraLinhaDefault = this.GrossuraLinhaDefault;
        //    novaConfiguracao.CorObjetoDefault = this.CorObjetoDefault;
        //    novaConfiguracao.UsarLinhaInfinita = this.UsarLinhaInfinita;
        //    novaConfiguracao.NivelZoom = this.NivelZoom;
        //    novaConfiguracao.LinhaDeComando = this.LinhaDeComando;
        //    novaConfiguracao.CorIndicadorPadrao = this.CorIndicadorPadrao;
        //    novaConfiguracao.CorSerieFilha1Padrao = this.CorSerieFilha1Padrao;
        //    novaConfiguracao.CorSerieFilha2Padrao = this.CorSerieFilha2Padrao;
        //    novaConfiguracao.GrossuraSerieFilha1Padrao = this.GrossuraSerieFilha1Padrao;
        //    novaConfiguracao.GrossuraSerieFilha2Padrao = this.GrossuraSerieFilha2Padrao;
        //    novaConfiguracao.TipoLinhaSerieFilha1Padrao = this.TipoLinhaSerieFilha1Padrao;
        //    novaConfiguracao.TipoLinhaSerieFilha2Padrao = this.TipoLinhaSerieFilha2Padrao;
        //    novaConfiguracao.CorFundo = this.CorFundo;
        //    novaConfiguracao.CorBordaCandleAlta = this.CorBordaCandleAlta;
        //    novaConfiguracao.CorBordaCandleBaixa = this.CorBordaCandleBaixa;
        //    novaConfiguracao.CorCandleAlta = this.CorCandleAlta;
        //    novaConfiguracao.CorCandleBaixa = this.CorCandleBaixa;
        //    novaConfiguracao.TipoEscala = this.TipoEscala;
        //    novaConfiguracao.PrecisaoEscala = this.PrecisaoEscala;
        //    novaConfiguracao.PosicaoEscala = this.PosicaoEscala;
        //    novaConfiguracao.GradeHorizontal = this.GradeHorizontal;
        //    novaConfiguracao.GradeVertical = this.GradeVertical;
        //    novaConfiguracao.PainelInfo = this.PainelInfo;
        //    novaConfiguracao.EstiloPreco = this.EstiloPreco;
        //    novaConfiguracao.EstiloPrecoParam1 = this.EstiloPrecoParam1;
        //    novaConfiguracao.EstiloPrecoParam2 = this.EstiloPrecoParam2;
        //    novaConfiguracao.EstiloBarra = this.EstiloBarra;
        //    novaConfiguracao.EspacoADireitaGrafico = this.EspacoADireitaGrafico;
        //    novaConfiguracao.UsarCoresAltaBaixaVolume = this.UsarCoresAltaBaixaVolume;
        //    novaConfiguracao.LinhaMagnetica = this.LinhaMagnetica;
        //    novaConfiguracao.ConfiguracaoRetracement = this.ConfiguracaoRetracement;


        //    return novaConfiguracao;
        //}

        //#endregion

    }
}
