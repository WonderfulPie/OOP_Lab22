using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Lab22
{
    public partial class DocumentForm : UserControl
    {
        private string _currentFileName;
        public bool IsFileSaved { get; private set; }

        public event EventHandler<FileSavedEventArgs> FileSaved;

        public DocumentForm()
        {
            InitializeComponent();
            SetDefaultParagraphProperties();
            Editor.Focus();
            IsFileSaved = false;

            // Обработчики событий для перетаскивания и вставки
            Editor.PreviewDragOver += Editor_PreviewDragOver;
            Editor.Drop += Editor_Drop;
            Editor.PreviewKeyDown += Editor_PreviewKeyDown;
            DataObject.AddPastingHandler(Editor, OnPaste);
        }

        private void SetDefaultParagraphProperties()
        {
            foreach (var block in Editor.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    paragraph.LineHeight = double.NaN;
                    paragraph.Margin = new Thickness(0);
                }
            }
        }

        private void Editor_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (filePaths.Length > 0 && IsImageFile(filePaths[0]))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
                e.Handled = true;
            }
        }

        private void Editor_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (filePaths.Length > 0 && IsImageFile(filePaths[0]))
                {
                    InsertImageFromFile(filePaths[0]);
                }
                e.Handled = true;
            }
        }

        private void Editor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Clipboard.ContainsImage())
                {
                    InsertImageFromClipboard();
                    e.Handled = true;
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    var fileDropList = Clipboard.GetFileDropList();
                    if (fileDropList.Count > 0 && IsImageFile(fileDropList[0]))
                    {
                        InsertImageFromFile(fileDropList[0]);
                        e.Handled = true;
                    }
                }
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.SourceDataObject.GetDataPresent(DataFormats.Bitmap))
            {
                var image = new Image
                {
                    Source = (BitmapSource)e.SourceDataObject.GetData(DataFormats.Bitmap),
                    Width = 300
                };
                var container = new InlineUIContainer(image, Editor.CaretPosition);
                Editor.CaretPosition = container.ElementEnd;
                e.CancelCommand();
            }
            else if (e.SourceDataObject.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.SourceDataObject.GetData(DataFormats.FileDrop);
                if (filePaths.Length > 0 && IsImageFile(filePaths[0]))
                {
                    InsertImageFromFile(filePaths[0]);
                    e.CancelCommand();
                }
            }
        }

        private bool IsImageFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif";
        }

        private void InsertImageFromFile(string filePath)
        {
            var image = new Image
            {
                Source = new BitmapImage(new Uri(filePath)),
                Width = 300
            };
            var container = new InlineUIContainer(image, Editor.CaretPosition);
            Editor.CaretPosition = container.ElementEnd;
        }

        private void InsertImageFromClipboard()
        {
            var image = new Image
            {
                Source = Clipboard.GetImage(),
                Width = 300
            };
            var container = new InlineUIContainer(image, Editor.CaretPosition);
            Editor.CaretPosition = container.ElementEnd;
        }

        public void LoadFile(string filePath)
        {
            _currentFileName = filePath;
            TextRange documentTextRange = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                documentTextRange.Load(fileStream, DataFormats.Rtf);
            }
            SetDefaultParagraphProperties();
            IsFileSaved = true;
        }

        public void SaveFile()
        {
            if (!string.IsNullOrEmpty(_currentFileName))
            {
                SaveToFile(_currentFileName);
                IsFileSaved = true;
                RaiseFileSaved(_currentFileName);
            }
        }

        public void SaveFileAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|All Files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _currentFileName = saveFileDialog.FileName;
                SaveToFile(_currentFileName);
                IsFileSaved = true;
                RaiseFileSaved(_currentFileName);
            }
        }

        private void SaveToFile(string filePath)
        {
            TextRange documentTextRange = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                documentTextRange.Save(fileStream, DataFormats.Rtf);
            }
        }

        public void ChangeFont()
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog())
            {
                Editor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(fontDialog.FontName));
                Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontDialog.FontSize);
            }
        }

        public void ChangeColor()
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog())
            {
                Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(colorDialog.SelectedColor));
            }
        }

        public void InsertImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                InsertImageFromFile(openFileDialog.FileName);
            }
        }

        // Методы для выравнивания текста
        private void AlignLeft_Click(object sender, RoutedEventArgs e)
        {
            ApplyTextAlignment(TextAlignment.Left);
        }

        private void AlignCenter_Click(object sender, RoutedEventArgs e)
        {
            ApplyTextAlignment(TextAlignment.Center);
        }

        private void AlignRight_Click(object sender, RoutedEventArgs e)
        {
            ApplyTextAlignment(TextAlignment.Right);
        }

        private void AlignJustify_Click(object sender, RoutedEventArgs e)
        {
            ApplyTextAlignment(TextAlignment.Justify);
        }

        private void ApplyTextAlignment(TextAlignment alignment)
        {
            TextRange textRange = new TextRange(Editor.Selection.Start, Editor.Selection.End);
            textRange.ApplyPropertyValue(Paragraph.TextAlignmentProperty, alignment);

            // Применим выравнивание к каждому параграфу в выделенном тексте
            foreach (var block in Editor.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    if (textRange.Contains(paragraph.ContentStart))
                    {
                        paragraph.TextAlignment = alignment;
                    }
                }
            }
        }

        private void RaiseFileSaved(string newFileName)
        {
            string fileName = Path.GetFileName(newFileName);
            FileSaved?.Invoke(this, new FileSavedEventArgs(fileName));
        }

        // Обработчики для кнопок на панели инструментов
        private void ChangeFont_Click(object sender, RoutedEventArgs e)
        {
            ChangeFont();
        }

        private void ChangeColor_Click(object sender, RoutedEventArgs e)
        {
            ChangeColor();
        }

        private void InsertImage_Click(object sender, RoutedEventArgs e)
        {
            InsertImage();
        }
    }

    public class FileSavedEventArgs : EventArgs
    {
        public string NewFileName { get; }

        public FileSavedEventArgs(string newFileName)
        {
            NewFileName = newFileName;
        }
    }
}