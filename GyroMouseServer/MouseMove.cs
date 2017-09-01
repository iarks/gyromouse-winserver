using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Drawing;

namespace GyroMouseServer_MouseMove
{
    class MouseMove
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);



        public void movePointer(float x, float y)
        {
            //Point p = System.Windows.Forms.Cursor.Position;
            x = x * 25;
            y = y * 25;

            int a = (int)x;
            int b = (int)y;


            //SetCursorPos(a, b);

            Cursor cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X + a, Cursor.Position.Y + b);
        }
    }
}

