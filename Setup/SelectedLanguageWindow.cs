// Decompiled with JetBrains decompiler
// Type: Setup.SelectedLanguageWindow
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Setup
{
    public class SelectedLanguageWindow : Window, IComponentConnector
    {
        private ConfigInfo config = Env.Instance.Config;
        internal Image image;
        internal TextBlock tbPrompt;
        internal ComboBox cbxLanguage;
        internal Button btnOK;
        internal Button btnCancel;
        private bool _contentLoaded;

        public SelectedLanguageWindow()
        {
            this.Icon = Application.Current.MainWindow.Icon;
            this.InitializeComponent();
            this.Initialize();
            this.KeyDown += new KeyEventHandler(this.SelectedLanguage_KeyDown);
            this.cbxLanguage.SelectionChanged += new SelectionChangedEventHandler(this.CbxLanguage_SelectionChanged);
            this.cbxLanguage.Loaded += (RoutedEventHandler)((o, e) =>
           {
               int index = this.config.LanguageList.FindIndex((Predicate<LanguageInfo>)(langugae => langugae.Culture == this.config.DefaultLanguage));
               this.cbxLanguage.SelectedIndex = index == -1 ? 0 : index;
           });
        }

        private void CbxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.config.LanguageList[this.cbxLanguage.SelectedIndex].Name == "zh")
            {
                this.Title = "安装";
                this.tbPrompt.Text = "请选择安装时使用语言: ";
                this.btnOK.Content = (object)"确定";
                this.btnCancel.Content = (object)"取消";
            }
            else
            {
                this.Title = "Installer";
                this.tbPrompt.Text = "Please select a language.";
                this.btnOK.Content = (object)"OK";
                this.btnCancel.Content = (object)"Cancel";
            }
        }

        protected override void OnSourceInitialized(EventArgs e) => IconHelper.RemoveIcon((Window)this);

        private void Initialize()
        {
            this.cbxLanguage.ItemsSource = (IEnumerable)this.config.LanguageList;
            this.cbxLanguage.DisplayMemberPath = "NativeName";
            BitmapSource bitmapSource = App.GetBitmapSource("ncstudio", true);
            if (bitmapSource == null)
                return;
            this.image.Source = (ImageSource)bitmapSource;
        }

        private void SelectedLanguage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Left)
            {
                e.Handled = true;
                if (this.cbxLanguage.SelectedIndex <= 0)
                    return;
                --this.cbxLanguage.SelectedIndex;
            }
            else
            {
                if (e.Key != Key.Down && e.Key != Key.Right)
                    return;
                e.Handled = true;
                if (this.cbxLanguage.SelectedIndex >= this.cbxLanguage.Items.Count - 1)
                    return;
                ++this.cbxLanguage.SelectedIndex;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            App.languageInfo = this.config.LanguageList[this.cbxLanguage.SelectedIndex];
            this.config.SelectedLanguage = App.languageInfo.Culture;
            if (App.languageInfo.Name != "zh")
                Env.Instance.SetLocal("en-US");
            else
                Env.Instance.SetLocal("zh-CN");
            this.DialogResult = new bool?(true);
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/selectedlanguagewindow.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.image = (Image)target;
                    break;
                case 2:
                    this.tbPrompt = (TextBlock)target;
                    break;
                case 3:
                    this.cbxLanguage = (ComboBox)target;
                    break;
                case 4:
                    this.btnOK = (Button)target;
                    this.btnOK.Click += new RoutedEventHandler(this.OK_Click);
                    break;
                case 5:
                    this.btnCancel = (Button)target;
                    this.btnCancel.Click += new RoutedEventHandler(this.Cancel_Click);
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
