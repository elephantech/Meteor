﻿#pragma checksum "..\..\..\SetParamWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "C72EA1D8A44729A789AEE8B972CCF6CEC6FEEE81EDDC2C457110B9859B634B13"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MeteorSwathTestApp {
    
    
    /// <summary>
    /// SetParamWindow
    /// </summary>
    public partial class SetParamWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\SetParamWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonSendParam;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\SetParamWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBoxParamIds;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\SetParamWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxValueEntry;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\SetParamWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock label1;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\SetParamWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock label2;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\SetParamWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonClose;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\SetParamWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBlockInfo;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\SetParamWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock label3;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\SetParamWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textBox1;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MeteorSwathTestApp;component/setparamwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\SetParamWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.buttonSendParam = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\..\SetParamWindow.xaml"
            this.buttonSendParam.Click += new System.Windows.RoutedEventHandler(this.buttonSendParam_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.comboBoxParamIds = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.textBoxValueEntry = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.label1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.label2 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.buttonClose = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\..\SetParamWindow.xaml"
            this.buttonClose.Click += new System.Windows.RoutedEventHandler(this.buttonClose_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.textBlockInfo = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.label3 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.textBox1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

