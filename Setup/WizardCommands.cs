// Decompiled with JetBrains decompiler
// Type: Setup.WizardCommands
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System.Windows.Input;

namespace Setup
{
    public static class WizardCommands
    {
        public static RoutedCommand Cancel { get; } = new RoutedCommand();

        public static RoutedCommand Finish { get; } = new RoutedCommand();

        public static RoutedCommand Help { get; } = new RoutedCommand();

        public static RoutedCommand NextPage { get; } = new RoutedCommand();

        public static RoutedCommand PreviousPage { get; } = new RoutedCommand();

        public static RoutedCommand SelectPage { get; } = new RoutedCommand();
    }
}
