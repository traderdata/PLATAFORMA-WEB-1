using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Traderdata.Client.TerminalWEB.Util
{
    public static class ConvertUtil
    {
       
        /// <summary>
        /// Converte a hex string para Color
        /// </summary>
        /// <param name="hexColor">a hex string: "FFFFFF", "#000000"</param>
        public static Color ConvertFromStringToColor(string hc)
        {
            string[] argb = hc.Split(';');
            Color cor = new Color();
            cor.A = Convert.ToByte(argb[0]);
            cor.R = Convert.ToByte(argb[1]);
            cor.G = Convert.ToByte(argb[2]);
            cor.B = Convert.ToByte(argb[3]);

            return cor;
        }

        /// <summary>
        /// Converte a hex string para SolidColorBrush
        /// </summary>
        /// <param name="hexColor">a hex string: "FFFFFF", "#000000"</param>
        public static SolidColorBrush ConvertFromStringToBrush(string hc)
        {
            return new SolidColorBrush(ConvertFromStringToColor(hc));
        }

        /// <summary>
        /// Converte a hex string para SolidColorBrush
        /// </summary>
        /// <param name="hexColor">a hex string: "FFFFFF", "#000000"</param>
        public static string ConvertFromBrushToString(Brush brush)
        {
            SolidColorBrush solid = (SolidColorBrush)brush;

            string argb = "";
            argb = solid.Color.A.ToString() + ";";
            argb += solid.Color.R.ToString() + ";";
            argb += solid.Color.G.ToString() + ";";
            argb += solid.Color.B.ToString();

            return argb;
        }

        /// <summary>
        /// Converte a Color to a string
        /// </summary>
        /// <param name="hexColor">a hex string: "FFFFFF", "#000000"</param>
        public static string ConvertFromColorToString(Color cor)
        {
            string argb = "";
            argb = cor.A.ToString() + ";";
            argb += cor.R.ToString() + ";";
            argb += cor.G.ToString() + ";";
            argb += cor.B.ToString();

            return argb;
        }        
    }
}
