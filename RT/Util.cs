using System.Collections.Generic;
using Traderdata.Client.TerminalWEB.RT.DTO;
using System.Globalization;

namespace Traderdata.Client.TerminalWEB.RT
{
    /// <summary>
    /// Classe Util DDF para armazenamento de métodos genéricos
    /// </summary>
    internal class Util
    {
        private static NumberFormatInfo numberProvider = new NumberFormatInfo();

        /// <summary>
        /// Provider padrão para número. Utiliza 2 casas decimais e separador decimal ".".
        /// </summary>
        public static NumberFormatInfo NumberProvider
        {
            get
            {
                numberProvider.NumberDecimalDigits = 2;
                numberProvider.NumberDecimalSeparator = ".";
                numberProvider.NumberGroupSeparator = "";

                return Util.numberProvider.Clone() as NumberFormatInfo;
            }
        }

        
    }
}
