// Decompiled with JetBrains decompiler
// Type: XamlGeneratedNamespace.GeneratedInternalTypeHelper
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;

namespace XamlGeneratedNamespace
{
    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class GeneratedInternalTypeHelper : InternalTypeHelper
    {
        protected override object CreateInstance(Type type, CultureInfo culture) => Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder)null, (object[])null, culture);

        protected override object GetPropertyValue(
          PropertyInfo propertyInfo,
          object target,
          CultureInfo culture)
        {
            return propertyInfo.GetValue(target, BindingFlags.Default, (Binder)null, (object[])null, culture);
        }

        protected override void SetPropertyValue(
          PropertyInfo propertyInfo,
          object target,
          object value,
          CultureInfo culture)
        {
            propertyInfo.SetValue(target, value, BindingFlags.Default, (Binder)null, (object[])null, culture);
        }

        protected override Delegate CreateDelegate(Type delegateType, object target, string handler) => (Delegate)target.GetType().InvokeMember("_CreateDelegate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, (Binder)null, target, new object[2]
        {
      (object) delegateType,
      (object) handler
        }, (CultureInfo)null);

        protected override void AddEventHandler(EventInfo eventInfo, object target, Delegate handler) => eventInfo.AddEventHandler(target, handler);
    }
}
