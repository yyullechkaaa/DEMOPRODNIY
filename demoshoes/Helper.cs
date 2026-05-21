using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace demoshoes
{
    public partial class ShoesProduct
    {
        public bool IsDiscountGreat => Discount > 15;
        public bool IsCountNull => CountStore == 0;

        public string OldPriceText
        {
            get
            {
                if (Discount.HasValue && Discount.Value > 0)
                    return $"{Price}";
                return "";
            }
        }

        public string FinalPriceText
        {
            get
            {
                if (Discount.HasValue && Discount.Value > 0)
                {
                    decimal finalPrice = Price - (Price * Discount.Value / 100);
                    return $"{finalPrice}";
                }
                return $"{Price}";
            }
        }

        public BitmapImage PhotoImage
        {
            get
            {
                if (string.IsNullOrEmpty(Photo))
                {
                    return LoadPhoto("picture.png");
                }
                try
                {
                    string filename = Path.GetFileName(Photo.TrimStart('/', '\\').Replace('/', '\\'));
                    return LoadPhoto(filename) ?? LoadPhoto("picture.png");
                }
                catch
                {
                    return LoadPhoto("picture.png");
                }
            }
        }

        private BitmapImage LoadPhoto(string filename)
        {
            string[] paths =
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", filename),
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..", "..", "images", filename))
            };

            foreach (var path in paths.Where(File.Exists)) {
            try {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.DecodePixelWidth = 120;
                bmp.EndInit();
                return bmp;
                 }
                catch { }
            }
            return null;
        }
    }
}

