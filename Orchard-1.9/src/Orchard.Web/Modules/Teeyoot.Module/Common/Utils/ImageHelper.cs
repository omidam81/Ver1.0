using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using System.Web.Mvc;

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

        public Bitmap ApplyDesignNoTransparent(Bitmap image, Bitmap design, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight, int width = 0, int height = 0)
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

        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public void CreateSocialImg(string destForder, CampaignRecord campaign, Bitmap imgPath, String campaignData)
        {
            var p = campaign.Products[0];

            var imageFolder = System.Web.Hosting.HostingEnvironment.MapPath("/Modules/Teeyoot.Module/Content/images/");
            var rgba = ColorTranslator.FromHtml(p.ProductColorRecord.Value);

            var campaignImgTemplate = new Bitmap(imgPath);

            var campaignImg = BuildProductImage(campaignImgTemplate, Base64ToBitmap(campaignData), rgba, p.ProductRecord.ProductImageRecord.Width, p.ProductRecord.ProductImageRecord.Height,
            p.ProductRecord.ProductImageRecord.PrintableFrontTop, p.ProductRecord.ProductImageRecord.PrintableFrontLeft,
            p.ProductRecord.ProductImageRecord.PrintableFrontWidth, p.ProductRecord.ProductImageRecord.PrintableFrontHeight);

            Image backImage = Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("/Media/Default/images/background.png"));
            backImage = ResizeImage(backImage, 1200, 627);
            Graphics g = Graphics.FromImage(backImage);
            g.DrawImage(campaignImg, 150, 0, 900, 900);

            ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
            Encoder encoder = Encoder.Quality;
            EncoderParameter encoderParameter = new EncoderParameter(encoder, 75L);
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = encoderParameter;

            Bitmap socialImg = new Bitmap(backImage);
            socialImg.Save(Path.Combine(destForder, "campaign.jpg"), imageCodecInfo, encoderParameters);

            g.Dispose();
            campaignImgTemplate.Dispose();
            campaignImg.Dispose();
            socialImg.Dispose();
            backImage.Dispose();
        }

        private Bitmap BuildProductImage(Bitmap image, Bitmap design, Color color, int width, int height, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight)
        {
            var background = CreateBackground(width, height, color);
            image = ApplyBackground(image, background, width, height);

            return ApplyDesignNoTransparent(image, design, printableAreaTop, printableAreaLeft, printableAreaWidth, printableAreaHeight, width, height);
        }

    }
}