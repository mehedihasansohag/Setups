// Decompiled with JetBrains decompiler
// Type: Setup.SelectedConfigPageNK300
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Setup
{
    public class SelectedConfigPageNK300 : UserControl, IComponentConnector
    {
        internal Grid LayoutRoot;
        internal TextBlock tbkDescription;
        internal ConfigDataGrid dgConfigList;
        private bool _contentLoaded;

        public event RoutedEventHandler OKEvent
        {
            add => this.AddHandler(MainWindow_NK300.OKEvent, (Delegate)value);
            remove => this.RemoveHandler(MainWindow_NK300.OKEvent, (Delegate)value);
        }

        public event RoutedEventHandler CancelEvent
        {
            add => this.AddHandler(MainWindow_NK300.CancelEvent, (Delegate)value);
            remove => this.RemoveHandler(MainWindow_NK300.CancelEvent, (Delegate)value);
        }

        public SelectedConfigPageNK300()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.SelectedConfigPage_Loaded);
            this.OKEvent += (RoutedEventHandler)((sender, e) => this.ChangePage());
            this.CancelEvent += (RoutedEventHandler)((sender, e) => Application.Current.MainWindow.Close());
        }

        private void SelectedConfigPage_Loaded(object sender, RoutedEventArgs e) => this.dgConfigList.Focus();

        private void DgConfigList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => this.ChangePage();

        private void ChangePage()
        {
            if (this.dgConfigList.SelectedValue == null || TempDialog.Show(Application.Current.MainWindow, string.Format(Setup.Properties.Resources.Msg_EnsureConfig, (object)(this.dgConfigList.SelectedValue as ConfigEntity).DisplayName), Setup.Properties.Resources.Msg_Warning, TempDialogButton.YesNo, TempDialogImage.Warning) == TempDialogButton.No)
                return;
            App.ContinueSetup.Set();
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/selectedconfigpagenk300.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, (object)this, handler);

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.LayoutRoot = (Grid)target;
                    break;
                case 2:
                    this.tbkDescription = (TextBlock)target;
                    break;
                case 3:
                    this.dgConfigList = (ConfigDataGrid)target;
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
