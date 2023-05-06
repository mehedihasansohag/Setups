// Decompiled with JetBrains decompiler
// Type: Setup.ConfigInfo
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library.Entity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace Setup
{
    internal class ConfigInfo
    {
        private string selectedLanguage = string.Empty;
        internal Dictionary<string, string> dicBatFileParam = new Dictionary<string, string>();

        public ConfigInfo() => this.TransformDatas = new List<object>();

        internal string Vendor { get; set; } = string.Empty;

        internal string VendorCryptograph { get; set; } = string.Empty;

        internal string Name { get; set; }

        internal string ToolVersion { get; set; } = string.Empty;

        internal string Version { get; set; }

        internal string VersionLevel { get; set; }

        internal string LocalMachineModel { get; set; } = string.Empty;

        [Obsolete]
        internal string TargetMachineModel { get; set; } = string.Empty;

        internal string Path { get; set; }

        internal string CurrentWorkPath { get; set; }

        internal InstallType InstallType { get; set; }

        internal bool IsAutoDirver { get; set; } = true;

        internal List<object> TransformDatas { get; set; }

        internal string AppName { get; set; } = "NcStudio";

        internal string OriginalAppName { get; set; }

        internal string ExeName { get; set; } = "NcStudio.exe";

        internal bool IsCustomPackUp { get; set; }

        internal string AppConfig { get; set; }

        internal string LangPath { get; set; } = string.Empty;

        internal bool IsShowLangDialog { get; set; } = true;

        internal string DefaultLanguage { get; set; } = string.Empty;

        internal bool IsStartup { get; set; }

        internal string OriginalActiveConfigDir { get; set; } = string.Empty;

        internal string OriginalOEMConfigDir { get; set; } = string.Empty;

        internal string OriginalDefaultConfigDir { get; set; } = string.Empty;

        internal string PackUpIconCryptograph { get; set; }

        internal string SetupIconCryptograph { get; set; }

        internal string SetupBackgroundImageCryptograph { get; set; }

        internal string SetupHeaderImageCryptograph { get; set; }

        internal string SelectedLanguage
        {
            get => this.selectedLanguage;
            set
            {
                this.selectedLanguage = value;
                LogWriter.Write("安装语言为 " + value);
            }
        }

        internal XElement xmlOriginalSourcePaths { get; set; }

        internal string OriginalSourcePath { get; set; } = string.Empty;

        internal string BeforeSetup { get; set; } = string.Empty;

        internal string BeforePackup { get; set; } = string.Empty;

        internal bool IsReservedManufacturer { get; set; } = true;

        internal bool IsReservedUser { get; set; } = true;

        internal bool IsCreateShortcut { get; set; } = true;

        internal bool IsInstallNcCloud { get; set; } = true;

        internal bool IsInstallSunloginClient { get; set; } = true;

        internal bool IsCheckMachine { get; set; }

        internal string AfterSetup { get; set; } = string.Empty;

        internal string AfterPackup { get; set; } = string.Empty;

        internal string ActiveConfigDir { get; set; } = string.Empty;

        internal string OEMConfigDir { get; set; } = string.Empty;

        internal string DefaultConfigDir { get; set; } = string.Empty;

        internal string ActiveConfigPath { get; set; } = string.Empty;

        internal string OEMConfigPath { get; set; } = string.Empty;

        internal string OriginalActiveConfigPath { get; set; } = string.Empty;

        internal string OriginalOEMConfigPath { get; set; } = string.Empty;

        internal string OriginalAssetsPath { get; set; } = string.Empty;

        internal AutoStartType AutoStartType { get; set; }

        internal List<LanguageInfo> LanguageList { get; set; } = new List<LanguageInfo>();

        internal List<CustWizardPage> WizardPageOrderList { get; set; } = new List<CustWizardPage>();

        internal List<string> BackgroundProcesses { get; set; } = new List<string>();

        internal XElement xmlCustPages { get; set; }

        internal XElement xmlWizardPageOrder { get; set; }

        internal CustomizePagesInfo CustomizePages { get; set; } = new CustomizePagesInfo();

        public string CRC32 { get; set; } = "";

        public string PackUpMachineModel { get; internal set; }

        internal XElement UserParameterTipXElement { private get; set; }

        public string UserParameterTip
        {
            get
            {
                string userParameterTip = this.UserParameterTipXElement?.Element((XName)Thread.CurrentThread.CurrentCulture.Name)?.Value;
                if (string.IsNullOrEmpty(userParameterTip))
                    userParameterTip = this.UserParameterTipXElement?.Element((XName)"en")?.Value;
                return userParameterTip;
            }
        }

        internal XElement ManufacturerParameterTipXElement { private get; set; }

        public string ManufacturerParameterTip
        {
            get
            {
                string manufacturerParameterTip = this.ManufacturerParameterTipXElement?.Element((XName)Thread.CurrentThread.CurrentCulture.Name)?.Value;
                if (string.IsNullOrEmpty(manufacturerParameterTip))
                    manufacturerParameterTip = this.ManufacturerParameterTipXElement?.Element((XName)"en")?.Value;
                return manufacturerParameterTip;
            }
        }
    }
}
