// Decompiled with JetBrains decompiler
// Type: Setup.LanguageComboBox
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System.Windows.Controls;
using System.Windows.Input;

namespace Setup
{
    internal class LanguageComboBox : ComboBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                if (e.Key == Key.Up)
                    this.SelectedIndex = (this.SelectedIndex - 1 + this.Items.Count) % this.Items.Count;
                else if (e.Key == Key.Down)
                    this.SelectedIndex = (this.SelectedIndex + 1) % this.Items.Count;
                else if (e.Key == Key.Space || e.Key == Key.F12 || e.Key == Key.Escape || e.Key == Key.Return)
                    this.IsDropDownOpen = false;
            }
            else if (e.Key == Key.F12)
                this.SelectedIndex = (this.SelectedIndex + 1) % this.Items.Count;
            else if (e.Key == Key.Space)
                this.IsDropDownOpen = true;
            e.Handled = true;
        }
    }
}
