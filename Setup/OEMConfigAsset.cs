﻿// Decompiled with JetBrains decompiler
// Type: Setup.OEMConfigAsset
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System.IO;

namespace Setup
{
    public class OEMConfigAsset
    {
        public string OEMConfigPath { get; set; }

        public string MachineModel { get; set; }

        public string FolderName => Path.GetDirectoryName(this.OEMConfigPath);
    }
}
