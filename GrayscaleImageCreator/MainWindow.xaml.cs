using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GrayscaleImageCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Microsoft.VisualStudio.PlatformUI.GrayscaleImageConverter _Converter;

        public MainWindow()
        {
            InitializeComponent();
            _Converter = new Microsoft.VisualStudio.PlatformUI.GrayscaleImageConverter();
        }

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
            open.Filter = "Image|*.png";
            open.Multiselect = true;
            var result = open.ShowDialog(this);
            if (result != null && result.HasValue && result.Value)
            {
                foreach (var item in open.FileNames)
                {
                    ConvertImage(item);
                }
            }
        }

        private void ConvertImage(string filename)
        {
            BitmapSource img = new BitmapImage(new Uri(filename, UriKind.Absolute));
            Image image = _Converter.Convert(img, typeof(Image), Color.FromArgb(160, 255, 255, 255), System.Globalization.CultureInfo.CurrentCulture) as Image;
            if (image != null)
            {
                string dir = System.IO.Path.GetDirectoryName(filename);
                string name = System.IO.Path.GetFileNameWithoutExtension(filename);
                string ext = System.IO.Path.GetExtension(filename);

                string dir2 = System.IO.Path.Combine(dir, "Converted");
                if (!System.IO.Directory.Exists(dir2)) System.IO.Directory.CreateDirectory(dir2);
                string destination = System.IO.Path.Combine(dir2, name + _txtPostfix.Text + ext);
                if (System.IO.File.Exists(destination)) System.IO.File.Delete(destination);
                using (var fileStream = new System.IO.FileStream(destination, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image.Source as BitmapSource));
                    encoder.Save(fileStream);
                }
            }
        }
    }
}
