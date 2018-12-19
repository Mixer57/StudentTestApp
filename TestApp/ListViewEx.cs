using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mixer57
{
    public class ListViewEx:ListView
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!DesignMode && Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
                SetWindowTheme(Handle, "explorer", null);
        }
    }
}
