﻿using Orchard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Common.Utils
{
    public interface IimageHelper : IDependency
    {
        Bitmap CreateBackground(int width, int height, Color newColor);
        Bitmap ApplyBackground(Bitmap image, Bitmap background, int width = 0, int height = 0);
        Bitmap ApplyDesign(Bitmap image, Bitmap design, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight, int width = 0, int height = 0);
        Bitmap ApplyDesignNoTransparent(Bitmap image, Bitmap design, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight, int width = 0, int height = 0);     
        Bitmap Base64ToBitmap(string base64String);
        Bitmap ResizeImage(Image image, int width, int height);
        ImageCodecInfo GetEncoderInfo(String mimeType);
        void CreateSocialImg(string destForder, CampaignRecord campaign, Bitmap imgPath, String campaignData);

    }
}
