using Orchard.ContentManagement.Drivers;
using Teeyoot.Module.Models;

namespace Teeyoot.Account.Drivers
{
    public class TeeyootUserPartDriver : ContentPartDriver<TeeyootUserPart>
    {
        /*
        protected override DriverResult Editor(TeeyootUserPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
         */
    }
}