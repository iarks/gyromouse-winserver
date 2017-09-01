using System.Runtime.InteropServices;
using System.Threading;

namespace GyroMouseServer
{
    class MouseMove
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        public void movePointer(int x, int y)
        {
            SetCursorPos(x, y);
        }
    }
}

