// Decompiled with JetBrains decompiler
// Type: Setup.MachineDataGrid
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System.Windows.Controls;
using System.Windows.Input;

namespace Setup
{
    internal class MachineDataGrid : DataGrid
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                this.SelectedIndex = (this.SelectedIndex + 1) % this.Items.Count;
            }
            else
            {
                if (e.Key != Key.Up)
                    return;
                this.SelectedIndex = (this.SelectedIndex - 1 + this.Items.Count) % this.Items.Count;
            }
        }
    }
}
