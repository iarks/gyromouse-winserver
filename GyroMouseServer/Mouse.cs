using System.Drawing.Drawing2D;
using System;
using System.Threading;
using Windows.UI.Input;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

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

            //Console.WriteLine("startPosition " + Cursor.Position.X + "," + Cursor.Position.Y);

            SetCursorPos(Cursor.Position.X + a, Cursor.Position.Y + b);
        }

        public void smoothMovePointer(float x, float y, float pastX, float pastY)
        {
            int smoothing = 1000;
            x = x + (x - pastX) / smoothing;
            y = y + (y - pastX) / smoothing;
            movePointer(x*GyroMouseServer.Properties.Settings.Default.sensitivity, y*GyroMouseServer.Properties.Settings.Default.sensitivity);
        }



        //public void movePointer(float x, float y)
        //{
        //    Point newPosition = new Point();
        //    newPosition.X = Cursor.Position.X+(int)x;
        //    newPosition.Y = Cursor.Position.Y+(int)y;
        //    Console.WriteLine(newPosition.X.ToString() + "," + newPosition.Y.ToString());

        //    int steps = 1000;
        //    int MouseEventDelayMS = 1;


        //    Point start = new Point();
        //    start.X = Cursor.Position.X;
        //    start.Y = Cursor.Position.Y;
        //    Console.WriteLine("startPosition " + Cursor.Position.X + "," + Cursor.Position.Y);

        //    PointF iterPoint = start;

        //    // Find the slope of the line segment defined by start and newPosition
        //    PointF slope = new PointF(newPosition.X - start.X, newPosition.Y - start.Y);

        //    // Divide by the number of steps
        //    slope.X = slope.X / steps;
        //    slope.Y = slope.Y / steps;

        //    for (int i = 0; i < steps; i++)
        //    {
        //        iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
        //        SetCursorPos((int)iterPoint.X, (int)iterPoint.Y);
        //        //Thread.Sleep(MouseEventDelayMS);
        //    }

        //    // Move the mouse to the final destination.
        //    SetCursorPos(newPosition.X,newPosition.Y);
        //}

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