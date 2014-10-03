using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Line=System.Windows.Shapes.Line;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_FibonacciRetracements()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.FibonacciRetracements, typeof(FibonacciRetracements), "Fibonacci Retracements");
        }
    }
}

namespace Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies
{
    /// <summary>
    /// Fibonacci Retracements line study
    /// </summary>
    public partial class FibonacciRetracements : LineStudy, IContextAbleLineStudy
    {
        private int _linesCount = 7;
        private Line[] _lines;
        private TextBlock[] _txts;
        private Line _handle = new Line();
        private ContextLine _contextLine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public FibonacciRetracements(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            double fibNum = 1.618;
            
            for (int i = 0; i < _linesCount; i++)
            {
                _params[i] = fibNum;
                fibNum *= 0.618;
            }

            _lines = new Line[_linesCount + 1];
            _txts = new TextBlock[_linesCount + 1];
            _studyType = StudyTypeEnum.FibonacciRetracements;
        }

        /// <summary>
        /// Seta parametros de salvamento.
        /// </summary>
        /// <param name="parametros"></param>
        /// <param name="strConcatenacao"></param>
        public override void SetaParametros(string parametros, char strConcatenacao)
        {
            if (parametros == null)
                return;

            System.Globalization.NumberFormatInfo nf = new System.Globalization.NumberFormatInfo();
            nf.NumberDecimalDigits = 2;
            nf.NumberDecimalSeparator = ".";


            //Obtendo parametros
            string[] paramet = parametros.Split(';');

            //Zerando parametros
            for (int i = 0; i < _params.Length; i++)
            {
                _params[i] = null;
            }

            //Obtendo parametros e setando parametros e determinando numero de linhas
            for (int i =0; i< paramet.Length;i++)
            {
                if (paramet[i] == "null")
                    _params[i] = null;
                else
                {
                    double valor = 0;

                    if (Double.TryParse(paramet[i].Replace(",", "."), System.Globalization.NumberStyles.Number, nf, out valor))
                        _params[i] = valor;
                    else
                        _params[i] = null;
                }
            }

            //DrawLineStudy(new Types.RectEx(_x1, _y1, _x2, _y2), LineStatus.InserindoDiretamente);
        }

        public override string ObtemParametros(char strConcatenacao)
        {
            string retorno = "";
            bool deveContinuar = true;

            //Obtendo parametros
            for (int i = 0; i < _params.Length; i++)
            {
                if (!deveContinuar)
                    break;

                //Colocando caracter de split
                if (retorno.Length > 0)
                    retorno += strConcatenacao;

                //Salvando o parametro
                if (_params[i] == null)
                    retorno += "null";
                else
                    retorno += _params[i].ToString();

                //Verificando se há algum valor válido após este parametro
                if (i < 100)
                {
                    for (int j = i + 1; j < _params.Length; j++)
                    {
                        //Se encontrar um valor diferente de null, devo continuar salvando os parametros
                        if (_params[j] != null)
                        {
                            deveContinuar = true;
                            break;
                        }

                        //Se chegar ao ultimo item e n achou um param diferente de null, deve indicar q devo parar de salvar parametros
                        if (j == _params.Length - 1)
                            deveContinuar = false;
                    }
                }
            }

            return retorno;
        }


        /// <summary>
        /// Adiciona ou edita parametro ao estudo Fibonacci Retracements.
        /// </summary>
        public void SetaParametros(List<double?> listaParam)
        {
            //Zerando parametros
            for (int i = 0; i < _params.Length; i++)
            {
                _params[i] = null;
            }

            //Setando parametros e determinando numero de linhas
            for (int i = 0; i < listaParam.Count; i++)
            {
                _params[i] = listaParam[i];
            }

            DrawLineStudy(new Types.RectEx(_x1, _y1, _x2, _y2), LineStatus.RePaint);
        }

        /// <summary>
        /// Obtém parametro para Fibonacci Retracements
        /// </summary>
        /// <returns></returns>
        public List<double?> ObtemParametro()
        {
            List<double?> listaParam = new List<double?>();

            for (int i = 0; i < _params.Length; i++)
            {
                listaParam.Add(_params[i]);
            }
            return listaParam;
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            int i;

            //Desenhando a linha inicialmente
            if ((lineStatus == LineStatus.StartPaint) || (lineStatus == LineStatus.InserindoDiretamente))
            {
                //Criando linhas e textos
                for (i = 0; i < _linesCount + 1; i++)
                {
                    _lines[i] = new Line
                                  {
                                      Stroke = Stroke,
                                      StrokeThickness = StrokeThickness,
                                      Tag = this,
                                  };
                    C.Children.Add(_lines[i]);

                    _txts[i] = new TextBlock
                                 {
                                     Foreground = _chartX.FontForeground,
                                     FontFamily = new FontFamily(_chartX.FontFace),
                                     FontSize = _chartX.FontSize,
                                     Tag = this,
                                 };
                    C.Children.Add(_txts[i]);

                    Canvas.SetZIndex(_lines[i], ZIndexConstants.LineStudies1);
                    Canvas.SetZIndex(_txts[i], ZIndexConstants.LineStudies1);
                }

                //Criando Handle
                _handle = new Line
                            {
                                Stroke = Stroke,
                                StrokeThickness = StrokeThickness,
                                Tag = this
                            };
                C.Children.Add(_handle);
                Canvas.SetZIndex(_handle, ZIndexConstants.LineStudies1);

                if (_contextLine == null)
                    _contextLine = new ContextLine(this);

                _internalObjectCreated = true;

                if (lineStatus == LineStatus.StartPaint)
                    return;
            }

            rect.Normalize();

            //Movendo
            if (lineStatus != LineStatus.Moving)
            {
                _handle.Visibility = Visibility.Visible;
                _handle.X1 = rect.Left;
                _handle.Y1 = rect.Top;
                _handle.X2 = rect.Right;
                _handle.Y2 = rect.Bottom;
            }
            //Escondendo o handle se estiver movendo
            else
            {
                _handle.Visibility = Visibility.Collapsed;
            }

            rect.Right = C.ActualWidth;

            double max = _chartPanel.GetY(rect.Top);
            rect.Right -= _chartX.GetTextWidth(string.Format("{0:f2}            ", max));

            bool upsideDown = false;

            //if (_params.Length > 1)
            //    upsideDown = Convert.ToDouble(_params[0]) < Convert.ToDouble(_params[1]);

            double fibNum = 1.618;
            double textTop;
            double y;

            //Desenhando linhas de acordo com os valores de _params
            for (i = 0; i < _linesCount; i++)
            {
                if (i < _params.Length && _params[i].HasValue)
                    fibNum = Convert.ToDouble(_params[i]);
                else if (_params[i] == null)
                {
                    _lines[i].Visibility = Visibility.Collapsed;
                    _txts[i].Visibility = Visibility.Collapsed;
                    continue;
                }
                else
                {
                    fibNum *= 0.618;
                    _params[i] = fibNum;
                }


                _lines[i].X1 = rect.Left;
                _lines[i].Y1 = _lines[i].Y2 = rect.Top + rect.Height * (1 - fibNum);
                _lines[i].X2 = rect.Right;

                double display = fibNum;
                if (display < 0)
                    display = 0;

                //Essa linha limita o fibonacci a 100% (soh o texto)
                //if (display > 1) display = 1;

                string strNum1 = upsideDown ? ((1 - display) * 100).ToString("0.0") : (display * 100).ToString("0.0");

                textTop = (rect.Top + (rect.Height * (1 - fibNum))) - 6;
                y = _chartPanel.GetReverseY(textTop + 6);
                string strNum2 = string.Format("{0:f" + _chartX.EscalaPrecisao + "}", y);

                _txts[i].Text = strNum2 + " (" + strNum1 + ")";
                Canvas.SetLeft(_txts[i], rect.Right + 2);
                Canvas.SetTop(_txts[i], textTop);

                _lines[i].Visibility = Visibility.Visible;
                _txts[i].Visibility = Visibility.Visible;
            }

            textTop = rect.Bottom - 6;
            y = _chartPanel.GetReverseY(textTop + 6);
            _txts[i].Text = string.Format("{0:f" + _chartX.EscalaPrecisao + "}", y) +
                            (upsideDown ? " (100%)" : " (0%)");
            Canvas.SetLeft(_txts[i], rect.Right + 2);
            Canvas.SetTop(_txts[i], textTop);

            _lines[i].X1 = rect.Left;
            _lines[i].Y1 = _lines[i].Y2 = rect.Bottom;
            _lines[i].X2 = rect.Right;

        }

        internal override void SetCursor()
        {
            if (_lines[0] == null) return;
            if (_selectionVisible)
            {
                foreach (Line line in _lines)
                {
                    line.Cursor = Cursors.Hand;
                }
                return;
            }
            if (_selectionVisible || _lines[0].Cursor == Cursors.Arrow) return;
            foreach (Line line in _lines)
            {
                line.Cursor = Cursors.Arrow;
            }
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = _newRect.TopLeft},
                 new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = _newRect.TopRight},
                 new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = _newRect.BottomLeft},
                 new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _newRect.BottomRight},
               };
        }

        internal override void SetStrokeThickness()
        {
            foreach (Line line in _lines)
            {
                line.StrokeThickness = StrokeThickness;
            }
        }

        internal override void SetStroke()
        {
            foreach (Line line in _lines)
            {
                line.Stroke = Stroke;
            }
        }

        internal override void SetStrokeType()
        {
            foreach (Line line in _lines)
            {
                Types.SetLinePattern(line, StrokeType);
            }
        }

        internal override void RemoveLineStudy()
        {
            foreach (Line line in _lines)
            {
                C.Children.Remove(line);
            }
            foreach (TextBlock txt in _txts)
            {
                C.Children.Remove(txt);
            }
            C.Children.Remove(_handle);
        }

        internal override void SetOpacity()
        {
            foreach (var line in _lines)
            {
                line.Opacity = Opacity;
            }
            foreach (var txt in _txts)
            {
                txt.Opacity = Opacity;
            }
        }

        #region Implementation of IContextAbleLineStudy

        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        public UIElement Element
        {
            get { return _handle; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_newRect.Left, _newRect.Top, _newRect.Right, _newRect.Bottom).Normalize(); }
        }

        /// <summary>
        /// Parent where <see cref="IContextAbleLineStudy.Element"/> belongs
        /// </summary>
        public Canvas Parent
        {
            get { return C; }
        }

        /// <summary>
        /// Gets if <see cref="IContextAbleLineStudy.Element"/> is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _selected; }
        }

        /// <summary>
        /// Z Index of <see cref="IContextAbleLineStudy.Element"/>
        /// </summary>
        public int ZIndex
        {
            get { return ZIndexConstants.LineStudies1; }
        }

        /// <summary>
        /// Gets the chart object associated with <see cref="IContextAbleLineStudy.Element"/> object
        /// </summary>
        public StockChartX Chart
        {
            get { return _chartX; }
        }

        /// <summary>
        /// Gets the reference to <see cref="LineStudies.LineStudy"/> 
        /// </summary>
        public LineStudy LineStudy
        {
            get { return this; }
        }

        #endregion
    }
}
