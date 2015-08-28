using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teeyoot.Module.ViewModels
{
    public class LaunchCampaignData
    {
        // come from front-end now
        public int ProductCountGoal { get; set; }
        public string CampaignTitle { get; set; }
        public string Description { get; set; }
        public int CampaignLength { get; set; }
        public string Alias { get; set; }
        public bool BackSideByDefault { get; set; }
        public bool IsForCharity { get; set; }
        public string CampaignProfit { get; set; }
        public int ProductMinimumGoal { get; set; }

        // needed as well
        [AllowHtml]
        public string Design { get; set; }
        public string[] Tags { get; set; }
        public CampaignProductData[] Products { get; set; }
    }

    public class CampaignProductData
    {
        public int CurrencyId { get; set; }

        public int ProductId { get; set; }

        public int ColorId { get; set; }

        public int SecondColorId { get; set; }

        public int ThirdColorId { get; set; }

        public int FourthColorId { get; set; }

        public int FifthColorId { get; set; }

        public string BaseCost { get; set; }

        public string Price { get; set; }
    }

    public class DesignInfo
    {
        public string Front { get; set; }

        public string Back { get; set; }
    }

    public class ImgInfo
    {
        public string Type { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        public string Top { get; set; }

        public string Left { get; set; }

        public string zIndex { get; set; }

        public string Svg { get; set; }

        public int Rotate { get; set; }

        public int ChangeColor { get; set; }

        public string FileStandart { get; set; }

        public FileInf FileLoaded { get; set; }

        public string Url { get; set; }

        public string Text { get; set; }

        public string Color { get; set; }

        public string FontFamily { get; set; }

         public string OutlineC { get; set; }

        public string OutlineW { get; set; }
    }

    public class FileInf
    {
        public string Type { get; set; }
    }
}