// Decompiled with JetBrains decompiler
// Type: Setup.SelectedLanguageWindowNK300
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
    public class SelectedLanguageWindowNK300 : Window, IComponentConnector
    {
        private ConfigInfo config = Env.Instance.Config;
        private Control[] controlList = new Control[3];
        internal Image image;
        internal TextBlock tbPrompt;
        internal LanguageComboBox languageComboBox;
        internal TextBox OperateIntroduce;
        internal Button btnOK;
        internal Button btnCancel;
        private bool _contentLoaded;

        public int SelectedControlIndex { get; set; }

        public SelectedLanguageWindowNK300()
        {
            this.Icon = Application.Current.MainWindow.Icon;
            this.InitializeComponent();
            this.Initialize();
            this.languageComboBox.SelectionChanged += new SelectionChangedEventHandler(this.SelectionChangedHandler);
            this.languageComboBox.GotKeyboardFocus += (KeyboardFocusChangedEventHandler)((sender, e) => this.OperateIntroduce.Visibility = Visibility.Visible);
            this.languageComboBox.LostKeyboardFocus += (KeyboardFocusChangedEventHandler)((sender, e) =>
           {
               if (this.languageComboBox.IsDropDownOpen)
                   return;
               this.OperateIntroduce.Visibility = Visibility.Hidden;
           });
            this.languageComboBox.Loaded += (RoutedEventHandler)((o, e) =>
           {
               Keyboard.Focus((IInputElement)this.controlList[this.SelectedControlIndex]);
               int index = this.config.LanguageList.FindIndex((Predicate<LanguageInfo>)(langugae => langugae.Culture == this.config.DefaultLanguage));
               this.languageComboBox.SelectedIndex = index == -1 ? 0 : index;
           });
            this.SelectedControlIndex = 0;
            this.controlList[0] = (Control)this.languageComboBox;
            this.controlList[1] = (Control)this.btnOK;
            this.controlList[2] = (Control)this.btnCancel;
        }

        private void SelectionChangedHandler(object sender, SelectionChangedEventArgs e)
        {
            if (this.config.LanguageList[this.languageComboBox.SelectedIndex].Name == "zh")
            {
                this.Title = "安装";
                this.tbPrompt.Text = "请选择安装时使用的语言: ";
                this.OperateIntroduce.Text = "按 <Select> 切换语言。\r\n按 <空格> 弹出下拉框。";
                this.btnOK.Content = (object)"确定(F7)";
                this.btnCancel.Content = (object)"取消(F8)";
            }
            else
            {
                this.Title = "Installer";
                this.tbPrompt.Text = "Please select a language.";
                this.OperateIntroduce.Text = "Press <Select> to switch language.\r\nPress <Space> to show list.";
                this.btnOK.Content = (object)"OK(F7)";
                this.btnCancel.Content = (object)"Cancel(F8)";
            }
        }

        protected override void OnSourceInitialized(EventArgs e) => IconHelper.RemoveIcon((Window)this);

        private void Initialize()
        {
            this.languageComboBox.ItemsSource = (IEnumerable)this.config.LanguageList;
            this.languageComboBox.DisplayMemberPath = "NativeName";
            BitmapSource bitmapSource = App.GetBitmapSource("ncstudio", true);
            if (bitmapSource == null)
                return;
            this.image.Source = (ImageSource)bitmapSource;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (this.languageComboBox.IsDropDownOpen)
            {
                switch (e.Key)
                {
                    case Key.Return:
                    case Key.Escape:
                    case Key.Space:
                    case Key.Up:
                    case Key.Down:
                    case Key.F7:
                    case Key.F8:
                    case Key.F12:
                        e.Handled = false;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Tab:
                    case Key.Right:
                    case Key.Down:
                        this.SelectedControlIndex = (this.SelectedControlIndex + 1) % this.controlList.Length;
                        Keyboard.Focus((IInputElement)this.controlList[this.SelectedControlIndex]);
                        e.Handled = true;
                        break;
                    case Key.Return:
                    case Key.F7:
                        this.OKButtonClicked((object)this, (RoutedEventArgs)e);
                        e.Handled = true;
                        break;
                    case Key.Escape:
                    case Key.F8:
                        this.CancelButtonClicked((object)this, (RoutedEventArgs)e);
                        e.Handled = true;
                        break;
                    case Key.Space:
                    case Key.F12:
                        e.Handled = false;
                        break;
                    case Key.Left:
                    case Key.Up:
                        this.SelectedControlIndex = (this.SelectedControlIndex - 1 + this.controlList.Length) % this.controlList.Length;
                        Keyboard.Focus((IInputElement)this.controlList[this.SelectedControlIndex]);
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
            }
        }

        private void OKButtonClicked(object sender, RoutedEventArgs e)
        {
            LanguageInfo language = this.config.LanguageList[this.languageComboBox.SelectedIndex];
            this.config.SelectedLanguage = language.Culture;
            if (language.Name != "zh")
                Env.Instance.SetLocal("en-US");
            else
                Env.Instance.SetLocal("zh-CN");
            this.DialogResult = new bool?(true);
            this.Close();
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
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
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/selectedlanguagewindownk300.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, (object)this, handler);

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
                    this.languageComboBox = (LanguageComboBox)target;
                    break;
                case 4:
                    this.OperateIntroduce = (TextBox)target;
                    break;
                case 5:
                    this.btnOK = (Button)target;
                    this.btnOK.Click += new RoutedEventHandler(this.OKButtonClicked);
                    break;
                case 6:
                    this.btnCancel = (Button)target;
                    this.btnCancel.Click += new RoutedEventHandler(this.CancelButtonClicked);
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
