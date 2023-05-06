// Decompiled with JetBrains decompiler
// Type: Setup.SelectedMachinePage
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
    public class SelectedMachinePage : UserControl, IComponentConnector
    {
        internal Grid LayoutRoot;
        internal TextBlock tbkDescription;
        internal DataGrid dgMachineList;
        private bool _contentLoaded;

        public SelectedMachinePage()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.SelectedMachinePage_Loaded);
        }

        private void SelectedMachinePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.dgMachineList.Focus();
            if (this.dgMachineList.Items.Count > 0 && this.dgMachineList.SelectedItem == null)
                this.dgMachineList.SelectedIndex = 0;
            if (this.dgMachineList.SelectedItem == null)
                return;
            this.dgMachineList.ScrollIntoView(this.dgMachineList.SelectedItem);
        }

        private void DgMachineList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (Env.Instance.Config.InstallType == InstallType.AIO)
                    this.ChangePage(sender);
                else
                    Wizard.Instance.ExecuteSelectNextPage((object)null, (ExecutedRoutedEventArgs)null);
            }
            else
            {
                if (e.Key != Key.Escape || Env.Instance.Config.InstallType != InstallType.AIO)
                    return;
                Application.Current.MainWindow.Close();
            }
        }

        private void DgMachineList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Env.Instance.Config.InstallType == InstallType.AIO)
                this.ChangePage(sender);
            else
                Wizard.Instance.ExecuteSelectNextPage((object)null, (ExecutedRoutedEventArgs)null);
        }

        private void ChangePage(object sender)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid.SelectedValue == null || MessageBox.Show(Application.Current.MainWindow, string.Format(Setup.Properties.Resources.Msg_EnsureMachine, (object)(dataGrid.SelectedValue as MachineEntity).DisplayName), Setup.Properties.Resources.Msg_Warning, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.OK) == MessageBoxResult.No)
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
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/selectedmachinepage.xaml", UriKind.Relative));
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
                    this.tbkDescription = (TextBlock)target;
                    break;
                case 3:
                    this.dgMachineList = (DataGrid)target;
                    this.dgMachineList.PreviewKeyDown += new KeyEventHandler(this.DgMachineList_PreviewKeyDown);
                    this.dgMachineList.MouseDoubleClick += new MouseButtonEventHandler(this.DgMachineList_MouseDoubleClick);
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
