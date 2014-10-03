using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Label=Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Label;

#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils;
#endif

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    ///<summary>
    ///</summary>
    public class YAxisCanvas : Canvas
    {
        #region Campos e Construtores

#if WPF
            private bool _mouseRightButtonDown;
#endif

        private double _min;
        private double _max;
        internal ChartPanel _chartPanel;
        internal bool _isLeftAligned;
        internal bool _painted;
        private bool _resizing;
        private bool _moving;

        //Variaveis para capturar duplo click na escala
        private DateTime horaClick = new DateTime();
        private int click = 0;

        private readonly PaintObjectsManager<Line> _lines = new PaintObjectsManager<Line>();
        private readonly PaintObjectsManager<Label> _labels = new PaintObjectsManager<Label>();
        
        //Variavies de controle
        private bool canResizeScale = true;

       
        ///<summary>
        /// Constructor
        ///</summary>
        public YAxisCanvas()
        {
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
        #if WPF
          MouseRightButtonDown += OnMouseRightButtonDown;
          MouseRightButtonUp += OnMouseRightButtonUp;
        #endif
        #if SILVERLIGHT
            Mouse.RegisterMouseMoveAbleElement(this);
            MouseMove += (sender, e) => Mouse.UpdateMousePosition(this, e.GetPosition(this));
        #endif
        }

        #endregion Campos e Construtores

        #region Propriedades

        internal int LabelCount { set; get; }
        internal double GridStep { set; get; }

        /// <summary>
        /// Nova propriedade que permite ou n resize da escala.
        /// </summary>
        public bool CanResizeScale
        {
            get { return canResizeScale; }
            set { canResizeScale = value; }
        }


        #endregion Propriedades

        #region Eventos

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (canResizeScale)
            {
                ReleaseMouseCapture();
                Cursor = Cursors.Arrow;

                if (_resizing) _chartPanel.StopYResize(this);
                if (_moving) _chartPanel.StopYMoveUpDown(this);
            }
        }

        /// <summary>
        /// Click Esquerdo na Escala
        /// Inicia o resize.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canResizeScale)
            {
                if (DateTime.Now.Subtract(horaClick).TotalSeconds > 0.6)
                    click = 0;

                click++;

                CaptureMouse();
                Cursor = Cursors.SizeNS;

                //Resetando a escala com duplo click
                if ((DateTime.Now.Subtract(horaClick).TotalSeconds < 0.6) && (click == 2))
                    _chartPanel._chartX.ResetYScale(_chartPanel.Index);

                _resizing = _moving = false;
                if (!_chartPanel._chartX._ctrlDown)
                {
                    _chartPanel.StartYResize(this);
                    _resizing = true;
                }
                else
                {
                    _chartPanel.StartYMoveUpDown(this);
                    _moving = true;
                }

                horaClick = DateTime.Now;

                if (click >= 2)
                    click = 0;
            }
        }

#if WPF
    private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (_mouseRightButtonDown)
        ReleaseMouseCapture();
      _mouseRightButtonDown = false;
      Cursor = Cursors.Arrow;
      _chartPanel.StopYMoveUpDown(this);
    }

    private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      _mouseRightButtonDown = true;
      CaptureMouse();
      Cursor = Cursors.ScrollNS;
      _chartPanel.StartYMoveUpDown(this);
    }
#endif

        #endregion Eventos

        #region Métodos

        #region SetMinMax()
        /// <summary>
        /// Seta o máximo e o mínimo da escala.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        internal void SetMinMax(double min, double max)
        {
            SetMinMax(min, max, true);
        }
        #endregion SetMinMax()

        #region SetMinMax(+1)
        /// <summary>
        /// Seta o máximo e o mínim da escala, permitindo renderizar ou não.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="render"></param>
        internal void SetMinMax(double min, double max, bool render)
        {
            _min = min;
            _max = max;
            _painted = false;

            if (render)
                Render();
        }
        #endregion SetMinMax(+1)

        #region Render()
        /// <summary>
        /// Renderiza a escala
        /// </summary>
        internal void Render()
        {
            _lines.C = this;
            _labels.C = this;
            _lines.Start();
            _labels.Start();

            //Obtendo proporcao da escala
            Rect rcBounds = new Rect(0, 0, ActualWidth, ActualHeight);
            
            //Verificando se está em modo designer
            if (Utils.GetIsInDesignMode(this))
            {
                _min = 0;
                _max = 1;
            }

            //A escala deve possuir height maior que 2 para ser renderizada
            if (rcBounds.Height < 2) 
                return;

            //Se o painel possui volume, a precisao da escala fica em 0 casas decimais, senão obtenho o padrao atual
            int decimals = _chartPanel._hasVolume ? 0 : _chartPanel._chartX.EscalaPrecisao;

            //String de formato da escala
            string formatString = _chartPanel.FormatYValueString;

            //Verificando se é um painel de volume
            bool isVolume = (decimals == 0) && (_chartPanel._chartX.SufixoVolume.Length > 0);

            //Obtendo o tamanho para cada label
            double k = rcBounds.Height / LabelCount;

            //Aplicando correção para vzlores minimos igual a zero, quando a escala está em log
            if ((_min == 0) && (_chartPanel._chartX.escalaTipo == EnumGeral.TipoEscala.Semilog))
                _min = 1;

            //Se a escala é linear, devo setar o valor min, se for log devo fazer o log do min
            double min = _chartPanel._chartX.escalaTipo == EnumGeral.TipoEscala.Linear ? _min : Math.Log10(_min);

            //Atenção: este procedimento evita que sejam gerados vários NaN nos labels, porem a parte negativa fica como linear
            //if ((double.IsNaN(min)) && (_chartPanel._chartX.escalaTipo == EnumGeral.TipoEscala.Semilog))
            //    min = _min;

            //Se a escala é linear, devo setar o valor max, se for log devo fazer o log do max
            double max = _chartPanel._chartX.escalaTipo == EnumGeral.TipoEscala.Linear ? _max : Math.Log10(_max);

            //??
            double startValue = min + (max - min) * (_chartPanel._yOffset / rcBounds.Height);

            //Obtendo intervalos da grid??
            GridStep = (max - min) / LabelCount;

            //Inicia a renderizacao da grid
            _chartPanel.StartPaintingYGridLines();

            //if (_chartPanel.Index == 0)
            //  Debug.WriteLine(string.Format("Y Height = {0}, Min = {1}, Max = {2}, Count = {3}", rcBounds.Height, _min, _min + (GridStep * LabelCount), LabelCount));

            #region Obtendo labels da escala

            StringBuilder stringBuilder = new StringBuilder();

            //Iterando pela quantidade de labels para gerá-los
            for (int i = 0; i < LabelCount; i++)
            {
                //Obtendo a posicao vertical do label
                double y = rcBounds.Height - (i * k);

                //Verificando se a posicao é invalida
                if (double.IsNaN(y)) 
                    continue;

                //Se a posicao for menor que zero, encerra a geracao dos labels
                if (y < 0)
                    break;

                //Se não for o primeiro label, devo traca-se uma linha separadora entre os valores da escala (labels)
                if (i > 0)
                {
                    Utils.DrawLine(_isLeftAligned ? rcBounds.Width - 10 : 0, y, _isLeftAligned ? rcBounds.Width : 10, y,
                      _chartPanel._chartX.GradeCor, EnumGeral.TipoLinha.Solido, 1, _lines);
                    _chartPanel.PaintYGridLine(y);
                }

                stringBuilder.Length = 0;

                //Obtendo valor que será mostrado no label
                double value = startValue + (GridStep * i);

                //Se a escala for log, devo calculor o log do valor obtido
                if (_chartPanel._chartX.escalaTipo == EnumGeral.TipoEscala.Semilog)
                    value = Math.Pow(10, value);

                //Se for um volume, o valor deve ser dividido pelo divisor de volume atual
                if (isVolume)
                    value /= _chartPanel._chartX.VolumeDivisor;

                //Guardando valor no strin builder
                stringBuilder.AppendFormat(formatString, value);

                //Ainda se for volume, devo guardar o sufixo do volume
                if (isVolume)
                    stringBuilder.Append(" ").Append(_chartPanel._chartX.SufixoVolume);

                //Gerando o label atual e setando valores
                Label tb = _labels.GetPaintObject();
                tb.Text = stringBuilder.ToString();

                //Setando o espaco à esquerda do label:
                //Se estiver alinhado à esquerda, a propriedade Left deve receber a largura da escala menos o tamanho do texto menos 2
                //Se estiver alinhado à direita, deve receber 2
                //Isso garante que o label fique alinhado com o canto da escala que toca o gráfico
                tb.Left = _isLeftAligned ? rcBounds.Width - _chartPanel._chartX.GetTextWidth(stringBuilder.ToString()) - 2 : 2;

                //O espaco acima do label deve ser a posicao atual do label o seu tamanho menos 2
                tb.Top = y - _chartPanel._chartX.GetTextHeight(stringBuilder.ToString()) - 2;

                //Setando propriedades de fonte
                tb._textBlock.FontSize = _chartPanel._chartX.FontSize;
                tb._textBlock.Foreground = _chartPanel._chartX.FontForeground;
                tb._textBlock.FontFamily = new FontFamily(_chartPanel._chartX.FontFace);
            }

            #endregion Obtendo labels da escala

            //Plotando uma linha abaixo
            Utils.DrawLine(_isLeftAligned ? rcBounds.Width - 1 : 1, 0, _isLeftAligned ? rcBounds.Width - 1 : 1, rcBounds.Height,
              _chartPanel._chartX.GradeCor, EnumGeral.TipoLinha.Solido, 1, _lines);

            //Parando o processo de renderizacao da escala
            _chartPanel.StopPaintingYGridLines();
            _painted = true;

            //??
            _lines.Stop();
            _labels.Stop();

            //Setando ZIndex para as linhas e labels da escala
            _lines.Do(l => l.ZIndex = ZIndexConstants.GridLines);
            _labels.Do(t => t.ZIndex = ZIndexConstants.DarvasBoxes1);
        }
        #endregion Render()

        #endregion Métodos
    }
}
