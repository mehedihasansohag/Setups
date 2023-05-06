// Decompiled with JetBrains decompiler
// Type: Setup.MainWindow_NK300
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library;
using Packup.Library.Entity;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Setup
{
    public class MainWindow_NK300 : Window, IDisposable, IComponentConnector
    {
        internal static readonly RoutedEvent OKEvent = EventManager.RegisterRoutedEvent(nameof(OKEvent), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(MainWindow_NK300));
        internal static readonly RoutedEvent CancelEvent = EventManager.RegisterRoutedEvent(nameof(CancelEvent), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(MainWindow_NK300));
        private AutoResetEvent autoReset = new AutoResetEvent(false);
        private BackgroundWorker worker;
        private Installer installer = Installer.Instance;
        private ConfigInfo config = Env.Instance.Config;
        internal Wizard wizard;
        internal WizardPage installPage;
        internal InstallListView lv;
        internal WizardPage SelectConfig;
        internal WizardPage Machine;
        internal WizardPage CustConfigPage;
        internal WizardPage Parameter;
        internal Button OKButton;
        internal Button CancelButton;
        private bool _contentLoaded;

        public MainWindow_NK300()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.MainWindow_NK300_Loaded);
            this.Closing += new CancelEventHandler(this.MainWindow_NK300_Closing);
        }

        private void MainWindow_NK300_Loaded(object sender, RoutedEventArgs e)
        {
            InstallContext.Instance.InstallTitle_NK300 = Setup.Properties.Resources.Msg_Installing;
            InstallContext.Instance.InstallDescription_NK300 = StringParser.Parse(Setup.Properties.Resources.Msg_NcStudioInstalling);
            Mutex ncStudioMutex = (Mutex)null;
            App.installWorker.DoWork += (DoWorkEventHandler)((o, et) =>
           {
               bool flag = true;
               try
               {
                   this.Dispatch_Message(Setup.Properties.Resources.Msg_StartSetup);
                   this.Dispatch_Message(Setup.Properties.Resources.Msg_ConfirmInstall);
                   if (!this.Dispatch_Confirm())
                   {
                       flag = false;
                   }
                   else
                   {
                       this.Dispatch_Message(StringParser.Parse("${res:Msg_CheckNcStudio}"));
                       this.Dispatch_CloseNcstudio();
                       this.Dispatch_CloseProcess();
                       this.ShowWizardPageByLocationConfig(WizardPageName.SelectConfig, WizardPageLocation.Before);
                       if (InstallContext.Instance.IsMultipleConfig)
                           this.ShowSelectedConfigPage();
                       this.ShowWizardPageByLocationConfig(WizardPageName.SelectConfig, WizardPageLocation.After);
                       this.ShowWizardPageByLocationConfig(WizardPageName.Machine, WizardPageLocation.Before);
                       if (InstallContext.Instance.IsMultipleMachine)
                           this.ShowSelectedMachinePage();
                       this.ShowWizardPageByLocationConfig(WizardPageName.Machine, WizardPageLocation.After);
                       this.ShowWizardPageByLocationConfig(WizardPageName.Parameter, WizardPageLocation.Before);
                       this.ShowSelectedParamMigration();
                       this.ShowWizardPageByLocationConfig(WizardPageName.Parameter, WizardPageLocation.After);
                       this.ShowWizardPageAtDefaultLocation(new MainWindow_NK300.NotHaveParamHandler(this.ShowCustConfigPage), WizardPageName.CustConfigPage);
                       this.GotoSetupPage();
                       Application.Current.Dispatcher.BeginInvoke((Action)(() => this.installer.DisableCloseButton()));
                       this.installer.IsBeginInstall = true;
                       if (this.installer.CheckExeProcess())
                           throw new Exception(Setup.Properties.Resources.Msg_ProcessExist);
                       bool createdNew;
                       ncStudioMutex = new Mutex(true, "SingletonPhoenixAppMutex", out createdNew);
                       this.Dispatcher.Invoke((Action)(() =>
                 {
                     this.OKButton.IsEnabled = false;
                     this.OKButton.Content = (object)Setup.Properties.Resources.Btn_Finish_NK300;
                     this.CancelButton.IsEnabled = false;
                     this.CancelButton.Visibility = Visibility.Collapsed;
                 }));
                       this.Dispatch_Schedule(Setup.Properties.Resources.Msg_CheckEnvironment);
                       this.installer.CloseBackgroundProcesses();
                       this.installer.CloseBackgroundProcesses();
                       this.installer.CheckInstallEnvironment();
                       this.installer.IsCheckEnvironment = true;
                       this.StopUpdateSchedule();
                       if (!File.Exists(Path.Combine(App.SetupPath, "Release.zip")))
                           App.WaitExtract.WaitOne();
                       this.Dispatch_Schedule(Setup.Properties.Resources.Msg_CopyFiles);
                       this.installer.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
                 {
                     this.installer.RenameOriginalPathToCurrentPath();
                     string zipPath = Path.Combine(App.SetupPath, "Release.zip");
                     this.installer.RunBeforeSetup(zipPath);
                     this.installer.ExtractZip(zipPath);
                     this.StopUpdateSchedule();
                 }));
                       this.installer.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
                 {
                     this.Dispatch_Message(Setup.Properties.Resources.Msg_CheckTransformParameter);
                     this.Dispatch_Schedule(Setup.Properties.Resources.Msg_ExecuteTransform);
                     this.installer.TransParam();
                     this.StopUpdateSchedule();
                 }));
                       this.installer.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
                 {
                     this.Dispatch_Schedule(Setup.Properties.Resources.Msg_InstallDriver);
                     this.installer.InstallDirver();
                     this.installer.IsDriverSetup = true;
                     this.installer.DisableSleep();
                     this.installer.PromptAdmin();
                     this.StopUpdateSchedule();
                 }));
                       this.installer.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() => this.Dispatch_Schedule(Setup.Properties.Resources.Msg_InstallRemoteProc)));
                       this.installer.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.installer.InstallSunloginClient));
                       this.installer.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.installer.InstallFirstRun));
                       this.installer.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.installer.CreateShortCut));
                       this.installer.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.installer.ClearStartMachineStart));
                       this.installer.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.installer.ClearStartMachineStartNoDesktop));
                       this.installer.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
                 {
                     InstallContext.Instance.IsAutoStart = true;
                     InstallContext.Instance.IsAutoRestartNoDesktop = true;
                     InstallContext.Instance.IsAutoRestart = false;
                     this.installer.CreateAutoRestart();
                 }));
                       this.installer.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.installer.RunAfterSetup));
                       this.installer.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.installer.FirstRunAutoStart));
                       this.installer.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.StopUpdateSchedule));
                       this.Dispatch_Message(Setup.Properties.Resources.Msg_FinishingSetup);
                       this.installer.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() => this.installer.IsSucceed = true));
                       if (this.installer.IsSucceed || this.installer.IsRestored || !this.installer.IsFinishedBackUpFile)
                           return;
                       this.Dispatch_Schedule(Setup.Properties.Resources.Msg_Restoring);
                       this.installer.RestoreFile_NK300();
                       this.StopUpdateSchedule();
                       this.Dispatch_Message(Setup.Properties.Resources.Msg_RestoreFinish);
                       this.installer.IsRestored = true;
                       this.installer.IsSucceed = true;
                   }
               }
               catch (Exception ex)
               {
                   Application.Current.Dispatcher.Invoke((Action)(() =>
             {
                   this.StopUpdateSchedule();
                   this.installer.SetException(ex);
                   int num = (int)TempDialog.Show(Application.Current.MainWindow, Setup.Properties.Resources.Err_SetupException + ex.Message, Setup.Properties.Resources.Msg_Warning);
               }));
                   if (this.installer.IsSucceed || this.installer.IsRestored)
                       return;
                   if (this.installer.IsFinishedBackUpFile)
                   {
                       this.Dispatch_Schedule(Setup.Properties.Resources.Msg_Restoring);
                       this.installer.RestoreFile_NK300();
                       this.StopUpdateSchedule();
                       this.Dispatch_Message(Setup.Properties.Resources.Msg_RestoreFinish);
                       this.installer.IsRestored = true;
                       this.installer.IsSucceed = true;
                   }
                   else
                   {
                       this.installer.RestoreBackupFileForEnvCheckFailed();
                       this.installer.IsSucceed = true;
                   }
               }
               finally
               {
                   Application.Current.Dispatcher.BeginInvoke((Action)(() => this.installer.EnableCloseButton()));
                   if (!App.installWorker.CancellationPending)
                       Env.Instance.CompleteUpdateConfigFile();
                   this.installer.IsFinished = true;
                   if (this.installer.IsSucceed || !this.installer.IsFinishedBackUpFile)
                   {
                       this.Dispatcher.Invoke((Action)(() => this.OKButton.IsEnabled = true));
                       if (InstallContext.Instance.IsDirverFailed)
                       {
                           this.Dispatch_Message(Setup.Properties.Resources.Err_InstallDriverFailed);
                           Application.Current.Dispatcher.BeginInvoke((Action)(() => (this.lv.SelectedItem as ListViewItem).Foreground = (Brush)new SolidColorBrush(Colors.Red)));
                       }
                       this.installer.DeleteBackupConfigFileByPath(new string[3]
                  {
              this.config.ActiveConfigPath,
              this.config.OEMConfigPath,
              Path.Combine(new DirectoryInfo(this.config.OEMConfigPath).Parent.FullName, "Assets")
                 });
                       if (!this.installer.IsRestored)
                       {
                           if (this.installer.IsFinishedBackUpFile)
                           {
                               Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                         {
                         InstallContext.Instance.InstallTitle_NK300 = Setup.Properties.Resources.Msg_FinishedSetup;
                         InstallContext.Instance.InstallDescription_NK300 = StringParser.Parse(Setup.Properties.Resources.Msg_NcStudioInstalled);
                     }));
                               this.Dispatch_Message(Setup.Properties.Resources.Msg_FinishedSetup);
                           }
                           else
                           {
                               Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                         {
                         InstallContext.Instance.InstallTitle_NK300 = Setup.Properties.Resources.Msg_InstallFailed;
                         InstallContext.Instance.InstallDescription_NK300 = StringParser.Parse(Setup.Properties.Resources.Msg_InstallFailed);
                     }));
                               this.Dispatch_Message(Setup.Properties.Resources.Msg_InstallFailed);
                           }
                       }
                       else
                       {
                           Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                     {
                       InstallContext.Instance.InstallTitle_NK300 = Setup.Properties.Resources.Msg_RestoreFinish;
                       InstallContext.Instance.InstallDescription_NK300 = StringParser.Parse(Setup.Properties.Resources.Msg_RestoreFinish);
                   }));
                           this.Dispatch_Message(Setup.Properties.Resources.Msg_FinishedSetup);
                       }
                       if ((InstallContext.Instance.IsDirverFailed || this.installer.IsRestored || !this.installer.IsFinishedBackUpFile) && flag)
                           App.WaitClickOK.WaitOne();
                       if (ncStudioMutex != null)
                       {
                           ncStudioMutex.Dispose();
                           ncStudioMutex = (Mutex)null;
                       }
                       this.installer.RunAfterInstallSuccess();
                   }
                   Application.Current.Dispatcher.BeginInvoke((Action)(() => Application.Current.Shutdown()));
               }
           });
            App.installWorker.RunWorkerAsync();
        }

        private void ShowWizardPageAtDefaultLocation(
          MainWindow_NK300.NotHaveParamHandler handler,
          WizardPageName wizardPageName)
        {
            List<CustWizardPage> wizardPageOrderList = Env.Instance.Config.WizardPageOrderList;
            // ISSUE: explicit non-virtual call
            if (wizardPageOrderList != null && (wizardPageOrderList.Count) > 0)
            {
                List<CustWizardPage> all = wizardPageOrderList?.FindAll((Predicate<CustWizardPage>)(p => p.Name.Equals(Enum.GetName(typeof(WizardPageName), (object)wizardPageName), StringComparison.OrdinalIgnoreCase)));
                if (all != null && all.Count != 0)
                    return;
                handler();
            }
            else
                handler();
        }

        private void ShowWizardPageByLocationConfig(
          WizardPageName currentWizardPage,
          WizardPageLocation location)
        {
            List<CustWizardPage> wizardPageOrderList = Env.Instance.Config.WizardPageOrderList;
            // ISSUE: explicit non-virtual call
            if (wizardPageOrderList == null || (wizardPageOrderList.Count) <= 0)
                return;
            switch (location)
            {
                case WizardPageLocation.Before:
                    wizardPageOrderList?.FindAll((Predicate<CustWizardPage>)(p => Enum.GetName(typeof(WizardPageName), (object)currentWizardPage).Equals(p.Before, StringComparison.OrdinalIgnoreCase)))?.ForEach((Action<CustWizardPage>)(o =>
                {
                    if (!"CustConfigPage".Equals(o?.Name, StringComparison.OrdinalIgnoreCase))
                        return;
                    this.ShowCustConfigPage();
                }));
                    break;
                case WizardPageLocation.After:
                    wizardPageOrderList?.FindAll((Predicate<CustWizardPage>)(p => Enum.GetName(typeof(WizardPageName), (object)currentWizardPage).Equals(p.After, StringComparison.OrdinalIgnoreCase)))?.ForEach((Action<CustWizardPage>)(o =>
                {
                    if (!"CustConfigPage".Equals(o?.Name, StringComparison.OrdinalIgnoreCase))
                        return;
                    this.ShowCustConfigPage();
                }));
                    break;
            }
        }

        private void ShowSelectedConfigPage()
        {
            App.ContinueSetup.Reset();
            this.Dispatch_Message(Setup.Properties.Resources.Msg_SelectedConfigProc);
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
           {
               WizardPage page = this.wizard.GetPage<SelectedConfigPageNK300>();
               SelectedConfigPageNK300 content = page.Content as SelectedConfigPageNK300;
               this.wizard.CurrentPage = page;
               content.tbkDescription.Text = StringParser.Parse(Setup.Properties.Resources.Msg_SelectedConfigAndStartInstall_NK300);
           }));
            App.ContinueSetup.WaitOne();
        }

        private void ShowSelectedMachinePage()
        {
            App.ContinueSetup.Reset();
            this.Dispatch_Message(Setup.Properties.Resources.Msg_SelectedMachineProc);
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
           {
               WizardPage page = this.wizard.GetPage<SelectedMachinePageNK300>();
               SelectedMachinePageNK300 content = page.Content as SelectedMachinePageNK300;
               this.wizard.CurrentPage = page;
               content.tbkDescription.Text = StringParser.Parse(Setup.Properties.Resources.Msg_SelectedMachineAndStartInstall_NK300);
           }));
            App.ContinueSetup.WaitOne();
        }

        private void ShowSelectedParamMigration()
        {
            string name = new DirectoryInfo(Env.Instance.Config.DefaultConfigDir).Name;
            string[] defaultConfigDirs = new string[0];
            if (Directory.Exists(this.config.Path))
                defaultConfigDirs = Directory.GetDirectories(this.config.Path, name + "*", SearchOption.TopDirectoryOnly);
            if (!string.IsNullOrEmpty(Env.Instance.Config.OriginalSourcePath))
            {
                InstallContext.Instance.IsReservedManufacturer = false;
                InstallContext.Instance.IsReservedUser = false;
                App.ContinueSetup.Reset();
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
               {
                   WizardPage page = this.wizard.GetPage<SelectedParamMigrationNK300>();
                   SelectedParamMigrationNK300 content = page.Content as SelectedParamMigrationNK300;
                   this.wizard.CurrentPage = page;
                   content.tbkDescription.Text = StringParser.Parse(Setup.Properties.Resources.Msg_SelectedParamMigration_NK300);
                   if (defaultConfigDirs.Length <= 1 || Directory.Exists(Env.Instance.Config.ActiveConfigDir))
                       return;
                   content.IsReservedUser.IsChecked = new bool?(false);
                   content.IsReservedUser.IsEnabled = false;
               }));
            }
            else if (!Directory.Exists(Env.Instance.Config.OEMConfigDir) && !Directory.Exists(Env.Instance.Config.ActiveConfigDir) && defaultConfigDirs.Length <= 1)
            {
                InstallContext.Instance.IsReservedManufacturer = false;
                InstallContext.Instance.IsReservedUser = false;
            }
            else
            {
                App.ContinueSetup.Reset();
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
               {
                   WizardPage page = this.wizard.GetPage<SelectedParamMigrationNK300>();
                   SelectedParamMigrationNK300 content = page.Content as SelectedParamMigrationNK300;
                   this.wizard.CurrentPage = page;
                   content.tbkDescription.Text = StringParser.Parse(Setup.Properties.Resources.Msg_SelectedParamMigration_NK300);
                   if (defaultConfigDirs.Length <= 1 || Directory.Exists(Env.Instance.Config.ActiveConfigDir))
                       return;
                   content.IsReservedUser.IsChecked = new bool?(false);
                   content.IsReservedUser.IsEnabled = false;
               }));
            }
            this.CheckFlag();
        }

        private void CheckFlag()
        {
            string name = new DirectoryInfo(Env.Instance.Config.DefaultConfigDir).Name;
            string[] strArray = new string[0];
            if (Directory.Exists(this.config.Path))
                strArray = Directory.GetDirectories(this.config.Path, name + "*", SearchOption.TopDirectoryOnly);
            bool? nullable1 = new bool?();
            while (!nullable1.HasValue)
            {
                if (Directory.Exists(Env.Instance.Config.OEMConfigDir) || Directory.Exists(Env.Instance.Config.ActiveConfigDir) || !string.IsNullOrEmpty(Env.Instance.Config.OriginalSourcePath) || !Directory.Exists(Env.Instance.Config.OEMConfigDir) && !Directory.Exists(Env.Instance.Config.ActiveConfigDir) && strArray.Length > 1)
                    App.ContinueSetup.WaitOne();
                nullable1 = Installer.Instance.CheckFlag();
                bool? nullable2 = nullable1;
                bool flag = false;
                if (nullable2.GetValueOrDefault() == flag & nullable2.HasValue)
                    Application.Current.Dispatcher.Invoke((Action)(() => this.Close()));
            }
        }

        private void ShowCustConfigPage()
        {
            InstallContext installContext = InstallContext.Instance;
            int pageIndex = 0;
            bool flag = true;
            installContext.InitCustConfigList();
            if (installContext.CustomizePageList == null || installContext.CustomizePageList.Count <= 0)
                return;
            while (flag)
            {
                CustomizePage page1 = installContext.CustomizePageList.Find((Predicate<CustomizePage>)(o => o.Index == pageIndex));
                if (page1 != null)
                {
                    InstallContext.ResetCustConfigContext(installContext, page1);
                    App.ContinueSetup.Reset();
                    Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                   {
                       WizardPage page2 = this.wizard.GetPage<CustSelectedConfigPageNK300>();
                       if (page2 == null)
                           return;
                       this.wizard.CurrentPage = page2;
                       this.wizard.CurrentPage.Title = installContext.CurrentCustomizePage.Text;
                       this.wizard.CurrentPage.Description = installContext.CurrentCustomizePage.Description;
                   }));
                    App.ContinueSetup.WaitOne();
                }
                else
                    flag = false;
                pageIndex++;
            }
        }

        private void GotoSetupPage() => Application.Current.Dispatcher.BeginInvoke((Action)(() => this.wizard.CurrentPage = this.wizard.Items[0] as WizardPage));

        private void MainWindow_NK300_Closing(object sender, CancelEventArgs e)
        {
            Installer.Instance?.DealOriginalSourcePath();
            if (this.installer.CanClose())
            {
                if (!Installer.Instance.IsBeginInstall || Installer.Instance.IsSucceed || this.installer.IsRestored)
                    return;
                App.installWorker.CancelAsync();
                App.AsynCancellationToken.Cancel();
                e.Cancel = true;
            }
            else if (!Installer.Instance.CanCloseWindow)
                e.Cancel = true;
            else if (TempDialog.Show(Application.Current.MainWindow, Setup.Properties.Resources.Msg_IsClose, Setup.Properties.Resources.Msg_Warning, TempDialogButton.YesNo) == TempDialogButton.No)
            {
                e.Cancel = true;
            }
            else
            {
                App.installWorker.CancelAsync();
                App.AsynCancellationToken.Cancel();
                if (!Installer.Instance.IsSucceed && !this.installer.IsRestored)
                    e.Cancel = true;
                else
                    Installer.Instance.IsFinished = true;
            }
        }

        private void Dispatch_CloseNcstudio()
        {
            foreach (Process process in Process.GetProcessesByName(this.config.AppName))
            {
                process.CloseMainWindow();
                Thread.Sleep(1000);
                if (!process.HasExited)
                    process.Kill();
            }
        }

        private void Dispatch_CloseProcess()
        {
            string lower = Directory.GetParent(this.config.Path).FullName.ToLower();
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainModule.FileName.ToLower().Contains(lower))
                        process.Kill();
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void Dispatch_Close() => Application.Current.Dispatcher.Invoke((Action)(() => this.Close()));

        private bool Dispatch_Confirm()
        {
            bool ret = false;
            Application.Current.Dispatcher.Invoke((Action)(() => ret = TempDialog.Show(Application.Current.MainWindow, Setup.Properties.Resources.Msg_EnsureInstalling, Setup.Properties.Resources.Msg_SetupWarning, TempDialogButton.YesNo, TempDialogImage.Warning) == TempDialogButton.Yes));
            return ret;
        }

        private void Dispatch_Message(string s) => Application.Current.Dispatcher.BeginInvoke((Action)(() =>
       {
           ListViewItem newItem = new ListViewItem()
           {
               Height = 25.0,
               Content = (object)s
           };
           this.lv.Items.Add((object)newItem);
           this.lv.SelectedItem = (object)newItem;
           this.lv.ScrollIntoView((object)newItem);
       }));

        private void StopUpdateSchedule()
        {
            this.worker.CancelAsync();
            this.autoReset.WaitOne();
            if (this.worker == null)
                return;
            this.worker.Dispose();
            this.worker = (BackgroundWorker)null;
        }

        private void Dispatch_Schedule(string s)
        {
            ListViewItem listViewItem = (ListViewItem)null;
            this.worker = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true
            };
            this.worker.DoWork += (DoWorkEventHandler)((o, e) =>
           {
               int num = 0;
               string content = s;
               while (this.worker != null && !this.worker.CancellationPending)
               {
                   ++num;
                   if (num % 3 == 0)
                   {
                       num = 0;
                       content = s + ".";
                   }
                   else
                       content += ".";
                   Application.Current.Dispatcher.BeginInvoke((Action)(() => listViewItem.Content = (object)content));
                   Thread.Sleep(500);
               }
               e.Cancel = true;
           });
            this.worker.RunWorkerCompleted += (RunWorkerCompletedEventHandler)((o, e) =>
           {
               Application.Current.Dispatcher.BeginInvoke((Action)(() => listViewItem.Content = (object)(s + "...")));
               this.autoReset.Set();
           });
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
           {
               listViewItem = new ListViewItem()
               {
                   Height = 25.0,
                   Content = (object)s
               };
               this.lv.Items.Add((object)listViewItem);
               this.lv.SelectedItem = (object)listViewItem;
               this.lv.ScrollIntoView((object)listViewItem);
           }));
            this.worker.RunWorkerAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (this.autoReset != null)
            {
                this.autoReset.Dispose();
                this.autoReset = (AutoResetEvent)null;
            }
            if (this.worker == null)
                return;
            this.worker.Dispose();
            this.worker = (BackgroundWorker)null;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private void RaiseOKEvent(object sender, RoutedEventArgs e) => (this.wizard.CurrentPage.Content as UIElement).RaiseEvent(new RoutedEventArgs(MainWindow_NK300.OKEvent, this.wizard.CurrentPage.Content)
        {
            Source = this.wizard.CurrentPage.Content
        });

        private void RaiseCancelEvent(object sender, RoutedEventArgs e) => (this.wizard.CurrentPage.Content as UIElement).RaiseEvent(new RoutedEventArgs(MainWindow_NK300.CancelEvent, this.wizard.CurrentPage.Content)
        {
            Source = this.wizard.CurrentPage.Content
        });

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.F7 || e.Key == Key.Return)
            {
                if (this.OKButton.IsEnabled)
                    this.RaiseOKEvent((object)this, (RoutedEventArgs)e);
                e.Handled = true;
            }
            else
            {
                if (e.Key != Key.F8 && e.Key != Key.Escape)
                    return;
                if (this.CancelButton.IsEnabled)
                    this.RaiseCancelEvent((object)this, (RoutedEventArgs)e);
                e.Handled = true;
            }
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/mainwindow_nk300.xaml", UriKind.Relative));
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
                    this.wizard = (Wizard)target;
                    break;
                case 2:
                    this.installPage = (WizardPage)target;
                    break;
                case 3:
                    this.lv = (InstallListView)target;
                    break;
                case 4:
                    this.SelectConfig = (WizardPage)target;
                    break;
                case 5:
                    this.Machine = (WizardPage)target;
                    break;
                case 6:
                    this.CustConfigPage = (WizardPage)target;
                    break;
                case 7:
                    this.Parameter = (WizardPage)target;
                    break;
                case 8:
                    this.OKButton = (Button)target;
                    this.OKButton.Click += new RoutedEventHandler(this.RaiseOKEvent);
                    break;
                case 9:
                    this.CancelButton = (Button)target;
                    this.CancelButton.Click += new RoutedEventHandler(this.RaiseCancelEvent);
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }

        internal delegate void NotHaveParamHandler();
    }
}
