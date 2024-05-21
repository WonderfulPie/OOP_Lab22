using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace Lab22
{
    public partial class MainWindow : Window
    {
        public static readonly RoutedCommand InsertImageCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(InsertImageCommand, InsertImage_Executed));
            LocalizationManager.UpdateResourceDictionary(); // Убедимся, что ресурсы обновляются при запуске
            UpdateLocalization(); // Обновите локализацию при запуске
        }

        private void NewFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewFile_Click(sender, e);
        }

        private void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFile_Click(sender, e);
        }

        private void SaveFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFile_Click(sender, e);
        }

        private void InsertImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertImage_Click(sender, e);
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab(LocalizationManager.T("Untitled"));
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                AddNewTab(openFileDialog.FileName, openFileDialog.FileName);
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentTabs.SelectedItem is TabItem selectedTab && selectedTab.Content is DocumentForm documentForm)
            {
                if (documentForm.IsFileSaved)
                {
                    documentForm.SaveFile();
                }
                else
                {
                    SaveFileAs_Click(sender, e);
                }
            }
        }

        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentTabs.SelectedItem is TabItem selectedTab && selectedTab.Content is DocumentForm documentForm)
            {
                documentForm.SaveFileAs();
            }
        }

        private void Font_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentTabs.SelectedItem is TabItem selectedTab && selectedTab.Content is DocumentForm documentForm)
            {
                documentForm.ChangeFont();
            }
        }

        private void Color_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentTabs.SelectedItem is TabItem selectedTab && selectedTab.Content is DocumentForm documentForm)
            {
                documentForm.ChangeColor();
            }
        }

        private void InsertImage_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentTabs.SelectedItem is TabItem selectedTab && selectedTab.Content is DocumentForm documentForm)
            {
                documentForm.InsertImage();
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
            UpdateLocalization();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(LocalizationManager.T("About"), LocalizationManager.T("About"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UpdateLocalization()
        {
            LocalizationManager.UpdateResourceDictionary();
        }

        private void AddNewTab(string title, string filePath = null)
        {
            var documentForm = new DocumentForm();
            if (filePath != null)
            {
                documentForm.LoadFile(filePath);
                title = System.IO.Path.GetFileName(filePath); // Извлекаем только имя файла
            }

            var newTab = new TabItem
            {
                Header = title,
                Content = documentForm,
                IsSelected = true
            };

            DocumentTabs.Items.Add(newTab);

            UpdateSaveMenuState();
            DocumentTabs.SelectionChanged += (s, e) => UpdateSaveMenuState();

            // Подписка на событие обновления заголовка вкладки
            documentForm.FileSaved += (s, e) => newTab.Header = System.IO.Path.GetFileName(e.NewFileName);

            // Установим фокус на RichTextBox после добавления новой вкладки
            Dispatcher.BeginInvoke(new Action(() =>
            {
                documentForm.Editor.Focus();
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void UpdateSaveMenuState()
        {
            SaveMenuItem.IsEnabled = SaveAsMenuItem.IsEnabled = DocumentTabs.Items.Count > 0;
        }
    }
}