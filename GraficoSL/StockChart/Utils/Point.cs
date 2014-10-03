using Traderdata.Client.Componente.GraficoSL.Enum;
using System.Windows;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  public class PointEx
  {
    public static Point Parse(string p)
    {
      string[] vals = p.Split(',');
      if (vals.Length != 2)
        return new Point(0, 0);

      return new Point(double.Parse(vals[0]), double.Parse(vals[1]));
    }
  }
}
