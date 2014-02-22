using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing;
using ZXing.Common;

namespace QRAsYouType
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BarcodeWriter barcodeWriter;
        private BitmapImage errorImage;
        private const int MinimumSideLength = 300;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonGenerateClicked(object sender, RoutedEventArgs e)
        {
            this.GenerateQRCode();
        }

        private void GenerateQRCode(int width, int height)
        {
            string data = this.inputText.Text;
            if (string.IsNullOrEmpty(data))
            {
                data = " ";
            }
            var ms = new MemoryStream();

            var imageSource = new BitmapImage();
            try
            {
                if (barcodeWriter == null || barcodeWriter.Options.Width != width || barcodeWriter.Options.Height != height)
                {
                    barcodeWriter = new BarcodeWriter()
                        {
                            Format = BarcodeFormat.QR_CODE,
                            Options = new EncodingOptions() { Height = height, Width = width }
                        };
                }
                var bitmap = barcodeWriter.Write(data);
                bitmap.Save(ms, ImageFormat.Bmp);
                ms.Position = 0;
                imageSource.BeginInit();
                imageSource.StreamSource = ms;
                imageSource.EndInit();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Caught exception " + e);
                imageSource = GetErrorImage();
            }


            this.qrCodeImage.Source = imageSource;
        }

        private BitmapImage GetErrorImage()
        {
            if (errorImage == null)
            {
                var uri = new Uri("pack://application:,,,/QRAsYouType;component/error.bmp", UriKind.Absolute);
                errorImage = new BitmapImage(uri);
            }

            return errorImage;
        }

        private void GenerateQRCode()
        {

            var height = (int)qrCodeImage.ActualWidth;
            var width = (int)qrCodeImage.ActualHeight;
            if (width < MinimumSideLength || height < MinimumSideLength)
            {
                width = height = MinimumSideLength;
            }

            GenerateQRCode(width, height);
        }

        private void inputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.GenerateQRCode();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                inputText.Focus();
            }
            catch
            {

            }
        }
    }
}
