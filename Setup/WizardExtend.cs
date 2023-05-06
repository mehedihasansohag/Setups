// Decompiled with JetBrains decompiler
// Type: Setup.WizardExtend
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using System;

namespace Setup
{
    public static class WizardExtend
    {
        public static bool RemovePage<T>(this Wizard wizard)
        {
            bool flag = false;
            try
            {
                WizardPage page = wizard.GetPage<T>();
                if (page != null)
                {
                    Wizard.Instance.Items.Remove((object)page);
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        public static WizardPage GetPage<T>(this Wizard wizard)
        {
            WizardPage page = null;
            try
            {
                for (int index = wizard.Items.Count - 1; index >= 0; --index)
                {
                    if (wizard.Items[index] is WizardPage wizardPage && wizardPage.Content is T)
                    {
                        page = wizardPage;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return page;
        }
    }
}
