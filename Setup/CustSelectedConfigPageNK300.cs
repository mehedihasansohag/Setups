// Decompiled with JetBrains decompiler
// Type: Setup.CustSelectedConfigPageNK300
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library.Entity;
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
    public class CustSelectedConfigPageNK300 : UserControl, IComponentConnector
    {
        internal Grid LayoutRoot;
        internal StackPanel listStackPanel;
        internal TextBlock tbkDescription;
        internal DataGrid listConfigList;
        internal StackPanel optionStackPanel;
        internal TextBlock optionTbkDescription;
        internal DataGrid optionDgConfigList;
        internal StackPanel dropDownStackPanel;
        internal TextBlock dropDownTbkDescription;
        internal ComboBox dropDownDgConfigList;
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

        public CustSelectedConfigPageNK300()
        {
            this.InitializeComponent();
            this.OKEvent += (RoutedEventHandler)((sender, e) => this.ChangePage());
            this.CancelEvent += (RoutedEventHandler)((sender, e) => Application.Current.MainWindow.Close());
        }

        private void DgCustConfigList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => this.ChangePage();

        private void ChangePage()
        {
            CustomizeItem selectedValue = this.GetSelectedValue();
            if (selectedValue == null || TempDialog.Show(Application.Current.MainWindow, string.Format(Setup.Properties.Resources.Msg_EnsureCustConfig, (object)selectedValue.Text), Setup.Properties.Resources.Msg_Warning, TempDialogButton.YesNo, TempDialogImage.Warning) == TempDialogButton.No)
                return;
            App.ContinueSetup.Set();
        }

        private CustomizeItem GetSelectedValue()
        {
            CustomizeItem selectedValue = (CustomizeItem)null;
            if (InstallContext.Instance.CustPageListVisibility == Visibility.Visible && this.listConfigList.SelectedValue is CustomizeItem)
                selectedValue = this.listConfigList.SelectedValue as CustomizeItem;
            else if (InstallContext.Instance.CustPageOptionVisibility == Visibility.Visible && this.optionDgConfigList.SelectedValue is CustomizeItem)
                selectedValue = this.optionDgConfigList.SelectedValue as CustomizeItem;
            else if (InstallContext.Instance.CustDropDownVisibility == Visibility.Visible && this.dropDownDgConfigList.SelectedValue is CustomizeItem)
                selectedValue = this.dropDownDgConfigList.SelectedValue as CustomizeItem;
            return selectedValue;
        }

        private void StackPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            bool oldValue = false; // initialize to default value
            if (e.OldValue is bool)
            {
                oldValue = (bool)e.OldValue;
            }

            if (oldValue == false || e.NewValue == null || (bool)e.NewValue == false)
            {
                return;
            }
            this.Dispatcher.BeginInvoke((Action)(() => this.ChangeForce()));
        }

        private void ChangeForce()
        {
            if (InstallContext.Instance.CustPageListVisibility == Visibility.Visible)
                this.listConfigList.Focus();
            else if (InstallContext.Instance.CustPageOptionVisibility == Visibility.Visible)
            {
                this.optionDgConfigList.Focus();
            }
            else
            {
                if (InstallContext.Instance.CustDropDownVisibility != Visibility.Visible)
                    return;
                this.dropDownDgConfigList.Focus();
            }
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/custselectedconfigpagenk300.xaml", UriKind.Relative));
        }

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
                    this.listStackPanel = (StackPanel)target;
                    this.listStackPanel.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.StackPanel_IsVisibleChanged);
                    break;
                case 3:
                    this.tbkDescription = (TextBlock)target;
                    break;
                case 4:
                    this.listConfigList = (DataGrid)target;
                    this.listConfigList.MouseDoubleClick += new MouseButtonEventHandler(this.DgCustConfigList_MouseDoubleClick);
                    break;
                case 5:
                    this.optionStackPanel = (StackPanel)target;
                    this.optionStackPanel.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.StackPanel_IsVisibleChanged);
                    break;
                case 6:
                    this.optionTbkDescription = (TextBlock)target;
                    break;
                case 7:
                    this.optionDgConfigList = (DataGrid)target;
                    this.optionDgConfigList.MouseDoubleClick += new MouseButtonEventHandler(this.DgCustConfigList_MouseDoubleClick);
                    break;
                case 8:
                    this.dropDownStackPanel = (StackPanel)target;
                    this.dropDownStackPanel.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.StackPanel_IsVisibleChanged);
                    break;
                case 9:
                    this.dropDownTbkDescription = (TextBlock)target;
                    break;
                case 10:
                    this.dropDownDgConfigList = (ComboBox)target;
                    this.dropDownDgConfigList.MouseDoubleClick += new MouseButtonEventHandler(this.DgCustConfigList_MouseDoubleClick);
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
