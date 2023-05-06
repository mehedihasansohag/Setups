// Decompiled with JetBrains decompiler
// Type: Setup.SelectedParamMigrationNK300
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
    public class SelectedParamMigrationNK300 : UserControl, IComponentConnector
    {
        private ConfigInfo config = Env.Instance.Config;
        internal StackPanel paramMigrationPanel;
        internal TextBlock tbkDescription;
        internal CheckBox IsReservedManufacturer;
        internal CheckBox IsReservedUser;
        internal StackPanel Warning;
        internal TextBlock WarningText;
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

        public SelectedParamMigrationNK300()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.SelectedParamMigrationLoaded);
            this.OKEvent += (RoutedEventHandler)((sender, e) => this.ConfirmedResult());
            this.CancelEvent += (RoutedEventHandler)((sender, e) => Application.Current.MainWindow.Close());
            if (!this.config.IsCheckMachine || !this.config.IsCustomPackUp)
                return;
            if (Installer.Instance.MachineCheckResult == MachineCheckResultEnum.InstallIsNull)
            {
                this.Warning.Visibility = this.Visibility;
                this.WarningText.Text = Setup.Properties.Resources.War_MachineNotFound;
            }
            else
            {
                if (Installer.Instance.MachineCheckResult != MachineCheckResultEnum.SelectMismatch)
                    return;
                this.Warning.Visibility = this.Visibility;
                this.WarningText.Text = Setup.Properties.Resources.War_MachineMisMatch;
            }
        }

        private void SelectedParamMigrationLoaded(object sender, RoutedEventArgs e)
        {
            if (this.IsReservedManufacturer.Content.ToString().IndexOf("(F5)") < 0)
            {
                CheckBox reservedManufacturer = this.IsReservedManufacturer;
                reservedManufacturer.Content = (object)(reservedManufacturer.Content?.ToString() + "(F5)");
            }
            if (this.IsReservedUser.Content.ToString().IndexOf("(F6)") < 0)
            {
                CheckBox isReservedUser = this.IsReservedUser;
                isReservedUser.Content = (object)(isReservedUser.Content?.ToString() + "(F6)");
            }
            if (InstallContext.Instance.IsReservedUser)
                this.IsReservedUser.Focus();
            else
                this.IsReservedManufacturer.Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!InstallContext.Instance.IsStartEnabled)
                return;
            if (e.Key == Key.Down)
            {
                e.Handled = true;
                if (!InstallContext.Instance.IsReservedManufacturer)
                    return;
                this.IsReservedUser.Focus();
            }
            else
            {
                if (e.Key != Key.Up)
                    return;
                e.Handled = true;
                if (InstallContext.Instance.IsReservedManufacturer || !InstallContext.Instance.IsReservedUser)
                    return;
                this.IsReservedManufacturer.Focus();
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.F5)
            {
                CheckBox reservedManufacturer = this.IsReservedManufacturer;
                bool? isChecked = this.IsReservedManufacturer.IsChecked;
                bool? nullable = isChecked.HasValue ? new bool?(!isChecked.GetValueOrDefault()) : new bool?();
                reservedManufacturer.IsChecked = nullable;
                this.IsReservedManufacturer.Focus();
                e.Handled = true;
            }
            else
            {
                if (e.Key != Key.F6 || !this.IsReservedUser.IsEnabled)
                    return;
                CheckBox isReservedUser = this.IsReservedUser;
                bool? isChecked = this.IsReservedUser.IsChecked;
                bool? nullable = isChecked.HasValue ? new bool?(!isChecked.GetValueOrDefault()) : new bool?();
                isReservedUser.IsChecked = nullable;
                this.IsReservedUser.Focus();
                e.Handled = true;
            }
        }

        private void ConfirmedResult()
        {
            if (Env.Instance.Config.InstallType == InstallType.PC || TempDialog.Show(Application.Current.MainWindow, !InstallContext.Instance.IsReservedUser || !InstallContext.Instance.IsReservedManufacturer ? (!InstallContext.Instance.IsReservedUser || InstallContext.Instance.IsReservedManufacturer ? (InstallContext.Instance.IsReservedUser || !InstallContext.Instance.IsReservedManufacturer ? Setup.Properties.Resources.Msg_SureNotTranformParameter : string.Format(Setup.Properties.Resources.Msg_SureOnlyTranformParameter, (object)Setup.Properties.Resources.Msg_ManufacturerParameter.ToLower())) : string.Format(Setup.Properties.Resources.Msg_SureOnlyTranformParameter, (object)Setup.Properties.Resources.Msg_UserParameter.ToLower())) : string.Format(Setup.Properties.Resources.Msg_SureTranformParameter, (object)(Setup.Properties.Resources.Msg_UserParameter.ToLower() + ", " + Setup.Properties.Resources.Msg_ManufacturerParameter.ToLower())), Setup.Properties.Resources.Msg_Warning, TempDialogButton.YesNo, TempDialogImage.Warning) == TempDialogButton.No)
                return;
            App.ContinueSetup.Set();
        }

        private void IsReservedManufacturer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox))
                return;
            InstallContext.Instance.ParameterTip = Env.Instance.Config.ManufacturerParameterTip;
        }

        private void IsReservedManufacturer_LostFocus(object sender, RoutedEventArgs e) => InstallContext.Instance.ParameterTip = string.Empty;

        private void IsReservedUser_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox))
                return;
            InstallContext.Instance.ParameterTip = Env.Instance.Config.UserParameterTip;
        }

        private void IsReservedUser_LostFocus(object sender, RoutedEventArgs e) => InstallContext.Instance.ParameterTip = string.Empty;

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/selectedparammigrationnk300.xaml", UriKind.Relative));
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
                    break;
                case 2:
                    this.tbkDescription = (TextBlock)target;
                    break;
                case 3:
                    this.IsReservedManufacturer = (CheckBox)target;
                    this.IsReservedManufacturer.GotFocus += new RoutedEventHandler(this.IsReservedManufacturer_GotFocus);
                    this.IsReservedManufacturer.LostFocus += new RoutedEventHandler(this.IsReservedManufacturer_LostFocus);
                    break;
                case 4:
                    this.IsReservedUser = (CheckBox)target;
                    this.IsReservedUser.GotFocus += new RoutedEventHandler(this.IsReservedUser_GotFocus);
                    this.IsReservedUser.LostFocus += new RoutedEventHandler(this.IsReservedUser_LostFocus);
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
