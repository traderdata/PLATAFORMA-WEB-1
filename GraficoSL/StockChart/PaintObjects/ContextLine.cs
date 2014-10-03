using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.ChartElementProperties;
using Traderdata.Client.Componente.GraficoSL.StockChart.Controls;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects
{
    internal interface IContextAbleLineStudy
    {
        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        UIElement Element { get; }
        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        Segment Segment { get; }
        /// <summary>
        /// Parent where <see cref="Element"/> belongs
        /// </summary>
        Canvas Parent { get; }
        /// <summary>
        /// Gets if <see cref="Element"/> is selected
        /// </summary>
        bool IsSelected { get; }
        /// <summary>
        /// Z Index of <see cref="Element"/>
        /// </summary>
        int ZIndex { get; }
        /// <summary>
        /// Gets the chart object associated with <see cref="Element"/> object
        /// </summary>
        StockChartX Chart { get; }
        /// <summary>
        /// Gets the reference to <see cref="LineStudies.LineStudy"/> 
        /// </summary>
        LineStudy LineStudy { get; }


    }

    internal class ContextLine
    {
        private const int ContextLineStrokeThickness = 14;

        private readonly IContextAbleLineStudy _contextAble;
        private Point[] _surroundPolygon;
        private readonly Line _line;

        public ContextLine(IContextAbleLineStudy contextAble)
        {
            _contextAble = contextAble;

            _contextAble.Element.MouseEnter += ParentOnMouseEnter;
            _contextAble.Element.MouseMove += ParentOnMouseMove;
            _contextAble.Element.MouseLeave += ParentOnMouseLeave;

            _line = new Line();
            _line.AddTo(_contextAble.Parent);
            _line._line.StrokeStartLineCap = PenLineCap.Round;
            _line._line.StrokeEndLineCap = PenLineCap.Round;
            _line._line.StrokeThickness = ContextLineStrokeThickness;
            _line.Stroke = new SolidColorBrush(Color.FromArgb(0x55, 0xCC, 0xCC, 0xCC));
            _line._line.Cursor = Cursors.Hand;
            _line._line.Tag = this;
            _line.ZIndex = _contextAble.ZIndex;

            _line._line.MouseLeftButtonUp += LineOnMouseLeftButtonUp;
            _line._line.MouseLeave += LineOnMouseLeave;
        }

        internal void LsContextMenuChoose(int menuId)
        {
            switch (menuId)
            {
                case 0: //properties
                    if (_contextAble.LineStudy != null)
                    {
                        IChartElementPropertyAble propertyAble = _contextAble.LineStudy;

                        List<IChartElementProperty> properties = new List<IChartElementProperty>(propertyAble.Properties);
                        PropertiesDialog dialog = new PropertiesDialog(propertyAble.Title, properties)
#if SILVERLIGHT
 { AppRoot = _contextAble.Chart.AppRoot }
#endif
;
                        dialog.Background = _contextAble.Chart.LineStudyPropertyDialogBackground;

                        foreach (IChartElementProperty property in properties)
                        {
                            if (property is ChartElementColorProperty)
                                ((ChartElementColorProperty)property).AppRoot
#if SILVERLIGHT
 = _contextAble.Chart.AppRoot;
#endif
#if WPF
                  = null;
#endif
                        }
#if SILVERLIGHT
                        dialog.Show(Dialog.DialogStyle.ModalDimmed);
#endif
#if WPF
            dialog.ShowDialog();
#endif
                    }
                    break;
                case 1: //delete
                    _contextAble.Chart.RemoveObject(_contextAble.LineStudy);
                    break;
            }
        }

        private void LineOnMouseLeave(object sender, MouseEventArgs args)
        {
            _line.Visible = false;
            //      _contextAble.Element.MouseEnter += ParentOnMouseEnter;
            //      _contextAble.Element.MouseLeave += ParentOnMouseLeave;
        }

        private void LineOnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            //      _contextAble.Element.MouseEnter += ParentOnMouseEnter;
            //      _contextAble.Element.MouseLeave += ParentOnMouseLeave;

            _line.Visible = false;

            if (_contextAble.Chart.InvokeLineStudyContextMenu(_contextAble.LineStudy))
                return; //host program canceled context menu

            //show context menu
            _contextAble.Chart.ShowLineStudyContextMenu(args.GetPosition(_contextAble.Chart), this);
        }

        private void ParentOnMouseLeave(object sender, MouseEventArgs args)
        {
            _line.Visible = false;
        }

        private bool _eventsRemoved;
        private void ParentOnMouseMove(object sender, MouseEventArgs args)
        {
            if (!_contextAble.IsSelected) return;
            if (_surroundPolygon == null)
                PositionLine();

            bool visible = args.GetPosition(_contextAble.Parent).InPolygon(_surroundPolygon);
            //_line.Visible = visible;
            if (visible)
            {
                _eventsRemoved = true;
                _contextAble.Element.MouseEnter -= ParentOnMouseEnter;
                _contextAble.Element.MouseLeave -= ParentOnMouseLeave;
            }
            else if (_eventsRemoved)
            {
                _eventsRemoved = false;
                _contextAble.Element.MouseEnter += ParentOnMouseEnter;
                _contextAble.Element.MouseLeave += ParentOnMouseLeave;
            }
        }

        private void ParentOnMouseEnter(object sender, MouseEventArgs args)
        {
            if (!_contextAble.IsSelected || _line.Visible) return;

            PositionLine();
            //      _contextAble.Element.MouseEnter -= ParentOnMouseEnter;
            //      _contextAble.Element.MouseLeave -= ParentOnMouseLeave;
        }

        private void PositionLine()
        {
            Segment segment = _contextAble.Segment;

            _surroundPolygon = segment.SurroundRectangle(ContextLineStrokeThickness);

            _line._line.X1 = segment.X1;
            _line._line.Y1 = segment.Y1;
            _line._line.X2 = segment.X2;
            _line._line.Y2 = segment.Y2;

        }
    }
}
