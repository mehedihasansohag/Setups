﻿#pragma checksum "..\..\..\..\ui\selectedparammigrationnk300.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A71A93EAA7EF9ED6BE2539D33E846F4A2009D881A02891C189F28E0685AB2D97"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Setup;
using Setup.Properties;
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


namespace Setup {
    
    
    /// <summary>
    /// SelectedParamMigrationNK300
    /// </summary>
    public partial class SelectedParamMigrationNK300 : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel paramMigrationPanel;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbkDescription;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox IsReservedManufacturer;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox IsReservedUser;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Warning;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock WarningText;
        
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
            System.Uri resourceLocater = new System.Uri("/Setup;component/ui/selectedparammigrationnk300.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
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
            this.paramMigrationPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.tbkDescription = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.IsReservedManufacturer = ((System.Windows.Controls.CheckBox)(target));
            
            #line 25 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
            this.IsReservedManufacturer.GotFocus += new System.Windows.RoutedEventHandler(this.IsReservedManufacturer_GotFocus);
            
            #line default
            #line hidden
            
            #line 25 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
            this.IsReservedManufacturer.LostFocus += new System.Windows.RoutedEventHandler(this.IsReservedManufacturer_LostFocus);
            
            #line default
            #line hidden
            return;
            case 4:
            this.IsReservedUser = ((System.Windows.Controls.CheckBox)(target));
            
            #line 28 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
            this.IsReservedUser.GotFocus += new System.Windows.RoutedEventHandler(this.IsReservedUser_GotFocus);
            
            #line default
            #line hidden
            
            #line 28 "..\..\..\..\ui\selectedparammigrationnk300.xaml"
            this.IsReservedUser.LostFocus += new System.Windows.RoutedEventHandler(this.IsReservedUser_LostFocus);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Warning = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.WarningText = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

