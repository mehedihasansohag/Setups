// Decompiled with JetBrains decompiler
// Type: Setup.InstallRestoreFilePage
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
    public class InstallRestoreFilePage : UserControl, IComponentConnector
    {
        private Installer installer = Installer.Instance;
        private bool _contentLoaded;

        public InstallRestoreFilePage()
        {
            this.InitializeComponent();
            LogWriter.NotifyProgressChanged += new LogWriter.WriteEventHandler(this.Log_WriteEvent);
            this.Loaded += new RoutedEventHandler(this.InstallRestoreFilePage_Loaded);
        }

        private void Log_WriteEvent(object sender, LogEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Message))
                return;
            InstallContext.Instance.SubCurrentText = e.Message;
        }

        private void InstallRestoreFilePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsVisible)
                return;
            Installer.Instance.RestoreFile(new RunWorkerCompletedEventHandler(this.RunWorkerCompleted));
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs ex)
        {
            Installer.Instance.IsRestored = true;
            if (ex.Error != null)
            {
                string str = Installer.Instance.SetException(ex.Error);
                WizardPage wizardPage = Wizard.Instance.GetWizardPage("RestoreErrorPage");
                if (wizardPage == null)
                    return;
                wizardPage.Description = str;
                Wizard.Instance.CurrentPage.NextPage = wizardPage;
                Wizard.Instance.CurrentPage.CanFinish = new bool?(true);
                Wizard.Instance.ExecuteSelectNextPage((object)null, (ExecutedRoutedEventArgs)null);
                LogWriter.UpdatePrompt(ex.Error.Message);
            }
            else
            {
                WizardPage wizardPage = Wizard.Instance.GetWizardPage("FinishPage");
                if (wizardPage == null)
                    return;
                string str = Setup.Properties.Resources.Msg_RestoreFinish;
                if (!string.IsNullOrEmpty(this.installer.RestoreErrorMessage))
                    str = str + Environment.NewLine + Environment.NewLine + Setup.Properties.Resources.Msg_RestoreReason + this.installer.RestoreErrorMessage;
                wizardPage.Description = str;
                if (Wizard.Instance.CurrentPage != null)
                {
                    Wizard.Instance.CurrentPage.NextPage = wizardPage;
                    Wizard.Instance.CurrentPage.CanFinish = new bool?(true);
                }
                Wizard.Instance.ExecuteSelectNextPage((object)null, (ExecutedRoutedEventArgs)null);
            }
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/installrestorefilepage.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target) => this._contentLoaded = true;
    }
}
