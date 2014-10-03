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

namespace Traderdata.Client.TerminalWEB.DTO
{
    public class EnumLocal
    {
        public enum Bolsa
        {
            Bovespa = 1, BMF = 2
        }

        public enum AcaoTemplate { Excluir, Aplicar, Salvar, CarregarNovoGrafico, CarregarAlteracaoGrafico, Avulso, CarregaAreaTrabalho, AvulsoSimpleTrader };

    }
}
