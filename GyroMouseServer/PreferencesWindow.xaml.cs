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
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        Configuration config;
        public PreferencesWindow()
        {
            InitializeComponent();
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("HERE!");
            config.AppSettings.Settings.Add("ModificationDate", DateTime.Now.ToLongTimeString() + " ");
            config.Save(ConfigurationSaveMode.Modified);

            //string value = ConfigurationManager.AppSettings["setting2"];
            //Console.WriteLine("Key: {0}, Value: {1}", "setting2", value);
        }

        static void ShowConfig()
        {

            // For read access you do not need to call OpenExeConfiguraton
            foreach (string key in ConfigurationManager.AppSettings)
            {
                string value = ConfigurationManager.AppSettings[key];
                Console.WriteLine("Key: {0}, Value: {1}", key, value);
            }
        }

    }
}
