// Decompiled with JetBrains decompiler
// Type: Setup.TempDialog
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Setup
{
    public class TempDialog : Window, IComponentConnector
    {
        private TempDialogViewModel viewModel;
        internal Button YesButton;
        internal Button NoButton;
        private bool _contentLoaded;

        public TempDialogButton Result { get; set; }

        private TempDialog(
          Window owner,
          string messageText = null,
          string caption = null,
          TempDialogButton buttonType = TempDialogButton.Yes,
          TempDialogImage imageType = TempDialogImage.None)
        {
            this.Owner = owner;
            this.viewModel = new TempDialogViewModel(messageText, caption, buttonType, imageType);
            this.Result = TempDialogButton.No;
            this.DataContext = (object)this.viewModel;
            this.InitializeComponent();
            this.Loaded += (RoutedEventHandler)((sender, e) => FocusManager.SetFocusedElement((DependencyObject)this.YesButton, (IInputElement)this.YesButton));
        }

        public static TempDialogButton Show(
          Window owner,
          string messageText = null,
          string caption = null,
          TempDialogButton buttonType = TempDialogButton.Yes,
          TempDialogImage imageType = TempDialogImage.None)
        {
            TempDialog tempDialog = new TempDialog(owner, messageText, caption, buttonType, imageType);
            tempDialog.Icon = (ImageSource)App.GetBitmapSource("ncstudio", true);
            tempDialog.ShowDialog();
            return tempDialog.Result;
        }

        private void NoButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Result = TempDialogButton.No;
            this.Close();
        }

        private void YesButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Result = TempDialogButton.Yes;
            this.Close();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab || e.Key == Key.Space)
                return;
            if (e.Key == Key.Return || e.Key == Key.F7)
            {
                e.Handled = true;
                this.YesButtonClicked((object)this, (RoutedEventArgs)e);
            }
            else if (e.Key == Key.Escape || e.Key == Key.F8)
            {
                e.Handled = true;
                this.NoButtonClicked((object)this, (RoutedEventArgs)e);
            }
            else
                e.Handled = true;
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/Setup;component/ui/tempdialog.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            if (connectionId != 1)
            {
                if (connectionId == 2)
                {
                    this.NoButton = (Button)target;
                    this.NoButton.Click += new RoutedEventHandler(this.NoButtonClicked);
                }
                else
                    this._contentLoaded = true;
            }
            else
            {
                this.YesButton = (Button)target;
                this.YesButton.Click += new RoutedEventHandler(this.YesButtonClicked);
            }
        }
    }
}
