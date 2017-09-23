using System.Windows.Controls;
using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Forms;

namespace GyroMouseServer
{
    public partial class PreferencesWindow : Window
    {
        String initUDP, initTCP;
        bool advanceChangesMade = false;
        public PreferencesWindow()
        {
            InitializeComponent();
            LoadPreferences();
        }

        private void Button_done_click(object sender, RoutedEventArgs e)
        {
            savePrefs();
            Close();
            if(advanceChangesMade==true && ServerState.serverRunning==true)
            {
                string messageBoxText = "Port addresses changed.\nRestart server for changes to take effect?";
                string caption = "Gyro Mouse";
                MessageBoxButton button = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Information;
                MessageBoxResult result = System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);

                // Process message box results
                switch (result)
                {
                    case MessageBoxResult.OK:
                        var main = (MainWindow)this.Owner;
                        main.restartServer();
                        break;
                }
            }
        }

        private void Button_apply_Click(object sender, RoutedEventArgs e)
        {
            savePrefs();
        }

        void LoadPreferences()
        {
            checkBox_autoStart.IsChecked = GyroMouseServer.Properties.Settings.Default.autoStart;
            checkBox_autoServer.IsChecked = GyroMouseServer.Properties.Settings.Default.autoServe;
            checkBox_minStart.IsChecked = GyroMouseServer.Properties.Settings.Default.startMin;
            checkBox_minTray.IsChecked = GyroMouseServer.Properties.Settings.Default.minTray;
            checkBox_showNotif.IsChecked = GyroMouseServer.Properties.Settings.Default.showNotif;

            slider_sensitivity.Value = GyroMouseServer.Properties.Settings.Default.sensitivity;
            slider_acceleration.Value = GyroMouseServer.Properties.Settings.Default.acceleration;
            
            textBox_preferredUDPPort.Text = GyroMouseServer.Properties.Settings.Default.preferredUDPPort;
            textBox_preferredTCPPort.Text = GyroMouseServer.Properties.Settings.Default.preferredTCPPort;

            textBox_sensitivity.Text = GyroMouseServer.Properties.Settings.Default.sensitivity.ToString();
            textBox_acceleration.Text = GyroMouseServer.Properties.Settings.Default.acceleration.ToString();
            
            initUDP = GyroMouseServer.Properties.Settings.Default.preferredUDPPort;
            initTCP = GyroMouseServer.Properties.Settings.Default.preferredTCPPort;
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

            if (textBox_preferredUDPPort.Text != "")
                GyroMouseServer.Properties.Settings.Default.preferredUDPPort = textBox_preferredUDPPort.Text;
            

            if(textBox_preferredTCPPort.Text != "")
                GyroMouseServer.Properties.Settings.Default.preferredTCPPort = textBox_preferredTCPPort.Text;

            GyroMouseServer.Properties.Settings.Default.Save();

            checkAdvanceChangesMade();

            
            
        }

        void checkAdvanceChangesMade()
        {
            if (initTCP != GyroMouseServer.Properties.Settings.Default.preferredTCPPort || initUDP != GyroMouseServer.Properties.Settings.Default.preferredUDPPort)
            {
                advanceChangesMade = true;
            }

            if (GyroMouseServer.Properties.Settings.Default.preferredTCPPort == initTCP && GyroMouseServer.Properties.Settings.Default.preferredUDPPort == initUDP)
            {
                advanceChangesMade = false;
            }
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
                    textBox_preferredUDPPort.Text = DefaultPreferences.preferredUDPPort;
                    textBox_preferredTCPPort.Text = DefaultPreferences.preferredTCPPort;
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

        


        private void textBox_preferredUDPPort_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            String key = e.Key.ToString();
            String lastKey = key.Substring(key.Length - 1, 1);
            switch(lastKey)
            {
                case "9":
                case "8":
                case "7":
                case "6":
                case "5":
                case "4":
                case "3":
                case "2":
                case "1":
                case "0":
                    break;
                default:
                    WinkeyInput.KeyDown(Keys.Back);
                    WinkeyInput.KeyUp(Keys.Back);
                    break;
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void textBox_preferredTCPPort_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            String key = e.Key.ToString();
            String lastKey = key.Substring(key.Length - 1, 1);
            switch (lastKey)
            {
                case "9":
                case "8":
                case "7":
                case "6":
                case "5":
                case "4":
                case "3":
                case "2":
                case "1":
                case "0":
                    break;
                default:
                    WinkeyInput.KeyDown(Keys.Back);
                    WinkeyInput.KeyUp(Keys.Back);
                    break;
            }
        }
    }
}
