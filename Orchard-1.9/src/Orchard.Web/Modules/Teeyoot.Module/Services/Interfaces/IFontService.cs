using System.Linq;
using Orchard;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public interface IFontService : IDependency
    {
        IQueryable<FontRecord> GetAllfonts();

        void DeleteFont(int id);

        void EditFont(FontRecord font);

        void AddFont(FontRecord font);

        FontRecord GetFont(int id);

        FontRecord GetFontByFamily(string fontFamily);
    }

    
}
