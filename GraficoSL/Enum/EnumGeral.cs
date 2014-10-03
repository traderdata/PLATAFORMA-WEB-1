using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace Traderdata.Client.Componente.GraficoSL.Enum
{
    public static class EnumGeral
    {

        public enum TipoRefresh
        {
            Layout, Dados, Tudo, TudoMantemIndicadoresEObjetos, SomenteUpdate, LayoutSemIndicadores, TudoLinha
        }

        /// <summary>
        /// Enum que identifica se o gráfico é diario ou intraday
        /// </summary>
        public enum TipoPeriodicidade
        { 
            Diario = 1, Intraday = 2
        }

        
        /// <summary>
        /// Posicao do TickBox na escala.
        /// </summary>
        public enum PosicaoTickBox
        {
            ///<summary>
            /// Posicao do tickBox na esquerda do eixo Y, se visivel
            ///</summary>
            Esquerda,
            ///<summary>
            /// Posicao do tickBox na direita do eixo Y, se visivel
            ///</summary>
            Direita,
            ///<summary>
            /// Tick box is invisible
            ///</summary>
            Nenhuma
        }

        /// <summary>
        /// OHLC tipo da série
        /// </summary>
        public enum TipoSerieOHLC
        {
            ///<summary>
            /// Abertura
            ///</summary>
            Abertura,
            ///<summary>
            /// Maximo
            ///</summary>
            Maximo,
            ///<summary>
            /// Minimo
            ///</summary>
            Minimo,
            ///<summary>
            /// Ultimo
            ///</summary>
            Ultimo,
            ///<summary>
            /// Volume
            ///</summary>
            Volume,
            /// <summary>
            /// Normalmente, refere-se a indicadores
            /// </summary>
            Desconhecido
        }

        /// <summary>
        /// Enum para posicao do info panel
        /// </summary>
        public enum InfoPanelPosicaoEnum
        {
            /// <summary>
            /// Info Panel fica escondido.
            /// </summary>
            Escondido,
            /// <summary>
            /// Info panel fica fixo onde o usuário desejar.
            /// </summary>
            Fixo,
            /// <summary>
            /// Ínfo panel aparece seguindo o mouse
            /// </summary>
            SeguindoMouse
        }

        /// <summary>
        /// Tipo de compressao para o tico do gráfico.
        /// </summary>
        public enum CompressaoTickEnum
        {
            /// <summary>
            /// Ticks serão comprimidos baseados na diferencao de tempo.
            /// </summary>
            Tempo,
            /// <summary>
            /// Ticks serão comprimidos baseados no número de ticks
            /// </summary>
            Ticks
        }

        /// <summary>
        /// Tipo do gráfico.
        /// </summary>
        public enum TipoGraficoEnum
        {
            /// <summary>
            /// Tick values
            /// </summary>
            Tick,
            /// <summary>
            /// OHLC
            /// </summary>
            OHLC
        }

        /// <summary>
        /// Objeto sob o cursor. Usado com o metodo GetObjectFromCursor()
        /// </summary>
        public enum ObjetoSobCursor
        {
            /// <summary>
            /// Left Y axis
            /// </summary>
            PanelLeftYAxis,
            /// <summary>
            /// Right Y axis
            /// </summary>
            PanelRightYAxis,
            /// <summary>
            /// Left non paintable area
            /// </summary>
            PanelLeftNonPaintableArea,
            /// <summary>
            /// Right non paitable area
            /// </summary>
            PanelRightNonPaintableArea,
            /// <summary>
            /// Panel's paintable area
            /// </summary>
            PanelPaintableArea,
            /// <summary>
            /// Panel's title bar
            /// </summary>
            PanelTitleBar,
            /// <summary>
            /// Calendar
            /// </summary>
            Calendar,
            /// <summary>
            /// Minimized panel's bar
            /// </summary>
            PanelsBar,
            /// <summary>
            /// No object
            /// </summary>
            NoObject
        }

        ///<summary>
        /// Tipo de simbolo.
        ///</summary>
        public enum TipoSimbolo
        {
            /// <summary>
            /// Compra
            /// </summary>
            Compra = 0,
            /// <summary>
            /// Venda
            /// </summary>
            Venda = 1,
            /// <summary>
            /// Saida Curta
            /// </summary>
            SaidaCurta = 2,
            /// <summary>
            /// Saida Longa
            /// </summary>
            SaidaLonga = 3,
            /// <summary>
            /// Sinal
            /// </summary>
            Sinal = 4
        }

        /// <summary>
        /// Tipo de dado do gráfico.
        /// </summary>
        public enum TipoDadoGrafico
        {
            /// <summary>
            /// Pontos
            /// </summary>
            Pontos = 1,
            ///<summary>
            /// Percentual
            ///</summary>
            Percentual = 2
        }

        /// <summary>
        /// Tipo do parâmetro usado para indicadores.
        /// </summary>
        public enum TipoParametroIndicador
        {
            /// <summary>
            /// Tipo média móvel
            /// </summary>
            TipoMediaMovel,
            /// <summary>
            /// %D Tipo média móvel
            /// </summary>
            PercentDTipoMediaMovel,
            ///<summary>
            /// Ativo
            ///</summary>
            Ativo,
            /// <summary>
            /// Série - ativo
            /// </summary>
            Serie,
            /// <summary>
            /// Série 1 - ativo
            /// </summary>
            Serie1,
            /// <summary>
            /// Série 2 - ativo
            /// </summary>
            Serie2,
            /// <summary>
            /// Série 3 - ativo
            /// </summary>
            Serie3,
            /// <summary>
            /// Volume
            /// </summary>
            Volume,
            /// <summary>
            /// Pontos ou percent
            /// </summary>
            PontosOuPercent,
            /// <summary>
            /// Periodos
            /// </summary>
            Periodos,
            /// <summary>
            /// Ciclo 1
            /// </summary>
            Ciclo1,
            /// <summary>
            /// Ciclo 2
            /// </summary>
            Ciclo2,
            /// <summary>
            /// Ciclo 3
            /// </summary>
            Ciclo3,
            /// <summary>
            /// Curto Prazo
            /// </summary>
            CurtoPrazo,
            /// <summary>
            /// Longo Prazo
            /// </summary>
            LongoPrazo,
            /// <summary>
            /// Taxa de variacao
            /// </summary>
            TaxaVariacao,
            /// <summary>
            /// %K Periodos
            /// </summary>
            PercentKPeriodos,
            /// <summary>
            /// %K Retardo
            /// </summary>
            PercentKRetardo,
            /// <summary>
            /// %D Suave
            /// </summary>
            PercentDSuave,
            /// <summary>
            /// %K Suave
            /// </summary>
            PercentKSuave,
            /// <summary>
            /// %D Double Suave
            /// </summary>
            PercentDDoubleSuave,
            /// <summary>
            /// %D Periodos
            /// </summary>
            PercentDPeriodos,
            /// <summary>
            /// %K Double Suave
            /// </summary>
            PercentKDoubleSuave,
            /// <summary>
            /// Ciclo Curto
            /// </summary>
            CicloCurto,
            /// <summary>
            /// Ciclo longo
            /// </summary>
            CicloLongo,
            /// <summary>
            /// Desvio Padrao
            /// </summary>
            DesvioPadrao,
            /// <summary>
            /// Escala E2
            /// </summary>
            EscalaR2,
            /// <summary>
            /// AF Minimo
            /// </summary>
            AFMinimo,
            /// <summary>
            /// AF Maximo
            /// </summary>
            AFMaximo,
            /// <summary>
            /// Shift
            /// </summary>
            Shift,
            /// <summary>
            /// Fator
            /// </summary>
            Fator,
            /// <summary>
            /// Periodos de Sinal
            /// </summary>
            PeriodosSinal,
            /// <summary>
            /// Valor movel limite
            /// </summary>
            ValorMovelLimite,
            /// <summary>
            /// Valor minimo tick
            /// </summary>
            ValorMinimoTick,
            /// <summary>
            /// Levels
            /// </summary>
            Levels,
            /// <summary>
            /// Barra Historica
            /// </summary>
            BarraHistorica
        }

        /// <summary>
        /// Supported indicators
        /// </summary>
        public enum IndicatorType
        {
            /// <summary>
            /// Simple Moving Average
            /// </summary>
            MediaMovelSimples,

            /// <summary>
            /// Exponential Moving Average
            /// </summary>
            MediaMovelExponencial,

            /// <summary>
            /// Time Series Moving Average
            /// </summary>
            MediaMovelSerieTempo,

            /// <summary>
            /// Triangular Moving Average
            /// </summary>
            MediaMovelTriangular,

            /// <summary>
            /// Variable Moving Average
            /// </summary>
            MediaMovelVariavel,

            /// <summary>
            /// VIDYA Moving Average
            /// </summary>
            VIDYA,

            /// <summary>
            /// Welles Wilder Smoothing
            /// </summary>
            WellesWilderSmoothing,

            /// <summary>
            /// Weighted Moving Average
            /// </summary>
            MediaMovelPonderada,

            /// <summary>
            /// Williams R
            /// </summary>
            WilliamsPctR,

            /// <summary>
            /// Williams Accumulation Dist
            /// </summary>
            AcumulacaoDistribuicaoWilliams,

            /// <summary>
            /// Volume Oscillator
            /// </summary>
            OsciladorVolume,

            /// <summary>
            /// Vertical Horizontal Filter
            /// </summary>
            FiltroVerticalHorizontal,

            /// <summary>
            /// Ultimate Oscillator
            /// </summary>
            OsciladorUltimate,

            /// <summary>
            /// True Range
            /// </summary>
            TrueRange,

            /// <summary>
            /// TRIX
            /// </summary>
            TRIX,

            /// <summary>
            /// Rainbow Oscillator
            /// </summary>
            OsciladorRainbow,

            /// <summary>
            /// Price Oscillator
            /// </summary>
            OsciladorPreco,

            /// <summary>
            /// Parabolic SAR
            /// </summary>
            SARParabólico,

            /// <summary>
            /// Momentum Oscillator
            /// </summary>
            OsciladorMomentum,

            /// <summary>
            /// MACD
            /// </summary>
            MACD,

            /// <summary>
            /// Ease Of Movement
            /// </summary>
            EaseOfMovement,

            /// <summary>
            /// Directional Movement System
            /// </summary>
            MovimentoDirecionalADX,

            /// <summary>
            /// Detrended Price Oscillator
            /// </summary>
            OsciladorDetrendedPrice,

            /// <summary>
            /// Chande Momentum Oscillator
            /// </summary>
            OsciladorChandeMomentum,

            /// <summary>
            /// Chaikin Volatility
            /// </summary>
            ChaikinVolatilidade,

            /// <summary>
            /// AroonOscillator
            /// </summary>
            Aroon,

            /// <summary>
            /// AroonOscillator Oscillator
            /// </summary>
            OsciladorAroon,

            /// <summary>
            /// Linear Regression R-Squared
            /// </summary>
            RegressaoLinearRaizQuadrada,

            /// <summary>
            /// Linear Regression Forecast
            /// </summary>
            RgressaoLinearForecast,

            /// <summary>
            /// Linear Regression Slope
            /// </summary>
            RegressaoLinearSlope,

            /// <summary>
            /// Linear Regression Intercept
            /// </summary>
            RegressaoLinearIntercept,

            /// <summary>
            /// Price Volume Trend
            /// </summary>
            TendenciaPrecoVolume,

            /// <summary>
            /// Performance Index
            /// </summary>
            IndicePerformance,

            /// <summary>
            /// Commodity Channel Index
            /// </summary>
            CommodityChannelIndex,

            /// <summary>
            /// Chaikin Money Flow
            /// </summary>
            ChaikinFluxoFinanceiro,

            /// <summary>
            /// Weighted Close
            /// </summary>
            FechamentoPonderado,

            /// <summary>
            /// Volume ROC
            /// </summary>
            VolumeROC,

            /// <summary>
            /// Typical Price
            /// </summary>
            TypicalPrice,

            /// <summary>
            /// Standard Deviation
            /// </summary>
            DesvioPadrao,

            /// <summary>
            /// Price ROC
            /// </summary>
            PriceROC,

            /// <summary>
            /// Median Price
            /// </summary>
            PrecoMedio,

            /// <summary>
            /// High Minus Low
            /// </summary>
            HighMinusLow,

            /// <summary>
            /// Bollinger Bands
            /// </summary>
            BandasBollinger,

            /// <summary>
            /// Fractal Chaos Bands
            /// </summary>
            BandasFractalChaos,

            /// <summary>
            /// High/Low Bands
            /// </summary>
            BandasMaximoMinimo,

            /// <summary>
            /// Moving Average Envelope
            /// </summary>
            MediaMovelEnvelope,

            /// <summary>
            /// Swing Index
            /// </summary>
            SwingIndex,

            /// <summary>
            /// Accumulative Swing Index
            /// </summary>
            AccumulativeSwingIndex,

            /// <summary>
            /// Comparative RSI
            /// </summary>
            IndiceForcaRelativaComparada,

            /// <summary>
            /// Mass Index
            /// </summary>
            MassIndex,

            /// <summary>
            /// Money Flow Index
            /// </summary>
            IndiceFluxoFinanceiro,

            /// <summary>
            /// Negative Volume Index
            /// </summary>
            IndiceVolumeNegativo,

            /// <summary>
            /// On Balance Volume
            /// </summary>
            OnBalanceVolume,

            /// <summary>
            /// Positive Volume Index
            /// </summary>
            IndiceVolumePositivo,

            /// <summary>
            /// Relative Strength Index
            /// </summary>
            IndiceForcaRelativa,

            /// <summary>
            /// Trade Volume Index
            /// </summary>
            TradeVolumeIndex,

            /// <summary>
            /// Stochastic Oscillator
            /// </summary>
            OsciladorEstocastico,

            /// <summary>
            /// Stochastic Momentum Index
            /// </summary>
            StochasticMomentumIndex,

            /// <summary>
            /// Fractal Chaos Oscillator
            /// </summary>
            OsciladorFractalChaos,

            /// <summary>
            /// Prime Number Oscillator
            /// </summary>
            OsciladorNumerosPrimos,

            /// <summary>
            /// Prime Number Bands
            /// </summary>
            BandasNumerosPrimos,

            /// <summary>
            /// Historical Volatility
            /// </summary>
            VolatilidadeHistorica,

            /// <summary>
            /// MACD Histogram
            /// </summary>
            MACDHistograma,

            /// <summary>
            /// An indicator whos values are populated by the user
            /// </summary>
            CustomIndicator,

            /// <summary>
            /// Unknown
            /// </summary>        
            Unknown,

            /// <summary>
            /// Indicador DIDI
            /// </summary>
            Agulhada,

            /// <summary>
            /// Indicador Keltner
            /// </summary>
            Keltner
        }

        /// <summary>
        /// Tipos de série
        /// </summary>
        public enum TipoSeriesEnum
        {
            /// <summary>
            /// Linha
            /// </summary>
            Linha,

            /// <summary>
            /// Barras de volume
            /// </summary>
            Volume,

            /// <summary>
            /// Barra
            /// </summary>
            Barra,

            /// <summary>
            /// Barra HLC
            /// </summary>
            BarraHLC,

            /// <summary>
            /// Candle Chart
            /// </summary>
            Candle,

            /// <summary>
            /// Usado internamente
            /// </summary>
            Indicador,

            /// <summary>
            /// Desconhecida
            /// </summary>
            Desconhecida
        }

        /// <summary>
        /// Estilo preço
        /// </summary>
        public enum EstiloPrecoEnum
        {
            /// <summary>
            /// Padrao
            /// </summary>
            Padrao,
            /// <summary>
            /// Point and Figure
            /// </summary>
            PontoEFigura,
            /// <summary>
            /// Renko
            /// </summary>
            Renko,
            /// <summary>
            /// Kagi
            /// </summary>
            Kagi,
            /// <summary>
            /// Three Line Break
            /// </summary>
            ThreeLineBreak,
            /// <summary>
            /// Equivolume
            /// </summary>
            EquiVolume,
            /// <summary>
            /// Equivolume Shadow
            /// </summary>
            EquiVolumeShadow,
            /// <summary>
            /// Candle Volume
            /// </summary>
            CandleVolume,
            /// <summary>
            /// Heikin Ashi
            /// xClose = (Open+High+Low+Close)/4 - Average price of the current bar
            /// xOpen = [xOpen(Previous Bar) + Close(Previous Bar)]/2 - Midpoint of the previous bar
            /// xHigh = Max(High, xOpen, xClose) - Highest value in the set
            /// xLow = Min(Low, xOpen, xClose) - Lowest value in the set 
            /// </summary>
            HeikinAshi
        }

        /// <summary>
        /// Tipo Escala
        /// </summary>
        public enum TipoEscala
        {
            /// <summary>
            /// Linear
            /// </summary>
            Linear,
            /// <summary>
            /// Semi log
            /// </summary>
            Semilog
        }

        /// <summary>
        /// Tipo alinhamento.
        /// </summary>
        public enum TipoAlinhamentoEscalaEnum
        {
            /// <summary>
            /// Lado esquerdo
            /// </summary>
            Esquerda,
            /// <summary>
            /// Lado direito
            /// </summary>
            Direita,
            /// <summary>
            /// Ambas (ainda n suportado)
            /// </summary>
            Ambas
        }

        /// <summary>
        /// Tipo da linha a ser usada para desenhar objetos.
        /// </summary>
        public enum TipoLinha
        {
            ///<summary>
            /// Solido
            ///</summary>
            Solido = 1,
            /// <summary>
            /// Tracejado
            /// </summary>
            Tracejado = 2,
            /// <summary>
            /// Pontilhado
            /// </summary>
            Pontilhado = 3,
            /// <summary>
            /// Tracejado e pontilhado
            /// </summary>
            TracejadoPontilhado = 4,
            /// <summary>
            /// Nenhum, linha fica escondida
            /// </summary>
            Nenhum = 5
        }

        
        /// <summary>
        /// Enumerador com os tipos de mensagens e ícones
        /// </summary>
        public enum Icones
        {
            Informacao = 1,
            Atencao,
            Interrogacao,
            Erro,
            Sucesso
        }

    }
}
