﻿#pragma checksum "C:\Users\Felipe\Documents\Visual Studio 2010\Projects\GRAFICOSL\GraficoSL\Configuracao\ConfiguraDarvaBox.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "29380326BCF090ACBD7E6838E5F90094"
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
    
    
    public partial class ConfiguraDarvaBox : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.NumericUpDown numPercentual;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.COMP.GRAFICOSL;component/Configuracao/ConfiguraDarvaBox.xaml", System.UriKind.Relative));
            this.numPercentual = ((System.Windows.Controls.NumericUpDown)(this.FindName("numPercentual")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}

