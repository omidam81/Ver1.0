using System.Linq;
using Orchard.Data;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class FontService : IFontService
    {
        private readonly IRepository<FontRecord> _fontRepository;

        public FontService(IRepository<FontRecord> fontRepository)
        {
            _fontRepository = fontRepository;
        }

        public IQueryable<FontRecord> GetAllfonts()
        {
            return _fontRepository.Table;
        }

        public void DeleteFont(int id)
        {
            _fontRepository.Delete(_fontRepository.Get(id));
        }

        public void EditFont(FontRecord font)
        {
            FontRecord record = _fontRepository.Get(f => f.Id == font.Id);
            record.Family = font.Family;
            //record.FileName = font.FileName;
            record.Priority = font.Priority;
            record.Tags = font.Tags;
            _fontRepository.Update(record);
            
        }

        public void AddFont(FontRecord font)
        {
            _fontRepository.Create(font);

        }


        public FontRecord GetFont(int id)
        {
            return _fontRepository.Get(f => f.Id == id);
        }

        public FontRecord GetFontByFamily(string fontFamily)
        {
            return _fontRepository.Get(f => f.Family == fontFamily);
        }
    }
}