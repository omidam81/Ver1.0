using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Common.Utils
{
    public class ImageHelper : IimageHelper
    {
        public Bitmap CreateBackground(int width, int height, Color newColor)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(newColor);
            }

            return bmp;
        }

        public Bitmap ApplyBackground(Bitmap image, Bitmap background, int width = 0, int height = 0)
        {
            if (width == 0)
                width = image.Width;
            if (height == 0)
                height = image.Height;
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(background, new Rectangle(0, 0, width, height));
                g.DrawImage(image, new Rectangle(0, 0, width, height));
            }

            bmp.MakeTransparent(Color.White);
            return bmp;
        }

        public Bitmap ApplyDesign(Bitmap image, Bitmap design, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight, int width = 0, int height = 0)
        {
            if (width == 0)
                width = image.Width;
            if (height == 0)
                height = image.Height;
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(image, new Rectangle(0, 0, width, height));
                g.DrawImage(design, new Rectangle(printableAreaLeft, printableAreaTop, printableAreaWidth, printableAreaHeight));
            }

            bmp.MakeTransparent(Color.White);
            return bmp;
        }

        public Bitmap Base64ToBitmap(string base64String)
        {
            var array = new[] { @"data:image/png;base64,", @"data:image/jpeg;base64,", @"data:image/gif;base64", @"data:image/pjpeg;base64", @"data:image/svg+xml;base64", @"data:image/tiff;base64", @"data:image/vnd.microsoft.icon;base64", @"data:image/vnd.wap.wbmp;base64" };

            foreach (var str in array)
            {
                if (base64String.StartsWith(str))
                {
                    base64String = base64String.Replace(str, "").Trim();
                    break;
                }
            }

            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes))
            {
                ms.Position = 0;
                return new Bitmap(ms);
            }
        }
    }
}