// Decompiled with JetBrains decompiler
// Type: Setup.Installer
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using IWshRuntimeLibrary;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using Packup.Library;
using Setup.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml.Linq;

namespace Setup
{
    public class Installer
    {
        private static Installer instance;
        internal const string SHELL_BAT_FILE = "Shell.bat";
        internal const string SETTING_BAT_FILE = "Setting.bat";
        private Mutex firstrunMutex;
        private Mutex ncStudioMutex;
        public static CallSite<Func<CallSite, object, IWshShortcut>> callSite;
        internal const int INSTALLER_CHECK_ENVIRONMENT = 5;
        internal const int INSTALLER_PREPARE_INSTALL = 10;
        internal const int INSTALLER_INSTALLING = 15;
        internal const int INSTALLER_TRANSFORMDATA = 85;
        internal const int INSTALLER_INSTALL_DRIVER = 90;
        internal const int INSTALLER_FINISH_OTHERTASK = 95;
        internal const int INSTALLER_FINISHED = 99;

        internal static Installer Instance
        {
            get
            {
                if (Installer.instance == null)
                    Installer.instance = new Installer();
                return Installer.instance;
            }
        }

        public bool Init()
        {
            LogWriter.NotifyProgressChanged += new LogWriter.WriteEventHandler(InstallContext.Instance.UpdateWriteEvent);
            return true;
        }

        internal bool IsBeginInstall { get; set; }

        internal bool IsBeginRestore { get; set; }

        private int GetCopyFilePercent(double copyPercent)
        {
            try
            {
                double num = 70.0;
                return 15 + int.Parse(Math.Round(copyPercent * num, 0).ToString());
            }
            catch
            {
                return InstallContext.Instance.ProgressValue;
            }
        }

        private ConfigInfo Config => Env.Instance.Config;

        internal MachineCheckResultEnum MachineCheckResult { get; set; }

        internal bool IsFinished { get; set; }

        internal bool IsRestored { get; set; }

        internal bool CanCloseWindow { get; set; } = true;

        internal bool IsDriverSetup { get; set; }

        internal bool IsSucceed { get; set; }

        internal bool IsCheckEnvironment { get; set; }

        internal bool IsFinishedBackUpFile { get; set; }

        internal bool hasHistoryVersion { get; set; }

        internal string ErrorMessage { get; set; }

        internal string RestoreErrorMessage { get; set; }

        internal Dictionary<string, string> dirBackUpMapping { get; set; } = new Dictionary<string, string>();

        internal void Start(RunWorkerCompletedEventHandler completed)
        {
            App.installWorker.ProgressChanged += (ProgressChangedEventHandler)((o, e) =>
           {
               InstallContext.Instance.CurrentText = e.UserState.ToString();
               if (InstallContext.Instance.ProgressValue >= e.ProgressPercentage)
                   return;
               InstallContext.Instance.ProgressValue = e.ProgressPercentage;
           });
            App.installWorker.DoWork += new DoWorkEventHandler(this.DoInstall);
            App.installWorker.RunWorkerCompleted += completed;
            App.installWorker.RunWorkerAsync();
        }

        internal void DoInstall(object obj, DoWorkEventArgs e)
        {
            try
            {
                this.IsBeginInstall = true;
                LogWriter.UpdatePrompt(Resources.Msg_StartSetup);
                App.installWorker.ReportProgress(5, (object)Resources.Msg_CheckEnvironmentProc);
                this.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
               {
                   if (this.CheckExeProcess())
                       throw new Exception(Resources.Msg_ProcessExist);
               }));
                bool createdNew1;
                this.ncStudioMutex = new Mutex(true, "SingletonPhoenixAppMutex", out createdNew1);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                this.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
               {
                   this.CloseBackgroundProcesses();
                   this.CloseBackgroundProcesses();
               }));
                bool createdNew2;
                this.firstrunMutex = new Mutex(true, "phoenixMutexFirstRun.exe", out createdNew2);
                this.CheckInstallEnvironment();
                stopwatch.Stop();
                LogWriter.Write(string.Format("检查安装环境: {0}", (object)stopwatch.Elapsed.TotalMilliseconds));
                this.IsCheckEnvironment = true;
                App.installWorker.ReportProgress(10, (object)Resources.Msg_PrepareInstall);
                string zipPath = Path.Combine(App.SetupPath, "Release.zip");
                if (!File.Exists(zipPath))
                    App.WaitExtract.WaitOne();
                this.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
               {
                   App.installWorker.ReportProgress(15, (object)Resources.Msg_InstallingProc);
                   this.RenameOriginalPathToCurrentPath();
                   this.RunBeforeSetup(zipPath);
                   this.ExtractZip(zipPath);
               }));
                this.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
               {
                   App.installWorker.ReportProgress(85, (object)Resources.Msg_TransformData);
                   LogWriter.UpdatePrompt(Resources.Msg_ManufacturerParameterProc);
                   this.TransParam();
               }));
                this.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
               {
                   App.installWorker.ReportProgress(90, (object)Resources.Msg_InstallingProc);
                   this.InstallDirver();
                   this.IsDriverSetup = true;
               }));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.DisableSleep));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.PromptAdmin));
                this.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() => App.installWorker.ReportProgress(95, (object)Resources.Msg_FinishOtherTask)));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.InstallSunloginClient));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.InstallFirstRun));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.CreateShortCut));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.ClearStartMachineStart));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.ClearStartMachineStartNoDesktop));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.CreateAutoRestart));
                this.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() => App.installWorker.ReportProgress(99, (object)Resources.Msg_FinishOtherTask)));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.RunAfterSetup));
                this.RunInstallCheckWorkerCancellationPending(new Installer.RunInstallCheckWorkStatusDelegate(this.FirstRunAutoStart));
                this.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() => this.IsSucceed = true));
            }
            finally
            {
                this.RunInstallCheckWorkerCancellationPending((Installer.RunInstallCheckWorkStatusDelegate)(() =>
               {
                   this.IsFinished = true;
                   if (!this.IsSucceed)
                       return;
                   this.RunAfterInstallSuccess();
                   Env.Instance.CompleteUpdateConfigFile();
                   this.DeleteBackupConfigFileByPath(new string[3]
            {
            this.Config.ActiveConfigPath,
            this.Config.OEMConfigPath,
            Path.Combine(new DirectoryInfo(this.Config.OEMConfigPath).Parent.FullName, "Assets")
                 }, false);
               }));
                if (this.ncStudioMutex != null)
                {
                    this.ncStudioMutex.Dispose();
                    this.ncStudioMutex = (Mutex)null;
                }
            }
        }

        internal void InitBatFile(
          Dictionary<string, string> fileParams,
          List<string> batFileList,
          string workPath)
        {
            string targetPath1 = Path.Combine(workPath, "Shell.bat");
            string targetPath2 = Path.Combine(workPath, "Setting.bat");
            Packup.Library.Log.Write("InitBatFile start ... ");
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.Append("@echo off" + Environment.NewLine);
            stringBuilder1.Append("call Setting.bat" + Environment.NewLine);
            if (batFileList != null)
            {
                foreach (string batFile in batFileList)
                {
                    stringBuilder1.Append("call " + batFile + Environment.NewLine);
                    Packup.Library.Log.Write("InitBatFile file ... " + batFile);
                }
            }
            using (MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(stringBuilder1.ToString())))
                AtomicFileService.AtomicWriteEx(targetPath1, (Stream)memoryStream);
            StringBuilder stringBuilder2 = new StringBuilder();
            stringBuilder2.Append("@echo off" + Environment.NewLine);
            if (fileParams != null)
            {
                foreach (KeyValuePair<string, string> fileParam in fileParams)
                {
                    stringBuilder2.Append("set " + fileParam.Key + "=" + fileParam.Value + Environment.NewLine);
                    Packup.Library.Log.Write("InitBatFile param ... " + fileParam.Key + "=" + fileParam.Value);
                }
            }
            using (MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(stringBuilder2.ToString())))
                AtomicFileService.AtomicWriteEx(targetPath2, (Stream)memoryStream);
        }

        private void DeleteBatFile(string workPath)
        {
            string path1 = Path.Combine(workPath, "Shell.bat");
            string path2 = Path.Combine(workPath, "Setting.bat");
            Packup.Library.Log.Write("DeleteBatFile start ... ");
            if (File.Exists(path1))
                File.Delete(path1);
            if (!File.Exists(path2))
                return;
            File.Delete(path2);
        }

        internal void RunInstallCheckWorkerCancellationPending(
          Installer.RunInstallCheckWorkStatusDelegate installMethodDelegate)
        {
            if (App.installWorker.CancellationPending)
                return;
            installMethodDelegate();
        }

        internal void RunAfterInstallSuccess()
        {
            string str = Path.Combine(Directory.GetParent(this.Config.Path).FullName, "~Temp");
            if (!Directory.Exists(str) || !Directory.Exists(this.Config.Path))
                return;
            this.CleanTempDir(str);
            PathLib.CreateDirectory(str);
            ShellFileOperation.Move(str, this.Config.Path);
        }

        internal void RestoreFile(RunWorkerCompletedEventHandler completed)
        {
            App.restoreWorker.ProgressChanged += (ProgressChangedEventHandler)((o, e) =>
           {
               InstallContext.Instance.CurrentText = e.UserState.ToString();
               if (e.ProgressPercentage < 0)
                   return;
               InstallContext.Instance.ProgressValue = e.ProgressPercentage;
           });
            App.restoreWorker.DoWork += (DoWorkEventHandler)((o, e) =>
           {
               try
               {
                   this.IsBeginRestore = true;
                   LogWriter.UpdatePrompt(Resources.Msg_RestoreFile);
                   App.restoreWorker.ReportProgress(0, (object)Resources.Msg_RestoreCheckDriverStatus);
                   if (!App.restoreWorker.CancellationPending)
                   {
                       LogWriter.Write("检查还原环境...");
                       App.restoreWorker.ReportProgress(95, (object)Resources.Msg_CheckRestoreEnvironment);
                       if (!this.CheckRestoreEnvironment())
                           return;
                       LogWriter.Write("删除后台进程...");
                       App.restoreWorker.ReportProgress(90, (object)Resources.Msg_CloseBackgroundProcesses);
                       this.CloseBackgroundProcesses();
                       this.CloseBackgroundProcesses();
                       LogWriter.Write("检查驱动状态开始...");
                       App.restoreWorker.ReportProgress(85, (object)Resources.Msg_CheckDirverProcessStatus);
                       this.CheckDirverProcessStatus();
                   }
                   if (!App.restoreWorker.CancellationPending)
                   {
                       App.restoreWorker.ReportProgress(85, (object)Resources.Msg_RestoreSetupFile);
                       LogWriter.Write("还原安装文件开始...");
                       this.RestoreSetupFile();
                   }
                   if (!App.restoreWorker.CancellationPending && !this.IsSucceed)
                   {
                       App.restoreWorker.ReportProgress(10, (object)Resources.Msg_RestoreConfigFile);
                       LogWriter.Write("还原配置文件开始...");
                       this.RestoreConfigFileByPath(new string[3]
                  {
              this.Config.ActiveConfigPath,
              this.Config.OEMConfigPath,
              Path.Combine(new DirectoryInfo(this.Config.OEMConfigPath).Parent.FullName, "Assets")
                 });
                   }
                   if (!App.restoreWorker.CancellationPending && !this.IsSucceed && this.IsDriverSetup)
                   {
                       App.restoreWorker.ReportProgress(5, (object)Resources.Msg_RestoreSetupDriver);
                       LogWriter.Write("驱动还原开始...");
                       this.InstallDirver(true);
                       LogWriter.Write("驱动还原结束...");
                   }
                   this.FirstRunAutoStart();
               }
               finally
               {
                   this.IsFinished = true;
               }
           });
            App.restoreWorker.RunWorkerCompleted += completed;
            App.restoreWorker.RunWorkerAsync();
        }

        private bool CheckRestoreEnvironment()
        {
            if (!this.IsCheckEnvironment)
            {
                LogWriter.Write("验证环境未通过，不需要还原操作...");
                this.IsRestored = true;
                return false;
            }
            if (!this.IsSucceed)
                return true;
            LogWriter.Write("已成功安装，不需要还原操作...");
            this.IsRestored = true;
            return false;
        }

        internal void RestoreFile_NK300()
        {
            if (!this.CheckRestoreEnvironment())
                return;
            try
            {
                this.IsBeginRestore = true;
                LogWriter.Write("删除后台进程...");
                this.CloseBackgroundProcesses();
                this.CloseBackgroundProcesses();
                LogWriter.Write("检查驱动状态开始...");
                this.CheckDirverProcessStatus();
                LogWriter.Write("还原安装文件开始...");
                this.RestoreSetupFile();
                LogWriter.Write("还原配置文件开始...");
                this.RestoreConfigFileByPath(new string[3]
                {
          this.Config.ActiveConfigPath,
          this.Config.OEMConfigPath,
          Path.Combine(new DirectoryInfo(this.Config.OEMConfigPath).Parent.FullName, "Assets")
                });
                if (this.IsDriverSetup)
                {
                    LogWriter.Write("驱动安装开始...");
                    this.InstallDirver(true);
                    LogWriter.Write("驱动安装结束...");
                }
                this.FirstRunAutoStart();
            }
            catch (SpaceNotEnoughException ex)
            {
                LogWriter.Write(ex.Message);
                throw ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                LogWriter.Write(ex.Message);
                throw new Exception(string.Format(Resources.Err_UnauthorizedAccessException, (object)this.Config.Path));
            }
            catch (IOException ex)
            {
                LogWriter.Write(ex.Message);
                throw new IOException(string.Format(Resources.Err_DirectoryOpened, (object)this.Config.Path));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.Err_UnknowError, (object)ex.Message));
            }
            finally
            {
                Env.Instance.CompleteUpdateConfigFile();
                this.IsFinished = true;
            }
        }

        internal void CreateAutoRestart()
        {
            if (!InstallContext.Instance.IsAutoStart)
                return;
            if (InstallContext.Instance.IsAutoRestartNoDesktop)
                this.StartMachineStartNoDesktop();
            if (!InstallContext.Instance.IsAutoRestart)
                return;
            this.StartMachineStart();
        }

        internal bool CheckExeProcess()
        {
            bool flag = false;
            try
            {
                LogWriter.UpdatePrompt(StringParser.Parse("${res:Msg_CheckNcStudioProcess}"));
                Process[] processesByName = Process.GetProcessesByName(this.Config.AppName);
                if (processesByName != null && processesByName.Length != 0)
                    flag = true;
                if (!flag)
                {
                    bool createdNew;
                    using (new Mutex(true, "SingletonPhoenixAppMutex", out createdNew))
                    {
                        if (!createdNew)
                            flag = true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return flag;
        }

        internal void CheckInstallEnvironment()
        {
            if (string.IsNullOrWhiteSpace(this.Config.Path))
                throw new Exception(Resources.Err_InstallPathIsNull);
            this.CheckPathEnvironment(this.Config.Path);
            this.CheckPathEnvironment(this.Config.ActiveConfigPath);
            this.CheckPathEnvironment(this.Config.OEMConfigPath);
            DriveInfo[] drives = DriveInfo.GetDrives();
            string activeConfigRoot = Path.GetPathRoot(this.Config.ActiveConfigPath);
            if (((IEnumerable<DriveInfo>)drives).FirstOrDefault<DriveInfo>((Func<DriveInfo, bool>)(o => o.Name.Equals(activeConfigRoot))) == null)
                throw new Exception(string.Format(Resources.Err_DriveNotExist, (object)activeConfigRoot));
            string oemConfigRoot = Path.GetPathRoot(this.Config.OEMConfigPath);
            if (((IEnumerable<DriveInfo>)drives).FirstOrDefault<DriveInfo>((Func<DriveInfo, bool>)(o => o.Name.Equals(oemConfigRoot))) == null)
                throw new Exception(string.Format(Resources.Err_DriveNotExist, (object)oemConfigRoot));
        }

        private void CheckPathEnvironment(string path)
        {
            if (!Directory.Exists(path))
                return;
            try
            {
                foreach (string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    FileStream fileStream = (FileStream)null;
                    try
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    }
                    finally
                    {
                        fileStream?.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.UpdatePrompt(ex.Message);
                throw new Exception(string.Format(Resources.Err_CheckFileUsed, (object)path));
            }
        }

        internal void CloseOriginalPathBackgroundProcesses()
        {
            if (string.IsNullOrEmpty(this.Config.OriginalSourcePath) || !Directory.Exists(this.Config.OriginalSourcePath))
                return;
            string lower = this.Config.OriginalSourcePath.ToLower();
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainModule.FileName.ToLower().Contains(lower))
                    {
                        process?.Kill();
                        process?.WaitForExit(200);
                    }
                }
                catch (Exception ex)
                {
                    LogWriter.Write("进程 " + process.ProcessName + " 处理异常！" + ex.Message);
                }
            }
        }

        internal void CloseBackgroundProcesses()
        {
            string lower = Directory.GetParent(this.Config.Path).FullName.ToLower();
            string procName = Path.GetFileNameWithoutExtension("firstrun.exe");
            if (this.Config.BackgroundProcesses.FindIndex((Predicate<string>)(o => o.Equals(procName, StringComparison.OrdinalIgnoreCase))) != -1)
                this.KillProcess(lower, procName);
            foreach (string backgroundProcess in this.Config.BackgroundProcesses)
                this.KillProcess(lower, backgroundProcess);
            this.CloseOriginalPathBackgroundProcesses();
        }

        internal void KillProcess(string rootDir, string processName)
        {
            foreach (Process process in Process.GetProcessesByName(processName))
            {
                try
                {
                    if (process.MainModule.FileName.ToLower().Contains(rootDir))
                    {
                        process?.Kill();
                        process?.WaitForExit(200);
                    }
                }
                catch (Exception ex)
                {
                    LogWriter.Write("进程 " + processName + " 无法关闭！" + ex.Message);
                }
            }
        }

        private MachineEntity GetCurrentSetupMachineInSetupPack(MachineEntity setupedMachine)
        {
            MachineEntity machineInSetupPack = (MachineEntity)null;
            if (setupedMachine != null)
                machineInSetupPack = InstallContext.Instance.MachineList.Find((Predicate<MachineEntity>)(m => string.Compare(m.MachineModel, setupedMachine.MachineModel, true) == 0 || string.Compare(m.GetConfigName("zh:"), setupedMachine.GetConfigName("zh:"), true) == 0));
            return machineInSetupPack;
        }

        private void DeleteReadOnlyFile(string path, List<string> list)
        {
            if (list != null && list.Contains(path))
                return;
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    LogWriter.Write("删除只读文件 " + file);
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                foreach (string directory in Directory.GetDirectories(path))
                    this.DeleteReadOnlyFile(directory, list);
                if (Directory.GetFiles(path).Length != 0 || Directory.GetDirectories(path).Length != 0)
                    return;
                LogWriter.Write("删除空文件夹 " + path);
                Directory.Delete(path);
            }
            else
            {
                if (!File.Exists(path))
                    return;
                LogWriter.Write("删除只读文件 " + path);
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
        }

        private void DeleteReadOnlyFile(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                }
                foreach (string directory in Directory.GetDirectories(path))
                    this.DeleteReadOnlyFile(directory);
            }
            else
            {
                if (!File.Exists(path))
                    return;
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
        }

        private void DeleteFolder(string path)
        {
            if (!Directory.Exists(path))
                return;
            string[] directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            if (directories.Length != 0)
            {
                foreach (string path1 in directories)
                    this.DeleteFolder(path1);
            }
            try
            {
                Directory.Delete(path);
            }
            catch (Exception ex)
            {
                LogWriter.Write(string.Format("删除文件夹{0}失败：{1}", (object)path, (object)ex.Message));
                this.Config.CurrentWorkPath = path;
                throw new IOException(string.Format(Resources.Err_DirectoryOpened, (object)path));
            }
        }

        private void CopyDirectory(
          string sourcePath,
          string destPath,
          List<string> listFilterDir,
          List<string> listFilterFile = null,
          bool notifyProgressChanged = false)
        {
            LogWriter.Write("正在复制文件夹 " + sourcePath);
            if (listFilterDir != null && listFilterDir.Contains(sourcePath))
                return;
            if (sourcePath[sourcePath.Length - 1] != '\\')
                sourcePath += "\\";
            if (destPath[destPath.Length - 1] != '\\')
                destPath += "\\";
            if (!Directory.Exists(sourcePath))
                return;
            if (!Directory.Exists(destPath))
                PathLib.CreateDirectory(destPath);
            if (!this.CopyFiles(sourcePath, destPath, listFilterFile, notifyProgressChanged))
                return;
            foreach (string directory in Directory.GetDirectories(sourcePath))
                this.CopyDirectory(directory, Path.Combine(destPath, directory.Substring(sourcePath.Length)), listFilterDir, listFilterFile, notifyProgressChanged);
        }

        private bool CopyFiles(
          string sourcePath,
          string destPath,
          List<string> listFilterFile = null,
          bool notifyProgressChanged = false)
        {
            try
            {
                if (sourcePath[sourcePath.Length - 1] != '\\')
                    sourcePath += "\\";
                if (destPath[destPath.Length - 1] != '\\')
                    destPath += "\\";
                foreach (string file in Directory.GetFiles(sourcePath))
                {
                    if (listFilterFile == null || !listFilterFile.Contains(file))
                    {
                        LogWriter.Write(Resources.Msg_File_Copy + file, notifyProgressChanged);
                        PathLib.CopyFile(file, Path.Combine(destPath, file.Substring(sourcePath.Length)));
                    }
                }
            }
            catch (SpaceNotEnoughException ex)
            {
                string text = string.Format(Resources.Err_MemoryNotEnough, (object)Directory.GetDirectoryRoot(destPath).Substring(0, 1));
                LogWriter.Write(ex.Message);
                LogWriter.UpdatePrompt(text);
                throw ex;
            }
            catch (Exception ex)
            {
                string str = string.Format(Resources.Err_UnknowError, (object)ex.Message);
                LogWriter.Write(ex.Message);
                LogWriter.UpdatePrompt(str);
                throw new Exception(str);
            }
            return true;
        }

        internal void ExtractZip(string zipPath)
        {
            LogWriter.UpdatePrompt(Resources.Msg_CopyToSetupPath);
            string str1 = Path.Combine(Directory.GetParent(this.Config.Path).FullName, "~Temp");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            if (!File.Exists(zipPath))
                throw new Exception(string.Format(Resources.Err_MemoryNotEnoughOrPackageDamaged, (object)Directory.GetDirectoryRoot(zipPath).Substring(0, 1)));
            LogWriter.Write("将 NcStudio 目录重命名重命名");
            if (Directory.Exists(this.Config.Path))
            {
                this.hasHistoryVersion = true;
                if (Directory.Exists(str1))
                {
                    this.DeleteReadOnlyFile(str1);
                    this.DeleteFolder(str1);
                }
                int n = ShellFileOperation.Rename(this.Config.Path, str1);
                if (n != 0)
                {
                    string errorString = ShellFileOperation.GetErrorString(n);
                    if (n.ToString("X").Equals(errorString, StringComparison.OrdinalIgnoreCase))
                        throw new IOException(string.Format("ShellFileOperation Rename Unknown Error:{0}.", (object)n));
                    throw new Exception(errorString);
                }
                if (!this.dirBackUpMapping.ContainsKey(this.Config.Path))
                    this.dirBackUpMapping.Add(this.Config.Path, str1);
            }
            this.BackUpConfigFileByPath(new string[3]
            {
        this.Config.ActiveConfigPath,
        this.Config.OEMConfigPath,
        Path.Combine(new DirectoryInfo(this.Config.OEMConfigPath).Parent.FullName, "Assets")
            });
            this.IsFinishedBackUpFile = true;
            LogWriter.Write("将安装文件拷贝至安装目录");
            if (!Directory.Exists(this.Config.Path))
                PathLib.CreateDirectory(this.Config.Path);
            using (FileStream baseInputStream = File.OpenRead(zipPath))
            {
                if (baseInputStream.Length == 0L)
                    throw new SpaceNotEnoughException(string.Format("{0}: {1} byte", (object)zipPath, (object)baseInputStream.Length))
                    {
                        File = zipPath,
                        NeedBytes = baseInputStream.Length
                    };
                using (ZipInputStream zipInputStream = new ZipInputStream((Stream)baseInputStream))
                {
                    ZipEntry nextEntry;
                    while ((nextEntry = zipInputStream.GetNextEntry()) != null)
                    {
                        if (!this.IsFinished)
                        {
                            LogWriter.Write("正在抽取 " + nextEntry.Name);
                            LogWriter.UpdatePrompt(Path.GetFileName(nextEntry.Name) ?? "");
                            int copyFilePercent = this.GetCopyFilePercent((double)baseInputStream.Position / (double)baseInputStream.Length);
                            try
                            {
                                App.installWorker?.ReportProgress(copyFilePercent, (object)Resources.Msg_InstallingProc);
                            }
                            catch
                            {
                            }
                            string str2 = this.Config.Path;
                            string directoryName = Path.GetDirectoryName(nextEntry.Name);
                            string fileName = Path.GetFileName(nextEntry.Name);
                            if (!string.IsNullOrWhiteSpace(directoryName))
                            {
                                str2 = Path.Combine(str2, directoryName);
                                PathLib.CreateDirectory(str2);
                            }
                            if (zipInputStream.Length >= 0L && !string.IsNullOrWhiteSpace(fileName))
                            {
                                string str3 = Path.Combine(str2, fileName);
                                byte[] numArray = new byte[zipInputStream.Length];
                                zipInputStream.Read(numArray, 0, numArray.Length);
                                if (PathLib.GetHardDiskSpace(Path.GetPathRoot(str3).ToUpper()) < (long)numArray.Length)
                                    throw new InvalidDataException(string.Format("{0}: {1} byte", (object)str3, (object)numArray.Length));
                                if (Path.GetFileName(nextEntry.Name).IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                                    throw new FileNameInvalidException("file name invalid")
                                    {
                                        FileName = nextEntry.Name
                                    };
                                if (PathLib.PathIsInvalid(nextEntry.Name))
                                    throw new PathInvalidException("package file path invalid")
                                    {
                                        Path = nextEntry.Name
                                    };
                                Installer.WriteFile(str3, numArray);
                            }
                        }
                        else
                            break;
                    }
                }
            }
            this.InstallDefaultConfig();
            if (!InstallContext.Instance.IsReservedManufacturer)
                this.InstallAssets();
            this.InstallNcAdapter();
            stopwatch.Stop();
            LogWriter.Write(string.Format("抽取文件: {0}", (object)stopwatch.Elapsed.TotalMilliseconds));
        }

        internal void InstallAssets()
        {
            string str1 = Path.Combine(this.Config.Path, "Assets");
            string str2 = Path.Combine(new DirectoryInfo(this.Config.OEMConfigDir).Parent.FullName, "Assets");
            string path1 = Path.Combine(str1, "OEMConfigs");
            string str3 = Path.Combine(str2, "OEMConfigs");
            if (Directory.Exists(str3))
            {
                string str4 = str3 + ".delete";
                try
                {
                    Directory.Move(str3, str4);
                }
                catch
                {
                    ShellFileOperation.Rename(str3, str4);
                }
                this.DeleteReadOnlyFile(str4);
                this.DeleteFolder(str4);
            }
            if (!Directory.Exists(str1))
                return;
            if (!Directory.Exists(str2) || !Directory.Exists(str3))
            {
                this.CopyDirectory(str1, str2, new List<string>());
            }
            else
            {
                if (!Directory.Exists(str3))
                    return;
                Dictionary<string, OEMConfigAsset> dictionary1 = new Dictionary<string, OEMConfigAsset>();
                Dictionary<string, OEMConfigAsset> dictionary2 = new Dictionary<string, OEMConfigAsset>();
                foreach (string directory in Directory.GetDirectories(path1))
                {
                    string configMachineModel = PathLib.GetOemConfigMachineModel(directory);
                    OEMConfigAsset oemConfigAsset = new OEMConfigAsset()
                    {
                        MachineModel = configMachineModel,
                        OEMConfigPath = directory
                    };
                    if (dictionary1.ContainsKey(configMachineModel))
                        dictionary1[configMachineModel] = oemConfigAsset;
                    else
                        dictionary1.Add(configMachineModel, oemConfigAsset);
                }
                foreach (string path2 in Directory.Exists(str3) ? Directory.GetDirectories(str3) : new string[0])
                {
                    string configMachineModel = PathLib.GetOemConfigMachineModel(path2);
                    OEMConfigAsset oemConfigAsset = new OEMConfigAsset()
                    {
                        MachineModel = configMachineModel,
                        OEMConfigPath = path2
                    };
                    if (!dictionary2.ContainsKey(configMachineModel))
                        dictionary2.Add(configMachineModel, oemConfigAsset);
                    else
                        dictionary2[configMachineModel] = oemConfigAsset;
                }
                foreach (KeyValuePair<string, OEMConfigAsset> keyValuePair in dictionary1)
                {
                    string key = keyValuePair.Key;
                    string sourceFile = "";
                    if (dictionary2.ContainsKey(key))
                    {
                        sourceFile = dictionary2[key].OEMConfigPath;
                        string str5 = sourceFile + ".delete";
                        ShellFileOperation.Move(sourceFile, str5);
                        this.DeleteReadOnlyFile(str5);
                        this.DeleteFolder(str5);
                    }
                    string destPath = string.IsNullOrEmpty(sourceFile) ? PathLib.GetUnUsedPath(str3, "OEMConfig_") : sourceFile;
                    LogWriter.Write("Copy Assert:" + keyValuePair.Value.OEMConfigPath + "->" + destPath);
                    this.CopyDirectory(keyValuePair.Value.OEMConfigPath, destPath, new List<string>());
                }
            }
        }

        internal void RestoreBackupFileForEnvCheckFailed()
        {
            try
            {
                if (this.dirBackUpMapping == null || this.dirBackUpMapping.Keys == null || this.dirBackUpMapping.Keys.Count <= 0)
                    return;
                foreach (string key in this.dirBackUpMapping.Keys)
                {
                    string str = this.dirBackUpMapping[key];
                    LogWriter.Write("环境异常备份还原。。。");
                    LogWriter.Write("备份文件夹:" + str);
                    LogWriter.Write("安装文件夹:" + key);
                    if (Directory.Exists(str))
                    {
                        if (Directory.Exists(key))
                        {
                            this.DeleteReadOnlyFile(key);
                            this.DeleteFolder(key);
                        }
                        PathLib.RenameFile(str, key);
                        LogWriter.Write("环境异常备份还原完成。。。");
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Write("还原备份文件夹失败：" + ex.Message);
            }
        }

        public void GotoRestoreFilePage()
        {
            if (this.IsSucceed)
                return;
            WizardPage wizardPage = Wizard.Instance.GetWizardPage("RestorePage");
            if (wizardPage == null)
                return;
            Wizard.Instance.CurrentPage.NextPage = wizardPage;
            Wizard.Instance.CurrentPage.CanFinish = new bool?(true);
            Wizard.Instance.ExecuteSelectNextPage((object)null, (ExecutedRoutedEventArgs)null);
        }

        internal void RestoreSetupFile()
        {
            string empty = string.Empty;
            string str = Path.Combine(Directory.GetParent(this.Config.Path).FullName, "~Temp");
            if (!Directory.Exists(str))
                return;
            if (Directory.Exists(this.Config.Path))
            {
                try
                {
                    this.DeleteFileForUseredDirectory(this.Config.Path);
                    Directory.Delete(this.Config.Path, true);
                }
                catch (Exception ex1)
                {
                    try
                    {
                        this.DeleteReadOnlyFile(this.Config.Path);
                        this.DeleteFolder(this.Config.Path);
                    }
                    catch (Exception ex2)
                    {
                        LogWriter.Write("RestoreSetupFile：" + ex2.Message);
                        throw;
                    }
                }
            }
            LogWriter.Write("NcStudio 安装未完成，将 NcStudio 还原至安装目录");
            try
            {
                this.CloseBackgroundProcesses();
                int n = ShellFileOperation.Rename(str, this.Config.Path);
                if (n != 0)
                {
                    string errorString = ShellFileOperation.GetErrorString(n);
                    if (n.ToString("X").Equals(errorString, StringComparison.OrdinalIgnoreCase))
                        throw new IOException(string.Format("ShellFileOperation Rename Unknown Error:{0}.", (object)n));
                    throw new Exception(errorString);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Write("NcStudio 还原安装文件失败：" + ex.Message);
                throw;
            }
            this.CleanTempDir(str);
        }

        private void CleanTempDir(string tempName)
        {
            if (!Directory.Exists(tempName))
                return;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            try
            {
                Directory.Delete(tempName, true);
            }
            catch (Exception ex1)
            {
                try
                {
                    this.DeleteReadOnlyFile(tempName);
                    this.DeleteFolder(tempName);
                }
                catch (Exception ex2)
                {
                    LogWriter.Write("清理备份目录失败:" + ex2.Message);
                }
            }
            stopwatch.Stop();
            LogWriter.Write(string.Format("清理备份目录: {0}", (object)stopwatch.Elapsed.TotalMilliseconds));
        }

        public void BackUpConfigFileByPath(string[] paths)
        {
            string str1 = string.Empty;
            try
            {
                foreach (string path in paths)
                {
                    string empty = string.Empty;
                    string str2 = string.Empty;
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    str1 = path;
                    if (string.IsNullOrEmpty(path))
                        break;
                    string path1 = !path.EndsWith("\\") ? Directory.GetParent(path).FullName : Directory.GetParent(path.Remove(path.Length - 2)).FullName;
                    if (!string.IsNullOrWhiteSpace(directoryInfo.Name))
                        str2 = Path.Combine(path1, "~Temp" + directoryInfo.Name);
                    LogWriter.Write("备份配置文件夹... " + path);
                    LogWriter.Write("备份配置文件夹到：" + str2);
                    if (Directory.Exists(path))
                    {
                        if (Directory.Exists(str2))
                        {
                            LogWriter.Write("删除文件夹... " + str2);
                            this.DeleteReadOnlyFile(str2);
                            this.DeleteFolder(str2);
                        }
                        PathLib.RenameFile(path, str2);
                        if (!this.dirBackUpMapping.ContainsKey(path))
                            this.dirBackUpMapping.Add(path, str2);
                        this.CopyDirectory(str2, path, new List<string>(), this.GetFilterFile(str2));
                    }
                }
            }
            catch (SpaceNotEnoughException ex)
            {
                LogWriter.Write(ex.Message);
                throw ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                LogWriter.Write(ex.Message);
                throw new Exception(string.Format(Resources.Err_UnauthorizedAccessException, (object)this.Config.Path));
            }
            catch (IOException ex)
            {
                LogWriter.Write(ex.Message);
                this.Config.CurrentWorkPath = str1;
                throw new IOException(string.Format(Resources.Err_DirectoryOpened, (object)str1));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.Err_UnknowError, (object)ex.Message));
            }
        }

        public void DeleteBackupConfigFileByPath(string[] paths, bool throwExecption = true)
        {
            foreach (string path1 in paths)
            {
                string empty = string.Empty;
                string path2 = string.Empty;
                DirectoryInfo directoryInfo = new DirectoryInfo(path1);
                if (string.IsNullOrEmpty(path1))
                    break;
                string path1_1 = !path1.EndsWith("\\") ? Directory.GetParent(path1).FullName : Directory.GetParent(path1.Remove(path1.Length - 2)).FullName;
                if (!string.IsNullOrWhiteSpace(directoryInfo.Name))
                    path2 = Path.Combine(path1_1, "~Temp" + directoryInfo.Name);
                try
                {
                    LogWriter.Write("删除配置备份文件... " + path2);
                    if (Directory.Exists(path2))
                    {
                        this.DeleteReadOnlyFile(path2);
                        this.DeleteFolder(path2);
                    }
                }
                catch (Exception ex1)
                {
                    LogWriter.Write("尝试删除备份文件失败：" + ex1.Message);
                    try
                    {
                        LogWriter.Write("尝试再次删除备份文件... " + path2);
                        if (Directory.Exists(path2))
                        {
                            this.DeleteReadOnlyFile(path2);
                            this.DeleteFolder(path2);
                        }
                    }
                    catch (Exception ex2)
                    {
                        LogWriter.Write("尝试再次删除备份文件失败：" + ex2.Message);
                        this.Config.CurrentWorkPath = path2;
                        if (throwExecption)
                            throw new IOException(string.Format(Resources.Err_DirectoryOpened, (object)path2));
                    }
                }
            }
        }

        internal void RestoreConfigFileByPath(string[] paths)
        {
            string empty1 = string.Empty;
            try
            {
                foreach (string path in paths)
                {
                    string empty2 = string.Empty;
                    string str = string.Empty;
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    if (string.IsNullOrEmpty(path))
                        break;
                    string path1 = !path.EndsWith("\\") ? Directory.GetParent(path).FullName : Directory.GetParent(path.Remove(path.Length - 2)).FullName;
                    if (!string.IsNullOrWhiteSpace(directoryInfo.Name))
                        str = Path.Combine(path1, "~Temp" + directoryInfo.Name);
                    LogWriter.Write("还原配置文件夹... " + path);
                    LogWriter.Write("还原配置文件夹来源：" + str);
                    if (Directory.Exists(str))
                    {
                        if (Directory.Exists(path))
                        {
                            LogWriter.Write("删除文件夹... " + path);
                            this.DeleteReadOnlyFile(path);
                            this.DeleteFolder(path);
                        }
                        PathLib.RenameFile(str, path);
                    }
                }
            }
            finally
            {
                if (this.Config != null)
                    this.Config.CurrentWorkPath = string.Empty;
            }
        }

        internal static void WriteFile(string destFileName, byte[] bytes)
        {
            if (PathLib.PathIsInvalid(destFileName))
                throw new PathInvalidException("file path invalid")
                {
                    Path = destFileName
                };
            using (FileStream fileStream = new FileStream(destFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.WriteThrough))
                fileStream.Write(bytes, 0, bytes.Length);
        }

        internal void InstallNcAdapter()
        {
            if (InstallContext.Instance.IsNcCloudAdapterSetUp)
            {
                this.UnstallLocalNcAdapter();
            }
            else
            {
                string path = Path.Combine(this.Config.Path, "Bin", "NcAdapter");
                if (!Directory.Exists(path))
                    return;
                this.DeleteReadOnlyFile(path);
                this.DeleteFolder(path);
            }
        }

        private void UnstallLocalNcAdapter()
        {
            try
            {
                string name1 = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{A12F97E0-5776-45D8-86BC-182B73B5DC37}_is1";
                string name2 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{A12F97E0-5776-45D8-86BC-182B73B5DC37}_is1";
                RegistryKey localMachine = Registry.LocalMachine;
                RegistryKey registryKey = localMachine.OpenSubKey(name1) ?? localMachine.OpenSubKey(name2);
                if (registryKey == null)
                    return;
                object obj = registryKey.GetValue("UninstallString");
                string str1;
                if (obj == null)
                    str1 = (string)null;
                else
                    str1 = obj.ToString().Trim('"');
                string str2 = str1;
                if (string.IsNullOrEmpty(str2) || !File.Exists(str2))
                    return;
                string fullName = Directory.GetParent(str2).FullName;
                PathLib.CreateProcess(str2, fullName, "/verysilent");
            }
            catch (Exception ex)
            {
                LogWriter.Write("UnstallLocalNcAdapter Exception." + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        internal void InstallDefaultConfig()
        {
            string name = new DirectoryInfo(this.Config.DefaultConfigDir).Name;
            string str = Path.Combine(this.Config.Path, name);
            ConfigEntity selectedValue = InstallContext.Instance.SelectedValue;
            if (selectedValue != null && !selectedValue.DirectoryName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                string sourcePath = Path.Combine(this.Config.Path, InstallContext.Instance.SelectedValue.DirectoryName);
                if (Directory.Exists(str))
                {
                    this.DeleteReadOnlyFile(str);
                    this.DeleteFolder(str);
                }
                this.CopyDirectory(sourcePath, str, new List<string>());
            }
            foreach (ConfigEntity config in InstallContext.Instance.ConfigList)
            {
                if (!config.DirectoryName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    string path = Path.Combine(this.Config.Path, config.DirectoryName);
                    this.DeleteReadOnlyFile(path);
                    this.DeleteFolder(path);
                }
            }
        }

        internal void InstallOEMConfig()
        {
            string oemConfigDir = this.Config.OEMConfigDir;
            string directoryName = Path.GetDirectoryName(oemConfigDir);
            MachineEntity machineSelectedValue = InstallContext.Instance.MachineSelectedValue;
            if (machineSelectedValue == null)
                return;
            string str = Path.Combine(this.Config.Path, machineSelectedValue.DirectoryName);
            if (machineSelectedValue.DirectoryName.Equals(directoryName, StringComparison.OrdinalIgnoreCase))
                return;
            if (Directory.Exists(str))
            {
                this.DeleteReadOnlyFile(oemConfigDir);
                this.DeleteFolder(oemConfigDir);
            }
            this.CopyDirectory(str, oemConfigDir, new List<string>()
      {
        Path.Combine(str, "Machine")
      });
        }

        internal void CopyConfigFiles()
        {
            string path1 = Path.Combine(this.Config.Path, new DirectoryInfo(this.Config.DefaultConfigDir).Name);
            string str1 = Path.Combine(path1, "Config.cfg");
            string str2 = Path.Combine(this.Config.ActiveConfigPath, "Config.cfg");
            string str3 = Path.Combine(this.Config.OEMConfigPath, "Config.cfg");
            LogWriter.Write("defaultCfg = " + str1);
            LogWriter.Write("acitveCfg = " + str2);
            LogWriter.Write("oemCfg = " + str3);
            if (!File.Exists(str2) && File.Exists(str3))
            {
                LogWriter.Write("copy oemCfg to activeCfg");
                PathLib.CopyFile(str3, str2);
            }
            else if (File.Exists(str2) && !File.Exists(str3))
            {
                LogWriter.Write("copy activeCfg to oemCfg");
                PathLib.CopyFile(str2, str3);
            }
            else if (File.Exists(str2) && File.Exists(str3))
            {
                LogWriter.Write("Update oemCfg to activeCfg!");
                XElement xelement1 = XElement.Load(str3);
                XElement xelement2 = XElement.Load(str2);
                string[] strArray = new string[2]
                {
          "MachineModel",
          "Name"
                };
                bool flag = false;
                foreach (string name in strArray)
                {
                    string str4 = xelement1.Element((XName)name)?.Value;
                    XElement content = xelement2.Element((XName)name);
                    if (content == null)
                    {
                        content = new XElement((XName)name);
                        xelement2.Add((object)content);
                        flag = true;
                    }
                    if (str4 != null && !content.Value.Equals(str4, StringComparison.OrdinalIgnoreCase))
                    {
                        content.Value = str4;
                        flag = true;
                    }
                }
                if (flag)
                    xelement2.Save(str2);
            }
            else if (File.Exists(str1) && !File.Exists(str2) && !File.Exists(str3))
            {
                LogWriter.Write("copy defaultCfg");
                PathLib.CopyFile(str1, str3);
                PathLib.CopyFile(str1, str2);
            }
            string str5 = Path.Combine(path1, "Config.cmd");
            if (!File.Exists(str5))
                return;
            PathLib.CreateProcess(str5, this.Config.ActiveConfigPath, (string)null);
            File.Delete(str5);
        }

        internal void InstallFirstRun()
        {
            string str1 = Path.Combine(this.Config.Path, "Bin", "firstrun.exe");
            string str2 = Path.Combine(Env.Instance.FirstRunDir, "firstrun.exe");
            if (!File.Exists(str1))
                return;
            try
            {
                if (File.Exists(str2))
                {
                    string strA = (string)null;
                    using (FileStream inputStream = File.Open(str1, FileMode.Open, FileAccess.Read))
                        strA = this.GetMd5Hash((Stream)inputStream);
                    using (FileStream inputStream = File.Open(str2, FileMode.Open, FileAccess.Read))
                    {
                        string md5Hash = this.GetMd5Hash((Stream)inputStream);
                        if (!string.IsNullOrWhiteSpace(md5Hash))
                        {
                            if (!string.IsNullOrWhiteSpace(strA))
                            {
                                if (string.Compare(strA, md5Hash, StringComparison.OrdinalIgnoreCase) == 0)
                                    return;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            if (File.Exists(str2))
            {
                File.SetAttributes(str2, FileAttributes.Normal);
                File.Delete(str2);
            }
            PathLib.CopyFile(str1, str2);
        }

        private string GetMd5Hash(Stream inputStream) => BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(inputStream));

        internal void InstallDirver(bool isForce = false)
        {
            if (!isForce && this.IsFinished)
                return;
            LogWriter.UpdatePrompt(Resources.Msg_UpdatingDriver);
            string[] files = Directory.GetFiles(this.Config.Path, "DriverPath.ini", SearchOption.AllDirectories);
            if (files.Length == 0)
                return;
            string fullName = Directory.GetParent(files[0]).FullName;
            string str = Path.Combine(fullName, "InstallDriver.exe");
            if (Environment.Is64BitOperatingSystem)
                str = Path.Combine(fullName, "InstallDriver_64.exe");
            if (!File.Exists(str) || PathLib.CreateProcess(str, fullName, (string)null))
                return;
            InstallContext.Instance.IsDirverFailed = true;
        }

        internal void CheckDirverProcessStatus()
        {
            LogWriter.Write("确认驱动运行状态...开始");
            if (!File.Exists(this.Config.Path))
                return;
            string[] files = Directory.GetFiles(this.Config.Path, "DriverPath.ini", SearchOption.AllDirectories);
            if (files.Length == 0)
                return;
            string fullName = Directory.GetParent(files[0]).FullName;
            string str = Path.Combine(fullName, "InstallDriver.exe");
            string processName = "InstallDriver.exe";
            if (Environment.Is64BitOperatingSystem)
            {
                str = Path.Combine(fullName, "InstallDriver_64.exe");
                processName = "InstallDriver_64.exe";
            }
            if (!File.Exists(str))
                return;
            while (this.CheckProcessRunning(str, processName))
            {
                Thread.Sleep(1000);
                LogWriter.Write("驱动进程" + processName + "正在运行...");
            }
            LogWriter.Write("确认驱动运行状态...结束");
        }

        internal bool CheckProcessRunning(string processPath, string processName)
        {
            bool flag = false;
            try
            {
                LogWriter.Write("确认进程" + processName + "运行状态...开始");
                foreach (Process process in Process.GetProcessesByName(processName))
                {
                    if (process.MainModule.FileName.ToLower().Contains(processPath))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            catch
            {
                flag = false;
            }
            LogWriter.Write("进程" + processName + "运行状态...结束：" + flag.ToString());
            return flag;
        }

        internal void DisableSleep() => PathLib.CreateProcess("cmd.exe", "", string.Format("/c powercfg /h off && reg add {0} /v AwayModeEnabled /t REG_DWORD /d 1 /f", (object)"\"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\""));

        internal void PromptAdmin() => PathLib.CreateProcess("cmd.exe", "", "/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v EnableLUA /t REG_DWORD /d 0 /f", true);

        internal void TransParam()
        {
            this.CreateFlag();
            string path1 = Path.Combine(this.Config.ActiveConfigPath, "_DAC");
            if (Directory.Exists(path1))
                this.DeleteReadOnlyFile(path1, new List<string>());
            if (Directory.Exists(this.Config.ActiveConfigPath))
            {
                foreach (string file in Directory.GetFiles(this.Config.ActiveConfigPath))
                {
                    string upper = Path.GetExtension(file).Replace(".", "").ToUpper();
                    if (upper == "DYN" || upper == "LOG" || upper == "BAK")
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                }
            }
            if (!InstallContext.Instance.IsReservedUser)
                this.DeleteReadOnlyFile(this.Config.ActiveConfigPath, new List<string>()
        {
          Path.Combine(this.Config.ActiveConfigPath, "Machine")
        });
            string oemConfigDir = this.Config.OEMConfigDir;
            if (Directory.Exists(oemConfigDir))
            {
                if (!InstallContext.Instance.IsReservedManufacturer)
                {
                    this.DeleteReadOnlyFile(this.Config.OEMConfigPath, new List<string>());
                    this.InstallOEMConfig();
                }
                if (Directory.Exists(oemConfigDir))
                {
                    string path2 = Path.Combine(oemConfigDir, "_DAC");
                    if (Directory.Exists(path2))
                        this.DeleteReadOnlyFile(path2, new List<string>());
                    foreach (string file in Directory.GetFiles(oemConfigDir))
                    {
                        string upper = Path.GetExtension(file).Replace(".", "").ToUpper();
                        if (upper == "DYN" || upper == "LOG" || upper == "BAK")
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Delete(file);
                        }
                    }
                }
            }
            else
                this.InstallOEMConfig();
            if (!Directory.Exists(this.Config.ActiveConfigPath))
                PathLib.CreateDirectory(this.Config.ActiveConfigPath);
            if (!Directory.Exists(this.Config.OEMConfigPath))
                PathLib.CreateDirectory(this.Config.OEMConfigPath);
            this.CopyConfigFiles();
            try
            {
                string path3 = Path.Combine(this.Config.Path, "Assets");
                if (Directory.Exists(path3))
                {
                    this.DeleteReadOnlyFile(path3);
                    this.DeleteFolder(path3);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Write("删除安装目录的Assets异常." + ex.Message + "." + ex.StackTrace);
            }
            File.Delete(Env.Instance.TransParamFlagPath);
        }

        internal void DealOriginalSourcePath()
        {
            try
            {
                if (string.IsNullOrEmpty(this.Config.OriginalSourcePath))
                    return;
                Packup.Library.Log.Write("安装完成后原安装路径：" + this.Config.OriginalSourcePath + "处理");
                string assets = Path.Combine(new DirectoryInfo(this.Config.OEMConfigPath).Parent.FullName, "Assets");
                if (Installer.Instance.IsSucceed)
                {
                    Packup.Library.Log.Write("删除原快捷方式");
                    this.DeleteOriginalShutcut();
                    if (!Directory.Exists(this.Config.OriginalSourcePath))
                        return;
                    this.DeleteReadOnlyFile(this.Config.OriginalSourcePath);
                    this.DeleteFolder(this.Config.OriginalSourcePath);
                }
                else
                {
                    Packup.Library.Log.Write("还原原安装路径");
                    this.RestoreToOriginalPath(assets);
                }
            }
            catch (Exception ex)
            {
                Packup.Library.Log.Write("安装完成处理原安装目录异常：" + ex.Message);
            }
        }

        private void DeleteOriginalShutcut()
        {
            string path2_1 = this.Config.OriginalAppName + ".lnk";
            string folderPath1 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
            string path2_2 = path2_1;
            string path1 = Path.Combine(folderPath1, path2_2);
            if (File.Exists(path1))
                File.Delete(path1);
            string path2 = Path.Combine(folderPath2, path2_1);
            if (!File.Exists(path2))
                return;
            File.Delete(path2);
        }

        private void RestoreToOriginalPath(string assets)
        {
            try
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add(this.Config.OriginalActiveConfigPath, this.Config.ActiveConfigPath);
                dictionary.Add(this.Config.OriginalOEMConfigPath, this.Config.OEMConfigPath);
                dictionary.Add(this.Config.OriginalAssetsPath, assets);
                foreach (string key in dictionary.Keys)
                {
                    string str = dictionary[key];
                    LogWriter.Write("原安装目录还原。。。");
                    LogWriter.Write("新安装文件夹:" + str);
                    LogWriter.Write("原安装文件夹:" + key);
                    if (Directory.Exists(str))
                    {
                        if (Directory.Exists(key))
                        {
                            this.DeleteReadOnlyFile(key);
                            this.DeleteFolder(key);
                        }
                        PathLib.MoveFile(str, key);
                        LogWriter.Write("原安装目录还原完成。。。");
                    }
                }
            }
            catch (Exception ex)
            {
                Packup.Library.Log.Write("安装完成处理原安装目录异常：" + ex.Message);
            }
        }

        internal void RenameOriginalPathToCurrentPath()
        {
            if (Directory.Exists(this.Config.OriginalActiveConfigPath))
            {
                Packup.Library.Log.Write("原ActiveConfig路径：" + this.Config.OriginalActiveConfigPath + " 重命名为当前ActiveConfig路径");
                if (Directory.Exists(this.Config.ActiveConfigPath))
                {
                    this.DeleteReadOnlyFile(this.Config.ActiveConfigPath);
                    this.DeleteFolder(this.Config.ActiveConfigPath);
                }
                PathLib.MoveFile(this.Config.OriginalActiveConfigPath, this.Config.ActiveConfigPath);
            }
            if (Directory.Exists(this.Config.OriginalOEMConfigPath))
            {
                Packup.Library.Log.Write("原OEMConfig路径：" + this.Config.OriginalOEMConfigPath + " 重命名为当前OEMConfig路径");
                if (Directory.Exists(this.Config.OEMConfigPath))
                {
                    this.DeleteReadOnlyFile(this.Config.OEMConfigPath);
                    this.DeleteFolder(this.Config.OEMConfigPath);
                }
                PathLib.MoveFile(this.Config.OriginalOEMConfigPath, this.Config.OEMConfigPath);
            }
            string str = Path.Combine(new DirectoryInfo(this.Config.OEMConfigPath).Parent.FullName, "Assets");
            string originalAssetsPath = this.Config.OriginalAssetsPath;
            if (!Directory.Exists(originalAssetsPath))
                return;
            Packup.Library.Log.Write("原Assets路径：" + originalAssetsPath + " 重命名为当前Assets路径");
            if (Directory.Exists(str))
            {
                this.DeleteReadOnlyFile(str);
                this.DeleteFolder(str);
            }
            PathLib.MoveFile(originalAssetsPath, str);
        }

        internal List<string> GetFilterFile(string path)
        {
            List<string> filterFile = new List<string>();
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    string upper = Path.GetExtension(file).Replace(".", "").ToUpper();
                    if (upper == "DYN" || upper == "LOG" || upper == "BAK")
                        filterFile.Add(file);
                }
            }
            return filterFile;
        }

        internal void CreateFlag()
        {
            try
            {
                if (!Directory.Exists(Env.Instance.TransParamFlagDir))
                    PathLib.CreateDirectory(Env.Instance.TransParamFlagDir);
                if (File.Exists(Env.Instance.TransParamFlagPath))
                {
                    File.SetAttributes(Env.Instance.TransParamFlagPath, FileAttributes.Normal);
                    File.Delete(Env.Instance.TransParamFlagPath);
                }
                using (FileStream fileStream = new FileStream(Env.Instance.TransParamFlagPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.WriteThrough))
                {
                    string s = DateTime.Now.ToString();
                    fileStream.Write(Encoding.UTF8.GetBytes(s), 0, Encoding.UTF8.GetByteCount(s));
                }
            }
            catch (Exception ex)
            {
                LogWriter.Write("创建安装标记文件异常！" + ex.Message);
            }
        }

        internal bool? CheckFlag()
        {
            if (!InstallContext.Instance.IsReservedManufacturer && !InstallContext.Instance.IsReservedUser)
                return new bool?(true);
            if (!File.Exists(Env.Instance.TransParamFlagPath))
                return new bool?(true);
            string msg = string.Format(Resources.War_ConfigRisk, (object)this.Config.ActiveConfigPath, (object)this.Config.OEMConfigPath);
            SetupMessageBoxButton resultButton = SetupMessageBoxButton.Close;
            Application.Current.Dispatcher.Invoke((Action)(() => resultButton = new SetupMessageBox().Show(msg, Resources.Msg_Warning, SystemIcons.Warning.ToBitmap(), Resources.Btn_Clear + "(F6)", Resources.Btn_Continue + "(F7)", Resources.Btn_Exit + "(F8)")));
            switch (resultButton)
            {
                case SetupMessageBoxButton.Button1:
                    string clearConfirmMsg = string.Format(Resources.Msg_SetupOldParamWillClear, (object)this.Config.ActiveConfigPath, (object)this.Config.OEMConfigPath);
                    if (InstallType.AIO == this.Config.InstallType)
                    {
                        TempDialogButton result = TempDialogButton.Yes;
                        Application.Current.Dispatcher.Invoke((Action)(() => result = TempDialog.Show((Window)null, clearConfirmMsg, Resources.Msg_Warning, TempDialogButton.YesNo)));
                        if (result != TempDialogButton.Yes)
                            return new bool?();
                        this.ClearConfig();
                        return new bool?(true);
                    }
                    if (MessageBox.Show(clearConfirmMsg, Resources.Msg_Warning, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                        return new bool?();
                    this.ClearConfig();
                    return new bool?(true);
                case SetupMessageBoxButton.Button2:
                    return new bool?(true);
                case SetupMessageBoxButton.Button3:
                    return new bool?(false);
                default:
                    return new bool?();
            }
        }

        internal void ClearConfig()
        {
            this.DeleteReadOnlyFile(this.Config.ActiveConfigPath, new List<string>()
      {
        Path.Combine(this.Config.ActiveConfigPath, "Machine")
      });
            this.DeleteReadOnlyFile(this.Config.OEMConfigPath);
        }

        internal bool CheckMachine(out MachineEntity setupedMachine)
        {
            bool flag = false;
            setupedMachine = (MachineEntity)null;
            try
            {
                string str = Path.Combine(this.Config.OEMConfigDir, "Config.cfg");
                if (!Directory.Exists(this.Config.OEMConfigDir))
                    flag = true;
                else if (File.Exists(str))
                {
                    MachineEntity machineSelectedValue = InstallContext.Instance.MachineSelectedValue;
                    setupedMachine = PathLib.GetSetupedMachineModel(str);
                    MachineEntity machineInSetupPack = this.GetCurrentSetupMachineInSetupPack(setupedMachine);
                    if (machineInSetupPack != null)
                    {
                        setupedMachine = machineInSetupPack;
                        if (string.IsNullOrWhiteSpace(machineSelectedValue.MachineModel) && string.IsNullOrWhiteSpace(setupedMachine.MachineModel) && string.Compare(machineSelectedValue.MachineModel, setupedMachine.MachineModel, true) == 0)
                            flag = true;
                        else if (string.Compare(machineSelectedValue.ZhName, setupedMachine.ZhName, true) == 0)
                        {
                            if (string.Compare(machineSelectedValue.EnName, setupedMachine.EnName, true) == 0)
                                flag = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Write("CheckMachine Error." + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return flag;
        }

        private void CreateExeShortcut(
          string shortcutDir,
          string exeName,
          string exePath = null,
          bool isModifyIco = true)
        {
            if (!Directory.Exists(shortcutDir))
                PathLib.CreateDirectory(shortcutDir);
            if (string.IsNullOrWhiteSpace(exePath))
                exePath = Path.Combine(this.Config.Path, "Bin", exeName + ".exe");
            string str = Path.Combine(shortcutDir, exeName + ".lnk");
            if (File.Exists(str))
                File.Delete(str);
            WshShell instance = (WshShell)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
            //    if (Installer.\u003C\u003Eo__129.\u003C\u003Ep__0 == null)
            //{

            //        Installer.\u003C\u003Eo__129.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, IWshShortcut>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IWshShortcut), typeof(Installer)));
            //    }

            //    IWshShortcut wshShortcut = Installer.\u003C\u003Eo__129.\u003C\u003Ep__0.Target((CallSite)Installer.\u003C\u003Eo__129.\u003C\u003Ep__0, instance.CreateShortcut(str));
            if (callSite == null)
            {
                callSite = CallSite<Func<CallSite, object, IWshShortcut>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IWshShortcut), typeof(Installer)));
            }

            IWshShortcut wshShortcut = callSite.Target((CallSite)callSite, instance.CreateShortcut(str));
            wshShortcut.TargetPath = exePath;
            wshShortcut.WindowStyle = 1;
            string path = string.Empty;
            if (exePath.Contains(this.Config.ExeName))
                path = exePath.Replace(this.Config.ExeName, "ncstudio.ico");
            if (File.Exists(path) & isModifyIco)
                wshShortcut.IconLocation = string.Format("{0}", (object)path);
            wshShortcut.Arguments = "";

            wshShortcut.Save();
        }

        private void CreateNcStudioShortCut()
        {
            string path2_1 = this.Config.AppName + ".lnk";
            string folderPath1 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
            string str1 = Path.Combine(folderPath1, path2_1);
            if (File.Exists(str1))
                File.Delete(str1);
            string path2_2 = path2_1;
            string path = Path.Combine(folderPath2, path2_2);
            if (File.Exists(path))
                File.Delete(path);
            this.CreateExeShortcut(folderPath1, this.Config.AppName);
            string str2 = Path.Combine(Env.Instance.StartMenuDir, path2_1);
            if (!Directory.Exists(Env.Instance.StartMenuDir))
                PathLib.CreateDirectory(Env.Instance.StartMenuDir);
            else if (File.Exists(str2))
                File.Delete(str2);
            PathLib.CopyFile(str1, str2);
        }

        internal void CreateShortCut()
        {
            if (!InstallContext.Instance.IsCreateShotCut)
                return;
            LogWriter.UpdatePrompt(Resources.Msg_CreateShortcut);
            if (Directory.Exists(Env.Instance.StartMenuNcStudioDir))
                Directory.Delete(Env.Instance.StartMenuNcStudioDir, true);
            if (Directory.Exists(Env.Instance.StartMenuDir))
                Directory.Delete(Env.Instance.StartMenuDir, true);
            this.CreateNcStudioShortCut();
            this.CreateExeShortcut(Env.Instance.StartMenuDir, "NcTune");
            if (this.Config.IsInstallSunloginClient)
                this.CreateExeShortcut(Env.Instance.StartMenuDir, Resources.Msg_RemoteSupport, Env.Instance.FirstRunDir + "\\SunloginClient\\SunloginClient.exe", false);
            Packup.Library.NativeMethods.SHChangeNotify(134217728U, 0U, IntPtr.Zero, IntPtr.Zero);
        }

        private void StartMachineStart()
        {
            LogWriter.UpdatePrompt(Resources.Msg_StartupSelf);
            LogWriter.UpdatePrompt(Resources.Msg_StartupSelfWithDesktop);
            string[] files = Directory.GetFiles(this.Config.Path, this.Config.ExeName, SearchOption.AllDirectories);
            if (files.Length == 0)
                return;
            string str = files[0];
            Installer.WriteRegistryKey(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "NcStudio", str, true);
        }

        private void StartMachineStartNoDesktop()
        {
            LogWriter.UpdatePrompt(Resources.Msg_StartupSelf);
            LogWriter.UpdatePrompt(Resources.Msg_StartupSelfWithoutDesktop);
            string[] files = Directory.GetFiles(this.Config.Path, this.Config.ExeName, SearchOption.AllDirectories);
            if (files.Length == 0)
                return;
            string str = files[0];
            Installer.WriteRegistryKey(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "Shell", str, true);
        }

        internal void ClearStartMachineStart()
        {
            if (Directory.GetFiles(this.Config.Path, this.Config.ExeName, SearchOption.AllDirectories).Length == 0)
                return;
            Installer.WriteRegistryKey(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "NcStudio", "");
        }

        internal void ClearStartMachineStartNoDesktop()
        {
            if (Directory.GetFiles(this.Config.Path, this.Config.ExeName, SearchOption.AllDirectories).Length == 0)
                return;
            Installer.WriteRegistryKey(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "Shell", "");
        }

        internal void FirstRunAutoStart()
        {
            string path = Path.Combine(Env.Instance.FirstRunDir, "firstrun.exe");
            if (!File.Exists(path))
                return;
            if (InstallContext.Instance.IsAutoRestartNoDesktop)
            {
                string[] files = Directory.GetFiles(this.Config.Path, this.Config.ExeName, SearchOption.AllDirectories);
                if (files.Length == 0)
                    return;
                string str = files[0];
                Installer.WriteRegistryKey(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "FirstRun", string.Empty);
                Installer.WriteRegistryKey(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "Shell", str + "," + path + " -p \"" + this.Config.AppName + "\"", true);
            }
            else
                Installer.WriteRegistryKey(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "FirstRun", path + " -p \"" + this.Config.AppName + "\"", true);
            this.StartFirstRun();
        }

        internal void StartFirstRun()
        {
            if (InstallContext.Instance.IsFirstRunStarupFailed)
                return;
            string path = Path.Combine(Env.Instance.FirstRunDir, "firstrun.exe");
            if (!File.Exists(path))
                return;
            try
            {
                if (this.firstrunMutex != null)
                {
                    this.firstrunMutex.Dispose();
                    this.firstrunMutex = (Mutex)null;
                }
                string str = "-p \"" + this.Config.AppName + "\"";
                Process.Start(new ProcessStartInfo()
                {
                    FileName = path,
                    Arguments = str,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    UseShellExecute = false
                });
            }
            catch (Exception ex)
            {
                InstallContext.Instance.IsFirstRunStarupFailed = true;
                if (this.Config.InstallType != InstallType.PC)
                    return;
                InstallContext.Instance.IsFirstRunStarupFailed = true;
                InstallContext.Instance.ThirdPartyFailedMsg = path + " " + ex.Message;
            }
        }

        private static void WriteRegistryKey(
          RegistryKey node,
          string address,
          string key,
          string value,
          bool isFlush = false)
        {
            RegistryKey registryKey = node;
            RegistryKey subKey = registryKey.CreateSubKey(address);
            subKey.SetValue(key, (object)value);
            if (isFlush)
            {
                subKey.Flush();
                registryKey.Flush();
            }
            subKey.Close();
            registryKey.Close();
        }

        internal bool CanClose() => !this.IsBeginInstall || this.IsFinished;

        internal void InstallSunloginClient()
        {
            if (!this.Config.IsInstallSunloginClient)
                return;
            LogWriter.UpdatePrompt(Resources.Msg_InstallRemote);
            string str1 = Path.Combine(this.Config.Path, "Bin", "NcAdapter");
            string str2 = Path.Combine(str1, "SunloginClient.exe");
            if (!File.Exists(str2))
                return;
            string argments = "--mod=install --cmd=silent --serviceonly=1 --path=\"" + Env.Instance.FirstRunDir + "\" --autorun=0";
            PathLib.CreateProcess(str2, str1, argments);
        }

        internal void RunBeforeSetup(string zipPath)
        {
            string str1 = Path.Combine(Directory.GetParent(this.Config.Path).FullName, "~BeforeSetup");
            try
            {
                LogWriter.Write("执行安装前批处理命令");
                LogWriter.UpdatePrompt(Resources.Msg_SetupWillStart);
                string workDir = this.RunBeforeGetWorkDir(zipPath, str1);
                if (!File.Exists(Path.Combine(workDir, this.Config.BeforeSetup)))
                    return;
                string str2 = "\"" + this.Config.Path + "\" \"" + this.Config.OEMConfigDir + "\" \"" + this.Config.ActiveConfigDir + "\"" + " " + this.GetRunBatLastParam();
                Packup.Library.Log.Write("批处理参数：" + str2);
                this.InitBatFile(this.Config.dicBatFileParam, new List<string>()
        {
          this.Config.BeforeSetup + " " + str2
        }, workDir);
                this.RunShellBatFile("Shell.bat", workDir);
            }
            finally
            {
                try
                {
                    if (Directory.Exists(str1))
                        Directory.Delete(str1, true);
                }
                catch
                {
                    this.DeleteFileForUseredDirectory(str1);
                }
            }
        }

        public void DeleteFileForUseredDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;
            try
            {
                foreach (string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                LogWriter.UpdatePrompt(ex.Message);
                throw new Exception(string.Format(Resources.Err_FileUsed, (object)path));
            }
        }

        internal string RunBeforeGetWorkDir(string zipPath, string beforeSetupPath)
        {
            string workDir = string.Empty;
            using (FileStream baseInputStream = File.OpenRead(zipPath))
            {
                if (baseInputStream.Length == 0L)
                    throw new SpaceNotEnoughException(string.Format("{0}: {1} byte", (object)zipPath, (object)baseInputStream.Length))
                    {
                        File = zipPath,
                        NeedBytes = baseInputStream.Length
                    };
                using (ZipInputStream zipInputStream = new ZipInputStream((Stream)baseInputStream))
                {
                    ZipEntry nextEntry;
                    while ((nextEntry = zipInputStream.GetNextEntry()) != null)
                    {
                        if (nextEntry.IsFile)
                        {
                            string empty = string.Empty;
                            string name;
                            try
                            {
                                name = new DirectoryInfo(Path.GetDirectoryName(nextEntry.Name)).Name;
                            }
                            catch
                            {
                                continue;
                            }
                            if (nextEntry.Name.StartsWith("Bin", StringComparison.OrdinalIgnoreCase) && string.Compare(name, "PackUp", true) == 0)
                            {
                                string str = Path.Combine(beforeSetupPath, nextEntry.Name);
                                string directoryName = Path.GetDirectoryName(str);
                                if (!Directory.Exists(directoryName))
                                    PathLib.CreateDirectory(directoryName);
                                byte[] numArray = new byte[zipInputStream.Length];
                                zipInputStream.Read(numArray, 0, numArray.Length);
                                if (PathLib.GetHardDiskSpace(Path.GetPathRoot(str).ToUpper()) < (long)numArray.Length)
                                    throw new InvalidDataException(string.Format("{0}: {1} byte", (object)str, (object)numArray.Length));
                                if (Path.GetFileName(nextEntry.Name).IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                                    throw new FileNameInvalidException("file name invalid")
                                    {
                                        FileName = nextEntry.Name
                                    };
                                Installer.WriteFile(str, numArray);
                                if (string.Compare(Path.GetFileName(str), this.Config.BeforeSetup, true) == 0)
                                    workDir = Path.GetDirectoryName(str);
                            }
                        }
                    }
                }
            }
            return workDir;
        }

        private void RunShellBatFile(string subPath, string workPath)
        {
            string str = Path.Combine(workPath, subPath);
            if (!File.Exists(str))
                return;
            PathLib.CreateProcess(str, workPath, string.Empty, true);
        }

        internal string GetRunBatLastParam()
        {
            string selectedLanguage = Env.Instance.Config.SelectedLanguage;
            bool flag = false;
            if (InstallContext.Instance.IsReservedManufacturer)
                flag = InstallContext.Instance.IsReservedManufacturer;
            else if (InstallContext.Instance.IsReservedUser)
                flag = InstallContext.Instance.IsReservedUser;
            bool isCreateShortcut = this.Config.IsCreateShortcut;
            if (InstallContext.Instance.IsAutoStart)
            {
                if (InstallContext.Instance.IsAutoRestart)
                    this.Config.AutoStartType = AutoStartType.Complate;
                if (InstallContext.Instance.IsAutoRestartNoDesktop)
                    this.Config.AutoStartType = AutoStartType.Exclusive;
            }
            else
                this.Config.AutoStartType = AutoStartType.None;
            AutoStartType autoStartType = this.Config.AutoStartType;
            string empty = string.Empty;
            string str1 = !InstallContext.Instance.IsMultipleConfig ? new DirectoryInfo(this.Config.ActiveConfigDir).Name : InstallContext.Instance.SelectedValue.DirectoryName;
            string str2;
            try
            {
                str2 = !InstallContext.Instance.IsMultipleMachine ? new DirectoryInfo(this.Config.OEMConfigDir).Name : InstallContext.Instance.MachineSelectedValue?.MachineModel;
            }
            catch
            {
                str2 = string.Empty;
            }
            TransformType transformType = TransformType.User;
            if (InstallContext.Instance.IsReservedUser && InstallContext.Instance.IsReservedManufacturer)
                transformType = TransformType.Local;
            else if (!InstallContext.Instance.IsReservedUser && !InstallContext.Instance.IsReservedManufacturer)
                transformType = TransformType.Machine;
            else if (InstallContext.Instance.IsReservedManufacturer && !InstallContext.Instance.IsReservedUser)
                transformType = TransformType.Manufacturer;
            else if (!InstallContext.Instance.IsReservedManufacturer && InstallContext.Instance.IsReservedUser)
                transformType = TransformType.User;
            return selectedLanguage + " " + flag.ToString() + " " + autoStartType.ToString() + " " + isCreateShortcut.ToString() + " " + str1 + " " + transformType.ToString() + " " + str2;
        }

        internal void RunAfterSetup()
        {
            LogWriter.Write("执行安装后批处理命令");
            string[] files = Directory.GetFiles(this.Config.Path, "PackUp.exe", SearchOption.AllDirectories);
            if (files.Length == 0)
                return;
            string workPath = Path.Combine(Directory.GetParent(files[0]).FullName, "PackUp");
            string str = "\"" + this.Config.Path + "\"" + " " + this.GetRunBatLastParam();
            Packup.Library.Log.Write("批处理参数：" + str);
            this.InitBatFile(this.Config.dicBatFileParam, new List<string>()
      {
        this.Config.AfterSetup + " " + str
      }, workPath);
            this.RunShellBatFile("Shell.bat", workPath);
        }

        internal void StartNcStudio()
        {
            LogWriter.Write("启动NcStudio");
            string path = Path.Combine(this.Config.Path, "Bin\\" + this.Config.AppName + ".exe");
            string fullName = Directory.GetParent(path).FullName;
            if (!File.Exists(path) || this.CheckExeProcess())
                return;
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.WorkingDirectory = fullName;
            process.Start();
            process.WaitForExit(1000);
            process.Close();
        }

        public void DisableCloseButton()
        {
            Packup.Library.NativeMethods.EnableMenuItem(Packup.Library.NativeMethods.GetSystemMenu(new WindowInteropHelper(Application.Current.MainWindow).Handle, false), Packup.Library.NativeMethods.MenuOperator.SC_CLOSE, Packup.Library.NativeMethods.MenuOperator.MF_GRAYED);
            this.CanCloseWindow = false;
        }

        public void EnableCloseButton()
        {
            Packup.Library.NativeMethods.EnableMenuItem(Packup.Library.NativeMethods.GetSystemMenu(new WindowInteropHelper(Application.Current.MainWindow).Handle, false), Packup.Library.NativeMethods.MenuOperator.SC_CLOSE, Packup.Library.NativeMethods.MenuOperator.MF_BYCOMMAND);
            this.CanCloseWindow = true;
        }

        internal string SetException(Exception ex)
        {
            string empty = string.Empty;
            ConfigInfo config = Env.Instance.Config;
            string str1 = Directory.GetDirectoryRoot(config.Path).Substring(0, 1);
            StringBuilder stringBuilder = new StringBuilder();
            switch (ex)
            {
                case InvalidDataException _:
                    stringBuilder.AppendLine(string.Format(Resources.Err_MemoryNotEnoughOrPackageDamaged, (object)str1));
                    goto case null;
                case ZipException _:
                    stringBuilder.AppendLine(string.Format(Resources.Err_PackageDamaged, (object)str1));
                    goto case null;
                case SharpZipBaseException _:
                    stringBuilder.AppendLine(string.Format(Resources.Err_PackageDamaged, (object)str1));
                    goto case null;
                case UnauthorizedAccessException _:
                    stringBuilder.AppendLine(string.Format(Resources.Err_UnauthorizedAccessException, (object)config.Path));
                    goto case null;
                case BatExitException batExitException:
                    stringBuilder.AppendLine(string.Format(Resources.Msg_ProcessExitCode, (object)batExitException.ProcessName, (object)batExitException.ExitCode));
                    goto case null;
                case ProcessException processException:
                    stringBuilder.AppendLine(string.Format(Resources.Msg_ProcessError, (object)processException.ProcessName, (object)processException.Message));
                    goto case null;
                case SpaceNotEnoughException notEnoughException:
                    string str2 = string.Empty;
                    if (!string.IsNullOrEmpty(notEnoughException.File))
                        str2 = Path.GetPathRoot(notEnoughException.File).Substring(0, 1).ToUpper();
                    if (!string.IsNullOrEmpty(str2))
                    {
                        stringBuilder.AppendLine(string.Format(Resources.Err_MemoryNotEnough, (object)str2));
                        goto case null;
                    }
                    else
                    {
                        stringBuilder.AppendLine(notEnoughException.Message);
                        goto case null;
                    }
                case IOException _:
                    string str3 = config.Path;
                    if (!string.IsNullOrEmpty(config.CurrentWorkPath))
                        str3 = config.CurrentWorkPath;
                    stringBuilder.AppendLine(string.Format(Resources.Err_DirectoryOpened, (object)str3));
                    goto case null;
                case PathInvalidException invalidException1:
                    stringBuilder.AppendLine(Resources.Err_PathError + invalidException1.Path);
                    goto case null;
                case FileNameInvalidException invalidException2:
                    stringBuilder.AppendLine(Resources.Err_FileNameError + invalidException2.FileName);
                    goto case null;
                case null:
                    string str4 = stringBuilder.ToString();
                    this.ErrorMessage = stringBuilder.ToString();
                    StringBuilder sb = new StringBuilder();
                    sb.Append((object)stringBuilder);
                    sb.AppendLine("-----------------------Exception-----------------------");
                    sb.AppendLine("SetupPath:<" + config.Path + ">");
                    sb.AppendLine("CurrentWorkPath:<" + config.CurrentWorkPath + ">");
                    sb.AppendLine("OEMConfigDir:<" + config.OEMConfigDir + ">");
                    Packup.Library.Log.WriteExceptionString(ex, sb);
                    return str4;
                default:
                    if (!string.IsNullOrEmpty(ex.Message))
                    {
                        stringBuilder.Append(ex.Message);
                        goto case null;
                    }
                    else
                    {
                        stringBuilder.Append(string.Format(Resources.Err_UnknowError_Retore));
                        goto case null;
                    }
            }
        }

        internal delegate void RunInstallCheckWorkStatusDelegate();
    }
}
