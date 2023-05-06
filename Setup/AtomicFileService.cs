// Decompiled with JetBrains decompiler
// Type: Setup.AtomicFileService
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Setup
{
    internal static class AtomicFileService
    {
        private const string DEFAULT_TEMP_FILE_SUFFIX = ".tmp";
        private const string DEFAULT_ALT_FILE_SUFFIX = ".alt";
        private static readonly bool isWindows = true;

        static AtomicFileService() => AtomicFileService.isWindows = Environment.OSVersion.Platform != PlatformID.Unix;

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void AtomicWriteEx(string targetPath, Stream stream)
        {
            string tmpPath = AtomicFileService.GetTmpPath(targetPath);
            string altPath = AtomicFileService.GetAltPath(targetPath);
            using (FileStream destination = new FileStream(tmpPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.WriteThrough))
            {
                stream.Position = 0L;
                stream.CopyTo((Stream)destination);
                destination.Flush(true);
            }
            if (File.Exists(targetPath))
                AtomicFileService.RenameFile(targetPath, altPath);
            AtomicFileService.RenameFile(tmpPath, targetPath);
            if (!File.Exists(altPath))
                return;
            File.Delete(altPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static Stream AtomicReadEx(string targetPath, bool tryRecover = false)
        {
            string tmpPath = AtomicFileService.GetTmpPath(targetPath);
            string altPath = AtomicFileService.GetAltPath(targetPath);
            if (File.Exists(targetPath))
            {
                if (File.Exists(tmpPath))
                    File.Delete(tmpPath);
                if (File.Exists(altPath))
                    File.Delete(altPath);
                return (Stream)File.OpenRead(targetPath);
            }
            if (tryRecover && File.Exists(tmpPath) && File.Exists(altPath))
            {
                AtomicFileService.RenameFile(tmpPath, targetPath);
                File.Delete(altPath);
                return (Stream)File.OpenRead(targetPath);
            }
            if (File.Exists(tmpPath))
                File.Delete(tmpPath);
            if (File.Exists(altPath))
                File.Delete(altPath);
            return (Stream)null;
        }

        private static void RenameFile(string existFile, string newFile)
        {
            if (AtomicFileService.isWindows)
            {
                if (!AtomicFileService.NativeMethods.MoveFileEx(existFile, newFile, AtomicFileService.NativeMethods.MoveFileFlags.ReplaceExisting | AtomicFileService.NativeMethods.MoveFileFlags.WriteThrough))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "AtomicFileService.RenameFile Exception!");
            }
            else
            {
                if (File.Exists(newFile))
                    File.Delete(newFile);
                File.Move(existFile, newFile);
            }
        }

        private static string GetTmpPath(string path) => string.Format("{0}{1}", (object)path, (object)".tmp");

        private static string GetAltPath(string path) => string.Format("{0}{1}", (object)path, (object)".alt");

        internal static class NativeMethods
        {
            private const string dll_kernal32 = "kernel32";

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool MoveFileEx(
              [In] string lpExistingFileName,
              [In] string lpNewFileName,
              [In] AtomicFileService.NativeMethods.MoveFileFlags dwFlags);

            [Flags]
            internal enum MoveFileFlags
            {
                None = 0,
                ReplaceExisting = 1,
                CopyAllowed = 2,
                DelayUntilReboot = 4,
                WriteThrough = 8,
                CreateHardlink = 16, // 0x00000010
                FailIfNotTrackable = 32, // 0x00000020
            }
        }
    }
}
