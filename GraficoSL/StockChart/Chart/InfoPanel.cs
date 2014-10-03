using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;
using Label=Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Label;
using Rectangle=System.Windows.Shapes.Rectangle;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    ///<summary>
    ///</summary>
    public abstract partial class InfoPanelItem
    {
        internal InfoPanel _infoPanel;
        internal bool _noCaption;
        internal string _sufixo;

        ///<summary>
        ///</summary>
        public abstract string Caption { get; }
        ///<summary>
        ///</summary>
        public abstract string Value(int record);
        ///<summary>
        ///</summary>
        public abstract string Value();
        ///<summary>
        ///</summary>
        public abstract string Sufixo();

        ///<summary>
        ///</summary>
        public string ValueEx(int record)
        {
            return string.IsNullOrEmpty(Value(record)) ? "-" : Value(record); 
        }

        ///<summary>
        ///</summary>
        public string ValueEx()
        {
            return string.IsNullOrEmpty(Value()) ? "-" : Value();
        }

       
    }

    interface IInfoPanelAble
    {
        IEnumerable<InfoPanelItem> InfoPanelItems { get; }
    }

    ///<summary>
    ///</summary>
    public class InfoPanel : Canvas
    {
        private readonly List<InfoPanelItem> _items = new List<InfoPanelItem>(12);
        private readonly PaintObjectsManager<Label> _labels = new PaintObjectsManager<Label>();
        private readonly PaintObjectsManager<Label> _values = new PaintObjectsManager<Label>();

        private Rectangle _rectLabels;
        private Rectangle _rectValues;

        internal Point? _position;

        internal readonly StockChartX _chartX;

        private readonly List<Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Tuple<string, string>> _entries 
            = new List<Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Tuple<string, string>>();

        private double GetRealLeftOffset()
        {
            if (_chartX.EscalaAlinhamento == EnumGeral.TipoAlinhamentoEscalaEnum.Direita)
                return 0;

            return Constants.YAxisWidth;
        }
        /// <summary>
        /// A special version of this function for InfoPanel. The standard version rounds to candle start,
        /// this version will round to the middle of candle
        /// </summary>
        /// <returns></returns>
        internal double GetReverseX()
        {
            return _chartX.GetReverseXInternal(X + _chartX.espacamentoBarra + _chartX.larguraBarra / 2 - GetRealLeftOffset());
        }

        ///<summary>
        ///</summary>
        ///<param name="chartX"></param>
        public InfoPanel(StockChartX chartX)
        {
            _chartX = chartX;
            PanelOwnerIndex = -1;

            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseMove += OnMouseMove;
        }

        private bool _leftMouseDown;
        private Point _ptStart;
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            //Point p = Mouse.GetPosition(PanelsContainer);
            if (!_leftMouseDown) 
                return;
                
            Point p = e.GetPosition(PanelsContainer);

            _position = new Point(GetLeft(this) + p.X - _ptStart.X, GetTop(this) + p.Y - _ptStart.Y);

            SetLeft(this, _position.Value.X);
            SetTop(this, _position.Value.Y);
            
            _ptStart = p;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _leftMouseDown = false;
            ReleaseMouseCapture();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _leftMouseDown = true;
            //_ptStart = Mouse.GetPosition(PanelsContainer);
            _ptStart = e.GetPosition(PanelsContainer);
            CaptureMouse();
        }

        /// <summary>
        /// values from which panel to display
        /// </summary>
        public int PanelOwnerIndex { get; set; }

        internal double X { get; set; }

        internal double Y { get; set; }

        internal PanelsContainer PanelsContainer { get; set; }

        internal void AddInfoPanelItem(InfoPanelItem item)
        {
            item._infoPanel = this;
            _items.Add(item);
            //RecalculateLayout();
        }

        internal void AddInfoPanelItems(IEnumerable<InfoPanelItem> items)
        {
            foreach (InfoPanelItem item in items)
            {
                item._infoPanel = this;
                _items.Add(item);
            }
            //RecalculateLayout();
        }

        internal void RemoveInfoPanelItem(InfoPanelItem item)
        {
            _items.Remove(item);
            if (_items.Count > 0)
                RecalculateLayout(0);
            else
                Visible = false;
        }

        internal void Clear()
        {
            _items.Clear();
            //Visible = false;
        }

        internal bool Visible
        {
            get { return Visibility == Visibility.Visible; }
            set { Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        internal Point Position
        {
            get { return new Point(GetLeft(this), GetTop(this)); }
            set
            {
                SetLeft(this, value.X);
                SetTop(this, value.Y);
            }
        }

        internal List<Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Tuple<string, string>> Entries
        {
            get { return _entries; }
        }

        internal void UpdateValues(int record)
        {
            Debug.Assert(_values.Count == _items.Count);

            _values.Start();
            foreach (InfoPanelItem item in _items)
            {
                Label label = _values.GetPaintObject();
                label._textBlock.Text = item.ValueEx(record);
            } 
            _values.Stop();
        }
               

        internal void RecalculateLayout(int record)
        {
            Debug.Assert(_chartX != null);

            _values.Start();
            _labels.Start();
            _values.C = this;
            _labels.C = this;

            double labelsMaxWidth = double.MinValue;
            double valuesMaxWidth = double.MinValue;
            double valuesNoCaptionMaxWidth = double.MinValue;
            int valueIndex;

            _entries.Clear();

            foreach (InfoPanelItem item in _items)
            {
                _entries.Add(new Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Tuple<string, string>
                               {
                                   Value1 = item.Caption,
                                   Value2 = item.ValueEx(record),
                               });

                if (!item._noCaption)
                {
                    Label labelCaption = _labels.GetPaintObject();
                    labelCaption._textBlock.Text = item.Caption;
                    labelCaption._textBlock.FontFamily = _chartX.InfoPanelFonte;
                    labelCaption._textBlock.FontSize = _chartX.InfoPanelTamanhoFonte;
                    labelCaption._textBlock.Foreground = _chartX.InfoPanelCorFonteLabels;
                    SetZIndex(labelCaption._textBlock, 5);
                }

                Label labelValue = _values.GetPaintObject();
                labelValue._textBlock.Text = item.ValueEx(record) + item.Sufixo();
                labelValue._textBlock.FontFamily = _chartX.InfoPanelFonte;
                labelValue._textBlock.FontSize = _chartX.InfoPanelTamanhoFonte;
                labelValue._textBlock.Foreground = _chartX.InfoPanelCorFonteValores;
                SetZIndex(labelValue._textBlock, 5);
            }
            _values.Stop();
            _labels.Stop();

            int labelIndex = valueIndex = 0;
            double top = 1;
            foreach (InfoPanelItem item in _items)
            {
                Label labelValue = _values[valueIndex++];

                if (!item._noCaption)
                {
                    Label labelCaption = _labels[labelIndex++];
                    if (labelCaption._textBlock.ActualWidth > labelsMaxWidth)
                        labelsMaxWidth = labelCaption._textBlock.ActualWidth;
                    SetLeft(labelCaption._textBlock, 1);
                    SetTop(labelCaption._textBlock, top - 1);
                }

                top += labelValue._textBlock.ActualHeight + 4;
            }

            top = 1;
            valueIndex = 0;
            foreach (InfoPanelItem item in _items)
            {
                Label labelValue = _values[valueIndex++];

                if (item._noCaption)
                {
                    SetLeft(labelValue._textBlock, 2);
                    if (labelValue._textBlock.ActualWidth > valuesNoCaptionMaxWidth)
                        valuesNoCaptionMaxWidth = labelValue._textBlock.ActualWidth;
                }
                else
                {
                    SetLeft(labelValue._textBlock, labelsMaxWidth + 3);
                    if (labelValue._textBlock.ActualWidth > valuesMaxWidth)
                        valuesMaxWidth = labelValue._textBlock.ActualWidth;
                }
                SetTop(labelValue._textBlock, top - 1);

                top += labelValue._textBlock.ActualHeight + 4;
            }

            double maxWidth = labelsMaxWidth + valuesMaxWidth + 4;
            Width = Math.Max(maxWidth, valuesNoCaptionMaxWidth);

            if (_rectLabels == null)
            {
                _rectLabels = new Rectangle { Stroke = null, StrokeThickness = 0 };
                Children.Add(_rectLabels);

                _rectValues = new Rectangle { Stroke = null, StrokeThickness = 0 };
                Children.Add(_rectValues);
            }

            SetLeft(_rectLabels, 0);
            SetTop(_rectLabels, 0);
            _rectLabels.Width = labelsMaxWidth + 2;
            _rectLabels.Height = top;
            _rectLabels.Fill = _chartX.InfoPanelCorFundoLabels;

            SetLeft(_rectValues, GetLeft(_rectLabels) + _rectLabels.Width);
            SetTop(_rectValues, 0);
            _rectValues.Width = Math.Max(maxWidth, valuesNoCaptionMaxWidth) - labelsMaxWidth + 4;
            _rectValues.Height = top;
            _rectValues.Fill = _chartX.InfoPanelCorFundoValores;

            Height = top;
        }

        internal void RecalculateLayout()
        {
            Debug.Assert(_chartX != null);

            _values.Start();
            _labels.Start();
            _values.C = this;
            _labels.C = this;

            double labelsMaxWidth = double.MinValue;
            double valuesMaxWidth = double.MinValue;
            double valuesNoCaptionMaxWidth = double.MinValue;
            int valueIndex;

            _entries.Clear();

            foreach (InfoPanelItem item in _items)
            {
                _entries.Add(new Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects.Tuple<string, string>
                {
                    Value1 = item.Caption,
                    Value2 = item.ValueEx(),
                });

                if (!item._noCaption)
                {
                    Label labelCaption = _labels.GetPaintObject();
                    labelCaption._textBlock.Text = item.Caption;
                    labelCaption._textBlock.FontFamily = _chartX.InfoPanelFonte;
                    labelCaption._textBlock.FontSize = _chartX.InfoPanelTamanhoFonte;
                    labelCaption._textBlock.Foreground = _chartX.InfoPanelCorFonteLabels;
                    SetZIndex(labelCaption._textBlock, 5);
                }

                Label labelValue = _values.GetPaintObject();
                labelValue._textBlock.Text = item.ValueEx() + item.Sufixo();
                labelValue._textBlock.FontFamily = _chartX.InfoPanelFonte;
                labelValue._textBlock.FontSize = _chartX.InfoPanelTamanhoFonte;
                labelValue._textBlock.Foreground = _chartX.InfoPanelCorFonteValores;
                SetZIndex(labelValue._textBlock, 5);
            }
            _values.Stop();
            _labels.Stop();

            int labelIndex = valueIndex = 0;
            double top = 1;
            foreach (InfoPanelItem item in _items)
            {
                Label labelValue = _values[valueIndex++];

                if (!item._noCaption)
                {
                    Label labelCaption = _labels[labelIndex++];
                    if (labelCaption._textBlock.ActualWidth > labelsMaxWidth)
                        labelsMaxWidth = labelCaption._textBlock.ActualWidth;
                    SetLeft(labelCaption._textBlock, 1);
                    SetTop(labelCaption._textBlock, top - 1);
                }

                top += labelValue._textBlock.ActualHeight + 4;
            }

            top = 1;
            valueIndex = 0;
            foreach (InfoPanelItem item in _items)
            {
                Label labelValue = _values[valueIndex++];

                if (item._noCaption)
                {
                    SetLeft(labelValue._textBlock, 2);
                    if (labelValue._textBlock.ActualWidth > valuesNoCaptionMaxWidth)
                        valuesNoCaptionMaxWidth = labelValue._textBlock.ActualWidth;
                }
                else
                {
                    SetLeft(labelValue._textBlock, labelsMaxWidth + 3);
                    if (labelValue._textBlock.ActualWidth > valuesMaxWidth)
                        valuesMaxWidth = labelValue._textBlock.ActualWidth;
                }
                SetTop(labelValue._textBlock, top - 1);

                top += labelValue._textBlock.ActualHeight + 4;
            }

            double maxWidth = labelsMaxWidth + valuesMaxWidth + 4;
            Width = Math.Max(maxWidth, valuesNoCaptionMaxWidth);

            if (_rectLabels == null)
            {
                _rectLabels = new Rectangle { Stroke = null, StrokeThickness = 0 };
                Children.Add(_rectLabels);

                _rectValues = new Rectangle { Stroke = null, StrokeThickness = 0 };
                Children.Add(_rectValues);
            }

            SetLeft(_rectLabels, 0);
            SetTop(_rectLabels, 0);
            _rectLabels.Width = labelsMaxWidth + 2;
            _rectLabels.Height = top;
            _rectLabels.Fill = _chartX.InfoPanelCorFundoLabels;

            SetLeft(_rectValues, GetLeft(_rectLabels) + _rectLabels.Width);
            SetTop(_rectValues, 0);
            _rectValues.Width = Math.Max(maxWidth, valuesNoCaptionMaxWidth) - labelsMaxWidth + 4;
            _rectValues.Height = top;
            _rectValues.Fill = _chartX.InfoPanelCorFundoValores;

            Height = top;
        }

        
    }
}

