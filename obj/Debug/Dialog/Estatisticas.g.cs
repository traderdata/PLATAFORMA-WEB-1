﻿#pragma checksum "C:\Backup\Projetos2010\CLIENT\TERMINAL-WEB-2011\Dialog\Estatisticas.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "03C1D5DA8C0AAA61983BE743E509E908"
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
    
    
    public partial class Estatisticas : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.DataGrid dtgDados;
        
        internal System.Windows.Controls.StackPanel stackCampos;
        
        internal System.Windows.Controls.StackPanel stackPanel1;
        
        internal System.Windows.Controls.TextBlock lblTotalBovespa;
        
        internal System.Windows.Controls.TextBlock lblTotalBMF;
        
        internal System.Windows.Controls.TextBlock lblTotalTrial;
        
        internal System.Windows.Controls.TextBlock lblTotalClientes;
        
        internal System.Windows.Controls.TextBlock lblTotalLogado;
        
        internal System.Windows.Controls.Button btnExportaExcel;
        
        internal System.Windows.Controls.Button btnExportaTrial;
        
        internal System.Windows.Controls.Button btnSair;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.APP.TERMINALWEB;component/Dialog/Estatisticas.xaml", System.UriKind.Relative));
            this.dtgDados = ((System.Windows.Controls.DataGrid)(this.FindName("dtgDados")));
            this.stackCampos = ((System.Windows.Controls.StackPanel)(this.FindName("stackCampos")));
            this.stackPanel1 = ((System.Windows.Controls.StackPanel)(this.FindName("stackPanel1")));
            this.lblTotalBovespa = ((System.Windows.Controls.TextBlock)(this.FindName("lblTotalBovespa")));
            this.lblTotalBMF = ((System.Windows.Controls.TextBlock)(this.FindName("lblTotalBMF")));
            this.lblTotalTrial = ((System.Windows.Controls.TextBlock)(this.FindName("lblTotalTrial")));
            this.lblTotalClientes = ((System.Windows.Controls.TextBlock)(this.FindName("lblTotalClientes")));
            this.lblTotalLogado = ((System.Windows.Controls.TextBlock)(this.FindName("lblTotalLogado")));
            this.btnExportaExcel = ((System.Windows.Controls.Button)(this.FindName("btnExportaExcel")));
            this.btnExportaTrial = ((System.Windows.Controls.Button)(this.FindName("btnExportaTrial")));
            this.btnSair = ((System.Windows.Controls.Button)(this.FindName("btnSair")));
        }
    }
}

