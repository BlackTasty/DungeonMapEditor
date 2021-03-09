using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.Core
{
    static class Helper
    {
        public static double SnapPosition(double value, double snapValue)
        {
            double difference = value % snapValue;
            if (difference < 2.49)
            {
                return value - difference;
            }
            else
            {
                return value + (snapValue - difference);
            }
        }

        public static BitmapImage FileToBitmapImage(string path, Size? desiredSize = null)
        {
            if (File.Exists(path))
            {
                BitmapImage bitmap;
                if (desiredSize == null)
                {
                    bitmap = BitmapToBitmapImage(new Bitmap(path));
                }
                else
                {
                    using (Bitmap bmp = new Bitmap(path))
                    {
                        int diffWidth = bmp.Width - desiredSize.Value.Width;
                        int diffHeight = bmp.Height - desiredSize.Value.Height;

                        if (diffHeight > 0 && diffWidth > 0)
                        {
                            bitmap = BitmapToBitmapImage(new Bitmap(bmp, new Size(bmp.Width - diffWidth, bmp.Height - diffHeight)));
                        }
                        else
                            bitmap = BitmapToBitmapImage(bmp);
                    }
                }
                bitmap?.Freeze();
                return bitmap;
            }
            else
                return null;
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png); // Was .Bmp, but this did not show a transparent background.

                    stream.Position = 0;
                    BitmapImage result = new BitmapImage();
                    result.BeginInit();
                    // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                    // Force the bitmap to load right now so we can dispose the stream.
                    result.CacheOption = BitmapCacheOption.OnLoad;
                    //result.CacheOption = forceLoad ? BitmapCacheOption.OnLoad : BitmapCacheOption.OnDemand;
                    result.StreamSource = stream;
                    result.EndInit();
                    result.Freeze();
                    return result;
                }
            }
            else
                return null;
        }
        public static BitmapImage ExportToPng(string path, Canvas surface)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            System.Windows.Size size = new System.Windows.Size(surface.Width, surface.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new System.Windows.Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(path, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }

            // Restore previously saved layout
            surface.LayoutTransform = transform;

            return FileToBitmapImage(path);
        }
    }
}
