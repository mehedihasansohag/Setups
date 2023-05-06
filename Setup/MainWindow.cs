// Decompiled with JetBrains decompiler
// Type: Setup.MainWindow
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
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Setup
{
    public  class MainWindow : Window, IComponentConnector
    {
        private bool _contentLoaded;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Closing += new CancelEventHandler(this.MainWindow_Closing);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Installer.Instance?.DealOriginalSourcePath();
            if (Installer.Instance.CanClose())
            {
                if (!Installer.Instance.IsBeginInstall || Installer.Instance.IsSucceed || Installer.Instance.IsRestored)
                    return;
                App.installWorker.CancelAsync();
                App.AsynCancellationToken.Cancel();
                e.Cancel = true;
            }
            else if (!Installer.Instance.CanCloseWindow)
                e.Cancel = true;
            else if (MessageBox.Show(Application.Current.MainWindow, Setup.Properties.Resources.Msg_IsClose, Setup.Properties.Resources.Msg_Warning, MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                App.installWorker.CancelAsync();
                App.AsynCancellationToken.Cancel();
                if (!Installer.Instance.IsSucceed && !Installer.Instance.IsRestored)
                {
                    e.Cancel = true;
                }
                else
                {
                    Installer.Instance.IsFinished = true;
                    string str = Path.Combine(Env.Instance.FirstRunDir, "firstrun.exe");
                    if (!File.Exists(str))
                        return;
                    Process.Start(str);
                }
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (Wizard.Instance != null)
            {
                Wizard.Instance.CurrentPage = Wizard.Instance.Items[0] as WizardPage;
                Application.Current.MainWindow.Content = (object)Wizard.Instance;
                Wizard instance = Wizard.Instance;
                if (!InstallContext.Instance.IsMultipleConfig)
                    instance.RemovePage<SelectedConfigPage>();
                if (!InstallContext.Instance.IsMultipleMachine)
                    instance.RemovePage<SelectedMachinePage>();
                string name = new DirectoryInfo(Env.Instance.Config.DefaultConfigDir).Name;
                string[] strArray = new string[0];
                if (Directory.Exists(Env.Instance.Config.Path))
                    strArray = Directory.GetDirectories(Env.Instance.Config.Path, name + "*", SearchOption.TopDirectoryOnly);
                if (!string.IsNullOrEmpty(Env.Instance.Config.OriginalSourcePath))
                {
                    InstallContext.Instance.IsReservedManufacturer = false;
                    InstallContext.Instance.IsReservedUser = false;
                }
                else if (!Directory.Exists(Env.Instance.Config.OEMConfigDir) && !Directory.Exists(Env.Instance.Config.ActiveConfigDir))
                {
                    SelectedParamMigration selectedParamMigration = (SelectedParamMigration)null;
                    WizardPage page = instance.GetPage<SelectedParamMigration>();
                    if (page != null)
                        selectedParamMigration = page.Content as SelectedParamMigration;
                    if (strArray.Length <= 1)
                    {
                        InstallContext.Instance.IsReservedManufacturer = false;
                        InstallContext.Instance.IsReservedUser = false;
                        if (page != null)
                            Wizard.Instance.Items.Remove((object)page);
                    }
                    else
                    {
                        InstallContext.Instance.IsReservedUser = false;
                        if (selectedParamMigration != null)
                            selectedParamMigration.userParameter.IsEnabled = false;
                    }
                }
            }
            base.OnInitialized(e);
        }

        private void WizardPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is WizardPage wizardPage))
                return;
            if (wizardPage.PageType == WizardPageType.Exterior)
            {
                BitmapSource bitmapSource = App.GetBitmapSource("ExteriorImage");
                if (bitmapSource != null)
                {
                    wizardPage.ExteriorPanelBackground = (Brush)new ImageBrush((ImageSource)bitmapSource);
                }
                else
                {
                    Uri bitmapUri = new Uri("pack://application:,,,/ico/ExteriorImage.bmp", UriKind.RelativeOrAbsolute);
                    wizardPage.ExteriorPanelBackground = (Brush)new ImageBrush((ImageSource)BitmapFrame.Create(bitmapUri));
                }
            }
            else
            {
                if (wizardPage.PageType != WizardPageType.Interior)
                    return;
                BitmapSource bitmapSource = App.GetBitmapSource("SelectConfig");
                if (bitmapSource != null)
                {
                    wizardPage.HeaderImage = (ImageSource)bitmapSource;
                }
                else
                {
                    Uri bitmapUri = new Uri("pack://application:,,,/ico/SelectConfig.png", UriKind.RelativeOrAbsolute);
                    wizardPage.HeaderImage = (ImageSource)BitmapFrame.Create(bitmapUri);
                }
            }
        }

        private void Wizard_Next(object sender, Wizard.CancelRoutedEventArgs e)
        {
            if (Wizard.Instance == null || Wizard.Instance.CurrentPage == null)
                return;
            InstallContext instance = InstallContext.Instance;
            WizardPage nextPage = Wizard.Instance.GetNextPage();
            if (Wizard.Instance.CurrentPage?.Content is CustSelectedConfigPage)
            {
                instance.CustPageNextPage = Wizard.Instance.GetNextPage();
                List<CustomizePage> customizePageList = instance.CustomizePageList;
                // ISSUE: explicit non-virtual call
                if ((customizePageList != null ? ((customizePageList.Count) > 0 ? 1 : 0) : 0) != 0 && instance.CurrentCustomizePage != null)
                {
                    int pageIndex = instance.CurrentCustomizePage.Index + 1;
                    CustomizePage page = instance.CustomizePageList.Find((Predicate<CustomizePage>)(o => o.Index == pageIndex));
                    if (page != null)
                    {
                        InstallContext.ResetCustConfigContext(instance, page);
                        WizardPage wizardPageByContent = Wizard.Instance.GetWizardPageByContent<CustSelectedConfigPage>();
                        if (wizardPageByContent != null)
                        {
                            Wizard.Instance.CurrentPage.Title = instance.CurrentCustomizePage.Text;
                            Wizard.Instance.CurrentPage.Description = instance.CurrentCustomizePage.Description;
                            Wizard.Instance.CurrentPage.NextPage = wizardPageByContent;
                            Wizard.Instance.CurrentPage.CanFinish = new bool?(false);
                        }
                    }
                    else
                        Wizard.Instance.CurrentPage.NextPage = (WizardPage)null;
                }
            }
            else if (nextPage?.Content is CustSelectedConfigPage)
            {
                instance.CustPagePreviousPage = Wizard.Instance.CurrentPage;
                if (Env.Instance.Config.dicBatFileParam == null)
                    Env.Instance.Config.dicBatFileParam = new Dictionary<string, string>();
                CustomizePagesInfo customPagesInfo = CustPageHelper.CustomPagesInfo;
                int num1;
                if (customPagesInfo == null)
                {
                    num1 = 0;
                }
                else
                {
                    int? count = customPagesInfo.PageList?.Count;
                    int num2 = 0;
                    num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
                }
                if (num1 != 0)
                {
                    instance.InitCustConfigList();
                    List<CustomizePage> customizePageList = instance.CustomizePageList;
                    // ISSUE: explicit non-virtual call
                    if ((customizePageList != null ? ((customizePageList.Count) > 0 ? 1 : 0) : 0) != 0)
                    {
                        int pageIndex = 0;
                        if (instance.CurrentCustomizePage != null)
                            pageIndex = instance.CurrentCustomizePage.Index + 1;
                        CustomizePage page = instance.CustomizePageList.Find((Predicate<CustomizePage>)(o => o.Index == pageIndex));
                        if (page != null)
                        {
                            InstallContext.ResetCustConfigContext(instance, page);
                            WizardPage wizardPageByContent = Wizard.Instance.GetWizardPageByContent<CustSelectedConfigPage>();
                            if (wizardPageByContent != null)
                            {
                                wizardPageByContent.Title = instance.CurrentCustomizePage.Text;
                                wizardPageByContent.Description = instance.CurrentCustomizePage.Description;
                                Wizard.Instance.CurrentPage.NextPage = wizardPageByContent;
                                Wizard.Instance.CurrentPage.CanFinish = new bool?(false);
                            }
                        }
                    }
                    else
                        MainWindow.GotoNextPageOverPage(nextPage);
                }
                else
                    MainWindow.GotoNextPageOverPage(nextPage);
            }
            if (Wizard.Instance.CurrentPage?.Content is SelectedParamMigration)
            {
                bool? nullable1 = Installer.Instance.CheckFlag();
                bool? nullable2 = nullable1;
                bool flag = false;
                if (nullable2.GetValueOrDefault() == flag & nullable2.HasValue)
                {
                    this.Close();
                }
                else
                {
                    if (nullable1.HasValue)
                        return;
                    e.Cancel = true;
                }
            }
            else
            {
                if (!(Wizard.Instance.CurrentPage?.Content is SelectedMachinePage))
                    return;
                string str1 = "";
                MachineEntity setupedMachine;
                if (!Installer.Instance.CheckMachine(out setupedMachine))
                {
                    string warMisMatch = Setup.Properties.Resources.War_MisMatch;
                    bool flag = setupedMachine == null || string.IsNullOrWhiteSpace(setupedMachine.DisplayName);
                    MachineEntity machineSelectedValue = InstallContext.Instance.MachineSelectedValue;
                    string str2 = machineSelectedValue.DisplayName;
                    if (setupedMachine != null && machineSelectedValue != null && (string.IsNullOrWhiteSpace(setupedMachine.MachineModel) || string.IsNullOrWhiteSpace(machineSelectedValue.MachineModel)) && string.Compare(setupedMachine.EnName, machineSelectedValue.EnName, true) == 0 && string.Compare(setupedMachine.ZhName, machineSelectedValue.ZhName, true) != 0)
                    {
                        str1 = setupedMachine.EnName + "(" + setupedMachine.ZhName + ")";
                        str2 = machineSelectedValue.EnName + "(" + machineSelectedValue.ZhName + ")";
                    }
                    string empty = string.Empty;
                    string messageBoxText;
                    if (flag)
                    {
                        str1 = Setup.Properties.Resources.Msg_Unrecognized;
                        string warNotFound = Setup.Properties.Resources.War_NotFound;
                        messageBoxText = Setup.Properties.Resources.tbx_TargetMachine + str2 + Environment.NewLine + warNotFound;
                    }
                    else
                        messageBoxText = Setup.Properties.Resources.tbx_LocalMachine + setupedMachine.DisplayName + Environment.NewLine + Setup.Properties.Resources.tbx_TargetMachine + str2 + Environment.NewLine + warMisMatch;
                    switch (MessageBox.Show(Application.Current.MainWindow, messageBoxText, Setup.Properties.Resources.Msg_Warning, MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation))
                    {
                        case MessageBoxResult.Cancel:
                            e.Cancel = true;
                            break;
                        case MessageBoxResult.No:
                            Application.Current.MainWindow.Close();
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(setupedMachine?.MachineModel))
                            {
                                Installer.Instance.MachineCheckResult = MachineCheckResultEnum.InstallIsNull;
                                break;
                            }
                            Installer.Instance.MachineCheckResult = MachineCheckResultEnum.SelectMismatch;
                            break;
                    }
                }
                else
                    Installer.Instance.MachineCheckResult = MachineCheckResultEnum.Pass;
            }
        }

        private void Wizard_Previous(object sender, Wizard.CancelRoutedEventArgs e)
        {
            if (Wizard.Instance == null || Wizard.Instance.CurrentPage == null)
                return;
            InstallContext instance = InstallContext.Instance;
            WizardPage currentPage = Wizard.Instance.CurrentPage;
            WizardPage wizardPageByContent = Wizard.Instance.GetWizardPageByContent<CustSelectedConfigPage>();
            if (instance.CustPagePreviousPage != null && !(instance.CustPagePreviousPage?.NextPage?.Content is CustSelectedConfigPage))
            {
                instance.CustPagePreviousPage.NextPage = wizardPageByContent;
                instance.CustPagePreviousPage.CanFinish = new bool?(false);
            }
            if (instance.CustPageNextPage != null && !(instance.CustPageNextPage?.PreviousPage?.Content is CustSelectedConfigPage))
            {
                instance.CustPageNextPage.PreviousPage = wizardPageByContent;
                instance.CustPageNextPage.CanFinish = new bool?(false);
            }
            if (currentPage?.Content is CustSelectedConfigPage)
            {
                List<CustomizePage> customizePageList = instance.CustomizePageList;
                // ISSUE: explicit non-virtual call
                if ((customizePageList != null ? ((customizePageList.Count) > 0 ? 1 : 0) : 0) != 0)
                {
                    CustomizePage currentCustomizePage = instance.CurrentCustomizePage;
                    if ((currentCustomizePage != null ? (currentCustomizePage.Index > 0 ? 1 : 0) : 0) != 0)
                    {
                        int pageIndex = instance.CurrentCustomizePage.Index - 1;
                        CustomizePage page = instance.CustomizePageList.Find((Predicate<CustomizePage>)(o => o.Index == pageIndex));
                        InstallContext.ResetCustConfigContext(instance, page);
                        if (wizardPageByContent != null)
                        {
                            Wizard.Instance.CurrentPage.Title = instance.CurrentCustomizePage.Text;
                            Wizard.Instance.CurrentPage.Description = instance.CurrentCustomizePage.Description;
                            Wizard.Instance.CurrentPage.PreviousPage = wizardPageByContent;
                            Wizard.Instance.CurrentPage.CanFinish = new bool?(false);
                            return;
                        }
                        MainWindow.GotoPreviousPageOverPage(wizardPageByContent);
                        return;
                    }
                }
                MainWindow.GotoPreviousPageOverPage(wizardPageByContent);
            }
            else
            {
                if (!(Wizard.Instance.GetPreviousPage()?.Content is CustSelectedConfigPage) || instance.HasCustomizePage)
                    return;
                MainWindow.GotoPreviousPageOverPage(wizardPageByContent);
            }
        }

        private static void GotoPreviousPageOverPage(WizardPage previousPage)
        {
            int index = Wizard.Instance.Items.IndexOf((object)previousPage) - 1;
            if (index < 0 || index >= Wizard.Instance.Items.Count)
                return;
            Wizard.Instance.CurrentPage.PreviousPage = Wizard.Instance.Items[index] as WizardPage;
            Wizard.Instance.CurrentPage.CanFinish = new bool?(false);
        }

        private static void GotoNextPageOverPage(WizardPage nextPage)
        {
            int index = Wizard.Instance.Items.IndexOf((object)nextPage) + 1;
            if (index >= Wizard.Instance.Items.Count)
                return;
            Wizard.Instance.CurrentPage.NextPage = Wizard.Instance.Items[index] as WizardPage;
            Wizard.Instance.CurrentPage.CanFinish = new bool?(false);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/mainwindow.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, (object)this, handler);

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target) => this._contentLoaded = true;
    }
}
