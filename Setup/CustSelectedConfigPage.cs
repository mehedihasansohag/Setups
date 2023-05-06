// Decompiled with JetBrains decompiler
// Type: Setup.CustSelectedConfigPage
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
    public class CustSelectedConfigPage : UserControl, IComponentConnector
    {
        internal Grid LayoutRoot;
        internal TextBlock tbkDescription;
        internal DataGrid listConfigList;
        internal TextBlock optionTbkDescription;
        internal DataGrid optionDgConfigList;
        internal TextBlock dropDownTbkDescription;
        internal ComboBox dropDownDgConfigList;
        private bool _contentLoaded;

        public CustSelectedConfigPage() => this.InitializeComponent();

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

        private void DgCustConfigList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Env.Instance.Config.InstallType == InstallType.AIO)
                this.ChangePage(sender);
            else
                Wizard.Instance.ExecuteSelectNextPage((object)null, (ExecutedRoutedEventArgs)null);
        }

        private void ChangePage(object sender)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid.SelectedValue == null || MessageBox.Show(Application.Current.MainWindow, string.Format(Setup.Properties.Resources.Msg_EnsureConfig, (object)(dataGrid.SelectedValue as ConfigEntity).DisplayName), Setup.Properties.Resources.Msg_Warning, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.OK) == MessageBoxResult.No)
                return;
            App.ContinueSetup.Set();
        }

        //private void StackPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if ((!(e.OldValue is bool oldValue) ? 0 : 1) == 0 | oldValue)
        //        return;
        //    this.Dispatcher.BeginInvoke((Delegate)(() => tChangeForce()));
        //}
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


        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/custselectedconfigpage.xaml", UriKind.Relative));
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
                    ((UIElement)target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.StackPanel_IsVisibleChanged);
                    break;
                case 3:
                    this.tbkDescription = (TextBlock)target;
                    break;
                case 4:
                    this.listConfigList = (DataGrid)target;
                    this.listConfigList.MouseDoubleClick += new MouseButtonEventHandler(this.DgCustConfigList_MouseDoubleClick);
                    break;
                case 5:
                    ((UIElement)target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.StackPanel_IsVisibleChanged);
                    break;
                case 6:
                    this.optionTbkDescription = (TextBlock)target;
                    break;
                case 7:
                    this.optionDgConfigList = (DataGrid)target;
                    this.optionDgConfigList.MouseDoubleClick += new MouseButtonEventHandler(this.DgCustConfigList_MouseDoubleClick);
                    break;
                case 8:
                    ((UIElement)target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.StackPanel_IsVisibleChanged);
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
