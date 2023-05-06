// Decompiled with JetBrains decompiler
// Type: Setup.InstallPage
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
    public class InstallPage : UserControl, IComponentConnector
    {
        private Installer installer = Installer.Instance;
        private bool _contentLoaded;

        public InstallPage()
        {
            this.InitializeComponent();
            LogWriter.NotifyProgressChanged += new LogWriter.WriteEventHandler(this.Log_WriteEvent);
            this.Loaded += new RoutedEventHandler(this.InstallPage_Loaded);
        }

        private void Log_WriteEvent(object sender, LogEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Message))
                return;
            InstallContext.Instance.SubCurrentText = e.Message;
        }

        private void InstallPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsVisible)
                return;
            if (Application.Current.MainWindow != null)
                this.installer.DisableCloseButton();
            this.installer.Start(new RunWorkerCompletedEventHandler(this.RunWorkerCompleted));
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs ex)
        {
            Installer.Instance.EnableCloseButton();
            if (ex.Error != null)
                Installer.Instance.SetException(ex.Error);
            if (!Installer.Instance.IsSucceed)
            {
                if (Installer.Instance.hasHistoryVersion)
                {
                    this.installer.RestoreErrorMessage = this.installer.ErrorMessage;
                    if (Installer.Instance.IsFinishedBackUpFile)
                    {
                        this.installer.GotoRestoreFilePage();
                        return;
                    }
                    this.installer.RestoreBackupFileForEnvCheckFailed();
                    this.installer.IsRestored = true;
                }
                else
                    this.installer.IsRestored = true;
            }
            if (!Installer.Instance.IsFinished || this.installer.IsBeginRestore)
                return;
            if (ex.Error != null)
            {
                WizardPage wizardPage = Wizard.Instance.GetWizardPage("ErrorPage");
                if (wizardPage != null)
                {
                    wizardPage.Description = this.installer.ErrorMessage;
                    Wizard.Instance.CurrentPage.NextPage = wizardPage;
                    Wizard.Instance.CurrentPage.CanFinish = new bool?(true);
                }
            }
            else
            {
                Installer.Instance.IsSucceed = true;
                if (Wizard.Instance.CurrentPage != null)
                    Wizard.Instance.CurrentPage.CanFinish = new bool?(true);
            }
            Wizard.Instance.ExecuteSelectNextPage((object)null, (ExecutedRoutedEventArgs)null);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/installpage.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target) => this._contentLoaded = true;
    }
}
