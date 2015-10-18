using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;

namespace Teeyoot.Module.Common.Utils
{
    /// <summary>
    /// Provides methods to help working with image(.png) files
    ///  disc operations (saving to a disc, deleting).
    /// </summary>
    public class ImageFileHelper
    {
        private readonly string _fileNameTemplate;
        private readonly string _virtualRelativePath;
        private readonly Func<HttpServerUtilityBase> _getServerFunc;

        /// <param name="fileNameTemplate">
        /// Example:   "currency_{0}_flag.png"    </param>
        /// <param name="virtualRelativePath">
        /// Example:  "/Modules/Teeyoot.Module/Content/images"  
        /// Put no slash at the ending!!! 
        /// Put no tilde at the beginning!!!
        /// </param>
        /// <param name="getServerFunc">
        /// Lambda is used because Server property isn't avalilable 
        /// in the constructor of a controller, but it's just handy to init it there 
        /// </param>
        public ImageFileHelper(
            string fileNameTemplate,
            string virtualRelativePath,
            Func<HttpServerUtilityBase> getServerFunc)
        {
            _fileNameTemplate = fileNameTemplate;
            _virtualRelativePath = virtualRelativePath;
            _getServerFunc = getServerFunc;
        }

        public string FormImageFileNameById(int id, bool virtualPath)
        {
            var imageFileName = string.Format(_fileNameTemplate, id);

            return virtualPath
                ? _virtualRelativePath + "/" + imageFileName
                : Path.Combine(_getServerFunc().MapPath("~" + _virtualRelativePath), imageFileName);
        }

        public string SaveImageToDisc(HttpPostedFileBase imageFile, int id, out bool isNotPNG)
        {
            isNotPNG = false; // by default
            if (imageFile == null)
            {
                return null;
            }
            if (!IsImagePng(imageFile))
            {
                isNotPNG = true;
                return null;
            }
            using (var image = Image.FromStream(imageFile.InputStream, true, true))
            {
                image.Save(FormImageFileNameById(id, false), ImageFormat.Png);
                return FormImageFileNameById(id, true);
            }
        }

        public void DeleteImageFromDisc(int id)
        {
            var filename = FormImageFileNameById(id, false);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        private static bool IsImagePng(HttpPostedFileBase imageFile)
        {
            var imageHeader = new byte[4];

            imageFile.InputStream.Read(imageHeader, 0, 4);
            var strHeader = Encoding.ASCII.GetString(imageHeader);

            return strHeader.ToLowerInvariant().EndsWith("png");
        }
    }
}