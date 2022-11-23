using Microsoft.Win32;
using MiUpperMachine.Extensions;
using MiUpperMachine.MiImage;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MiUpperMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageData? imageData;
        private readonly ImageHandler handler = new();

        // 图片相对Image控件大小的缩放比例 Image控件大小/图片大小
        private double imageScale;

        public MainWindow()
        {
            InitializeComponent();

            string? dllPath = ConfigManager.globalConfig.ImageDLLPath;
            string? entryMethodName = ConfigManager.globalConfig.EntryMethodName;
            if (dllPath != null && entryMethodName != null)
            {
                ttbDLLName.Text = dllPath;
                ttbEntryMethodName.Text = entryMethodName;
                handler.Init(dllPath, entryMethodName);
            }
        }

        private string? OpenFileDialog()
        {
            string? initialDirectory = ConfigManager.globalConfig.LastOpenDirectory;
            if (string.IsNullOrEmpty(initialDirectory))
                initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = initialDirectory,
                Filter = "BMP图片|*.bmp"
            };

            bool? isOpenSuccess = openFileDialog.ShowDialog();
            if (isOpenSuccess != null && isOpenSuccess == true)
            {
                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    ConfigManager.globalConfig.LastOpenDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                    return openFileDialog.FileName;
                }
            }

            return null;
        }

        private void OnMenuButtonOpenClick(object sender, RoutedEventArgs e)
        {
            string? fileName = OpenFileDialog();
            if (fileName == null) return;

            imageData = new ImageData(new Bitmap(fileName), fileName);
            mainImage.Source = new BitmapImage(new Uri(fileName, UriKind.Absolute));

            int scaleAxis = 0;
            if (imageData.ImageHeight > imageData.ImageWidth) scaleAxis = 1;

            if (scaleAxis == 0)
            {
                imageScale = mainImage.Width / imageData.ImageWidth;
            }
            else
            {
                imageScale = mainImage.Height / imageData.ImageHeight;
            }
        }

        private void OnMenuButtonToGrayClick(object sender, RoutedEventArgs e)
        {
            if (imageData != null)
            {
                mainImage.Source = imageData.GrayBitmap.ToBitmapImage();
            }
        }

        private void OnMenuButtonToBinClick(object sender, RoutedEventArgs e)
        {
            if (imageData != null)
            {
                mainImage.Source = imageData.BinBitmap.ToBitmapImage();
            }
        }

        private void OnMenuButtonUserCustomClick(object sender, RoutedEventArgs e)
        {
            if (imageData != null)
            {
                try
                {
                    handler.Init(ttbDLLName.Text, ttbEntryMethodName.Text);
                    Bitmap? result = handler.Handle(imageData);
                    if (result != null)
                    {
                        mainImage.Source = result.ToBitmapImage();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OnMainImageMouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point mousePosition = e.GetPosition(mainImage);
            mousePosition.X = Math.Floor(mousePosition.X / imageScale); // 莫名其妙多个0.5
            mousePosition.Y = Math.Floor(mousePosition.Y / imageScale);
            statusItemMousePosition.ContentStringFormat = "位置:({0})";
            statusItemMousePosition.Content = mousePosition.ToString();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConfigManager.globalConfig.ImageDLLPath = ttbDLLName.Text;
            ConfigManager.globalConfig.EntryMethodName = ttbEntryMethodName.Text;
            ConfigManager.SaveGlobalConfig();
        }
    }
}
