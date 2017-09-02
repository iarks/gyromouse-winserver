using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Drawing;

namespace GyroMouseServer_MouseMove
{
    class Mouse
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        //Cursor cursor = new Cursor(Cursor.Current.Handle);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public void movePointer(float x, float y)
        {
            //Point p = System.Windows.Forms.Cursor.Position;
            //x = x * 25;
            //y = y * 25;

            int a = (int)x;
            int b = (int)y;


            SetCursorPos(Cursor.Position.X + a, Cursor.Position.Y + b);
            //Cursor.Position = new Point(Cursor.Position.X + a, Cursor.Position.Y + b);
        }

        public void leftDown()
        {
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
        }

        public void leftUp()
        {
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        public void rightDown()
        {
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_RIGHTDOWN, X, Y, 0, 0);
        }

        public void rightUp()
        {
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }

    }
}

