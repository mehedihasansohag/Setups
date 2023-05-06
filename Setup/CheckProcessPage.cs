// Decompiled with JetBrains decompiler
// Type: Setup.CheckProcessPage
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Setup
{
    public  class CheckProcessPage : UserControl, IComponentConnector
    {
        private Timer timer;
        internal ListView processList;
        private bool _contentLoaded;

        public CheckProcessPage()
        {
            //this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.CheckProcessPage_Loaded);
            this.timer = new Timer()
            {
                AutoReset = true,
                Interval = 500.0
            };
            this.timer.Elapsed += (ElapsedEventHandler)((oo, ee) => Application.Current.Dispatcher.Invoke(new Action(this.CheckProcess)));
        }

        private void CheckProcessPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(this.CheckProcessPage_Loaded);
            this.Loaded += (RoutedEventHandler)((o, args) =>
           {
               this.CheckProcess();
               if (this.processList.Items.Count <= 0 || this.timer == null)
                   return;
               this.timer.Start();
           });
            this.Unloaded += (RoutedEventHandler)((o, args) =>
           {
               if (this.timer == null)
                   return;
               this.timer.Stop();
           });
        }

        internal void CheckProcess()
        {
          //  this.processList.Items.Clear();
            string lower1 = Directory.GetParent(Env.Instance.Config.Path).FullName.ToLower();
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    string lower2 = process.MainModule.FileName.ToLower();
                    string fileName = process.ProcessName;
                    string str = lower1;
                    if (lower2.Contains(str))
                    {
                        if (Env.Instance.Config.BackgroundProcesses.FindIndex((Predicate<string>)(o => o.ToLower().Equals(fileName.ToLower()))) == -1)
                            this.processList.Items.Add((object)new CheckProcessPage.ProcessInfo(process));
                    }
                }
                catch
                {
                   
                }
            }
            if (this.processList.Items.Count > 0)
            {
                Wizard.Instance.CurrentPage.CanSelectNextPage = new bool?(false);
            }
            else
            {
                if (this.timer == null)
                    return;
                this.timer.Stop();
                this.timer.Dispose();
                this.timer = (Timer)null;
                Wizard.Instance.CurrentPage.CanSelectNextPage = new bool?(true);
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
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/checkprocesspage.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
           if (connectionId == 1)
               this.processList = (ListView)target;
           else
                this._contentLoaded = true;
        }

        public class ProcessInfo
        {
            public ProcessInfo(Process process)
            {
                this.Process = process;
                this.ProcessName = process.ProcessName;
            }

            public Process Process { get; set; }

            public string ProcessName { get; set; }
        }
    }
}
