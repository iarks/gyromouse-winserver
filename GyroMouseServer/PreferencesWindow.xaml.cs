using System.Windows.Forms.VisualStyles;
using System.Linq;
using System.Windows.Controls;
using System;
using System.Windows;
using Microsoft.Win32;

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

        void loadPreferences()
        {
            checkBox_autoStart.IsChecked = GyroMouseServer.Properties.Settings.Default.autoStart;
            checkBox_autoServer.IsChecked = GyroMouseServer.Properties.Settings.Default.autoServe;
            checkBox_minStart.IsChecked = GyroMouseServer.Properties.Settings.Default.startMin;
            checkBox_minTray.IsChecked = GyroMouseServer.Properties.Settings.Default.minTray;
            checkBox_showNotif.IsChecked = GyroMouseServer.Properties.Settings.Default.showNotif;

            slider_sensitivity.Value = GyroMouseServer.Properties.Settings.Default.sensitivity;
            slider_acceleration.Value = GyroMouseServer.Properties.Settings.Default.acceleration;
            
            textBox_preferredPort.Text = GyroMouseServer.Properties.Settings.Default.preferredPort;    

            textBox_sensitivity.Text = GyroMouseServer.Properties.Settings.Default.sensitivity.ToString();
            textBox_acceleration.Text = GyroMouseServer.Properties.Settings.Default.acceleration.ToString();
        }
        
        void savePrefs()
        {
            GyroMouseServer.Properties.Settings.Default.autoStart = (bool)checkBox_autoStart.IsChecked;
            setStartup();
            GyroMouseServer.Properties.Settings.Default.autoServe = (bool)checkBox_autoServer.IsChecked;
            GyroMouseServer.Properties.Settings.Default.startMin = (bool)checkBox_minStart.IsChecked;
            GyroMouseServer.Properties.Settings.Default.minTray = (bool)checkBox_minTray.IsChecked;
            GyroMouseServer.Properties.Settings.Default.showNotif = (bool)checkBox_showNotif.IsChecked;

            GyroMouseServer.Properties.Settings.Default.sensitivity = (int)slider_sensitivity.Value;

            GyroMouseServer.Properties.Settings.Default.acceleration = (int)slider_acceleration.Value;

            GyroMouseServer.Properties.Settings.Default.preferredPort = textBox_preferredPort.Text;
            
            GyroMouseServer.Properties.Settings.Default.Save();
        }

        private void checkBox_autoStart_Click(object sender, RoutedEventArgs e)
        {
            GyroMouseServer.Properties.Settings.Default.autoStart = (bool)checkBox_autoStart.IsChecked;
        }

        private void button_resetPage_Click(object sender, RoutedEventArgs e)
        {
            int currentTab = PreferencesTab.SelectedIndex;
            loadDefaults(currentTab);
        }

        void loadDefaults(int tabNumber)
        {
            switch(tabNumber)
            {
                case 0:
                    checkBox_autoStart.IsChecked = DefaultPreferences.autoStart;
                    checkBox_autoServer.IsChecked = DefaultPreferences.autoServe;
                    checkBox_minStart.IsChecked = DefaultPreferences.startMin;
                    checkBox_minTray.IsChecked = DefaultPreferences.minTray;
                    checkBox_showNotif.IsChecked = DefaultPreferences.showNotif;
                    break;
                case 1:
                    slider_sensitivity.Value = DefaultPreferences.sensitivity;
                    slider_acceleration.Value = DefaultPreferences.acceleration;
                    break;
                case 2:
                    textBox_preferredPort.Text = DefaultPreferences.preferredPort;
                    break;
            }
        }

        private void setStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if ((bool)checkBox_autoStart.IsChecked)
                rk.SetValue("GyroMouseServer", System.Reflection.Assembly.GetExecutingAssembly().Location);
            else
                rk.DeleteValue("GyroMouseServer", false);

        }


        private void slider_sensitivity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e != null && slider_sensitivity != null)
                textBox_sensitivity.Text = ((int)e.NewValue).ToString();
        }

        private void slider_acceleration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e != null && slider_acceleration!=null)
                textBox_acceleration.Text = ((int)e.NewValue).ToString();
        }

        private void textBox_sensitivity_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (slider_acceleration!=null)
            //    slider_acceleration.Value = Int32.Parse(textblock_sensitivity.Text.ToString());
        }
    }
}
