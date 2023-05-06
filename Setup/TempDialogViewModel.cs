// Decompiled with JetBrains decompiler
// Type: Setup.TempDialogViewModel
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using GalaSoft.MvvmLight;
using Setup.Properties;
using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Setup
{
    internal class TempDialogViewModel : ViewModelBase
    {
        private const int IMAGE_COUNT = 5;
        private static BitmapSource[] bitmapSources = new BitmapSource[5];
        private TempDialogButton buttonType;
        private TempDialogImage imageType;
        private string dialogCaption;
        private ImageSource messageTypeIcon;
        private Visibility contentTypeIconVisibility;
        private double iconHeight;
        private double iconWidth;
        private string messageContent;
        private Visibility noButtonVisibility;
        private Visibility yesButtonVisibility;

        static TempDialogViewModel()
        {
            TempDialogViewModel.bitmapSources[0] = (BitmapSource)null;
            Bitmap bitmap1 = SystemIcons.Information.ToBitmap();
            TempDialogViewModel.bitmapSources[1] = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap1.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Bitmap bitmap2 = SystemIcons.Question.ToBitmap();
            TempDialogViewModel.bitmapSources[2] = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap2.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Bitmap bitmap3 = SystemIcons.Warning.ToBitmap();
            TempDialogViewModel.bitmapSources[3] = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap3.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Bitmap bitmap4 = SystemIcons.Error.ToBitmap();
            TempDialogViewModel.bitmapSources[4] = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap4.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public TempDialogViewModel(
          string messageText = null,
          string caption = null,
          TempDialogButton buttonType = TempDialogButton.Yes,
          TempDialogImage imageType = TempDialogImage.None)
        {
            this.Initialize();
            this.MessageContent = messageText;
            this.DialogCaption = caption;
            this.buttonType = buttonType;
            this.imageType = imageType;
            if (this.buttonType == TempDialogButton.Yes)
            {
                this.NoButtonVisibility = Visibility.Collapsed;
                this.YesButtonVisibility = Visibility.Visible;
            }
            else if (this.buttonType == TempDialogButton.YesNo)
            {
                this.NoButtonVisibility = Visibility.Visible;
                this.YesButtonVisibility = Visibility.Visible;
            }
            int imageType1 = (int)this.imageType;
            if (imageType1 < 0 || imageType1 >= 5)
                return;
            this.MessageTypeIcon = (ImageSource)TempDialogViewModel.bitmapSources[imageType1];
        }

        private void Initialize()
        {
            this.DialogCaption = (string)null;
            this.MessageTypeIcon = (ImageSource)null;
            this.ContentTypeIconVisibility = Visibility.Collapsed;
            this.IconHeight = 0.0;
            this.IconWidth = 0.0;
            this.MessageContent = (string)null;
            this.NoButtonVisibility = Visibility.Collapsed;
            this.YesButtonVisibility = Visibility.Collapsed;
        }

        public string DialogCaption
        {
            get => this.dialogCaption;
            set
            {
                this.dialogCaption = value;
                this.RaisePropertyChanged<string>((Expression<Func<string>>)(() => this.DialogCaption));
            }
        }

        public ImageSource MessageTypeIcon
        {
            get => this.messageTypeIcon;
            set
            {
                this.messageTypeIcon = value;
                if (value != null)
                {
                    this.IconHeight = value.Height;
                    this.IconWidth = value.Width;
                }
                else
                {
                    this.IconHeight = 0.0;
                    this.IconWidth = 0.0;
                }
                this.RaisePropertyChanged<ImageSource>((Expression<Func<ImageSource>>)(() => this.MessageTypeIcon));
            }
        }

        public Visibility ContentTypeIconVisibility
        {
            get => this.contentTypeIconVisibility;
            set
            {
                this.contentTypeIconVisibility = value;
                this.RaisePropertyChanged<Visibility>((Expression<Func<Visibility>>)(() => this.ContentTypeIconVisibility));
            }
        }

        public double IconHeight
        {
            get => this.iconHeight;
            set
            {
                this.iconHeight = value;
                this.RaisePropertyChanged<double>((Expression<Func<double>>)(() => this.IconHeight));
            }
        }

        public double IconWidth
        {
            get => this.iconWidth;
            set
            {
                this.iconWidth = value;
                this.RaisePropertyChanged<double>((Expression<Func<double>>)(() => this.IconWidth));
            }
        }

        public string MessageContent
        {
            get => this.messageContent;
            set
            {
                this.messageContent = value;
                this.RaisePropertyChanged<string>((Expression<Func<string>>)(() => this.MessageContent));
            }
        }

        public string NoButtonContent => Resources.Btn_Cancel_NK300;

        public string YesButtonContent => Resources.Btn_OK_NK300;

        public Visibility NoButtonVisibility
        {
            get => this.noButtonVisibility;
            set
            {
                this.noButtonVisibility = value;
                this.RaisePropertyChanged<Visibility>((Expression<Func<Visibility>>)(() => this.NoButtonVisibility));
            }
        }

        public Visibility YesButtonVisibility
        {
            get => this.yesButtonVisibility;
            set
            {
                this.yesButtonVisibility = value;
                this.RaisePropertyChanged<Visibility>((Expression<Func<Visibility>>)(() => this.YesButtonVisibility));
            }
        }
    }
}
