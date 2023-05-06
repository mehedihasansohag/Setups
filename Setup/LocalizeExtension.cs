// Decompiled with JetBrains decompiler
// Type: Setup.LocalizeExtension
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library;
using System;
using System.Windows.Markup;

namespace Setup
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocalizeExtension : MarkupExtension
    {
        private readonly string key = string.Empty;

        public LocalizeExtension(string key) => this.key = key;

        public override object ProvideValue(IServiceProvider serviceProvider) => (object)StringParser.Parse(ResourceService.GetString(this.key));
    }
}
