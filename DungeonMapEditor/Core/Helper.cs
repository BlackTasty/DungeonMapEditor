using DungeonMapEditor.Core.Enum;
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
using System.Windows.Navigation;

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

        public static BitmapImage BitmapSourceToBitmapImage(BitmapSource bitmapSource)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            BitmapImage bitmapImage = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            memoryStream.Close();

            return bitmapImage;
        }

        public static BitmapSource CaptureCanvas(Canvas target, double dpiX, double dpiY, SolidColorBrush background = null)
        {
            if (target == null)
            {
                return null;
            }

            // Save current canvas transform
            Transform transform = target.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            target.LayoutTransform = null;

            if (background != null)
            {
                Canvas backgroundSurface = new Canvas()
                {
                    Width = target.Width,
                    Height = target.Height,
                    Background = background
                };
                backgroundSurface.Children.Add(target);
                target = backgroundSurface;
            }

            System.Windows.Size size = new System.Windows.Size(target.Width, target.Height);

            target.Measure(size);
            target.Arrange(new System.Windows.Rect(size));

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(target.Width * dpiX / 96.0),
                                                            (int)(target.Height * dpiY / 96.0),
                                                            dpiX,
                                                            dpiY,
                                                            PixelFormats.Pbgra32);

            DrawingVisual drawing = new DrawingVisual();
            using (DrawingContext drawingContext = drawing.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(target);
                drawingContext.DrawRectangle(brush, null, new System.Windows.Rect(new System.Windows.Point(), 
                                             size));
            }

            rtb.Render(drawing);

            target.LayoutTransform = transform;
            return rtb;
        }

        public static string SaveBitmapToFile(string path, BitmapSource bitmap)
        {
            int retries = 0;
            do
            {
                if (SaveBitmapInternal(path, bitmap))
                {
                    return path;
                }

                retries++;
            } while (retries < 3);

            return null;
        }

        private static bool SaveBitmapInternal(string path, BitmapSource bitmap)
        {
            try
            {
                using (FileStream outStream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    // Use png encoder for our data
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    // push the rendered bitmap to it
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    // save the data to the stream
                    encoder.Save(outStream);
                }
                return true;
            }
            catch
            {
                System.Threading.Thread.Sleep(200);
                return false;
            }
        }

        public static BitmapImage ExportToPng(string path, Canvas surface, SolidColorBrush background = null)
        {
            BitmapSource bitmap = CaptureCanvas(surface, 96, 96, background);
            path = SaveBitmapToFile(path, bitmap);
            return FileToBitmapImage(path);

            /*if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            if (background != null)
            {
                Canvas backgroundSurface = new Canvas()
                {
                    Width = surface.Width,
                    Height = surface.Height,
                    Background = background
                };
                backgroundSurface.Children.Add(surface);
                surface = backgroundSurface;
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
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            SaveBitmapToFile(path, renderBitmap);

            // Restore previously saved layout
            surface.LayoutTransform = transform;

            return FileToBitmapImage(path);*/
        }

        public static BitmapImage ExportToPng_Old(string path, Canvas surface, SolidColorBrush background = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            if (background != null)
            {
                Canvas backgroundSurface = new Canvas()
                {
                    Width = surface.Width,
                    Height = surface.Height,
                    Background = background
                };
                backgroundSurface.Children.Add(surface);
                surface = backgroundSurface;
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
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            SaveBitmapToFile(path, renderBitmap);

            // Restore previously saved layout
            surface.LayoutTransform = transform;

            return FileToBitmapImage(path);
        }

        public static double GetUpdatedAxisLocation(double currentAxisPosition, double clickAxisPosition, 
            double maxAxisPosition, double controlAxisSize)
        {
            double axisLocation = currentAxisPosition - clickAxisPosition;
            axisLocation = Math.Max(axisLocation, 0);
            if (maxAxisPosition > -1)
            {
                axisLocation = Math.Min(axisLocation, maxAxisPosition - controlAxisSize);
            }
            return axisLocation;
        }

        public static double GetUpdatedAxisLocation_Snap(double currentAxisPosition, double clickAxisPosition, 
            double maxAxisPosition, double controlAxisSize, double snapPosition)
        {
            return GetSnappedAxisLocation(currentAxisPosition - clickAxisPosition, maxAxisPosition, controlAxisSize, snapPosition);
        }

        public static double GetSnappedAxisLocation(double targetAxisPosition, double maxAxisPosition, double controlAxisSize, double snapPosition)
        {
            double axisLocation = (Math.Round(targetAxisPosition / snapPosition)) * snapPosition;

            axisLocation = Math.Max(axisLocation, 0);
            if (maxAxisPosition > -1)
            {
                axisLocation = Math.Min(axisLocation, maxAxisPosition - controlAxisSize);
            }

            double leftoverAxis = axisLocation % snapPosition;
            
            if (leftoverAxis == 0)
            {
                return axisLocation;
            }
            else
            {
                return axisLocation - leftoverAxis;
            }
        }

        public static System.Windows.Size ChangeSize_KeepAspectRatio(System.Windows.Size currentSize, System.Windows.Size desiredSize)
        {
            bool adjustingHeight = currentSize.Height != desiredSize.Height;

            if ((adjustingHeight && currentSize.Height == 0) || (!adjustingHeight && currentSize.Width == 0))
            {
                return desiredSize;
            }

            double scaling = adjustingHeight ? desiredSize.Height / currentSize.Height : desiredSize.Width / currentSize.Width;

            if (!adjustingHeight)
            {
                return new System.Windows.Size(desiredSize.Width, desiredSize.Height * scaling);
            }
            else
            {
                return new System.Windows.Size(desiredSize.Width * scaling, desiredSize.Height);
            }
        }

        public static System.Windows.Size GetDocumentSize(DocumentSizeType documentSize)
        {
            return GetDocumentSize(documentSize, new System.Windows.Size());
        }

        public static System.Windows.Size GetDocumentSize(System.Windows.Size customSize)
        {
            return GetDocumentSize(DocumentSizeType.Custom, customSize);
        }

        private static System.Windows.Size GetDocumentSize(DocumentSizeType documentSize, System.Windows.Size customSize)
        {
            switch (documentSize)
            {
                case DocumentSizeType.Document_A3:
                    return new System.Windows.Size(4961, 3508);
                case DocumentSizeType.Document_A4:
                    return new System.Windows.Size(3508, 2480);
                case DocumentSizeType.Document_A5:
                    return new System.Windows.Size(2480, 1748);

                case DocumentSizeType.Image_HD:
                    return new System.Windows.Size(1280, 720);
                case DocumentSizeType.Image_FullHD:
                    return new System.Windows.Size(1920, 1080);
                case DocumentSizeType.Image_2K:
                    return new System.Windows.Size(2560, 1440);
                case DocumentSizeType.Image_4K:
                    return new System.Windows.Size(3840, 2160);

                case DocumentSizeType.Custom:
                    return customSize;
                default:
                    return new System.Windows.Size();
            }
        }
    }
}
