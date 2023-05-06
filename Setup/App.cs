// Decompiled with JetBrains decompiler
// Type: Setup.App
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Setup
{
    public  class App : Application, IDisposable
    {
        internal static AutoResetEvent WaitExtract = new AutoResetEvent(false);
        internal static AutoResetEvent ContinueSetup = new AutoResetEvent(false);
        internal static AutoResetEvent WaitClickOK = new AutoResetEvent(false);
        internal static CancellationTokenSource AsynCancellationToken = new CancellationTokenSource();
        internal static readonly BackgroundWorker installWorker = new BackgroundWorker()
        {
            WorkerSupportsCancellation = true,
            WorkerReportsProgress = true
        };
        internal static readonly BackgroundWorker restoreWorker = new BackgroundWorker()
        {
            WorkerReportsProgress = true
        };
        internal static readonly string TempPath = Path.Combine(Path.GetTempPath(), "Weihong", "Setup");
        internal static readonly string SetupPath = Path.Combine(App.TempPath, string.Format("tmp-{0}", (object)DateTime.Now.ToString("yyyyMMddHHmmss")));
        private Mutex setupMutex;
        private static bool hasResource = false;
        private static ResourceManager resourceManager = (ResourceManager)null;
        private ConfigInfo config;
        internal static LanguageInfo languageInfo = (LanguageInfo)null;
        private bool _contentLoaded;

        private Installer installer => Installer.Instance;

        static App() => AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(App.CurrentDomain_AssemblyResolve);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                App.RegisterExceptionBoxForUnhandledExceptions();
                bool createdNew;
                this.setupMutex = new Mutex(true, "Setup.exe", out createdNew);
                if (!createdNew)
                {
                    int num = (int)MessageBox.Show(Setup.Properties.Resources.Err_HasStartedOne, Setup.Properties.Resources.Msg_SetupWarning);
                    this.Shutdown(-1);
                }
                else
                {
                    this.Extract();
                    App.WaitExtract.WaitOne();
                    Env.Instance.Init();
                    this.config = Env.Instance.Config;
                    InstallContext.Instance.Init();
                    App.CheckResource();
                    if (this.config.LanguageList.Count <= 0)
                    {
                        this.config.SelectedLanguage = "zh-CN";
                        Env.Instance.SetLocal("zh-CN");
                    }
                    else if (!this.SelectedLanguage())
                    {
                        this.Shutdown(-1);
                        return;
                    }
                    Installer.Instance.Init();
                    if (InstallContext.Instance.ConfigList.Count <= 0)
                        throw new Exception("当前安装包错误，配置列表为空。");
                    this.ShowMainWindow();
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("OnStartup Exception.");
                Packup.Library.Log.WriteExceptionString(ex, sb);
                int num = (int)MessageBox.Show(Setup.Properties.Resources.Err_SetupInitFailed + ex.Message, Setup.Properties.Resources.Msg_SetupWarning, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                this.Shutdown(-1);
            }
        }

        private void ShowMainWindow()
        {
            if (this.config.InstallType == InstallType.AIO)
                this.MainWindow = (Window)new MainWindow_NK300();
            else
                this.MainWindow = (Window)new Setup.MainWindow();
            if (Application.Current.MainWindow != null)
            {
                BitmapSource bitmapSource = App.GetBitmapSource("ncstudio", true);
                if (bitmapSource != null)
                    Application.Current.MainWindow.Icon = (ImageSource)bitmapSource;
            }
            this.MainWindow.Show();
        }

        private static bool CheckDotnetEnvironment()
        {
            bool flag = true;
            if (!DotnetDetection.IsDotnet45Installed())
                flag = false;
            return flag;
        }

        private static void CheckResource()
        {
            App.hasResource = false;
            string path = Path.Combine(App.SetupPath, "Setup.Image.resources");
            if (!File.Exists(path))
                return;
            App.resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), (Type)null);
            App.hasResource = true;
        }

        private bool CheckReleaseCRC32()
        {
            bool flag = true;
            long result;
            if (!string.IsNullOrEmpty(this.config.CRC32) && long.TryParse(this.config.CRC32, NumberStyles.HexNumber, (IFormatProvider)CultureInfo.InvariantCulture, out result))
            {
                using (FileStream fileStream = new FileStream(Assembly.GetExecutingAssembly().Location, FileMode.Open, FileAccess.Read))
                {
                    byte[] numArray1 = new byte[fileStream.Length];
                    fileStream.Read(numArray1, 0, (int)fileStream.Length);
                    byte num1 = 27;
                    string str = Encoding.ASCII.GetString(numArray1);
                    int startIndex1;
                    int num2;
                    for (startIndex1 = str.IndexOf("cab@", 0, StringComparison.Ordinal); (int)numArray1[startIndex1 - 1] != (int)num1; startIndex1 = str.IndexOf("cab@", num2 + 4, StringComparison.Ordinal))
                        num2 = startIndex1;
                    int num3;
                    int startIndex2;
                    for (num3 = str.IndexOf("@cab", startIndex1, StringComparison.Ordinal); (int)numArray1[num3 + "@cab".Length] != (int)num1; num3 = str.IndexOf("@cab", startIndex2, StringComparison.Ordinal))
                        startIndex2 = num3;
                    int num4 = startIndex1 + 4;
                    byte[] numArray2 = new byte[num3 - num4];
                    fileStream.Position = (long)num4;
                    fileStream.Read(numArray2, 0, numArray2.Length);
                    byte[] bytes = BitConverter.GetBytes(0);
                    numArray2[numArray2.Length - 2] = bytes[0];
                    numArray2[numArray2.Length - 1] = bytes[1];
                    if (CRC32.GetBytesCRC32(numArray2) != result)
                        flag = false;
                }
            }
            return flag;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (this.config != null && this.installer != null)
            {
                if (this.config.InstallType == InstallType.AIO)
                {
                    if (this.installer.IsSucceed && !InstallContext.Instance.IsDirverFailed)
                        this.installer.StartNcStudio();
                }
                else if (this.installer.IsSucceed && this.config.IsStartup && !InstallContext.Instance.IsDirverFailed)
                    this.installer.StartNcStudio();
            }
            Packup.Library.Log.CloseLogStream();
            base.OnExit(e);
        }

        internal static BitmapSource GetBitmapSource(string key, bool isIcon = false)
        {
            if (!App.hasResource || App.resourceManager == null)
                return (BitmapSource)null;
            if (isIcon)
            {
                Icon icon = (Icon)App.resourceManager.GetObject(key);
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(icon.Width, icon.Height));
            }
            Bitmap bitmap = (Bitmap)App.resourceManager.GetObject(key);
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));
        }

        private bool ShowMatchWarning()
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            bool? nullable = new MachineMatchWarning().ShowDialog();
            bool flag = true;
            if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
                return false;
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            return true;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Setup.Libs." + new AssemblyName(args.Name).Name + ".dll"))
            {
                byte[] numArray = new byte[(int)manifestResourceStream.Length];
                manifestResourceStream.Read(numArray, 0, numArray.Length);
                return Assembly.Load(numArray);
            }
        }

        private bool SelectedLanguage()
        {
            LogWriter.Write("选择安装语言");
            if (this.config.IsShowLangDialog)
                return this.ShowSelectedLanguageDialog();
            if (this.config.LanguageList.Count > 1)
            {
                App.languageInfo = this.config.LanguageList.Find((Predicate<LanguageInfo>)(language => language.Culture == this.config.DefaultLanguage));
                if (App.languageInfo == null)
                    App.languageInfo = this.config.LanguageList[0];
            }
            else
                App.languageInfo = this.config.LanguageList[0];
            this.config.SelectedLanguage = App.languageInfo.Culture;
            if (App.languageInfo.Name != "zh")
                Env.Instance.SetLocal("en-US");
            else
                Env.Instance.SetLocal("zh-CN");
            return true;
        }

        private bool ShowSelectedLanguageDialog()
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Window window;
            switch (this.config.InstallType)
            {
                case InstallType.PC:
                    window = (Window)new SelectedLanguageWindow();
                    break;
                case InstallType.AIO:
                    window = (Window)new SelectedLanguageWindowNK300();
                    break;
                default:
                    throw new Exception("错误的安装类型，InstallType 必须是 AIO 或者 PC");
            }
            bool? nullable = window.ShowDialog();
            bool flag = true;
            if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
                return false;
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            return true;
        }

        private void Extract()
        {
            using (AutoResetEvent clearAutoReset = new AutoResetEvent(false))
            {
                Task.Factory.StartNew((Action)(() =>
               {
                   clearAutoReset.Set();
                   this.ClearTemp();
               }));
                clearAutoReset.WaitOne();
            }
            Task.Factory.StartNew((Action)(() =>
           {
               try
               {
                   this.ExtractRelease();
               }
               finally
               {
                   App.WaitExtract.Set();
               }
           }), App.AsynCancellationToken.Token);
        }

        internal void ExtractRelease(string sourcePath = "")
        {
            if (!Directory.Exists(App.SetupPath))
                Directory.CreateDirectory(App.SetupPath);
            string path = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(sourcePath))
                path = sourcePath;
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            byte num1 = 27;
            int beginPos1 = -1;
            int beginPos2 = -1;
            int beginPos3 = -1;
            for (int index = 0; index < buffer.Length - 1 && !App.AsynCancellationToken.IsCancellationRequested; ++index)
            {
                int num2 = (int)buffer[index];
                if (num2 == (int)num1)
                {
                    if (buffer[index + 1] == (byte)99 && buffer[index + 2] == (byte)97 && buffer[index + 3] == (byte)98 && buffer[index + 4] == (byte)64)
                    {
                        beginPos2 = index + 5;
                        index += 4;
                    }
                    else if (buffer[index + 1] == (byte)114 && buffer[index + 2] == (byte)101 && buffer[index + 3] == (byte)115 && buffer[index + 4] == (byte)64)
                    {
                        beginPos3 = index + 5;
                        index += 4;
                    }
                    else if (buffer[index + 1] == (byte)99 && buffer[index + 2] == (byte)102 && buffer[index + 3] == (byte)103 && buffer[index + 4] == (byte)64)
                    {
                        beginPos1 = index + 5;
                        index += 4;
                    }
                }
                if (num2 == 64)
                {
                    if (buffer[index + 1] == (byte)99 && buffer[index + 2] == (byte)97 && buffer[index + 3] == (byte)98 && (int)buffer[index + 4] == (int)num1)
                    {
                        int endPos = index + 5;
                        int num3 = index + 4;
                        this.CreateFile(fileStream, "~Release.zip", beginPos2, endPos);
                        File.Move(Path.Combine(App.SetupPath, "~Release.zip"), Path.Combine(App.SetupPath, "Release.zip"));
                        break;
                    }
                    if (buffer[index + 1] == (byte)114 && buffer[index + 2] == (byte)101 && buffer[index + 3] == (byte)115 && (int)buffer[index + 4] == (int)num1)
                    {
                        int endPos = index;
                        index += 4;
                        this.CreateFile(fileStream, "Setup.Image.resources", beginPos3, endPos);
                    }
                    else if (buffer[index + 1] == (byte)99 && buffer[index + 2] == (byte)102 && buffer[index + 3] == (byte)103 && (int)buffer[index + 4] == (int)num1)
                    {
                        int endPos = index;
                        index += 4;
                        this.CreateFile(fileStream, "Setup.config", beginPos1, endPos);
                        App.WaitExtract.Set();
                    }
                }
            }
            fileStream.Close();
        }

        private void CreateFile(FileStream fileStream, string fileName, int beginPos, int endPos)
        {
            LogWriter.Write("正在提取 " + fileName);
            if (endPos <= beginPos)
                return;
            byte[] buffer = new byte[endPos - beginPos];
            fileStream.Position = (long)beginPos;
            fileStream.Read(buffer, 0, buffer.Length);
            using (FileStream fileStream1 = File.Create(Path.Combine(App.SetupPath, fileName)))
            {
                fileStream1.Write(buffer, 0, buffer.Length);
                fileStream1.Flush();
            }
        }

        internal void ClearTemp()
        {
            LogWriter.Write("清理临时目录下的过往安装文件");
            if (!Directory.Exists(App.TempPath))
                return;
            foreach (string directory in Directory.GetDirectories(App.TempPath, "*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    if (!(directory == App.SetupPath))
                        Directory.Delete(directory, true);
                }
                catch
                {
                }
            }
        }

        public void ExtractResFile(string resFileName, string outputFile)
        {
            BufferedStream bufferedStream = (BufferedStream)null;
            FileStream fileStream = (FileStream)null;
            try
            {
                bufferedStream = new BufferedStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(resFileName));
                fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                byte[] buffer = new byte[bufferedStream.Length];
                int count;
                if ((count = bufferedStream.Read(buffer, 0, buffer.Length)) > 0)
                    fileStream.Write(buffer, 0, count);
                fileStream.Flush();
            }
            finally
            {
                fileStream?.Close();
                bufferedStream?.Close();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || this.setupMutex == null)
                return;
            this.setupMutex.Dispose();
            this.setupMutex = (Mutex)null;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        public static void RegisterExceptionBoxForUnhandledExceptions()
        {
            LogWriter.Write("RegisterException handled.");
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App.Current_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App.CurrentDomain_UnhandledException);
            Dispatcher.CurrentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(App.Current_DispatcherUnhandledException);
        }

        private static void CurrentDomain_UnhandledException(
          object sender,
          UnhandledExceptionEventArgs e)
        {
            if (!(e?.ExceptionObject is Exception exceptionObject))
                return;
            App.ProcessException(exceptionObject);
        }

        private static void Current_DispatcherUnhandledException(
          object sender,
          DispatcherUnhandledExceptionEventArgs e)
        {
            App.ProcessException(e?.Exception);
        }

        private static void ProcessException(Exception ex)
        {
            if (ex == null)
                return;
            StringBuilder sb = new StringBuilder();
            sb.Append("Unhandled Exception.");
            Packup.Library.Log.WriteExceptionString(ex, sb);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/app.xaml", UriKind.Relative));
        }

        [STAThread]
        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            App app = new App();
            //app.InitializeComponent();
            app.Run();
        }
    }
}
