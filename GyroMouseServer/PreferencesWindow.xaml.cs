using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Configuration;

namespace GyroMouseServer
{
    public partial class PreferencesWindow : Window
    {
        public PreferencesWindow()
        {
            InitializeComponent();
            loadPreferences();
        }

        private void button_done_click(object sender, RoutedEventArgs e)
        {
            savePrefs();
            Close();
        }

        private void button_apply_Click(object sender, RoutedEventArgs e)
        {
            savePrefs();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        void loadPreferences()
        {
            checkBox_autoStart.IsChecked = GyroMouseServer.Properties.Settings.Default.autoStart;
            checkBox_minStart.IsChecked = GyroMouseServer.Properties.Settings.Default.startMin;
            checkBox_minTray.IsChecked = GyroMouseServer.Properties.Settings.Default.minTray;
            checkBox_showNotif.IsChecked = GyroMouseServer.Properties.Settings.Default.showNotif;

            slider_sensitivity.Value = GyroMouseServer.Properties.Settings.Default.sensitivity;
            slider_acceleration.Value = GyroMouseServer.Properties.Settings.Default.acceleration;
            
            textBox_preferredPort.Text = GyroMouseServer.Properties.Settings.Default.preferredPort;    
        }




        void savePrefs()
        {
            GyroMouseServer.Properties.Settings.Default.autoStart = (bool)checkBox_autoStart.IsChecked;
            GyroMouseServer.Properties.Settings.Default.startMin = (bool)checkBox_minStart.IsChecked;
            GyroMouseServer.Properties.Settings.Default.minTray = (bool)checkBox_minTray.IsChecked;
            GyroMouseServer.Properties.Settings.Default.showNotif = (bool)checkBox_showNotif.IsChecked;

            GyroMouseServer.Properties.Settings.Default.sensitivity = (int)slider_sensitivity.Value;

            GyroMouseServer.Properties.Settings.Default.acceleration = (int)slider_acceleration.Value;
            
            GyroMouseServer.Properties.Settings.Default.Save();
        }

        private void checkBox_autoStart_Click(object sender, RoutedEventArgs e)
        {
            GyroMouseServer.Properties.Settings.Default.autoStart = (bool)checkBox_autoStart.IsChecked;
        }
    }
}
