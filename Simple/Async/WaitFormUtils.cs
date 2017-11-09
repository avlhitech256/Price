using System;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;

namespace TasklistView.Utils
{
    public static class WaitFormUtils
    {
        public static void ShowWaitForm(this Form parent, string description)
        {
            SplashScreenManager.ShowForm(parent, typeof(DemoWaitForm), true, true, false, ParentFormState.Locked);
            SplashScreenManager @default = SplashScreenManager.Default;
            @default.SetWaitFormCaption(Properties.Resources.PleaseWait);
            @default.SetWaitFormDescription(description);
        }

        public static void CloseWaitForm(this Form parent)
        {
            try
            {
                SplashScreenManager.CloseForm();
            }
            catch (Exception) // parasoft-suppress SCC.EXC expected flow
            {
            }
        }
    }
}
