using WindowsInput;
using System.Collections;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroMouseServer
{
    internal class KeyboardInput
    {
        public void typeIn(String character)
        {
            switch (character)
            {
                case "~":
                case "(":
                case ")":
                case "{":
                case "}":
                    InputSimulator.SimulateTextEntry(character);
                    return;
            }
            SendKeys.SendWait(character);
        }
    }
}
