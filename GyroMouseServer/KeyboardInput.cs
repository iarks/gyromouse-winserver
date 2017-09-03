using WindowsInput;
using System;
using System.Windows.Forms;

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
