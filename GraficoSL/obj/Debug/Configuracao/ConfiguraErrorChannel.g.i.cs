﻿#pragma checksum "C:\Users\Felipe\Documents\Visual Studio 2010\Projects\GRAFICOSL\GraficoSL\Configuracao\ConfiguraErrorChannel.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F27587C3BA7D10EBD87304056262D9DC"
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


namespace Traderdata.Client.Componente.GraficoSL.Configuracao {
    
    
    public partial class ConfiguraErrorChannel : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.NumericUpDown numValor;
        
        internal System.Windows.Controls.NumericUpDown numEspessura;
        
        internal System.Windows.Shapes.Rectangle rectCor;
        
        internal System.Windows.Controls.Button OKButton;
        
        internal System.Windows.Controls.Button CancelButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.COMP.GRAFICOSL;component/Configuracao/ConfiguraErrorChannel.xaml", System.UriKind.Relative));
            this.numValor = ((System.Windows.Controls.NumericUpDown)(this.FindName("numValor")));
            this.numEspessura = ((System.Windows.Controls.NumericUpDown)(this.FindName("numEspessura")));
            this.rectCor = ((System.Windows.Shapes.Rectangle)(this.FindName("rectCor")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}
