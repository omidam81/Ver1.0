using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.WizardSettings.Services
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
    }
}