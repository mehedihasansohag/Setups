// Decompiled with JetBrains decompiler
// Type: Setup.SelectedParamMigration
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

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
    public class SelectedParamMigration : UserControl, IComponentConnector
    {
        private ConfigInfo config = Env.Instance.Config;
        internal StackPanel paramMigrationPanel;
        internal TextBlock tbkDescription;
        internal CheckBox manufacturerParameter;
        internal CheckBox userParameter;
        internal StackPanel Warning;
        internal TextBlock WarningText;
        private bool _contentLoaded;

        public SelectedParamMigration()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.SelectedParamMigration_Loaded);
            if (!this.config.IsCheckMachine || !this.config.IsCustomPackUp)
                return;
            this.CheckWarning();
        }

        private void SelectedParamMigration_Loaded(object sender, RoutedEventArgs e)
        {
            this.CheckWarning();
            if (InstallContext.Instance.IsReservedUser)
                this.manufacturerParameter.Focus();
            else
                this.userParameter.Focus();
        }

        private void CheckWarning()
        {
            switch (Installer.Instance.MachineCheckResult)
            {
                case MachineCheckResultEnum.Pass:
                    this.Warning.Visibility = Visibility.Collapsed;
                    this.WarningText.Text = string.Empty;
                    break;
                case MachineCheckResultEnum.InstallIsNull:
                    this.Warning.Visibility = this.Visibility;
                    this.WarningText.Text = Setup.Properties.Resources.War_MachineNotFound;
                    break;
                case MachineCheckResultEnum.SelectMismatch:
                    this.Warning.Visibility = this.Visibility;
                    this.WarningText.Text = Setup.Properties.Resources.War_MachineMisMatch;
                    break;
            }
        }

        private void StackPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.config.InstallType != InstallType.AIO)
                return;
            switch (e.Key)
            {
                case Key.Tab:
                case Key.Left:
                case Key.Right:
                    e.Handled = true;
                    break;
                case Key.Return:
                    e.Handled = true;
                    this.ConfirmedResult();
                    break;
                case Key.Escape:
                    e.Handled = true;
                    Application.Current.MainWindow.Close();
                    break;
                case Key.Up:
                    if (!InstallContext.Instance.IsStartEnabled)
                        break;
                    e.Handled = true;
                    if (InstallContext.Instance.IsReservedManufacturer || !InstallContext.Instance.IsReservedUser)
                        break;
                    this.userParameter.Focus();
                    break;
                case Key.Down:
                    if (!InstallContext.Instance.IsStartEnabled)
                        break;
                    e.Handled = true;
                    if (InstallContext.Instance.IsReservedManufacturer)
                    {
                        this.manufacturerParameter.Focus();
                        break;
                    }
                    if (!InstallContext.Instance.IsReservedUser)
                        break;
                    this.userParameter.IsChecked = new bool?(false);
                    this.manufacturerParameter.IsChecked = new bool?(false);
                    break;
            }
        }

        private void ConfirmedResult()
        {
            if (Env.Instance.Config.InstallType == InstallType.PC || MessageBox.Show(Application.Current.MainWindow, !InstallContext.Instance.IsReservedUser ? string.Format(Setup.Properties.Resources.Msg_SureTranformParameter, (object)Setup.Properties.Resources.Msg_ManufacturerParameter.ToLower()) : string.Format(Setup.Properties.Resources.Msg_SureTranformParameter, (object)Setup.Properties.Resources.Msg_UserParameter.ToLower()), Setup.Properties.Resources.Msg_Warning, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.OK) == MessageBoxResult.No)
                return;
            App.ContinueSetup.Set();
        }

        private void userParameter_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is CheckBox checkBox) || !checkBox.IsMouseOver)
                return;
            InstallContext.Instance.ParameterTip = Env.Instance.Config.UserParameterTip;
        }

        private void userParameter_MouseLeave(object sender, MouseEventArgs e) => InstallContext.Instance.ParameterTip = "";

        private void manufacturerParameter_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is CheckBox checkBox) || !checkBox.IsMouseOver)
                return;
            InstallContext.Instance.ParameterTip = Env.Instance.Config.ManufacturerParameterTip;
        }

        private void manufacturerParameter_MouseLeave(object sender, MouseEventArgs e) => InstallContext.Instance.ParameterTip = "";

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/selectedparammigration.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.paramMigrationPanel = (StackPanel)target;
                    this.paramMigrationPanel.PreviewKeyDown += new KeyEventHandler(this.StackPanel_PreviewKeyDown);
                    break;
                case 2:
                    this.tbkDescription = (TextBlock)target;
                    break;
                case 3:
                    this.manufacturerParameter = (CheckBox)target;
                    this.manufacturerParameter.MouseEnter += new MouseEventHandler(this.manufacturerParameter_MouseEnter);
                    this.manufacturerParameter.MouseLeave += new MouseEventHandler(this.manufacturerParameter_MouseLeave);
                    break;
                case 4:
                    this.userParameter = (CheckBox)target;
                    this.userParameter.MouseEnter += new MouseEventHandler(this.userParameter_MouseEnter);
                    this.userParameter.MouseLeave += new MouseEventHandler(this.userParameter_MouseLeave);
                    break;
                case 5:
                    this.Warning = (StackPanel)target;
                    break;
                case 6:
                    this.WarningText = (TextBlock)target;
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
