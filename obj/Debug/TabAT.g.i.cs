﻿#pragma checksum "C:\Backup\Projetos2010\CLIENT\TERMINAL-WEB-2011\TabAT.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C8109ABD9BE7150DFF0D8D144D454516"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using C1.Silverlight;
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


namespace Traderdata.Client.TerminalWEB {
    
    
    public partial class TabAT : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TabControl tabControl;
        
        internal System.Windows.Controls.TabItem tabItem;
        
        internal C1.Silverlight.C1Menu c1Menu1;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TD.APP.TERMINALWEB;component/TabAT.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.tabControl = ((System.Windows.Controls.TabControl)(this.FindName("tabControl")));
            this.tabItem = ((System.Windows.Controls.TabItem)(this.FindName("tabItem")));
            this.c1Menu1 = ((C1.Silverlight.C1Menu)(this.FindName("c1Menu1")));
        }
    }
}

