// Decompiled with JetBrains decompiler
// Type: IWshRuntimeLibrary.IWshShell3
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IWshRuntimeLibrary
{
    [CompilerGenerated]
    [Guid("41904400-BE18-11D3-A28B-00104BD35090")]
    [TypeIdentifier]
    [ComImport]
    public interface IWshShell3 : IWshShell2
    {
        [SpecialName]
        [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
        sealed extern void _VtblGap1_4();

        [DispId(1002)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.IDispatch)]
        object CreateShortcut([MarshalAs(UnmanagedType.BStr), In] string PathLink);
    }
}
