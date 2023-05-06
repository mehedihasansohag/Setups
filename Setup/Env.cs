// Decompiled with JetBrains decompiler
// Type: Setup.Env
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library;
using Setup.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Setup
{
    internal class Env
    {
        private static Env instance;
        internal const string PACKUP_CONFIG_NAME = "PackUp.cfg";
        internal const string PACKUP_TOOL_NAME = "PackUp.exe";
        internal const string PACKUP_TOOLS_DIR = "PackUp";
        internal const string PACKUP_CUSTPAGE_CONFIG_NAME = "CustomizePages.cfg";
        internal const string DRIVER_INI = "DriverPath.ini";
        internal const string NCSTUDIO = "NcStudio";
        internal const string WEIHONG = "Weihong";
        internal const string FLAG_FILE_NAME = "Install.flag";
        internal const string CONFIG_CFG = "Config.cfg";
        internal const string FIRSTRUN = "firstrun.exe";
        internal const string ORIGINAL_PATH = "OriginalPath";
        internal const string ORIGINAL_ACTIVE_CONFIG_PATH = "OriginalActiveConfigPath";
        internal const string ORIGINAL_OEM_CONFIG_PATH = "OriginalOEMConfigPath";
        internal const string FILENAME_SETUP_CONFIG = "Setup.config";

        internal static Env Instance
        {
            get
            {
                if (Env.instance == null)
                    Env.instance = new Env();
                return Env.instance;
            }
        }

        internal string FirstRunDir { get; private set; } = string.Empty;

        internal string TransParamFlagPath { get; private set; } = string.Empty;

        internal string TransParamFlagDir { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Weihong");

        internal string StartMenuNcStudioDir { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "NcStudio");

        internal string StartMenuDir { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Weihong");

        internal static LanguageInfo LanguageInfo { get; set; }

        internal ConfigInfo Config { get; set; }

        internal bool Init()
        {
            Packup.Library.StringParser.RegisterStringTagProvider("PN", (Func<string>)(() => this.Config.AppName));
            ResourceService.ResourceName = "Setup.Properties.Resources";
            this.InitConfig();
            return true;
        }

        private void InitConfig()
        {
            LogWriter.Write(Resources.Msg_ReadConfig);
            string str = Path.Combine(App.SetupPath, "Setup.config");
            if (!File.Exists(str))
                throw new Exception(string.Format(Resources.Err_MemoryNotEnoughOrPackageDamaged, (object)Directory.GetDirectoryRoot(str).Substring(0, 1)));
            using (FileStream fileStream = File.OpenRead(str))
            {
                if (fileStream.Length == 0L)
                    throw new Exception(string.Format(Resources.Err_MemoryNotEnoughOrPackageDamaged, (object)Directory.GetDirectoryRoot(str).Substring(0, 1)));
                this.SetConfigFromXml((Stream)fileStream);
            }
            using (XmlReader reader = XmlReader.Create(str))
            {
                CustPageHelper.GetCustPageList(reader, "Configuration");
                this.Config.CustomizePages = CustPageHelper.CustomPagesInfo;
            }
        }

        internal void SetConfigFromXml(Stream stream)
        {
            try
            {
                InstallContext instance = InstallContext.Instance;
                XElement xelement1 = XElement.Load(stream);
                if (this.Config == null)
                    this.Config = new ConfigInfo();
                this.Config.Name = xelement1.Element((XName)"Name").Value;
                this.Config.Version = xelement1.Element((XName)"Version").Value;
                this.Config.VersionLevel = xelement1.Element((XName)"VersionLevel").Value;
                this.Config.Path = xelement1.Element((XName)"Path").Value;
                this.Config.ExeName = xelement1.Element((XName)"ExeName").Value;
                this.Config.AppName = Path.GetFileNameWithoutExtension(this.Config.ExeName);
                this.Config.Vendor = xelement1.Element((XName)"Vendor")?.Value;
                this.Config.VendorCryptograph = xelement1.Element((XName)"VendorCryptograph")?.Value;
                this.Config.AppConfig = xelement1.Element((XName)"AppConfig").Value;
                this.Config.IsShowLangDialog = bool.Parse(xelement1.Element((XName)"IsShowLangDialog").Value);
                this.Config.DefaultLanguage = xelement1.Element((XName)"DefaultLanguage").Value;
                this.Config.IsStartup = bool.Parse(xelement1.Element((XName)"IsStartup").Value);
                this.Config.LangPath = xelement1.Element((XName)"LangPath") == null ? string.Empty : xelement1.Element((XName)"LangPath").Value;
                this.Config.BeforeSetup = xelement1.Element((XName)"BeforeSetup").Value;
                this.Config.AfterSetup = xelement1.Element((XName)"AfterSetup").Value;
                this.Config.BeforePackup = xelement1.Element((XName)"BeforePackup")?.Value;
                this.Config.AfterPackup = xelement1.Element((XName)"AfterPackup")?.Value;
                this.Config.UserParameterTipXElement = xelement1.Element((XName)"UserParameterTip");
                this.Config.ManufacturerParameterTipXElement = xelement1.Element((XName)"ManufacturerParameterTip");
                this.Config.BackgroundProcesses = ((IEnumerable<string>)xelement1.Element((XName)"BackgroundProcesses").Value.Split(';')).ToList<string>();
                this.Config.IsReservedManufacturer = bool.Parse(xelement1.Element((XName)"IsReservedManufacturer").Value);
                this.Config.IsReservedUser = bool.Parse(xelement1.Element((XName)"IsReservedUser").Value);
                this.Config.PackUpMachineModel = xelement1.Element((XName)"PackUpMachineModel")?.Value;
                this.Config.IsCreateShortcut = bool.Parse(xelement1.Element((XName)"IsCreateShortcut").Value);
                this.Config.IsInstallNcCloud = bool.Parse(xelement1.Element((XName)"IsInstallNcCloud").Value);
                this.Config.IsInstallSunloginClient = bool.Parse(xelement1.Element((XName)"IsInstallSunloginClient").Value);
                this.Config.PackUpIconCryptograph = xelement1.Element((XName)"PackUpIcon")?.Value;
                this.Config.SetupIconCryptograph = xelement1.Element((XName)"SetupIcon")?.Value;
                this.Config.SetupBackgroundImageCryptograph = xelement1.Element((XName)"SetupBackgroundImage")?.Value;
                this.Config.SetupHeaderImageCryptograph = xelement1.Element((XName)"SetupHeaderImage")?.Value;
                this.Config.CRC32 = xelement1.Element((XName)"CRC32")?.Value;
                this.Config.xmlCustPages = xelement1.Element((XName)"CustomizePages");
                this.FirstRunDir = Directory.GetParent(this.Config.Path).FullName;
                XElement xelement2 = xelement1.Element((XName)"ToolVersion");
                if (xelement2 != null)
                    this.Config.ToolVersion = xelement2.Value;
                XElement xelement3 = xelement1.Element((XName)"IsCheckMachine");
                if (xelement3 != null)
                    this.Config.IsCheckMachine = bool.Parse(xelement3.Value);
                this.Config.AutoStartType = (AutoStartType)Enum.Parse(typeof(AutoStartType), xelement1.Element((XName)"AutoStartType").Value);
                bool result;
                if (bool.TryParse(xelement1.Element((XName)"IsCustomPackUp").Value, out result))
                    this.Config.IsCustomPackUp = result;
                string str1 = xelement1.Element((XName)"InstallType").Value;
                this.Config.InstallType = str1 == null ? InstallType.PC : (InstallType)Enum.Parse(typeof(InstallType), str1);
                if (!string.IsNullOrWhiteSpace(this.Config.Vendor))
                {
                    this.StartMenuDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), this.Config.Vendor);
                    this.TransParamFlagDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), this.Config.Vendor);
                }
                this.TransParamFlagPath = Path.Combine(this.TransParamFlagDir, "Install.flag");
                this.Config.xmlWizardPageOrder = xelement1.Element((XName)"WizardPageOrder");
                IEnumerable<XElement> xelements = this.Config.xmlWizardPageOrder?.Elements((XName)"WizardPage");
                if (xelements != null)
                {
                    foreach (XElement xelement4 in xelements)
                    {
                        string str2 = xelement4.Attribute((XName)"Name").Value;
                        string str3 = xelement4.Attribute((XName)"After")?.Value;
                        string str4 = xelement4.Attribute((XName)"Before")?.Value;
                        this.Config.WizardPageOrderList.Add(new CustWizardPage()
                        {
                            Name = str2,
                            After = str3,
                            Before = str4
                        });
                    }
                }
                foreach (XElement element in xelement1.Element((XName)"LanguageList").Elements((XName)"Language"))
                {
                    string str5 = element.Attribute((XName)"Name").Value;
                    string str6 = element.Attribute((XName)"Culture").Value;
                    string str7 = element.Attribute((XName)"NativeName").Value;
                    this.Config.LanguageList.Add(new LanguageInfo()
                    {
                        Name = str5,
                        Culture = str6,
                        NativeName = str7
                    });
                }
                this.Config.ActiveConfigDir = xelement1.Element((XName)"ActiveConfigDir").Value;
                this.Config.ActiveConfigPath = !Path.IsPathRooted(this.Config.ActiveConfigDir) ? Path.GetFullPath(Path.Combine(this.Config.Path, "Bin", this.Config.ActiveConfigDir)) : Path.GetFullPath(this.Config.ActiveConfigDir);
                this.Config.OEMConfigDir = xelement1.Element((XName)"OEMConfigDir").Value;
                this.Config.OEMConfigPath = !Path.IsPathRooted(this.Config.OEMConfigDir) ? Path.GetFullPath(Path.Combine(this.Config.Path, "Bin", this.Config.OEMConfigDir)) : Path.GetFullPath(this.Config.OEMConfigDir);
                this.Config.DefaultConfigDir = xelement1.Element((XName)"DefaultConfigDir").Value;
                this.Config.xmlOriginalSourcePaths = xelement1.Element((XName)"OriginalSourcePaths");
                if (this.Config.xmlOriginalSourcePaths == null)
                    return;
                Log.Write("解析原安装路径信息");
                XElement originalSourcePaths = this.Config.xmlOriginalSourcePaths;
                List<string> stringList = new List<string>();
                XName name = (XName)"OriginalSourcePath";
                foreach (XElement descendant in originalSourcePaths.Descendants(name))
                    stringList.Add(descendant.Value);
                this.ParserOriginalBootstrapper(stringList.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("Setup.config 配置文件内容异常。" + ex.Message);
            }
        }

        internal void ParserOriginalBootstrapper(string[] sourcePaths)
        {
            if (Directory.Exists(this.Config.ActiveConfigPath) || Directory.Exists(this.Config.OEMConfigPath))
            {
                Log.Write("软件 ActiveConfig 或 OEMConfig 目录已存在，不解析原安装目录信息");
                Log.Write("ActiveConfig 路径：" + this.Config.ActiveConfigPath);
                Log.Write("OEMConfig 路径：" + this.Config.OEMConfigPath);
                this.Config.OriginalActiveConfigPath = string.Empty;
            }
            else
            {
                if (sourcePaths != null && sourcePaths.Length != 0)
                {
                    for (int index = sourcePaths.Length - 1; index >= 0; --index)
                    {
                        string sourcePath = sourcePaths[index];
                        if (!string.IsNullOrEmpty(sourcePath) && sourcePath != this.Config.Path && Directory.Exists(sourcePath))
                        {
                            this.Config.OriginalSourcePath = sourcePath;
                            break;
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(this.Config.OriginalSourcePath))
                    return;
                Log.Write("原安装路径：" + this.Config.OriginalSourcePath);
                string str = Path.Combine(this.Config.OriginalSourcePath, "Bin", "Bootstrapper.cfg");
                if (!File.Exists(str))
                    return;
                XElement xelement = XElement.Load(str);
                this.Config.OriginalDefaultConfigDir = xelement.Element((XName)"DefaultConfigDir")?.Value;
                this.Config.OriginalOEMConfigDir = xelement.Element((XName)"OEMConfigDir")?.Value;
                this.Config.OriginalActiveConfigDir = xelement.Element((XName)"ActiveConfigDir")?.Value;
                this.Config.OriginalActiveConfigPath = !Path.IsPathRooted(this.Config.OriginalActiveConfigDir) ? Path.GetFullPath(Path.Combine(this.Config.OriginalSourcePath, "Bin", this.Config.OriginalActiveConfigDir)) : Path.GetFullPath(this.Config.OriginalActiveConfigDir);
                this.Config.OriginalOEMConfigPath = !Path.IsPathRooted(this.Config.OriginalOEMConfigDir) ? Path.GetFullPath(Path.Combine(this.Config.OriginalSourcePath, "Bin", this.Config.OriginalOEMConfigDir)) : Path.GetFullPath(this.Config.OriginalOEMConfigDir);
                this.Config.OriginalAssetsPath = Path.Combine(new DirectoryInfo(this.Config.OriginalOEMConfigPath).Parent.FullName, "Assets");
                this.GetOriginalAppInfoFromXml();
                Log.Write("原ActiveConfig路径：" + this.Config.OriginalActiveConfigPath);
                Log.Write("原OEMConfig路径：" + this.Config.OriginalOEMConfigPath);
                Log.Write("原Assets路径：" + this.Config.OriginalAssetsPath);
                if (this.Config.dicBatFileParam == null)
                    this.Config.dicBatFileParam = new Dictionary<string, string>();
                if (this.Config.dicBatFileParam.ContainsKey("OriginalPath"))
                    this.Config.dicBatFileParam.Remove("OriginalPath");
                this.Config.dicBatFileParam.Add("OriginalPath", this.Config.OriginalSourcePath);
                if (this.Config.dicBatFileParam.ContainsKey("OriginalActiveConfigPath"))
                    this.Config.dicBatFileParam.Remove("OriginalActiveConfigPath");
                this.Config.dicBatFileParam.Add("OriginalActiveConfigPath", this.Config.OriginalActiveConfigPath);
                if (this.Config.dicBatFileParam.ContainsKey("OriginalOEMConfigPath"))
                    this.Config.dicBatFileParam.Remove("OriginalOEMConfigPath");
                this.Config.dicBatFileParam.Add("OriginalOEMConfigPath", this.Config.OriginalOEMConfigPath);
            }
        }

        internal void GetOriginalAppInfoFromXml()
        {
            try
            {
                foreach (string file in Directory.GetFiles(Path.Combine(this.Config.OriginalSourcePath, "bin"), "App.cfg", SearchOption.AllDirectories))
                    this.Config.OriginalAppName = XElement.Load(file).Element((XName)"Name").Value;
            }
            catch (Exception ex)
            {
                throw new Exception("Setup.config 配置文件内容异常。" + ex.Message);
            }
        }

        internal bool CompleteUpdateConfigFile()
        {
            bool flag = true;
            try
            {
                new XmlWriterSettings().Indent = true;
                LogWriter.Write("更新NcStudio语言");
                if (!string.IsNullOrWhiteSpace(this.Config.LangPath))
                {
                    XElement xelement = new XElement((XName)"Lang", (object)this.Config.SelectedLanguage);
                    string fullPath = Path.GetFullPath(Path.Combine(this.Config.ActiveConfigPath, this.Config.LangPath));
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        xelement.Save((Stream)memoryStream);
                        AtomicFileService.AtomicWriteEx(fullPath, (Stream)memoryStream);
                    }
                }
                LogWriter.Write("更新PackUp工具配置信息");
                XElement xelement1 = new XElement((XName)"Configuration");
                xelement1.Add((object)new XElement((XName)"Version", (object)this.Config.ToolVersion));
                xelement1.Add((object)new XElement((XName)"SourcePath", (object)this.Config.Path));
                xelement1.Add((object)new XElement((XName)"AppConfig", (object)this.Config.AppConfig));
                xelement1.Add((object)new XElement((XName)"BackBagsPath", (object)Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
                xelement1.Add((object)new XElement((XName)"IsCustomPackUp", (object)"true"));
                xelement1.Add((object)new XElement((XName)"InstallType", (object)this.Config.InstallType.ToString()));
                xelement1.Add((object)new XElement((XName)"DefaultLanguage", (object)this.Config.DefaultLanguage));
                xelement1.Add((object)new XElement((XName)"AutoStartType", (object)this.Config.AutoStartType.ToString()));
                xelement1.Add((object)new XElement((XName)"IsShowLangDialog", (object)this.Config.IsShowLangDialog.ToString()));
                xelement1.Add((object)new XElement((XName)"IsReservedUser", (object)this.Config.IsReservedUser.ToString()));
                xelement1.Add((object)new XElement((XName)"IsReservedManufacturer", (object)this.Config.IsReservedManufacturer.ToString()));
                xelement1.Add((object)new XElement((XName)"IsInstallNcCloud", (object)this.Config.IsInstallNcCloud.ToString()));
                xelement1.Add((object)new XElement((XName)"IsInstallSunloginClient", (object)this.Config.IsInstallSunloginClient.ToString()));
                xelement1.Add((object)new XElement((XName)"IsCheckMachine", (object)this.Config.IsCheckMachine.ToString()));
                xelement1.Add((object)new XElement((XName)"IsStartup", (object)this.Config.IsStartup.ToString()));
                xelement1.Add((object)new XElement((XName)"BeforeSetup", (object)this.Config.BeforeSetup));
                xelement1.Add((object)new XElement((XName)"AfterSetup", (object)this.Config.AfterSetup));
                xelement1.Add((object)new XElement((XName)"BeforePackup", (object)this.Config.BeforePackup));
                xelement1.Add((object)new XElement((XName)"AfterPackup", (object)this.Config.AfterPackup));
                if (this.Config.xmlWizardPageOrder != null)
                    xelement1.Add((object)this.Config.xmlWizardPageOrder);
                if (this.Config.xmlOriginalSourcePaths != null)
                    xelement1.Add((object)this.Config.xmlOriginalSourcePaths);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string backgroundProcess in this.Config.BackgroundProcesses)
                {
                    if (!string.IsNullOrWhiteSpace(backgroundProcess))
                        stringBuilder.Append(backgroundProcess + ";");
                }
                xelement1.Add((object)new XElement((XName)"BackgroundProcesses", (object)stringBuilder.ToString()));
                if (!string.IsNullOrWhiteSpace(this.Config.VendorCryptograph))
                    xelement1.Add((object)new XElement((XName)"Vendor", (object)this.Config.VendorCryptograph));
                if (!string.IsNullOrWhiteSpace(this.Config.PackUpIconCryptograph))
                    xelement1.Add((object)new XElement((XName)"PackUpIcon", (object)this.Config.PackUpIconCryptograph));
                if (!string.IsNullOrWhiteSpace(this.Config.SetupIconCryptograph))
                    xelement1.Add((object)new XElement((XName)"SetupIcon", (object)this.Config.SetupIconCryptograph));
                if (!string.IsNullOrWhiteSpace(this.Config.SetupBackgroundImageCryptograph))
                    xelement1.Add((object)new XElement((XName)"SetupBackgroundImage", (object)this.Config.SetupBackgroundImageCryptograph));
                if (!string.IsNullOrWhiteSpace(this.Config.SetupHeaderImageCryptograph))
                    xelement1.Add((object)new XElement((XName)"SetupHeaderImage", (object)this.Config.SetupHeaderImageCryptograph));
                string path = Path.Combine(App.SetupPath, "Setup.config");
                if (File.Exists(path))
                {
                    using (FileStream fileStream = File.OpenRead(path))
                    {
                        XElement xelement2 = XElement.Load((Stream)fileStream);
                        IEnumerable<XElement> xelements1 = xelement2.Element((XName)"ManufacturerParameterTip")?.Elements();
                        if (xelements1 != null)
                        {
                            XElement content1 = new XElement((XName)"ManufacturerParameterTip");
                            foreach (XElement content2 in xelements1)
                                content1.Add((object)content2);
                            xelement1.Add((object)content1);
                        }
                        IEnumerable<XElement> xelements2 = xelement2.Element((XName)"UserParameterTip")?.Elements();
                        if (xelements2 != null)
                        {
                            XElement content3 = new XElement((XName)"UserParameterTip");
                            foreach (XElement content4 in xelements2)
                                content3.Add((object)content4);
                            xelement1.Add((object)content3);
                        }
                    }
                }
                string[] files = Directory.GetFiles(this.Config.Path, "PackUp.exe", SearchOption.AllDirectories);
                string empty = string.Empty;
                if (files != null && files.Length != 0)
                {
                    empty = Directory.GetParent(files[0]).ToString();
                    string targetPath = Path.Combine(empty, "PackUp", "PackUp.cfg");
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        xelement1.Save((Stream)memoryStream);
                        AtomicFileService.AtomicWriteEx(targetPath, (Stream)memoryStream);
                    }
                }
                string str = Path.Combine(empty, "PackUp", "CustomizePages.cfg");
                if (!File.Exists(str))
                {
                    if (this.Config.xmlCustPages != null)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            XElement xelement3 = new XElement((XName)"Configuration");
                            xelement3.Add((object)this.Config.xmlCustPages);
                            xelement3.Save((Stream)memoryStream);
                            AtomicFileService.AtomicWriteEx(str, (Stream)memoryStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.UpdatePrompt("PackUp.cfg" + Resources.Err_CreateFileFailed + ex.Message);
                flag = false;
            }
            return flag;
        }

        internal void SetLocal(string lang)
        {
            try
            {
                CultureInfo cultureInfo = new CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Resources.Culture = cultureInfo;
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }
            catch
            {
            }
        }
    }
}
