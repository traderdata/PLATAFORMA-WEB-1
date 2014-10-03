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

namespace Traderdata.Client.Componente.GraficoSL.Enum
{
    public class EnumPeriodo
    {
        public static readonly Tupla UmDia = new Tupla(1, "1 Dia");
        public static readonly Tupla DoisDias = new Tupla(2, "2 Dias");
        public static readonly Tupla TresDias = new Tupla(3, "3 Dias");
        public static readonly Tupla CincoDias = new Tupla(5, "5 Dias");
        public static readonly Tupla DezDias = new Tupla(10, "10 Dias");
        public static readonly Tupla QuinzeDias = new Tupla(15, "15 Dias");
        public static readonly Tupla UmMes = new Tupla(30, "1 Mes");
        public static readonly Tupla DoisMeses = new Tupla(60, "2 Meses");
        public static readonly Tupla TresMeses = new Tupla(90, "3 Meses");
        public static readonly Tupla SeisMeses = new Tupla(180, "6 Meses");
        public static readonly Tupla UmAno = new Tupla(360, "1 Ano");
        public static readonly Tupla TresAnos = new Tupla(1080, "3 Anos");
        public static readonly Tupla CincoAnos = new Tupla(1800, "5 Anos");
        public static readonly Tupla DezAnos = new Tupla(3600, "10 Anos");
        private static List<Tupla> periodos = new List<Tupla>();

        public static List<Tupla> GetList()
        {

            periodos.Clear();
            periodos.Add(UmDia);
            periodos.Add(DoisDias);
            periodos.Add(TresDias);
            periodos.Add(CincoDias);
            periodos.Add(DezDias);
            periodos.Add(QuinzeDias);
            periodos.Add(UmMes);
            periodos.Add(DoisMeses);
            periodos.Add(TresMeses);
            periodos.Add(SeisMeses);
            periodos.Add(UmAno);
            periodos.Add(TresAnos);
            periodos.Add(CincoAnos);
            periodos.Add(DezAnos);

            return periodos;         
        }

        public static List<Tupla> GetListDiario()
        {

            periodos.Clear();
            periodos.Add(SeisMeses);
            periodos.Add(UmAno);
            periodos.Add(TresAnos);
            periodos.Add(CincoAnos);
            periodos.Add(DezAnos);

            return periodos;
        }

        public static List<Tupla> GetListIntraday()
        {

            periodos.Clear();
            periodos.Add(UmDia);
            periodos.Add(DoisDias);
            periodos.Add(TresDias);
            periodos.Add(CincoDias);
            periodos.Add(DezDias);
            periodos.Add(QuinzeDias);
            periodos.Add(UmMes);            

            return periodos;
        }

        public static Tupla GetPeriodo(int periodo)
        {

            foreach (Tupla obj in GetList())
            {
                if (obj.Value == periodo)
                    return obj;
            }

            return null; 
        }
    }
}
