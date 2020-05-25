using System;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace YankeeShower
{
    public static class VisualExtensions
    {
        /// <summary>
        /// Returns the contents of a WPF Visual as a Bitmap in PNG format.
        /// </summary>
        /// <param name="visual">A WPF Visual.</param>
        /// <returns>A GDI+ System.Drawing.Bitmap.</returns>
        public static Bitmap PngBitmap(this Visual visual)
        {
            try
            {
                // Get height and width
                int width = (int)(double)visual.GetValue(FrameworkElement.ActualWidthProperty);
                int height = (int)(double)visual.GetValue(FrameworkElement.ActualHeightProperty);

                // Render
                RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
                rtb.Render(visual);

                // Encode
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                encoder.Save(stream);

                // Create Bitmap
                Bitmap bmp = new Bitmap(stream);
                stream.Close();

                return bmp;
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// Returns the contents of a WPF Visual as a BitmapSource, e.g.
        /// for binding to an Image control.
        /// </summary>
        /// <param name="visual">A WPF Visual.</param>
        /// <returns>A set of pixels.</returns>
        public static BitmapSource BitmapSource(this Visual visual)
        {
            try
            {
                Bitmap bmp = visual.PngBitmap();
                IntPtr hBitmap = bmp.GetHbitmap();
                BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
                return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            }
            catch
            {

            }
            return null;
        }

        public static BitmapSource CropBitMap(this Visual visual, int width)
        {
            try
            {        
                BitmapSource rtb = BitmapSource(visual);
            
                if (rtb.Width <= width)
                {
                    return rtb;
                }
                else
                {
                    CroppedBitmap croppedBitmap = new CroppedBitmap(rtb, new Int32Rect(0, 0, width, (int)rtb.Height));
                    return croppedBitmap;
                }
            }
            catch
            {
            }
            return null;

        }
    }
}
