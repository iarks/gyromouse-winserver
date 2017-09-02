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

        //Cursor cursor = new Cursor(Cursor.Current.Handle);


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
    }
}

