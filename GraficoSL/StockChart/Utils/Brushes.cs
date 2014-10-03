using System.Windows.Media;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart.SL
{
    public static class Brushes
    {
        public static Brush Transparent = new SolidColorBrush(Colors.Transparent);
        public static Brush Navy = new SolidColorBrush(ColorsEx.Navy);
        public static Brush SkyBlue = new SolidColorBrush(ColorsEx.SkyBlue);
        public static Brush Silver = new SolidColorBrush(ColorsEx.Silver);
        public static Brush White = new SolidColorBrush(Colors.White);
        public static Brush Black = new SolidColorBrush(Colors.Black);
        public static Brush Yellow = new SolidColorBrush(Colors.Yellow);
        public static Brush Red = new SolidColorBrush(Colors.Red);
        public static Brush Blue = new SolidColorBrush(Colors.Blue);
        public static Brush DarkRed = new SolidColorBrush(ColorsEx.DarkRed);
        public static Brush LightBlue = new SolidColorBrush(ColorsEx.LightBlue);
        public static Brush Green = new SolidColorBrush(Colors.Green);
    }

    public static class ColorsEx
    {
        public static Color Gray = Color.FromArgb(0xFF, 0x2F, 0x4F, 0x4F);
        public static Color Navy = Color.FromArgb(0xFF, 0x00, 0x00, 0x80);
        public static Color SkyBlue = Color.FromArgb(0xFF, 0x87, 0xCE, 0xEB);
        public static Color Silver = Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0);
        public static Color Lime = Color.FromArgb(0xFF, 0x00, 0xFF, 0x00);
        public static Color LightBlue = Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6);
        public static Color LightSteelBlue = Color.FromArgb(0xFF, 0xB0, 0xC4, 0xDE);
        public static Color DarkRed = Color.FromArgb(0xFF, 0x8B, 0x00, 0x00);
        public static Color MidnightBlue = Color.FromArgb(0xFF, 0x19, 0x19, 0x70);
    }
}
