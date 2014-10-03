using Traderdata.Client.Componente.GraficoSL.Enum;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.SL
{
  public static class Extensions
  {
    public static void Inflate(this Rect self, double width, double height)
    {
      self.X -= width / 2;
      self.Width += width / 2;
      self.Y -= height / 2;
      self.Height += height / 2;
    }

    public static void Offset(this Point self, double offsetX, double offsetY)
    {
      self.X += offsetX;
      self.Y += offsetY;
    }

    public static int FindIndex<T>(this List<T> self, Func<T, bool> predicat)
    {
      int index = 0;
      foreach (var t in self)
      {
        if (predicat(t))
          return index;
        index++;
      }
      return -1;
    }
  }
}
