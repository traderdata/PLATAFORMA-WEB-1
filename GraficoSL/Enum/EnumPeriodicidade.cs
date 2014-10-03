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
    public class EnumPeriodicidade
    {
        
        public static readonly Tupla UmMinuto = new Tupla(1, "1 Minuto");
        public static readonly Tupla DoisMinutos = new Tupla(2, "2 Minutos");
        public static readonly Tupla TresMinutos = new Tupla(3, "3 Minutos");
        public static readonly Tupla CincoMinutos = new Tupla(5, "5 Minutos");
        public static readonly Tupla DezMinutos = new Tupla(10, "10 Minutos");
        public static readonly Tupla QuinzeMinutos = new Tupla(15, "15 Minutos");
        public static readonly Tupla TrintaMinutos = new Tupla(30, "30 Minutos");
        public static readonly Tupla UmaHora = new Tupla(60, "1 Hora");
        public static readonly Tupla DuasHoras = new Tupla(120, "2 Horas");
        public static readonly Tupla Diario = new Tupla(1440, "Diario");
        public static readonly Tupla Semanal = new Tupla(10080, "Semanal");
        public static readonly Tupla Mensal = new Tupla(43200, "Mensal");
        
        private static List<Tupla> periodicidades = new List<Tupla>();

        public static List<Tupla> GetList(EnumGeral.TipoPeriodicidade tipoPeriodicidade)
        {
            periodicidades.Clear();
            if (tipoPeriodicidade == EnumGeral.TipoPeriodicidade.Intraday)
            {
                periodicidades.Add(UmMinuto);
                periodicidades.Add(DoisMinutos);
                periodicidades.Add(TresMinutos);
                periodicidades.Add(CincoMinutos);
                periodicidades.Add(DezMinutos);
                periodicidades.Add(QuinzeMinutos);
                periodicidades.Add(TrintaMinutos);
                periodicidades.Add(UmaHora);
                //periodicidades.Add(DuasHoras);
            }
            else
            {
                periodicidades.Add(Diario);
                periodicidades.Add(Semanal);
                periodicidades.Add(Mensal);
            }
            return periodicidades;
        }

        public static Tupla GetPeriodicidade(int periodicidade)
        {

            foreach (Tupla obj in GetList(EnumGeral.TipoPeriodicidade.Intraday))
            {
                if (obj.Value == periodicidade)
                    return obj;
            }

            foreach (Tupla obj in GetList(EnumGeral.TipoPeriodicidade.Diario))
            {
                if (obj.Value == periodicidade)
                    return obj;
            }

            return null;
        }
    }
}
