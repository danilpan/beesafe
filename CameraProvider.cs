using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;

namespace BeeSafe
{
    class CameraProvider
    {
        private VideoCapture capture;
        public CameraProvider()
        {
            if (this.capture != null)
            {
                this.capture.Dispose();
            }

            this.capture = new VideoCapture(0);
            this.capture.Open(0);
        }

        public Bitmap getImage()
        {
            capture.RetrieveMat();
            return BitmapConverter.ToBitmap(capture.RetrieveMat());
        }
    }
}
