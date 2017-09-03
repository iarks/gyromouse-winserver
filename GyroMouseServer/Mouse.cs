using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GyroMouseServer_MouseMove
{
    internal class Mouse
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;


        private const int MOUSEEVENTF_WHEEL = 0x0800;

        public void movePointer(float x, float y)
        {
            int a = (int)x;
            int b = (int)y;
            
            SetCursorPos(Cursor.Position.X + a, Cursor.Position.Y + b);
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

        public void scroll(float param)
        {
            int acceleration = GyroMouseServer.Properties.Settings.Default.acceleration;
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)param*(uint)acceleration, 0);
        }

    }
}