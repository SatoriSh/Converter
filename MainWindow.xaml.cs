using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SkiaSharp;

namespace ImageConverter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.ico*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
                StatusTextBlock.Text = "Status: File selected.";
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            string inputFilePath = FilePathTextBox.Text;

            if (string.IsNullOrWhiteSpace(inputFilePath) || !File.Exists(inputFilePath))
            {
                StatusTextBlock.Text = "Status: Please select a valid file.";
                return;
            }

            if (FormatComboBox.SelectedItem is ComboBoxItem selectedFormatItem)
            {
                string targetFormat = selectedFormatItem.Content.ToString();
                string outputFilePath = Path.ChangeExtension(inputFilePath, targetFormat.ToLower());

                try
                {
                    using var inputStream = File.OpenRead(inputFilePath);
                    using var skBitmap = SKBitmap.Decode(inputStream);

                    if (skBitmap == null)
                    {
                        throw new Exception("Failed to load the image.");
                    }

                    using var outputStream = File.OpenWrite(outputFilePath);
                    SKEncodedImageFormat format = targetFormat.ToUpper() switch
                    {
                        "PNG" => SKEncodedImageFormat.Png,
                        "JPEG" => SKEncodedImageFormat.Jpeg,
                        "BMP" => SKEncodedImageFormat.Bmp,
                        "ICO" => SKEncodedImageFormat.Png, // ICO часто сохраняется как PNG
                        _ => throw new NotSupportedException("Unsupported format selected.")
                    };

                    // Сохранение изображения
                    skBitmap.Encode(outputStream, format, 100);
                    StatusTextBlock.Text = $"Status: Conversion successful. Saved at {outputFilePath}";
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = $"Status: Error during conversion. {ex.Message}";
                }
            }
            else
            {
                StatusTextBlock.Text = "Status: Please select a target format.";
            }
        }
    }
}
