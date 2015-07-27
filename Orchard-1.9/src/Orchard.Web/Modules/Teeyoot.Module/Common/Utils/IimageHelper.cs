using Orchard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teeyoot.Module.Common.Utils
{
    public interface IimageHelper : IDependency
    {
        Bitmap CreateBackground(int width, int height, Color newColor);
        Bitmap ApplyBackground(Bitmap image, Bitmap background);
        Bitmap ApplyDesign(Bitmap image, Bitmap design, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight);
        Bitmap Base64ToBitmap(string base64String);
    }
}
