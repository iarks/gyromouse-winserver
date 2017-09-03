using System;

namespace GyroMouseServer
{
    static class DefaultPreferences
    {
        public static bool autoStart = false;
        public static bool autoServe = false;
        public static bool startMin = false;
        public static bool minTray = false;
        public static bool showNotif = false;

        public static int sensitivity = 25;
        public static int acceleration = 50;

        public static String preferredPort = "9050";
    }
}
