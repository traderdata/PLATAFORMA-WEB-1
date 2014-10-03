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
using Traderdata.Client.TerminalWEB.RT.DTO;

namespace Traderdata.Client.TerminalWEB.RT
{
    public class DataHub
    {
        public static List<AtivoDTO> listaAtivosBovespa;
        public static List<AtivoDTO> listaAtivosBMF;
        public static List<TickDTO> listaTicks = new List<TickDTO>();

        public static List<AtivoDTO> Ativos()
        {
            List<AtivoDTO> listaAtivos = new List<AtivoDTO>();
            foreach (AtivoDTO obj in listaAtivosBMF)
            {
                listaAtivos.Add(obj);
            }
            foreach (AtivoDTO obj in listaAtivosBovespa)
            {
                listaAtivos.Add(obj);
            }
            listaAtivos.Sort(delegate(AtivoDTO a1, AtivoDTO a2) { return a1.Ativo.CompareTo(a2.Ativo); });
            return listaAtivos;
            
        }
    }
}
