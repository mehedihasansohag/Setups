// Decompiled with JetBrains decompiler
// Type: IWshRuntimeLibrary.IWshShortcut
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IWshRuntimeLibrary
{
    [CompilerGenerated]
    [DefaultMember("FullName")]
    [Guid("F935DC23-1CF0-11D0-ADB9-00C04FD58A0B")]
    [TypeIdentifier]
    [ComImport]
    public interface IWshShortcut
    {
        [DispId(0)]
        string FullName { [DispId(0), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

        [DispId(1000)]
        string Arguments { [DispId(1000), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1000), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

        [SpecialName]
        [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
        sealed extern void _VtblGap1_4();

        [DispId(1003)]
        string IconLocation { [DispId(1003), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1003), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

        [SpecialName]
        [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
        sealed extern void _VtblGap2_1();

        [DispId(1005)]
        string TargetPath { [DispId(1005), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1005), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

        [DispId(1006)]
        int WindowStyle { [DispId(1006), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1006), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [SpecialName]
        [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
        sealed extern void _VtblGap3_3();

        [DispId(2001)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Save();
    }
}
