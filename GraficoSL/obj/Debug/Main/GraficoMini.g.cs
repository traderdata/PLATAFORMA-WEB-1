﻿#pragma checksum "C:\Users\Felipe\Documents\Visual Studio 2010\Projects\GRAFICOSL\GraficoSL\Main\GraficoMini.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "11D21FD42CAAE8B6788AD0C429AFB4ED"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using Traderdata.Client.Componente.GraficoSL.StockChart;


namespace Traderdata.Client.Componente.GraficoSL.Main {
    
    
    public partial class GraficoMini : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel stpEstudos;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnZoomArea;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnResetZoom;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnDiario;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnIntraday;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnVolume;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnLinha;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnBarra;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnCandle;
        
        internal Traderdata.Client.Componente.GraficoSL.StockChart.StockChartX stockChart;
        
        internal Traderdata.Client.Componente.GraficoSL.StockChart.StockChartX stockChartIndicadores;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.COMP.GRAFICOSL;component/Main/GraficoMini.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.stpEstudos = ((System.Windows.Controls.StackPanel)(this.FindName("stpEstudos")));
            this.btnZoomArea = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnZoomArea")));
            this.btnResetZoom = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnResetZoom")));
            this.btnDiario = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnDiario")));
            this.btnIntraday = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnIntraday")));
            this.btnVolume = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnVolume")));
            this.btnLinha = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnLinha")));
            this.btnBarra = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnBarra")));
            this.btnCandle = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnCandle")));
            this.stockChart = ((Traderdata.Client.Componente.GraficoSL.StockChart.StockChartX)(this.FindName("stockChart")));
            this.stockChartIndicadores = ((Traderdata.Client.Componente.GraficoSL.StockChart.StockChartX)(this.FindName("stockChartIndicadores")));
        }
    }
}
