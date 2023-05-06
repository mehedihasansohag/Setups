// Decompiled with JetBrains decompiler
// Type: Setup.MachineMatchWarning
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Setup
{
    public class MachineMatchWarning : Window, IComponentConnector
    {
        private ConfigInfo config = Env.Instance.Config;
        internal Button btnYes;
        internal Button btnNo;
        internal TextBlock tbPrompt1;
        internal TextBlock local;
        internal TextBlock target;
        internal TextBlock massage;
        private bool _contentLoaded;

        public MachineMatchWarning()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.MachineMatchWarning_Loaded);
        }

        private void MachineMatchWarning_Loaded(object sender, RoutedEventArgs e)
        {
            this.local.Text = this.config.LocalMachineModel;
            this.target.Text = InstallContext.Instance.MachineSelectedValue.MachineModel;
            if (string.IsNullOrWhiteSpace(this.local.Text))
            {
                this.local.Text = Setup.Properties.Resources.Msg_Unrecognized;
                this.massage.Text = Setup.Properties.Resources.War_NotFound;
            }
            else
                this.massage.Text = Setup.Properties.Resources.War_MisMatch;
            this.btnNo.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = new bool?(true);
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = new bool?(false);
            this.Close();
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/machinematchwarning.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.btnYes = (Button)target;
                    this.btnYes.Click += new RoutedEventHandler(this.Button_Click);
                    break;
                case 2:
                    this.btnNo = (Button)target;
                    this.btnNo.Click += new RoutedEventHandler(this.Button_Click_1);
                    break;
                case 3:
                    this.tbPrompt1 = (TextBlock)target;
                    break;
                case 4:
                    this.local = (TextBlock)target;
                    break;
                case 5:
                    this.target = (TextBlock)target;
                    break;
                case 6:
                    this.massage = (TextBlock)target;
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
