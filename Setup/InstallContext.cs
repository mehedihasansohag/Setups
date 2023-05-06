// Decompiled with JetBrains decompiler
// Type: Setup.InstallContext
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library;
using Packup.Library.Condition;
using Packup.Library.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace Setup
{
    public class InstallContext : INotifyPropertyChanged
    {
        private const string DISPLAYNAME = "DisplayName";
        private const string DIRECTORYNAME = "DirectoryName";
        private const string MACHINELIST = "MachineList";
        private const string MACHINEMODEL = "MachineModel";
        private const string MACHINE = "Machine";
        private const string CONFIGLIST = "ConfigList";
        private const string CONFIG = "Config";
        private static InstallContext instance;
        private ConfigEntity selectedValue;
        private MachineEntity machineSelectedValue;
        private List<CustomizePage> customizePageList = new List<CustomizePage>();
        private string custConfigSelected = string.Empty;
        private CustomizeList custPageList;
        private List<CustomizeItem> custPageListItems;
        private CustomizeOption custPageOption;
        private List<CustomizeItem> custPageOptionsItems;
        private CustomizeDropDown custPageDropDown;
        private List<CustomizeItem> custDropDownsItems;
        private CustomizeItem customizeSelectedValue;
        private string parameterTip = string.Empty;
        private bool isReservedManufacturerFail;
        private bool isReservedUserFail;
        private bool isReservedManufacturer = true;
        private bool isReservedUser = true;
        private bool isdirverfailed;
        private string installpath = "C:\\Program Files\\Weihong\\NcStudio";
        private bool isCreateShotCut = true;
        private bool isNcCloudAdapterSetUp = true;
        private bool isAutoRestart;
        private bool isAutoRestartNoDesktop;
        private bool isAutoStart;
        private string installDescription_NK300;
        private string installTitle_NK300;
        private string currentText = string.Empty;
        private string subcurrentText = string.Empty;
        private int progressValue;
        private string openedProcessList = string.Empty;
        private string installFailedMsg = string.Empty;
        private string thirdPartyFailedMsg = string.Empty;

        public static InstallContext Instance
        {
            get
            {
                if (InstallContext.instance == null)
                    InstallContext.instance = new InstallContext();
                return InstallContext.instance;
            }
        }

        internal ConfigInfo Config => Env.Instance.Config;

        public bool Init()
        {
            this.IsReservedManufacturer = this.Config.IsReservedManufacturer;
            this.IsReservedUser = this.Config.IsReservedUser;
            this.IsCreateShotCut = this.Config.IsCreateShortcut;
            this.IsNcCloudAdapterSetUp = this.Config.IsInstallNcCloud;
            if (this.Config.AutoStartType == AutoStartType.None)
            {
                this.IsAutoStart = false;
                this.IsAutoRestart = false;
                this.IsAutoRestartNoDesktop = false;
            }
            else if (this.Config.AutoStartType == AutoStartType.Complate)
            {
                this.IsAutoStart = true;
                this.IsAutoRestart = true;
                this.IsAutoRestartNoDesktop = false;
            }
            else if (this.Config.AutoStartType == AutoStartType.Exclusive)
            {
                this.IsAutoStart = true;
                this.IsAutoRestart = false;
                this.IsAutoRestartNoDesktop = true;
            }
            this.InitMachineList();
            this.InitConfigList();
            this.InitCustPageConfigInfoLocation();
            return true;
        }

        private void InitCustPageConfigInfoLocation()
        {
            List<CustWizardPage> wizardPageOrderList = Env.Instance.Config.WizardPageOrderList;
            // ISSUE: explicit non-virtual call
            if (wizardPageOrderList == null || (wizardPageOrderList.Count) <= 0)
                return;
            CustWizardPage custWizardPage = wizardPageOrderList.Find((Predicate<CustWizardPage>)(o => o.Name.Equals("CustConfigPage", StringComparison.OrdinalIgnoreCase)));
            this.CustomizePageShowAfterConfigPage = ("SelectConfig".Equals(custWizardPage?.After, StringComparison.OrdinalIgnoreCase) || "SelectConfig".Equals(custWizardPage?.Before, StringComparison.OrdinalIgnoreCase)) && this.IsMultipleConfig;
            this.CustomizePageShowAfterMachinePage = ("Machine".Equals(custWizardPage?.After, StringComparison.OrdinalIgnoreCase) || "Machine".Equals(custWizardPage?.Before, StringComparison.OrdinalIgnoreCase)) && this.IsMultipleMachine;
            this.CustomizePageShow = !this.CustomizePageShowAfterConfigPage && !this.CustomizePageShowAfterMachinePage;
        }

        public List<ConfigEntity> ConfigList { get; } = new List<ConfigEntity>();

        internal bool IsMultipleConfig => this.ConfigList.Count >= 2;

        public ConfigEntity SelectedValue
        {
            get => this.selectedValue;
            set
            {
                if (this.selectedValue == value)
                    return;
                this.selectedValue = value;
                this.OnPropertyChanged("SelectedItem");
                this.OnPropertyChanged("SelectedConfig");
                this.OnPropertyChanged("SelectedConfigVisibility");
            }
        }

        public string SelectedConfig => this.selectedValue != null ? this.selectedValue.DisplayName : string.Empty;

        public Visibility SelectedConfigVisibility => this.selectedValue != null ? Visibility.Visible : Visibility.Collapsed;

        public List<MachineEntity> MachineList { get; } = new List<MachineEntity>();

        internal bool IsMultipleMachine => this.MachineList.Count >= 2;

        public MachineEntity MachineSelectedValue
        {
            get => this.machineSelectedValue;
            set
            {
                if (this.machineSelectedValue == value)
                    return;
                this.machineSelectedValue = value;
                this.OnPropertyChanged("MachineSelectedItem");
                this.OnPropertyChanged("MachineSelectedConfig");
                this.OnPropertyChanged("MachineSelectedConfigVisibility");
            }
        }

        public string MachineSelectedConfig => this.machineSelectedValue != null ? this.machineSelectedValue.DisplayName : string.Empty;

        public Visibility MachineSelectedConfigVisibility => this.machineSelectedValue != null && this.IsMultipleMachine ? Visibility.Visible : Visibility.Collapsed;

        internal WizardPage CustPagePreviousPage { get; set; }

        internal WizardPage CustPageNextPage { get; set; }

        internal void InitCustConfigList()
        {
            try
            {
                string displayName1 = Instance.SelectedValue?.DisplayName;
                string displayName2 = Instance.machineSelectedValue?.DisplayName;
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                if (Instance.ConfigList.Count > 1)
                    dictionary.Add("DefaultConfig", displayName1);
                if (Instance.MachineList.Count > 1)
                    dictionary.Add("OEMConfig", displayName2);
                InstallContext instance = Instance;
                instance.CurrentCustomizePage = null;
                int num = 0;
                List<CustomizePage> customizePageList = new List<CustomizePage>();
                foreach (CustomizePage page in CustPageHelper.CustomPagesInfo.PageList)
                {
                    if (dictionary.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                        {
                            if (VisiblityCondition.GetFailedAction(page.conditions.FindAll((Predicate<ICondition>)(o => o.Action == ConditionFailedAction.Visible)), (object)keyValuePair.Key, (object)keyValuePair.Value) == ConditionFailedAction.Visible)
                            {
                                page.Index = num;
                                customizePageList.Add(page);
                                ++num;
                                break;
                            }
                        }
                    }
                    else
                    {
                        List<ICondition> all = page.conditions.FindAll(o => o.Action == ConditionFailedAction.Visible);
                        if (all == null || all.Count == 0)
                        {
                            page.Index = num;
                            customizePageList.Add(page);
                            ++num;
                        }
                    }
                }
                instance.CustomizePageList = customizePageList;
            }
            catch
            {
                throw new Exception("Setup.config 配置文件内容异常。 ");
            }
        }

        public bool HasCustomizePage => this.CustomizePageList != null && this.CustomizePageList.Count > 0 && Env.Instance.Config.InstallType != InstallType.AIO;

        internal bool CustomizePageShowAfterConfigPage { get; set; }

        internal bool CustomizePageShowAfterMachinePage { get; set; }

        internal bool CustomizePageShow { get; set; }

        public List<CustomizePage> CustomizePageList
        {
            get => customizePageList;
            set
            {
                customizePageList = value;
                OnPropertyChanged("CustConfigSelected");
            }
        }

        public Visibility CustPageListVisibility => this.CustPageListItems != null && this.CustPageListItems.Count != 0 ? Visibility.Visible : Visibility.Collapsed;

        public string CustConfigSelected
        {
            get
            {
                List<CustomizePage> customizePageList = this.CustomizePageList;
                if ((customizePageList != null ? (customizePageList.Count > 0 ? 1 : 0) : 0) != 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int index = 0; index < CustomizePageList.Count; ++index)
                    {
                        CustomizePage customizePage = CustomizePageList[index];
                        if (!string.IsNullOrEmpty(customizePage.CustomizeSelectedValue?.Text))
                        {
                            if (index != CustomizePageList.Count - 1)
                            {
                                stringBuilder.Append(customizePage.CustomizeSelectedValue.Text + Environment.NewLine);

                            }
                            else
                            {
                                stringBuilder.Append(customizePage.CustomizeSelectedValue.Text);

                            }
                        }
                    }
                    custConfigSelected = stringBuilder.ToString();
                }
                else
                    custConfigSelected = string.Empty;
                return custConfigSelected;
            }
            set
            {
                custConfigSelected = value;
                OnPropertyChanged(nameof(CustConfigSelected));
            }
        }

        public CustomizeList CustPageList
        {
            get => custPageList;
            set
            {
               custPageList = value;
                OnPropertyChanged(nameof(CustPageList));
            }
        }

        public List<CustomizeItem> CustPageListItems
        {
            get => custPageListItems;
            set
            {
                custPageListItems = value;
                OnPropertyChanged(nameof(CustPageListItems));
                OnPropertyChanged("CustPageListVisibility");
            }
        }

        public Visibility CustPageOptionVisibility => this.CustPageOptionsItems != null && this.CustPageOptionsItems.Count != 0 ? Visibility.Visible : Visibility.Collapsed;

        public CustomizeOption CustPageOption
        {
            get =>custPageOption;
            set
            {
                custPageOption = value;
                OnPropertyChanged(nameof(CustPageOption));
            }
        }

        public List<CustomizeItem> CustPageOptionsItems
        {
            get => this.custPageOptionsItems;
            set
            {
                this.custPageOptionsItems = value;
                this.OnPropertyChanged(nameof(CustPageOptionsItems));
                this.OnPropertyChanged("CustPageOptionVisibility");
            }
        }

        public Visibility CustDropDownVisibility => this.CustDropDownsItems != null && this.CustDropDownsItems.Count != 0 ? Visibility.Visible : Visibility.Collapsed;

        public CustomizeDropDown CustPageDropDown
        {
            get => this.custPageDropDown;
            set
            {
                this.custPageDropDown = value;
                this.OnPropertyChanged(nameof(CustPageDropDown));
            }
        }

        public List<CustomizeItem> CustDropDownsItems
        {
            get => this.custDropDownsItems;
            set
            {
                this.custDropDownsItems = value;
                this.OnPropertyChanged(nameof(CustDropDownsItems));
                this.OnPropertyChanged("CustDropDownVisibility");
            }
        }

        public CustomizePage CurrentCustomizePage { get; set; } = new CustomizePage();

        public CustomizeItem CustomizeSelectedValue
        {
            get => this.customizeSelectedValue;
            set
            {
                if (this.customizeSelectedValue == value)
                    return;
                if (value != null)
                {
                    if (this.Config.dicBatFileParam == null)
                        this.Config.dicBatFileParam = new Dictionary<string, string>();
                    if (this.CustPageListVisibility == Visibility.Visible && this.CustPageList != null && !string.IsNullOrEmpty(this.CustPageList.Key))
                    {
                        if (this.Config.dicBatFileParam.ContainsKey(this.CustPageList.Key))
                            this.Config.dicBatFileParam.Remove(this.CustPageList.Key);
                        this.Config.dicBatFileParam.Add(this.CustPageList.Key, value.Key);
                    }
                    if (this.CustPageOptionVisibility == Visibility.Visible && this.CustPageOption != null && !string.IsNullOrEmpty(this.CustPageOption.Key))
                    {
                        if (this.Config.dicBatFileParam.ContainsKey(this.CustPageOption.Key))
                            this.Config.dicBatFileParam.Remove(this.CustPageOption.Key);
                        this.Config.dicBatFileParam.Add(this.CustPageOption.Key, value.Key);
                    }
                    if (this.CustDropDownVisibility == Visibility.Visible && this.CustPageDropDown != null && !string.IsNullOrEmpty(this.CustPageDropDown.Key))
                    {
                        if (this.Config.dicBatFileParam.ContainsKey(this.CustPageDropDown.Key))
                            this.Config.dicBatFileParam.Remove(this.CustPageDropDown.Key);
                        this.Config.dicBatFileParam.Add(this.CustPageDropDown.Key, value.Key);
                    }
                }
                this.customizeSelectedValue = value;
                if (this.custPageOptionsItems != null)
                {
                    foreach (CustomizeItem custPageOptionsItem in this.custPageOptionsItems)
                        custPageOptionsItem.IsChecked = false;
                    if (this.customizeSelectedValue != null)
                        this.customizeSelectedValue.IsChecked = true;
                }
                if (this.CurrentCustomizePage != null)
                    this.CurrentCustomizePage.CustomizeSelectedValue = this.customizeSelectedValue;
                this.OnPropertyChanged("CustConfigSelected");
                this.OnPropertyChanged("CustConfigPageVisibilty");
                this.OnPropertyChanged("CustConfigPageAfterConfigPageVisibilty");
                this.OnPropertyChanged("CustConfigPageAfterMachinePageVisibilty");
                this.OnPropertyChanged(nameof(CustomizeSelectedValue));
                this.OnPropertyChanged("CustomizeSelectedConfig");
                this.OnPropertyChanged("CustomizeSelectedConfigVisibility");
            }
        }

        public string CustomizeSelectedConfig => this.customizeSelectedValue != null ? this.customizeSelectedValue.Text : string.Empty;

        public Visibility CustomizeSelectedConfigVisibility => this.customizeSelectedValue != null ? Visibility.Visible : Visibility.Collapsed;

        public static void ResetCustConfigContext(InstallContext installContext, CustomizePage page)
        {
            if (page == null)
                return;
            installContext.CurrentCustomizePage = page;
            installContext.CurrentCustomizePage.Text = PathLib.GetLocalString(installContext.CurrentCustomizePage.Text);
            installContext.CurrentCustomizePage.Description = PathLib.GetLocalString(installContext.CurrentCustomizePage.Description);
            CustomizeItem customizeSelectedValue = page.CustomizeSelectedValue;
            installContext.CustPageList = (CustomizeList)null;
            installContext.CustPageListItems = (List<CustomizeItem>)null;
            installContext.CustPageOption = (CustomizeOption)null;
            installContext.CustPageOptionsItems = (List<CustomizeItem>)null;
            installContext.CustPageDropDown = (CustomizeDropDown)null;
            installContext.CustDropDownsItems = (List<CustomizeItem>)null;
            installContext.CustPagePreviousPage = (WizardPage)null;
            installContext.CustPageNextPage = (WizardPage)null;
            if (page.List != null && page.List.Count > 0)
            {
                installContext.CustPageList = page.List[0];
                string localString = PathLib.GetLocalString(installContext.CustPageList.Text);
                installContext.CustPageList.Text = localString;
                installContext.CustPageListItems = page.List[0].Items;
                if (installContext.CustPageListItems == null || installContext.CustPageListItems.Count <= 0)
                    return;
                installContext.CustPageListItems.ForEach((Action<CustomizeItem>)(o => o.Text = PathLib.GetLocalString(o.Text)));
                if (customizeSelectedValue != null)
                    installContext.CustomizeSelectedValue = customizeSelectedValue;
                else
                    installContext.CustomizeSelectedValue = installContext.CustPageListItems[0];
            }
            else if (page.Options != null && page.Options.Count > 0)
            {
                installContext.CustPageOption = page.Options[0];
                string localString = PathLib.GetLocalString(installContext.CustPageOption.Text);
                installContext.CustPageOption.Text = localString;
                installContext.CustPageOptionsItems = page.Options[0].Items;
                if (installContext.CustPageOptionsItems == null || installContext.CustPageOptionsItems.Count <= 0)
                    return;
                installContext.CustPageOptionsItems.ForEach((Action<CustomizeItem>)(o => o.Text = PathLib.GetLocalString(o.Text)));
                if (customizeSelectedValue != null)
                    installContext.CustomizeSelectedValue = customizeSelectedValue;
                else
                    installContext.CustomizeSelectedValue = installContext.CustPageOptionsItems[0];
            }
            else
            {
                if (page.DropDowns == null || page.DropDowns.Count <= 0)
                    return;
                installContext.CustPageDropDown = page.DropDowns[0];
                string localString = PathLib.GetLocalString(installContext.CustPageDropDown.Text);
                installContext.CustPageDropDown.Text = localString;
                installContext.CustDropDownsItems = page.DropDowns[0].Items;
                if (installContext.CustDropDownsItems == null || installContext.CustDropDownsItems.Count <= 0)
                    return;
                installContext.CustDropDownsItems.ForEach((Action<CustomizeItem>)(o => o.Text = PathLib.GetLocalString(o.Text)));
                if (customizeSelectedValue != null)
                    installContext.CustomizeSelectedValue = customizeSelectedValue;
                else
                    installContext.CustomizeSelectedValue = installContext.CustDropDownsItems[0];
            }
        }

        public string ParameterTip
        {
            get => this.parameterTip;
            set
            {
                this.parameterTip = value;
                this.OnPropertyChanged(nameof(ParameterTip));
            }
        }

        internal bool IsReservedManufacturerFail
        {
            get => this.isReservedManufacturerFail;
            set
            {
                this.isReservedManufacturerFail = value;
                this.OnPropertyChanged("ReservedManufacturerFailVisibility");
            }
        }

        internal bool IsReservedUserFail
        {
            get => this.isReservedUserFail;
            set
            {
                this.isReservedUserFail = value;
                this.OnPropertyChanged("ReservedUserFailVisibility");
            }
        }

        public bool IsReservedManufacturer
        {
            get => this.isReservedManufacturer;
            set
            {
                this.isReservedManufacturer = value;
                this.OnPropertyChanged(nameof(IsReservedManufacturer));
                this.OnPropertyChanged("TransformParameterVisibilty");
                this.OnPropertyChanged("OtherVisibilty");
                this.OnPropertyChanged("OtherTaskVisibilty");
            }
        }

        public bool IsReservedUser
        {
            get => this.isReservedUser;
            set
            {
                this.isReservedUser = value;
                this.OnPropertyChanged(nameof(IsReservedUser));
                this.OnPropertyChanged("MachineParameterVisibilty");
                this.OnPropertyChanged("OtherVisibilty");
                this.OnPropertyChanged("OtherTaskVisibilty");
            }
        }

        private void InitConfigList()
        {
            try
            {
                string str1 = Path.Combine(App.SetupPath, "Setup.config");
                IEnumerable<XElement> xelements = File.Exists(str1) ? XElement.Load(str1).Element((XName)"ConfigList").Elements((XName)"Config") : throw new Exception(string.Format("{0} 配置文件不存在！ ", (object)str1));
                InstallContext.Instance.ConfigList.Clear();
                foreach (XElement xelement in xelements)
                {
                    string str2 = xelement.Attribute((XName)"DisplayName").Value;
                    InstallContext.Instance.ConfigList.Add(new ConfigEntity()
                    {
                        DirectoryName = xelement.Attribute((XName)"DirectoryName").Value,
                        ConfigNameConfigBase = str2
                    });
                }
            }
            catch
            {
                throw new Exception("Setup.config 配置文件内容异常。 ");
            }
        }

        private void InitMachineList()
        {
            try
            {
                InstallContext instance = InstallContext.Instance;
                string str1 = Path.Combine(App.SetupPath, "Setup.config");
                if (!File.Exists(str1))
                    throw new Exception(string.Format("{0} 配置文件不存在！ ", (object)str1));
                instance.MachineList.Clear();
                XElement xelement1 = XElement.Load(str1).Element((XName)"MachineList");
                if (xelement1 != null)
                {
                    IEnumerable<XElement> source = xelement1.Elements((XName)"Machine");
                    if (source.Count<XElement>() > 0)
                    {
                        foreach (XElement xelement2 in source)
                        {
                            string str2 = xelement2.Attribute((XName)"DisplayName").Value;
                            string str3 = xelement2.Attribute((XName)"MachineModel").Value;
                            instance.MachineList.Add(new MachineEntity()
                            {
                                DirectoryName = xelement2.Attribute((XName)"DirectoryName").Value,
                                MachineNameConfigBase = str2,
                                MachineModel = str3
                            });
                        }
                    }
                }
                MachineEntity packUpMachine = this.GetPackUpMachine();
                if (packUpMachine != null)
                {
                    instance.MachineSelectedValue = packUpMachine;
                }
                else
                {
                    List<MachineEntity> machineList = instance.MachineList;
                    // ISSUE: explicit non-virtual call
                    if ((machineList != null ? ((machineList.Count) > 0 ? 1 : 0) : 0) == 0)
                        return;
                    instance.MachineSelectedValue = instance.MachineList[0];
                }
            }
            catch
            {
            }
        }

        private MachineEntity GetPackUpMachine()
        {
            MachineEntity packUpMachine = (MachineEntity)null;
            string packUpMachineModel = this.Config.PackUpMachineModel;
            if (!string.IsNullOrWhiteSpace(packUpMachineModel))
                packUpMachine = InstallContext.Instance.MachineList.Find((Predicate<MachineEntity>)(m => string.Compare(m.MachineModel, packUpMachineModel, true) == 0));
            return packUpMachine;
        }

        internal bool IsDirverFailed
        {
            get => this.isdirverfailed;
            set
            {
                this.isdirverfailed = value;
                this.OnPropertyChanged("DirverFailVisibilty");
            }
        }

        public string InstallPath
        {
            get => this.installpath;
            set
            {
                this.installpath = value;
                this.OnPropertyChanged(nameof(InstallPath));
            }
        }

        public bool IsCreateShotCut
        {
            get => this.isCreateShotCut;
            set
            {
                this.isCreateShotCut = true;
                this.OnPropertyChanged(nameof(IsCreateShotCut));
                this.OnPropertyChanged("ShortCutVisibilty");
                this.OnPropertyChanged("OtherTaskVisibilty");
            }
        }

        public bool IsNcCloudAdapterSetUp
        {
            get => this.isNcCloudAdapterSetUp;
            set
            {
                this.isNcCloudAdapterSetUp = value;
                this.OnPropertyChanged(nameof(IsNcCloudAdapterSetUp));
            }
        }

        public bool IsStartEnabled
        {
            get
            {
                if (Directory.Exists(Env.Instance.Config.ActiveConfigDir))
                {
                    string[] directories = Directory.GetDirectories(Env.Instance.Config.ActiveConfigDir);
                    string[] files = Directory.GetFiles(Env.Instance.Config.ActiveConfigDir);
                    if (directories.Length != 0 || files.Length != 0)
                        return true;
                }
                if (Directory.Exists(Env.Instance.Config.OEMConfigDir))
                {
                    string[] directories = Directory.GetDirectories(Env.Instance.Config.OEMConfigDir);
                    string[] files = Directory.GetFiles(Env.Instance.Config.OEMConfigDir);
                    if (directories.Length != 0 || files.Length != 0)
                        return true;
                }
                return false;
            }
        }

        public bool IsAutoRestart
        {
            get => this.isAutoRestart;
            set
            {
                this.isAutoRestart = value;
                this.OnPropertyChanged(nameof(IsAutoRestart));
                this.OnPropertyChanged("AutoRestartVisibilty");
                this.OnPropertyChanged("OtherVisibilty");
                this.OnPropertyChanged("OtherTaskVisibilty");
                this.OnPropertyChanged("IsAutoRestartVisibilty");
            }
        }

        public bool IsAutoRestartNoDesktop
        {
            get => this.isAutoRestartNoDesktop;
            set
            {
                this.isAutoRestartNoDesktop = value;
                this.OnPropertyChanged(nameof(IsAutoRestartNoDesktop));
                this.OnPropertyChanged("AutoRestartVisibilty");
                this.OnPropertyChanged("OtherVisibilty");
                this.OnPropertyChanged("OtherTaskVisibilty");
                this.OnPropertyChanged("IsAutoRestartNoDesktopVisibilty");
            }
        }

        public Visibility ShortCutVisibilty => !this.IsCreateShotCut ? Visibility.Collapsed : Visibility.Visible;

        public Visibility TransformParameterVisibilty => !this.IsReservedManufacturer ? Visibility.Collapsed : Visibility.Visible;

        public Visibility MachineParameterVisibilty => !this.IsReservedUser ? Visibility.Collapsed : Visibility.Visible;

        public Visibility AutoRestartVisibilty => !this.IsAutoRestart && !this.IsAutoRestartNoDesktop ? Visibility.Collapsed : Visibility.Visible;

        public Visibility CustConfigPageVisibilty => this.CustomizePageList == null || this.CustomizePageList.Count == 0 || this.CustomizePageShowAfterConfigPage || this.CustomizePageShowAfterMachinePage ? Visibility.Collapsed : Visibility.Visible;

        public Visibility CustConfigPageAfterConfigPageVisibilty => this.CustomizePageList == null || this.CustomizePageList.Count == 0 || !this.CustomizePageShowAfterConfigPage ? Visibility.Collapsed : Visibility.Visible;

        public Visibility CustConfigPageAfterMachinePageVisibilty => this.CustomizePageList == null || this.CustomizePageList.Count == 0 || !this.CustomizePageShowAfterMachinePage ? Visibility.Collapsed : Visibility.Visible;

        public bool IsAutoStart
        {
            get => this.isAutoStart;
            set
            {
                this.isAutoStart = value;
                this.OnPropertyChanged(nameof(IsAutoStart));
                if (!value)
                {
                    this.IsAutoRestartNoDesktop = false;
                    this.IsAutoRestart = false;
                }
                if (!value)
                    return;
                this.IsAutoRestart = true;
                this.IsAutoRestartNoDesktop = false;
            }
        }

        public Visibility OtherVisibilty => !this.IsAutoRestart && !this.IsReservedManufacturer && !this.IsAutoRestartNoDesktop ? Visibility.Collapsed : Visibility.Visible;

        public Visibility ReservedManufacturerFailVisibility => !this.isReservedManufacturerFail ? Visibility.Collapsed : Visibility.Visible;

        public Visibility ReservedUserFailVisibility => !this.IsReservedUserFail ? Visibility.Collapsed : Visibility.Visible;

        public Visibility DirverFailVisibilty => !this.isdirverfailed ? Visibility.Collapsed : Visibility.Visible;

        public string InstallDescription_NK300
        {
            get => this.installDescription_NK300;
            set
            {
                this.installDescription_NK300 = value;
                this.OnPropertyChanged(nameof(InstallDescription_NK300));
            }
        }

        public string InstallTitle_NK300
        {
            get => this.installTitle_NK300;
            set
            {
                this.installTitle_NK300 = value;
                this.OnPropertyChanged(nameof(InstallTitle_NK300));
            }
        }

        public string CurrentText
        {
            get => this.currentText;
            set
            {
                this.currentText = value;
                this.OnPropertyChanged(nameof(CurrentText));
            }
        }

        public string SubCurrentText
        {
            get => this.subcurrentText;
            set
            {
                this.subcurrentText = value;
                this.OnPropertyChanged(nameof(SubCurrentText));
            }
        }

        public int ProgressValue
        {
            get => this.progressValue;
            set
            {
                this.progressValue = value;
                this.OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public string OpenedProcessList
        {
            get => this.openedProcessList;
            set
            {
                this.openedProcessList = value;
                this.OnPropertyChanged(nameof(OpenedProcessList));
                this.OnPropertyChanged("ProcessListVisiblity");
            }
        }

        public Visibility ProcessListVisiblity
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.OpenedProcessList))
                    return Visibility.Collapsed;
                Wizard.Instance.CurrentPage.CanSelectNextPage = new bool?(false);
                return Visibility.Visible;
            }
        }

        public Visibility IsAutoRestartVisibilty => !this.IsAutoRestart ? Visibility.Collapsed : Visibility.Visible;

        public Visibility IsAutoRestartNoDesktopVisibilty => !this.IsAutoRestartNoDesktop ? Visibility.Collapsed : Visibility.Visible;

        public string InstallFailedMsg
        {
            get => this.installFailedMsg;
            set
            {
                this.installFailedMsg = value;
                this.OnPropertyChanged(nameof(InstallFailedMsg));
            }
        }

        public string ThirdPartyFailedMsg
        {
            get => this.thirdPartyFailedMsg;
            set
            {
                this.thirdPartyFailedMsg = value;
                this.OnPropertyChanged(nameof(ThirdPartyFailedMsg));
                this.OnPropertyChanged("IsThirdPartyFailedMsgVisibility");
            }
        }

        public Visibility IsThirdPartyFailedMsgVisibility => !string.IsNullOrWhiteSpace(this.ThirdPartyFailedMsg) ? Visibility.Visible : Visibility.Collapsed;

        public bool IsFirstRunStarupFailed { get; set; }

        internal void UpdateWriteEvent(object sender, LogEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Message))
                return;
            this.SubCurrentText = e.Message;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
