using System;
using System.Collections.Generic;
using System.Reflection;
using Traderdata.Client.Componente.GraficoSL.Enum;

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
    public static class Enum
    {
        public static IEnumerable<string> GetNames(Type enumType)
        {
            FieldInfo[] fieldInfos = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var info in fieldInfos)
            {
                yield return info.Name;
            }
        }
    }
}