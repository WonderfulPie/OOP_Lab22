using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Lab22
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadCurrentLanguage();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedLanguage = selectedItem.Content.ToString();
                if (selectedLanguage == "English")
                {
                    LocalizationManager.SetLanguage(new CultureInfo("en-US"));
                }
                else if (selectedLanguage == "Ukrainian")
                {
                    LocalizationManager.SetLanguage(new CultureInfo("uk-UA"));
                }
                MessageBox.Show(LocalizationManager.T("Language changed. Restart the application for changes to take effect."), LocalizationManager.T("Information"), MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void LoadCurrentLanguage()
        {
            foreach (ComboBoxItem item in LanguageComboBox.Items)
            {
                if ((LocalizationManager.CurrentLanguage.Name == "en-US" && item.Content.ToString() == "English") ||
                    (LocalizationManager.CurrentLanguage.Name == "uk-UA" && item.Content.ToString() == "Ukrainian"))
                {
                    LanguageComboBox.SelectedItem = item;
                    break;
                }
            }
        }
    }
}