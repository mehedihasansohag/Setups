// Decompiled with JetBrains decompiler
// Type: Setup.SelecedOtherTaskPage
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Setup
{
    public class SelecedOtherTaskPage : UserControl, IComponentConnector
    {
        internal CheckBox chkAutoStart;
        private bool _contentLoaded;

        public SelecedOtherTaskPage() => this.InitializeComponent();

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/selecedothertaskpage.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            if (connectionId == 1)
                this.chkAutoStart = (CheckBox)target;
            else
                this._contentLoaded = true;
        }
    }
}
