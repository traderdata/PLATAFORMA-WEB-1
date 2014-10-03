using System;
using Traderdata.Client.Componente.GraficoSL.Enum;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
#if SILVERLIGHT
using Traderdata.Client.Componente.GraficoSL.StockChart.SL;
#endif


namespace Traderdata.Client.Componente.GraficoSL.StockChart.PaintObjects
{
  ///<summary>
  /// Extra types used in library
  ///</summary>
  public static class Types
  {

    internal static void SetLinePattern(Shape shape, EnumGeral.TipoLinha pattern)
    {
      switch (pattern)
      {
          case EnumGeral.TipoLinha.Tracejado:
          shape.StrokeDashArray = new DoubleCollection {4, 3};
          break;
          case EnumGeral.TipoLinha.Pontilhado:
          shape.StrokeDashArray = new DoubleCollection {1, 2};
          break;
          case EnumGeral.TipoLinha.TracejadoPontilhado:
          shape.StrokeDashArray = new DoubleCollection {4, 2, 1, 2};
          break;

          case EnumGeral.TipoLinha.Solido:
          shape.StrokeDashArray = new DoubleCollection();
          break;

          case EnumGeral.TipoLinha.Nenhum:
          shape.Stroke = Brushes.Transparent;
          break;
      }
    }

    internal enum Corner
    {
      None,
      MoveAll,
      TopLeft,
      TopRight,
      BottomLeft,
      BottomRight,
      TopCenter,
      BottomCenter,
      MiddleLeft,
      MiddleRight
    }

    internal struct Location
    {
      public double X1;
      public double X2;
      public double Y1;
      public double Y2;

      public Location(double x1, double y1, double x2, double y2)
      {
        X1 = x1;
        X2 = x2;
        Y1 = y1;
        Y2 = y2;
      }

      public Location(Point p1, Point p2)
      {
        X1 = p1.X;
        Y1 = p1.Y;
        X2 = p2.X;
        Y2 = p2.Y;
      }

      public Point P1
      {
        get { return new Point(X1, Y1); }
      }

      public Point P2
      {
        get { return new Point(X2, Y2); }
      }
    }

    /// <summary>
    /// Used instead of standard Rectangle, cause standard one has too many checks against negative Width &amp; Height
    /// </summary>
    public struct RectEx
    {
      ///<summary>
      ///</summary>
      public double Left;
      ///<summary>
      ///</summary>
      public double Top;
      ///<summary>
      ///</summary>
      public double Right;
      ///<summary>
      ///</summary>
      public double Bottom;

      ///<summary>
      /// Ctor
      ///</summary>
      ///<param name="left"></param>
      ///<param name="top"></param>
      ///<param name="right"></param>
      ///<param name="bottom"></param>
      public RectEx(double left, double top, double right, double bottom)
      {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
      }

      ///<summary>
      ///</summary>
      public double Width
      {
        get { return Right - Left; }
      }

      ///<summary>
      ///</summary>
      public double Height
      {
        get { return Bottom - Top; }
      }

      ///<summary>
      ///</summary>
      public bool IsZero
      {
        get { return Width == 0 || Height == 0; }
      }

      ///<summary>
      ///</summary>
      ///<returns></returns>
      public RectEx Normalize()
      {
        if (Left > Right)
          Utils.Swap(ref Left, ref Right);
        if (Top > Bottom)
          Utils.Swap(ref Top, ref Bottom);

        return this;
      }

      ///<summary>
      ///</summary>
      public Point TopLeft
      {
        get { return new Point(Left, Top); }
      }

      ///<summary>
      ///</summary>
      public Point TopRight
      {
        get { return new Point(Right, Top); }
      }

      ///<summary>
      ///</summary>
      public Point BottomLeft
      {
        get { return new Point(Left, Bottom); }
      }

      ///<summary>
      ///</summary>
      public Point BottomRight
      {
        get { return new Point(Right, Bottom); }
      }

      ///<summary>
      ///</summary>
      public Point TopCenter
      {
        get { return new Point((Left + Right) / 2, Top); }
      }

      ///<summary>
      ///</summary>
      public Point BottomCenter
      {
        get { return new Point((Left + Right) / 2, Bottom); }
      }

      ///<summary>
      ///</summary>
      public Point MiddleLeft
      {
        get { return new Point(Left, (Top + Bottom) / 2); }
      }

      ///<summary>
      ///</summary>
      public Point MiddleRight
      {
        get { return new Point(Right, (Top + Bottom) / 2); }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
        return string.Format("Esquerda: {0}, Top: {1}, Width: {2}, Height: {3}", 
          Left, Top, Width, Height);
      }

      ///<summary>
      ///</summary>
      ///<param name="thickness"></param>
      ///<returns></returns>
      public Point[] MainDiagonalPolygon(double thickness)
      {
        return new Segment(Left, Top, Right, Bottom).SurroundRectangle(thickness);
      }

      ///<summary>
      ///</summary>
      ///<param name="p"></param>
      ///<param name="strokeThickness"></param>
      ///<returns></returns>
      public bool PointOnMainDiagonal(Point p, int strokeThickness)
      {
        return p.InPolygon(MainDiagonalPolygon(strokeThickness));
      }
    }
  }

  ///<summary>
  ///</summary>
  public struct Segment
  {
    ///<summary>
    ///</summary>
    public double X1;
    ///<summary>
    ///</summary>
    public double Y1;
    ///<summary>
    ///</summary>
    public double X2;
    ///<summary>
    ///</summary>
    public double Y2;

    ///<summary>
    /// Ctor
    ///</summary>
    ///<param name="x1"></param>
    ///<param name="y1"></param>
    ///<param name="x2"></param>
    ///<param name="y2"></param>
    public Segment(double x1, double y1, double x2, double y2)
    {
      X1 = x1;
      Y1 = y1;
      X2 = x2;
      Y2 = y2;
    }

    ///<summary>
    /// Ctor
    ///</summary>
    ///<param name="origin"></param>
    ///<param name="length"></param>
    ///<param name="angle"></param>
    public Segment(Point origin, double length, double angle)
    {
      X1 = origin.X;
      Y1 = origin.Y;

      angle *= Math.PI / 180.0;

      X2 = X1 + length * Math.Cos(angle);
      Y2 = Y1 + length * Math.Sin(angle);
    }

    ///<summary>
    ///</summary>
    ///<returns></returns>
    public Segment Normalize()
    {
      if (X1 > X2)
        Utils.Swap(ref X1, ref X2);
      if (Y1 > Y2)
        Utils.Swap(ref Y1, ref Y2);

      return this;
    }

    ///<summary>
    /// Returns a polygon that surrounds the given segment
    ///</summary>
    ///<param name="thickness"></param>
    ///<returns></returns>
    public Point[] SurroundRectangle(double thickness)
    {
      double dx = X2 - X1;
      double dy = Y2 - Y1;

      if ((dx != 0) || (dy != 0))
      {
        double norm = 1.0 / Math.Sqrt(dx * dx + dy * dy);
        double uDX = dx * norm;
        double uDY = dy * norm;
        double tSum = (thickness / 2.0f) * (uDX + uDY);
        double tDiff = (thickness / 2.0f) * (uDX - uDY);

        return new[]
                 {
                   new Point(X2 + tDiff, Y2 + tSum),
                   new Point(X2 + tSum, Y2 - tDiff),
                   new Point(X1 - tDiff, Y1 - tSum),
                   new Point(X1 - tSum, Y1 + tDiff),
                 };
      }
      return new Point[0];
    }

    ///<summary>
    /// Gets the segment length
    ///</summary>
    public  double Length
    {
      get
      {
        return Math.Sqrt(Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2));
      }
    }

    ///<summary>
    /// Inflates the current segment
    ///</summary>
    ///<param name="pct"></param>
    ///<returns></returns>
    public Segment Inflate(double pct)
    {
      int signX = Math.Sign(X2 - X1);
      int signY = Math.Sign(Y2 - Y1);

      double l = Length;
      double d = (l - (l * (1 + pct / 100.0))) / 2;

      if (X2 == X1)
      {
        Y1 += signY * d;
        Y2 -= signY * d;
        return this;
      }
      if (Y2 == Y1)
      {
        X1 += signX * d;
        X2 -= signX * d;
        return this;
      }


      double slope = (Y1 - Y2) / (X1 - X2);
      double angle = Math.Atan(slope);

      X1 += signX * Math.Cos(angle) * d;
      Y1 += signX * Math.Sin(angle) * d;
      X2 += -signX * Math.Cos(angle) * d;
      Y2 += -signX * Math.Sin(angle) * d;

      return this;
    }

    ///<summary>
    /// Returns a point that will be used to draw a normal-line to current segment
    ///</summary>
    ///<returns></returns>
    public Point Normal()
    {
      double nx, ny;

      if (X1 == X2)
      {
        nx = Math.Sign(Y2 - Y1);
        ny = 0;
      }
      else
      {
        double f = (Y2 - Y1) / (X2 - X1);
        nx = f * Math.Sign(X2 - X1) / Math.Sqrt(1 + f * f);
        ny = -1 * Math.Sign(X2 - X1) / Math.Sqrt(1 + f * f);
      }
      return new Point(nx, ny);
    }

    ///<summary>
    /// Gets the distance between current segment and the specified point
    ///</summary>
    ///<param name="p"></param>
    ///<returns></returns>
    public double DistanceToPoint(Point p)
    {
      double normalLength = Math.Sqrt((X2 - X1) * (X2 - X1) + (Y2 - Y1) * (Y2 - Y1));
      return Math.Abs((p.X - X1) * (Y2 - Y1) - (p.Y - Y1) * (X2 - X1)) / normalLength;
    }
  }

  /// <summary>
  /// A tuple class
  /// </summary>
  /// <typeparam name="T1">Type for value1</typeparam>
  /// <typeparam name="T2">Type for value2</typeparam>
  public class Tuple<T1, T2>
  {
    /// <summary>
    /// Value 1
    /// </summary>
    public T1 Value1 { get; set; }
    /// <summary>
    /// Value 2
    /// </summary>
    public T2 Value2 { get; set; }
  }

}

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  ///<summary>
  ///</summary>
  ///<typeparam name="T"></typeparam>
  public class EventArgs<T> : EventArgs
  {
    ///<summary>
    ///</summary>
    public T Data { get; private set; }

    ///<summary>
    ///</summary>
    ///<param name="data"></param>
    public EventArgs(T data)
    {
      Data = data;
    }
  }
}


