using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroMouseServer
{
    class KeyboardInput
    {
        
        public void typeIn(String character)
        {
            String command = "(" + character + ")";
            SendKeys.SendWait(command);
        }
    }
}
