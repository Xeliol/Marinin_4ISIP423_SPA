using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ISIP423_Marinin_SPA.Pages
{
    /// <summary>
    /// Общие операции: загрузка, кадрирование по центру в квадрат, сохранение в images/.
    /// </summary>
    public static class ImageHelper
    {
        public const int DefaultSize = 250;

        public static BitmapImage LoadBitmap(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        /// <summary>Берёт центральный квадрат и масштабирует до targetSize×targetSize.</summary>
        public static BitmapSource CreateSquareBitmap(BitmapSource source, int targetSize = DefaultSize)
        {
            int side = Math.Min(source.PixelWidth, source.PixelHeight);
            int srcX = (source.PixelWidth - side) / 2;
            int srcY = (source.PixelHeight - side) / 2;
            var cropped = new CroppedBitmap(source, new Int32Rect(srcX, srcY, side, side));

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
                dc.DrawImage(cropped, new Rect(0, 0, targetSize, targetSize));

            var rtb = new RenderTargetBitmap(targetSize, targetSize, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            rtb.Freeze();
            return rtb;
        }

        /// <summary>
        /// Загружает файл, кадрирует до квадрата size×size, сохраняет PNG в images/
        /// рядом с exe и возвращает относительный путь "/images/xxx.png".
        /// </summary>
        public static string ProcessAndSaveImage(string sourcePath, int size = DefaultSize)
        {
            var source = LoadBitmap(sourcePath);
            var squared = CreateSquareBitmap(source, size);

            string imagesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
            Directory.CreateDirectory(imagesDir);

            string fileName = Guid.NewGuid().ToString("N") + ".png";
            string destPath = Path.Combine(imagesDir, fileName);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(squared));
            using (var fs = File.Create(destPath))
                encoder.Save(fs);

            return "/images/" + fileName;
        }

        /// <summary>"/images/x.png" -> полный путь на диске (в папке приложения).</summary>
        public static string GetFullPathFromRelative(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return null;
            if (!imagePath.StartsWith("/")) return imagePath; // уже абсолютный
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        }

        /// <summary>Загружает относительный путь и возвращает уже квадратный 250×250 для превью.</summary>
        public static BitmapSource LoadPreviewFromRelative(string imagePath, int size = DefaultSize)
        {
            string fullPath = GetFullPathFromRelative(imagePath);
            if (fullPath == null || !File.Exists(fullPath)) return null;
            try
            {
                var bmp = LoadBitmap(fullPath);
                return CreateSquareBitmap(bmp, size);
            }
            catch { return null; }
        }
    }

    /// <summary>Конвертер для XAML: строка ImagePath -> BitmapImage. Работает с runtime-файлами.</summary>
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            if (string.IsNullOrWhiteSpace(path)) return null;

            string fullPath = ImageHelper.GetFullPathFromRelative(path);
            if (fullPath == null || !File.Exists(fullPath)) return null;

            try { return ImageHelper.LoadBitmap(fullPath); }
            catch { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}