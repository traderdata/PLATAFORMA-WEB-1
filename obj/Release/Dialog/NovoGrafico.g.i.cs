﻿#pragma checksum "C:\Backup\Projetos2010\CLIENT\TERMINAL-WEB-2011\Dialog\NovoGrafico.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "873AA6657D7868FCD182EBA4C91DFC99"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
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
    
    
    public partial class NovoGrafico : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Border pnlCarregando;
        
        internal System.Windows.Controls.TextBlock lblCarregando;
        
        internal System.Windows.Controls.TextBox txtAtivo;
        
        internal System.Windows.Controls.Button btnPesquisaAtivo;
        
        internal System.Windows.Controls.ComboBox cmbPeriodo;
        
        internal System.Windows.Controls.ComboBox cmbPeriodicidade;
        
        internal System.Windows.Controls.ComboBox cmbTemplate;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.APP.TERMINALWEB;component/Dialog/NovoGrafico.xaml", System.UriKind.Relative));
            this.pnlCarregando = ((System.Windows.Controls.Border)(this.FindName("pnlCarregando")));
            this.lblCarregando = ((System.Windows.Controls.TextBlock)(this.FindName("lblCarregando")));
            this.txtAtivo = ((System.Windows.Controls.TextBox)(this.FindName("txtAtivo")));
            this.btnPesquisaAtivo = ((System.Windows.Controls.Button)(this.FindName("btnPesquisaAtivo")));
            this.cmbPeriodo = ((System.Windows.Controls.ComboBox)(this.FindName("cmbPeriodo")));
            this.cmbPeriodicidade = ((System.Windows.Controls.ComboBox)(this.FindName("cmbPeriodicidade")));
            this.cmbTemplate = ((System.Windows.Controls.ComboBox)(this.FindName("cmbTemplate")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}

