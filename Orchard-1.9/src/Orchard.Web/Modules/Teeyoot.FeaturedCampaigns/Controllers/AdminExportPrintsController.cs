using Ionic.Zip;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using Teeyoot.FeaturedCampaigns.ViewModels;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.FeaturedCampaigns.Controllers
{
    [Admin]
    public class AdminExportPrintsController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ISiteService _siteService;
        private readonly IimageHelper _imageHelper;
        private readonly IOrderService _orderService;
        private readonly IFontService _fontService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public AdminExportPrintsController(ICampaignService campaignService, ISiteService siteService, IShapeFactory shapeFactory, IOrderService orderService, IimageHelper imageHelper, IFontService fontService, IWorkContextAccessor workContextAccessor)
        {
            _campaignService = campaignService;
            _siteService = siteService;
            _imageHelper = imageHelper;
            _orderService = orderService;
            _fontService = fontService;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            _workContextAccessor = workContextAccessor;
        }

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var total =  _campaignService.GetAllCampaigns().Count();

            var campaigns =  _campaignService.GetAllCampaigns().Select(c => new { 
                                                    Id = c.Id,
                                                    Title = c.Title,
                                                    Sold = c.ProductCountSold,
                                                    Goal = c.ProductCountGoal,
                                                    Status = c.CampaignStatusRecord,
                                                    Alias = c.Alias,
                                                    CreatedDate = c.StartDate,
                                                    IsApproved = c.IsApproved,
                                                    IsActive = c.IsActive,
                                                    Minimum = c.ProductMinimumGoal
                                                })                               
                                .ToList()
                                .OrderBy(e => e.Status.Id);
            var totalNotApproved = _campaignService.GetAllCampaigns().Where(c => c.IsApproved == false && c.Rejected == false).Count();

            var yesterday = DateTime.UtcNow.AddDays(-1);
            var last24hoursOrders = _orderService.GetAllOrders().Where(o => o.IsActive && o.Created >= yesterday && o.OrderStatusRecord.Name != OrderStatus.Cancelled.ToString() && o.OrderStatusRecord.Name != OrderStatus.Unapproved.ToString());

            var entriesProjection = campaigns.Select(e =>
            {
                return Shape.campaign(
                    Id: e.Id,
                    Title: e.Title,
                    Sold: e.Sold,
                    Goal: e.Goal,
                    Status: e.Status,
                    Alias: e.Alias,
                    CreatedDate: e.CreatedDate.ToLocalTime().ToString("dd/MM/yyyy"),
                    IsApproved: e.IsApproved,
                    IsActive : e.IsActive,
                    Minimum: e.Minimum,
                    Last24HoursSold : last24hoursOrders
                                        .SelectMany(o => o.Products)
                                        .Where(p => p.CampaignProductRecord.CampaignRecord_Id == e.Id)
                                        .Sum(p => (int?)p.Count) ?? 0
                    );
            });

            return View(new ExportPrintsViewModel { Campaigns = entriesProjection.ToArray(), NotApprovedTotal = totalNotApproved });
        }

        public void ExportPrints(PagerParameters pagerParameters, int id)
        {
            var campaign = _campaignService.GetCampaignById(id);
            var p = campaign.Products.Where(pr => pr.WhenDeleted == null).First();

            var jss = new JavaScriptSerializer();
            jss.MaxJsonLength = int.MaxValue;
            var dict = jss.Deserialize<dynamic>(campaign.Design);
            string vectors = dict["vectors"];
            var vectorInfo = jss.Deserialize<dynamic>(vectors);

            Dictionary<string, dynamic> fronts = vectorInfo["front"];
            var listFront = new List<ImgInfo>();
            listFront = GetFileInfo(vectors, fronts, "front");
            string svgFront = "";

            if (listFront.Count > 0)
            {
                IEnumerable<ImgInfo> listFrontSort = listFront.OrderBy(l => l.zIndex);
                svgFront = CreateSVG(p.ProductRecord.ProductImageRecord.PrintableFrontHeight, p.ProductRecord.ProductImageRecord.PrintableFrontWidth, listFrontSort);
            }

            Dictionary<string, dynamic> backs = vectorInfo["back"];
            var listBack = new List<ImgInfo>();
            listBack = GetFileInfo(vectors, backs, "back");
            string svgBack = "";

            if (listBack.Count > 0)
            {
                IEnumerable<ImgInfo> listBackSort = listBack.OrderBy(l => l.zIndex);
                svgBack = CreateSVG(p.ProductRecord.ProductImageRecord.PrintableBackHeight, p.ProductRecord.ProductImageRecord.PrintableBackWidth, listBackSort);
            }

            var destFolder = Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), campaign.Products.Where(pr=>pr.WhenDeleted == null).First().Id.ToString()) + "\\normal";

                using (var archive = new ZipFile())
                {
                    if (Directory.Exists(destFolder))
                    {
                        var dir = new DirectoryInfo(destFolder);
                        FileInfo[]  images;
                        images = dir.GetFiles();
                        if (images != null)
                        {
                            for (int i = 0; i < images.Length; i++)
                            {
                                Image image = Image.FromFile(images[i].FullName);
                                ImageConverter _imageConverter = new ImageConverter();
                                byte[] imgByte = (byte[])_imageConverter.ConvertTo(image, typeof(byte[]));
                                archive.AddEntry(images[i].Name, imgByte);                          
                            }
                        }                      
                    }

                    byte[] byteArrayFront = Encoding.UTF8.GetBytes(svgFront);
                    MemoryStream streamFront = new MemoryStream(byteArrayFront);

                    if (svgFront != "")
                    {
                        var fileFront = archive.AddEntry("front.svg", streamFront);
                    }

                    byte[] byteArrayBack = Encoding.UTF8.GetBytes(svgBack);
                    MemoryStream streamBack = new MemoryStream(byteArrayBack);
                    if (svgBack != "")
                    {                      
                        var fileBack = archive.AddEntry("back.svg", streamBack);
                    }
                   
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.ContentType = "application/zip";
                    Response.AppendHeader("content-disposition", "attachment; filename=campaign_" + id + "_" + campaign.Alias + "_" + "_prints.zip");

                    archive.Save(Response.OutputStream);

                    streamFront.Close();
                    streamBack.Close();
                }                                
        }

        private string CreateSVG(int printableHeight, int printableWidth, IEnumerable<ImgInfo> listSort)
        {
            string svg = "<svg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' height='" + printableHeight + "' width='" + printableWidth + "'>";
            svg = svg + "<defs><style type='text/css'><![CDATA[";
          
            foreach (var item in listSort)
            {
                if (item.FontFamily != "")
                {  //font-style: normal; font-weight: 400;
                    svg = svg + "@font-face{font-family:'" + item.FontFamily + "';src: local('" + item.FontFamily + "'), local('" + item.FontFamily + "'), url('data:application/font-woff;charset=utf-8;base64,";

                    var fontName = _fontService.GetFontByFamily(item.FontFamily);

                    var imageFilePath = System.Web.Hosting.HostingEnvironment.MapPath("/Modules/Teeyoot.Module/Content/fonts/" + fontName.FileName + "-webfont.woff");

                    if (System.IO.File.Exists(imageFilePath))
                    {
                        Byte[] bytes = System.IO.File.ReadAllBytes(imageFilePath);
                        String file = Convert.ToBase64String(bytes);
                        svg = svg + file;
                    }
                    svg = svg + "') format('woff');}";
                }
            }
            svg = svg + "]]></style></defs>";

            foreach (var item in listSort)
            {
                var top = item.Top.Replace("px", "");
                var left = item.Left.Replace("px", "");

                if (item.FileStandart != "")
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(item.Svg);
                    var root = doc.FirstChild;

                    if (root.Attributes.Count > 0)
                    {
                        if (root.Attributes["x"] != null)
                            root.Attributes.Remove(root.Attributes["x"]);
                        if (root.Attributes["y"] != null)
                            root.Attributes.Remove(root.Attributes["y"]);
                    }
                    item.Svg = doc.OuterXml;
                }

                if (item.Rotate != 0)
                {
                    int width = int.Parse(item.Width.Replace("px", "")) / 2;
                    int height = int.Parse(item.Height.Replace("px", "")) / 2;

                    svg = svg + "<g transform=\"translate(" + left + ", " + top + ")  rotate(" + item.Rotate + " , " + width + " , " + height + ")\">" + item.Svg + "</g>";
                }
                else
                {
                    svg = svg + item.Svg.Replace("<svg ", "<svg y='" + top + "' x='" + left + "' ");
                }
            }

            svg = svg + "</svg>";
            svg = svg.Replace("<?xml version='1.0'?>", "");

            return svg;
        }

        private List<ImgInfo> GetFileInfo(string vectors, Dictionary<string, dynamic> layers , string side)
        {
            var jss = new JavaScriptSerializer();
            jss.MaxJsonLength = int.MaxValue;
            var vectorInfo = jss.Deserialize<dynamic>(vectors);
            var list = new List<ImgInfo>();

            for (int i = 0; i < layers.Count(); i++)
            {
                Dictionary<string, dynamic> layer = vectorInfo[side][i.ToString()];

                var type = "";
                var text = "";
                int changeColor = 0;
                var color = "";
                var fileName = "";
                var fontFamily = "";
                var height = "";
                var left = "";
                var outlineC = "";
                var outlineW = "";
                int rotate = 0;
                var svg = "";
                var top = "";
                var url = "";
                var width = "";
                var zIndex = "";
                FileInf fileInfoLoaded = new FileInf();

                if (layer.ContainsKey("type"))
                {
                    type = layer["type"];
                }
                if (layer.ContainsKey("text"))
                {
                    text = layer["text"];
                }
                if (layer.ContainsKey("change_color"))
                {
                    changeColor = layer["change_color"];
                }
                if (layer.ContainsKey("color"))
                {
                    color = layer["color"];
                }
                if (layer.ContainsKey("file"))
                {
                    try
                    {
                        fileName = layer["file"];
                    }catch
                    {                       
                        fileInfoLoaded.Type = layer["file"]["type"];
                    }
                }
                if (layer.ContainsKey("fontFamily"))
                {
                    fontFamily = layer["fontFamily"];
                }
                if (layer.ContainsKey("height"))
                {
                    height = layer["height"];
                }
                if (layer.ContainsKey("left"))
                {
                    left = layer["left"];
                }
                if (layer.ContainsKey("outlineC"))
                {
                    outlineC = layer["outlineC"];
                }
                if (layer.ContainsKey("outlineW"))
                {
                    outlineW = layer["outlineW"];
                }
                if (layer.ContainsKey("rotate"))
                {
                    rotate = layer["rotate"];
                }
                if (layer.ContainsKey("svg"))
                {
                    svg = layer["svg"];
                }
                if (layer.ContainsKey("top"))
                {
                    top = layer["top"];
                }
                if (layer.ContainsKey("url"))
                {
                    url = layer["url"];
                }
                if (layer.ContainsKey("width"))
                {
                    width = layer["width"];
                }
                if (layer.ContainsKey("zIndex"))
                {
                    zIndex = layer["zIndex"];
                }

                var imgInfo = new ImgInfo
                {
                    Type = type,
                    Text = text,
                    ChangeColor = changeColor,
                    Color = color,
                    FileStandart = fileName,
                    FileLoaded = fileInfoLoaded,
                    FontFamily = fontFamily,
                    Height = height,
                    Left = left,
                    OutlineC = outlineC,
                    OutlineW = outlineW,
                    Rotate = rotate,
                    Svg = svg,
                    Top = top,
                    Url = url,
                    Width = width,
                    zIndex = zIndex
                };

                list.Add(imgInfo);
            }
            return list;
        }

        public ActionResult StartPrinting(PagerParameters pagerParameters, int id)
        {
            _campaignService.SetCampaignStatus(id, CampaignStatus.Paid);
            return RedirectToAction("Index", new { pagerParameters});
        }
	}
}