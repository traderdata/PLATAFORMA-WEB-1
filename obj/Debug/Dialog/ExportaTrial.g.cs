﻿#pragma checksum "C:\Backup\Projetos2010\CLIENT\TERMINAL-WEB-2011\Dialog\ExportaTrial.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DCFC7C00DC4D2B2F21698D16EFE434CB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
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


namespace Traderdata.Client.TerminalWEB.Dialog {
    
    
    public partial class ExportaTrial : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.TextBlock txbLiberaTitulo;
        
        internal System.Windows.Controls.DataGrid dtgDados;
        
        internal System.Windows.Controls.DatePicker dpkDataInicial;
        
        internal System.Windows.Controls.DatePicker dpkDataFinal;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.APP.TERMINALWEB;component/Dialog/ExportaTrial.xaml", System.UriKind.Relative));
            this.txbLiberaTitulo = ((System.Windows.Controls.TextBlock)(this.FindName("txbLiberaTitulo")));
            this.dtgDados = ((System.Windows.Controls.DataGrid)(this.FindName("dtgDados")));
            this.dpkDataInicial = ((System.Windows.Controls.DatePicker)(this.FindName("dpkDataInicial")));
            this.dpkDataFinal = ((System.Windows.Controls.DatePicker)(this.FindName("dpkDataFinal")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}

