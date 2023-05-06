// Decompiled with JetBrains decompiler
// Type: Setup.WizardPageButtonVisibilityConverter
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Setup
{
    public class WizardPageButtonVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
            {
                throw new ArgumentException("Wrong number of arguments for WizardPageButtonVisibilityConverter.");

            }
            Visibility visibility1 = values[0] == null || values[0] == DependencyProperty.UnsetValue ? Visibility.Hidden : (Visibility)values[0];
            WizardPageButtonVisibility buttonVisibility = values[1] == null || values[1] == DependencyProperty.UnsetValue ? WizardPageButtonVisibility.Hidden : (WizardPageButtonVisibility)values[1];
            Visibility visibility2 = Visibility.Visible;
            switch (buttonVisibility)
            {
                case WizardPageButtonVisibility.Inherit:
                    visibility2 = visibility1;
                    break;
                case WizardPageButtonVisibility.Collapsed:
                    visibility2 = Visibility.Collapsed;
                    break;
                case WizardPageButtonVisibility.Hidden:
                    visibility2 = Visibility.Hidden;
                    break;
                case WizardPageButtonVisibility.Visible:
                    visibility2 = Visibility.Visible;
                    break;
            }
            return visibility2;
        }

        public object[] ConvertBack(
          object value,
          Type[] targetTypes,
          object parameter,
          CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
