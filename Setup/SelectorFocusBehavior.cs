// Decompiled with JetBrains decompiler
// Type: Setup.SelectorFocusBehavior
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace Setup
{
    public class SelectorFocusBehavior : Behavior<Selector>
    {
        private bool isFocused;
        private bool isAttched;
        private bool firstLayout = true;
        private int selectionIndex = -1;
        private int columnIndex = -1;

        protected override void OnAttached()
        {
            if (this.isAttched)
                return;
            base.OnAttached();
            this.AssociatedObject.LayoutUpdated += new EventHandler(this.LayoutUpdated);
            this.AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(this.SelectionChanged);
            this.AssociatedObject.GotFocus += new RoutedEventHandler(this.Focus);
            this.AssociatedObject.Unloaded += new RoutedEventHandler(this.Reset);
            if (this.AssociatedObject is DataGrid)
                (this.AssociatedObject as DataGrid).CurrentCellChanged += new EventHandler<EventArgs>(this.SelectorFocusBehavior_CurrentCellChanged);
            this.isAttched = true;
        }

        private void SelectorFocusBehavior_CurrentCellChanged(object sender, EventArgs e)
        {
            if ((sender as DataGrid).CurrentCell.Column == null)
                return;
            this.columnIndex = (sender as DataGrid).CurrentCell.Column.DisplayIndex;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.LayoutUpdated -= new EventHandler(this.LayoutUpdated);
            this.AssociatedObject.SelectionChanged -= new SelectionChangedEventHandler(this.SelectionChanged);
            this.AssociatedObject.GotFocus -= new RoutedEventHandler(this.Focus);
            this.AssociatedObject.Unloaded -= new RoutedEventHandler(this.Reset);
            if (this.AssociatedObject is DataGrid)
                (this.AssociatedObject as DataGrid).CurrentCellChanged -= new EventHandler<EventArgs>(this.SelectorFocusBehavior_CurrentCellChanged);
            this.isAttched = false;
        }

        private void Focus(object sender, EventArgs e) => this.AssociatedObject.Dispatcher.BeginInvoke((Action)(() =>
       {
           if (this.AssociatedObject.SelectedIndex == -1)
               this.AssociatedObject.SelectedIndex = 0;
           this.AssociatedObject.UpdateLayout();
           if (!(this.AssociatedObject.ItemContainerGenerator.ContainerFromIndex(this.AssociatedObject.SelectedIndex) is UIElement uiElement2))
               return;
           if (this.AssociatedObject is DataGrid associatedObject2)
           {
               DataGridColumn dataGridColumn = associatedObject2.CurrentColumn;
               if (dataGridColumn == null && this.columnIndex != -1)
                   dataGridColumn = associatedObject2.ColumnFromDisplayIndex(this.columnIndex);
               if (dataGridColumn == null)
                   dataGridColumn = associatedObject2.Columns[0];
               FrameworkElement cellContent = dataGridColumn.GetCellContent(uiElement2 as DataGridRow);
               if (cellContent != null)
                   uiElement2 = cellContent.Parent as UIElement;
           }
           uiElement2.Focus();
           this.isFocused = true;
       }));

        private void SelectionChanged(object sender, EventArgs e)
        {
            if (this.AssociatedObject.SelectedIndex != -1)
                this.selectionIndex = this.AssociatedObject.SelectedIndex;
            else
                this.selectionIndex = 0;
        }

        private void LayoutUpdated(object sender, EventArgs e)
        {
            if (!this.AssociatedObject.IsVisible)
                return;
            if (this.firstLayout)
            {
                this.firstLayout = false;
            }
            else
            {
                if (this.AssociatedObject.SelectedIndex != -1 || this.AssociatedObject.Items.Count == 0)
                    return;
                if (this.selectionIndex >= this.AssociatedObject.Items.Count)
                    this.selectionIndex = this.AssociatedObject.Items.Count - 1;
                this.AssociatedObject.SelectedIndex = this.selectionIndex;
                this.Reset(sender, e);
                this.AssociatedObject.Focus();
            }
        }

        private void Reset(object sender, EventArgs e)
        {
            this.isFocused = false;
            this.firstLayout = true;
        }
    }
}
