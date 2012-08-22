using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Skill.Fbx;

namespace Skill.Studio.Tools.HToMesh
{
    public enum RawPixelFormat
    {
        Bit8 = 1,
        Bit16 = 2,
    }

    public class RawFile : INotifyPropertyChanged
    {
        public const double DPI = 72;

        private BitmapSource _Image;
        public BitmapSource Image
        {
            get { return _Image; }
            private set
            {
                if (_Image != value)
                {
                    _Image = value;
                    OnPropertyChanged("Image");
                }
            }
        }

        public double[] HeightsDouble { get; private set; }
        public ushort[] Heights { get; private set; }
        public ObservableCollection<Patch> Patches { get; private set; }
        public string FileName { get; private set; }
        public RawPixelFormat PixelFormat { get; private set; }
        public Dimension Size { get; private set; }
        public Dimension SplitSize { get; private set; }

        public RawFile()
        {
            Patches = new ObservableCollection<Patch>();
        }

        public void Open(string fileName, RawPixelFormat pixelFormat)
        {
            if (File.Exists(fileName))
            {
                this.FileName = fileName;
                this.PixelFormat = pixelFormat;

                byte[] data = File.ReadAllBytes(FileName);

                int pf = (int)PixelFormat;
                int dataLength = data.Length / pf;

                Heights = new ushort[dataLength];
                HeightsDouble = new double[dataLength];

                if (pf == 1)
                {
                    int m = ushort.MaxValue / byte.MaxValue;
                    double md = ((double)ushort.MaxValue) / byte.MaxValue;
                    for (int i = 0; i < data.Length; i += pf)
                    {
                        Heights[i] = Convert.ToUInt16(data[i] * m);
                        HeightsDouble[i] = data[i] * md;
                    }
                }
                else if (pf == 2)
                {
                    for (int i = 0; i < data.Length; i += pf)
                    {
                        HeightsDouble[i / pf] = Heights[i / pf] = BitConverter.ToUInt16(data, i);                        
                    }
                }

                int size = (int)Math.Sqrt(Heights.Length);
                if (Heights.Length == size * size)
                {
                    TryCreateImage(new Dimension() { Width = size, Height = size });
                }
            }
        }

        public void TryCreateImage(Dimension size)
        {
            if (size.Width * size.Height == Heights.Length)
            {
                Image = BitmapImage.Create(size.Width, size.Height, DPI, DPI, PixelFormats.Gray16, null, Heights, size.Width * 2);
                Size = size;
            }
            else
                Image = null;
        }


        public int[] GetAvailableSplitSize()
        {
            List<int> splits = new List<int>();

            for (int i = 16; i < Size.Width; i *= 2)
            {
                splits.Add(i);
            }
            return splits.ToArray();
        }

        private ushort[] GetImageData(int indexI, int indexJ, Dimension patchSize)
        {
            ushort[] result = new ushort[patchSize.Width * patchSize.Height];

            for (int i = 0; i < patchSize.Height; i++)
            {
                int dataOffset = ((indexI * patchSize.Height + i) * this.Size.Width) + (indexJ * patchSize.Width);
                int resultOffset = i * patchSize.Width;
                for (int j = 0; j < patchSize.Width; j++)
                {
                    int jj = dataOffset + j;
                    if (jj >= Heights.Length || resultOffset + j >= resultOffset + patchSize.Width)
                        continue;
                    result[resultOffset + j] = Heights[jj];
                }
            }

            return result;
        }


        public void Split(Dimension splitSize)
        {
            Patches.Clear();
            this.SplitSize = splitSize;
            int rowCount = this.Size.Height / this.SplitSize.Height;
            int columnCount = this.Size.Width / this.SplitSize.Width;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    Patch p = new Patch();
                    p.Row = i;
                    p.Column = j;
                    p.Selected = true;
                    Patches.Add(p);
                }
            }
        }


        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public bool IsPowerOfTwo(int length)
        {
            if (length % 2 != 0) return false;

            while (length > 1)
            {
                if (length == 2) return true;
                length = length / 2;
            }
            return false;
        }
    }
}
