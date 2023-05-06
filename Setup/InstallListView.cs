// Decompiled with JetBrains decompiler
// Type: Setup.InstallListView
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;
using System.Windows;
using System.Windows.Controls;

namespace Setup
{
    internal class InstallListView : ListView
    {
        public event RoutedEventHandler OKEvent
        {
            add => this.AddHandler(MainWindow_NK300.OKEvent, (Delegate)value);
            remove => this.RemoveHandler(MainWindow_NK300.OKEvent, (Delegate)value);
        }

        public event RoutedEventHandler CancelEvent
        {
            add => this.AddHandler(MainWindow_NK300.CancelEvent, (Delegate)value);
            remove => this.RemoveHandler(MainWindow_NK300.CancelEvent, (Delegate)value);
        }

        public InstallListView()
        {
            this.OKEvent += new RoutedEventHandler(this.OKEventHandler);
            this.CancelEvent += new RoutedEventHandler(this.CancelEventHandler);
        }

        private void OKEventHandler(object sender, RoutedEventArgs e)
        {
            if (!Installer.Instance.IsSucceed)
                return;
            App.WaitClickOK.Set();
        }

        private void CancelEventHandler(object sender, RoutedEventArgs e)
        {
            if (!Installer.Instance.IsSucceed)
                return;
            Application.Current.MainWindow.Close();
        }
    }
}
