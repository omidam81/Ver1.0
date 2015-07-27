using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }

    
}
