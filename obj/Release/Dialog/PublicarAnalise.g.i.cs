﻿#pragma checksum "C:\Backup\Projetos2010\CLIENT\TERMINAL-WEB-2011\Dialog\PublicarAnalise.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FEC31A91AA55064A9854077926685E45"
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
    
    
    public partial class PublicarAnalise : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.RadioButton rdbPublico;
        
        internal System.Windows.Controls.RadioButton rdbPrivado;
        
        internal System.Windows.Controls.TextBox txtEmails;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.APP.TERMINALWEB;component/Dialog/PublicarAnalise.xaml", System.UriKind.Relative));
            this.rdbPublico = ((System.Windows.Controls.RadioButton)(this.FindName("rdbPublico")));
            this.rdbPrivado = ((System.Windows.Controls.RadioButton)(this.FindName("rdbPrivado")));
            this.txtEmails = ((System.Windows.Controls.TextBox)(this.FindName("txtEmails")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.CancelButton = ((System.Windows.Controls.Button)(this.FindName("CancelButton")));
        }
    }
}

