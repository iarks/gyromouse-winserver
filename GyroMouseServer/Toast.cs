using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using GyroMouseServer_LocalHost;

namespace GyroMouseServer
{
    public static class Toast
    {
        static NotifyIcon nIcon = new NotifyIcon();
        public static void generateToastInfo(int timeout, string header, string message)
        {
            nIcon.Icon = new Icon(@"D:\Users\Arkadeep\Documents\OneDrive\GyroMouseServer\GyroMouseServer\02_Acrobat.ico");
            nIcon.Visible = true;
            nIcon.ShowBalloonTip(timeout,header,message,ToolTipIcon.Info);
            nIcon.Visible = false;
        }

        public static void generateToast(int timeout, string header, string message)
        {
            nIcon.Icon = new Icon(@"D:\Users\Arkadeep\Documents\OneDrive\GyroMouseServer\GyroMouseServer\02_Acrobat.ico");
            nIcon.Visible = true;
            nIcon.ShowBalloonTip(timeout, header, message, ToolTipIcon.None);
            nIcon.Visible = false;
        }

        public static void generateToastWarning(int timeout, string header, string message)
        {
            nIcon.Icon = new Icon(@"D:\Users\Arkadeep\Documents\OneDrive\GyroMouseServer\GyroMouseServer\02_Acrobat.ico");
            nIcon.Visible = true;
            nIcon.ShowBalloonTip(timeout, header, message, ToolTipIcon.Warning);
            nIcon.Visible = false;
        }

        public static void generateToastError(int timeout, string header, string message)
        {
            nIcon.Icon = new Icon(@"D:\Users\Arkadeep\Documents\OneDrive\GyroMouseServer\GyroMouseServer\02_Acrobat.ico");
            nIcon.Visible = true;
            nIcon.ShowBalloonTip(timeout, header, message, ToolTipIcon.Error);
            nIcon.Visible = false;
        }

    }
}
