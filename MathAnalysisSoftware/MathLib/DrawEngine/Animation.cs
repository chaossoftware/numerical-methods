using System;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MathLib.DrawEngine
{
    public class Animation
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private GifBitmapEncoder GifEncoder;


        public Animation()
        {
            GifEncoder = new GifBitmapEncoder();
        }


        public void AddFrame(Bitmap frame)
        {
            IntPtr hBitmap = frame.GetHbitmap();

            BitmapSource bitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            GifEncoder.Frames.Add(BitmapFrame.Create(bitmap));

            DeleteObject(hBitmap);
        }



        public void SaveAnimation(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                GifEncoder.Save(fs);
            }
        }
    }
}
