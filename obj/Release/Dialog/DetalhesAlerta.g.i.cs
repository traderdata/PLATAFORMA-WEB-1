﻿#pragma checksum "C:\Backup\Projetos2010\CLIENT\TERMINAL-WEB-2011\Dialog\DetalhesAlerta.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9D276AC31CA5D2184AC3EC2E4613D9A3"
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
    
    
    public partial class DetalhesAlerta : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.TextBlock txbAlertaTitulo;
        
        internal System.Windows.Controls.TextBox txbAtivo;
        
        internal System.Windows.Controls.Button btnBuscaAtivo;
        
        internal System.Windows.Controls.TextBox txbMensagem;
        
        internal System.Windows.Controls.TextBox txbPreco;
        
        internal System.Windows.Controls.TextBox txbVariacao;
        
        internal System.Windows.Controls.ComboBox cmbAcao;
        
        internal System.Windows.Controls.Button btnSalvarAlerta;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.APP.TERMINALWEB;component/Dialog/DetalhesAlerta.xaml", System.UriKind.Relative));
            this.txbAlertaTitulo = ((System.Windows.Controls.TextBlock)(this.FindName("txbAlertaTitulo")));
            this.txbAtivo = ((System.Windows.Controls.TextBox)(this.FindName("txbAtivo")));
            this.btnBuscaAtivo = ((System.Windows.Controls.Button)(this.FindName("btnBuscaAtivo")));
            this.txbMensagem = ((System.Windows.Controls.TextBox)(this.FindName("txbMensagem")));
            this.txbPreco = ((System.Windows.Controls.TextBox)(this.FindName("txbPreco")));
            this.txbVariacao = ((System.Windows.Controls.TextBox)(this.FindName("txbVariacao")));
            this.cmbAcao = ((System.Windows.Controls.ComboBox)(this.FindName("cmbAcao")));
            this.btnSalvarAlerta = ((System.Windows.Controls.Button)(this.FindName("btnSalvarAlerta")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}

