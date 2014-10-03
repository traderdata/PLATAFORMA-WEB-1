using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.StockChart.Exceptions;
using Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies;
using Traderdata.Client.Componente.GraficoSL.Enum;
using Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public static partial class StockChartX_LineStudiesParams
  {
    internal static void Register_WpfFrameworkElement()
    {
      RegisterLineStudy(LineStudy.StudyTypeEnum.FrameworkElement,
        typeof(LineStudies.FrameworkElement), "FrameworkElement");
    }
  }
}


namespace Traderdata.Client.Componente.GraficoSL.StockChart.LineStudies
{
  ///<summary>
  /// WPF UI Element
  ///</summary>
  public class FrameworkElement : LineStudy
  {
    ///<summary>
    ///</summary>
    ///<param name="key"></param>
    ///<param name="stroke"></param>
    ///<param name="chartPanel"></param>
    public FrameworkElement(string key, Brush stroke, ChartPanel chartPanel)
      : base(key, stroke, chartPanel)
    {
      _studyType = StudyTypeEnum.FrameworkElement;
    }

    internal override void SetArgs(params object[] args)
    {
      Debug.Assert(args.Length > 0);

      Element = args[0] as System.Windows.FrameworkElement;
      if (Element == null)                    
        throw new ChartException("FrameworkElement class can work only objects derived from System.Windows.FrameworkElement");

      Element.Visibility = Visibility.Collapsed;
    }

    internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
    {
      if (Element == null)
      {
        Debug.WriteLine("FrameworkElement NULL");
        return;
      }

      if (lineStatus == LineStatus.StartPaint)
      {
        C.Children.Add(Element);

        Canvas.SetZIndex(Element, ZIndexConstants.LineStudies1);
        Element.Tag = this;

        _internalObjectCreated = true;

        xEnumVisuals(Element);

        return;
      }

      if (Element.Visibility == Visibility.Collapsed)
        Element.Visibility = Visibility.Visible;

      rect.Normalize();
      Canvas.SetLeft(Element, rect.Left);
      Canvas.SetTop(Element, rect.Top);
      Element.Width = rect.Width;
      Element.Height = rect.Height;
    }

    internal override List<SelectionDotInfo> GetSelectionPoints()
    {
      List<SelectionDotInfo> res =
        new List<SelectionDotInfo>
          {
            new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = _newRect.TopLeft},
            new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = _newRect.TopRight},
            new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = _newRect.BottomLeft},
            new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _newRect.BottomRight},
          };
      return res;
    }

    internal override void SetCursor()
    {
      if (_selectionVisible && Element.Cursor != Cursors.Hand)
      {
        Element.Cursor = Cursors.Hand;
        return;
      }
      if (_selectionVisible || Element.Cursor == Cursors.Arrow) return;
      Element.Cursor = Cursors.Arrow;
    }

    internal override void RemoveLineStudy()
    {
      C.Children.Remove(Element);
    }

    internal override void SetOpacity()
    {
      Element.Opacity = Opacity;
    }

    private void xEnumVisuals(System.Windows.FrameworkElement root)
    {
      Stack<System.Windows.FrameworkElement> children
        = new Stack<System.Windows.FrameworkElement>();

      children.Push(root);

      while (children.Count > 0)
      {
        System.Windows.FrameworkElement child = children.Pop();

        child.Tag = this;

        if (child is ContentControl)
        {
          System.Windows.FrameworkElement contentElement
            = ((ContentControl)child).Content as System.Windows.FrameworkElement;
          if (contentElement != null)
            children.Push(contentElement);
          continue;
        }

        if (child is Panel)
        {
          Panel panelElement = child as Panel;
          foreach (System.Windows.FrameworkElement panelChild in panelElement.Children)
          {
            children.Push(panelChild);
          }
        }
      }
    }

    ///<summary>
    /// Returns a reference to the internal <see cref="System.Windows.FrameworkElement"/>
    ///</summary>
    public System.Windows.FrameworkElement Element { get; private set; }
  }
}
