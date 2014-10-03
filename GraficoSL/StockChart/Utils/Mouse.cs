using Traderdata.Client.Componente.GraficoSL.Enum;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.SL.Utils
{
    public static class Mouse
    {
        private static readonly Dictionary<UIElement, Point> _mouseMoveAbleElements = new Dictionary<UIElement, Point>();

        public static void RegisterMouseMoveAbleElement(UIElement element)
        {
            _mouseMoveAbleElements.Add(element, new Point());
        }

        public static void UpdateMousePosition(UIElement element, Point newPoint)
        {
            _mouseMoveAbleElements[element] = newPoint;
        }

        public static Point GetPosition(UIElement relativeTo)
        {
            if (relativeTo == null)
                return new Point(0, 0);
            Point p;
            return _mouseMoveAbleElements.TryGetValue(relativeTo, out p) ? p : new Point(0, 0);
        }

        public static Cursor OverrideCursor
        {
            get;
            set;
        }
    }
}
